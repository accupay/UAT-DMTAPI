using APT.DMT.API.BusinessLogic;
using APT.DMT.API.BusinessObjects;
using APT.DMT.API.BusinessObjects.Models;
using APT.PaymentServices.API.BusinessLogic;
using APT.PaymentServices.API.BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.Reflection;

namespace APT.PaymentServices.API.Controllers
{
    public class CustomerAppController : ControllerBase
    {
        public static CustomerAppService _custAppService = new CustomerAppService();

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
        [SwaggerOperation("Get All Payee API", "Lists all Payee in system")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/CustApp/GetAllPayee")]
        public IActionResult? GetAllPayee([FromBody] APTGetAllPayeeCustomerAppReq objRequest)
        {
            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(objRequest), objRequest.Customer_ref_id);

            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Request Null", JsonConvert.SerializeObject(objRequest), objRequest.Customer_ref_id);

                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            SessionValidation session = ExtractFromHeader(objRequest.Customer_ref_id);
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
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Session Validation Failed", JsonConvert.SerializeObject(objresponse), objRequest.Customer_ref_id);

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

                objresponse = _custAppService.APTGetAllPayee(objRequest);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    //_log.WriteLog(MethodBase.GetCurrentMethod().Name, "Success", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.customer_mobile_no);

                    objresponse.session_details = loginResponse;
                    return Ok(objresponse);
                }
                else
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.Customer_ref_id);

                    return UnprocessableEntity(objresponse);
                }

            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, objRequest.Customer_ref_id);

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
        [Route("api/CustApp/AddPayee")]
        public IActionResult? AddPayee([FromBody] APTInsertPayeeCustomerAppReq objRequest)
        {
            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(objRequest), objRequest.mobile_number);

            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Request Null", JsonConvert.SerializeObject(objRequest), objRequest.mobile_number);

                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            SessionValidation session = ExtractFromHeader(objRequest.mobile_number);
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
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Session Validation Failed", JsonConvert.SerializeObject(objresponse), objRequest.mobile_number);

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
               
                    objresponse = _custAppService.APTAddPayeeCustApp(objRequest);
                
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Success", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.mobile_number);

                    objresponse.session_details = loginResponse;
                    return Ok(objresponse);
                }
                else
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.mobile_number);

                    return UnprocessableEntity(objresponse);
                }

            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, objRequest.mobile_number);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
        }

        #region Swagger
        [SwaggerOperation("DashboardAPI API", "DashboardAPI in system")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/CustApp/DashboardAPI")]
        public IActionResult? DashboardAPI([FromBody] APTDashboardRequest objRequest)
        {
            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(objRequest), objRequest.mobile_number);

            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Request Null", JsonConvert.SerializeObject(objRequest), objRequest.mobile_number);

                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            //SessionValidation session = ExtractFromHeader(objRequest.mobile_number);
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
            //        _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Session Validation Failed", JsonConvert.SerializeObject(objresponse), objRequest.mobile_number);

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

                objresponse = _custAppService.APTDashboardAPI(objRequest);

                if (objresponse.response_code == APIResponseCode.Success)
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Success", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.mobile_number);

                    objresponse.session_details = loginResponse;
                    return Ok(objresponse);
                }
                else
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.mobile_number);

                    return UnprocessableEntity(objresponse);
                }

            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, objRequest.mobile_number);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
        }

        #region Swagger
        [SwaggerOperation("Link Creation API API", "Link creation in system")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/CustApp/PaymentTxn")]
        public IActionResult? APTCustomerPaymentTransaction([FromBody] APTCustomerAppTransactionRequest objRequest)
        {
            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(objRequest), objRequest.agent_mobile);

            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Request Null", JsonConvert.SerializeObject(objRequest), objRequest.agent_mobile);

                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            //SessionValidation session = ExtractFromHeader(objRequest.mobile_number);
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
            //        _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Session Validation Failed", JsonConvert.SerializeObject(objresponse), objRequest.mobile_number);

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

                objresponse = _custAppService.APTCustomerPaymentTransaction(objRequest);

                if (objresponse.response_code == APIResponseCode.Success)
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Success", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.agent_mobile);

                    objresponse.session_details = loginResponse;
                    return Ok(objresponse);
                }
                else
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.agent_mobile);

                    return UnprocessableEntity(objresponse);
                }

            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, objRequest.agent_mobile);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
        }
    }
}
