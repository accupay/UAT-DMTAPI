using APT.DMT.API.BusinessObjects.Models;
using APT.DMT.API.BusinessObjects;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;
using APT.DMT.API.BusinessLogic.BankService;
using APT.DMT.API.DataAccessLayer.DMT;
using APT.PaymentServices.API.DataAccessLayer;

namespace APT.DMT.API.BusinessLogic
{
    public class CommonService
    {
        public static IConfiguration Configuration { get; set; }
        private readonly string IFSCLookupURL;
        private readonly string SessionValidationURL;
        public static LogService _logService = new LogService();
        public static CommonRepository _commRepo = new CommonRepository();
        public static CommonService _commonService = new CommonService();

        public CommonService()
        {
            Configuration = GetConfiguration();
            SessionValidationURL = Configuration["SessionValidation:Domain"] + Configuration["SessionValidation:URL"];
            IFSCLookupURL = Configuration["Cache:Domain"] + Configuration["Cache:IFSCLookupURL"];

        }

        #region Common

        public APTGenericResponse? ValidateSession(SessionValidation sessionValidationRequest)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            try
            {
                var client = new HttpClient();
                var request = SessionValidationURL;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls |
                    SecurityProtocolType.Tls11;

                HttpWebRequest webReq = WebRequest.Create(request) as HttpWebRequest;
                string postData = JsonConvert.SerializeObject(sessionValidationRequest).ToString();

                byte[] postBytes = Encoding.UTF8.GetBytes(postData);

                try
                {
                    webReq.ContentLength = postBytes.Length;
                    webReq.ContentType = "application/json;charset=utf-8";
                    webReq.Method = "POST";
                    webReq.Timeout = 1800000;
                    Stream requestStream = webReq.GetRequestStream();
                    // now send it
                    requestStream.Write(postBytes, 0, postBytes.Length);
                    requestStream.Close();

                    HttpWebResponse response;
                    response = (HttpWebResponse)webReq.GetResponse();

                    Stream responseStream = response.GetResponseStream();
                    string responsestring = new StreamReader(responseStream).ReadToEnd();
                    objresponse = JsonConvert.DeserializeObject<APTGenericResponse>(responsestring);
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
                                    objresponse = JsonConvert.DeserializeObject<APTGenericResponse>(error);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
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

                return null;
            }
            return objresponse;
        }

        public APTGenericResponse? IFSCLookup(string IFSCCode)
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", "", IFSCCode);

            APTGenericResponse objAPIresponse = new APTGenericResponse();
            try
            {
                var client = new HttpClient();
                var request = IFSCLookupURL + IFSCCode;
                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "IFSC API Request", request, IFSCCode);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls |
                    SecurityProtocolType.Tls11;

                try
                {
                    HttpWebRequest webReq = WebRequest.Create(request) as HttpWebRequest;
                    webReq.ContentType = "application/json;charset=utf-8";
                    webReq.Method = "POST";
                    webReq.Timeout = 1800000;
                    HttpWebResponse response;
                    response = (HttpWebResponse)webReq.GetResponse();

                    Stream responseStream = response.GetResponseStream();
                    string responsestring = new StreamReader(responseStream).ReadToEnd();
                    objAPIresponse = JsonConvert.DeserializeObject<APTGenericResponse>(responsestring);
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "IFSC API Response", JsonConvert.SerializeObject(objAPIresponse), IFSCCode);

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
                                    objAPIresponse = JsonConvert.DeserializeObject<APTGenericResponse>(error);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
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
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, IFSCCode);

