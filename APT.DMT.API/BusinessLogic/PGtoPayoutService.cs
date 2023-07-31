using APT.DMT.API.BusinessLogic;
using APT.DMT.API.BusinessLogic.BankService;
using APT.DMT.API.BusinessObjects;
using APT.DMT.API.BusinessObjects.Models;
using APT.DMT.API.Businessstrings.Models;
using APT.DMT.API.DataAccessLayer;
using APT.DMT.API.DataAccessLayer.Paytout;
using APT.PaymentServices.API.BusinessLogic.BankService;
using APT.PaymentServices.API.BusinessObjects.Models;
using APT.PaymentServices.API.DataAccessLayer;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;


namespace APT.PaymentServices.API.BusinessLogic
{
    public class PGtoPayoutService
    {
        private readonly string SessionValidationURL;

        public static IConfiguration Configuration { get; set; }
        public static CommonService _commonService = new CommonService();

        public static PgtoPayoutRepository _pgToPayoutRepo = new PgtoPayoutRepository();
        //public static PGCashFreeAPI _cashFreeAPI = new PGCashFreeAPI();

        public static LogService _logService = new LogService();

        public PGtoPayoutService()
        {
            Configuration = _commonService.GetConfiguration();
            SessionValidationURL = Configuration["SessionValidation:Domain"] + Configuration["SessionValidation:URL"];
        }

        public APTGenericResponse? InsertCustomerInfo(APTInsertCustomerRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.mobile_number);

            try
            {
                DataSet dst = _pgToPayoutRepo.InsertCustomerMaster(Request);
                if (_commonService.IsValidDataSet(dst))
                {
                    if (dst.Tables[0].Rows[0][0].ToString()=="100")
                    {
                        objresponse.response_code = "200";
                        objresponse.response_message = "Success";
                        objresponse.data = null;
                    }
                    else
                    {
                        objresponse.response_code = dst.Tables[0].Rows[0][0].ToString();
                        objresponse.response_message = dst.Tables[0].Rows[0][1].ToString();
                        objresponse.data = null;
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
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.mobile_number);
            }

            return objresponse;
        }

        public APTGenericResponse? GetCustomerInfo(APTGetCustomerInfoRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.mobile_number);
            try
            {
                DataSet dst = _pgToPayoutRepo.GetCustomer(Request);
                if (_commonService.IsValidDataSet(dst))
                {
                    if (dst.Tables[0].Rows[0][0].ToString() == "100")
                    {
                        APTGetCustInfoResponse CustInfo = new APTGetCustInfoResponse();
                        CustInfo.customer_ref_id = dst.Tables[0].Rows[0]["customerrefid"].ToString();
                        CustInfo.email_id = dst.Tables[0].Rows[0]["Emailid"].ToString();
                        CustInfo.agent_code = dst.Tables[0].Rows[0]["agentcode"].ToString();
                        CustInfo.mobile_number = dst.Tables[0].Rows[0]["Mobilenumber"].ToString();
                        CustInfo.agent_name = dst.Tables[0].Rows[0]["Agentname"].ToString();


                        objresponse.response_code = "200";
                        objresponse.response_message = "Success";
                        objresponse.data = CustInfo;
                    }
                    else
                    {
                        objresponse.response_code = dst.Tables[0].Rows[0][0].ToString();
                        objresponse.response_message = dst.Tables[0].Rows[0][1].ToString();
                        objresponse.data = null;
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
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.mobile_number);
            }

            return objresponse;
        }

        public APTGenericResponse? UploadKYCDocument(APTPGtoPayoutuploadkycRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.user_id);
            try
            {
                DataSet dst = _pgToPayoutRepo.UploadKYC(Request);
                if (_commonService.IsValidDataSet(dst))
                {
                    if (dst.Tables[0].Rows[0][0].ToString() == "100")
                    {

                        objresponse.response_code = "200";
                        objresponse.response_message = "Success";
                        objresponse.data = null;
                    }
                    else
                    {
                        objresponse.response_code = dst.Tables[0].Rows[0][0].ToString();
                        objresponse.response_message = dst.Tables[0].Rows[0][1].ToString();
                        objresponse.data = null;
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
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.user_id);
            }

            return objresponse;
        }

        public APTGenericResponse? uploadCreditCard(APTPGtoPayoutuploadCreditCardRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.user_id);
            try
            {
                DataSet dst = _pgToPayoutRepo.UploadCreditCard(Request);
                if (_commonService.IsValidDataSet(dst))
                {
                    if (dst.Tables[0].Rows[0][0].ToString() == "100")
                    {
                        objresponse.response_code = "200";
                        objresponse.response_message = "Success";
                        objresponse.data = null;
                    }
                    else
                    {
                        objresponse.response_code = dst.Tables[0].Rows[0][0].ToString();
                        objresponse.response_message = dst.Tables[0].Rows[0][1].ToString();
                        objresponse.data = null;
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
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.user_id);
            }

            return objresponse;
        }
    }
}
