using APT.DMT.API.BusinessLogic;
using APT.DMT.API.BusinessLogic.BankService;
using APT.DMT.API.BusinessObjects;
using APT.DMT.API.BusinessObjects.Models;
using APT.DMT.API.Businessstrings.Models;
using APT.DMT.API.DataAccessLayer.DMT;
using APT.PaymentServices.API.BusinessObjects.Models;
using APT.PaymentServices.API.DataAccessLayer;
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

namespace APT.PaymentServices.API.BusinessLogic
{
    public class CustomerAppService
    {
        private readonly string SessionValidationURL;
        private readonly string IFSCLookupURL;

        public static IConfiguration Configuration { get; set; }
        public static CommonService _commonService = new CommonService();

        public static CustomerAppRepository _custRepo = new CustomerAppRepository();
        PaymentGatewayService _pgService = new PaymentGatewayService();
        public static LogService _logService = new LogService();

        public CustomerAppService()
        {
            Configuration = _commonService.GetConfiguration();
            SessionValidationURL = Configuration["SessionValidation:Domain"] + Configuration["SessionValidation:URL"];
            IFSCLookupURL = Configuration["Cache:Domain"] + Configuration["Cache:IFSCLookupURL"];

        }
        public APTGenericResponse? APTAddPayeeCustApp(APTInsertPayeeCustomerAppReq Request)
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.mobile_number);

