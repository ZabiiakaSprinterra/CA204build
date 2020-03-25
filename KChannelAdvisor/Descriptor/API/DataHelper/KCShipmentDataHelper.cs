
using KChannelAdvisor.Descriptor.API.Entity;
using KChannelAdvisor.Descriptor.API.APIHelper;
using KChannelAdvisor.Descriptor.API.Helper;
using KChannelAdvisor.Descriptor.API.LoggerProvider;
using KChannelAdvisor.Descriptor.API.Mapper;
using KChannelAdvisor.BLC;
using KChannelAdvisor.DAC;
using KChannelAdvisor.Descriptor.Extensions;
using KChannelAdvisor.Descriptor.Logger;
using KChannelAdvisor.ShippingService;
using PX.Data;
using PX.Objects.SO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KChannelAdvisor.Descriptor.API.DataHelper
{
    public class KCShipmentDataHelper
    {
        KCILoggerProvider logger;

        public KCShipmentDataHelper(KCLoggerProperties loggerProperties)
        {
            logger = new KCLoggerProvider(loggerProperties);
        }

        public void ExportShipments(KCStore store)
        {
            if (store.DateTo < store.DateFrom) throw new PXException(KCMessages.DateToBiggerThanDateFrom);
            if (store.DateTo.GetValueOrDefault() != default) store.DateTo = store.DateTo.GetValueOrDefault().AddDays(1);
            bool anyExported = false;
            KCDataExchangeMaint graph = PXGraph.CreateInstance<KCDataExchangeMaint>();
            SOOrderShipmentProcess orderShipmentGraph = PXGraph.CreateInstance<SOOrderShipmentProcess>();

            KCSiteMaster connection = graph.Connection.Select().RowCast<KCSiteMaster>().Where(x => x.SiteMasterCD.Equals(store.SiteMasterCD)).First();
            KCARestClient client = new KCARestClient(connection);
            KCOrderAPIHelper helperOrder = new KCOrderAPIHelper(client);
            KCShipmentAPIHelper helperShipment = new KCShipmentAPIHelper(client);

            List<SOOrder> orders = graph.Orders.Select().RowCast<SOOrder>().Where(x =>
                                                            x.GetExtension<KCSOOrderExt>().UsrKCSiteName?.EndsWith("/Non-FBA") == true).ToList();

            foreach (SOOrder order in orders)
            {
                IEnumerable<SOOrderShipment> orderShipments = KCGeneralDataHelper.GetOrderShipmentsByOrderNbr(graph, order.OrderNbr);
                if (orderShipments == null) continue;

                foreach (SOOrderShipment orderShipment in orderShipments)
                {
                    if (!CheckData(orderShipment, store.DateFrom, store.DateTo)) continue;
                    PXResultset<SOLine> lines = graph.OrderLines.Select(orderShipment.ShipmentNbr);
                    SOShipment shipment = KCGeneralDataHelper.GetShipmentByShipmentNbr(orderShipmentGraph, orderShipment.ShipmentNbr);
                    if (!CheckShippingCarrier(helperShipment, shipment))
                    {
                        logger.Information(KCMessages.ShipViaDoesnotExist(shipment.ShipmentNbr));
                        continue;
                    }
                    KCSOShipmentExt shipmentKCExt = shipment?.GetExtension<KCSOShipmentExt>();
                    SOPackageDetail package = KCGeneralDataHelper.GetPackageByShipmentNbr(orderShipmentGraph, orderShipment.ShipmentNbr);
                    KCSOOrderExt orderExt = order.GetExtension<KCSOOrderExt>();
                    KCMapShipment shipmentMapper = new KCMapShipment();

                    int? customerOrderNbr = Convert.ToInt32(order.CustomerOrderNbr);

                    if (shipment != null && orderShipment.Confirmed.GetValueOrDefault()
                        && KCGeneralDataHelper.GetExistingCAOrderById(helperOrder, customerOrderNbr) != null && shipmentKCExt?.UsrKCExported != true)
                        
                    {
                        string log;
                        KCErrorResponse response = new KCErrorResponse();
                        logger.SetParentAndEntityIds(order.OrderNbr, shipment.ShipmentNbr);

                        try
                        {
                            response = helperShipment.MarkTheOrderAsShipped(shipmentMapper.GetAPIShipment(shipment, package, lines), customerOrderNbr);
                        }
                        catch(Exception ex)
                        {
                            log = KCMessages.CorruptedShipment(shipment.ShipmentNbr);
                            logger.Information(log);
                            continue;
                        }


                        if (response != null)
                        {
                            log = KCMessages.ShipmentExportFailure(shipment.ShipmentNbr, response.Error.Message);
                        }
                        else
                        {
                            shipmentKCExt.UsrKCExported = true;
                            orderShipmentGraph.Shipments.Update(shipment);
                            orderShipmentGraph.Save.Press();
                            anyExported = true;
                            log = KCMessages.ShipmentExported(shipment.ShipmentNbr);
                        }

                        logger.Information(log);
                    }
                }
            }

            logger.ClearLoggingIds();
            logger.Information(anyExported ? KCMessages.ShipmentExportSuccess : KCMessages.NoShipmentsToExport);
        }

        public bool CheckData(SOOrderShipment orderShipment, DateTime? dateFrom, DateTime? dateTo)
        {
            bool isBiggerThan = dateFrom.GetValueOrDefault() == default ? true : orderShipment.CreatedDateTime.GetValueOrDefault().BiggerThan(dateFrom.GetValueOrDefault());
            bool isLessThan = dateTo.GetValueOrDefault() == default ? true : orderShipment.CreatedDateTime.GetValueOrDefault().LessThan(dateTo.GetValueOrDefault());
            return isBiggerThan && isLessThan;
        }
 
        #region Validation
        public bool CheckShippingCarrier(KCShipmentAPIHelper helper, SOShipment shipment)
        {
            APIResultOfArrayOfShippingCarrier carriers = helper.GetShippingCarriers();
            if (shipment.ShipVia != null)
            return carriers.ResultData.Any(x => x.CarrierName.Equals(shipment.ShipVia));
            else return true;
        }
        #endregion
    }
}
