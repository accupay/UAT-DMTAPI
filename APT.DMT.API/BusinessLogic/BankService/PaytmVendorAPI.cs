using APT.DMT.API.BusinessObjects.Models;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Reflection;
using System.Text;

namespace APT.DMT.API.BusinessLogic.BankService
{
    public class PaytmVendorAPI
    {
        private static IConfiguration Configuration { get; set; }
        public static DMTService _dmtService = new DMTService();
        public static LogService _log = new LogService();
        public static CommonService _commonService = new CommonService();
        private string PaytmDomain { get; set; } = string.Empty;
        private string PrevalidateCustomerURL { get; set; } = string.Empty;
        private string SendOTPForRegistrationURL { get; set; } = string.Empty;
        private string CustomerRegistrationURL { get; set; } = string.Empty;
        private string BeneRegistrationURL { get; set; } = string.Empty;
        private string BeneValidationURL { get; set; } = string.Empty;
        private string TransactionURL { get; set; } = string.Empty;
        private string GetBeneListURL { get; set; } = string.Empty;
        private string DeleteBeneficiaryURL { get; set; } = string.Empty;
        private string GetStatusURL { get; set; } = string.Empty;
        public PaytmVendorAPI()
        {
            Configuration = GetConfiguration();
            PaytmDomain = Configuration["PayTmValues:Domain"];
            PrevalidateCustomerURL = Configuration["PayTmValues:PrevalidateCustomerURL"];
            SendOTPForRegistrationURL = Configuration["PayTmValues:SendOTPForRegistrationURL"];
            CustomerRegistrationURL = Configuration["PayTmValues:CustomerRegistrationURL"];
            BeneRegistrationURL = Configuration["PayTmValues:BeneRegistrationURL"];
            BeneValidationURL = Configuration["PayTmValues:BeneValidationURL"];
            TransactionURL = Configuration["PayTmValues:TransactionURL"];
            GetBeneListURL = Configuration["PayTmValues:GetBeneListURL"];
            DeleteBeneficiaryURL = Configuration["PayTmValues:DeleteBeneficiaryURL"];
            GetStatusURL = Configuration["PayTmValues:StatusCheckURL"];

        }
        public IConfiguration GetConfiguration()
        {
            Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();
            return Configuration;
        }
        private string GenerateJwtToken()
        {
            // Define const Key this should be private secret key  stored in some safe place
            string PayTmSecretKey = Configuration["JwtTokenDetails:SecretKey"];
            string PayTmiss = Configuration["JwtTokenDetails:iss"];
            string PayTmpartnerId = Configuration["JwtTokenDetails:partnerId"];
            string PayTmpartnerSubId = Configuration["JwtTokenDetails:partnerSubId"];


            // Create Security key  using private key above:
            // not that latest version of JWT using Microsoft namespace instead of System
            var securityKey = new Microsoft
               .IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(PayTmSecretKey));

            // Also note that securityKey length should be >256b
            // so you have to make sure that your private key has a proper length
            //
            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials
                              (securityKey, SecurityAlgorithms.HmacSha256);

            //  Finally create a Token
            var header = new JwtHeader(credentials);
            long milliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            //Some PayLoad that contain information about the  customer
            var payload = new JwtPayload
           {
              { "iss",PayTmiss},
               { "timestamp", milliseconds},
               { "partnerId",PayTmpartnerId},
               { "partnerSubId", PayTmpartnerSubId},
               { "requestReferenceId", "12345"},

           };

            //
            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            // Token to String so you can use it in your client
            var tokenString = handler.WriteToken(secToken);


            // And finally when  you received token from client
            // you can  either validate it or try to  read
            var token = handler.ReadJwtToken(tokenString);

            return tokenString.ToString();
        }

