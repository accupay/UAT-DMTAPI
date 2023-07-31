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
    public class PGtoPayoutController : ControllerBase
    {
        public static PGtoPayoutService _pgPayoutService = new PGtoPayoutService();
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
        [SwaggerOperation("Insert Customer API", "Used to insert Customer to DB")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/PGPayout/InsertCustomer")]
        public IActionResult? InsertCustomer([FromBody] APTInsertCustomerRequest Request)
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
                objresponse = _pgPayoutService.InsertCustomerInfo(Request);
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
        [SwaggerOperation("Get Customer Info  API", "Used to get Customer info from DB")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ok - Request Successful", typeof(APTGenericResponse))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed - Unprocessable Entity", typeof(APTGenericResponse))]
        #endregion
        [HttpPost]
        [Route("api/PGPayout/GetCustomerInfo")]
        public IActionResult? GetCustomerInfo([FromBody] APTGetCustomerInfoRequest Request)
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
                objresponse = _pgPayoutService.GetCustomerInfo(Request);
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

        [Route("api/PGPayout/UploadCreditCard")]
        [HttpPost]
        public async Task<IActionResult> UploadCC(IFormFile file, IFormCollection FormData)
        {


            APTPGtoPayoutuploadCreditCardRequest RequestObj = new APTPGtoPayoutuploadCreditCardRequest();
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // string we= keys.ToArray();
            APTGenericResponse objresponse = new APTGenericResponse();
            try
            {
                // Process the file as needed (e.g., save it to a specific location).
                //string root = HttpContext.Current.Server.MapPath(@"E:\\HostingContents\\Files.Bankstack\\BankAccount");
                //var provide = new MultipartFormDataStreamProvider("");

                if (FormData["credit_card_number"].ToString() != null)
                    RequestObj.credit_card_number = FormData["credit_card_number"].ToString();
                else
                    return BadRequest("credit_card_number empty.");

                if (FormData["credit_card_front_filename"].ToString() != null)
                    RequestObj.credit_card_front_filename = FormData["credit_card_front_filename"].ToString();
                else
                    return BadRequest("credit_card_front_filename empty.");

                if (FormData["transaction_id"].ToString() != null)
                    RequestObj.transaction_id = FormData["transaction_id"].ToString();
                else
                    return BadRequest("transaction_id empty.");

                if (FormData["user_id"].ToString() != null)
                    RequestObj.user_id = FormData["user_id"].ToString();
                else
                    return BadRequest("user_id empty.");

               


                string filename = RequestObj.credit_card_number;
                //string filePath = Path.Combine(@"E:\\HostingContents\\Files.Bankstack\\BankAccount", filename);

                //RequestObj.bank_file_name = "https://files.bankstack.in/BankAccount/"+ file.FileName+filename + System.IO.Path.GetExtension(file.FileName);

                string filePath = Path.Combine(@"F:\API Publish\Admin\InternalApp\dist\img", filename);

                RequestObj.credit_card_image_path = "http://prodadmin.accupayd.co/dist/img/" + file.FileName + filename + System.IO.Path.GetExtension(file.FileName);

                string credit_card_image_path = RequestObj.credit_card_image_path.Trim();
                RequestObj.credit_card_image_path = credit_card_image_path;
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                objresponse = _pgPayoutService.uploadCreditCard(RequestObj);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Success", "Request : " + JsonConvert.SerializeObject(RequestObj) + "|Response :" + JsonConvert.SerializeObject(objresponse), RequestObj.credit_card_number);

                    // objresponse.session_details = loginResponse;
                    return Ok(objresponse);
                }
                else
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", "Request : " + JsonConvert.SerializeObject(RequestObj) + "|Response :" + JsonConvert.SerializeObject(objresponse), RequestObj.credit_card_number);

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

        [Route("api/ECOLLECT/UploadKYC")]
        [HttpPost]
        public async Task<IActionResult> UploadKYC(List<IFormFile> file, IFormCollection FormData)
        {


            APTPGtoPayoutuploadkycRequest RequestObj = new APTPGtoPayoutuploadkycRequest();
            if (file == null || file[0].Length == 0 || file.Count > 2)
            {
                return BadRequest("Please add file.");
            }

            // string we= keys.ToArray();
            APTGenericResponse objresponse = new APTGenericResponse();
            try
            {
                // Process the file as needed (e.g., save it to a specific location).
                //string root = HttpContext.Current.Server.MapPath(@"E:\\HostingContents\\Files.Bankstack\\BankAccount");
                //var provide = new MultipartFormDataStreamProvider("");

                if (FormData["id_prrof_ref_number"].ToString() != null)
                    RequestObj.id_prrof_ref_number = FormData["id_prrof_ref_number"].ToString();
                else
                    return BadRequest("id_prrof_ref_number empty.");

                if (FormData["user_id"].ToString() != null)
                    RequestObj.user_id = FormData["user_id"].ToString();
                else
                    return BadRequest("user_id empty.");

                if (FormData["transaction_id"].ToString() != null)
                    RequestObj.transaction_id = FormData["transaction_id"].ToString();
                else
                    return BadRequest("transaction_id empty.");

              



                string filename1 = RequestObj.id_prrof_ref_number +"FrontImage";
                string filename2 = RequestObj.id_prrof_ref_number + "BackImage";
                //string filePath = Path.Combine(@"E:\\HostingContents\\Files.Bankstack\\BankAccount", filename);

                //RequestObj.bank_file_name = "https://files.bankstack.in/BankAccount/"+ file.FileName+filename + System.IO.Path.GetExtension(file.FileName);

                string filePath1 = Path.Combine(@"F:\API Publish\Admin\InternalApp\dist\img", filename1);
                string filePath2 = Path.Combine(@"F:\API Publish\Admin\InternalApp\dist\img", filename2);

                //  RequestObj.credit_card_image_path = "http://prodadmin.accupayd.co/dist/img/" + file.FileName + filename + System.IO.Path.GetExtension(file.FileName);

               // string credit_card_image_path = RequestObj.credit_card_image_path.Trim();
                RequestObj.id_proof_front = filename1;
                using (var stream = new FileStream(filePath1, FileMode.Create))
                {
                    await file[0].CopyToAsync(stream);
                }
                RequestObj.id_proof_back = filename2;
                using (var stream = new FileStream(filePath2, FileMode.Create))
                {
                    await file[1].CopyToAsync(stream);
                }
                objresponse = _pgPayoutService.UploadKYCDocument(RequestObj);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Success", "Request : " + JsonConvert.SerializeObject(RequestObj) + "|Response :" + JsonConvert.SerializeObject(objresponse), RequestObj.id_prrof_ref_number);

                    // objresponse.session_details = loginResponse;
                    return Ok(objresponse);
                }
                else
                {
                    _log.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", "Request : " + JsonConvert.SerializeObject(RequestObj) + "|Response :" + JsonConvert.SerializeObject(objresponse), RequestObj.id_prrof_ref_number);

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
