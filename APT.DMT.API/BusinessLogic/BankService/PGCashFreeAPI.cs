using APT.DMT.API.BusinessLogic;
using APT.DMT.API.BusinessObjects.Models;
using APT.PaymentServices.API.BusinessObjects.Models;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Reflection;
using System.Text;
namespace APT.PaymentServices.API.BusinessLogic.BankService
{

    public class PGCashFreeAPI
    {
        private static IConfiguration Configuration { get; set; }
        public static PaymentGatewayService _dmtService = new PaymentGatewayService();
        public static LogService _log = new LogService();
        public static CommonService _commonService = new CommonService();
        private string CashFreeDomain { get; set; } = string.Empty;
        private string CreateOrGETOrderURL { get; set; } = string.Empty;
        private string SecretKey { get; set; } = string.Empty;
        private string AppID { get; set; } = string.Empty;
        private string Version { get; set; } = string.Empty;

        public PGCashFreeAPI()
        {
            Configuration = _commonService.GetConfiguration();
            CashFreeDomain = Configuration["CashFreePGValues:Domain"];
            CreateOrGETOrderURL = Configuration["CashFreePGValues:CreateOrGETOrderURL"];
            SecretKey = Configuration["CashFreePGValues:SecretKey"];
            AppID = Configuration["CashFreePGValues:AppID"];
            Version = Configuration["CashFreePGValues:Version"];

        }

        /// <summary>
        /// This API is used to Get Order Details
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        public CashFreeOrderDetailsResponse? GetOrderDetails(string orderNumber)
        {
            CashFreeOrderDetailsResponse objAPIresponse = new CashFreeOrderDetailsResponse();
            try
            {
                var client = new HttpClient();
                var request = CashFreeDomain + CreateOrGETOrderURL +"/"+ orderNumber;
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry","", orderNumber);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls |
                    SecurityProtocolType.Tls11;

                try
                {
                    HttpWebRequest webReq = WebRequest.Create(request) as HttpWebRequest;
                    webReq.ContentType = "application/json;charset=utf-8";
                    webReq.Method = "GET";
                    webReq.Timeout = 1800000;
                    webReq.Headers.Add("x-api-version", Version);
                    webReq.Headers.Add("x-client-id", AppID);
                    webReq.Headers.Add("x-client-secret", SecretKey);
                    webReq.Headers.Add("accept", "application/json");
                    HttpWebResponse response;
                    response = (HttpWebResponse)webReq.GetResponse();

                    Stream responseStream = response.GetResponseStream();
                    string responsestring = new StreamReader(responseStream).ReadToEnd();
                    _log.WriteToFile(MethodBase.GetCurrentMethod().Name + request + "|| Response From Bank" + responsestring);
                    objAPIresponse = JsonConvert.DeserializeObject<CashFreeOrderDetailsResponse>(responsestring);
                }
                catch (WebException wex)
                {
                    if (wex.Response != null)
                    {
                        try
                        {
                            using (var errorResponse = (HttpWebResponse)wex.Response)
                            {
                                using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                                {
                                    string error = reader.ReadToEnd();
                                    _log.WriteToFile(MethodBase.GetCurrentMethod().Name + request + "Web Ex From Bank" + error);
                                    objAPIresponse = JsonConvert.DeserializeObject<CashFreeOrderDetailsResponse>(error);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message ,ex.StackTrace ,orderNumber);
                            return null;
                        }
                    }
                    else
                    {
                        _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, "", "Exception", orderNumber);

                        return null;
                    }
                }

            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, orderNumber);

                return null;
            }
            return objAPIresponse;
        }

