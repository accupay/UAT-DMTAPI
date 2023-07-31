using APT.DMT.API.BusinessLogic;
using APT.DMT.API.BusinessLogic.BankService;
using APT.DMT.API.BusinessObjects;
using APT.DMT.API.BusinessObjects.Models;
using APT.DMT.API.Businessstrings.Models;
using APT.DMT.API.DataAccessLayer;
using APT.DMT.API.DataAccessLayer.DMT;
using APT.DMT.API.DataAccessLayer.Paytout;
using APT.PaymentServices.API.BusinessObjects.Models;
using APT.PaymentServices.API.DataAccessLayer;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;
using File = System.IO.File;

namespace APT.PaymentServices.API.BusinessLogic
{
    public class ECollectService
    {
        public static IConfiguration Configuration { get; set; }
        public static PaymentGatewayRepository _pgRepo = new PaymentGatewayRepository();
        public static ECollectRepository _ECollRepo = new ECollectRepository();
        public static LogService _logService = new LogService();
        public static CommonService _commonService = new CommonService();
        public ECollectService()
        {
            Configuration = GetConfiguration();
        }
        public IConfiguration GetConfiguration()
        {
            Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();
            return Configuration;
        }

        public APTGenericResponse? APTNotifyPayment(YesbankECNewRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            UpdatePGTransactionRequest TReq = new UpdatePGTransactionRequest();
            try
            {
                TReq.bank_ref_id = "8";
                TReq.topup_mode_ref_id = "27";
                string Mobilenum = Request.notify.bene_account_no;
                Mobilenum = Mobilenum.Substring(10, Mobilenum.Length - 10);
                TReq.agent_mobile_number = Mobilenum;
                TReq.vendor_payment_id = Request.notify.transfer_unique_no;
                TReq.agent_name = Request.notify.rmtr_full_name;
                TReq.transfer_amount = Request.notify.transfer_amt.ToString();
                if (Request.notify.status== "CREDITED")
                {
                    TReq.status = "2";
                    TReq.status_desc = "Success";
                    ECollectDataInsertRequest insertRequest = new ECollectDataInsertRequest();
                    insertRequest.IFSCCode = Request.notify.bene_account_ifsc;
                    insertRequest.AccountNumber = Request.notify.rmtr_account_no;
                    insertRequest.Amount = Request.notify.transfer_amt;
                    insertRequest.RemitterName = Request.notify.rmtr_full_name;
                    insertRequest.MobileNumber = Mobilenum;

                    DataSet InsertRecord = _ECollRepo.ECollectInsert(insertRequest);
                    DataSet topup_dst = _pgRepo.APTPaymentGatewayTopup(TReq);
                    if (_commonService.IsValidDataSet(topup_dst))
                    {
                        if (topup_dst.Tables[0].Rows[0][0].ToString() == "100")
                        {
                            objresponse.response_code = APIResponseCode.Success;
                            objresponse.response_message = "Transaction Successful";
                            objresponse.data = Request.notify.transfer_unique_no;
                            return objresponse;
                        }
                        else
                        {
                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Not a API Response - topup fail dst", JsonConvert.SerializeObject(Request), Request.notify.bene_account_no);
                            objresponse.response_code = topup_dst.Tables[0].Rows[0][0].ToString();
                            objresponse.response_message = topup_dst.Tables[0].Rows[0][1].ToString();
                            objresponse.data = null;
                            return objresponse;
                        }
                    }
                    else
                    {
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Not a API Response - topup fail", JsonConvert.SerializeObject(Request), Request.notify.bene_account_no);

                        objresponse.response_code = APIResponseCode.DatasetNull;
                        objresponse.response_message = APIResponseCodeDesc.DatasetNull;
                        objresponse.data = null;
                        return objresponse;
                    }
                }
                else
                {
                    TReq.status = "1";
                    TReq.status_desc = "Failed";
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Not a API Response - topup fail", JsonConvert.SerializeObject(Request), Request.notify.bene_account_no);

                    objresponse.response_code = APIResponseCode.DatasetNull;
                    objresponse.response_message = APIResponseCodeDesc.DatasetNull;
                    objresponse.data = null;
                    return objresponse;
                }
                
               
               
            }
            catch(Exception ex)
            {
                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Not a API Response - topup fail", JsonConvert.SerializeObject(Request), Request.notify.bene_account_no);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                return objresponse;
            }
        }

        public APTGenericResponse? APTEcollectAccountAddition(ECOllectAccountAdditionRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            ECOllectAccountAdditionResponse Resp = new ECOllectAccountAdditionResponse();
            try
            {
                DataSet Dst = _ECollRepo.ECollectAccountAddition(Request);
                if( Dst != null  )
                {
                    if (Dst.Tables[0].Rows[0]["ResponseCode"].ToString() == "100")
                    {
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Success", JsonConvert.SerializeObject(Request), Request.agent_mobile);
                        Resp.account_ref_id = Dst.Tables[0].Rows[0]["AccountRefID"].ToString();
                        Resp.bank_account_id = Dst.Tables[0].Rows[0]["BankAccountID"].ToString();
                        objresponse.response_code = APIResponseCode.Success;
                        objresponse.response_message = APIResponseCodeDesc.Success;
                        objresponse.data = Resp;
                        return objresponse;

                    }
                    else
                    {
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed in DB", JsonConvert.SerializeObject(Request), Request.agent_mobile);

                        objresponse.response_code = Dst.Tables[0].Rows[0]["ResponseCode"].ToString();
                        objresponse.response_message = Dst.Tables[0].Rows[0]["ResponseDescription"].ToString();
                        objresponse.data = null;
                        return objresponse;
                    }
                }
                else
                {
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "DB Null", JsonConvert.SerializeObject(Request), Request.agent_mobile);

                    objresponse.response_code = APIResponseCode.DatasetNull;
                    objresponse.response_message = APIResponseCodeDesc.DatasetNull;
                    objresponse.data = null;
                    return objresponse;
                }
            }
            catch(Exception ex)
            {
                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Account Addition Exception", JsonConvert.SerializeObject(Request), Request.agent_mobile);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                return objresponse;
            }
        }

