using APT.DMT.API.BusinessLogic;
using APT.DMT.API.BusinessObjects;
using APT.DMT.API.BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.Data;
using System.Reflection;
namespace APT.PaymentServices.API.Controllers
{
    
    public class CommonController : ControllerBase
    {
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
        [SwaggerOperation("Get Topup Info", "Used to fetch top info")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/common/GetTopupInfo")]
        public IActionResult? GetTopupInfo()
        {

            APTGenericResponse objresponse = new APTGenericResponse();
            
            
            //SessionValidation session = ExtractFromHeader("");
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
            //        _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Session Validation Failed", JsonConvert.SerializeObject(objresponse), "");

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
                objresponse = _commonService.GetTopupInfo();
                if (objresponse.response_code == APIResponseCode.Success )
                {

                    objresponse.session_details = loginResponse;
                    return Ok(objresponse);
                }
                else
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", "Response :" + JsonConvert.SerializeObject(objresponse), "");

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
        [SwaggerOperation("Get Support Info", "Used to fetch support info")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/common/GetSupportInfo")]
        public IActionResult? GetSupportInfo(string AgentRefID)
        {

            APTGenericResponse objresponse = new APTGenericResponse();


            //SessionValidation session = ExtractFromHeader("");
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
            //        _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Session Validation Failed", JsonConvert.SerializeObject(objresponse), "");

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
                objresponse = _commonService.GetSupportInfo(AgentRefID);
                if (objresponse.response_code == APIResponseCode.Success )
                {

                    objresponse.session_details = loginResponse;
                    return Ok(objresponse);
                }
                else
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", "Response :" + JsonConvert.SerializeObject(objresponse), "");

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

       
    }
}
