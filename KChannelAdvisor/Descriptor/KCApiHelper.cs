using KChannelAdvisor.DAC.Helper;
using PX.Data;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace KChannelAdvisor.Descriptor
{
    public class KCApiHelper
    {
        #region MAKE API CALLS ---> GET,POST,PUT,PATCH
        /// <summary>
        /// MAKE API CALLS ----> GET,POST,PUT,PATCH
        /// </summary>
        /// <param name="apiurl"></param>
        /// <param name="apirequest"></param>
        /// <param name="apimethod"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public static KCAPIWebReponse CallWebAPI(string apiurl, string apirequest, string apimethod, WebHeaderCollection header, string contenttype = "application/json")
        {
            KCAPIWebReponse response = new KCAPIWebReponse();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                            | SecurityProtocolType.Tls11
                                                            | SecurityProtocolType.Tls12
                                                            | SecurityProtocolType.Ssl3;
            HttpWebRequest web_request = (HttpWebRequest)WebRequest.Create(apiurl);
            web_request.Headers = header;
            web_request.Method = apimethod;
            web_request.ContentType = contenttype;// ;
            if (!string.IsNullOrEmpty(apirequest))
            {
                byte[] byteArray = Encoding.ASCII.GetBytes(apirequest);
                web_request.ContentLength = byteArray.Length;
                Stream req_datastream = web_request.GetRequestStream();
                req_datastream.Write(byteArray, 0, byteArray.Length);
                req_datastream.Close();
            }

            try
            {
                HttpWebResponse web_response = (HttpWebResponse)web_request.GetResponse();
                Stream res_datastream = web_response.GetResponseStream();

                using (StreamReader reader = new StreamReader(res_datastream))

                {
                    string responseFromServer = reader.ReadToEnd();

                    response.httpWebRes = web_response;
                    response.response = responseFromServer;
                    response.Message = "Success";
                    response.IsSuccess = true;
                    reader.Close();
                    //res_datastream.Close();
                    web_response.Close();
                }


            }
            catch (WebException exception)
            {
                string sResponse = new StreamReader(exception.Response.GetResponseStream()).ReadToEnd();
                PXTrace.WriteInformation(sResponse);
                response.Message = exception.Message + Environment.NewLine;
                response.IsSuccess = false;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
                response.IsSuccess = false;
            }

            return response;
        }
        #endregion
    }

}
