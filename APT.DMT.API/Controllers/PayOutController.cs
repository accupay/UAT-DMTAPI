using APT.DMT.API.BusinessLogic;
using APT.DMT.API.BusinessObjects;
using APT.DMT.API.BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.Data;
using System.Reflection;

namespace APT.DMT.API.Controllers
{
    
   // [ApiController]

    public class PayOutController : ControllerBase
    {

        public static PayoutService _payoutService = new PayoutService();
        public static LogService _log = new LogService();
        public static CommonService _commonService = new CommonService();
        public static DMTService _dmtService = new DMTService();
        private static IConfiguration Configuration { get; set; }

        public PayOutController()
        {
            Configuration = GetConfiguration();
        }
        public IConfiguration GetConfiguration()
        {
            Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();
            return Configuration;
        }
        public SessionValidation ExtractFromHeaderPayout(string MobileNo)
        {
            try
            {
                SessionValidation objsession = new SessionValidation();

                objsession.MacID = Request.Headers["Macid"].ToString();
                objsession.SID = Request.Headers["SID"].ToString();
                if (!string.IsNullOrWhiteSpace(objsession.SID) && !string.IsNullOrEmpty(objsession.MacID))
                {
                    return objsession;
                }
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Incorrect Header Values", Request.Headers.ToString(), MobileNo);
                return null;

            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, MobileNo);
                return null;
            }
        }

