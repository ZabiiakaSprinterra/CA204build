using System;
using System.Net;
using System.Text;
using System.Xml;
using KChannelAdvisor.DAC;
using KChannelAdvisor.DAC.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PX.Data;

namespace KChannelAdvisor.Descriptor
{
    public class KCSiteMasterHelper
    {

        public KCInternalReponse RequestAPIAccess(KCSiteMaster xSiteMaster)
        {
            KCInternalReponse response = new KCInternalReponse();
            try
            {
                WebHeaderCollection headers = new WebHeaderCollection();
                headers["Cache-Control"] = xSiteMaster.ChacheControlHeader; 
                headers["soapaction"] = xSiteMaster.SoapCaptionHeader;
                string reqbody = "<soapenv:Envelope xmlns:soapenv=\""+xSiteMaster.Envelop+"\" xmlns:web=\""+xSiteMaster.Webservices+"\">\r\n   <soapenv:Header>\r\n      <web:APICredentials>\r\n         <web:DeveloperKey>" + xSiteMaster.DevKey + "</web:DeveloperKey>\r\n         <web:Password>" + xSiteMaster.DevPassword + "</web:Password>\r\n      </web:APICredentials>\r\n   </soapenv:Header>\r\n   <soapenv:Body>\r\n      <web:RequestAccess>\r\n         <web:localID>" + xSiteMaster.ProfileId + "</web:localID>\r\n      </web:RequestAccess>\r\n   </soapenv:Body>\r\n</soapenv:Envelope>";
                var apiresponse = KCApiHelper.CallWebAPI(xSiteMaster.ApiResponse, reqbody, "POST", headers, "text/xml; charset=utf-8");

                if (apiresponse == null)
                {
                    PXTrace.WriteError(KCConstants.RequestApiAccess + " - " + KCMessages.NullException);
                    response.IsSuccess = false;
                    response.Message = string.Format(KCMessages.RequestApiAccessFailed, "");
                }

                if (apiresponse.IsSuccess)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(apiresponse.response);
                    string json = JsonConvert.SerializeXmlNode(doc);

                    if (json != null)
                    {
                        JObject obj = JsonConvert.DeserializeObject<JObject>(json);
                        JObject innerObj = obj["soap:Envelope"] as JObject;
                        JObject lolSummorLvl = innerObj["soap:Body"] as JObject;
                        JObject lolSummorLvl2 = lolSummorLvl["RequestAccessResponse"] as JObject;
                        JObject lolSummorLvl3 = lolSummorLvl2["RequestAccessResult"] as JObject;
                        string status = (string)lolSummorLvl3["Status"];
                        string MessageCode = (string)lolSummorLvl3["MessageCode"];
                        string Message = "";

                        if (status == KCConstants.Failure)
                        {
                            Message = (string)lolSummorLvl3["Message"];
                        }

                        string ResultData = (string)lolSummorLvl3["ResultData"];


                        if (status != KCConstants.Failure)
                        {
                            response.IsSuccess = true;
                            response.Message = KCMessages.RequestApiAccessSuccess;
                        }
                        else
                        {
                            string errormsg = string.Format(KCConstants.DualParameters, KCConstants.RequestApiAccess, Message);
                            PXTrace.WriteError(errormsg);

                            response.IsSuccess = false;
                            response.Message = string.Format(KCMessages.RequestApiAccessFailed, Message);
                        }
                    }
                    else
                    {
                        string errormsg = string.Format(KCConstants.DualParameters, KCConstants.RequestApiAccess, KCMessages.XmlNullException);
                        PXTrace.WriteError(errormsg);

                        PXTrace.WriteError(KCConstants.RequestApiAccess + " - " + KCMessages.XmlNullException);
                        response.IsSuccess = false;
                        response.Message = string.Format(KCMessages.RequestApiAccessFailed, "");
                    }

                }
                else
                {
                    string errormsg = string.Format(KCConstants.DualParameters, KCConstants.RequestApiAccess, apiresponse.Message);
                    PXTrace.WriteError(errormsg);

                    response.IsSuccess = false;
                    response.Message = string.Format(KCMessages.RequestApiAccessFailed, "");
                }

                return response;
            }
            catch (Exception ex)
            {
                string errormsg = string.Format(KCConstants.DualParameters, KCConstants.RequestApiAccess, ex.Message);
                PXTrace.WriteError(errormsg);

                response.IsSuccess = false;
                response.Message = string.Format(KCMessages.RequestApiAccessFailed, ex.Message);
                return response;
            }
        }

        public KCInternalReponse VerifyApiAccess(KCSiteMaster xSiteMaster)
        {
            try
            {
                KCInternalReponse response = new KCInternalReponse();
                if (xSiteMaster != null)
                {
                    xSiteMaster.ApplicationId = !string.IsNullOrEmpty(xSiteMaster.ApplicationId) ? xSiteMaster.ApplicationId.Trim() : "";
                    xSiteMaster.SharedSecret = !string.IsNullOrEmpty(xSiteMaster.SharedSecret) ? xSiteMaster.SharedSecret.Trim() : "";
                    xSiteMaster.RefreshToken = !string.IsNullOrEmpty(xSiteMaster.RefreshToken) ? xSiteMaster.RefreshToken.Trim() : "";
                }


                byte[] bytes = Encoding.UTF8.GetBytes(string.Format("{0}:{1}", xSiteMaster.ApplicationId, xSiteMaster.SharedSecret));
                WebHeaderCollection headers = new WebHeaderCollection();
                headers["Authorization"] = "Basic " + Convert.ToBase64String(bytes);
                headers["Cache-Control"] = "no-cache";
                string reqbody = "grant_type=refresh_token&refresh_token=" + xSiteMaster.RefreshToken;
                var apiresponse = KCApiHelper.CallWebAPI("https://api.channeladvisor.com/oauth2/token", reqbody, "POST", headers);

                if (apiresponse == null)
                {
                    PXTrace.WriteError(KCConstants.VerifyApiAccess + " - " + KCMessages.NullException);
                    response.IsSuccess = false;
                    response.Message = KCMessages.VerifyApiAccessFailed;
                }

                if (apiresponse.IsSuccess)
                {
                    var rootthread = JsonConvert.DeserializeObject<KCAPIAccessToken>(apiresponse.response);
                    if (rootthread != null)
                    {
                        response.IsSuccess = true;
                        response.Message = KCMessages.VerifyApiAccessSuccess;
                        response.Data = apiresponse.response;
                    }
                    else
                    {
                        string errormsg = string.Format(KCConstants.DualParameters, KCConstants.VerifyApiAccess, apiresponse.Message);
                        PXTrace.WriteError(errormsg);
                        response.IsSuccess = false;
                        response.Message = KCMessages.VerifyApiAccessFailed;
                    }
                }
                else
                {
                    string errormsg = string.Format(KCConstants.DualParameters, KCConstants.VerifyApiAccess, apiresponse.Message);
                    PXTrace.WriteError(errormsg);

                    response.IsSuccess = false;
                    response.Message = KCMessages.VerifyApiAccessFailed;
                }


                return response;
            }
            catch (Exception ex)
            {
                string errormsg = string.Format(KCConstants.DualParameters, KCMessages.VerifyApiAccessFailed, ex.Message);
                PXTrace.WriteError(errormsg);

                KCInternalReponse response = new KCInternalReponse();
                response.IsSuccess = false;
                response.Message = errormsg;
                return response;
            }
        }
    }
}
