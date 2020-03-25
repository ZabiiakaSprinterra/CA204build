using System;
using KChannelAdvisor.Descriptor;
using PX.Data;
using PX.Data.BQL;

namespace KChannelAdvisor.DAC
{
    [Serializable]
    public class KCSiteAccess : IBqlTable
    {
        #region SiteMasterCD
        [PXDBString(30, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Site Master CD")]
        [PXDefault(typeof(KCSiteMaster.siteMasterCD))]
        [PXParent(typeof(Select<KCSiteMaster, Where<KCSiteMaster.siteMasterCD, Equal<Current<siteMasterCD>>>>))]
        public virtual string SiteMasterCD { get; set; }
        public abstract class siteMasterCD : BqlString.Field<siteMasterCD> { }
        #endregion

        #region AccountId
        [PXDefault]
        [PXDBInt(IsKey =true)]
        [PXUIField(DisplayName = "Account Id")]
        public virtual int? AccountId { get; set; }
        public abstract class accountId : BqlInt.Field<accountId> { }
        #endregion

        #region ProfileId
        [PXDefault]
        [PXDBInt]
        [PXUIField(DisplayName = "Profile Id")]
        public virtual int? ProfileId { get; set; }
        public abstract class profileId : BqlInt.Field<profileId> { }
        #endregion

        #region DevKey
        [PXDefault]
        [PXDBString(200, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Developer Key")]
        public virtual string DevKey { get; set; }
        public abstract class devKey : BqlString.Field<devKey> { }
        #endregion

        #region Password
        [PXDefault]
        [PXUIField(DisplayName = "Password")]
        [PXRSACryptString(1000, IsUnicode = true)]
        public virtual string DevPassword { get; set; }
        public abstract class devPassword : BqlString.Field<devPassword> { }
        #endregion

        #region ConfirmPassword
        [PXDefault]
        [PXUIField(DisplayName = "Confirm Password")]
        [PXRSACryptString(1000, IsUnicode = true)]
        public virtual string DevConfirmPassword { get; set; }
        public abstract class devConfirmPassword : BqlString.Field<devConfirmPassword> { }
        #endregion

        #region Mode
        [PXDefault]
        [PXDBInt()]
        [PXUIField(DisplayName = "Mode")]
        [PXIntList(new int[] { (int)KCConstants.Mode.Test, (int)KCConstants.Mode.Live }, new string[] { KCConstants.Test, KCConstants.Live })]
        public virtual int? Mode { get; set; }
        public abstract class mode : IBqlField { }
        #endregion

        #region FTPHostname
        [PXDBString(150, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Hostname", Enabled = false)]
        public virtual string FTPHostname { get; set; }
        public abstract class fTPHostname : BqlString.Field<fTPHostname> { }
        #endregion

        #region FTPUsername
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Username", Enabled = false)]
        public virtual string FTPUsername { get; set; }
        public abstract class fTPUsername : BqlString.Field<fTPUsername> { }
        #endregion

        #region FTPPassword
        [PXUIField(DisplayName = "Password", Enabled = false)]
        [PXRSACryptString(1000, IsUnicode = true)]
        public virtual string FTPPassword { get; set; }
        public abstract class fTPPassword : BqlString.Field<fTPPassword> { }
        #endregion

        #region FTPConfirmPassword
        [PXUIField(DisplayName = "Confirm Password", Enabled = false)]
        [PXRSACryptString(1000, IsUnicode = true)]
        public virtual string FTPConfirmPassword { get; set; }
        public abstract class fTPConfirmPassword : BqlString.Field<fTPConfirmPassword> { }
        #endregion

        #region FTPInputDirectory
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Input Directory", Enabled = false)]
        public virtual string FTPInputDirectory { get; set; }
        public abstract class fTPInputDirectory : BqlString.Field<fTPInputDirectory> { }
        #endregion

        #region RefreshToken
        [PXDBString(500, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Refresh Token")]
        public virtual string RefreshToken { get; set; }
        public abstract class refreshToken : BqlString.Field<refreshToken> { }
        #endregion

        #region ApplicationId
        [PXDBString(250, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Application Id")]
        public virtual string ApplicationId { get; set; }
        public abstract class applicationId : BqlString.Field<applicationId> { }
        #endregion

        #region SharedSecret
        [PXDBString(250, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Shared Secret")]
        public virtual string SharedSecret { get; set; }
        public abstract class sharedSecret : BqlString.Field<sharedSecret> { }
        #endregion

        #region FTPBatchMode
        [PXDBBool()]
        [PXUIField(DisplayName = "Enable Batch mode for FTP")]
        public virtual bool? FTPBatchMode { get; set; }
        public abstract class fTPBatchMode : BqlBool.Field<fTPBatchMode> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public Guid? CreatedByID { get; set; }
        public abstract class createdByID : BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : BqlString.Field<createdByScreenID> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        [PXUIField(DisplayName = "Created Date Time")]
        public DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : BqlGuid.Field<lastModifiedByID> { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : BqlString.Field<lastModifiedByScreenID> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        [PXUIField(DisplayName = "Last Modified Date Time")]
        public DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public byte[] Tstamp { get; set; }
        public abstract class tstamp : BqlByte.Field<tstamp> { }
        #endregion

        #region Noteid
        [PXDBGuid()]
        [PXUIField(DisplayName = "Noteid")]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : BqlGuid.Field<noteID> { }
        #endregion


    }
}

