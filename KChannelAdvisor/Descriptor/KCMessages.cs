using PX.Common;

namespace KChannelAdvisor.Descriptor
{
    [PXLocalizable("KNChannelAdvisor")]
    public static class KCMessages
    {

        #region Site Configuration
        public const string AccountIdGreater0 = "Account ID should be greater than zero.";
        public const string AlreadyExistAccountId = "Account ID is already exists in site master.";
        public const string DevConfirmPassword = "Confirm Password in not matched to Password.";
        public const string FTPConfirmPassword = "FTP Confirm Password in not matched to FTP Password.";
        public const string RequestApiAccessFailed = "Request API Access failed {0}.";
        public const string RequestApiAccessSuccess = "Request API Access successful";
        public const string VerifyApiAccessFailed = "Verify API Access failed";
        public const string VerifyApiAccessSuccess = "Verify API Access successful.";
        public const string NullException = "null exception.";
        public const string XmlNullException = "xml null exception.";
        public const string SiteAssociationCheck = "Company to Single Site Association checkbox or Branch to Sites Association checkbox is mandatory.";
        public const string ChannelAdvisorSiteRequired = "ChannelAdvisor Site field is required.";
        #endregion

        #region Variation Relationship
        public const string RelationshipIdStartsWithDigit = "Relationship ID cannot start with a digit.";
        public const string RelationshipIdContainsSpaces = "Relationship IDs cannot contain spaces.";
        public const string RelationAttributeDuplicatedException = "Variation Relationship Attributes should be different.";
        public const string RelationItemClassDuplicatedException = "Item Class already involved in another Relationship.";
        public const string RelatedConfigurableItemExistsException = "Relationship cannot be deleted: There are Configurable Items that are using this Relationship.";
        public const string RelationshipIDCannotBeBull = "Please provide Relationship ID.";
        #endregion

        #region Classification
        public const string NoClassificationsFound = "No classifications were found";
        public const string ThisItemClassCanNotBeDeletedBecauseItIsUsedInMapping = "The Item Class is mapped to Classification. Item Class may not be deleted";
        public const string ClassificationCanNotBeEmpty = "Please provide respective Classifications to the mapped Item Classes";
        public const string ClassificationMoreThanOnce = "One Classification may be associated with only one Item Class";
        public static string DeleteClassifications(string classification, string itemClass) => $"The Classification \"{classification}\" mapped to the Item Class \"{itemClass}\" was removed form ChannelAdvisor";
        #endregion

        #region Mapping Settings
        public const string MappingDirectionException = "Mapping Direction was not properly set during the mapping process.";
        public const string TargetExpressionException = "couldn't identify Expression type.";
        public const string FieldNameException = "value for the specified field was not found.";
        public const string SumHandlerException = "we couldn't find sum operator for this combination of fields.";
        public const string SubtractHandlerException = "we couldn't find subtract operator for this combination of fields.";
        public const string UnknownOperationException = "this type of expression is not supported.";
        public const string UnknownFieldNameException = "field with the specified name was not found.";
        public const string ImagePlacementShouldBeFilled = "Please provide values for mapped Image Placements";
        public const string AttributeCanNotBeNull = "Attribute value can not be null";
        #endregion

        #region Image Placements
        public const string ImagePlacementMoreThanOnce = "One Image Placement may be associated with one Attribute.";
        public const string PaymentMethodCanNotBeEmpty = "Please provide values for the mapped Payment Methods.";
        public const string WarehouseCanNotBeEmpty = "Please provide values for mapped Distribution Centers. Select at least one Warehouse or Vendor Inventory.";
        public const string DefaultDCCanNotBeEmpty = "Please provide value for Default Distribution Center.";
        public const string ImagePlacementsMoreThan20 = "Number of Image Placements associations cannot exceed 20 entries";
        public const string PaymentMethodMoreThanOnce = "One ChannelAdvisor Payment Method may be associated with one Acumatica Payment Method.";
        public const string CAAttributeMoreThanOnce = "One ChannelAdvisor Attribute cannot be mapped to more than one Acumatica attribute.";
        public const string CAAttributeCanNotBeEmpty = "Please provide value for mapped Attributes.";
        #endregion

