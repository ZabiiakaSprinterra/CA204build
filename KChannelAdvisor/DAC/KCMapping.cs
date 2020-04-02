using KChannelAdvisor.Descriptor.CustomAttributes;
using KChannelAdvisor.Descriptor;
using PX.Data;
using PX.Data.BQL;
using System;

namespace KChannelAdvisor.DAC
{
    [Serializable]
    public class KCMapping : IBqlTable
    {
        #region Id
        [PXDBIdentity(IsKey = true)]
        public virtual int? Id { get; set; }
        public abstract class id : BqlInt.Field<id> { }
        #endregion
        #region LineNbr
        [PXDBInt]
        [PXUIField(DisplayName = "Line Nbr")]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : BqlInt.Field<lineNbr> { }
        #endregion
        #region EntityType
        [PXDBString(25, IsUnicode = true)]
        [PXUIField(Visible = false)]
        public virtual string EntityType { get; set; }
        public abstract class entityType : BqlString.Field<entityType> { }
        #endregion
        #region Direction
        [PXDBString(1, IsUnicode = true)]
        [PXUIField(Visible = false)]
        public virtual string Direction { get; set; }
        public abstract class direction : BqlString.Field<direction> { }
        #endregion
        #region MappingRule
        [PXDBString(25, IsUnicode = true)]
        [PXUIField(DisplayName = "Mapping Rule")]
        [KCMappingRules]
        public virtual string MappingRule { get; set; }
        public abstract class mappingRule : BqlString.Field<mappingRule> { }
        #endregion
        #region RuleType
        [PXDBString(1, IsUnicode = true)]
        [PXUIField(DisplayName = "Rule Type")]
        public virtual string RuleType { get; set; }
        public abstract class ruleType : BqlString.Field<ruleType> { }
        #endregion
        #region AViewName
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "Acumatica View")]
        public virtual string AViewName { get; set; }
        public abstract class aViewName : BqlString.Field<aViewName> { }
        #endregion
        #region AFieldHash
        [PXDBString(50, IsUnicode =true)]
        [PXUIField(DisplayName = KCConstants.AMappingField)]
        public virtual string AFieldHash { get; set; }
        public abstract class aFieldHash : BqlString.Field<aFieldHash> { }
        #endregion     
        #region CFieldHash
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = KCConstants.CaMappingField)]
        public virtual string CFieldHash { get; set; }
        public abstract class cFieldHash : BqlString.Field<cFieldHash> { }
        #endregion
        #region SourceExpression
        [PXUIField(DisplayName = "Expression")]
        [PXDBString(200, IsUnicode= true)]
        public virtual string SourceExpression { get; set; }
        public abstract class sourceExpression : BqlString.Field<sourceExpression> { }
        #endregion

        #region Noteid
        [PXDBGuid()]
        [PXUIField(DisplayName = "Noteid")]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : BqlGuid.Field<noteID> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : BqlString.Field<createdByScreenID> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime(InputMask = "g")]
        [PXUIField(DisplayName = "Requested Date")]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : BqlGuid.Field<lastModifiedByID> { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : BqlString.Field<lastModifiedByScreenID> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        [PXUIField(DisplayName = "Last Modified Date Time")]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : BqlByte.Field<tstamp> { }
        #endregion 
    }
}
