using APT.DMT.API.BusinessLogic;
using APT.DMT.API.BusinessObjects;
using APT.DMT.API.BusinessObjects.Models;
using APT.PaymentServices.API.BusinessLogic;
using APT.PaymentServices.API.BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web;



namespace APT.DMT.API.Controllers
{
    public class ECollectController : ControllerBase
    {
        public static LogService _log = new LogService();
        public static ECollectService _eCollServ = new ECollectService();

        #region Swagger
        [SwaggerOperation("Ecollect Validate API", "Ecollect Validate API")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("YESBANK/V1/ECOLLECT/Notify_old")]
        [Consumes("application/xml")]
        [Produces("application/xml")]
        public IActionResult? Notify_old([FromBody] notify Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            NotifyResult notifyResult = new NotifyResult();
            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry Yesbank Notify", Request.ToString(), "");
            if (Request == null)
            {
                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            try
            {
                //objresponse = _eCollServ.APTNotifyPayment(Request);
                //if(objresponse.response_code == "200")
                //{
                //    notifyResult.result = "ok";
                //}
                //else
                //{
                //    notifyResult.result = "retry";
                //}
                objresponse.data = Request;
                return Ok(notifyResult);
                

            }
            catch (Exception e)
            {
                notifyResult.result = "Internal Server Error";
                return UnprocessableEntity(notifyResult);
            }
        }

        #region Swagger
        [SwaggerOperation("Ecollect Validate API", "Ecollect Validate API")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("YESBANK/V1/ECOLLECT/Validate_old")]
        [Consumes("application/xml")]
        [Produces("application/xml")]
        public IActionResult? Validate_old([FromBody]validate Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry YesBank Ecollect Validate", Request.ToString(), "");
            if (Request == null)
            {
                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            //if (string.IsNullOrEmpty(Request.from_date) || string.IsNullOrEmpty(Request.to_date) || string.IsNullOrEmpty(Request.agent_ref_id))
            //{
            //    objresponse.response_code = APIResponseCode.InvalidRequest;
            //    objresponse.response_message = APIResponseCodeDesc.InvalidRequest;
            //    objresponse.data = null;
            //    return UnprocessableEntity(objresponse);
            //}
            //objresponse = _dmtservice.ValidateSession(Request.session);
            //if (objresponse == null || objresponse.response_code != "100")
            //{
            //    return UnprocessableEntity(objresponse);
            //}
            try
            {
                EcollectYesBankValidateResponse Response = new EcollectYesBankValidateResponse();
                Response.decision = "pass";
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry YesBank Ecollect V Response", Response.decision, "");
                return Ok(Response);
                

            }
            catch (Exception e)
            {
                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry YesBank Ecollect V Exception", e.ToString(), "");
                return UnprocessableEntity(objresponse);
            }
        }


        //------------------JSON------------------------//
        #region Swagger
        [SwaggerOperation("Ecollect Validate API", "Ecollect Validate API")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("YESBANK/V1/ECOLLECT/Validate")]
        public IActionResult? Validate([FromBody]ECollectYesbankRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry YesBank Ecollect Validate", JsonConvert.SerializeObject(Request), "");
            if (Request == null)
            {
                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            //if (string.IsNullOrEmpty(Request.from_date) || string.IsNullOrEmpty(Request.to_date) || string.IsNullOrEmpty(Request.agent_ref_id))
            //{
            //    objresponse.response_code = APIResponseCode.InvalidRequest;
            //    objresponse.response_message = APIResponseCodeDesc.InvalidRequest;
            //    objresponse.data = null;
            //    return UnprocessableEntity(objresponse);
            //}
            //objresponse = _dmtservice.ValidateSession(Request.session);
            //if (objresponse == null || objresponse.response_code != "100")
            //{
            //    return UnprocessableEntity(objresponse);
            //}
            try
            {
                ECollectNewResponse Response = new ECollectNewResponse();
                ECollectNewResponseValidateResponse Resp = new ECollectNewResponseValidateResponse();
                Resp.decision = "pass";
                Response.validateResponse = Resp;
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry YesBank Ecollect V Response", Response.validateResponse.decision, "");
                return Ok(Response);


            }
            catch (Exception e)
            {
                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry YesBank Ecollect V Exception", e.ToString(), "");
                return UnprocessableEntity(objresponse);
            }
        }

        #region Swagger
        [SwaggerOperation("Ecollect Validate API", "Ecollect Validate API")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("YESBANK/V1/ECOLLECT/Notify")]
        public IActionResult? Notify([FromBody] YesbankECNewRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            YesbankECNewResponse Response = new YesbankECNewResponse();
            YesbankECNewNotifyResult notifyResult = new YesbankECNewNotifyResult();
            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry Yesbank Notify", JsonConvert.SerializeObject(Request), "");
            if (Request == null)
            {
                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            try
            {
                objresponse = _eCollServ.APTNotifyPayment(Request);
                if (objresponse.response_code == "200")
                {
                    notifyResult.result = "ok";
                }
                else
                {
                    notifyResult.result = "retry";
                }
                Response.notifyResult = notifyResult;
                objresponse.data = Request;
                return Ok(Response);


            }
            catch (Exception e)
            {
                notifyResult.result = "Internal Server Error";
                Response.notifyResult = notifyResult;
                return UnprocessableEntity(Response);
            }
        }

        //#region Swagger
        //[SwaggerOperation("Ecollect Account Addition", "Ecollect ValidateBank Account Addition")]
        //[SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        //[SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        //#endregion
        //[HttpPost]
        //[Route("api/ECOLLECT/AccountAddition")]
        //public IActionResult? AccountAddition([FromBody] ECOllectAccountAdditionRequest Request)
        //{
        //    APTGenericResponse objresponse = new APTGenericResponse();
            
        //    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry Yesbank Notify", JsonConvert.SerializeObject(Request), "");
        //    if (Request == null)
        //    {
        //        objresponse.response_code = APIResponseCode.RequestNull;
        //        objresponse.response_message = APIResponseCodeDesc.RequestNull;
        //        objresponse.data = null;
        //        return UnprocessableEntity(objresponse);
        //    }
        //    try
        //    {
        //        objresponse = _eCollServ.APTEcollectAccountAddition(Request);
        //        if (objresponse.response_code == APIResponseCode.Success)
        //        {
        //            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Success", "Request : " + JsonConvert.SerializeObject(Request) + "|Response :" + JsonConvert.SerializeObject(objresponse), Request.agent_mobile);

        //           // objresponse.session_details = loginResponse;
        //            return Ok(objresponse);
        //        }
        //        else
        //        {
        //            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", "Request : " + JsonConvert.SerializeObject(Request) + "|Response :" + JsonConvert.SerializeObject(objresponse), Request.agent_mobile);

        //            return UnprocessableEntity(objresponse);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        //notifyResult.result = "Internal Server Error";
        //        //Response.notifyResult = notifyResult;
        //        return UnprocessableEntity(objresponse);
        //    }
        //}


        #region Swagger
        [SwaggerOperation("Ecollect Account Addition", "Ecollect ValidateBank Account Addition")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/ECOLLECT/ViewAccount")]
        public IActionResult? ViewAccount([FromBody] ECOllectViewAccountRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();

            _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry Yesbank Notify", JsonConvert.SerializeObject(Request), "");
            if (Request == null)
            {
                objresponse.response_code = APIResponseCode.RequestNull;
                objresponse.response_message = APIResponseCodeDesc.RequestNull;
                objresponse.data = null;
                return UnprocessableEntity(objresponse);
            }
            try
            {
                objresponse = _eCollServ.APTEcollectViewAccount(Request);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Success", "Request : " + JsonConvert.SerializeObject(Request) + "|Response :" + JsonConvert.SerializeObject(objresponse), Request.agent_ref_id);

                    // objresponse.session_details = loginResponse;
                    return Ok(objresponse);
                }
                else
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", "Request : " + JsonConvert.SerializeObject(Request) + "|Response :" + JsonConvert.SerializeObject(objresponse), Request.agent_ref_id);

                    return UnprocessableEntity(objresponse);
                }
            }
            catch (Exception e)
            {
                //notifyResult.result = "Internal Server Error";
                //Response.notifyResult = notifyResult;
                return UnprocessableEntity(objresponse);
            }
        }

        //[Route("api/ECOLLECT/FileUpload")]
        //[HttpPost]
        //[Consumes("multipart/form-data")]
        //public async Task<IActionResult> FileUpload(IFormFile file, ECOllectViewAccountRequest Request)
        //{
        //    if (file == null || file.Length == 0)
        //    {
        //        return BadRequest("No file uploaded.");
        //    }

        //    try
        //    {
        //        // Process the file as needed (e.g., save it to a specific location).
        //        string filePath = Path.Combine("path/to/your/directory", file.FileName);
        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await file.CopyToAsync(stream);
        //        }

        //        // Return a success response.
        //        return Ok("File uploaded successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

        #region Swagger
        [SwaggerOperation("Deposit Slip Upload", "Upload deposit slip api with multipart")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [Route("api/ECOLLECT/DepositSlipUpload")]
        [HttpPost]
        public async Task<IActionResult> DepositSlipUpload(IFormFile file,IFormCollection FormData)
        {
           
           
            DepositSlipUploadRequest RequestObj = new DepositSlipUploadRequest();
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            
            // string we= keys.ToArray();
            APTGenericResponse objresponse = new APTGenericResponse();
            try
            {

                if (FormData["account_ref_id"].ToString() != null)
                    RequestObj.account_ref_id = FormData["account_ref_id"].ToString();
                else
                    return BadRequest("account_ref_id  empty.");
                //---------------------------------------
                if (FormData["account_type_ref_id"].ToString() != null)
                    RequestObj.account_type_ref_id = FormData["account_type_ref_id"].ToString();
                else
                    return BadRequest("account_type_ref_id empty.");
                //---------------------------------------
                if (FormData["mobile_number"].ToString() != null)
                    RequestObj.mobile_number = FormData["mobile_number"].ToString();
                else
                    return BadRequest("bank_account_number empty.");
                //---------------------------------------
                if (FormData["bank_transaction_id"].ToString() != null)
                    RequestObj.bank_transaction_id = FormData["bank_transaction_id"].ToString();
                else
                    return BadRequest("bank_transaction_id empty.");
                //---------------------------------------
                if (FormData["amount"].ToString() != null)
                    RequestObj.amount = FormData["amount"].ToString();
                else
                    return BadRequest("amount empty.");
                //---------------------------------------
                if (FormData["deposited_date"].ToString() != null)
                    RequestObj.deposited_date = FormData["deposited_date"].ToString();
                else
                    return BadRequest("deposited_date empty.");
                //---------------------------------------
                if (FormData["topup_type_refid"].ToString() != null)
                    RequestObj.topup_type_refid = FormData["topup_type_refid"].ToString();
                else
                    return BadRequest("topup_type_refid empty.");
                //---------------------------------------
                if (FormData["deposited_bank"].ToString() != null)
                    RequestObj.deposited_bank = FormData["deposited_bank"].ToString();
                else
                    return BadRequest("deposited_bank empty.");
                //---------------------------------------
                if (FormData["comments"].ToString() != null)
                    RequestObj.comments = FormData["comments"].ToString();
                else
                    return BadRequest("comments empty.");
                


                string filename = "DepositSlip"+ RequestObj.amount+ RequestObj.mobile_number+DateTime.Now.ToString("dd-MMM-yyyy");
                //string filePath = Path.Combine(@"E:\\HostingContents\\Files.Bankstack\\BankAccount", filename);

                //RequestObj.bank_file_name = "https://files.bankstack.in/BankAccount/"+ file.FileName+filename + System.IO.Path.GetExtension(file.FileName);

                string filePath = Path.Combine(@"F:\API Publish\Admin\InternalApp\dist\img", filename);

                RequestObj.image = "http://prodadmin.accupayd.co/dist/img/" + file.FileName + filename + System.IO.Path.GetExtension(file.FileName);

                
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                objresponse = _eCollServ.APTDepositSlipUpload(RequestObj);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Success", "Request : " + JsonConvert.SerializeObject(RequestObj) + "|Response :" + JsonConvert.SerializeObject(objresponse), RequestObj.mobile_number);

                    // objresponse.session_details = loginResponse;
                    return Ok(objresponse);
                }
                else
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", "Request : " + JsonConvert.SerializeObject(RequestObj) + "|Response :" + JsonConvert.SerializeObject(objresponse), RequestObj.mobile_number);

                    return UnprocessableEntity(objresponse);
                }
                // Return a success response.
                return Ok("File uploaded successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

       

    }
}
