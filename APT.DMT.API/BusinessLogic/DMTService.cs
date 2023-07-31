
using APT.DMT.API.BusinessLogic;
using APT.DMT.API.BusinessLogic.BankService;
using APT.DMT.API.BusinessObjects;
using APT.DMT.API.BusinessObjects.Models;
using APT.DMT.API.Businessstrings.Models;
using APT.DMT.API.DataAccessLayer.DMT;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;
using File = System.IO.File;

namespace APT.DMT.API.BusinessLogic
{
    public class DMTService
    {
        private readonly string SessionValidationURL;
        private readonly string IFSCLookupURL;

        public static IConfiguration Configuration { get; set; }
        public static CommonService _commonService = new CommonService();

        public static DMTRepository _dmtRepo = new DMTRepository();
        public static PaytmVendorAPI _paytmAPI = new PaytmVendorAPI();

        public static LogService _logService = new LogService();

        public DMTService()
        {
            Configuration = _commonService.GetConfiguration();
            SessionValidationURL = Configuration["SessionValidation:Domain"] + Configuration["SessionValidation:URL"];
            IFSCLookupURL = Configuration["Cache:Domain"] + Configuration["Cache:IFSCLookupURL"];

        }

        #region DMT

        #region GetCustomerInfo
        public APTGenericResponse? APTGetCustomerInfo(APTGetCustomerInfo Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.mobile_no);