        /// <summary>
        /// This API is used to Get Order Details
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        public List<PaymentDetailsResponse>? GetPaymentDetails(string orderNumber)
        {
            List<PaymentDetailsResponse> objAPIresponse = new List<PaymentDetailsResponse>();
            try
            {
                var client = new HttpClient();
                var request = CashFreeDomain + CreateOrGETOrderURL + "/" + orderNumber+"/payments";
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", "", orderNumber);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls |
                    SecurityProtocolType.Tls11;

                try
                {
                    HttpWebRequest webReq = WebRequest.Create(request) as HttpWebRequest;
                    webReq.ContentType = "application/json;charset=utf-8";
                    webReq.Method = "GET";
                    webReq.Timeout = 1800000;
                    webReq.Headers.Add("x-api-version", Version);
                    webReq.Headers.Add("x-client-id", AppID);
                    webReq.Headers.Add("x-client-secret", SecretKey);
                    webReq.Headers.Add("accept", "application/json");
                    HttpWebResponse response;
                    response = (HttpWebResponse)webReq.GetResponse();

                    Stream responseStream = response.GetResponseStream();
                    string responsestring = new StreamReader(responseStream).ReadToEnd();
                    _log.WriteToFile(MethodBase.GetCurrentMethod().Name + request + "|| Response From Bank" + responsestring);
                    objAPIresponse = JsonConvert.DeserializeObject<List<PaymentDetailsResponse>>(responsestring);
                }
                catch (WebException wex)
                {
                    if (wex.Response != null)
                    {
                        try
                        {
                            using (var errorResponse = (HttpWebResponse)wex.Response)
                            {
                                using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                                {
                                    string error = reader.ReadToEnd();
                                    _log.WriteToFile(MethodBase.GetCurrentMethod().Name + request + "Web Ex From Bank" + error);
                                    objAPIresponse = JsonConvert.DeserializeObject<List<PaymentDetailsResponse>>(error);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, orderNumber);
                            return null;
                        }
                    }
                    else
                    {
                        _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, "", "Exception", orderNumber);

                        return null;
                    }
                }

            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, orderNumber);

                return null;
            }
            return objAPIresponse;
        }


        /// <summary>
        /// CreateOrder API
        /// </summary>
        /// <param name="ObjRequest"></param>
        /// <returns></returns>
        public CashFreeOrderDetailsResponse? CreateOrder(CashFreePGCreateOrderRequest ObjRequest)
        {
            CashFreeOrderDetailsResponse objAPIresponse = new CashFreeOrderDetailsResponse();
            try
            {
                var client = new HttpClient();
                var request = CashFreeDomain + CreateOrGETOrderURL;
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", "", ObjRequest.order_id);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls |
                    SecurityProtocolType.Tls11;

                try
                {
                    string postData = JsonConvert.SerializeObject(ObjRequest).ToString();
                    byte[] postBytes = Encoding.UTF8.GetBytes(postData);
                    _log.WriteToFile(MethodBase.GetCurrentMethod().Name + request + " || Data : " + postData);
                    HttpWebRequest webReq = WebRequest.Create(request) as HttpWebRequest;
                    webReq.ContentType = "application/json;charset=utf-8";
                    webReq.Method = "POST";
                    webReq.Timeout = 1800000;
                    webReq.Headers.Add("x-api-version", Version);
                    webReq.Headers.Add("x-client-id", AppID);
                    webReq.Headers.Add("x-client-secret", SecretKey);
                    webReq.Headers.Add("accept", "application/json");
                    Stream requestStream = webReq.GetRequestStream();
                    // now send it
                    requestStream.Write(postBytes, 0, postBytes.Length);
                    requestStream.Close();

                    HttpWebResponse response;
                    response = (HttpWebResponse)webReq.GetResponse();

                    Stream responseStream = response.GetResponseStream();
                    string responsestring = new StreamReader(responseStream).ReadToEnd();
                    objAPIresponse = JsonConvert.DeserializeObject<CashFreeOrderDetailsResponse>(responsestring);
                }
                catch (WebException wex)
                {
                    if (wex.Response != null)
                    {
                        try
                        {
                            using (var errorResponse = (HttpWebResponse)wex.Response)
                            {
                                using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                                {
                                    string error = reader.ReadToEnd();
                                    _log.WriteToFile(MethodBase.GetCurrentMethod().Name + request + "Web Ex From Bank" + error);
                                    objAPIresponse = JsonConvert.DeserializeObject<CashFreeOrderDetailsResponse>(error);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, ObjRequest.order_id);
                            return null;
                        }
                    }
                    else
                    {
                        _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, "", "Exception", ObjRequest.order_id);

                        return null;
                    }
                }

            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, ObjRequest.order_id);

                return null;
            }
            return objAPIresponse;
        }
    }
}
