using KChannelAdvisor.DAC;
using KChannelAdvisor.Descriptor;
using PX.Data;
using PX.Data.EP;

namespace KChannelAdvisor.BLC
{
    public class KCRequestLogInq : PXGraph<KCRequestLogInq>
    {
        #region Views
        [PXFilterable]
        public PXSelectOrderBy<KCLog, OrderBy<Desc<KCLog.createdDateTime>>> Logs;
        public PXSelectOrderBy<KCLog, OrderBy<Desc<KCLog.requestId>>> LastLog;
        public PXSelect<KCLog> LogMessage;
        #endregion

        #region Actions
        public PXAction<KCLog> ClearLogs;
        #endregion

        #region Action handlers
        [PXButton]
        [PXUIField(DisplayName = KCMessages.ClearLog)]
        public virtual void clearLogs()
        {

            if (LogMessage.AskExt() == WebDialogResult.Yes)
            {
                var logs = Logs.Select();
                if (logs != null && logs.Count > 0)
                {
                    foreach (var log in logs)
                    {
                        Logs.Delete(log);
                    }

                    Actions.PressSave();
                }
            }
            
            


        }
        #endregion
    }
}