            try
            {
                DataSet dst = _dmtRepo.APTGetCustomerInfo(Request);
                if (_commonService.IsValidDataSet(dst))
                {
                    APTGetCustomerInfoResponse CustInfo = new APTGetCustomerInfoResponse();
                    CustInfo.customer_ref_id = dst.Tables[0].Rows[0]["CustomerRefID"].ToString();
                    CustInfo.paytm_ref_id = dst.Tables[0].Rows[0]["PaytmRefid"].ToString();
                    CustInfo.agent_ref_id = dst.Tables[0].Rows[0]["AgentRefID"].ToString();
                    CustInfo.account_type_ref_id = dst.Tables[0].Rows[0]["AccountTypeRefID"].ToString();
                    CustInfo.customer_name = dst.Tables[0].Rows[0]["CustomerName"].ToString();
                    CustInfo.last_name = dst.Tables[0].Rows[0]["LastName"].ToString();
                    CustInfo.mobile_number = dst.Tables[0].Rows[0]["MobileNumber"].ToString();
                    CustInfo.pin = dst.Tables[0].Rows[0]["PIN"].ToString();
                    CustInfo.email = dst.Tables[0].Rows[0]["EMail"].ToString();
                    CustInfo.address1 = dst.Tables[0].Rows[0]["Address1"].ToString();
                    CustInfo.address2 = dst.Tables[0].Rows[0]["Address2"].ToString();
                    CustInfo.pincode = dst.Tables[0].Rows[0]["Pincode"].ToString();
                    CustInfo.dob = dst.Tables[0].Rows[0]["DOB"].ToString();
                    CustInfo.balance = dst.Tables[0].Rows[0]["Balance"].ToString();

                    CustInfo.bank_1_current_balance = dst.Tables[0].Rows[0]["Bank1CurrentBalance"].ToString();

                    CustInfo.active_status_ref_id = dst.Tables[0].Rows[0]["ActiveStatusRefID"].ToString();
                    PaytmPrevalidateCustomerResponse objAPIResponse = new PaytmPrevalidateCustomerResponse();
                    objAPIResponse = _paytmAPI.PrevalidateCustomer(Request.mobile_no);
                    if (objAPIResponse != null && objAPIResponse.response_code == "0")
                    {
                        CustInfo.bank_1_monthly_balance = objAPIResponse.limitLeft;
                    }
                    else
                    {
                        CustInfo.bank_1_monthly_balance = dst.Tables[0].Rows[0]["Bank1MonthlyBalance"].ToString();

                    }
                    if (CustInfo.active_status_ref_id == "1")
                    {

                        if (objAPIResponse != null && objAPIResponse.response_code == "0")
                        {
                            CustInfo.is_internal = true;
                        }
                        else if (objAPIResponse != null && objAPIResponse.response_code == "1032")
                        {
                            CustInfo.is_internal = false;
                        }
                        else
                        {
                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Register Customer Prevalidate Failed", JsonConvert.SerializeObject(objAPIResponse), Request.mobile_no);

                            objresponse.response_code = "110";
                            objresponse.response_message = "Fetching data from Bank Failed, please try again.";
                            objresponse.data = null;
                        }
                    }
                    CustInfo.status = dst.Tables[0].Rows[0]["Status"].ToString();
                    CustInfo.customer_status = dst.Tables[0].Rows[0]["CustomerStatus"].ToString(); // doubt
                    CustInfo.city = dst.Tables[0].Rows[0]["City"].ToString();


                    CustInfo.bank_1_monthly_transcount = dst.Tables[0].Rows[0]["Bank1MonthlyTransCount"].ToString();
                    CustInfo.new_current_balance = dst.Tables[0].Rows[0]["NewCurrentBalance"].ToString();
                    CustInfo.entity_id = dst.Tables[0].Rows[0]["entityId"].ToString();
                    CustInfo.gender = dst.Tables[0].Rows[0]["Gender"].ToString();
                    CustInfo.state = dst.Tables[0].Rows[0]["State"].ToString();

                    objresponse.response_code = "200";
                    objresponse.response_message = "Success";
                    objresponse.data = CustInfo;

                }
                else
                {
                    objresponse.response_code = "101";
                    objresponse.response_message = "Customer Not Found, Please Register Customer";
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
        #endregion

        #region RegisterCustomer
        public APTGenericResponse? APTRegisterCustomer(APTRegisterCustomer Request)
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.mobile_no);

            APTGenericResponse objresponse = new APTGenericResponse();
            PaytmPrevalidateCustomerResponse objAPIResponse = new PaytmPrevalidateCustomerResponse();
            APTRegisterCustomerResponse objregResponse = new APTRegisterCustomerResponse();
            try
            {
                objAPIResponse = _paytmAPI.PrevalidateCustomer(Request.mobile_no);
                if (objAPIResponse != null && objAPIResponse.response_code == "0")//User Exist in bank
                {
                    Request.bank_id = "0";
                    Request.bank_ref_id = Request.mobile_no;
                    Request.otp_validation_flag = "1";

                    DataSet register_dst = _dmtRepo.APTRegisterCustomerWithID(Request);
                    if (_commonService.IsValidDataSet(register_dst))
                    {
                        if (register_dst.Tables[0].Rows[0][0].ToString() == "100")
                        {
                            GetBeneListResponse beneListResponse = new GetBeneListResponse();
                            beneListResponse = _paytmAPI.GetBeneList(Request.mobile_no);
                            if (beneListResponse != null && beneListResponse.response_code == "0")
                            {
                                //_logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Customer Available , Bene Count :" + beneListResponse.totalCount.ToString(), JsonConvert.SerializeObject(beneListResponse), Request.mobile_no);

                                AddBeneToDB(beneListResponse.beneficiaries, Request.agent_ref_id, Request.mobile_no);
                            }
                            objresponse.response_code = "201"; //no need to validate OTP 
                            objresponse.response_message = "Customer Registered Successfully";
                            objresponse.data = null;

                        }
                        else
                        {
                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Issue in Inserting Customer to DB", JsonConvert.SerializeObject(Request), Request.mobile_no);

                            objresponse.response_code = register_dst.Tables[0].Rows[0][0].ToString();
                            objresponse.response_message = register_dst.Tables[0].Rows[0][1].ToString();
                            objresponse.data = null;
                        }
                    }
                    else
                    {
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Not a Valid Dataset", JsonConvert.SerializeObject(Request), Request.mobile_no);

                        objresponse.response_code = APIResponseCode.DatasetNull;
                        objresponse.response_message = APIResponseCodeDesc.DatasetNull;
                        objresponse.data = null;
                    }
                }
                else if (objAPIResponse != null && objAPIResponse.response_code == "1032") // User does not exist in bank
                {
                    Request.otp_validation_flag = "0";
                    DataSet dst = _dmtRepo.APTRegisterCustomer(Request);
                    if (_commonService.IsValidDataSet(dst))
                    {
                        if (dst.Tables[0].Rows[0][0].ToString() == "100")
                        {
                            PaytmSendOTPRequest otpRequest = new PaytmSendOTPRequest();
                            PaytmSendOTPResponse otpResponse = new PaytmSendOTPResponse();
                            otpRequest.customerMobile = Request.mobile_no;
                            otpResponse = _paytmAPI.SendOTPForRegistration(otpRequest);
                            if (otpResponse.status == "success")
                            {
                                objregResponse.is_internal = false;
                                objregResponse.otp_state = otpResponse.state;
                                objresponse.response_code = APIResponseCode.Success;
                                objresponse.response_message = "OTP Sent Successfully";
                                objresponse.data = objregResponse;
                            }
                            else
                            {
                                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Issue in Sending OTP :", JsonConvert.SerializeObject(otpResponse), Request.mobile_no);

                                objresponse.response_code = "102";
                                objresponse.response_message = "Sending OTP From Bank Failed. Please Try Again";
                                objresponse.data = otpResponse;
                            }
                        }
                        else
                        {
                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Issue in Inserting Customer to DB", JsonConvert.SerializeObject(Request), Request.mobile_no);

                            objresponse.response_code = dst.Tables[0].Rows[0][0].ToString();
                            objresponse.response_message = dst.Tables[0].Rows[0][1].ToString();
                            objresponse.data = null;
                        }
                    }
                    else
                    {
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Not a Valid Dataset", JsonConvert.SerializeObject(Request), Request.mobile_no);

                        objresponse.response_code = APIResponseCode.DatasetNull;
                        objresponse.response_message = APIResponseCodeDesc.DatasetNull;
                        objresponse.data = null;
                    }
                }
                else
                {
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Prevalidate Failed Response", JsonConvert.SerializeObject(objAPIResponse), Request.mobile_no);

                    objresponse.response_code = APIResponseCode.Failed;
                    objresponse.response_message = APIResponseCodeDesc.Failed;
                    objresponse.data = objAPIResponse;
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

        #endregion RegisterCustomer

        #region ValidateOTP
        public APTGenericResponse? ValidateRegisterCustomerOTP(ValidateOTPRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.mobile_no);

            try
            {
                if (Request.otp_state == "undefined")
                {

                    DataSet dst = _dmtRepo.APTValidateRegisterCustomerOTP(Request);
                    if (_commonService.IsValidDataSet(dst) && dst.Tables[0].Rows[0][0].ToString() == APIResponseCode.DBSuccess)
                    {
                        if (Request.isRetailer == true)
                        {
                            objresponse.response_code = "200";
                            objresponse.response_message = "Customer Registered Successfully";
                            objresponse.data = null;
                        }
                        else
                        {
                            APTUpdateCustomerRequest updateCustReq = new APTUpdateCustomerRequest();
                            updateCustReq.updated_by = Request.agent_ref_id;
                            updateCustReq.active_status_ref_id = "2";
                            updateCustReq.bank_ref_id = Request.mobile_no;
                            updateCustReq.mobile_no = Request.mobile_no;
                            objresponse = UpdateCustomer(updateCustReq);
                        }

                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "OTP Validated", JsonConvert.SerializeObject(objresponse), Request.mobile_no);

                    }
                    else
                    {
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed Dataset", JsonConvert.SerializeObject(Request), Request.mobile_no);

                        objresponse.response_code = dst.Tables[0].Rows[0][0].ToString();
                        objresponse.response_message = dst.Tables[0].Rows[0][1].ToString();
                        objresponse.data = null;
                    }
                }
                else
                {
                    APTGetCustomerInfo objRequest = new APTGetCustomerInfo();
                    objRequest.flag = "0";
                    objRequest.is_internal = true;
                    objRequest.mobile_no = Request.mobile_no;
                    objRequest.agent_ref_id = Request.agent_ref_id;

                    APTGenericResponse objCustData = APTGetCustomerInfo(objRequest);
                    if (objCustData != null && objCustData.response_code == APIResponseCode.Success)
                    {
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Customer Info Fetched", JsonConvert.SerializeObject(Request), Request.mobile_no);

                        APTGetCustomerInfoResponse CustInfo = new APTGetCustomerInfoResponse();
                        CustInfo = (APTGetCustomerInfoResponse)objCustData.data;
                        if (CustInfo != null)
                        {
                            PaytmCustomerRegistrationRequest paytmCustRegReq = new PaytmCustomerRegistrationRequest();
                            PaytmCustomerRegistrationResponse paytmCustRegResponse = new PaytmCustomerRegistrationResponse();

                            PaytmCustomerRegistrationAddress objAddress = new PaytmCustomerRegistrationAddress();
                            PaytmCustomerRegistrationName objCustname = new PaytmCustomerRegistrationName();
                            paytmCustRegReq.name = new PaytmCustomerRegistrationName();
                            paytmCustRegReq.address = new PaytmCustomerRegistrationAddress();

                            paytmCustRegReq.customerMobile = Request.mobile_no;
                            paytmCustRegReq.otp = Request.otp;
                            paytmCustRegReq.state = Request.otp_state;
                            paytmCustRegReq.name.firstName = CustInfo.customer_name;

                            Random ran = new Random();

                            String b = "abcdefghijklmnopqrstuvwxyz";

                            int length = 6;

                            String random = "";

                            for (int i = 0; i < length; i++)
                            {
                                int a = ran.Next(25);
                                random = random + b.ElementAt(a);
                            }


                            paytmCustRegReq.name.lastName = random;
                            paytmCustRegReq.address.name = CustInfo.customer_name;
                            paytmCustRegReq.address.address1 = CustInfo.address1;
                            paytmCustRegReq.address.address2 = CustInfo.address2;
                            paytmCustRegReq.address.mobile = Request.mobile_no;
                            paytmCustRegReq.address.pin = CustInfo.pin;
                            paytmCustRegReq.address.city = CustInfo.city;
                            paytmCustRegReq.address.country = "India";
                            paytmCustRegReq.address.state = CustInfo.state;

                            paytmCustRegResponse = _paytmAPI.CustomerRegistration(paytmCustRegReq);
                            if (paytmCustRegResponse != null && paytmCustRegResponse.status == "success")
                            {
                                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Customer Registered at Bank End", "Request : " + JsonConvert.SerializeObject(Request) + "| Response :" + JsonConvert.SerializeObject(paytmCustRegResponse), Request.mobile_no);

                                APTUpdateCustomerRequest updateCustReq = new APTUpdateCustomerRequest();
                                updateCustReq.updated_by = Request.agent_ref_id;
                                updateCustReq.active_status_ref_id = "2";
                                updateCustReq.bank_ref_id = Request.mobile_no;
                                updateCustReq.mobile_no = Request.mobile_no;
                                objresponse = UpdateCustomer(updateCustReq);
                            }
                            else
                            {
                                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Customer Registration Failed at Bank End", "Request : " + JsonConvert.SerializeObject(Request) + "| Response :" + JsonConvert.SerializeObject(paytmCustRegResponse), Request.mobile_no);

                                objresponse.response_code = APIResponseCode.Failed;
                                objresponse.response_message = APIResponseCodeDesc.Failed;
                                objresponse.data = paytmCustRegResponse;
                            }
                        }
                    }
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

        public APTGenericResponse? ResendOTP(ResendOTPRequest Request)
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.mobile_no);

            APTGenericResponse objresponse = new APTGenericResponse();
            try
            {
                if (Request.flag.ToString() == "5") // refund 
                {
                    Random generator = new Random();
                    string OTP = generator.Next(0, 1000000).ToString("D6");
                    string RefId = Request.agent_ref_id;

                    APTInsertOTPRequest insertOTPRequest = new APTInsertOTPRequest();
                    insertOTPRequest.mobile_no = Request.mobile_no;
                    insertOTPRequest.account_ref_id = RefId;
                    insertOTPRequest.account_type = Request.account_type;
                    insertOTPRequest.otp = OTP;
                    bool IsSuccess = SendOTPFromDBforCustomerRegistration(insertOTPRequest);
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
                else
                {
                    objresponse = SendPaytmOTP(Request.mobile_no);
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Paytm OTP", "Request : " + JsonConvert.SerializeObject(Request) + "| Response :" + JsonConvert.SerializeObject(objresponse), Request.mobile_no);

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

        public APTGenericResponse? UpdateCustomer(APTUpdateCustomerRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.mobile_no);

            try
            {
                DataSet dst = _dmtRepo.APTUpdateCustomer(Request);
                if (_commonService.IsValidDataSet(dst) && dst.Tables[0].Rows[0][0].ToString() == "100")
                {
                    objresponse.response_code = "200";
                    objresponse.response_message = "Customer Registered Successfully";
                    objresponse.data = null;
                }
                else if (dst.Tables[0].Rows[0][0].ToString() == "105")
                {
                    objresponse.response_code = "200";
                    objresponse.response_message = "Customer Registered Successfully";
                    objresponse.data = null;
                }
                else
                {
                    objresponse.response_code = "103";
                    objresponse.response_message = "Customer Update to DB failed, Please Try Again";
                    objresponse.data = null;
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Update Failed", JsonConvert.SerializeObject(objresponse), Request.mobile_no);

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
        #endregion

        #region SendOTP
        public APTGenericResponse? SendPaytmOTP(String MobileNo, string OTPType = "")
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", "", MobileNo);

            APTGenericResponse objresponse = new APTGenericResponse();
            PaytmSendOTPRequest objAPIRequest = new PaytmSendOTPRequest();
            PaytmSendOTPResponse objAPIResponse = new PaytmSendOTPResponse();
            if (OTPType == APIConstants.BeneDeletion)
            {
                objAPIRequest.otpType = "deletionOtp";
            }
            objAPIRequest.customerMobile = MobileNo;
            objAPIResponse = _paytmAPI.SendOTPForRegistration(objAPIRequest);
            if (objAPIResponse != null && objAPIResponse.status == "success")
            {
                objresponse.response_code = APIResponseCode.Success;
                objresponse.response_message = "OTP Sent To Registered Number. Please validate OTP";
                objresponse.data = objAPIResponse;
            }
            else
            {
                objresponse.response_code = "102";
                objresponse.response_message = "Sending OTP From Bank Failed. Please Try Again";
                objresponse.data = objAPIResponse;
                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Update Failed", JsonConvert.SerializeObject(objresponse), MobileNo);

            }
            return objresponse;
        }
        public bool SendOTPFromDBforCustomerRegistration(APTInsertOTPRequest Request)
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.mobile_no);

            APTGenericResponse objResponse = new APTGenericResponse();
            try
            {
                Random generator = new Random();
                string OTP = generator.Next(0, 1000000).ToString("D6");
                Request.otp = OTP;

                DataSet dstinsotp = _dmtRepo.APTInsertOTP(Request);
                if (_commonService.IsValidDataSet(dstinsotp))
                {
                    if (dstinsotp.Tables[0].Rows[0][0].ToString() == "100" || dstinsotp.Tables[0].Rows[0][0].ToString() == "106")
                    {
                        if (dstinsotp.Tables[0].Rows[0][0].ToString() == "106")
                        {
                            OTP = dstinsotp.Tables[0].Rows[0]["OldOTP"].ToString();
                        }
                        string mobileno = APIConstants.Countrycode + Request.mobile_no;
                        objResponse = _commonService.APTSendOTP(OTP, mobileno);
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

        #region AddPayee
        public APTGenericResponse? APTAddPayee(APTAddPayee Request)
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.mobile_no);

            APTGenericResponse objresponse = new APTGenericResponse();
            try
            {
                Request.active_status_ref_id = "1";
                Request.bank_type_id = "1";
                DataSet dst = _dmtRepo.APTAddPayee(Request);
                if (_commonService.IsValidDataSet(dst))
                {
                    if (dst.Tables[0].Rows[0][0].ToString() == "100")
                    {
                        APTUpdatePayeeRequest Req = new APTUpdatePayeeRequest();
                        Req.otp = "";
                        Req.otp_state = "";
                        Req.payee_ref_id = dst.Tables[0].Rows[0]["Payeerefid"].ToString();
                        Req.mobile_no = Request.mobile_no;
                        objresponse = ValidatePayeeOTP(Req);
                        return objresponse;
                    }
                    else
                    {

                        objresponse.response_code = dst.Tables[0].Rows[0][0].ToString();
                        objresponse.response_message = dst.Tables[0].Rows[0][1].ToString();
                        objresponse.data = null;
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Add PAyee Failed", JsonConvert.SerializeObject(objresponse), Request.mobile_no);

                    }
                }
                else
                {
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Add Payee Dataset Null", JsonConvert.SerializeObject(objresponse), Request.mobile_no);

                    objresponse.response_code = APIResponseCode.DatasetNull;
                    objresponse.response_message = APIResponseCodeDesc.DatasetNull;
                    objresponse.data = null;
                }

            }
            catch (Exception ex)
            {
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.mobile_no);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
            }

            return objresponse;
        }
        public APTGenericResponse? AddBeneToDB(List<GetBeneListResponseBeneficiary> beneListResponse, string AgentRefID, string CustomerMobile)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            for (int i = 0; i < beneListResponse.Count; i++)
            {
                List<BanksListResponse> ifscListResponse = new List<BanksListResponse>();
                try
                {
                    objresponse = _commonService.IFSCLookup(beneListResponse[i].accountDetail.ifscCode);
                    string ifscList = JsonConvert.SerializeObject(objresponse.data);
                    ifscListResponse = JsonConvert.DeserializeObject<List<BanksListResponse>>(ifscList);
                }
                catch (Exception e)
                {

                }
                if (ifscListResponse.Count() > 0)
                {
                    //_logService.WriteLog(MethodBase.GetCurrentMethod().Name, "IFSC Available" + beneListResponse[i].accountDetail.ifscCode, JsonConvert.SerializeObject(beneListResponse), CustomerMobile);

                    APTAddPayee insertPayee = new APTAddPayee();
                    insertPayee.account_number = beneListResponse[i].accountDetail.accountNumber;
                    //insertPayee.session = Request.session;
                    insertPayee.flag = "0";
                    insertPayee.payee_name_npci = beneListResponse[i].accountDetail.accountHolderName;
                    insertPayee.payee_ref_id = "0";
                    insertPayee.agent_ref_id = AgentRefID;
                    insertPayee.bank_ref_id = beneListResponse[i].beneficiaryId;
                    insertPayee.bank_type_id = "";
                    insertPayee.bank_id = ifscListResponse[0].bank_ref_id.ToString();
                    insertPayee.payee_mobile_no = "";
                    insertPayee.ifsc_code = beneListResponse[i].accountDetail.ifscCode;
                    insertPayee.ifsc_bank_ref_id = ifscListResponse[0].bank_ref_id.ToString();
                    insertPayee.ifsc_bank_branch_ref_id = ifscListResponse[0].bank_branch_ref_id.ToString();
                    insertPayee.mobile_no = CustomerMobile;
                    insertPayee.payee_name = beneListResponse[i].accountDetail.accountHolderName;
                    objresponse = APTAddPayee(insertPayee);

                }
                else
                {
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "IFSC Not Available" + beneListResponse[i].accountDetail.ifscCode, JsonConvert.SerializeObject(beneListResponse), CustomerMobile);

                }
            }
            return objresponse;
        }
        public APTGenericResponse? APTBeneValidate(APTTransactionRequest Request)
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.sender_mobile_number);

            APTGenericResponse objresponse = new APTGenericResponse();
            try
            {
                string mode = "";
                if (Request.pay_mode_ref_id == "1")
                {
                    mode = "imps";
                }
                if (Request.pay_mode_ref_id == "2")
                {
                    mode = "neft";
                }
                DataSet dst = _dmtRepo.APTTransaction(Request);
                if (_commonService.IsValidDataSet(dst))
                {
                    if (dst.Tables[0].Rows[0][0].ToString() == "100")
                    {
                        BeneValidateRequest beneValidateRequest = new BeneValidateRequest();
                        beneValidateRequest.customerMobile = Request.sender_mobile_number;
                        beneValidateRequest.txnReqId = dst.Tables[0].Rows[0]["TransactionID"].ToString();
                        beneValidateRequest.beneficiaryDetails = new BeneValidateDetails();
                        beneValidateRequest.beneficiaryDetails.accountNumber = Request.account_number_in;
                        beneValidateRequest.beneficiaryDetails.bankName = Request.bank_name;
                        beneValidateRequest.beneficiaryDetails.benIfsc = Request.ifsc_code;


                        BeneValidateResponse validateBeneResponseObj = _paytmAPI.BeneValidation(beneValidateRequest);
                        if (validateBeneResponseObj != null && validateBeneResponseObj.status == "success")
                        {
                            APTUpdateTransaction updateRequest = new APTUpdateTransaction();
                            updateRequest.transaction_id = dst.Tables[0].Rows[0]["TransactionID"].ToString() + "0";
                            updateRequest.request = JsonConvert.SerializeObject(beneValidateRequest);
                            updateRequest.response = JsonConvert.SerializeObject(validateBeneResponseObj);
                            updateRequest.response_description = validateBeneResponseObj.message;
                            updateRequest.bank_reference_number = validateBeneResponseObj.rrn;
                            updateRequest.response_code = validateBeneResponseObj.response_code.ToString();
                            updateRequest.beneficiary_name = validateBeneResponseObj.extra_info.beneficiaryName.ToString(); // for agent account addition payment txn type 5
                            DataSet dstUpdate = _dmtRepo.APTUpdateTransaction(updateRequest);
                            if (_commonService.IsValidDataSet(dstUpdate))
                            {
                                if (dstUpdate.Tables[0].Rows[0][0].ToString() == APIResponseCode.DBSuccess)
                                {
                                    APTUpdatePayeeRequest obj = new APTUpdatePayeeRequest();
                                    obj.npci_name = validateBeneResponseObj.extra_info.beneficiaryName.ToString();
                                    obj.validated = "1";
                                    obj.payee_ref_id = Request.payee_ref_id;
                                    obj.agent_ref_id = Request.agent_ref_id;
                                    obj.updated_by = Request.agent_ref_id;
                                    obj.mobile_no = Request.sender_mobile_number;
                                    DataSet dt = _dmtRepo.APTUpdateBeneValidationStatus(obj);

                                    APTBeneValidateResponse objdata = new APTBeneValidateResponse();
                                    objdata.customerMobile = validateBeneResponseObj.customerMobile;
                                    objdata.beneficiaryName = validateBeneResponseObj.extra_info.beneficiaryName;
                                    objdata.transactionDate = validateBeneResponseObj.transactionDate.ToString();
                                    objresponse.response_code = APIResponseCode.Success;
                                    objresponse.response_message = "Bene Validated Successfully";
                                    objresponse.data = objdata;
                                }
                                else
                                {
                                    objresponse.response_code = dst.Tables[0].Rows[0][0].ToString();
                                    objresponse.response_message = dst.Tables[0].Rows[0][1].ToString();
                                    objresponse.data = null;
                                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Bene Validate Db Update Failed", JsonConvert.SerializeObject(objresponse), Request.sender_mobile_number);

                                }
                            }
                            else
                            {
                                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Bene Validate dataset Null", "", Request.sender_mobile_number);

                                objresponse.response_code = APIResponseCode.DatasetNull;
                                objresponse.response_message = APIResponseCodeDesc.DatasetNull;
                                objresponse.data = null;
                            }

                        }
                        else
                        {
                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Bene Validate API Failed", JsonConvert.SerializeObject(objresponse), Request.sender_mobile_number);

                            objresponse.response_code = APIResponseCode.Failed;
                            objresponse.response_message = APIResponseCodeDesc.Failed;
                            objresponse.data = validateBeneResponseObj;
                        }

                    }
                    else
                    {
                        objresponse.response_code = dst.Tables[0].Rows[0][0].ToString();
                        objresponse.response_message = dst.Tables[0].Rows[0][1].ToString();
                        objresponse.data = null;
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Bene Validate Db Insert Failed", JsonConvert.SerializeObject(objresponse), Request.sender_mobile_number);

                    }
                }
                else
                {
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Bene Validate Db Insert Dataset Null", JsonConvert.SerializeObject(objresponse), Request.sender_mobile_number);

                    objresponse.response_code = APIResponseCode.DatasetNull;
                    objresponse.response_message = APIResponseCodeDesc.DatasetNull;
                    objresponse.data = null;
                }

            }
            catch (Exception ex)
            {
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.sender_mobile_number);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
            }

            return objresponse;
        }
        public APTGenericResponse? ValidatePayeeOTP(APTUpdatePayeeRequest Request)
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.mobile_no);

