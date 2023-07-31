using APT.DMT.API.BusinessLogic;
using APT.DMT.API.BusinessObjects;
using APT.DMT.API.BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.Reflection;

namespace APT.DMT.API.Controllers
{
    public class DMTController : ControllerBase
    {
        public static DMTService _dmtService = new DMTService();
        public static PayoutService _payoutService = new PayoutService();
        public static LogService _log = new LogService();
        public static CommonService _commonService = new CommonService();

        public SessionValidation ExtractFromHeader(string MobileNo)
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
        [SwaggerOperation("Get Customer Info", "Used to fetch customer details using mobile number, also used to validate customer")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/dmt/GetCustomerInfo")]
        public IActionResult? GetCustomerInfo([FromBody] APTGetCustomerInfo objRequest)
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
            SessionValidation session = ExtractFromHeader(objRequest.mobile_no);
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
                objresponse = _dmtService.APTGetCustomerInfo(objRequest);
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
        [SwaggerOperation("Register Customer API", "Register the customer in system")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion


        [HttpPost]
        [Route("api/dmt/RegisterCustomer")]
        public IActionResult? RegisterCustomer([FromBody] APTRegisterCustomer objRequest)
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

            SessionValidation session = ExtractFromHeader(objRequest.mobile_no);
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

                if (objRequest.flag == "0") //DMT
                {
                    objresponse = _dmtService.APTRegisterCustomer(objRequest);

                }
                else // PayOut
                {
                    objresponse = _payoutService.APTRegisterCustomer(objRequest);
                }

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
        [SwaggerOperation("Validate Register Customer OTP API", "Validates the OTP sent to the Customer Mobile Number")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/dmt/ValidateRegisterCustomerOTP")]
        public IActionResult? ValidateRegisterCustomerOTP([FromBody] ValidateOTPRequest objRequest)
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
            SessionValidation session = ExtractFromHeader(objRequest.mobile_no);
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

                objresponse = _dmtService.ValidateRegisterCustomerOTP(objRequest);
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
        [SwaggerOperation("Resend OTP API", "Resend the OTP sent to the Customer Mobile Number")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/dmt/ResendOTP")]
        public IActionResult? ResendOTP([FromBody] ResendOTPRequest objRequest)
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
            SessionValidation session = ExtractFromHeader(objRequest.mobile_no);
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


                objresponse = _dmtService.ResendOTP(objRequest);
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
        [SwaggerOperation("Get All Payee API", "Lists all Payee in system")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/dmt/GetAllPayee")]
        public IActionResult? GetAllPayee([FromBody] APTGetAllPayee objRequest)
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
            SessionValidation session = ExtractFromHeader(objRequest.customer_mobile_no);
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

                objresponse = _dmtService.APTGetAllPayee(objRequest);
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
        }
        #region Swagger
        [SwaggerOperation("Add Payee API", "Add the Payee in system")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/dmt/AddPayee")]
        public IActionResult? AddPayee([FromBody] APTAddPayee objRequest)
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
            SessionValidation session = ExtractFromHeader(objRequest.mobile_no);
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
                if (objRequest.flag == "0")
                {
                    objresponse = _dmtService.APTAddPayee(objRequest);
                }
                else
                {
                    objresponse = _payoutService.APTAddPayee(objRequest);
                }
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
        [SwaggerOperation("Validate Payee API", "Validate the Payee in system")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/dmt/BeneValidate")]
        public IActionResult? BeneValidate([FromBody] APTTransactionRequest objRequest)
        {
            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(objRequest), objRequest.sender_mobile_number);

            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Request Null", JsonConvert.SerializeObject(objRequest), objRequest.sender_mobile_number);

                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            //SessionValidation session = ExtractFromHeader(objRequest.sender_mobile_number);
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

                objresponse = _dmtService.APTBeneValidate(objRequest);
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
        }

        #region Swagger
        [SwaggerOperation("Send OTP For Delete Payee", "Send the OTP sent to the Customer Mobile Number")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/dmt/SendOTPForPayeeDeletion")]
        public IActionResult? SendOTPForPayeeDeletion([FromBody] ResendOTPRequest objRequest)
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
            SessionValidation session = ExtractFromHeader(objRequest.mobile_no);
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


                objresponse = _dmtService.SendOTPForPayeeDeletion(objRequest);
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
        [Route("api/dmt/DeletePayee")]
        public IActionResult? DeletePayee([FromBody] APTDeletePayee objRequest)
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
            SessionValidation session = ExtractFromHeader(objRequest.mobile_no);
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
        [SwaggerOperation("Validate Payee OTP API", "Validates the OTP sent to the Customer Mobile Number")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/dmt/ValidatePayeeOTP")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult? ValidatePayeeOTP([FromBody] APTUpdatePayeeRequest objRequest)
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
            SessionValidation session = ExtractFromHeader(objRequest.mobile_no);
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

                objresponse = _dmtService.ValidatePayeeOTP(objRequest);
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
        [SwaggerOperation("Transaction API ", "Insert the Transaction")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/dmt/Transaction")]
        public IActionResult? Transaction([FromBody] APTTransactionRequest objRequest)
        {
            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(objRequest), objRequest.sender_mobile_number);

            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Request Null", JsonConvert.SerializeObject(objRequest), objRequest.sender_mobile_number);

                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            if (!_commonService.isValidMobileNumber(objRequest.sender_mobile_number))
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Incorrect Mobile Number", JsonConvert.SerializeObject(objRequest), objRequest.sender_mobile_number);

                objresponse.response_code = APIResponseCode.InvalidRequest;
                objresponse.response_message = APIResponseCodeDesc.InvalidRequest;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            decimal amounttosend = Convert.ToDecimal(objRequest.amount);
            if (amounttosend > 25000 || amounttosend < 1)
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Invalid Amount", JsonConvert.SerializeObject(objRequest), objRequest.sender_mobile_number);

                objresponse.response_code = APIResponseCode.InvalidRequest;
                objresponse.response_message = APIResponseCodeDesc.InvalidRequest + ": Invalid Amount";
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            SessionValidation session = ExtractFromHeader(objRequest.sender_mobile_number);
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
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Session Validation Failed", JsonConvert.SerializeObject(objresponse), objRequest.agent_ref_id);

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

                objresponse = _dmtService.Transaction(objRequest);
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
        }


        #region Swagger
        [SwaggerOperation("Refund API ", "Refund the Transaction")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/dmt/Refund")]
        public IActionResult? Refund([FromBody] RefundOTPRequest objRequest)
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
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Incorrect Mobile Number", JsonConvert.SerializeObject(objRequest), objRequest.mobile_no);

                objresponse.response_code = APIResponseCode.InvalidRequest;
                objresponse.response_message = APIResponseCodeDesc.InvalidRequest;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }

            SessionValidation session = ExtractFromHeader(objRequest.mobile_no);
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

                objresponse = _dmtService.RefundTransaction(objRequest);
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
        [SwaggerOperation("Payout Check Status", "Used to check the status of Txn")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/dmt/CheckStatus")]
        public IActionResult? CheckStatus([FromBody] PaytmStatusRequest Request)
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
                objresponse = _dmtService.GetStatus(Request.transactionID);
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




    }
}
