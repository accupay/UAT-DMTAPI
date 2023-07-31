using APT.DMT.API.BusinessLogic;
using APT.DMT.API.BusinessObjects;
using APT.DMT.API.BusinessObjects.Models;
using APT.DMT.API.Businessstrings.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.Reflection;

namespace APT.DMT.API.Controllers
{
    public class ReportController : ControllerBase
    {
        public static ReportService _reportService = new ReportService();
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
        [SwaggerOperation("Transaction Ledger Report API", "Transaction Ledger Report")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/reports/TransactionLedgerReport")]
        public IActionResult? TransactionLedgerReport([FromBody] TransactionLedgerReportRequest objRequest)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            if (Request == null)
            {
                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            if (string.IsNullOrEmpty(objRequest.from_date) || string.IsNullOrEmpty(objRequest.to_date) || string.IsNullOrEmpty(objRequest.agent_ref_id))
            {
                objresponse.response_code = APIResponseCode.InvalidRequest;
                objresponse.response_message = APIResponseCodeDesc.InvalidRequest;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            SessionValidation session = ExtractFromHeader(objRequest.agent_ref_id);
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

                objresponse = _reportService.APTTransactionLedgerReport(objRequest);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    objresponse.session_details = loginResponse;
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

        #region Swagger
        [SwaggerOperation("Get Refund Payment Transaction Report API", "Get Refund Payment Transaction Report")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/reports/GetRefundPaymentTransactionReport")]
        public IActionResult? GetRefundPaymentTransactionReport([FromBody] GetRefundPaymentTransactionReportRequest objRequest)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            if (string.IsNullOrEmpty(objRequest.from_date) || string.IsNullOrEmpty(objRequest.to_date) || string.IsNullOrEmpty(objRequest.agent_ref_id))
            {
                objresponse.response_code = APIResponseCode.InvalidRequest;
                objresponse.response_message = APIResponseCodeDesc.InvalidRequest;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            SessionValidation session = ExtractFromHeader(objRequest.agent_ref_id);
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

                objresponse = _reportService.APTGetRefundPaymentTransactionReport(objRequest);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    objresponse.session_details = loginResponse;
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

        #region Swagger
        [SwaggerOperation("Get Refund PayOut Payment Transaction Report API", "Get Refund PayOut Payment Transaction Report")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/reports/GetRefundPaymentTransactionReportPayout")]
        public IActionResult? GetRefundPaymentTransactionReportPayout([FromBody] GetRefundPaymentTransactionReportRequest objRequest)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            if (string.IsNullOrEmpty(objRequest.from_date) || string.IsNullOrEmpty(objRequest.to_date) || string.IsNullOrEmpty(objRequest.agent_ref_id))
            {
                objresponse.response_code = APIResponseCode.InvalidRequest;
                objresponse.response_message = APIResponseCodeDesc.InvalidRequest;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            SessionValidation session = ExtractFromHeader(objRequest.agent_ref_id);
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

                objresponse = _reportService.APTGetRefundPaymentTransactionReportPayOut(objRequest);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    objresponse.session_details = loginResponse;
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


        #region Swagger
        [SwaggerOperation("Get Retailer Payment Transaction Report API", "Get Retailer Payment Transaction Report")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/reports/GetRetailerPaymentTransactionReport")]
        public IActionResult? GetRetailerPaymentTransactionReport([FromBody] GetRetailerPaymentTransactionReportRequest objRequest)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            if (string.IsNullOrEmpty(objRequest.from_date) || string.IsNullOrEmpty(objRequest.to_date) || string.IsNullOrEmpty(objRequest.agent_ref_id))
            {
                objresponse.response_code = APIResponseCode.InvalidRequest;
                objresponse.response_message = APIResponseCodeDesc.InvalidRequest;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            SessionValidation session = ExtractFromHeader(objRequest.agent_ref_id);
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

                objresponse = _reportService.APTGetRetailerPaymentTransactionReport(objRequest);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    objresponse.session_details = loginResponse;
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

        #region Swagger
        [SwaggerOperation("Get Retailer Payout Transaction Report API", "Get Retailer PayOut Transaction Report")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/reports/GetRetailerPaymentTransactionReportPayOut")]
        public IActionResult? GetRetailerPaymentTransactionReportPayOut([FromBody] GetRetailerPaymentTransactionReportRequest objRequest)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            if (string.IsNullOrEmpty(objRequest.from_date) || string.IsNullOrEmpty(objRequest.to_date) || string.IsNullOrEmpty(objRequest.agent_ref_id))
            {
                objresponse.response_code = APIResponseCode.InvalidRequest;
                objresponse.response_message = APIResponseCodeDesc.InvalidRequest;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            SessionValidation session = ExtractFromHeader(objRequest.agent_ref_id);
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

                objresponse = _reportService.APTGetRetailerPaymentTransactionReportPayOut(objRequest);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    objresponse.session_details = loginResponse;
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

        #region Swagger
        [SwaggerOperation("Get Retailer Topup Report API", "Get Retailer Topup Report")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/reports/GetRetailerTopupReport")]
        public IActionResult? GetRetailerTopupReport([FromBody] GetRetailerTopupReportRequest objRequest)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            if (string.IsNullOrEmpty(objRequest.from_date) || string.IsNullOrEmpty(objRequest.to_date) || string.IsNullOrEmpty(objRequest.agent_ref_id))
            {
                objresponse.response_code = APIResponseCode.InvalidRequest;
                objresponse.response_message = APIResponseCodeDesc.InvalidRequest;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            //SessionValidation session = ExtractFromHeader(objRequest.agent_ref_id);
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
            //        _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Session Validation Failed", JsonConvert.SerializeObject(objresponse), objRequest.agent_ref_id);

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

                objresponse = _reportService.APTGetRetailerTopupReport(objRequest);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    objresponse.session_details = loginResponse;
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

        #region Swagger
        [SwaggerOperation("Get Retailer Topup Report API", "Get Retailer Topup Report")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/reports/GetScrollMessage")]
        public IActionResult? GetScrollMessage([FromBody]GetScrollMessageRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            if (Request.MobileNumber == null)
            {
                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            //if (string.IsNullOrEmpty(objRequest.from_date) || string.IsNullOrEmpty(objRequest.to_date) || string.IsNullOrEmpty(objRequest.agent_ref_id))
            //{
            //    objresponse.response_code = APIResponseCode.InvalidRequest;
            //    objresponse.response_message = APIResponseCodeDesc.InvalidRequest;
            //    objresponse.data = null;
            //    return UnprocessableEntity(objresponse);
            //}
            //SessionValidation session = ExtractFromHeader(objRequest.agent_ref_id);
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
            //        _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Session Validation Failed", JsonConvert.SerializeObject(objresponse), objRequest.agent_ref_id);

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

                objresponse = _reportService.APTGetScrollMesssage(Request.MobileNumber);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    objresponse.session_details = loginResponse;
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

        #region Swagger
        [SwaggerOperation("Get Retailer PG Report API", "Get Retailer PG Report")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/reports/GetPGReport")]
        public IActionResult? GetPGReport([FromBody] GetRetailerTopupReportRequest objRequest)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            //if (string.IsNullOrEmpty(objRequest.from_date) || string.IsNullOrEmpty(objRequest.to_date) || string.IsNullOrEmpty(objRequest.agent_ref_id))
            //{
            //    objresponse.response_code = APIResponseCode.InvalidRequest;
            //    objresponse.response_message = APIResponseCodeDesc.InvalidRequest;
            //    objresponse.data = null;
            //    return UnprocessableEntity(objresponse);
            //}
            //SessionValidation session = ExtractFromHeader(objRequest.agent_ref_id);
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
            //        _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Session Validation Failed", JsonConvert.SerializeObject(objresponse), objRequest.agent_ref_id);

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

                objresponse = _reportService.APTPGTransactionReport(objRequest);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    objresponse.session_details = loginResponse;
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

        #region Swagger
        [SwaggerOperation("Get Retailer PG Report API", "Get Retailer PG Report")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/reports/GetCommonDetailReport")]
        public IActionResult? GetCommonDetailsReport([FromBody] GetRetailerCommonDetailsRequest objRequest)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            //if (string.IsNullOrEmpty(objRequest.from_date) || string.IsNullOrEmpty(objRequest.to_date) || string.IsNullOrEmpty(objRequest.agent_ref_id))
            //{
            //    objresponse.response_code = APIResponseCode.InvalidRequest;
            //    objresponse.response_message = APIResponseCodeDesc.InvalidRequest;
            //    objresponse.data = null;
            //    return UnprocessableEntity(objresponse);
            //}
            //SessionValidation session = ExtractFromHeader(objRequest.agent_ref_id);
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
            //        _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Session Validation Failed", JsonConvert.SerializeObject(objresponse), objRequest.agent_ref_id);

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

                objresponse = _reportService.APTGetCommonDetails(objRequest);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    objresponse.session_details = loginResponse;
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

        #region Swagger
        [SwaggerOperation("Get Customer Payout Txn Report API", "Get  Customer Payout Txn Report")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/reports/APTGetCustomerPayoutTxnReport")]
        public IActionResult? APTGetCustomerPayoutTxnReport([FromBody] GetRetailerCustomerPayoutTxnReportRequest objRequest)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            if (objRequest == null)
            {
                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            //if (string.IsNullOrEmpty(objRequest.from_date) || string.IsNullOrEmpty(objRequest.to_date) || string.IsNullOrEmpty(objRequest.agent_ref_id))
            //{
            //    objresponse.response_code = APIResponseCode.InvalidRequest;
            //    objresponse.response_message = APIResponseCodeDesc.InvalidRequest;
            //    objresponse.data = null;
            //    return UnprocessableEntity(objresponse);
            //}
            //SessionValidation session = ExtractFromHeader(objRequest.agent_ref_id);
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
            //        _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Session Validation Failed", JsonConvert.SerializeObject(objresponse), objRequest.agent_ref_id);

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

                objresponse = _reportService.APTGetCustomerPayoutTxnReport(objRequest);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    objresponse.session_details = loginResponse;
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