            APTGenericResponse objresponse = new APTGenericResponse();

            try
            {

                DataSet dst = _dmtRepo.APTGetPayeeDetails(Request.payee_ref_id, Request.mobile_no);
                if (!_commonService.IsValidDataSet(dst))
                {
                    objresponse.response_code = APIResponseCode.DatasetNull;
                    objresponse.response_message = APIResponseCodeDesc.DatasetNull;
                    objresponse.data = null;
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Bene Registration Dataset Null", "", Request.mobile_no);

                }
                else
                {
                    if (dst.Tables[0].Columns.Count > 4)
                    {
                        PaytmAddBeneRequest beneRequest = new PaytmAddBeneRequest();
                        beneRequest.otp = Request.otp;
                        beneRequest.state = Request.otp_state;
                        beneRequest.customerMobile = Request.mobile_no;
                        beneRequest.beneficiaryDetails = new BeneficiaryDetails();
                        BeneficiaryDetails bene = new BeneficiaryDetails();
                        bene.bankName = dst.Tables[0].Rows[0]["BankName"].ToString();
                        bene.accountNumber = dst.Tables[0].Rows[0]["AccountNumber"].ToString();
                        bene.nickName = dst.Tables[0].Rows[0]["BeneName"].ToString();
                        bene.benIfsc = dst.Tables[0].Rows[0]["BeneIFSCCode"].ToString();
                        bene.name = dst.Tables[0].Rows[0]["BeneName"].ToString();
                        beneRequest.beneficiaryDetails = bene;
                        PaytmAddBeneResponse addBeneResponseObj = _paytmAPI.BeneRegistration(beneRequest);
                        if (addBeneResponseObj != null && addBeneResponseObj.status == "success")
                        {
                            Request.bank_ref_id = addBeneResponseObj.beneficiaryId;
                            Request.updated_by = Request.agent_ref_id;
                            Request.active_status_ref_id = "2";
                            DataSet dstUpdate = _dmtRepo.APTUpdatePayee(Request);
                            if (_commonService.IsValidDataSet(dstUpdate))
                            {
                                if (dstUpdate.Tables[0].Rows[0][0].ToString() == APIResponseCode.DBSuccess)
                                {
                                    objresponse.response_code = APIResponseCode.Success;
                                    objresponse.response_message = "Bene Added Successfully";
                                    objresponse.data = null;
                                }
                                else
                                {
                                    objresponse.response_code = dstUpdate.Tables[0].Rows[0][0].ToString();
                                    objresponse.response_message = dstUpdate.Tables[0].Rows[0][1].ToString();
                                    objresponse.data = null;
                                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Bene Registration Update Failed", JsonConvert.SerializeObject(objresponse), Request.mobile_no);

                                }
                            }
                            else
                            {
                                objresponse.response_code = APIResponseCode.DatasetNull;
                                objresponse.response_message = APIResponseCodeDesc.DatasetNull;
                                objresponse.data = null;
                                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Bene Registration Dataset Null", "", Request.mobile_no);

                            }

                        }
                        else
                        {

                            objresponse.response_code = APIResponseCode.Failed;
                            objresponse.response_message = APIResponseCodeDesc.Failed;
                            objresponse.data = addBeneResponseObj;
                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Bene Registration API Failed", JsonConvert.SerializeObject(objresponse), Request.mobile_no);

                        }

                    }
                    else
                    {
                        objresponse.response_code = dst.Tables[0].Rows[0][0].ToString();
                        objresponse.response_message = dst.Tables[0].Rows[0][1].ToString();
                        objresponse.data = null;
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Bene Registration Data Failed", JsonConvert.SerializeObject(objresponse), Request.mobile_no);

                    }
                }
            }
            catch (Exception ex)
            {
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.mobile_no);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
            }