            APTGenericResponse objresponse = new APTGenericResponse();
            try
            {
                
                DataSet dst = _custRepo.APTInsertPayee(Request);
                if (_commonService.IsValidDataSet(dst))
                {
                    if (dst.Tables[0].Rows[0][0].ToString() == "100")
                    {
                        objresponse.response_code = "200";
                        objresponse.response_message = "Payee Added Successfully";
                        return objresponse;
                    }
                    else
                    {

                        objresponse.response_code = dst.Tables[0].Rows[0][0].ToString();
                        objresponse.response_message = dst.Tables[0].Rows[0][1].ToString();
                        objresponse.data = null;
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Add APTAddPayeeCustApp Failed", JsonConvert.SerializeObject(objresponse), Request.mobile_number);

                    }
                }
                else
                {
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Add APTAddPayeeCustApp Dataset Null", JsonConvert.SerializeObject(objresponse), Request.mobile_number);

                    objresponse.response_code = APIResponseCode.DatasetNull;
                    objresponse.response_message = APIResponseCodeDesc.DatasetNull;
                    objresponse.data = null;
                }

            }
            catch (Exception ex)
            {
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.mobile_number);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
            }

            return objresponse;
        }

        public List<APTGetAllPayeeCustAppResponse>? GetPayeeFromDB(APTGetAllPayeeCustomerAppReq Request)
        {
            List<APTGetAllPayeeCustAppResponse> lst_objPayeeResponse = new List<APTGetAllPayeeCustAppResponse>();

            APTGenericResponse objresponse = new APTGenericResponse();
            DataSet dst = _custRepo.APTGetAllPayee(Request);
            if (_commonService.IsValidDataSet(dst))
            {
                for (int i = 0; i < dst.Tables[0].Rows.Count; i++)
                {
                    if (dst.Tables[0].Rows[i]["PayeeRefID"].ToString() != "0")
                    {
                        APTGetAllPayeeCustAppResponse payeeobj = new APTGetAllPayeeCustAppResponse();
                        payeeobj.payee_ref_id = dst.Tables[0].Rows[i]["PayeeRefID"].ToString();
                        payeeobj.mobile_no = dst.Tables[0].Rows[i]["MobileNumber"].ToString();
                        payeeobj.payee_name = dst.Tables[0].Rows[i]["PayeeName"].ToString();
                        payeeobj.mail_id = dst.Tables[0].Rows[i]["EmailID"].ToString();
                        payeeobj.account_number = dst.Tables[0].Rows[i]["AccountNumber"].ToString();
                        payeeobj.account_type = dst.Tables[0].Rows[i]["AccountType"].ToString();
                        payeeobj.ifsc_code = dst.Tables[0].Rows[i]["IFSCCode"].ToString();
                        payeeobj.bank_name = dst.Tables[0].Rows[i]["BankName"].ToString();
                        payeeobj.branch_name = dst.Tables[0].Rows[i]["BranchName"].ToString();
                        lst_objPayeeResponse.Add(payeeobj);
                    }

                }
            }
            else
            {
                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Dataset Empty", JsonConvert.SerializeObject(Request), Request.Customer_ref_id);

            }
            return lst_objPayeeResponse;
        }

        public APTGenericResponse? APTGetAllPayee(APTGetAllPayeeCustomerAppReq Request)
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.Customer_ref_id);

            List<APTGetAllPayeeCustAppResponse> lst_objPayeeResponse = new List<APTGetAllPayeeCustAppResponse>();
            APTGenericResponse objresponse = new APTGenericResponse();
            try
            {

                lst_objPayeeResponse = GetPayeeFromDB(Request);

                objresponse.response_code = APIResponseCode.Success;
                objresponse.response_message = APIResponseCodeDesc.Success;
                objresponse.data = lst_objPayeeResponse;
            }
            catch (Exception ex)
            {
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.Customer_ref_id);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
            }

            return objresponse;
        }

        public APTGenericResponse? APTDashboardAPI(APTDashboardRequest Request)
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.mobile_number);
            APTDashboardResponse Resp = new APTDashboardResponse();
            APTGenericResponse objresponse = new APTGenericResponse();
            Resp.last5txns = new List<APTDashboardReport>();
            try
            {

                DataSet Dst = _custRepo.APTDashboardAPI(Request);
                if(Dst != null)
                {
                    Resp.settlement_balance = Dst.Tables[0].Rows[0]["SettlementBalance"].ToString();
                    for(int i=0;i< Dst.Tables[1].Rows.Count; i++)
                    {
                        APTDashboardReport request = new APTDashboardReport();
                        request.transaction_id = Dst.Tables[1].Rows[i]["TransactionID"].ToString();
                        //request.created_date = Dst.Tables[1].Rows[0]["CreatedDate"].ToString();
                        request.transaction_date = Dst.Tables[1].Rows[i]["CreatedDate"].ToString();
                        request.bank_name = Dst.Tables[1].Rows[i]["BankName"].ToString();
                        request.branch_name = Dst.Tables[1].Rows[i]["BranchName"].ToString();
                        request.account_number = Dst.Tables[1].Rows[i]["AccountNumber"].ToString();
                        request.customer_name = Dst.Tables[1].Rows[i]["CustomerName"].ToString();
                        request.payee_name = Dst.Tables[1].Rows[i]["PayeeName"].ToString();
                        request.customer_mobile_number = Dst.Tables[1].Rows[i]["CustomerMobileNumber"].ToString();
                        request.payee_mobile_number = Dst.Tables[1].Rows[i]["PayeeMobileNumber"].ToString();
                        Resp.last5txns.Add(request);
                    }
                }

                objresponse.response_code = APIResponseCode.Success;
                objresponse.response_message = APIResponseCodeDesc.Success;
                objresponse.data = Resp;
            }
            catch (Exception ex)
            {
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.mobile_number);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
            }

            return objresponse;
        }

        public APTGenericResponse? APTCustomerPaymentTransaction(APTCustomerAppTransactionRequest Request)
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.agent_mobile);
            InsertPGTransactionRequest InsertPg = new InsertPGTransactionRequest();
            APTGenericResponse Response = new APTGenericResponse();
            try
            {
                InsertPg.transfer_amount = Request.amount;
                InsertPg.customer_phone = Request.customer_mobile_number;
                InsertPg.agent_ref_id = "";
                InsertPg.account_type_ref_id = "4";
                InsertPg.channel_type_ref_id = "1";
                InsertPg.agent_name = "";
                InsertPg.agent_mobile_number = Request.agent_mobile;
                InsertPg.pg_service_id = 5;
                Response = _pgService.APTCreatePaymentGatewayOrder_CustPayout(InsertPg);
                if(Response.response_code == "200")
                {
                    InsertPGTransactionResponse obj = new InsertPGTransactionResponse();
                    obj = (InsertPGTransactionResponse)Response.data;
                    Request.pg_plateform_bank_id = obj.bank_id;
                    Request.bank_type_id = obj.bank_id;
                    Request.pg_transaction_id = obj.cp_transaction_id;
                    Request.vendor_payment_sessionid = obj.vendor_session_id;
                    DataSet Dst = _custRepo.APTCustomerPaymentTransaction(Request);
                    if(Dst != null)
                    {
                        if (Dst.Tables[0].Rows.Count > 0 && Dst.Tables[0].Rows[0][0].ToString() == "100")
                        {
                            return Response;
                        }
                        else
                        {
                            Response.response_code = "102";
                            Response.response_message = "Error from Backend ";
                        }
                    }
                }
                else
                {
                    return Response;
                }

            }
            catch (Exception ex)
            {
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.agent_mobile);

                Response.response_code = APIResponseCode.Exception;
                Response.response_message = APIResponseCodeDesc.Exception;
                Response.data = null;
            }

            return Response;
        }


    }
}