        /// <summary>
        /// This API is used to Pre validate the customer at Paytm end
        /// </summary>
        /// <param name="CustomerMobileNumber"></param>
        /// <returns></returns>
        public PaytmPrevalidateCustomerResponse? PrevalidateCustomer(string CustomerMobileNumber)
        {
            PaytmPrevalidateCustomerResponse objAPIresponse = new PaytmPrevalidateCustomerResponse();
            try
            {
                string JwtToken = GenerateJwtToken();
                var client = new HttpClient();
                var request = PaytmDomain + PrevalidateCustomerURL + CustomerMobileNumber;
                _log.WriteToFile("PrevalidateCustomer URL :" + request + "--CustomerMobile :" + CustomerMobileNumber);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls |
                    SecurityProtocolType.Tls11;

                try
                {
                    HttpWebRequest webReq = WebRequest.Create(request) as HttpWebRequest;
                    webReq.ContentType = "application/json;charset=utf-8";
                    webReq.Method = "GET";
                    webReq.Timeout = 1800000;
                    webReq.Headers.Add("Authorization", JwtToken);
                    HttpWebResponse response;
                    response = (HttpWebResponse)webReq.GetResponse();

                    Stream responseStream = response.GetResponseStream();
                    string responsestring = new StreamReader(responseStream).ReadToEnd();
                    _log.WriteToFile("PrevalidateCustomer URL :" + request + "Response From Bank" + responsestring);
                    objAPIresponse = JsonConvert.DeserializeObject<PaytmPrevalidateCustomerResponse>(responsestring);
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
                                    _log.WriteToFile("PrevalidateCustomer URL :" + request + "Web Ex From Bank" + error);
                                    objAPIresponse = JsonConvert.DeserializeObject<PaytmPrevalidateCustomerResponse>(error);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.WriteToFile("PrevalidateCustomer URL :" + request + "Ex From Bank" + ex.Message + ex.StackTrace + ex.InnerException);
                            return null;
                        }
                    }
                    else
                    {
                        _log.WriteToFile("PrevalidateCustomer URL :" + request + "No response From Bank");
                        return null;
                    }
                }

            }
            catch (Exception ex)
            {
                _log.WriteToFile("PrevalidateCustomer:" + CustomerMobileNumber + "Ex In Method" + ex.Message + ex.StackTrace + ex.InnerException);
                return null;
            }
            return objAPIresponse;
        }

        /// <summary>
        /// This API is used to Send OTP for Customer Registration at Paytm end
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        public PaytmSendOTPResponse? SendOTPForRegistration(PaytmSendOTPRequest Request)
        {
            PaytmSendOTPResponse objAPIresponse = new PaytmSendOTPResponse();
            try
            {
                string JwtToken = GenerateJwtToken();
                var client = new HttpClient();
                var request = PaytmDomain + SendOTPForRegistrationURL;
                _log.WriteToFile("SendOTPForRegistration Entry:" + Request.customerMobile);
               
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls |
                    SecurityProtocolType.Tls11;

                HttpWebRequest webReq = WebRequest.Create(request) as HttpWebRequest;
                string postData = JsonConvert.SerializeObject(Request).ToString();
                _log.WriteToFile("SendOTPForRegistration Entry:" + Request.customerMobile + "--Request :" + postData);
               
                byte[] postBytes = Encoding.UTF8.GetBytes(postData);

                try
                {
                    webReq.ContentLength = postBytes.Length;
                    webReq.ContentType = "application/json;charset=utf-8";
                    webReq.Method = "POST";
                    webReq.Timeout = 1800000;
                    webReq.Headers.Add("Authorization", JwtToken);
                    Stream requestStream = webReq.GetRequestStream();
                    // now send it
                    requestStream.Write(postBytes, 0, postBytes.Length);
                    requestStream.Close();

                    HttpWebResponse response;
                    response = (HttpWebResponse)webReq.GetResponse();

                    Stream responseStream = response.GetResponseStream();
                    string responsestring = new StreamReader(responseStream).ReadToEnd();
                    _log.WriteToFile("SendOTPForRegistration Entry:" + Request.customerMobile + "--Request :" + postData + "--Response :" + responsestring);
                   
                    objAPIresponse = JsonConvert.DeserializeObject<PaytmSendOTPResponse>(responsestring);
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
                                    _log.WriteToFile("SendOTPForRegistration Entry:" + Request.customerMobile + "--Request :" + postData + "--Error From API :" + error);
                                   
                                    objAPIresponse = JsonConvert.DeserializeObject<PaytmSendOTPResponse>(error);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.WriteToFile("SendOTPForRegistration Entry:" + Request.customerMobile + "--Request :" + postData + "--Ex in Wex :" + ex.StackTrace + ex.InnerException + ex.Message);
                           
                            return null;
                        }
                    }
                    else
                    {
                        _log.WriteToFile("SendOTPForRegistration Entry:" + Request.customerMobile + "--Request :" + postData + "--No Response :");
                       
                        return null;
                    }
                }


            }

            catch (Exception ex)
            {
                _log.WriteToFile("SendOTPForRegistration Entry:" + Request.customerMobile + "--Ex in Method :" + ex.StackTrace + ex.InnerException + ex.Message);
               
                return null;
            }
            return objAPIresponse;
        }

        /// <summary>
        /// This API is used to do Customer Registration at Paytm end
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        public PaytmCustomerRegistrationResponse? CustomerRegistration(PaytmCustomerRegistrationRequest Request)
        {
            PaytmCustomerRegistrationResponse objAPIresponse = new PaytmCustomerRegistrationResponse();
            try
            {
                string JwtToken = GenerateJwtToken();
                var client = new HttpClient();
                var request = PaytmDomain + CustomerRegistrationURL;
                _log.WriteToFile("CustomerRegistration Entry:" + Request.customerMobile);
               
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls |
                    SecurityProtocolType.Tls11;

                HttpWebRequest webReq = WebRequest.Create(request) as HttpWebRequest;
                string postData = JsonConvert.SerializeObject(Request).ToString();
                _log.WriteToFile("CustomerRegistration URL:" + request + "---Request :" + postData);
               
                byte[] postBytes = Encoding.UTF8.GetBytes(postData);
                try
                {
                    webReq.ContentLength = postBytes.Length;
                    webReq.ContentType = "application/json;charset=utf-8";
                    webReq.Method = "POST";
                    webReq.Timeout = 1800000;
                    webReq.Headers.Add("Authorization", JwtToken);
                    Stream requestStream = webReq.GetRequestStream();
                    // now send it
                    requestStream.Write(postBytes, 0, postBytes.Length);
                    requestStream.Close();

                    HttpWebResponse response;
                    response = (HttpWebResponse)webReq.GetResponse();

                    Stream responseStream = response.GetResponseStream();
                    string responsestring = new StreamReader(responseStream).ReadToEnd();
                    _log.WriteToFile("CustomerRegistration URL:" + request + "---Request :" + postData + "---Response :" + responsestring);
                   
                    objAPIresponse = JsonConvert.DeserializeObject<PaytmCustomerRegistrationResponse>(responsestring);
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
                                    _log.WriteToFile("CustomerRegistration URL:" + request + "---Request :" + postData + "---Error Response :" + error);
                                   
                                    objAPIresponse = JsonConvert.DeserializeObject<PaytmCustomerRegistrationResponse>(error);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.WriteToFile("CustomerRegistration URL:" + request + "---Request :" + postData + "---Ex in wex :" + ex.Message + ex.InnerException + ex.StackTrace);
                           
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }

            }
            catch (Exception ex)
            {
                _log.WriteToFile("CustomerRegistration URL:" + Request.customerMobile + "---Ex in method :" + ex.Message + ex.InnerException + ex.StackTrace);
               
                return null;
            }
            return objAPIresponse;
        }

        /// <summary>
        /// This API is used to Pre validate the customer at Paytm end
        /// </summary>
        /// <param name="CustomerMobileNumber"></param>
        /// <returns></returns>
        public GetBeneListResponse? GetBeneList(string CustomerMobileNumber)
        {
            GetBeneListResponse objAPIresponse = new GetBeneListResponse();
            try
            {
                string JwtToken = GenerateJwtToken();
                var client = new HttpClient();
                var request = PaytmDomain + GetBeneListURL + CustomerMobileNumber + "&limit=1000&offset=0";
                _log.WriteToFile("GetBeneList Entry:" + CustomerMobileNumber + "--URL :" + request);
               
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls |
                    SecurityProtocolType.Tls11;

                try
                {
                    HttpWebRequest webReq = WebRequest.Create(request) as HttpWebRequest;
                    webReq.ContentType = "application/json;charset=utf-8";
                    webReq.Method = "GET";
                    webReq.Timeout = 1800000;
                    webReq.Headers.Add("Authorization", JwtToken);
                    HttpWebResponse response;
                    response = (HttpWebResponse)webReq.GetResponse();

                    Stream responseStream = response.GetResponseStream();
                    string responsestring = new StreamReader(responseStream).ReadToEnd();
                   // _log.WriteToFile("GetBeneListURL :" + request + "--Response : " + responsestring);
                    objAPIresponse = JsonConvert.DeserializeObject<GetBeneListResponse>(responsestring);
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
                                    _log.WriteToFile("GetBeneListURL :" + request + "--Error Response : " + error);
                                   
                                    objAPIresponse = JsonConvert.DeserializeObject<GetBeneListResponse>(error);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.WriteToFile("GetBeneListURL :" + request + "--Ex in wex : " + ex.StackTrace);
                           
                            return null;
                        }
                    }
                    else
                    {
                        _log.WriteToFile("GetBeneListURL :" + request + "--empty wex : " + wex.StackTrace);
                       
                        return null;
                    }
                }

            }
            catch (Exception ex)
            {
                _log.WriteToFile("GetBeneListURL :" + CustomerMobileNumber + "--ex in method : " + ex.StackTrace);
                return null;
            }
            return objAPIresponse;
        }

        /// <summary>
        /// Adding Beneficiary
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        public PaytmAddBeneResponse? BeneRegistration(PaytmAddBeneRequest Request)
        {
            PaytmAddBeneResponse objAPIresponse = new PaytmAddBeneResponse();
            try
            {
                string JwtToken = GenerateJwtToken();
                var client = new HttpClient();
                var request = PaytmDomain + BeneRegistrationURL;
                _log.WriteToFile("BeneRegistration Entry:" + Request.customerMobile + "--URL :" + request);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls |
                    SecurityProtocolType.Tls11;

                HttpWebRequest webReq = WebRequest.Create(request) as HttpWebRequest;
                string postData = JsonConvert.SerializeObject(Request).ToString();
                _log.WriteToFile("BeneRegistration URL :" + request + "--Request :" + postData);
                byte[] postBytes = Encoding.UTF8.GetBytes(postData);

                try
                {
                    webReq.ContentLength = postBytes.Length;
                    webReq.ContentType = "application/json;charset=utf-8";
                    webReq.Method = "POST";
                    webReq.Timeout = 1800000;
                    webReq.Headers.Add("Authorization", JwtToken);
                    Stream requestStream = webReq.GetRequestStream();
                    // now send it
                    requestStream.Write(postBytes, 0, postBytes.Length);
                    requestStream.Close();

                    HttpWebResponse response;
                    response = (HttpWebResponse)webReq.GetResponse();

                    Stream responseStream = response.GetResponseStream();
                    string responsestring = new StreamReader(responseStream).ReadToEnd();
                    _log.WriteToFile("BeneRegistration URL :" + request + "--Request :" + postData + "--Response :" + responsestring);
                    objAPIresponse = JsonConvert.DeserializeObject<PaytmAddBeneResponse>(responsestring);
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
                                    _log.WriteToFile("BeneRegistration URL :" + request + "--Request :" + postData + "--Error Response :" + error);
                                    objAPIresponse = JsonConvert.DeserializeObject<PaytmAddBeneResponse>(error);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.WriteToFile("BeneRegistration URL :" + request + "--Request :" + postData + "--Ex in wex:" + ex.StackTrace);
                            return null;
                        }
                    }
                    else
                    {
                        _log.WriteToFile("BeneRegistration URL :" + request + "--Request :" + postData + "--empty wex:" + wex.StackTrace);
                        return null;
                    }
                }

            }
            catch (Exception ex)
            {
                _log.WriteToFile("BeneRegistration URL :" + Request.customerMobile + "--Ex in method:" + ex.StackTrace);
                return null;
            }
            return objAPIresponse;
        }

        /// <summary>
        /// Adding Beneficiary
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        public PaytmDeletePayeeResponse? DeleteBeneficiary(PaytmDeletePayeeRequest Request)
        {
            PaytmDeletePayeeResponse objAPIresponse = new PaytmDeletePayeeResponse();
            try
            {
                string JwtToken = GenerateJwtToken();
                var client = new HttpClient();
                var request = PaytmDomain + DeleteBeneficiaryURL;
                _log.WriteToFile("DeleteBeneficiary Entry:" + Request.customerMobile + "--URL :" + request);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls |
                    SecurityProtocolType.Tls11;

                HttpWebRequest webReq = WebRequest.Create(request) as HttpWebRequest;
                string postData = JsonConvert.SerializeObject(Request).ToString();
                _log.WriteToFile("DeleteBeneficiary URL :" + request + "--Request :" + postData);
                byte[] postBytes = Encoding.UTF8.GetBytes(postData);

                try
                {
                    webReq.ContentLength = postBytes.Length;
                    webReq.ContentType = "application/json;charset=utf-8";
                    webReq.Method = "POST";
                    webReq.Timeout = 1800000;
                    webReq.Headers.Add("Authorization", JwtToken);
                    Stream requestStream = webReq.GetRequestStream();
                    // now send it
                    requestStream.Write(postBytes, 0, postBytes.Length);
                    requestStream.Close();

                    HttpWebResponse response;
                    response = (HttpWebResponse)webReq.GetResponse();

                    Stream responseStream = response.GetResponseStream();
                    string responsestring = new StreamReader(responseStream).ReadToEnd();
                    _log.WriteToFile("DeleteBeneficiary URL :" + request + "--Request :" + postData + "--Response :" + responsestring);
                    objAPIresponse = JsonConvert.DeserializeObject<PaytmDeletePayeeResponse>(responsestring);
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
                                    _log.WriteToFile("DeleteBeneficiary URL :" + request + "--Request :" + postData + "--Error Response :" + error);
                                    objAPIresponse = JsonConvert.DeserializeObject<PaytmDeletePayeeResponse>(error);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.WriteToFile("DeleteBeneficiary URL :" + request + "--Request :" + postData + "--Ex in wex:" + ex.StackTrace);
                            return null;
                        }
                    }
                    else
                    {
                        _log.WriteToFile("DeleteBeneficiary URL :" + request + "--Request :" + postData + "--empty wex:" + wex.StackTrace);
                        return null;
                    }
                }

            }
            catch (Exception ex)
            {
                _log.WriteToFile("DeleteBeneficiary URL :" + Request.customerMobile + "--Ex in method:" + ex.StackTrace);
                return null;
            }
            return objAPIresponse;
        }
        /// <summary>
        ///Bene Penny Drop API
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        public BeneValidateResponse? BeneValidation(BeneValidateRequest Request)
        {
            BeneValidateResponse objAPIresponse = new BeneValidateResponse();
            try
            {
                string JwtToken = GenerateJwtToken();
                var client = new HttpClient();
                var request = PaytmDomain + BeneValidationURL;
                _log.WriteToFile("BeneValidation Entry:" + Request.customerMobile + " ---URL :" + request);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls |
                    SecurityProtocolType.Tls11;

                HttpWebRequest webReq = WebRequest.Create(request) as HttpWebRequest;
                string postData = JsonConvert.SerializeObject(Request).ToString();
                _log.WriteToFile("BeneValidation URL :" + request + " ---Request :" + postData);
                byte[] postBytes = Encoding.UTF8.GetBytes(postData);

                try
                {
                    webReq.ContentLength = postBytes.Length;
                    webReq.ContentType = "application/json;charset=utf-8";
                    webReq.Method = "POST";
                    webReq.Timeout = 1800000;
                    webReq.Headers.Add("Authorization", JwtToken);
                    Stream requestStream = webReq.GetRequestStream();
                    // now send it
                    requestStream.Write(postBytes, 0, postBytes.Length);
                    requestStream.Close();

                    HttpWebResponse response;
                    response = (HttpWebResponse)webReq.GetResponse();

                    Stream responseStream = response.GetResponseStream();
                    string responsestring = new StreamReader(responseStream).ReadToEnd();
                    _log.WriteToFile("BeneValidation URL :" + request + " ---Request :" + postData + " ---Response :" + responsestring);
                    objAPIresponse = JsonConvert.DeserializeObject<BeneValidateResponse>(responsestring);
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
                                    _log.WriteToFile("BeneValidation URL :" + request + " ---Request :" + postData + " ---ErrorResponse :" + error);
                                    objAPIresponse = JsonConvert.DeserializeObject<BeneValidateResponse>(error);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.WriteToFile("BeneValidation URL :" + request + " ---Request :" + postData + " ---Ex in wex:" + ex.StackTrace);
                            return null;
                        }
                    }
                    else
                    {
                        _log.WriteToFile("BeneValidation URL :" + request + " ---Request :" + postData + " ---Empty wex:" + wex.StackTrace);
                        return null;
                    }
                }

            }
            catch (Exception ex)
            {
                _log.WriteToFile("BeneValidation URL :" + Request.customerMobile + "---Ex in Method:" + ex.StackTrace);
                return null;
            }
            return objAPIresponse;
        }

        /// <summary>
        /// Transaction API
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        public PaytmDMTTransactionResponse? DMTTransaction(PaytmDMTTransactionRequest Request)
        {
            PaytmDMTTransactionResponse objAPIresponse = new PaytmDMTTransactionResponse();
            try
            {
                string JwtToken = GenerateJwtToken();
                var client = new HttpClient();
                var request = PaytmDomain + TransactionURL;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls |
                    SecurityProtocolType.Tls11;

                HttpWebRequest webReq = WebRequest.Create(request) as HttpWebRequest;
                string postData = JsonConvert.SerializeObject(Request).ToString();

                byte[] postBytes = Encoding.UTF8.GetBytes(postData);

                try
                {
                    webReq.ContentLength = postBytes.Length;
                    webReq.ContentType = "application/json;charset=utf-8";
                    webReq.Method = "POST";
                    webReq.Timeout = 1800000;
                    webReq.Headers.Add("Authorization", JwtToken);
                    Stream requestStream = webReq.GetRequestStream();
                    // now send it
                    requestStream.Write(postBytes, 0, postBytes.Length);
                    requestStream.Close();

                    HttpWebResponse response;
                    response = (HttpWebResponse)webReq.GetResponse();

                    Stream responseStream = response.GetResponseStream();
                    string responsestring = new StreamReader(responseStream).ReadToEnd();
                    objAPIresponse = JsonConvert.DeserializeObject<PaytmDMTTransactionResponse>(responsestring);
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "PayTm Req: "+JsonConvert.SerializeObject(Request), responsestring, Request.txnReqId);
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
                                    objAPIresponse = JsonConvert.DeserializeObject<PaytmDMTTransactionResponse>(error);
                                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "PayTm Req Error: " + JsonConvert.SerializeObject(Request), error, Request.txnReqId);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "PayTm Req Ex Wex: " + JsonConvert.SerializeObject(Request), JsonConvert.SerializeObject(ex), Request.txnReqId);
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }

            }
            catch (Exception ex)
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "PayTm Req Ex: " + JsonConvert.SerializeObject(Request), JsonConvert.SerializeObject(ex), Request.txnReqId);
                return null;
            }
            return objAPIresponse;
        }

        /// <summary>
        /// This API is used to Pre validate the customer at Paytm end
        /// </summary>
        /// <param name="TxnID"></param>
        /// <returns></returns>
        public PaytmGetStatusResponse? GetTxnStatus(string TxnId)
        {
            PaytmGetStatusResponse objAPIresponse = new PaytmGetStatusResponse();
            try
            {
                string JwtToken = GenerateJwtToken();
                var client = new HttpClient();
                var request = PaytmDomain + GetStatusURL + TxnId;
                _log.WriteToFile("PaytmGetstatus Entry:" + TxnId + "--URL :" + request);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls |
                    SecurityProtocolType.Tls11;

                try
                {
                    HttpWebRequest webReq = WebRequest.Create(request) as HttpWebRequest;
                    webReq.ContentType = "application/json;charset=utf-8";
                    webReq.Method = "GET";
                    webReq.Timeout = 1800000;
                    webReq.Headers.Add("Authorization", JwtToken);
                    HttpWebResponse response;
                    response = (HttpWebResponse)webReq.GetResponse();

                    Stream responseStream = response.GetResponseStream();
                    string responsestring = new StreamReader(responseStream).ReadToEnd();
                    // _log.WriteToFile("GetBeneListURL :" + request + "--Response : " + responsestring);
                    objAPIresponse = JsonConvert.DeserializeObject<PaytmGetStatusResponse>(responsestring);
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
                                    _log.WriteToFile("PaytmGetStatus :" + request + "--Error Response : " + error);

                                    objAPIresponse = JsonConvert.DeserializeObject<PaytmGetStatusResponse>(error);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.WriteToFile("PaytmGetStatus :" + request + "--Ex in wex : " + ex.StackTrace);

                            return null;
                        }
                    }
                    else
                    {
                        _log.WriteToFile("PaytmGetStatus :" + request + "--empty wex : " + wex.StackTrace);

                        return null;
                    }
                }

            }
            catch (Exception ex)
            {
                _log.WriteToFile("GetBeneListURL :" + TxnId + "--ex in method : " + ex.StackTrace);
                return null;
            }
            return objAPIresponse;
        }
    }
}