            return objresponse;
        }

        #endregion AddPayee

        #region GetAllPayee
        public List<APTGetAllPayeeResponse>? GetPayeeFromDB(APTGetAllPayee Request)
        {
            List<APTGetAllPayeeResponse> lst_objPayeeResponse = new List<APTGetAllPayeeResponse>();

            APTGenericResponse objresponse = new APTGenericResponse();
            DataSet dst = _dmtRepo.APTGetAllPayee(Request);
            if (_commonService.IsValidDataSet(dst))
            {
                for (int i = 0; i < dst.Tables[0].Rows.Count; i++)
                {
                    if (dst.Tables[0].Rows[i]["PayeeRefID"].ToString() != "0")
                    {
                        APTGetAllPayeeResponse payeeobj = new APTGetAllPayeeResponse();
                        payeeobj.payee_name = dst.Tables[0].Rows[i]["PayeeName"].ToString();
                        payeeobj.payee_ref_id = dst.Tables[0].Rows[i]["PayeeRefID"].ToString();
                        payeeobj.validated = dst.Tables[0].Rows[i]["Validated"].ToString();
                        payeeobj.bank_name = dst.Tables[0].Rows[i]["BankName"].ToString();
                        payeeobj.status = dst.Tables[0].Rows[i]["Status"].ToString();
                        payeeobj.n_bin = dst.Tables[0].Rows[i]["NBIN"].ToString();
                        payeeobj.account_number = dst.Tables[0].Rows[i]["AccountNumber"].ToString();
                        payeeobj.regn_date = dst.Tables[0].Rows[i]["RegnDate"].ToString();
                        payeeobj.account_number = dst.Tables[0].Rows[i]["AccountNumber"].ToString();
                        payeeobj.activation_date = dst.Tables[0].Rows[i]["ActivationDate"].ToString();
                        payeeobj.active_status_ref_id = dst.Tables[0].Rows[i]["ActiveStatusRefID"].ToString();
                        payeeobj.bank_ref_id = dst.Tables[0].Rows[i]["BankRefID"].ToString();
                        payeeobj.current_balance = dst.Tables[0].Rows[i]["CurrentBalance"].ToString();
                        payeeobj.global_ifsc_code = dst.Tables[0].Rows[i]["GlobalIFSCCode"].ToString();
                        payeeobj.ifsc_bank_branch_ref_id = dst.Tables[0].Rows[i]["IFSCBankBranchRefID"].ToString();
                        payeeobj.ifsc_code = dst.Tables[0].Rows[i]["IFSCCode"].ToString();
                        payeeobj.imps_status_ref_id = dst.Tables[0].Rows[i]["IMPSStatusRefID"].ToString();
                        payeeobj.mobile_no = dst.Tables[0].Rows[i]["MobileNo"].ToString();
                        payeeobj.neft_status_ref_id = dst.Tables[0].Rows[i]["NEFTStatusRefID"].ToString();
                        payeeobj.npci_payee_name = dst.Tables[0].Rows[i]["NPCIPayeeName"].ToString();
                        payeeobj.beneficiaryId = dst.Tables[0].Rows[i]["PaytmRefId"].ToString();
                        payeeobj.isBankflowNeeded = dst.Tables[0].Rows[i]["bankflag"].ToString();
                        lst_objPayeeResponse.Add(payeeobj);
                    }

                }
            }
            else
            {
                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Dataset Empty", JsonConvert.SerializeObject(Request), Request.customer_mobile_no);

            }
            return lst_objPayeeResponse;
        }
        public APTGenericResponse? APTGetAllPayee(APTGetAllPayee Request)
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.customer_mobile_no);

            List<APTGetAllPayeeResponse> lst_objPayeeResponse = new List<APTGetAllPayeeResponse>();
            APTGenericResponse objresponse = new APTGenericResponse();
            try
            {

                lst_objPayeeResponse = GetPayeeFromDB(Request);
                if (lst_objPayeeResponse.Count > 0)
                {
                    for (int j = 0; j < lst_objPayeeResponse.Count; j++)
                    {
                        if (lst_objPayeeResponse[j].isBankflowNeeded.ToUpper() == "YES")
                        {
                            GetBeneListResponse beneListResponse = new GetBeneListResponse();
                            beneListResponse = _paytmAPI.GetBeneList(Request.customer_mobile_no);
                            if (beneListResponse != null && beneListResponse.response_code == "0")
                            {
                                var MissingBene = beneListResponse.beneficiaries.Where(x => !lst_objPayeeResponse.Any(y => y.beneficiaryId == x.beneficiaryId));
                                string MissingBeneobj = JsonConvert.SerializeObject(MissingBene);
                                List<GetBeneListResponseBeneficiary> MissingBeneobjList = JsonConvert.DeserializeObject<List<GetBeneListResponseBeneficiary>>(MissingBeneobj);
                                if (MissingBeneobjList.Count > 0)
                                {
                                    AddBeneToDB(MissingBeneobjList, Request.agent_ref_id, Request.customer_mobile_no);
                                    lst_objPayeeResponse = GetPayeeFromDB(Request);
                                }
                                //List<APTGetAllPayeeResponse> DeleteBene = lst_objPayeeResponse.Where(x => !beneListResponse.beneficiaries.Any(y => y.beneficiaryId == x.beneficiaryId)).ToList();
                                //string DeleteBeneobj = JsonConvert.SerializeObject(DeleteBene);
                                //if (DeleteBene.Count > 0)
                                //{
                                //    for (int i = 0; i < DeleteBene.Count; i++)
                                //    {
                                //        APTDeletePayee delpayeeRequest = new APTDeletePayee();
                                //        delpayeeRequest.payee_ref_id = DeleteBene[i].payee_ref_id;
                                //        delpayeeRequest.mobile_no = Request.customer_mobile_no;
                                //        _dmtRepo.APTDeletePayee(delpayeeRequest);
                                //    }
                                //    lst_objPayeeResponse = GetPayeeFromDB(Request);
                                //}

                                List<APTGetAllPayeeResponse> OnboardToBankBeneObj = lst_objPayeeResponse.Where(x => !beneListResponse.beneficiaries.Any(y => y.beneficiaryId == x.beneficiaryId)).ToList();
                                if (OnboardToBankBeneObj.Count > 0)
                                {
                                    for (int i = 0; i < OnboardToBankBeneObj.Count; i++)
                                    {
                                        APTUpdatePayeeRequest Req = new APTUpdatePayeeRequest();
                                        Req.otp = "";
                                        Req.otp_state = "";
                                        Req.payee_ref_id = OnboardToBankBeneObj[i].payee_ref_id;
                                        Req.mobile_no = Request.customer_mobile_no;
                                        objresponse = ValidatePayeeOTP(Req);
                                    }
                                    lst_objPayeeResponse = GetPayeeFromDB(Request);
                                }
                            }
                            break;
                        }
                       
                    }
                    
                }
                objresponse.response_code = APIResponseCode.Success;
                objresponse.response_message = APIResponseCodeDesc.Success;
                objresponse.data = lst_objPayeeResponse;
            }
            catch (Exception ex)
            {
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.customer_mobile_no);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
            }

            return objresponse;
        }

        #endregion GetAllPayee

        #region Transaction
        public APTGenericResponse? RefundTransaction(RefundOTPRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.mobile_no);

            try
            {
                ValidateOTPRequest validateOTPRequest = new ValidateOTPRequest();
                validateOTPRequest.otp = Request.otp;


                DataSet dst = _dmtRepo.APTValidateRegisterCustomerOTP(validateOTPRequest);
                if (_commonService.IsValidDataSet(dst) && dst.Tables[0].Rows[0][0].ToString() == APIResponseCode.DBSuccess)
                {
                    DataSet dst1 = new DataSet();
                    if (Request.payment_type == "DMT")
                    {
                        dst1 = _dmtRepo.APTRefundTransaction(Request);
                    }
                    else if (Request.payment_type == "PAYOUT")
                    {
                        dst1 = _dmtRepo.APTRefundPayOutTransaction(Request);
                    }

                    if (_commonService.IsValidDataSet(dst1) && dst1.Tables[0].Rows[0][0].ToString() == APIResponseCode.DBSuccess)
                    {
                        objresponse.response_code = APIResponseCode.Success;
                        objresponse.response_message = "Transaction Refunded Succesfully.";
                        objresponse.data = null;
                    }
                    else
                    {
                        objresponse.response_code = dst.Tables[0].Rows[0][0].ToString();
                        objresponse.response_message = dst.Tables[0].Rows[0][1].ToString();
                        objresponse.data = null;
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed Dataset Refund", JsonConvert.SerializeObject(objresponse), Request.mobile_no);

                    }

                }
                else
                {

                    objresponse.response_code = dst.Tables[0].Rows[0][0].ToString();
                    objresponse.response_message = dst.Tables[0].Rows[0][1].ToString();
                    objresponse.data = null;
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed Dataset", JsonConvert.SerializeObject(objresponse), Request.mobile_no);

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
        public APTGenericResponse? Transaction(APTTransactionRequest Request)
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.sender_mobile_number);

            APTGenericResponse objresponse = new APTGenericResponse();
            APTTransactionResponse txnResponse = new APTTransactionResponse();
            try
            {
                decimal TransferAmount = Convert.ToDecimal(Request.amount);

                PaytmPrevalidateCustomerResponse objAPIResponse = new PaytmPrevalidateCustomerResponse();
                objAPIResponse = _paytmAPI.PrevalidateCustomer(Request.sender_mobile_number);
                if (objAPIResponse != null && !string.IsNullOrEmpty(objAPIResponse.limitLeft) && Convert.ToDecimal(objAPIResponse.limitLeft) < TransferAmount)
                {
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Prevalidate Limit Check ", "Request :" + JsonConvert.SerializeObject(Request) + "| Response : " + JsonConvert.SerializeObject(objAPIResponse), Request.sender_mobile_number);

                    objresponse.response_code = "300";
                    objresponse.response_message = "Transfer Amount is greater than the limit available. Current Limit :" + objAPIResponse.limitLeft;
                    objresponse.data = null;
                    return objresponse;
                }
                string mode = "";
                if (Request.pay_mode_ref_id == "1")
                {
                    mode = "imps";
                }
                if (Request.pay_mode_ref_id == "2")
                {
                    mode = "neft";
                }
                int Successcount = 0;
                int IterationCount = 0;
                DataSet dst = _dmtRepo.APTTransaction(Request);
                if (!_commonService.IsValidDataSet(dst))
                {

                    objresponse.response_code = APIResponseCode.DatasetNull;
                    objresponse.response_message = APIResponseCodeDesc.DatasetNull;
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Transaction Insert Failed", "Response :" + JsonConvert.SerializeObject(objresponse), Request.sender_mobile_number);

                    objresponse.data = null;
                }
                else
                {
                    if (dst.Tables[0].Rows[0][0].ToString() == APIResponseCode.DBSuccess)
                    {
                        string TransactionID = dst.Tables[0].Rows[0]["TransactionID"].ToString();
                        string CustomerName = dst.Tables[0].Rows[0]["Customername"].ToString();
                        string Commision = dst.Tables[0].Rows[0]["CustomerFee"].ToString();
                        string BeneName = dst.Tables[0].Rows[0]["payeename"].ToString();


                        decimal AmountToCheck = 5000;

                        decimal TotalAmount = Convert.ToDecimal(Commision) + TransferAmount;



                        for (int i = 0; i < 5; i++)
                        {
                            try
                            {
                                if (TransferAmount > 0)
                                {
                                    IterationCount++;
                                    decimal AmtToTransfer = TransferAmount;
                                    if (TransferAmount > AmountToCheck)
                                    {
                                        AmtToTransfer = AmountToCheck;
                                    }
                                    PaytmDMTTransactionRequest dMTTransactionRequest = new PaytmDMTTransactionRequest();
                                    dMTTransactionRequest.beneficiaryId = dst.Tables[0].Rows[0]["PaytmRefID"].ToString();
                                    dMTTransactionRequest.txnReqId = TransactionID + i;
                                    dMTTransactionRequest.amount = AmtToTransfer;
                                    dMTTransactionRequest.customerMobile = Request.sender_mobile_number;
                                    dMTTransactionRequest.mode = mode;
                                    dMTTransactionRequest.ifscBased = "true";


                                    PaytmDMTTransactionResponse transactionResponseObj = _paytmAPI.DMTTransaction(dMTTransactionRequest);
                                    if (transactionResponseObj != null)
                                    {
                                        APTUpdateTransaction updateRequest = new APTUpdateTransaction();
                                        updateRequest.transaction_id = TransactionID + i;
                                        updateRequest.bank_reference_number = transactionResponseObj.rrn;
                                        updateRequest.request = JsonConvert.SerializeObject(dMTTransactionRequest);
                                        updateRequest.response = JsonConvert.SerializeObject(transactionResponseObj);
                                        updateRequest.response_description = transactionResponseObj.message;
                                        updateRequest.response_code = transactionResponseObj.response_code.ToString();
                                        updateRequest.beneficiary_name = transactionResponseObj.extra_info.beneficiaryName;
                                        //updateRequest.trans_status = transactionResponseObj.status;
                                        DataSet Dst1 = _dmtRepo.APTUpdateTransaction(updateRequest);
                                        if (!_commonService.IsValidDataSet(Dst1))
                                        {
                                            objresponse.response_code = APIResponseCode.DatasetNull;
                                            objresponse.response_message = "Update Transaction Failed for iteration " + i;
                                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Update Transaction Failed for iteration " + i, "Request :" + JsonConvert.SerializeObject(Request), Request.sender_mobile_number);

                                            objresponse.data = null;
                                        }
                                        else
                                        {
                                            if (Dst1.Tables[0].Rows[0][0].ToString() == APIResponseCode.DBSuccess)
                                            {
                                                if (transactionResponseObj.status == "success")
                                                {
                                                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Transaction Success", "Request :" + JsonConvert.SerializeObject(Request), Request.sender_mobile_number);

                                                    Successcount++;
                                                    txnResponse.rrn += "," + transactionResponseObj.rrn.ToString().TrimStart(',');
                                                    txnResponse.transaction_date = transactionResponseObj.transactionDate.ToString();
                                                }
                                                else
                                                {
                                                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Transaction not Success", "Request :" + JsonConvert.SerializeObject(Request) + "| Response :" + JsonConvert.SerializeObject(transactionResponseObj), Request.sender_mobile_number);

                                                }
                                            }
                                            else
                                            {
                                                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Database not Success", "Request :" + JsonConvert.SerializeObject(Request), Request.sender_mobile_number);

                                                objresponse.response_code = dst.Tables[0].Rows[0][0].ToString();
                                                objresponse.response_message = dst.Tables[0].Rows[0][1].ToString();
                                                objresponse.data = null;
                                            }
                                        }
                                    }
                                    TransferAmount = TransferAmount - AmountToCheck;
                                }


                            }
                            catch (Exception ex)
                            {
                                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.sender_mobile_number);

                                objresponse.response_code = APIResponseCode.Exception;
                                objresponse.response_message = APIResponseCodeDesc.Exception;
                                objresponse.data = null;
                            }
                        }
                        txnResponse.transaction_id = TransactionID;
                        txnResponse.customer_name = CustomerName;
                        txnResponse.commision = Commision;
                        txnResponse.bene_acnt_no = Request.account_number_in;
                        txnResponse.bene_name = BeneName;
                        txnResponse.totalAmount = TotalAmount.ToString();
                        txnResponse.amount = Request.amount;
                        if (Successcount == IterationCount)
                        {

                            Successcount++;
                            objresponse.response_code = APIResponseCode.Success;
                            objresponse.response_message = "Transaction Successful";

                            txnResponse.rrn = txnResponse.rrn.TrimStart(',');
                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Transaction Successful", "Request :" + JsonConvert.SerializeObject(Request) + "|Response :" + JsonConvert.SerializeObject(txnResponse), Request.sender_mobile_number);


                            objresponse.data = txnResponse;
                        }
                        else
                        {
                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Transaction Pending", "Request :" + JsonConvert.SerializeObject(Request) + "|Response :" + JsonConvert.SerializeObject(txnResponse), Request.sender_mobile_number);
                            txnResponse.rrn = "NA";
                            objresponse.response_code = APIResponseCode.Success;
                            objresponse.response_message = "Transaction Pending";
                            objresponse.data = txnResponse;
                        }
                    }
                    else
                    {

                        objresponse.response_code = dst.Tables[0].Rows[0][0].ToString();
                        objresponse.response_message = dst.Tables[0].Rows[0][1].ToString();
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "DB not Success", JsonConvert.SerializeObject(objresponse), Request.sender_mobile_number);

                        objresponse.data = null;
                    }
                }
            }
            catch (Exception ex)
            {
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.sender_mobile_number);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
            }
            return objresponse;
        }

        public APTGenericResponse? GetStatus(string TxnId)
        {
            //_logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", TxnId, TxnId);

            APTGenericResponse objresponse = new APTGenericResponse();
            try
            {
                PaytmGetStatusResponse objAPIResponse = new PaytmGetStatusResponse();

                objAPIResponse = _paytmAPI.GetTxnStatus(TxnId);
                if (objAPIResponse != null && objAPIResponse.response_code == 0 && objAPIResponse.status == "success")
                {
                    APTUpdateTransaction updateRequest = new APTUpdateTransaction();
                    updateRequest.transaction_id = TxnId;
                    updateRequest.bank_reference_number = objAPIResponse.rrn;
                    updateRequest.request = "";
                    updateRequest.response = JsonConvert.SerializeObject(objAPIResponse);
                    updateRequest.response_description = objAPIResponse.status;
                    updateRequest.response_code = objAPIResponse.response_code.ToString();
                    //updateRequest.trans_status = transactionResponseObj.status;
                    DataSet Dst1 = _dmtRepo.APTUpdateTransaction(updateRequest);
                    if (!_commonService.IsValidDataSet(Dst1))
                    {
                        objresponse.response_code = APIResponseCode.DatasetNull;
                        objresponse.response_message = APIResponseCodeDesc.DatasetNull;
                        objresponse.data = objAPIResponse;
                        // _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Get STatus Updation Dataset Null", "", TxnId);
                    }
                    else
                    {
                        if (Dst1.Tables[0].Rows[0][0].ToString() == APIResponseCode.DBSuccess)
                        {
                            objresponse.response_code = APIResponseCode.Success;
                            objresponse.response_message = "status Updated successfully";
                            objresponse.data = objAPIResponse;
                        }
                        else
                        {
                            objresponse.response_code = Dst1.Tables[0].Rows[0][0].ToString();
                            objresponse.response_message = Dst1.Tables[0].Rows[0][1].ToString();
                            objresponse.data = objAPIResponse;
                            //_logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Get STatus Updation Failed", JsonConvert.SerializeObject(objresponse), TxnId);

                        }
                    }
                }
                else
                {
                    if (objAPIResponse != null)
                    {
                        objresponse.response_code = APIResponseCode.Failed;
                        objresponse.response_message = APIResponseCodeDesc.Failed;
                        APTUpdateTransaction updateRequest = new APTUpdateTransaction();
                        updateRequest.transaction_id = TxnId;
                        if (string.IsNullOrEmpty(objAPIResponse.rrn))
                        {
                            objAPIResponse.rrn = "";
                        }
                        updateRequest.bank_reference_number = objAPIResponse.rrn;
                        updateRequest.request = "";
                        updateRequest.response = JsonConvert.SerializeObject(objAPIResponse);
                        updateRequest.response_description = objAPIResponse.status;
                        updateRequest.response_code = objAPIResponse.response_code.ToString();
                        //updateRequest.trans_status = transactionResponseObj.status;
                        DataSet Dst1 = _dmtRepo.APTUpdateTransaction(updateRequest);
                        objresponse.data = objAPIResponse;
                    }
                    else
                    {
                        objresponse.response_code = "102";
                        objresponse.response_message = "error from bank getstatus";
                        objresponse.data = objAPIResponse;
                        //_logService.WriteLog(MethodBase.GetCurrentMethod().Name, "null error from GetStatus", JsonConvert.SerializeObject(objresponse), TxnId);
                    }
                }
                // _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "get status dmt Failed", JsonConvert.SerializeObject(objresponse), TxnId);


            }
            catch (Exception ex)
            {
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, TxnId);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
            }

            return objresponse;
        }
        #endregion Transaction

        #region DeletePayee
        public APTGenericResponse? SendOTPForPayeeDeletion(ResendOTPRequest Request)
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.mobile_no);

            APTGenericResponse objresponse = new APTGenericResponse();
            try
            {
                objresponse = SendPaytmOTP(Request.mobile_no, APIConstants.BeneDeletion);
                if (objresponse.response_code == APIResponseCode.Success)
                {
                    objresponse.response_code = APIResponseCode.Success;
                    objresponse.response_message = "OTP Sent To Registered Number. Please validate OTP";
                }
                else
                {
                    objresponse.response_code = "102";
                    objresponse.response_message = "Sending OTP From Bank Failed. Please Try Again";
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Sending SMS for Bene Deletion Failed", JsonConvert.SerializeObject(objresponse), Request.mobile_no);
                }

            }
            catch (Exception ex)
            {
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.mobile_no);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
            }

            return objresponse;
        }

        public APTGenericResponse? DeletePayee(APTDeletePayee Request)
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.mobile_no);

            APTGenericResponse objresponse = new APTGenericResponse();
            try
            {
               
                    DataSet dst = _dmtRepo.APTDeletePayee(Request);
                    if (!_commonService.IsValidDataSet(dst))
                    {
                        objresponse.response_code = APIResponseCode.DatasetNull;
                        objresponse.response_message = APIResponseCodeDesc.DatasetNull;
                        objresponse.data = null;
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Payee Deletion Dataset Null", "", Request.mobile_no);
                    }
                    else
                    {
                        if (dst.Tables[0].Rows[0][0].ToString() == APIResponseCode.DBSuccess)
                        {
                            objresponse.response_code = APIResponseCode.Success;
                            objresponse.response_message = "Bene Deleted Successfully";
                            objresponse.data = null;
                        }
                        else
                        {
                            objresponse.response_code = dst.Tables[0].Rows[0][0].ToString();
                            objresponse.response_message = dst.Tables[0].Rows[0][1].ToString();
                            objresponse.data = null;
                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Bene Deletion Update Failed", JsonConvert.SerializeObject(objresponse), Request.mobile_no);

                        }
                    }
               
                //_logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Bene Deletion Failed", JsonConvert.SerializeObject(objAPIRequest), Request.mobile_no);


            }
            catch (Exception ex)
            {
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.mobile_no);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
            }

            return objresponse;
        }
        #endregion DeletePayee

        #endregion DMT






    }
}