        #region Serilog
        public const string DbSinkException = "LoggerConfiguration was not set for KCDbSink.";
        public const string TraceSinkException = "LoggerConfiguration was not set for KCTraceSink.";
        #endregion

        #region Product Sync
        public static string SiteConfigNotSet = "Site Configuration should be set up for the Product Sync process to work.";
        public static string ProductSkipped(string productCD) => $"Product \"{productCD?.Trim()}\" was skipped for export, because not each child item of this product has mapped Item Class.";
        public static string ProductExported(string productCD) => $"Product \"{productCD?.Trim()}\" was successfully exported to the ChannelAdvisor.";
        public static string ProductUpdated(string productCD) => $"Product \"{productCD?.Trim()}\" was successfully updated at the ChannelAdvisor.";
        public static string ProductDeleted(string productCD) => $"Product \"{productCD?.Trim()}\" was successfully deleted from the ChannelAdvisor.";
        public static string BulkUploadSuccess(string siteMasterCD) => $"Product Sync for {siteMasterCD} has been processed successfully";
        public const string UnsupportedSyncType = "Unsupported Sync Type";
        public const string NoProductsToExport = "There are no products for export.";
        public const string ProductIdsRetrievalSuccess = "ChannelAdvisor IDs for exported products were successfully retrieved.";
        public const string DateToBiggerThanDateFrom = "Date From should be bigger than Date To.";
        public static string DateShouldBeSet(string dateParamName) => $"{dateParamName} is required for the Custom sync.";
        #endregion

        #region Order Import
        public static string OrderImported(string ACOrderNbr, string CAOrderNbr) => $"Order imported successfully. Acumatica Order ID: \"{ACOrderNbr?.Trim()}\", ChannelAdvisor Order ID: \"{CAOrderNbr?.Trim()}\".";
        public const string OrderImportSuccess = "Order import process finished successfully.";
        public const string NoOrdersToImport = "There are no orders to import.";
        public static string PaymentMethodDoesntMapped(int orderID, string paymentMethod) => $"Order \"{orderID}\" was skipped for import, because Payment Method \"{paymentMethod}\" is not mapped on the Payment Methods Mapping screen.";
        public static string PaymentWasntCreated(int orderID, string paymentMethod) => $"Payment for order \"{orderID}\" hasn't been created/released. Please, check the configuration of the \"{paymentMethod}\" Payment Method.";
        public static string WebsitesAreNotAssociated(int orderID, string inventoryCD, string customerName) => $"Order \"{orderID}\" was skipped for import, because \"{inventoryCD}\" and \"{customerName}\" don't have any common Website.";
        public static string CustomerNotFound(int orderID, string customerName) => $"Order \"{orderID}\" was skipped for import, because customer \"{customerName}\" wasn't found in the system.";
        public static string ProductDoesntExist(int orderID, int productID) => $"Order \"{orderID}\" was skipped for import, because Product \"{productID}\" doesn't exist in the system.";
        public static string TaxDoesntConfigured(int orderID) => $"Taxes for order \"{orderID}\" was skipped for import, because Tax Zone MANUALZONE or Tax MANUALID doesn't configured in the system.";
        public static string ProductDoesntActive(int orderID, int productID) => $"Order \"{orderID}\" could not be imported because product \"{productID}\" is not Active.";
        public static string SalesAcctIDCannotBeEmpty(int orderID, int productID) => $"Order \"{orderID}\" could not be imported because Sales Account and/or Sales Sub of the product \"{productID}\" is empty.";