                return null;
            }
            return objAPIresponse;
        }
        public APTGenericResponse? APTSendOTP(string otp, string mobileno)
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", otp, mobileno);

            string smscontent = "Dear%20Customer,%20" + otp + "%20is%20the%20OTP.%20NEVER%20SHARE%20THE%20OTP%20WITH%20ANYONE.%20-%20ACCUPAYD%20TECH";
            string SMSUrl = Configuration["SmsSettings:URL"];
            string Username = Configuration["SmsSettings:Username"];
            string Password = Configuration["SmsSettings:Password"];
            string from = Configuration["SmsSettings:from"];
            string to = mobileno;

            APTGenericResponse objresponse = new APTGenericResponse();
            try
            {
                var client = new HttpClient();
                var request = SMSUrl + "username=" + Username + "&password=" + Password
                    + "&to=" + to + "&from=" + from + "&content=" + smscontent;
                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "SMS API Request", request, mobileno);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls |
                    SecurityProtocolType.Tls11;
                HttpWebRequest webReq = WebRequest.Create(request) as HttpWebRequest;
                webReq.ContentType = "application/json;charset=utf-8";
                webReq.Method = "GET";
                webReq.Timeout = 1800000;
                HttpWebResponse response;
                response = (HttpWebResponse)webReq.GetResponse();

                Stream responseStream = response.GetResponseStream();
                string responsestring = new StreamReader(responseStream).ReadToEnd();
                SMSResponse objsmsresponse = JsonConvert.DeserializeObject<SMSResponse>(responsestring);
                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "SMS API Response", JsonConvert.SerializeObject(objsmsresponse), mobileno);

                if (objsmsresponse != null)
                {
                    if (objsmsresponse.status.code == "200")
                    {
                        objresponse.response_code = APIResponseCode.Success;
                        objresponse.response_message = APIResponseCodeDesc.Success;
                    }
                    else
                    {
                        objresponse.response_code = objsmsresponse.status.code;
                        objresponse.response_message = objsmsresponse.status.reason;
                    }

                }

            }
            catch (Exception ex)
            {
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, mobileno);

                return null;
            }
            return objresponse;
        }
        public IConfiguration GetConfiguration()
        {
            Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();
            return Configuration;
        }
        public bool IsValidDataSet(DataSet dst)
        {
            if (dst == null || dst.Tables.Count == 0 || dst.Tables[0].Rows.Count == 0)
            {
                return false;
            }
            return true;
        }

        public const string motif = @"[6-9][0-9]{9}";
        public bool isValidMobileNumber(string number)
        {
            if (number != null) return Regex.IsMatch(number, motif);
            else return false;
        }


        public APTGenericResponse? GetTopupInfo()
        {
            List<TopupInfoResponse> lst_topupinfo = new List<TopupInfoResponse>();
            APTGenericResponse objresponse = new APTGenericResponse();

            try
            {

                DataSet dst = _commRepo.APTTopupInfo();
                if (_commonService.IsValidDataSet(dst))
                {
                    for (int i = 0; i < dst.Tables[0].Rows.Count; i++)
                    {

                        TopupInfoResponse topupInfo = new TopupInfoResponse();
                        topupInfo.bank_name = dst.Tables[0].Rows[i]["BankName"].ToString();
                        topupInfo.account_number = dst.Tables[0].Rows[i]["Accountnumber"].ToString();
                        topupInfo.ifsc_code = dst.Tables[0].Rows[i]["Ifsccode"].ToString();
                        lst_topupinfo.Add(topupInfo);
                    }
                    objresponse.response_code = APIResponseCode.Success;
                    objresponse.response_message = APIResponseCodeDesc.Success;
                    objresponse.data = lst_topupinfo;
                }
                else
                {
                    objresponse.response_code = APIResponseCode.Failed;
                    objresponse.response_message = APIResponseCodeDesc.Failed;
                    objresponse.data = null;

                }
            }
            catch (Exception ex)
            {

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, "");

            }
            return objresponse;
        }

        public APTGenericResponse? GetSupportInfo(string AgentRefID)
        {
            SupportInfoResponse supportInfo = new SupportInfoResponse();
            APTGenericResponse objresponse = new APTGenericResponse();
            supportInfo.staff_info = new StaffInfo();
            supportInfo.company_info = new CompanyInfo();
            supportInfo.dist_info = new DistributorInfo();
            try
            {

                DataSet dst = _commRepo.APTSupportInfo(AgentRefID);
                if (_commonService.IsValidDataSet(dst))
                {
                    StaffInfo staffinfo=    new StaffInfo();
                    CompanyInfo companyinfo= new CompanyInfo(); 
                    DistributorInfo distributorinfo= new DistributorInfo();

                    if(dst.Tables.Count==3)
                    {
                        staffinfo.staff_info = dst.Tables[1].Rows[0]["StaffInfo"].ToString();
                        staffinfo.name = dst.Tables[1].Rows[0]["Name"].ToString();
                        staffinfo.mobile_number = dst.Tables[1].Rows[0]["Mobilenumber"].ToString();
                        supportInfo.staff_info  =staffinfo;

                        distributorinfo.distributor_info = dst.Tables[0].Rows[0]["DistributorInfo"].ToString();
                        distributorinfo.name = dst.Tables[0].Rows[0]["Name"].ToString();
                        distributorinfo.mobile_number = dst.Tables[0].Rows[0]["Mobilenumber"].ToString();
                        supportInfo.dist_info = distributorinfo;

                        companyinfo.company_info = dst.Tables[2].Rows[0]["CompanyInfo"].ToString();
                        companyinfo.email_id = dst.Tables[2].Rows[0]["Emailid"].ToString();
                        companyinfo.mobile_number = dst.Tables[2].Rows[0]["mobilenumber"].ToString();
                        supportInfo.company_info = companyinfo;

                        objresponse.response_code = APIResponseCode.Success;
                        objresponse.response_message = APIResponseCodeDesc.Success;
                        objresponse.data = supportInfo;
                    }
                    
                }
                else
                {
                    objresponse.response_code = APIResponseCode.Failed;
                    objresponse.response_message = APIResponseCodeDesc.Failed;
                    objresponse.data = null;

                }
            }
            catch (Exception ex)
            {

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, AgentRefID);

            }
            return objresponse;
        }

        public APTGenericResponse? sendOTPToAgent(ResendOTPRequest Request)
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.mobile_no);

            APTGenericResponse objresponse = new APTGenericResponse();
            try
            {
                Random generator = new Random();
                string OTP = generator.Next(0, 1000000).ToString("D6");
                string RefId = Request.agent_ref_id;

                APTInsertOTPRequest insertOTPRequest = new APTInsertOTPRequest();
                insertOTPRequest.mobile_no = Request.mobile_no;
                insertOTPRequest.account_ref_id = RefId;
                insertOTPRequest.account_type = Request.account_type;
                insertOTPRequest.otp = OTP;
                bool IsSuccess = SendOTPToAgent(insertOTPRequest);
                if (IsSuccess)
                {
                    objresponse.response_code = APIResponseCode.Success;
                    objresponse.response_message = "OTP Sent To Registered Number. Please validate OTP";
                    objresponse.data = null;
                }
                else
                {
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Sending OTP Failed", "Request : " + JsonConvert.SerializeObject(Request), Request.mobile_no);

                    objresponse.response_code = "104";
                    objresponse.response_message = "Sending OTP Failed";
                    objresponse.data = null;
                }

            }
            catch (Exception ex)
            {
                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.mobile_no);

            }

            return objresponse;
        }

        public bool SendOTPToAgent(APTInsertOTPRequest Request)
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.mobile_no);

            APTGenericResponse objResponse = new APTGenericResponse();
            try
            {
                Random generator = new Random();
                string OTP = generator.Next(0, 1000000).ToString("D6");
                Request.otp = OTP;

                DataSet dstinsotp = _commRepo.APTInsertOTP(Request);
                if (_commonService.IsValidDataSet(dstinsotp))
                {
                    if (dstinsotp.Tables[0].Rows[0][0].ToString() == "100" || dstinsotp.Tables[0].Rows[0][0].ToString() == "106")
                    {
                        if (dstinsotp.Tables[0].Rows[0][0].ToString() == "106")
                        {
                            OTP = dstinsotp.Tables[0].Rows[0]["OldOTP"].ToString();
                        }
                        string mobileno = APIConstants.Countrycode + Request.mobile_no;
                        objResponse = APTSendOTP(OTP, mobileno);
                        if (objResponse.response_code == APIResponseCode.Success)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Insert Failed", "", Request.mobile_no);

                }
            }
            catch (Exception ex)
            {
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.mobile_no);

                return false;

            }
            return false;
        }
        #endregion
    }
}
