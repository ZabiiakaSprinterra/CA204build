using System;
using System.Linq;
using KChannelAdvisor.Descriptor.API.Entity;
using KChannelAdvisor.BLC.Ext;
using KChannelAdvisor.DAC;
using PX.Data;
using PX.Objects.SO;
using PX.Objects.TX;
using System.Collections.Generic;

namespace KChannelAdvisor.Descriptor.Helpers
{
    public class KCMarketplaceHelper
    {

        public string GetOrderType(KCAPIOrder order, SOOrderEntry orderEntry)
        {
            //string marketplaceName = order.SiteName;
            string marketplaceName = order.BillingFirstName.Split()[0];
            var orderExt = orderEntry.GetExtension<KCSOOrderEntryExt>();
            int? marketplaceId = orderExt.KCMarketplaceEntity.SelectSingle(marketplaceName)?.MarketplaceId;
            if (marketplaceId == null)
            {
                marketplaceName = order.SiteName;
                marketplaceId = orderExt.KCMarketplaceEntity.SelectSingle(marketplaceName)?.MarketplaceId;
            }
            var marketplace = orderExt.KCMarketplaceManagement.SelectSingle(marketplaceId.ToString());
            if (marketplace != null)
            {
                return marketplace.SOOrderType;
            }
            else
            {
                return orderExt.KCConnection.SelectSingle().SOOrderType;
            }
        }
        public string GetTaxSettings(KCAPIOrder order, SOOrderEntry orderEntry)
        {
            //string marketplaceName = order.SiteName;
            string marketplaceName = order.BillingFirstName.Split()[0];
            var orderExt = orderEntry.GetExtension<KCSOOrderEntryExt>();
            PXResultset<KCTaxManagement> taxManagement = null;
            KCTaxManagement taxrecord = null;
            int? marketplaceId = orderExt.KCMarketplaceEntity.SelectSingle(marketplaceName)?.MarketplaceId;
            if (marketplaceId == null)
            {
                marketplaceName = order.SiteName;
                marketplaceId = orderExt.KCMarketplaceEntity.SelectSingle(marketplaceName)?.MarketplaceId;
            }
            var marketplace = orderExt.KCMarketplaceManagement.SelectSingle(marketplaceId.ToString());
            var isCATaxZoneExist = orderExt.KCTaxZoneId.SelectSingle(KCConstants.Channel);
            Boolean.TryParse(orderExt.KCXSiteMaster.SelectSingle().IsImportTax, out bool IsImport);
            if (marketplace != null)
            {
                taxManagement = orderExt.KCTaxManagementView.Select(marketplace.MarketplaceId);
                var taxrecordlist = taxManagement.RowCast<KCTaxManagement>().ToList().Where
                    (x => x.CountryId == order.BillingCountry);
                foreach (var taxman in taxrecordlist)
                {
                    var byState = taxrecordlist.Where(x => (x?.StateId == order.BillingStateOrProvince) || x?.StateId?.Contains(order.BillingStateOrProvince) == true);
                    if (byState.Count() == 0 && taxrecordlist.Where(x => x.CountryId == order.BillingCountry && x?.StateId == null).Count() != 0)
                    {
                        taxrecord = taxrecordlist.Where(x => x.CountryId == order.BillingCountry && x?.StateId == null).FirstOrDefault();
                    }
                    else
                    {
                        taxrecord = byState.FirstOrDefault();
                    }
                }

                

            }
            if (marketplace != null) // If marketplace exist on Marketplace grid settings
            {

                if (taxManagement != null && taxrecord != null)
                {
                    if (!taxrecord.UseDefTaxZone.GetValueOrDefault()) // if this row has UseDefTaxZone on true
                    {

                        string defTaxZone = taxrecord.TaxZoneId;
                        return defTaxZone;
                    }
                    else
                    {
                        //if (orderExt.KCXSiteMaster.SelectSingle().IsImportTax == "0") // if UseDefTaxZone is true--use General settings
                        {

                            if (isCATaxZoneExist == null)
                            {
                                SalesTaxMaint stax = PXGraph.CreateInstance<SalesTaxMaint>();
                                TaxZoneMaint CAtaxzone = PXGraph.CreateInstance<TaxZoneMaint>();
                                Tax tax = stax.Tax.Insert();
                                TaxZone newTaxZone = CAtaxzone.TxZone.Insert();
                                TaxZoneDet taxZoneDet = CAtaxzone.TxZoneDet.Insert();
                                stax.Tax.Cache.SetValue<Tax.taxID>(tax, KCConstants.ChannelAdvisor);
                                stax.Tax.Cache.SetValue<Tax.descr>(tax, KCConstants.ChannelAdvisor);
                                stax.Tax.Cache.SetValue<Tax.salesTaxAcctID>(tax, KCConstants.salesTaxAcctID);
                                stax.Tax.Cache.SetValue<Tax.salesTaxSubID>(tax, KCConstants.salesTaxSubID);
                                stax.Tax.Cache.SetValue<Tax.taxCalcType>(tax, KCConstants.taxCalcType);
                                stax.Tax.Cache.SetValue<Tax.taxCalcLevel>(tax, KCConstants.taxCalcLevel);
                                stax.Persist();
                                CAtaxzone.TxZone.Cache.SetValue<TaxZone.taxZoneID>(newTaxZone, KCConstants.Channel);
                                CAtaxzone.TxZone.Cache.SetValue<TaxZone.descr>(newTaxZone, KCConstants.Channel);
                                CAtaxzone.TxZone.Cache.SetValue<TaxZone.dfltTaxCategoryID>(newTaxZone, KCConstants.Taxable);
                                CAtaxzone.TxZoneDet.Cache.SetValue<TaxZoneDet.taxZoneID>(taxZoneDet, newTaxZone.TaxZoneID);
                                CAtaxzone.TxZoneDet.Cache.SetValue<TaxZoneDet.taxID>(taxZoneDet, tax.TaxID);
                                CAtaxzone.Persist();
                                string defTaxZone = newTaxZone.TaxZoneID;
                                return defTaxZone;

                            }
                            else
                            {
                                string defTaxZone = isCATaxZoneExist.TaxZoneID;
                                return defTaxZone;
                            }
                        }
                        //else
                        //{
                        //    string defTaxZone = orderExt.KCXSiteMaster.SelectSingle().TaxZone;
                        //    return defTaxZone;
                        //}
                    }
                }
                else
                {
                    if (!marketplace.UseDefTaxZone.GetValueOrDefault()) // if this row has UseDefTaxZone on true
                    {

                        string defTaxZone = marketplace.TaxZone;
                        return defTaxZone;
                    }
                    else
                    {
                        //if (orderExt.KCXSiteMaster.SelectSingle().IsImportTax == "0") // if UseDefTaxZone is true--use General settings
                        {

                            if (isCATaxZoneExist == null)
                            {
                                SalesTaxMaint stax = PXGraph.CreateInstance<SalesTaxMaint>();
                                TaxZoneMaint CAtaxzone = PXGraph.CreateInstance<TaxZoneMaint>();
                                Tax tax = stax.Tax.Insert();
                                TaxZone newTaxZone = CAtaxzone.TxZone.Insert();
                                TaxZoneDet taxZoneDet = CAtaxzone.TxZoneDet.Insert();
                                stax.Tax.Cache.SetValue<Tax.taxID>(tax, KCConstants.ChannelAdvisor);
                                stax.Tax.Cache.SetValue<Tax.descr>(tax, KCConstants.ChannelAdvisor);
                                stax.Tax.Cache.SetValue<Tax.salesTaxAcctID>(tax, KCConstants.salesTaxAcctID);
                                stax.Tax.Cache.SetValue<Tax.salesTaxSubID>(tax, KCConstants.salesTaxSubID);
                                stax.Tax.Cache.SetValue<Tax.taxCalcType>(tax, KCConstants.taxCalcType);
                                stax.Tax.Cache.SetValue<Tax.taxCalcLevel>(tax, KCConstants.taxCalcLevel);
                                stax.Persist();
                                CAtaxzone.TxZone.Cache.SetValue<TaxZone.taxZoneID>(newTaxZone, KCConstants.Channel);
                                CAtaxzone.TxZone.Cache.SetValue<TaxZone.descr>(newTaxZone, KCConstants.Channel);
                                CAtaxzone.TxZone.Cache.SetValue<TaxZone.dfltTaxCategoryID>(newTaxZone, KCConstants.Taxable);
                                CAtaxzone.TxZoneDet.Cache.SetValue<TaxZoneDet.taxZoneID>(taxZoneDet, newTaxZone.TaxZoneID);
                                CAtaxzone.TxZoneDet.Cache.SetValue<TaxZoneDet.taxID>(taxZoneDet, tax.TaxID);
                                CAtaxzone.Persist();
                                string defTaxZone = newTaxZone.TaxZoneID;
                                return defTaxZone;

                            }
                            else
                            {
                                string defTaxZone = isCATaxZoneExist.TaxZoneID;
                                return defTaxZone;
                            }
                        }
                        //else
                        //{
                        //    string defTaxZone = orderExt.KCXSiteMaster.SelectSingle().TaxZone;
                        //    return defTaxZone;
                        //}

                    }
                }
            }
            else
            {
                if (orderExt.KCXSiteMaster.SelectSingle().IsImportTax == "0") // if UseDefTaxZone is true--use General settings
                {

                    if (isCATaxZoneExist == null)
                    {
                        SalesTaxMaint stax = PXGraph.CreateInstance<SalesTaxMaint>();
                        TaxZoneMaint CAtaxzone = PXGraph.CreateInstance<TaxZoneMaint>();
                        Tax tax = stax.Tax.Insert();
                        TaxZone newTaxZone = CAtaxzone.TxZone.Insert();
                        TaxZoneDet taxZoneDet = CAtaxzone.TxZoneDet.Insert();
                        stax.Tax.Cache.SetValue<Tax.taxID>(tax, KCConstants.ChannelAdvisor);
                        stax.Tax.Cache.SetValue<Tax.descr>(tax, KCConstants.ChannelAdvisor);
                        stax.Tax.Cache.SetValue<Tax.salesTaxAcctID>(tax, KCConstants.salesTaxAcctID);
                        stax.Tax.Cache.SetValue<Tax.salesTaxSubID>(tax, KCConstants.salesTaxSubID);
                        stax.Tax.Cache.SetValue<Tax.taxCalcType>(tax, KCConstants.taxCalcType);
                        stax.Tax.Cache.SetValue<Tax.taxCalcLevel>(tax, KCConstants.taxCalcLevel);
                        stax.Persist();
                        CAtaxzone.TxZone.Cache.SetValue<TaxZone.taxZoneID>(newTaxZone, KCConstants.Channel);
                        CAtaxzone.TxZone.Cache.SetValue<TaxZone.descr>(newTaxZone, KCConstants.Channel);
                        CAtaxzone.TxZone.Cache.SetValue<TaxZone.dfltTaxCategoryID>(newTaxZone, KCConstants.Taxable);
                        CAtaxzone.TxZoneDet.Cache.SetValue<TaxZoneDet.taxZoneID>(taxZoneDet, newTaxZone.TaxZoneID);
                        CAtaxzone.TxZoneDet.Cache.SetValue<TaxZoneDet.taxID>(taxZoneDet, tax.TaxID);
                        CAtaxzone.Persist();

                        string defTaxZone = newTaxZone.TaxZoneID;
                        return defTaxZone;

                    }
                    else
                    {
                        string defTaxZone = isCATaxZoneExist.TaxZoneID;
                        return defTaxZone;
                    }
                }
                else
                {
                    string defTaxZone = orderExt.KCXSiteMaster.SelectSingle().TaxZone;
                    return defTaxZone;
                }
            }

        }
    }
}