        public static string InvalidCountryInBillingAddress(int orderID, string country) => $"Order \"{orderID}\" contains invalid Country - \"{country}\" in Billing address and cannot be imported. Please correct the address and try again.";
        public static string InvalidCountryInShippingAddress(int orderID, string country) => $"Order \"{orderID}\" contains invalid Country - \"{country}\" in Shipping address and cannot be imported. Please correct the address and try again.";
        public static string InvalidZipInBillingAddress(int orderID, string zip) => $"Order \"{orderID}\" contains invalid Postal Code - \"{zip}\" in Billing address and cannot be imported. Please correct the address and try again.";
        public static string InvalidZipInShippingAddress(int orderID, string zip) => $"Order \"{orderID}\" contains invalid Postal Code - \"{zip}\" in Shipping address and cannot be imported. Please correct the address and try again.";
        public static string InvalidStateInBillingAddress(int orderID, string state) => $"Order \"{orderID}\" contains invalid State - \"{state}\" in Billing address and cannot be imported. Please correct the address and try again.";
        public static string InvalidStateInShippingAddress(int orderID, string state) => $"Order \"{orderID}\" contains invalid State - \"{state}\" in Shipping address and cannot be imported. Please correct the address and try again.";
        public static string InvalidOrderScenario(string orderID) => $"Invalid or Unsupported Order scenario experienced for order \"{orderID}\". Please notify the Kensium team.";
        public static string DateShouldBeSetDE(string dateParamName) => $"{dateParamName} is required.";
        #endregion

        #region Fulfillments
        public static string FulfillmentsUpdated(string orderNbr) => $"Fulfillment(s) for \"{orderNbr?.Trim()}\" was successfully imported to the Acumatica.";
        public static string FailedFulfillments(string orderNbr) => $"Fulfillment details for order \"{orderNbr?.Trim()}\" cannot be imported. Bundle parent and child items cannot be shipped separately.";
        public static string FulfillmentsWasntImported(string orderNbr, string InventoryCD, string siteCD) => $"Fulfillment(s) for \"{orderNbr?.Trim()}\" was skipped for import. There are not enough quantity of \"{InventoryCD.Trim()}\" in the \"{siteCD?.Trim()}\" Warehouse. Shipment cannot be created.";
        public static string FulfillmentProductInactive(string orderNbr, string InventoryCD) => $"Fulfillment for order \"{orderNbr?.Trim()}\" could not be imported because product \"{InventoryCD.Trim()}\" is not Active.";
        public static string ProductDoesntExistinFulfillment(int orderNbr, int InventoryID) => $"Fulfillment details for order \"{orderNbr}\" cannot be imported, because Product \"{InventoryID}\" doesn't exist in the system.";
        #endregion

        #region Shipment Export
        public static string ShipmentExported(string shipmentNbr) => $"Shipment \"{shipmentNbr?.Trim()}\" was successfully exported to the ChannelAdvisor.";
        public static string CorruptedShipment(string shipmentNbr) => $"Shipment \"{shipmentNbr?.Trim()}\" is corrupted and cannot be exported. (Parent item can not be shipped without child items)";
        
        public const string ShipmentExportSuccess = "Shipment export process finished successfully.";
        public const string NoShipmentsToExport = "There are no shipments for export.";
        public static string ShipmentExportFailure(string shipmentNbr, string response) => $"Unable to export shipment \"{shipmentNbr}\": {response}";
        public static string ShipViaDoesnotExist(string shipmentNbr) => $"Shipment \"{shipmentNbr}\" cannot be exported as ChannelAdvisor site doesn't have the respective shipping method configured.";
        #endregion

        #region Misc
        public static string ProcessException(string siteMasterCD, string message, string trace) => $"Data Exchange process for \"{siteMasterCD.Trim()}\" was executed with error: {message} {trace}";
        public const string InternalServerError = "Internal Server Error";
        public const string CompositeItemLinePlaceholder = "<<Configure>>";
        public const string InvalidDataExchangeProcess = "Please select action to initiate data exchange process";
        public const string ClearLog = "Clear Log";
        #endregion

        #region MSMQ
        public static string MSMQSyncFTP(int msgCount) => $"Push Notifications were synchronized with ChannelAdvisor via FTP. {msgCount} mesages were sent.";
        public static string MSMQSyncAPI = "Push Notification was synchronized with ChannelAdvisor via API.";
        public static string MSMQShouldBeInitialized = "Push Notifications configuration should be initialized.";
        #endregion

        #region Cross-Reference Mapping
        public const string CAAttributeMoreThanOnceCR = "One ChannelAdvisor Attribute cannot be mapped to more than one CA Field Reference.";
        #endregion

    }
}
