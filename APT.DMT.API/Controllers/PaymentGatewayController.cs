using APT.DMT.API.BusinessLogic;
using APT.DMT.API.BusinessObjects;
using APT.DMT.API.BusinessObjects.Models;
using APT.PaymentServices.API.BusinessLogic;
using APT.PaymentServices.API.BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Reflection;
using System.Text.Json;

using System.IO;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace APT.PaymentServices.API.Controllers
{
    public class PaymentGatewayController : ControllerBase
    {
        public static PaymentGatewayService _pgService = new PaymentGatewayService();
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

        public List<string> ExtractSignatureFromHeader()
        {
            try
            {
                List<string> objStringList = new List<string>();

                objStringList.Add( Request.Headers["x-webhook-timestamp"].ToString());
                objStringList.Add( Request.Headers["x-webhook-signature"].ToString());
                if (objStringList.Count>1)
                {
                    return objStringList;
                }
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Incorrect Signature Values", Request.Headers.ToString(), "");
                return null;

            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, "");
                return null;
            }
        }


        #region Swagger
        [SwaggerOperation("Create PG order", "Used to create an order")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/PG/CreateOrder")]
        public IActionResult? APTCreatePaymentGatewayOrder([FromBody] InsertPGTransactionRequest objRequest)
        {
            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(objRequest), objRequest.customer_phone);

            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Request Null", JsonConvert.SerializeObject(objRequest), objRequest.customer_phone);

                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            if (!_commonService.isValidMobileNumber(objRequest.customer_phone))
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Invalid Mobile Number", JsonConvert.SerializeObject(objRequest), objRequest.customer_phone);

                objresponse.response_code = APIResponseCode.InvalidRequest;
                objresponse.response_message = APIResponseCodeDesc.InvalidRequest;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            //SessionValidation session = ExtractFromHeader(objRequest.customer_phone);
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
            //        _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Session Validation Failed", JsonConvert.SerializeObject(objresponse), objRequest.customer_phone);

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
                objresponse = _pgService.APTCreatePaymentGatewayOrder(objRequest);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Success", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.customer_phone);

                    objresponse.session_details = loginResponse;
                    return Ok(objresponse);
                }
                else
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.customer_phone);

                    return UnprocessableEntity(objresponse);
                }

            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, objRequest.customer_phone);
                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
        }

        #region Swagger
        [SwaggerOperation("Update PG order", "Used to update an order")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/PG/UpdateOrder")]
        public IActionResult? APTUpdatePaymentGatewayOrder([FromBody] UpdatePGTransactionRequest objRequest)
        {
            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(objRequest), objRequest.agent_mobile_number);

            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Request Null", JsonConvert.SerializeObject(objRequest), objRequest.agent_mobile_number);

                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            if (!_commonService.isValidMobileNumber(objRequest.agent_mobile_number))
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Invalid Mobile Number", JsonConvert.SerializeObject(objRequest), objRequest.agent_mobile_number);

                objresponse.response_code = APIResponseCode.InvalidRequest;
                objresponse.response_message = APIResponseCodeDesc.InvalidRequest;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            //SessionValidation session = ExtractFromHeader(objRequest.customer_phone);
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
            //        _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Session Validation Failed", JsonConvert.SerializeObject(objresponse), objRequest.customer_phone);

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
                objresponse = _pgService.APTUpdatePaymentGatewayOrder(objRequest);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Success", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.agent_mobile_number);

                    objresponse.session_details = loginResponse;
                    return Ok(objresponse);
                }
                else
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.agent_mobile_number);

                    return UnprocessableEntity(objresponse);
                }

            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, objRequest.agent_mobile_number);
                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
        }


       

        #region Swagger
        [SwaggerOperation("Update PG order", "Used to update an order")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/PG/APTCashFreeWebhook")]
        public IActionResult? APTCashFreeWebhook([FromBody] WebhookReqCashFree objRequest)
        {
            Request.EnableBuffering();
            Request.Body.Seek(0, SeekOrigin.Begin);
            string rawjsonString = "";
            using (var reader = new StreamReader(Request.Body, Encoding.ASCII))
            {
                rawjsonString = reader.ReadToEnd().ToString();
              
            }
           
          
          

            WebhookReqCashFree objAPIresponse = JsonConvert.DeserializeObject<WebhookReqCashFree>(rawjsonString);


            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "APTCashFreeWebhook Entry", JsonConvert.SerializeObject(objRequest), objAPIresponse.data.order.order_id);

            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Request Null", JsonConvert.SerializeObject(objRequest), objAPIresponse.data.order.order_id);

                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            try
            {
                List<string> lstdetails = new List<string>();
                lstdetails = ExtractSignatureFromHeader();

                objresponse = _pgService.APTWebhookUpdate(objAPIresponse, lstdetails, rawjsonString);
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Success", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objAPIresponse.data.order.order_id);
                return Ok(objresponse);


            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, objAPIresponse.data.order.order_id);
                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }


        }


        [HttpPost]
        [Route("api/PG/UpdateRazorpayOrder")]
        public IActionResult? APTUpdateRazorpayPaymentGatewayOrder([FromBody] UpdatePGTransactionRequest objRequest)
        {
            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(objRequest), objRequest.agent_mobile_number);

            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Request Null", JsonConvert.SerializeObject(objRequest), objRequest.agent_mobile_number);

                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            if (!_commonService.isValidMobileNumber(objRequest.agent_mobile_number))
            {
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Invalid Mobile Number", JsonConvert.SerializeObject(objRequest), objRequest.agent_mobile_number);

                objresponse.response_code = APIResponseCode.InvalidRequest;
                objresponse.response_message = APIResponseCodeDesc.InvalidRequest;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            //SessionValidation session = ExtractFromHeader(objRequest.customer_phone);
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
            //        _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Session Validation Failed", JsonConvert.SerializeObject(objresponse), objRequest.customer_phone);

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
                objresponse = _pgService.APTUpdatePaymentGatewayOrderRazorpay(objRequest);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Success", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.agent_mobile_number);

                    objresponse.session_details = loginResponse;
                    return Ok(objresponse);
                }
                else
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", "Request : " + JsonConvert.SerializeObject(objRequest) + "|Response :" + JsonConvert.SerializeObject(objresponse), objRequest.agent_mobile_number);

                    return UnprocessableEntity(objresponse);
                }

            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, objRequest.agent_mobile_number);
                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
        }

        [HttpPost]
        [Route("api/PG/GetRazorpayOrder")]
        public IActionResult? APTGetRazorpayPaymentGatewayOrder([FromBody] GetTransactionDetails objRequest)
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
            //if (!_commonService.isValidMobileNumber(objRequest.agent_mobile_number))
            //{
            //    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Invalid Mobile Number", JsonConvert.SerializeObject(objRequest), objRequest.agent_mobile_number);

            //    objresponse.response_code = APIResponseCode.InvalidRequest;
            //    objresponse.response_message = APIResponseCodeDesc.InvalidRequest;
            //    objresponse.data = null;
            //    return UnprocessableEntity(objresponse);
            //}
            //SessionValidation session = ExtractFromHeader(objRequest.customer_phone);
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
            //        _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Session Validation Failed", JsonConvert.SerializeObject(objresponse), objRequest.customer_phone);

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
                objresponse = _pgService.APTGetTransactionDetails(objRequest);
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
        [SwaggerOperation("Payout Check Status", "Used to check the status of Txn")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/PG/CheckStatusRP")]
        public IActionResult? CheckStatus([FromBody] RazorpayServiceRequest Request)
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
                objresponse = _pgService.PGCheckStatus(Request);
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
