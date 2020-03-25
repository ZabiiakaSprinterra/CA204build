using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using KChannelAdvisor.DAC;
using PX.Objects.CS;
using KChannelAdvisor.BLC;
using System.Linq;
using PX.Objects.CR;
using System.Text.RegularExpressions;

namespace KChannelAdvisor.Descriptor.CustomAttributes
{
    public class KCTaxManagementState : PXCustomSelectorAttribute
    {
        public KCTaxManagementState() : base(typeof(Search<State.stateID>))
        {
            DescriptionField = typeof(State.stateID);
            SubstituteKey = typeof(State.stateID);
            ValidateValue = false;
            
        }

        protected virtual IEnumerable GetRecords()
        {
            KCSiteMasterMaint graph = (KCSiteMasterMaint)_Graph;
            var taxMappingIds = graph.TaxManag.Select().RowCast<KCTaxManagement>().Where(x => x.StateId != null)
                .Select(x => x.StateId).ToList();
            int count = taxMappingIds.Count;
            for (int i = 0; i < count; i++)
            {
                if (Regex.Matches(taxMappingIds[0], "[;]", RegexOptions.Singleline).Count != 0)
                {
                    taxMappingIds[0].Replace(" ", "").Split(';');
                    foreach (var splited in taxMappingIds[0].Replace(" ", "").Split(';'))
                    {
                        taxMappingIds.Add(splited.ToUpper());
                    }
                    taxMappingIds.Remove(taxMappingIds[0]);
                }
            }
            var taxMappingIdsArr = taxMappingIds.ToArray();
            var country = graph.TaxManag.Current.CountryId;// Select().RowCast<KCTaxManagement>()
                                                           //.Select(x => x.CountryId).ToArray();
            var allStates = new List<State>();
            if (taxMappingIds.Count() != 0)
            {
                allStates = graph.AllStates.Select().RowCast<State>().Where(x => x.CountryID == country).ToList();// graph.RequiredState.Select().RowCast<State>().ToList();
                foreach (var item in taxMappingIdsArr)
                {
                    allStates.Remove(allStates.Find(x=>x.StateID==item));
                }
            }

            else
            {
                allStates = graph.AllStates.Select().RowCast<State>().Where(x => x.CountryID == country).ToList();
            }

            foreach (State market in allStates)
            {
                yield return market;
                
            }
        }
    }
}