using PX.Data;

namespace KChannelAdvisor.Descriptor
{
    public class KCConstants
    {
        public enum Mode
        {
            Test = 1,
            Live = 2
        }

        public const string DefaultTaxValue = "1";
        public const int salesTaxAcctID = 1529;
        public const int salesTaxSubID = 557;
        public const string taxCalcType = "D";
        public const string taxCalcLevel = "1";
        public const string Taxable = "TAXABLE";
        public const string Channel = "Channel";
        public const string ChannelAdvisor = "CHANNELADVISOR";
        public const double SYNC_DELAY_SECONDS = 7.5;
        public const string LineTypeGI = "GI";
        public const string LineTypeGN = "GN";
        public const string Test = "Test";
        public const string Live = "Live";
        public const string RequestApiAccess = "Request API Access";
        public const string VerifyApiAccess = "Verify API Access";
        public const string VerifyFtpAccess = "Verify FTP Access";
        public const string Failure = "Failure";
        public const string Success = "Success";
        public const string DualParameters = "{0} - {1}";

        public const string ItemClassDimension = "INITEMCLASS";
        public const string InventoryItemEntityType = "PX.Objects.IN.InventoryItem";

        public const string CaMappingField = "ChannelAdvisor Field";
        public const string AMappingField = "Acumatica Field";

        public const string AcumaticaSiteName = "Acumatica";
        public const string DefaultSub = "Default";
        public const string NamespaceTesterPackage = "KN.CP";
        public const string Customer = "CUSTOMER";
        public const string Bizacct = "BIZACCT";

        #region Composite Stock Item Types
        public const string BundleProduct = "B";
        public const string ConfigurableProduct = "C";
        public const string GroupedProduct = "G";
        #endregion

        #region Mapping Expression Operators
        public const char Add = '+';
        public const char Subtract = '-';
        #endregion

        #region Entities
        public const string Product = "Product";
        #endregion

        #region Distribution Center Types
        public const string Warehouse = "Warehouse";
        public const string DropShip = "DropShip";
        public const string RetailStore = "RetailStore";
        #endregion

        #region Product Sync
        public const string DateTo = "Date To";
        public const string DateFrom = "Date From";
        public const int MaxQty = 100000000;
        public const string OpenTag = "/* RTE style begin */";
        public const string CloseTag = "/* RTE style end */";
        #endregion

        #region FTP
        public const string ProductTypeAttributeName = "Product Type";
        public const string DeleteMarker = "_Delete_";
        public const string Parent = "Parent";

        public class Configurable : Constant<string>
        {
            public Configurable() : base("C") { }
        }
        #endregion

        #region BQL Constants        
        public const string Active = "AC";
        public class active : Constant<string>
        {
            public active() : base(Active)
            {
            }
        }
        //public class defaultTaxValue : Constant<string>
        //{
        //    public defaultTaxValue() : base(DefaultTaxValue)
        //    {
        //    }
        //}
        #endregion

        #region RadioButtonValueSiteMaster
        public static class RadioButtonValue
        {
            public class KCListAttribute : PXStringListAttribute
            {
                public KCListAttribute()
                    : base(
                    new string[] { Import, Select },
                    new string[] { "Import Tax Values as-is from ChannelAdvisor", "Select existing Tax Zone in Acumatica", })
                {; }
            }
            public const string Import = "0";
            public const string Select = "1";

        }
        #endregion
    }
}