        public APTGenericResponse? APTEcollectViewAccount(ECOllectViewAccountRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            List<ECOllectViewAccountResponse> Resp = new List<ECOllectViewAccountResponse>();
            try
            {
                DataSet dst = _ECollRepo.ECollectViewAccountAddition(Request);
                if (dst != null)
                {
                    if (dst.Tables[0].Rows.Count > 0)
                    {
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Success", JsonConvert.SerializeObject(Request), Request.agent_ref_id);
                        for (int i = 0; i < dst.Tables[0].Rows.Count; i++)
                        {
                            ECOllectViewAccountResponse objResponse = new ECOllectViewAccountResponse();
                            objResponse.agent_mobile = dst.Tables[0].Rows[i]["AgentMobileNo"].ToString();
                            objResponse.agency_name = dst.Tables[0].Rows[i]["AgentName"].ToString();
                            objResponse.agency_name = dst.Tables[0].Rows[i]["AgencyName"].ToString();
                            objResponse.bank_name = dst.Tables[0].Rows[i]["BankName"].ToString();
                            objResponse.Bank_account_number = dst.Tables[0].Rows[i]["BankAccountNumber"].ToString();
                            objResponse.account_holder_name = dst.Tables[0].Rows[i]["AccountHolderName"].ToString();
                            objResponse.ifsc_code = dst.Tables[0].Rows[i]["IFSCCode"].ToString();
                            objResponse.bank_copy_file = dst.Tables[0].Rows[i]["BankCopyfile"].ToString();
                            objResponse.bank_account_id = dst.Tables[0].Rows[i]["BankAccountId"].ToString();
                            objResponse.account_validation_status = dst.Tables[0].Rows[i]["AccountValidationStatus"].ToString();
                            objResponse.transfer_amount = dst.Tables[0].Rows[i]["TransferAmount"].ToString();
                            objResponse.paymode = dst.Tables[0].Rows[i]["paymode"].ToString();
                            objResponse.created_date = dst.Tables[0].Rows[i]["CreatedDate"].ToString();
                            objResponse.updated_date = dst.Tables[0].Rows[i]["Updateddate"].ToString();
                            objResponse.npci_name = dst.Tables[0].Rows[i]["NPCIName"].ToString();
                            objResponse.name_match_status = dst.Tables[0].Rows[i]["NameMatchStatus"].ToString();

                            Resp.Add(objResponse);
                        }

                        //Resp.account_ref_id = Dst.Tables[0].Rows[0]["AccountRefID"].ToString();
                        //Resp.bank_account_id = Dst.Tables[0].Rows[0]["BankAccountID"].ToString();
                        objresponse.response_code = APIResponseCode.Success;
                        objresponse.response_message = APIResponseCodeDesc.Success;
                        objresponse.data = Resp;
                        return objresponse;

                    }
                    else
                    {
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed in DB", JsonConvert.SerializeObject(Request), Request.agent_ref_id);

                        objresponse.response_code = "102";
                        objresponse.response_message = "No Record found";
                        objresponse.data = null;
                        return objresponse;
                    }
                }
                else
                {
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "DB Null", JsonConvert.SerializeObject(Request), Request.agent_ref_id);

                    objresponse.response_code = APIResponseCode.DatasetNull;
                    objresponse.response_message = APIResponseCodeDesc.DatasetNull;
                    objresponse.data = null;
                    return objresponse;
                }
            }
            catch (Exception ex)
            {
                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Account Addition Exception", JsonConvert.SerializeObject(Request), Request.agent_ref_id);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                return objresponse;
            }
        }

        public APTGenericResponse? APTDepositSlipUpload(DepositSlipUploadRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            DepositSlipUploadResponse depResp = new DepositSlipUploadResponse();
            try
            {
                DataSet dst = _ECollRepo.DepositSlipUpload(Request);
                if (dst != null || dst.Tables[0].Rows.Count > 0)
                {
                    if (dst.Tables[0].Rows[0]["ReponseCode"].ToString() == "100")
                    {

                        depResp.deposit_slip_refid = dst.Tables[0].Rows[0]["DepositSlipRefID"].ToString();
                        objresponse.response_code = "200";
                        objresponse.response_message = "Success";
                        objresponse.data = depResp;
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Success Resp : " + JsonConvert.SerializeObject(objresponse), JsonConvert.SerializeObject(Request), Request.mobile_number);
                        return objresponse;
                    }
                    else
                    {
                        objresponse.response_code = dst.Tables[0].Rows[0]["ReponseCode"].ToString();
                        objresponse.response_message = dst.Tables[0].Rows[0]["ReponseDescription"].ToString();
                        objresponse.data = null;
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Failure Resp : "+ JsonConvert.SerializeObject(objresponse), JsonConvert.SerializeObject(Request), Request.mobile_number);
                        return objresponse;
                    }
                   
                }
                else
                {
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "DB Null", JsonConvert.SerializeObject(Request), Request.mobile_number);

                    objresponse.response_code = APIResponseCode.DatasetNull;
                    objresponse.response_message = APIResponseCodeDesc.DatasetNull;
                    objresponse.data = null;
                    return objresponse;
                }
            }
            catch (Exception ex)
            {
                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "APTDepositSlipUpload Exception", JsonConvert.SerializeObject(Request), Request.mobile_number);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                return objresponse;
            }
        }


    }
}