        #region Swagger
        [SwaggerOperation("Payout Fund Transfer", "Used to transfer the payment using payout API's")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/PayOut/FundTransfer")]
        public IActionResult? FundTransfer([FromBody] PayoutPaymentRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            if (Request == null)
            {
                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            try
            {
                objresponse = _payoutService.PayoutFundTransfer(Request);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    return Ok(objresponse);
                }
                else
                {
                    return UnprocessableEntity(objresponse);
                }
            }
            catch (Exception ex)
            {
                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }


        }
        #region Swagger
        [SwaggerOperation("Payout Check Status", "Used to check the status of Txn")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/PayOut/CheckStatus")]
        public IActionResult? CheckStatus(PayoutStatusRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            if (Request == null)
            {
                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            try
            {
                objresponse = _payoutService.PayoutCheckStatus(Request);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    return Ok(objresponse);
                }
                else
                {
                    return UnprocessableEntity(objresponse);
                }
            }
            catch (Exception e)
            {
                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }


        }

        [HttpPost]
        [Route("api/PayOut/RegisterCustomer_payout")]
        public IActionResult? RegisterCustomer_payout([FromBody] APTRegisterCustomer objRequest)
        {
            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(objRequest), objRequest.mobile_no);

            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Request Null", JsonConvert.SerializeObject(objRequest), objRequest.mobile_no);

                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            if (!_commonService.isValidMobileNumber(objRequest.mobile_no))
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Invalid Mobile No", JsonConvert.SerializeObject(objRequest), objRequest.mobile_no);

                objresponse.response_code = APIResponseCode.InvalidRequest;
                objresponse.response_message = APIResponseCodeDesc.InvalidRequest;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }

            SessionValidation session = ExtractFromHeaderPayout(objRequest.mobile_no);
            if (session == null)
            {

                objresponse.response_code = "401";
                objresponse.response_message = "Header is empty. Please pass session values";
                objresponse.data = null;
                return Unauthorized(objresponse);
            }
            else
            {
                objresponse = _commonService.ValidateSession(session);
                if (objresponse == null || objresponse.response_code != "100")
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Session Validation Failed", JsonConvert.SerializeObject(objresponse), objRequest.mobile_no);

                    objresponse.response_code = "401";
                    objresponse.response_message = "Invalid Session Values.";
                    objresponse.data = null;
                    return Unauthorized(objresponse);
                }
            }
            try
            {
                string sessionresponse = JsonConvert.SerializeObject(objresponse.data);
                LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(sessionresponse);
               
                objresponse = _payoutService.APTRegisterCustomer(objRequest);

                if (objresponse.response_code == APIResponseCode.Success || objresponse.response_code == "201")
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Success", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.mobile_no);

                    objresponse.session_details = loginResponse;
                    return Ok(objresponse);
                }
                else
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.mobile_no);

                    return UnprocessableEntity(objresponse);
                }
            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, objRequest.mobile_no);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
        } //Done

        #region Swagger
        [SwaggerOperation("Validate Register Customer OTP API", "Validates the OTP sent to the Customer Mobile Number")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/PayOut/ValidateRegisterCustomerOTP_payout")]
        public IActionResult? ValidateRegisterCustomerOTP_payout([FromBody] ValidateOTPRequest objRequest)
        {
            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(objRequest), objRequest.mobile_no);

            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Request Null", JsonConvert.SerializeObject(objRequest), objRequest.mobile_no);

                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            if (!_commonService.isValidMobileNumber(objRequest.mobile_no))
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Invalid Mobile No", JsonConvert.SerializeObject(objRequest), objRequest.mobile_no);

                objresponse.response_code = APIResponseCode.InvalidRequest;
                objresponse.response_message = APIResponseCodeDesc.InvalidRequest;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            SessionValidation session = ExtractFromHeaderPayout(objRequest.mobile_no);
            if (session == null)
            {

                objresponse.response_code = "401";
                objresponse.response_message = "Header is empty. Please pass session values";
                objresponse.data = null;
                return Unauthorized(objresponse);
            }
            else
            {
                objresponse = _commonService.ValidateSession(session);
                if (objresponse == null || objresponse.response_code != "100")
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Session Validation Failed", JsonConvert.SerializeObject(objresponse), objRequest.mobile_no);

                    objresponse.response_code = "401";
                    objresponse.response_message = "Invalid Session Values.";
                    objresponse.data = null;
                    return Unauthorized(objresponse);
                }
            }
            try
            {
                string sessionresponse = JsonConvert.SerializeObject(objresponse.data);
                LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(sessionresponse);

                objresponse = _payoutService.ValidateRegisterCustomerOTP(objRequest);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Success", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.mobile_no);

                    objresponse.session_details = loginResponse;
                    return Ok(objresponse);
                }
                else
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.mobile_no);

                    return UnprocessableEntity(objresponse);
                }
            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, objRequest.mobile_no);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
        } //Done

        #region Swagger
        [SwaggerOperation("Resend OTP API", "Resend the OTP sent to the Customer Mobile Number")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/PayOut/ResendOTP_payout")]
        public IActionResult? ResendOTP_payout([FromBody] ResendOTPRequest objRequest)
        {
            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(objRequest), objRequest.mobile_no);

            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Request Null", JsonConvert.SerializeObject(objRequest), objRequest.mobile_no);

                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            //if (!_commonService.isValidMobileNumber(objRequest.mobile_no))
            //{
            //    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Invalid Mobile no", JsonConvert.SerializeObject(objRequest), objRequest.mobile_no);

            //    objresponse.response_code = APIResponseCode.InvalidRequest;
            //    objresponse.response_message = APIResponseCodeDesc.InvalidRequest;
            //    objresponse.data = null;
            //    return UnprocessableEntity(objresponse);
            //}
            SessionValidation session = ExtractFromHeaderPayout(objRequest.mobile_no);
            if (session == null)
            {

                objresponse.response_code = "401";
                objresponse.response_message = "Header is empty. Please pass session values";
                objresponse.data = null;
                return Unauthorized(objresponse);
            }
            else
            {
                objresponse = _commonService.ValidateSession(session);
                if (objresponse == null || objresponse.response_code != "100")
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Session Validation Failed", JsonConvert.SerializeObject(objresponse), objRequest.mobile_no);

                    objresponse.response_code = "401";
                    objresponse.response_message = "Invalid Session Values.";
                    objresponse.data = null;
                    return Unauthorized(objresponse);
                }
            }
            try
            {
                string sessionresponse = JsonConvert.SerializeObject(objresponse.data);
                LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(sessionresponse);


                objresponse = _payoutService.ResendOTP(objRequest);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Success", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.mobile_no);

                    objresponse.session_details = loginResponse;
                    return Ok(objresponse);
                }
                else
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.mobile_no);

                    return UnprocessableEntity(objresponse);
                }
            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, objRequest.mobile_no);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
        } //Done

        #region Swagger
        [SwaggerOperation("Get All Payee API", "Lists all Payee in system")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/PayOut/GetAllPayee_payout")]
        public IActionResult? GetAllPayee_payout([FromBody] APTGetAllPayee objRequest)
        {
            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(objRequest), objRequest.customer_mobile_no);

            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Request Null", JsonConvert.SerializeObject(objRequest), objRequest.customer_mobile_no);

                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            SessionValidation session = ExtractFromHeaderPayout(objRequest.customer_mobile_no);
            if (session == null)
            {

                objresponse.response_code = "401";
                objresponse.response_message = "Header is empty. Please pass session values";
                objresponse.data = null;
                return Unauthorized(objresponse);
            }
            else
            {
                objresponse = _commonService.ValidateSession(session);
                if (objresponse == null || objresponse.response_code != "100")
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Session Validation Failed", JsonConvert.SerializeObject(objresponse), objRequest.customer_mobile_no);

                    objresponse.response_code = "401";
                    objresponse.response_message = "Invalid Session Values.";
                    objresponse.data = null;
                    return Unauthorized(objresponse);
                }
            }
            try
            {
                string sessionresponse = JsonConvert.SerializeObject(objresponse.data);
                LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(sessionresponse);

                objresponse = _payoutService.APTGetAllPayee(objRequest);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    //_log.WriteLog(MethodBase.GetCurrentMethod().Name, "Success", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.customer_mobile_no);

                    objresponse.session_details = loginResponse;
                    return Ok(objresponse);
                }
                else
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.customer_mobile_no);

                    return UnprocessableEntity(objresponse);
                }

            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, objRequest.customer_mobile_no);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
        } //Done
        #region Swagger
        [SwaggerOperation("Add Payee API", "Add the Payee in system")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/PayOut/AddPayee_payout")]
        public IActionResult? AddPayee_payout([FromBody] APTAddPayee objRequest)
        {
            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(objRequest), objRequest.mobile_no);

            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Request Null", JsonConvert.SerializeObject(objRequest), objRequest.mobile_no);

                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            SessionValidation session = ExtractFromHeaderPayout(objRequest.mobile_no);
            if (session == null)
            {

                objresponse.response_code = "401";
                objresponse.response_message = "Header is empty. Please pass session values";
                objresponse.data = null;
                return Unauthorized(objresponse);
            }
            else
            {
                objresponse = _commonService.ValidateSession(session);
                if (objresponse == null || objresponse.response_code != "100")
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Session Validation Failed", JsonConvert.SerializeObject(objresponse), objRequest.mobile_no);

                    objresponse.response_code = "401";
                    objresponse.response_message = "Invalid Session Values.";
                    objresponse.data = null;
                    return Unauthorized(objresponse);
                }
            }
            try
            {
                string sessionresponse = JsonConvert.SerializeObject(objresponse.data);
                LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(sessionresponse);
                
                objresponse = _payoutService.APTAddPayee(objRequest);
                
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Success", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.mobile_no);

                    objresponse.session_details = loginResponse;
                    return Ok(objresponse);
                }
                else
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.mobile_no);

                    return UnprocessableEntity(objresponse);
                }

            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, objRequest.mobile_no);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
        } //Done


        #region Swagger
        [SwaggerOperation("Validate Payee API", "Validate the Payee in system")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/PayOut/BeneValidate_payout")]
        public IActionResult? BeneValidate_payout([FromBody] APTTransactionRequest objRequest)
        {
            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(objRequest), objRequest.sender_mobile_number);

            APTGenericResponse objresponse = new APTGenericResponse();
            //if (objRequest == null)
            //{
            //    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Request Null", JsonConvert.SerializeObject(objRequest), objRequest.sender_mobile_number);

            //    objresponse.response_code = APIResponseCode.RequestNull;
            //    objresponse.response_message = APIResponseCodeDesc.RequestNull;
            //    objresponse.data = null;
            //    return UnprocessableEntity(objresponse);
            //}
            //SessionValidation session = ExtractFromHeaderPayout(objRequest.sender_mobile_number);
            //if (session == null)
            //{

            //    objresponse.response_code = "401";
            //    objresponse.response_message = "Header is empty. Please pass session values";
            //    objresponse.data = null;
            //    return Unauthorized(objresponse);
            //}
            //else
            //{
            //    objresponse = _commonService.ValidateSession(session);
            //    if (objresponse == null || objresponse.response_code != "100")
            //    {
            //        _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Session Validation Failed", JsonConvert.SerializeObject(objresponse), objRequest.sender_mobile_number);

            //        objresponse.response_code = "401";
            //        objresponse.response_message = "Invalid Session Values.";
            //        objresponse.data = null;
            //        return Unauthorized(objresponse);
            //    }
            //}
            try
            {
                string sessionresponse = JsonConvert.SerializeObject(objresponse.data);
                LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(sessionresponse);
                string BVDecider = Configuration["PayTmValues:BVPayOutDecider"];
                
                if(BVDecider != "1" || objRequest.agent_ref_id == "7002")
                {
                    PayoutPaymentRequest Request = new PayoutPaymentRequest();
                    Request.ARefID = objRequest.agent_ref_id;
                    Request.Accountnumerin = objRequest.account_number_in;
                    Request.Agentmobile = objRequest.agent_mobile;
                    Request.AgentRemarks = "";
                    Request.sendermobilenumber = objRequest.sender_mobile_number;
                    Request.Amount = "1";
                    Request.PaymentTransactionTypeRefID = "3";
                    Request.ifsccodein = objRequest.ifsc_code;
                    Request.PayModeRefID = "1";
                    Request.PayeeRefID = objRequest.payee_ref_id;
                    Request.MacID = "";
                    Request.agent_ip = "";
                    objresponse = _payoutService.PayoutFundTransfer_BeneValidate(Request);
                    if (objresponse.response_code == "200")
                    {
                        string J = JsonConvert.SerializeObject(objresponse.data); ;
                        APTPayOutTransactionResponse Response = JsonConvert.DeserializeObject<APTPayOutTransactionResponse>(J);

                        APTBeneValidateResponse objdata = new APTBeneValidateResponse();

                        objdata.customerMobile = Request.sendermobilenumber; var obj = objresponse.data;
                        objdata.beneficiaryName = Response.bene_name;
                        objdata.transactionDate = "";
                        objresponse.response_code = APIResponseCode.Success;
                        objresponse.response_message = "Bene Validated Successfully";
                        objresponse.data = objdata;
                    }
                }
                else if(BVDecider == "1")
                {
                    objresponse = _payoutService.APTBeneValidate_PaytmPayout(objRequest);
                }
                
                
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Success", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.sender_mobile_number);

                    objresponse.session_details = loginResponse;
                    return Ok(objresponse);
                }
                else
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.sender_mobile_number);

                    return UnprocessableEntity(objresponse);
                }

            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, objRequest.sender_mobile_number);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
        } //Done

        #region Swagger
        [SwaggerOperation("Send OTP For Delete Payee", "Send the OTP sent to the Customer Mobile Number")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/PayOut/SendOTPForPayeeDeletion_payout")]
        public IActionResult? SendOTPForPayeeDeletion_payout([FromBody] ResendOTPRequest objRequest)
        {
            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(objRequest), objRequest.mobile_no);

            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Request Null", JsonConvert.SerializeObject(objRequest), objRequest.mobile_no);

                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            if (!_commonService.isValidMobileNumber(objRequest.mobile_no))
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Invalid Mobile no", JsonConvert.SerializeObject(objRequest), objRequest.mobile_no);

                objresponse.response_code = APIResponseCode.InvalidRequest;
                objresponse.response_message = APIResponseCodeDesc.InvalidRequest;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            SessionValidation session = ExtractFromHeaderPayout(objRequest.mobile_no);
            if (session == null)
            {

                objresponse.response_code = "401";
                objresponse.response_message = "Header is empty. Please pass session values";
                objresponse.data = null;
                return Unauthorized(objresponse);
            }
            else
            {
                objresponse = _commonService.ValidateSession(session);
                if (objresponse == null || objresponse.response_code != "100")
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Session Validation Failed", JsonConvert.SerializeObject(objresponse), objRequest.mobile_no);

                    objresponse.response_code = "401";
                    objresponse.response_message = "Invalid Session Values.";
                    objresponse.data = null;
                    return Unauthorized(objresponse);
                }
            }
            try
            {
                string sessionresponse = JsonConvert.SerializeObject(objresponse.data);
                LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(sessionresponse);


                objresponse = _payoutService.SendOTPForPayeeDeletion(objRequest);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Success", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.mobile_no);

                    objresponse.session_details = loginResponse;
                    return Ok(objresponse);
                }
                else
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.mobile_no);

                    return UnprocessableEntity(objresponse);
                }
            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, objRequest.mobile_no);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
        }

        #region Swagger
        [SwaggerOperation("Delete Payee", "Deletes the Payee in system")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/PayOut/DeletePayee_payout")]
        public IActionResult? DeletePayee_payout([FromBody] APTDeletePayee objRequest)
        {
            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(objRequest), objRequest.mobile_no);

            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Request Null", JsonConvert.SerializeObject(objRequest), objRequest.mobile_no);

                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            if (!_commonService.isValidMobileNumber(objRequest.mobile_no))
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Invalid Mobile no", JsonConvert.SerializeObject(objRequest), objRequest.mobile_no);

                objresponse.response_code = APIResponseCode.InvalidRequest;
                objresponse.response_message = APIResponseCodeDesc.InvalidRequest;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            SessionValidation session = ExtractFromHeaderPayout(objRequest.mobile_no);
            if (session == null)
            {

                objresponse.response_code = "401";
                objresponse.response_message = "Header is empty. Please pass session values";
                objresponse.data = null;
                return Unauthorized(objresponse);
            }
            else
            {
                objresponse = _commonService.ValidateSession(session);
                if (objresponse == null || objresponse.response_code != "100")
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Session Validation Failed", JsonConvert.SerializeObject(objresponse), objRequest.mobile_no);

                    objresponse.response_code = "401";
                    objresponse.response_message = "Invalid Session Values.";
                    objresponse.data = null;
                    return Unauthorized(objresponse);
                }
            }
            try
            {
                string sessionresponse = JsonConvert.SerializeObject(objresponse.data);
                LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(sessionresponse);


                objresponse = _dmtService.DeletePayee(objRequest);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Success", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.mobile_no);

                    objresponse.session_details = loginResponse;
                    return Ok(objresponse);
                }
                else
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.mobile_no);

                    return UnprocessableEntity(objresponse);
                }
            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, objRequest.mobile_no);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
        }

        #region Swagger
        [SwaggerOperation("Get Customer Info", "Used to fetch customer details using mobile number, also used to validate customer")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/PayOut/GetCustomerInfo_payout")]
        public IActionResult? GetCustomerInfo_payout([FromBody] APTGetCustomerInfo objRequest)
        {
            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(objRequest), objRequest.mobile_no);

            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Request Null", JsonConvert.SerializeObject(objRequest), objRequest.mobile_no);

                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            if (!_commonService.isValidMobileNumber(objRequest.mobile_no))
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Invalid Mobile Number", JsonConvert.SerializeObject(objRequest), objRequest.mobile_no);

                objresponse.response_code = APIResponseCode.InvalidRequest;
                objresponse.response_message = APIResponseCodeDesc.InvalidRequest;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            SessionValidation session = ExtractFromHeaderPayout(objRequest.mobile_no);
            if (session == null)
            {

                objresponse.response_code = "401";
                objresponse.response_message = "Header is empty. Please pass session values";
                objresponse.data = null;
                return Unauthorized(objresponse);
            }
            else
            {
                objresponse = _commonService.ValidateSession(session);
                if (objresponse == null || objresponse.response_code != "100")
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Session Validation Failed", JsonConvert.SerializeObject(objresponse), objRequest.mobile_no);

                    objresponse.response_code = "401";
                    objresponse.response_message = "Invalid Session Values.";
                    objresponse.data = null;
                    return Unauthorized(objresponse);
                }
            }
            try
            {
                string sessionresponse = JsonConvert.SerializeObject(objresponse.data);
                LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(sessionresponse);
                objresponse = _payoutService.APTGetCustomerInfo(objRequest);
                if (objresponse.response_code == APIResponseCode.Success || objresponse.response_code == "201")
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Success", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.mobile_no);

                    objresponse.session_details = loginResponse;
                    return Ok(objresponse);
                }
                else
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.mobile_no);

                    return UnprocessableEntity(objresponse);
                }

            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, objRequest.mobile_no);
                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
        }


        #region Swagger
        [SwaggerOperation("Get Pin", "Get the status if pin needs to be updated or not")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/PayOut/GetPin_payout")]
        public IActionResult? GetPin([FromBody] Getpin_PayoutRequest objRequest)
        {
            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(objRequest), objRequest.retailer_number);

            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Request Null", JsonConvert.SerializeObject(objRequest), objRequest.retailer_number);

                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            //if (!_commonService.isValidMobileNumber(objRequest.retailer_number))
            //{
            //    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Invalid Mobile no", JsonConvert.SerializeObject(objRequest), objRequest.retailer_number);

            //    objresponse.response_code = APIResponseCode.InvalidRequest;
            //    objresponse.response_message = APIResponseCodeDesc.InvalidRequest;
            //    objresponse.data = null;
            //    return UnprocessableEntity(objresponse);
            //}
            SessionValidation session = ExtractFromHeaderPayout(objRequest.retailer_number);
            if (session == null)
            {

                objresponse.response_code = "401";
                objresponse.response_message = "Header is empty. Please pass session values";
                objresponse.data = null;
                return Unauthorized(objresponse);
            }
            else
            {
                objresponse = _commonService.ValidateSession(session);
                if (objresponse == null || objresponse.response_code != "100")
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Session Validation Failed", JsonConvert.SerializeObject(objresponse), objRequest.retailer_number);

                    objresponse.response_code = "401";
                    objresponse.response_message = "Invalid Session Values.";
                    objresponse.data = null;
                    return Unauthorized(objresponse);
                }
            }
            try
            {
                string sessionresponse = JsonConvert.SerializeObject(objresponse.data);
                LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(sessionresponse);


                objresponse = _payoutService.APTPayoutGetPin(objRequest);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Success", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.retailer_number);

                    objresponse.session_details = loginResponse;
                    return Ok(objresponse);
                }
                else
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.retailer_number);

                    return UnprocessableEntity(objresponse);
                }
            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, objRequest.retailer_number);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
        }


        #region Swagger
        [SwaggerOperation("Verify Pin", "Verification of pin")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/PayOut/VerifyPin_payout")]
        public IActionResult? VerifyPin([FromBody] Verifypin_PayoutRequest objRequest)
        {
            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(objRequest), objRequest.retailer_number);

            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Request Null", JsonConvert.SerializeObject(objRequest), objRequest.retailer_number);

                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            //if (!_commonService.isValidMobileNumber(objRequest.retailer_number))
            //{
            //    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Invalid Mobile no", JsonConvert.SerializeObject(objRequest), objRequest.retailer_number);

            //    objresponse.response_code = APIResponseCode.InvalidRequest;
            //    objresponse.response_message = APIResponseCodeDesc.InvalidRequest;
            //    objresponse.data = null;
            //    return UnprocessableEntity(objresponse);
            //}
            SessionValidation session = ExtractFromHeaderPayout(objRequest.retailer_number);
            if (session == null)
            {

                objresponse.response_code = "401";
                objresponse.response_message = "Header is empty. Please pass session values";
                objresponse.data = null;
                return Unauthorized(objresponse);
            }
            else
            {
                objresponse = _commonService.ValidateSession(session);
                if (objresponse == null || objresponse.response_code != "100")
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Session Validation Failed", JsonConvert.SerializeObject(objresponse), objRequest.retailer_number);

                    objresponse.response_code = "401";
                    objresponse.response_message = "Invalid Session Values.";
                    objresponse.data = null;
                    return Unauthorized(objresponse);
                }
            }
            try
            {
                string sessionresponse = JsonConvert.SerializeObject(objresponse.data);
                LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(sessionresponse);


                objresponse = _payoutService.VerifyPin(objRequest);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Success", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.retailer_number);

                    objresponse.session_details = loginResponse;
                    return Ok(objresponse);
                }
                else
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.retailer_number);

                    return UnprocessableEntity(objresponse);
                }
            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, objRequest.retailer_number);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
        }


        #region Swagger
        [SwaggerOperation("Verify Pin", "Verification of pin")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/PayOut/UpdatePin_payout")]
        public IActionResult? UpdatePin([FromBody] Insertpin_PayoutRequest objRequest)
        {
            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(objRequest), objRequest.retailer_number);

            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Request Null", JsonConvert.SerializeObject(objRequest), objRequest.retailer_number);

                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            //if (!_commonService.isValidMobileNumber(objRequest.retailer_number))
            //{
            //    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Invalid Mobile no", JsonConvert.SerializeObject(objRequest), objRequest.retailer_number);

            //    objresponse.response_code = APIResponseCode.InvalidRequest;
            //    objresponse.response_message = APIResponseCodeDesc.InvalidRequest;
            //    objresponse.data = null;
            //    return UnprocessableEntity(objresponse);
            //}
            SessionValidation session = ExtractFromHeaderPayout(objRequest.retailer_number);
            if (session == null)
            {

                objresponse.response_code = "401";
                objresponse.response_message = "Header is empty. Please pass session values";
                objresponse.data = null;
                return Unauthorized(objresponse);
            }
            else
            {
                objresponse = _commonService.ValidateSession(session);
                if (objresponse == null || objresponse.response_code != "100")
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Session Validation Failed", JsonConvert.SerializeObject(objresponse), objRequest.retailer_number);

                    objresponse.response_code = "401";
                    objresponse.response_message = "Invalid Session Values.";
                    objresponse.data = null;
                    return Unauthorized(objresponse);
                }
            }
            try
            {
                string sessionresponse = JsonConvert.SerializeObject(objresponse.data);
                LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(sessionresponse);


                objresponse = _payoutService.InsertUpdatePin(objRequest);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Success", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.retailer_number);

                    objresponse.session_details = loginResponse;
                    return Ok(objresponse);
                }
                else
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.retailer_number);

                    return UnprocessableEntity(objresponse);
                }
            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, objRequest.retailer_number);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
        }


        #region Swagger
        [SwaggerOperation("UpdateCustomerKYC_payout API Transaction", "Update CustomerKYC payout Transaction")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/PayOut/UpdateCustomerKYC_payout")]
        public IActionResult? UpdateCustomerKYC_payout([FromBody] APTUpdateCustomerKYCPayoutRequest objRequest)
        {
            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(objRequest), objRequest.transaction_id);

            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Request Null", JsonConvert.SerializeObject(objRequest), objRequest.transaction_id);

                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            //if (!_commonService.isValidMobileNumber(objRequest.retailer_number))
            //{
            //    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Invalid Mobile no", JsonConvert.SerializeObject(objRequest), objRequest.retailer_number);

            //    objresponse.response_code = APIResponseCode.InvalidRequest;
            //    objresponse.response_message = APIResponseCodeDesc.InvalidRequest;
            //    objresponse.data = null;
            //    return UnprocessableEntity(objresponse);
            //}
            //SessionValidation session = ExtractFromHeaderPayout(objRequest.retailer_number);
            //if (session == null)
            //{

            //    objresponse.response_code = "401";
            //    objresponse.response_message = "Header is empty. Please pass session values";
            //    objresponse.data = null;
            //    return Unauthorized(objresponse);
            //}
            //else
            //{
            //    objresponse = _commonService.ValidateSession(session);
            //    if (objresponse == null || objresponse.response_code != "100")
            //    {
            //        _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Session Validation Failed", JsonConvert.SerializeObject(objresponse), objRequest.retailer_number);

            //        objresponse.response_code = "401";
            //        objresponse.response_message = "Invalid Session Values.";
            //        objresponse.data = null;
            //        return Unauthorized(objresponse);
            //    }
            //}
            try
            {
                string sessionresponse = JsonConvert.SerializeObject(objresponse.data);
                LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(sessionresponse);


                objresponse = _payoutService.APTUpdateCustomerKYC_Payout(objRequest);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Success", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.transaction_id);

                    objresponse.session_details = loginResponse;
                    return Ok(objresponse);
                }
                else
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.transaction_id);

                    return UnprocessableEntity(objresponse);
                }
            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, objRequest.transaction_id);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
        }


        #region Swagger
        [SwaggerOperation("UpdateCustomerKYC_payout API Transaction", "Update CustomerKYC payout Transaction")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/PayOut/AV_FT")]
        public IActionResult? AV_FT(string Amount, string mode, string passcode)
        {
            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", Amount +"---"+ mode + "---" + passcode, "");

            APTGenericResponse objresponse = new APTGenericResponse();
            if (passcode != "PasVig_Acc123")
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Failure", Amount + "---" + mode + "---" + passcode, "");

                objresponse.response_code = "203";
                objresponse.response_message = "Wrong Passcode";
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
           
            try
            {
                if(IsDigitsOnly(Amount))
                {

                }
                else
                {
                    objresponse.response_code = "204";
                    objresponse.response_message = "Invalid Amount";
                    objresponse.data = null;
                    return UnprocessableEntity(objresponse);
                }

                if(mode == "1" || mode == "2"|| mode == "3")
                {

                }
                else
                {
                    objresponse.response_code = "204";
                    objresponse.response_message = "Invalid Mode only 1,2,3 allowed";
                    objresponse.data = null;
                    return UnprocessableEntity(objresponse);
                }

                objresponse = _payoutService.APTPayoutFundTransfer_v(Amount , mode);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Success", "Request : " + "" + "|Response :" + JsonConvert.SerializeObject(objresponse), "");

                   // objresponse.session_details = loginResponse;
                    return Ok(objresponse);
                }
                else
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", "Request : " + "" + "|Response :" + JsonConvert.SerializeObject(objresponse), "");

                    return UnprocessableEntity(objresponse);
                }
            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, "");

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
        }

        #region Swagger
        [SwaggerOperation("FundTransferCustomer API Transaction", "Fund Transfer Customer payout Transaction")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/PayOut/FundTransferCustomer")]
        public IActionResult? FundTransferCustomer([FromBody] PayOutPGtoPayoutTxnRequest objRequest)
        {
            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(objRequest), objRequest.transactionID);

            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Request Null", JsonConvert.SerializeObject(objRequest), objRequest.transactionID);

                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            //if (!_commonService.isValidMobileNumber(objRequest.retailer_number))
            //{
            //    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Invalid Mobile no", JsonConvert.SerializeObject(objRequest), objRequest.retailer_number);

            //    objresponse.response_code = APIResponseCode.InvalidRequest;
            //    objresponse.response_message = APIResponseCodeDesc.InvalidRequest;
            //    objresponse.data = null;
            //    return UnprocessableEntity(objresponse);
            //}
            //SessionValidation session = ExtractFromHeaderPayout(objRequest.retailer_number);
            //if (session == null)
            //{

            //    objresponse.response_code = "401";
            //    objresponse.response_message = "Header is empty. Please pass session values";
            //    objresponse.data = null;
            //    return Unauthorized(objresponse);
            //}
            //else
            //{
            //    objresponse = _commonService.ValidateSession(session);
            //    if (objresponse == null || objresponse.response_code != "100")
            //    {
            //        _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Session Validation Failed", JsonConvert.SerializeObject(objresponse), objRequest.retailer_number);

            //        objresponse.response_code = "401";
            //        objresponse.response_message = "Invalid Session Values.";
            //        objresponse.data = null;
            //        return Unauthorized(objresponse);
            //    }
            //}
            try
            {
                string sessionresponse = JsonConvert.SerializeObject(objresponse.data);
                LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(sessionresponse);


                objresponse = _payoutService.APTPGtoPayoutTxn(objRequest);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Success", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.transactionID);

                    objresponse.session_details = loginResponse;
                    return Ok(objresponse);
                }
                else
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.transactionID);

                    return UnprocessableEntity(objresponse);
                }
            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, objRequest.transactionID);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
        }

        bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

    }
}
