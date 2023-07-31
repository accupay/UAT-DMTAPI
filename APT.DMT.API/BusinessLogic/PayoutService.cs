using APT.DMT.API.BusinessLogic.BankService;
using APT.DMT.API.BusinessObjects;
using APT.DMT.API.BusinessObjects.Models;
using APT.DMT.API.Businessstrings.Models;
using APT.DMT.API.DataAccessLayer;
using APT.DMT.API.DataAccessLayer.DMT;
using APT.DMT.API.DataAccessLayer.Paytout;
using APT.PaymentServices.API.DataAccessLayer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;
using File = System.IO.File;
namespace APT.DMT.API.BusinessLogic
{
    public class PayoutService
    {
        public static AxisPayOut _axisservice = new AxisPayOut();
        public static YesPayOut _yesbanksevice = new YesPayOut();
        public static PayoutRepository _payoutRepo = new PayoutRepository();
        public static PaytmVendorAPI _paytmAPI = new PaytmVendorAPI();
        public static LogService _logService = new LogService();
        public static CommonService _commonService = new CommonService();
        public static CommonRepository _commRepo = new CommonRepository();
        public static DMTRepository _dmtRepo = new DMTRepository();
        public static IConfiguration Configuration { get; set; }
        #region RegisterCustomer

        public PayoutService()
        {
            Configuration = GetConfiguration();
        }
        #endregion RegisterCustomer

        public IConfiguration GetConfiguration()
        {
            Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();
            return Configuration;
        }
        public APTGenericResponse? PayoutFundTransfer(PayoutPaymentRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            APTPayOutTransactionResponse Resp = new APTPayOutTransactionResponse();
            AxisPaymentRequest values = new AxisPaymentRequest();
            YesBankPayRequest yesValues = new YesBankPayRequest();
            string commission = "";
            
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.sendermobilenumber);

            try
            {
                DataSet dst = _payoutRepo.PayoutFundTransfer(Request);

                if (dst.Tables[0].Rows[0][0].ToString() == "101")
                {
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Dataset Empty", JsonConvert.SerializeObject(Request), Request.sendermobilenumber);

                    objresponse.response_code = dst.Tables[0].Rows[0][0].ToString();
                    objresponse.response_message = dst.Tables[0].Rows[0][1].ToString();
                    objresponse.data = null;
                } // Axis bank Start
                else if(dst.Tables[0].Rows[0]["Walletflag"].ToString() == "1") // Internal Wallet Movement no need to go to bank 
                {
                    commission = dst.Tables[0].Rows[0]["CustomerFee"].ToString();
                    decimal TotalAmount = Convert.ToDecimal(commission) + Convert.ToDecimal(dst.Tables[0].Rows[0]["Amount"].ToString());

                    Resp.transaction_date = DateTime.Now.ToString("dd/MM/yyyy");
                    Resp.transaction_id = dst.Tables[0].Rows[0]["TransactionID"].ToString();
                    Resp.rrn = dst.Tables[0].Rows[0]["TransactionID"].ToString();
                    Resp.customer_name = dst.Tables[0].Rows[0]["BeneName"].ToString();
                    Resp.commision = commission;
                    Resp.bene_acnt_no = dst.Tables[0].Rows[0]["BeneAccountNumber"].ToString();
                    Resp.bene_name = dst.Tables[0].Rows[0]["BeneName"].ToString();
                    Resp.totalAmount = TotalAmount.ToString();
                    Resp.amount = dst.Tables[0].Rows[0]["Amount"].ToString();

                    objresponse.response_code = "200";
                    objresponse.response_message = "Success";
                    objresponse.data = Resp;
                    return objresponse;
                }
                else if (dst.Tables[0].Rows[0][0].ToString() == "100" && dst.Tables[0].Rows[0][5].ToString() == "3")
                {
                    values.txnPaymode = dst.Tables[0].Rows[0][8].ToString();
                    values.txnAmount = dst.Tables[0].Rows[0][6].ToString();
                    values.beneName = dst.Tables[0].Rows[0][9].ToString();
                    values.beneCode = dst.Tables[0].Rows[0][10].ToString(); //49395
                    values.beneAccNum = dst.Tables[0].Rows[0][11].ToString();
                    values.beneIfscCode = dst.Tables[0].Rows[0][12].ToString();
                    values.beneBankName = dst.Tables[0].Rows[0][13].ToString();
                    values.transactionID = dst.Tables[0].Rows[0][2].ToString();
                    commission = dst.Tables[0].Rows[0]["CustomerFee"].ToString();
                    //APTSendOTP(values.txnAmount,Request.sendermobilenumber, values.beneName, values.transactionID);
                    CombinedResponse paymentRespnse = _axisservice.FundTransfer(values);
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Axis Payout Request Response", JsonConvert.SerializeObject(paymentRespnse), Request.sendermobilenumber + "a");
                    if (paymentRespnse.ApiResponse == "Failed")
                    {
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed!... Something Wrong...Please Contact Admin", JsonConvert.SerializeObject(paymentRespnse), Request.sendermobilenumber);

                        objresponse.response_code = "201";
                        objresponse.response_message = "Failed";
                        objresponse.data = "Failed!... Something Wrong...Please Contact Admin";
                        return objresponse;
                    }
                    var transactionID = dst.Tables[0].Rows[0][2].ToString();
                    TransferResponseAxis responseStatus = JsonConvert.DeserializeObject<TransferResponseAxis>(paymentRespnse.ApiResponse);

                    UpdateTxnStatusPayout objResponse = new UpdateTxnStatusPayout();
                    objResponse.TransactionID = transactionID;
                    objResponse.BankReferrenceNumber = "";
                    objResponse.Request = paymentRespnse.ApiRequest;
                    objResponse.Response = paymentRespnse.ApiResponse;
                    objResponse.ResponseCode = responseStatus.status;
                    objResponse.ResponseDescription = responseStatus.message;
                    if (responseStatus.status.ToUpper() == "F") { objResponse.TranStatus = "3"; } else if (responseStatus.status.ToUpper() == "S") { objResponse.TranStatus = "2"; }
                    objResponse.Flag = "1";
                    objResponse.BankTypeID = dst.Tables[0].Rows[0][5].ToString();


                    //DataSet dsts = _payoutRepo.PayoutUpdateResponse(objResponse);

                    if (responseStatus.status.ToUpper() == "F")
                    {
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", JsonConvert.SerializeObject(paymentRespnse), Request.sendermobilenumber);

                        Dictionary<string, string> response = new Dictionary<string, string>();
                        response.Add("result", paymentRespnse.ApiResponse);
                        response.Add("transactionID", transactionID);
                        objresponse.response_code = "201";
                        objresponse.response_message = "Failed";
                        objresponse.data = paymentRespnse.ApiResponse;
                        DataSet dsts = _payoutRepo.PayoutUpdateResponse(objResponse);
                    }
                    else if (responseStatus.status.ToUpper() == "S")
                    {
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Error", JsonConvert.SerializeObject(paymentRespnse), Request.sendermobilenumber);

                        Thread.Sleep(30000);
                        decimal TotalAmount = Convert.ToDecimal(commission) + Convert.ToDecimal(values.txnAmount);
                        UpdateTxnStatusPayout objRequestStatus = new UpdateTxnStatusPayout();


                        CombinedResponse statusResponse = _axisservice.CheckStatus(transactionID);
                        if (statusResponse.ApiResponse == "Failed")
                        {
                            objresponse.response_code = "201";
                            objresponse.response_message = "Error";
                            objresponse.data = "Failed!... Something Wrong...Please Contact Admin";
                            return objresponse;
                        }
                        GetStatusResponseBody axisCheckResponse = JsonConvert.DeserializeObject<GetStatusResponseBody>(statusResponse.ApiResponse);
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Error", JsonConvert.SerializeObject(axisCheckResponse), Request.sendermobilenumber);

                        if (axisCheckResponse.status.ToUpper() == "S")
                        {
                            if (axisCheckResponse.data.CUR_TXN_ENQ.Count > 0)
                            {
                                if (axisCheckResponse.data.CUR_TXN_ENQ[0].transactionStatus == "PROCESSED")
                                {
                                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Error", JsonConvert.SerializeObject(axisCheckResponse), Request.sendermobilenumber);

                                    objRequestStatus.TransactionID = transactionID;
                                    objRequestStatus.BankReferrenceNumber = axisCheckResponse.data.CUR_TXN_ENQ[0].utrNo;
                                    objRequestStatus.Request = statusResponse.ApiRequest;
                                    objRequestStatus.Response = statusResponse.ApiResponse;
                                    objRequestStatus.ResponseCode = axisCheckResponse.data.CUR_TXN_ENQ[0].transactionStatus;
                                    objRequestStatus.ResponseDescription = axisCheckResponse.data.CUR_TXN_ENQ[0].statusDescription;
                                    objRequestStatus.TranStatus = "2"; // Success
                                    objRequestStatus.Flag = "2";
                                    objRequestStatus.BankTypeID = dst.Tables[0].Rows[0][5].ToString();


                                    DataSet dataset = _payoutRepo.PayoutUpdateResponse(objRequestStatus);
                                    try
                                    {
                                        if (dataset.Tables[0].Rows[0]["smsflag"].ToString() == "2")
                                        {
                                            if (dataset.Tables[0].Rows.Count > 0)
                                            {
                                                string Amount = dataset.Tables[0].Rows[0]["amount"].ToString();
                                                string PayeeName = dataset.Tables[0].Rows[0]["payeename"].ToString();
                                                string MobileNo = dataset.Tables[0].Rows[0]["SenderMobileNumber"].ToString();
                                                string RefId = dataset.Tables[0].Rows[0]["Refid"].ToString();

                                                APTSendOTP(Amount, MobileNo, PayeeName, RefId);
                                            }
                                            else
                                            {

                                            }
                                        }

                                    }
                                    catch { }
                                    Resp.transaction_date = DateTime.Now.ToString("dd/MM/yyyy");
                                    Resp.transaction_id = transactionID;
                                    Resp.rrn = axisCheckResponse.data.CUR_TXN_ENQ[0].utrNo;
                                    Resp.customer_name = values.beneName;
                                    Resp.commision = commission;
                                    Resp.bene_acnt_no = values.beneAccNum;
                                    Resp.bene_name = values.beneName;
                                    Resp.totalAmount = TotalAmount.ToString();
                                    Resp.amount = values.txnAmount;

                                    objresponse.response_code = "200";
                                    objresponse.response_message = "Success";
                                    objresponse.data = Resp;
                                }
                                else if (axisCheckResponse.data.CUR_TXN_ENQ[0].transactionStatus == "REJECTED" || axisCheckResponse.data.CUR_TXN_ENQ[0].transactionStatus == "RETURN" || axisCheckResponse.data.CUR_TXN_ENQ[0].transactionStatus == "FAILED")
                                {
                                    objRequestStatus.TransactionID = transactionID;
                                    objRequestStatus.BankReferrenceNumber = axisCheckResponse.data.CUR_TXN_ENQ[0].utrNo;
                                    objRequestStatus.Request = statusResponse.ApiRequest;
                                    objRequestStatus.Response = statusResponse.ApiResponse;
                                    objRequestStatus.ResponseCode = axisCheckResponse.data.CUR_TXN_ENQ[0].transactionStatus;
                                    objRequestStatus.ResponseDescription = axisCheckResponse.data.CUR_TXN_ENQ[0].statusDescription;
                                    objRequestStatus.TranStatus = "3"; // Failed
                                    objRequestStatus.Flag = "2";
                                    objRequestStatus.BankTypeID = dst.Tables[0].Rows[0][5].ToString();

                                    DataSet dataset = _payoutRepo.PayoutUpdateResponse(objRequestStatus);

                                    objresponse.response_code = "202";
                                    objresponse.response_message = "Transaction Failed";
                                    objresponse.data = "Your payment request is FAILED";
                                }
                                else if (axisCheckResponse.data.CUR_TXN_ENQ[0].transactionStatus == "PENDING")
                                {

                                    objRequestStatus.TransactionID = transactionID;
                                    objRequestStatus.BankReferrenceNumber = axisCheckResponse.data.CUR_TXN_ENQ[0].utrNo;
                                    objRequestStatus.Request = statusResponse.ApiRequest;
                                    objRequestStatus.Response = statusResponse.ApiResponse;
                                    objRequestStatus.ResponseCode = axisCheckResponse.data.CUR_TXN_ENQ[0].transactionStatus;
                                    objRequestStatus.ResponseDescription = axisCheckResponse.data.CUR_TXN_ENQ[0].statusDescription;
                                    objRequestStatus.TranStatus = "1"; // Pending
                                    objRequestStatus.Flag = "2";
                                    objRequestStatus.BankTypeID = dst.Tables[0].Rows[0][5].ToString();

                                    DataSet dataset = _payoutRepo.PayoutUpdateResponse(objRequestStatus);
                                    try
                                    {
                                        if (dataset.Tables[0].Rows[0]["smsflag"].ToString() == "2")
                                        {
                                            if (dataset.Tables[0].Rows.Count > 0)
                                            {
                                                string Amount = dataset.Tables[0].Rows[0]["amount"].ToString();
                                                string PayeeName = dataset.Tables[0].Rows[0]["payeename"].ToString();
                                                string MobileNo = dataset.Tables[0].Rows[0]["SenderMobileNumber"].ToString();
                                                string RefId = dataset.Tables[0].Rows[0]["Refid"].ToString();

                                                APTSendOTP(Amount, MobileNo, PayeeName, RefId);
                                            }
                                            else
                                            {

                                            }
                                        }

                                    }
                                    catch { }
                                    Resp.transaction_date = DateTime.Now.ToString("dd/MM/yyyy");
                                    Resp.transaction_id = transactionID;
                                    Resp.rrn = axisCheckResponse.data.CUR_TXN_ENQ[0].utrNo;
                                    Resp.customer_name = values.beneName;
                                    Resp.commision = commission;
                                    Resp.bene_acnt_no = values.beneAccNum;
                                    Resp.bene_name = values.beneName;
                                    Resp.totalAmount = TotalAmount.ToString();
                                    Resp.amount = values.txnAmount;

                                    objresponse.response_code = "200";
                                    objresponse.response_message = "Pending";
                                    objresponse.data = Resp;
                                }
                            }
                            else
                            {
                                objRequestStatus.TransactionID = transactionID;
                                objRequestStatus.BankReferrenceNumber = "";
                                objRequestStatus.Request = statusResponse.ApiRequest;
                                objRequestStatus.Response = statusResponse.ApiResponse;
                                objRequestStatus.ResponseCode = axisCheckResponse.data.errorMessage;
                                objRequestStatus.ResponseDescription = "";
                                objRequestStatus.TranStatus = "3"; // Failed
                                objRequestStatus.Flag = "2";
                                objRequestStatus.BankTypeID = dst.Tables[0].Rows[0][5].ToString();

                                DataSet dataset = _payoutRepo.PayoutUpdateResponse(objRequestStatus);

                                objresponse.response_code = "202";
                                objresponse.response_message = "Transaction Failed";
                                objresponse.data = "Your payment request is FAILED";
                            }

                        }
                        else if (axisCheckResponse.status.ToUpper() == "F")
                        {
                            objRequestStatus.TransactionID = transactionID;
                            objRequestStatus.BankReferrenceNumber = axisCheckResponse.data.CUR_TXN_ENQ[0].utrNo;
                            objRequestStatus.Request = statusResponse.ApiRequest;
                            objRequestStatus.Response = statusResponse.ApiResponse;
                            objRequestStatus.ResponseCode = axisCheckResponse.data.CUR_TXN_ENQ[0].transactionStatus;
                            objRequestStatus.ResponseDescription = axisCheckResponse.data.CUR_TXN_ENQ[0].statusDescription;
                            objRequestStatus.TranStatus = "3"; // Pending
                            objRequestStatus.Flag = "2";
                            objRequestStatus.BankTypeID = dst.Tables[0].Rows[0][5].ToString();

                            DataSet dataset = _payoutRepo.PayoutUpdateResponse(objRequestStatus);

                            objresponse.response_code = "105";
                            objresponse.response_message = "Failed";
                            objresponse.data = "Your payment request is Failed";
                        }

                        return objresponse;

                    }
                    else
                    {
                        objresponse.response_code = "201";
                        objresponse.response_message = "Failed";
                        objresponse.data = paymentRespnse;
                        return objresponse;
                    }
                }
                // Yes bank Start
                else if (dst.Tables[0].Rows[0][0].ToString() == "100" && dst.Tables[0].Rows[0][5].ToString() == "4")
                {
                    yesValues.TxnPaymode = dst.Tables[0].Rows[0]["PayModeRefID"].ToString();
                    yesValues.TxnAmount = dst.Tables[0].Rows[0]["Amount"].ToString();
                    yesValues.BeneName = dst.Tables[0].Rows[0]["BeneName"].ToString();
                    yesValues.BeneAccNum = dst.Tables[0].Rows[0]["BeneAccountNumber"].ToString();
                    yesValues.BeneIfscCode = dst.Tables[0].Rows[0]["BeneIFSCCode"].ToString();
                    yesValues.BeneBankName = dst.Tables[0].Rows[0]["BeneBankName"].ToString();
                    yesValues.TransactionID = dst.Tables[0].Rows[0]["TransactionID"].ToString();
                    yesValues.EmailAddress = dst.Tables[0].Rows[0]["Email"].ToString();
                    yesValues.Address1 = dst.Tables[0].Rows[0]["Address1"].ToString();
                    yesValues.Address2 = dst.Tables[0].Rows[0]["Address2"].ToString();
                    yesValues.Pincode = dst.Tables[0].Rows[0]["Pincode"].ToString();
                    yesValues.State = dst.Tables[0].Rows[0]["State"].ToString();
                    yesValues.PhoneNumber = dst.Tables[0].Rows[0]["Agentmobile"].ToString(); ////Change value
                    yesValues.customername = dst.Tables[0].Rows[0]["customername"].ToString();
                    commission = dst.Tables[0].Rows[0]["CustomerFee"].ToString();
                    decimal TotalAmount = Convert.ToDecimal(commission) + Convert.ToDecimal(yesValues.TxnAmount);
                   // APTSendOTP(yesValues.TxnAmount, Request.sendermobilenumber, yesValues.BeneName, yesValues.TransactionID);
                    if (string.IsNullOrEmpty(yesValues.EmailAddress))
                    {
                        yesValues.EmailAddress = "customercare@accupaydtech.com";
                    }
                    CombinedResponse paymentRespnse = _yesbanksevice.TransferPayment(yesValues);
                    var transactionID = dst.Tables[0].Rows[0][2].ToString();
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "YPayout Response22", JsonConvert.SerializeObject(paymentRespnse), Request.sendermobilenumber + "y");
                    if (paymentRespnse.ApiResponse == "Failed")
                    {
                        objresponse.response_code = "201";
                        objresponse.response_message = "Failed";
                        objresponse.data = "Failed!... Something Wrong...Please Contact Admin";
                        return objresponse;
                    }
                    else if(paymentRespnse.ApiResponse.Contains("Bad Request"))
                    {
                        YesBankPayoutErrorResponse Err = JsonConvert.DeserializeObject<YesBankPayoutErrorResponse>(paymentRespnse.ApiResponse);
                        objresponse.response_code = "206";
                        objresponse.response_message = Err.Message;
                        objresponse.data = Err;
                        UpdateTxnStatusPayout ObjResp1 = new UpdateTxnStatusPayout();
                        ObjResp1.TransactionID = transactionID;
                        ObjResp1.BankReferrenceNumber = "NA";
                        ObjResp1.Request = paymentRespnse.ApiRequest;
                        ObjResp1.Response = paymentRespnse.ApiResponse;
                        ObjResp1.TranStatus = "3";
                        ObjResp1.ResponseCode = "F";
                        ObjResp1.Flag = "1";
                        ObjResp1.BankTypeID = dst.Tables[0].Rows[0][5].ToString();
                        ObjResp1.ResponseDescription = "Failed";
                        DataSet dsts = _payoutRepo.PayoutUpdateResponse(ObjResp1);
                        return objresponse;

                    }
                    
                    YesBankResponse responseStatus = JsonConvert.DeserializeObject<YesBankResponse>(paymentRespnse.ApiResponse);

                    UpdateTxnStatusPayout objResponse = new UpdateTxnStatusPayout();
                    objResponse.TransactionID = transactionID;
                    objResponse.BankReferrenceNumber = responseStatus.Data.TransactionIdentification;
                    objResponse.Request = paymentRespnse.ApiRequest;
                    objResponse.Response = paymentRespnse.ApiResponse;
                    if (responseStatus.Data.Status == "Received") { objResponse.ResponseCode = "S"; } else { objResponse.ResponseCode = "F"; }

                    objResponse.ResponseDescription = "";
                    if (responseStatus.Data.Status != "Received") { objResponse.TranStatus = "3"; } else if (responseStatus.Data.Status == "Received") { objResponse.TranStatus = "2"; }
                    objResponse.Flag = "1";
                    objResponse.BankTypeID = dst.Tables[0].Rows[0][5].ToString();
                    //DataSet dsts = _payoutRepo.PayoutUpdateResponse(objResponse);

                    if (responseStatus.Data.Status != "Received")
                    {
                        Dictionary<string, string> response = new Dictionary<string, string>();
                        response.Add("result", paymentRespnse.ApiResponse);
                        response.Add("transactionID", transactionID);
                        objresponse.response_code = "201";
                        objresponse.response_message = "Failed";
                        objresponse.data = paymentRespnse.ApiResponse;
                        DataSet dsts = _payoutRepo.PayoutUpdateResponse(objResponse);
                    }
                    else if (responseStatus.Data.Status == "Received")
                    {
                        Thread.Sleep(4000);
                        UpdateTxnStatusPayout objRequestStatus = new UpdateTxnStatusPayout();
                        
                        CombinedResponse statusResponse = _yesbanksevice.CheckStatus(transactionID);
                        if (statusResponse.ApiResponse == "Failed")
                        {
                            objresponse.response_code = "201";
                            objresponse.response_message = "Error";
                            objresponse.data = "Failed!... Something Wrong...Please Contact Admin";
                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "YPayout Success!2", JsonConvert.SerializeObject(objresponse), Request.sendermobilenumber);
                            return objresponse;
                        }


                        YesBankStatusResponse YesCheckResponse = JsonConvert.DeserializeObject<YesBankStatusResponse>(statusResponse.ApiResponse);


                        if (YesCheckResponse.Data.Status == "SettlementCompleted")
                        {
                            objRequestStatus.TransactionID = transactionID;
                            objRequestStatus.BankReferrenceNumber = YesCheckResponse.Data.Initiation.EndToEndIdentification;
                            objRequestStatus.Request = statusResponse.ApiRequest;
                            objRequestStatus.Response = statusResponse.ApiResponse;
                            objRequestStatus.ResponseCode = YesCheckResponse.Data.Status;
                            objRequestStatus.ResponseDescription = "";
                            objRequestStatus.TranStatus = "2"; // Success
                            objRequestStatus.Flag = "2";
                            objRequestStatus.BankTypeID = dst.Tables[0].Rows[0][5].ToString();


                            DataSet dataset = _payoutRepo.PayoutUpdateResponse(objRequestStatus);
                            try
                            {
                                if(dataset.Tables[0].Rows[0]["smsflag"].ToString() == "2")
                                {
                                    if (dataset.Tables[0].Rows.Count > 0)
                                    {
                                        string Amount = dataset.Tables[0].Rows[0]["amount"].ToString();
                                        string PayeeName = dataset.Tables[0].Rows[0]["payeename"].ToString();
                                        string MobileNo = dataset.Tables[0].Rows[0]["SenderMobileNumber"].ToString();
                                        string RefId = dataset.Tables[0].Rows[0]["Refid"].ToString();

                                        APTSendOTP(Amount, MobileNo, PayeeName, RefId);
                                    }
                                    else
                                    {

                                    }
                                }
                                
                            }
                            catch { }
                            APTUpdateBeneNameYesbank_payout UpdateBeneNameReq = new APTUpdateBeneNameYesbank_payout();
                            UpdateBeneNameReq.transaction_id = transactionID;
                            UpdateBeneNameReq.rrn = YesCheckResponse.Data.Initiation.EndToEndIdentification;
                            UpdateBeneNameReq.bene_name = YesCheckResponse.Data.Initiation.CreditorAccount.Name;
                            UpdateBeneNameReq.status_update_datetime = DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt");
                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "YPayout Success!BeneNameUpdateReq", JsonConvert.SerializeObject(UpdateBeneNameReq), Request.sendermobilenumber);
                            DataSet dtst = _payoutRepo.PayoutYesBankUpdateBeneName(UpdateBeneNameReq);
                            //_logService.WriteLog(MethodBase.GetCurrentMethod().Name, "YPayout Success!BeneNameUpdateDBResponse", JsonConvert.SerializeObject(dtst), Request.sendermobilenumber);

                            Resp.transaction_date = DateTime.Now.ToString("dd/MM/yyyy");
                            Resp.transaction_id = transactionID;
                            Resp.rrn = YesCheckResponse.Data.Initiation.EndToEndIdentification;
                            Resp.customer_name = yesValues.BeneName;
                            Resp.commision = commission;
                            Resp.bene_acnt_no = yesValues.BeneAccNum;
                            Resp.bene_name = yesValues.BeneName;
                            Resp.totalAmount = TotalAmount.ToString();
                            Resp.amount = yesValues.TxnAmount;
                            objresponse.response_code = "200";
                            objresponse.response_message = "Success";
                            objresponse.data = Resp;//"Your payment request is successfully credited to " + YesCheckResponse.Data.Initiation.CreditorAccount.Name;
                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "YPayout Success!2", JsonConvert.SerializeObject(objresponse), Request.sendermobilenumber);
                        }
                        else if ((YesCheckResponse.Data.Status == "SettlementInProcess") || (YesCheckResponse.Data.Status == "Pending") || (YesCheckResponse.Data.Status == "Accepted"))
                        {
                            objRequestStatus.TransactionID = transactionID;
                            objRequestStatus.BankReferrenceNumber = (YesCheckResponse.Data.Status == "SettlementInProcess") ? YesCheckResponse.Data.Initiation.EndToEndIdentification : "";
                            objRequestStatus.Request = statusResponse.ApiRequest;
                            objRequestStatus.Response = statusResponse.ApiResponse;
                            objRequestStatus.ResponseCode = YesCheckResponse.Data.Status;
                            objRequestStatus.ResponseDescription = "";
                            objRequestStatus.TranStatus = "1"; // Failed
                            objRequestStatus.Flag = "2";
                            objRequestStatus.BankTypeID = dst.Tables[0].Rows[0][5].ToString();

                            DataSet dataset = _payoutRepo.PayoutUpdateResponse(objRequestStatus);
                            try
                            {
                                if (dataset.Tables[0].Rows[0]["smsflag"].ToString() == "2")
                                {
                                    if (dataset.Tables[0].Rows.Count > 0)
                                    {
                                        string Amount = dataset.Tables[0].Rows[0]["amount"].ToString();
                                        string PayeeName = dataset.Tables[0].Rows[0]["payeename"].ToString();
                                        string MobileNo = dataset.Tables[0].Rows[0]["SenderMobileNumber"].ToString();
                                        string RefId = dataset.Tables[0].Rows[0]["Refid"].ToString();

                                        APTSendOTP(Amount, MobileNo, PayeeName, RefId);
                                    }
                                    else
                                    {

                                    }
                                }

                            }
                            catch { }
                            Resp.transaction_date = DateTime.Now.ToString("dd/MM/yyyy");
                            Resp.transaction_id = transactionID;
                            Resp.rrn = YesCheckResponse.Data.Initiation.EndToEndIdentification;
                            Resp.customer_name = yesValues.BeneName;
                            Resp.commision = commission;
                            Resp.bene_acnt_no = yesValues.BeneAccNum;
                            Resp.bene_name = yesValues.BeneName;
                            Resp.totalAmount = TotalAmount.ToString();
                            Resp.amount = yesValues.TxnAmount;

                            objresponse.response_code = "200";
                            objresponse.response_message = "Pending";
                            objresponse.data = Resp;
                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "YPayout Pending!2", JsonConvert.SerializeObject(objresponse), Request.sendermobilenumber);
                        }
                        else if (YesCheckResponse.Data.Status == "FAILED")
                        {
                            objRequestStatus.TransactionID = transactionID;
                            objRequestStatus.BankReferrenceNumber = YesCheckResponse.Data.Initiation.EndToEndIdentification;
                            objRequestStatus.Request = statusResponse.ApiRequest;
                            objRequestStatus.Response = statusResponse.ApiResponse;
                            objRequestStatus.ResponseCode = YesCheckResponse.Data.Status;
                            objRequestStatus.ResponseDescription = "";
                            objRequestStatus.TranStatus = "3"; // Pending
                            objRequestStatus.Flag = "2";
                            objRequestStatus.BankTypeID = dst.Tables[0].Rows[0][5].ToString();

                            DataSet dataset = _payoutRepo.PayoutUpdateResponse(objRequestStatus);

                            objresponse.response_code = "202";
                            objresponse.response_message = "Failed";
                            objresponse.data = objRequestStatus;
                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "YPayout Failed!2", JsonConvert.SerializeObject(objresponse), Request.sendermobilenumber);
                        }

                        return objresponse;

                    }
                    else
                    {
                        objresponse.response_code = "201";
                        objresponse.response_message = "Failed";
                        objresponse.data = paymentRespnse;
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "YPayout Failed!!!", JsonConvert.SerializeObject(objresponse), Request.sendermobilenumber);
                        return objresponse;
                    }
                }


            }
            catch (Exception ex)
            {
                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed!!! Ex", JsonConvert.SerializeObject(ex), Request.sendermobilenumber);
            }
            return objresponse;
        }

        public APTGenericResponse? PayoutCheckStatus(PayoutStatusRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            UpdateTxnStatusPayout objResponse = new UpdateTxnStatusPayout();
            UpdateTxnStatusPayout objRequestStatus = new UpdateTxnStatusPayout();
            try
            {
                DataSet dst = _payoutRepo.PayoutCheckStatus(Request);

                if (dst.Tables[0].Rows[0][0].ToString() == "101")
                {
                    objresponse.response_code = dst.Tables[0].Rows[0][0].ToString();
                    objresponse.response_message = dst.Tables[0].Rows[0][1].ToString();
                    objresponse.data = null;
                }
                else if (dst.Tables[0].Rows[0][1].ToString() == "3") // Axis bank
                {

                    CombinedResponse statusResponse = _axisservice.CheckStatus(Request.transactionID);
                    if (statusResponse.ApiResponse == "Failed")
                    {
                        objresponse.response_code = "201";
                        objresponse.response_message = "Error";
                        objresponse.data = "Failed!... Something Wrong...Please Contact Admin";
                        return objresponse;
                    }
                    else if (statusResponse.ApiResponse.Contains("not found for Corporate Code ACCUPAYD"))
                    {
                        objRequestStatus.TransactionID = Request.transactionID;
                        objRequestStatus.BankReferrenceNumber = "";
                        objRequestStatus.Request = statusResponse.ApiRequest;
                        objRequestStatus.Response = statusResponse.ApiResponse;
                        objRequestStatus.ResponseCode = "Failed";
                        objRequestStatus.ResponseDescription = "404 Not found";
                        objRequestStatus.TranStatus = "3"; // Failed
                        objRequestStatus.Flag = "2";
                        objRequestStatus.BankTypeID = dst.Tables[0].Rows[0]["BankRefID"].ToString();

                        DataSet dataset = _payoutRepo.PayoutUpdateResponse(objRequestStatus);

                        objresponse.response_code = "205";
                        objresponse.response_message = "Failed Not found";
                        objresponse.data = objRequestStatus;
                        return objresponse;
                    }
                    GetStatusResponseBody axisCheckResponse = JsonConvert.DeserializeObject<GetStatusResponseBody>(statusResponse.ApiResponse);

                    if (axisCheckResponse.status.ToUpper() == "S")
                    {
                        if (axisCheckResponse.data.CUR_TXN_ENQ[0].transactionStatus == "PROCESSED")
                        {
                            objRequestStatus.TransactionID = Request.transactionID;
                            objRequestStatus.BankReferrenceNumber = axisCheckResponse.data.CUR_TXN_ENQ[0].utrNo;
                            objRequestStatus.Request = statusResponse.ApiRequest;
                            objRequestStatus.Response = statusResponse.ApiResponse;
                            objRequestStatus.ResponseCode = axisCheckResponse.data.CUR_TXN_ENQ[0].transactionStatus;
                            objRequestStatus.ResponseDescription = axisCheckResponse.data.CUR_TXN_ENQ[0].statusDescription;
                            objRequestStatus.TranStatus = "2"; // Success
                            objRequestStatus.Flag = "2";
                            objRequestStatus.BankTypeID = dst.Tables[0].Rows[0]["BankRefID"].ToString();

                            DataSet dataset = _payoutRepo.PayoutUpdateResponse(objRequestStatus);

                            objresponse.response_code = "200";
                            objresponse.response_message = "Success";
                            objresponse.data = "Your payment request is " + objRequestStatus.ResponseDescription;
                        }
                        else if (axisCheckResponse.data.CUR_TXN_ENQ[0].transactionStatus == "REJECTED" || axisCheckResponse.data.CUR_TXN_ENQ[0].transactionStatus == "RETURN" || axisCheckResponse.data.CUR_TXN_ENQ[0].transactionStatus == "FAILED")
                        {
                            objRequestStatus.TransactionID = Request.transactionID;
                            objRequestStatus.BankReferrenceNumber = axisCheckResponse.data.CUR_TXN_ENQ[0].utrNo;
                            objRequestStatus.Request = statusResponse.ApiRequest;
                            objRequestStatus.Response = statusResponse.ApiResponse;
                            objRequestStatus.ResponseCode = axisCheckResponse.data.CUR_TXN_ENQ[0].transactionStatus;
                            objRequestStatus.ResponseDescription = axisCheckResponse.data.CUR_TXN_ENQ[0].statusDescription;
                            objRequestStatus.TranStatus = "3"; // Failed
                            objRequestStatus.Flag = "2";
                            objRequestStatus.BankTypeID = dst.Tables[0].Rows[0]["BankRefID"].ToString();

                            DataSet dataset = _payoutRepo.PayoutUpdateResponse(objRequestStatus);

                            objresponse.response_code = "200";
                            objresponse.response_message = "Success";
                            objresponse.data = "Your payment request is  FAILED";
                        }
                        else if (axisCheckResponse.data.CUR_TXN_ENQ[0].transactionStatus == "PENDING")
                        {
                            objRequestStatus.TransactionID = Request.transactionID;
                            objRequestStatus.BankReferrenceNumber = axisCheckResponse.data.CUR_TXN_ENQ[0].utrNo;
                            objRequestStatus.Request = statusResponse.ApiRequest;
                            objRequestStatus.Response = statusResponse.ApiResponse;
                            objRequestStatus.ResponseCode = axisCheckResponse.data.CUR_TXN_ENQ[0].transactionStatus;
                            objRequestStatus.ResponseDescription = axisCheckResponse.data.CUR_TXN_ENQ[0].statusDescription;
                            objRequestStatus.TranStatus = "1"; // Pending
                            objRequestStatus.Flag = "2";
                            objRequestStatus.BankTypeID = dst.Tables[0].Rows[0]["BankRefID"].ToString();

                            DataSet dataset = _payoutRepo.PayoutUpdateResponse(objRequestStatus);

                            objresponse.response_code = "200";
                            objresponse.response_message = "Success";
                            objresponse.data = "Your payment request is: " + objRequestStatus.ResponseDescription;
                        }
                        else
                        {
                            objRequestStatus.TransactionID = Request.transactionID;
                            objRequestStatus.BankReferrenceNumber = axisCheckResponse.data.CUR_TXN_ENQ[0].utrNo;
                            objRequestStatus.Request = statusResponse.ApiRequest;
                            objRequestStatus.Response = statusResponse.ApiResponse;
                            objRequestStatus.ResponseCode = axisCheckResponse.data.CUR_TXN_ENQ[0].transactionStatus;
                            objRequestStatus.ResponseDescription = axisCheckResponse.data.CUR_TXN_ENQ[0].statusDescription;
                            objRequestStatus.TranStatus = "1"; // Pending
                            objRequestStatus.Flag = "2";
                            objRequestStatus.BankTypeID = dst.Tables[0].Rows[0]["BankRefID"].ToString();

                            DataSet dataset = _payoutRepo.PayoutUpdateResponse(objRequestStatus);

                            objresponse.response_code = "200";
                            objresponse.response_message = "Success";
                            objresponse.data = "Something Wrong Please Contact Admin";

                        }
                    }
                    else
                    {
                        objRequestStatus.TransactionID = Request.transactionID;
                        objRequestStatus.BankReferrenceNumber = axisCheckResponse.data.CUR_TXN_ENQ[0].utrNo;
                        objRequestStatus.Request = statusResponse.ApiRequest;
                        objRequestStatus.Response = statusResponse.ApiResponse;
                        objRequestStatus.ResponseCode = axisCheckResponse.data.CUR_TXN_ENQ[0].transactionStatus;
                        objRequestStatus.ResponseDescription = axisCheckResponse.data.CUR_TXN_ENQ[0].statusDescription;
                        objRequestStatus.TranStatus = "1"; // Pending
                        objRequestStatus.Flag = "2";
                        objRequestStatus.BankTypeID = dst.Tables[0].Rows[0]["BankRefID"].ToString();

                        DataSet dataset = _payoutRepo.PayoutUpdateResponse(objRequestStatus);

                        objresponse.response_code = "200";
                        objresponse.response_message = "Success";
                        objresponse.data = "Your payment request is FAILED";
                    }



                }
                else if (dst.Tables[0].Rows[0][1].ToString() == "4")
                {

                    CombinedResponse statusResponse = _yesbanksevice.CheckStatus(Request.transactionID);
                    if (statusResponse.ApiResponse == "Failed")
                    {
                        objresponse.response_code = "201";
                        objresponse.response_message = "Error";
                        objresponse.data = "Failed!... Something Wrong...Please Contact Admin";
                        return objresponse;
                    }
                    else if (statusResponse.ApiResponse.Contains("ns:E404"))
                    {
                        objRequestStatus.TransactionID = Request.transactionID;
                        objRequestStatus.BankReferrenceNumber = "";
                        objRequestStatus.Request = statusResponse.ApiRequest;
                        objRequestStatus.Response = statusResponse.ApiResponse;
                        objRequestStatus.ResponseCode = "Failed";
                        objRequestStatus.ResponseDescription = "404 Not found";
                        objRequestStatus.TranStatus = "3"; // Failed
                        objRequestStatus.Flag = "2";
                        objRequestStatus.BankTypeID = dst.Tables[0].Rows[0]["BankRefID"].ToString();

                        DataSet dataset = _payoutRepo.PayoutUpdateResponse(objRequestStatus);

                        objresponse.response_code = "205";
                        objresponse.response_message = "Not found";
                        objresponse.data = objRequestStatus;
                        return objresponse;
                    }


                    YesBankStatusResponse YesCheckResponse = JsonConvert.DeserializeObject<YesBankStatusResponse>(statusResponse.ApiResponse);


                    if (YesCheckResponse.Data.Status == "SettlementCompleted")
                    {
                        objRequestStatus.TransactionID = Request.transactionID;
                        objRequestStatus.BankReferrenceNumber = YesCheckResponse.Data.TransactionIdentification;
                        objRequestStatus.Request = statusResponse.ApiRequest;
                        objRequestStatus.Response = statusResponse.ApiResponse;
                        objRequestStatus.ResponseCode = YesCheckResponse.Data.Status;
                        objRequestStatus.ResponseDescription = "";
                        objRequestStatus.TranStatus = "2"; // Success
                        objRequestStatus.Flag = "2";
                        objRequestStatus.BankTypeID = dst.Tables[0].Rows[0]["BankRefID"].ToString();

                        DataSet dataset = _payoutRepo.PayoutUpdateResponse(objRequestStatus);
                        APTUpdateBeneNameYesbank_payout UpdateBeneNameReq = new APTUpdateBeneNameYesbank_payout();
                        UpdateBeneNameReq.transaction_id = objRequestStatus.TransactionID;
                        UpdateBeneNameReq.rrn = YesCheckResponse.Data.Initiation.EndToEndIdentification;
                        UpdateBeneNameReq.bene_name = YesCheckResponse.Data.Initiation.CreditorAccount.Name;
                        UpdateBeneNameReq.status_update_datetime = DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt");
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "YPayout Success!BeneNameUpdateReq", JsonConvert.SerializeObject(UpdateBeneNameReq), "");
                        DataSet dtst = _payoutRepo.PayoutYesBankUpdateBeneName(UpdateBeneNameReq);

                        objresponse.response_code = "200";
                        objresponse.response_message = "Success";
                        objresponse.data = "Your payment request is successfully credited to " + YesCheckResponse.Data.Initiation.CreditorAccount.Name;
                    }
                    else if ((YesCheckResponse.Data.Status == "SettlementInProcess") || (YesCheckResponse.Data.Status == "Pending"))
                    {
                        objRequestStatus.TransactionID = Request.transactionID;
                        objRequestStatus.BankReferrenceNumber = (YesCheckResponse.Data.Status == "SettlementInProcess") ? YesCheckResponse.Data.TransactionIdentification : "";
                        objRequestStatus.Request = statusResponse.ApiRequest;
                        objRequestStatus.Response = statusResponse.ApiResponse;
                        objRequestStatus.ResponseCode = YesCheckResponse.Data.Status;
                        objRequestStatus.ResponseDescription = "";
                        objRequestStatus.TranStatus = "1"; // Pending
                        objRequestStatus.Flag = "2";
                        objRequestStatus.BankTypeID = dst.Tables[0].Rows[0]["BankRefID"].ToString();

                        DataSet dataset = _payoutRepo.PayoutUpdateResponse(objRequestStatus);

                        objresponse.response_code = "200";
                        objresponse.response_message = "Success";
                        objresponse.data = "Your payment request is Pending";
                    }
                    else if (YesCheckResponse.Data.Status == "FAILED")
                    {
                        objRequestStatus.TransactionID = Request.transactionID;
                        objRequestStatus.BankReferrenceNumber = "";
                        objRequestStatus.Request = statusResponse.ApiRequest;
                        objRequestStatus.Response = statusResponse.ApiResponse;
                        objRequestStatus.ResponseCode = YesCheckResponse.Data.Status;
                        objRequestStatus.ResponseDescription = "";
                        objRequestStatus.TranStatus = "3"; // Failed
                        objRequestStatus.Flag = "2";
                        objRequestStatus.BankTypeID = dst.Tables[0].Rows[0]["BankRefID"].ToString();

                        DataSet dataset = _payoutRepo.PayoutUpdateResponse(objRequestStatus);

                        objresponse.response_code = "200";
                        objresponse.response_message = "Success";
                        objresponse.data = "Your payment request is Failed ";
                    }
                    else
                    {
                        objRequestStatus.TransactionID = Request.transactionID;
                        objRequestStatus.BankReferrenceNumber = "";
                        objRequestStatus.Request = statusResponse.ApiRequest;
                        objRequestStatus.Response = statusResponse.ApiResponse;
                        objRequestStatus.ResponseCode = YesCheckResponse.Data.Status;
                        objRequestStatus.ResponseDescription = "";
                        objRequestStatus.TranStatus = "1"; // Pending
                        objRequestStatus.Flag = "2";
                        objRequestStatus.BankTypeID = dst.Tables[0].Rows[0]["BankRefID"].ToString();

                        DataSet dataset = _payoutRepo.PayoutUpdateResponse(objRequestStatus);

                        objresponse.response_code = "200";
                        objresponse.response_message = "Success";
                        objresponse.data = "Some thing Wrong please Contact Admin";

                    }
                }

            }
            catch (Exception ex)
            {
                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
            }

            return objresponse;
        }

        public APTGenericResponse? APTRegisterCustomer(APTRegisterCustomer Request)
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.mobile_no);

            APTGenericResponse objresponse = new APTGenericResponse();
            PaytmPrevalidateCustomerResponse objAPIResponse = new PaytmPrevalidateCustomerResponse();
            APTRegisterCustomerResponse objregResponse = new APTRegisterCustomerResponse();
            try
            {

                Request.bank_id = "0";
                Request.bank_ref_id = "";
                Request.otp_validation_flag = "0";

                DataSet register_dst = _payoutRepo.APTRegisterCustomer(Request);
                if (_commonService.IsValidDataSet(register_dst))
                {
                    if (register_dst.Tables[0].Rows[0][0].ToString() == "100")
                    {
                        //Random generator = new Random();
                        //string OTP = generator.Next(0, 1000000).ToString("D6");
                        //string RefId = Request.agent_ref_id;

                        //APTInsertOTPRequest insertOTPRequest = new APTInsertOTPRequest();
                        //insertOTPRequest.mobile_no = Request.agent_mobile_no;
                        //insertOTPRequest.account_ref_id = RefId;
                        //insertOTPRequest.account_type = Request.account_type;
                        //insertOTPRequest.otp = OTP;
                        //bool sendotp = _commonService.SendOTPToAgent(insertOTPRequest);
                        //if (sendotp)
                        //{
                            objresponse.response_code = "200"; //
                            objresponse.response_message = "Customer Added";
                            objresponse.data = null;
                        //}
                        //else
                        //{
                        //    objresponse.response_code = "205"; // 
                        //    objresponse.response_message = "Sending OTP Failed From Backend";
                        //    objresponse.data = null;
                        //}
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Register Cust PayOut resp", JsonConvert.SerializeObject(objresponse), Request.mobile_no);
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

            catch (Exception ex)
            {
                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.mobile_no);

            }

            return objresponse;
        }

        public APTGenericResponse? APTAddPayee(APTAddPayee Request)
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.mobile_no);

            APTGenericResponse objresponse = new APTGenericResponse();
            try
            {
                Request.active_status_ref_id = "2";
                Request.bank_type_id = "0";
                DataSet dst = _payoutRepo.APTAddPayee(Request);
                if (_commonService.IsValidDataSet(dst))
                {
                    if (dst.Tables[0].Rows[0][0].ToString() == "100")
                    {
                        objresponse.response_code = APIResponseCode.Success;
                        objresponse.response_message = "Bene Added Successfully";
                        objresponse.data = null;
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

        public List<APTGetAllPayeeResponse>? GetPayeeFromDB(APTGetAllPayee Request)
        {
            List<APTGetAllPayeeResponse> lst_objPayeeResponse = new List<APTGetAllPayeeResponse>();

            APTGenericResponse objresponse = new APTGenericResponse();
            DataSet dst = _payoutRepo.APTGetAllPayee(Request);
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
        public APTGenericResponse? ValidateRegisterCustomerOTP(ValidateOTPRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.mobile_no);

            try
            {
                

                    DataSet dst = _payoutRepo.APTValidateRegisterCustomerOTP(Request);
                    if (_commonService.IsValidDataSet(dst) && dst.Tables[0].Rows[0][0].ToString() == APIResponseCode.DBSuccess)
                    {
                        if (Request.customer_mobile == "")
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
                            updateCustReq.mobile_no = Request.customer_mobile;
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
                DataSet dst = _payoutRepo.APTUpdateCustomer(Request);
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

        public bool SendOTPFromDBforCustomerRegistration(APTInsertOTPRequest Request)
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.mobile_no);

            APTGenericResponse objResponse = new APTGenericResponse();
            try
            {
                Random generator = new Random();
                string OTP = generator.Next(0, 1000000).ToString("D6");
                Request.otp = OTP;

                DataSet dstinsotp = _payoutRepo.APTInsertOTP(Request);
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

        public APTGenericResponse? APTGetCustomerInfo(APTGetCustomerInfo Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.mobile_no);

            try
            {
                DataSet dst = _payoutRepo.APTGetCustomerInfo(Request);
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
                   // PaytmPrevalidateCustomerResponse objAPIResponse = new PaytmPrevalidateCustomerResponse();
                    //objAPIResponse = _paytmAPI.PrevalidateCustomer(Request.mobile_no);
                    //if (objAPIResponse != null && objAPIResponse.response_code == "0")
                    //{
                    //    CustInfo.bank_1_monthly_balance = objAPIResponse.limitLeft;
                    //}
                    //else
                    //{
                        CustInfo.bank_1_monthly_balance = dst.Tables[0].Rows[0]["Bank1MonthlyBalance"].ToString();

                    //}
                    if (CustInfo.active_status_ref_id == "1")
                    {

                        //if (objAPIResponse != null && objAPIResponse.response_code == "0")
                        //{
                            CustInfo.is_internal = true;
                        //}
                        //else if (objAPIResponse != null && objAPIResponse.response_code == "1032")
                        //{
                        //    CustInfo.is_internal = false;
                        //}
                        //else
                        //{
                        //    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Register Customer Prevalidate Failed", JsonConvert.SerializeObject(objAPIResponse), Request.mobile_no);

                        //    objresponse.response_code = "110";
                        //    objresponse.response_message = "Fetching data from Bank Failed, please try again.";
                        //    objresponse.data = null;
                        //}
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

        public APTGenericResponse? APTPayoutGetPin(Getpin_PayoutRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            Getpin_PayoutResponse Response = new Getpin_PayoutResponse();
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.retailer_number);
            try
            {
                DataSet Dst = _payoutRepo.APTGetPin_Payout(Request.retailer_number);
                if (_commonService.IsValidDataSet(Dst))
                {
                    if (Dst.Tables[0].Rows[0]["Responsecode"].ToString() == "100")
                    {
                        Response.pin_Required = Dst.Tables[0].Rows[0]["Pinupdaterequired"].ToString();
                        Response.retailer_number = Request.retailer_number;
                        objresponse.response_code = "200";
                        objresponse.response_message = "Success";
                        objresponse.data = Response;
                    }
                    else
                    {
                        objresponse.response_code = "102";
                        objresponse.response_message = "Failed from backend";
                        objresponse.data = null;
                    }
                }
                else
                {
                    objresponse.response_code = "101";
                    objresponse.response_message = "Connectivity Issue";
                    objresponse.data = null;
                }

            }
            catch(Exception ex)
            {
                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.retailer_number);
            }
            return objresponse;
        }

        public APTGenericResponse? VerifyPin(Verifypin_PayoutRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            Verifypin_PayoutResponse Response = new Verifypin_PayoutResponse();
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.retailer_number);
            try
            {
                DataSet Dst = _payoutRepo.APTGetPin_Payout(Request.retailer_number);
                if (_commonService.IsValidDataSet(Dst))
                {
                    if (Dst.Tables[0].Rows[0]["Responsecode"].ToString() == "100")
                    {
                        if(Dst.Tables[0].Rows[0]["pin"].ToString() == Request.pin)
                        {
                            Response.retailer_number = Request.retailer_number;
                            Response.description = "Pin verification success";
                            objresponse.response_code = "200";
                            objresponse.response_message = "Success";
                            objresponse.data = Response;
                        }
                        else
                        {
                            Response.retailer_number = Request.retailer_number;
                            Response.description = "Pin verification Failed";
                            objresponse.response_code = "200";
                            objresponse.response_message = "Success";
                            objresponse.data = Response;
                        }
                        
                    }
                    else
                    {
                        objresponse.response_code = "102";
                        objresponse.response_message = "Failed from backend";
                        objresponse.data = null;
                    }
                }
                else
                {
                    objresponse.response_code = "101";
                    objresponse.response_message = "Connectivity Issue";
                    objresponse.data = null;
                }

            }
            catch (Exception ex)
            {
                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.retailer_number);
            }
            return objresponse;
        }

        public APTGenericResponse? InsertUpdatePin(Insertpin_PayoutRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            Verifypin_PayoutResponse Response = new Verifypin_PayoutResponse();
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.retailer_number);
            try
            {
                ValidateOTPRequest OTPrequest = new ValidateOTPRequest();
                OTPrequest.otp = Request.otp;
                DataSet dst = _payoutRepo.APTValidateRegisterCustomerOTP(OTPrequest);
                if(_commonService.IsValidDataSet(dst) && dst.Tables[0].Rows[0][0].ToString() == APIResponseCode.DBSuccess)
                {
                    DataSet Dst = _payoutRepo.APTInsertPin_Payout(Request);
                    if (_commonService.IsValidDataSet(Dst))
                    {
                        if (Dst.Tables[0].Rows[0]["Responsecode"].ToString() == "100")
                        {
                            
                           Response.retailer_number = Request.retailer_number;
                           Response.description = "Pin Updated successfully";
                           objresponse.response_code = "200";
                           objresponse.response_message = "Success";
                           objresponse.data = Response;

                        }
                        else
                        {
                            objresponse.response_code = "102";
                            objresponse.response_message = "Failed from backend";
                            objresponse.data = null;
                        }
                    }
                    else
                    {
                        objresponse.response_code = "101";
                        objresponse.response_message = "Connectivity Issue";
                        objresponse.data = null;
                    }
                }
                else
                {
                    objresponse.response_code = "151";
                    objresponse.response_message = "OTP Verification Failed";
                    objresponse.data = null;
                }
                

            }
            catch (Exception ex)
            {
                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.retailer_number);
            }
            return objresponse;
        }

        public APTGenericResponse? SendOTPForPayeeDeletion(ResendOTPRequest Request)
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.mobile_no);

            APTGenericResponse objresponse = new APTGenericResponse();
            try
            {
                
                objresponse = ResendOTP(Request);
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
                ValidateOTPRequest request = new ValidateOTPRequest();
                request.otp = Request.otp;
                DataSet DST = _payoutRepo.APTValidateRegisterCustomerOTP(request);
                if (_commonService.IsValidDataSet(DST) && DST.Tables[0].Rows[0][0].ToString() == APIResponseCode.DBSuccess)
                {
                    DataSet dst = _payoutRepo.APTDeletePayee(Request);
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
                }
                else
                {
                    objresponse.response_code = "155";
                    objresponse.response_message ="OTP Verification Failed";
                    objresponse.data = null;
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Bene Deletion Update Failed", JsonConvert.SerializeObject(objresponse), Request.mobile_no);
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

        public APTGenericResponse? APTBeneValidate_PaytmPayout(APTTransactionRequest Request)
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
                        beneValidateRequest.customerMobile = "8754554852";//Request.sender_mobile_number;
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
                            updateRequest.beneficiary_name = validateBeneResponseObj.extra_info.beneficiaryName;
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

        public APTGenericResponse? PayoutFundTransfer_BeneValidate(PayoutPaymentRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            APTPayOutTransactionResponse Resp = new APTPayOutTransactionResponse();
            AxisPaymentRequest values = new AxisPaymentRequest();
            YesBankPayRequest yesValues = new YesBankPayRequest();
            string commission = "";

            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.sendermobilenumber);

            try
            {
                DataSet dst = _payoutRepo.PayoutFundTransfer(Request);

                if (dst.Tables[0].Rows[0][0].ToString() == "101")
                {
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Dataset Empty", JsonConvert.SerializeObject(Request), Request.sendermobilenumber);

                    objresponse.response_code = dst.Tables[0].Rows[0][0].ToString();
                    objresponse.response_message = dst.Tables[0].Rows[0][1].ToString();
                    objresponse.data = null;
                } // Axis bank Start
                else if (dst.Tables[0].Rows[0][0].ToString() == "100")
                {

                    APTGetCustomerInfo ReqCustInfo = new APTGetCustomerInfo();
                    ReqCustInfo.agent_ref_id = Request.ARefID;
                    ReqCustInfo.mobile_no = Request.sendermobilenumber;
                    //ReqCustInfo.flag = "1";
                    DataSet CustInfo = _payoutRepo.APTGetCustomerInfo(ReqCustInfo);
                    if (CustInfo != null && CustInfo.Tables[0].Rows.Count > 0 && CustInfo.Tables[0].Columns.Count > 0)
                    {
                        yesValues.TxnPaymode = Request.PayModeRefID;
                        yesValues.TxnAmount = Request.Amount;
                        yesValues.BeneName = "NA";
                        yesValues.BeneAccNum = Request.Accountnumerin;
                        yesValues.BeneIfscCode = Request.ifsccodein;
                        yesValues.BeneBankName = "NA";
                        yesValues.TransactionID = dst.Tables[0].Rows[0]["TransactionID"].ToString();
                        yesValues.EmailAddress = CustInfo.Tables[0].Rows[0]["EMail"].ToString();
                        yesValues.Address1 = CustInfo.Tables[0].Rows[0]["Address1"].ToString();
                        yesValues.Address2 = CustInfo.Tables[0].Rows[0]["Address2"].ToString();
                        yesValues.Pincode = CustInfo.Tables[0].Rows[0]["Pincode"].ToString();
                        yesValues.State = CustInfo.Tables[0].Rows[0]["State"].ToString();
                        yesValues.PhoneNumber = Request.Agentmobile; ////Change value
                        yesValues.customername = Request.sendermobilenumber;
                    }
                    else
                    {
                        yesValues.TxnPaymode = Request.PayModeRefID;
                        yesValues.TxnAmount = Request.Amount;
                        yesValues.BeneName = "";
                        yesValues.BeneAccNum = Request.Accountnumerin;
                        yesValues.BeneIfscCode = Request.ifsccodein;
                        yesValues.BeneBankName = "";
                        yesValues.TransactionID = dst.Tables[0].Rows[0]["TransactionID"].ToString();
                        yesValues.EmailAddress = "";
                        yesValues.Address1 = "";
                        yesValues.Address2 = "";
                        yesValues.Pincode = "603202";
                        yesValues.State = "";
                        yesValues.PhoneNumber = Request.Agentmobile; ////Change value
                        yesValues.customername = Request.sendermobilenumber;
                    }
                    
                    commission = "0";
                    decimal TotalAmount = Convert.ToDecimal(commission) + Convert.ToDecimal(yesValues.TxnAmount);
                    if (string.IsNullOrEmpty(yesValues.EmailAddress))
                    {
                        yesValues.EmailAddress = "customercare@accupaydtech.com";
                    }
                    CombinedResponse paymentRespnse = _yesbanksevice.TransferPayment(yesValues);
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "YPayout Response22", JsonConvert.SerializeObject(paymentRespnse), Request.sendermobilenumber + "y");
                    if (paymentRespnse.ApiResponse == "Failed")
                    {
                        objresponse.response_code = "201";
                        objresponse.response_message = "Failed";
                        objresponse.data = "Failed!... Something Wrong...Please Contact Admin";
                        return objresponse;
                    }
                    var transactionID = dst.Tables[0].Rows[0][2].ToString();
                    YesBankResponse responseStatus = JsonConvert.DeserializeObject<YesBankResponse>(paymentRespnse.ApiResponse);

                    UpdateTxnStatusPayout objResponse = new UpdateTxnStatusPayout();
                    objResponse.TransactionID = transactionID;
                    objResponse.BankReferrenceNumber = responseStatus.Data.TransactionIdentification;
                    objResponse.Request = paymentRespnse.ApiRequest;
                    objResponse.Response = paymentRespnse.ApiResponse;
                    if (responseStatus.Data.Status == "Received") { objResponse.ResponseCode = "S"; } else { objResponse.ResponseCode = "F"; }

                    objResponse.ResponseDescription = "";
                    if (responseStatus.Data.Status != "Received") { objResponse.TranStatus = "3"; } else if (responseStatus.Data.Status == "Received") { objResponse.TranStatus = "2"; }
                    objResponse.Flag = "1";
                    objResponse.BankTypeID = "4"; //hardcoded because didn't receive it from db
                    //DataSet dsts = _payoutRepo.PayoutUpdateResponse(objResponse);

                    if (responseStatus.Data.Status != "Received")
                    {
                        Dictionary<string, string> response = new Dictionary<string, string>();
                        response.Add("result", paymentRespnse.ApiResponse);
                        response.Add("transactionID", transactionID);
                        objresponse.response_code = "201";
                        objresponse.response_message = "Failed";
                        objresponse.data = paymentRespnse.ApiResponse;
                        DataSet dsts = _payoutRepo.PayoutUpdateResponse(objResponse);
                    }
                    else if (responseStatus.Data.Status == "Received")
                    {
                        Thread.Sleep(4000);
                        UpdateTxnStatusPayout objRequestStatus = new UpdateTxnStatusPayout();

                        CombinedResponse statusResponse = _yesbanksevice.CheckStatus(transactionID);
                        if (statusResponse.ApiResponse == "Failed")
                        {
                            objresponse.response_code = "201";
                            objresponse.response_message = "Error";
                            objresponse.data = "Failed!... Something Wrong...Please Contact Admin";
                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "YPayout Success!2", JsonConvert.SerializeObject(objresponse), Request.sendermobilenumber);
                            return objresponse;
                        }


                        YesBankStatusResponse YesCheckResponse = JsonConvert.DeserializeObject<YesBankStatusResponse>(statusResponse.ApiResponse);


                        if (YesCheckResponse.Data.Status == "SettlementCompleted")
                        {
                            objRequestStatus.TransactionID = transactionID;
                            objRequestStatus.BankReferrenceNumber = YesCheckResponse.Data.Initiation.EndToEndIdentification;
                            objRequestStatus.Request = statusResponse.ApiRequest;
                            objRequestStatus.Response = statusResponse.ApiResponse;
                            objRequestStatus.ResponseCode = YesCheckResponse.Data.Status;
                            objRequestStatus.ResponseDescription = "";
                            objRequestStatus.TranStatus = "2"; // Success
                            objRequestStatus.Flag = "2";
                            objRequestStatus.BankTypeID = "4"; //hardcoded because didn't receive it from db


                            DataSet dataset = _payoutRepo.PayoutUpdateResponse(objRequestStatus);
                            APTUpdateBeneNameYesbank_payout UpdateBeneNameReq = new APTUpdateBeneNameYesbank_payout();
                            UpdateBeneNameReq.transaction_id = transactionID;
                            UpdateBeneNameReq.rrn = YesCheckResponse.Data.Initiation.EndToEndIdentification;
                            UpdateBeneNameReq.bene_name = YesCheckResponse.Data.Initiation.CreditorAccount.Name;
                            UpdateBeneNameReq.status_update_datetime = DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt");
                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "YPayout Success!BeneNameUpdateReq", JsonConvert.SerializeObject(UpdateBeneNameReq), Request.sendermobilenumber);
                            DataSet dtst = _payoutRepo.PayoutYesBankUpdateBeneName(UpdateBeneNameReq);
                            //_logService.WriteLog(MethodBase.GetCurrentMethod().Name, "YPayout Success!BeneNameUpdateDBResponse", JsonConvert.SerializeObject(dtst), Request.sendermobilenumber);
                           
                            APTUpdatePayeeRequest obj = new APTUpdatePayeeRequest();
                            obj.npci_name = YesCheckResponse.Data.Initiation.CreditorAccount.Name;
                            obj.validated = "1";
                            obj.payee_ref_id = Request.PayeeRefID;
                            obj.agent_ref_id = Request.ARefID;
                            obj.updated_by = Request.ARefID;
                            obj.mobile_no = Request.sendermobilenumber;
                            DataSet dt = _dmtRepo.APTUpdateBeneValidationStatus(obj);

                            Resp.transaction_date = DateTime.Now.ToString("dd/MM/yyyy");
                            Resp.transaction_id = transactionID;
                            Resp.rrn = YesCheckResponse.Data.Initiation.EndToEndIdentification;
                            Resp.customer_name = yesValues.BeneName;
                            Resp.commision = commission;
                            Resp.bene_acnt_no = yesValues.BeneAccNum;
                            Resp.bene_name = YesCheckResponse.Data.Initiation.CreditorAccount.Name;
                            Resp.totalAmount = TotalAmount.ToString();
                            Resp.amount = yesValues.TxnAmount;
                            objresponse.response_code = "200";
                            objresponse.response_message = "Success";
                            objresponse.data = Resp;//"Your payment request is successfully credited to " + YesCheckResponse.Data.Initiation.CreditorAccount.Name;
                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "YPayout Success!2", JsonConvert.SerializeObject(objresponse), Request.sendermobilenumber);
                        }
                        else if ((YesCheckResponse.Data.Status == "SettlementInProcess") || (YesCheckResponse.Data.Status == "Pending"))
                        {
                            objRequestStatus.TransactionID = transactionID;
                            objRequestStatus.BankReferrenceNumber = (YesCheckResponse.Data.Status == "SettlementInProcess") ? YesCheckResponse.Data.Initiation.EndToEndIdentification : "";
                            objRequestStatus.Request = statusResponse.ApiRequest;
                            objRequestStatus.Response = statusResponse.ApiResponse;
                            objRequestStatus.ResponseCode = YesCheckResponse.Data.Status;
                            objRequestStatus.ResponseDescription = "";
                            objRequestStatus.TranStatus = "1"; // Failed
                            objRequestStatus.Flag = "2";
                            objRequestStatus.BankTypeID = "4"; //hardcoded because didn't receive it from db

                            DataSet dataset = _payoutRepo.PayoutUpdateResponse(objRequestStatus);

                            Resp.transaction_date = DateTime.Now.ToString("dd/MM/yyyy");
                            Resp.transaction_id = transactionID;
                            Resp.rrn = YesCheckResponse.Data.Initiation.EndToEndIdentification;
                            Resp.customer_name = yesValues.BeneName;
                            Resp.commision = commission;
                            Resp.bene_acnt_no = yesValues.BeneAccNum;
                            Resp.bene_name = YesCheckResponse.Data.Initiation.CreditorAccount.Name;
                            Resp.totalAmount = TotalAmount.ToString();
                            Resp.amount = yesValues.TxnAmount;

                            objresponse.response_code = "200";
                            objresponse.response_message = "Pending";
                            objresponse.data = Resp;
                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "YPayout Pending!2", JsonConvert.SerializeObject(objresponse), Request.sendermobilenumber);
                        }
                        else if (YesCheckResponse.Data.Status == "FAILED")
                        {
                            objRequestStatus.TransactionID = transactionID;
                            objRequestStatus.BankReferrenceNumber = YesCheckResponse.Data.Initiation.EndToEndIdentification;
                            objRequestStatus.Request = statusResponse.ApiRequest;
                            objRequestStatus.Response = statusResponse.ApiResponse;
                            objRequestStatus.ResponseCode = YesCheckResponse.Data.Status;
                            objRequestStatus.ResponseDescription = "";
                            objRequestStatus.TranStatus = "3"; // Pending
                            objRequestStatus.Flag = "2";
                            objRequestStatus.BankTypeID = "4"; //hardcoded because didn't receive it from db

                            DataSet dataset = _payoutRepo.PayoutUpdateResponse(objRequestStatus);

                            objresponse.response_code = "202";
                            objresponse.response_message = "Failed";
                            objresponse.data = objRequestStatus;
                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "YPayout Failed!2", JsonConvert.SerializeObject(objresponse), Request.sendermobilenumber);
                        }

                        return objresponse;

                    }
                    else
                    {
                        objresponse.response_code = "201";
                        objresponse.response_message = "Failed";
                        objresponse.data = paymentRespnse;
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "YPayout Failed!!!", JsonConvert.SerializeObject(objresponse), Request.sendermobilenumber);
                        return objresponse;
                    }
                }


            }
            catch (Exception ex)
            {
                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed!!! Ex", JsonConvert.SerializeObject(ex), Request.sendermobilenumber);
            }

            return objresponse;
        }

        public APTGenericResponse? APTUpdateCustomerKYC_Payout(APTUpdateCustomerKYCPayoutRequest Request)
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.transaction_id);

            APTGenericResponse objresponse = new APTGenericResponse();
            try
            {
                //if (Request.approval_status.ToUpper() == "REQUIRED")
                //{
                //    Request.approval_status = "2";
                //}
                //else if (Request.approval_status.ToUpper() == "NOT_REQUIRED")
                //{
                //    Request.approval_status = "5";
                //}
                //else if (Request.approval_status.ToUpper() == "PENDING")
                //{
                //    Request.approval_status = "1";
                //}
                //else
                //{
                //    objresponse.response_code = "150";
                //    objresponse.response_message = "Invalid Approval status : it should be Required or Not_Required";
                //    objresponse.data = null;
                //    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "KYC Update Failed", JsonConvert.SerializeObject(objresponse), Request.approval_status+ Request.transaction_id);
                //    return objresponse;
                //}
                DataSet DST = _payoutRepo.APTUpdateKYC_CustomerPayout(Request);
                if (_commonService.IsValidDataSet(DST))
                {
                    if (DST.Tables[0].Rows[0][0].ToString() == APIResponseCode.DBSuccess)
                    {
                        objresponse.response_code = APIResponseCode.Success;
                        objresponse.response_message = "KYC Updated successfully";
                        objresponse.data = null;
                    }
                    else
                    {
                        objresponse.response_code = DST.Tables[0].Rows[0][0].ToString();
                        objresponse.response_message = DST.Tables[0].Rows[0][1].ToString();
                        objresponse.data = null;
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "KYC Update Failed", JsonConvert.SerializeObject(objresponse), Request.approval_status + Request.transaction_id);

                    }
                }
            }
                //_logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Bene Deletion Failed", JsonConvert.SerializeObject(objAPIRequest), Request.mobile_no);

            catch (Exception ex)
            {
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.approval_status + Request.transaction_id);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
            }

            return objresponse;
        }

        public APTGenericResponse? APTPayoutFundTransfer_v(string Amount, string mode)
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", Amount + "---"+mode, "");
            CombinedResponse Response = new CombinedResponse();
            APTGenericResponse objresponse = new APTGenericResponse();
            try
            {
                Random rnd = new Random();
                int txn = rnd.Next(10000000, 99999999);
                string Txnid = "APV" + txn.ToString();
                Response = _yesbanksevice.TransferPayment_vignesh(Amount,mode, Txnid);
                UpdateTxnStatusPayout objResponse = new UpdateTxnStatusPayout();
                if (Response.ApiResponse == "Failed")
                {
                    objResponse.BankReferrenceNumber = "NA";
                    objResponse.TranStatus = "Failed";
                    objResponse.TransactionID = Txnid;
                    objresponse.response_code = "100";
                    objresponse.response_message = "Please check status using Txnid";
                    objresponse.data = objResponse;
                    return objresponse;
                }
                var transactionID = Txnid;
                YesBankResponse responseStatus = JsonConvert.DeserializeObject<YesBankResponse>(Response.ApiResponse);

                
                objResponse.TransactionID = Txnid;
                objResponse.BankReferrenceNumber = responseStatus.Data.TransactionIdentification;
                objResponse.Request = Response.ApiRequest;
                objResponse.Response = Response.ApiResponse;

                objresponse.response_code = "200";
                objresponse.response_message = "Txn Initiated ,Please check status using Txnid";
                objresponse.data = objResponse;
                return objresponse;
            }
            catch(Exception ex)
            {
                objresponse.response_code = "500";
                objresponse.response_message = "Internal Server error";
                objresponse.data = ex;
                return objresponse;
            }
        }

        public APTGenericResponse? APTSendOTP(string Amount, string mobileno, string Bene, string TxnId)
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", TxnId, mobileno);
           //string smscontent = "Dear Customer, Money Transfer of Rs.55000 to Arun has been successfully processed. Ref ID: YIP202306160221 - ACCUPAYD TECH";

            string smscontent = "Dear Customer, Money Transfer of Rs." + Amount + " to " + Bene + " has been successfully processed. Ref ID: " + TxnId + " - ACCUPAYD TECH";
            string SMSUrl = Configuration["SmsSettings:URL"];
            string Username = Configuration["SmsSettings:Username"];
            string Password = Configuration["SmsSettings:Password"];
            string from = Configuration["SmsSettings:from"];
            string to = mobileno;

            APTGenericResponse objresponse = new APTGenericResponse();
            try
            {
                var client = new HttpClient();
                var request = SMSUrl + "username=" + Username + "&password=" + Password
                    + "&to=" + to + "&from=" + from + "&content=" + smscontent;
                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "SMS API Request", request, mobileno);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls |
                    SecurityProtocolType.Tls11;
                HttpWebRequest webReq = WebRequest.Create(request) as HttpWebRequest;
                webReq.ContentType = "application/json;charset=utf-8";
                webReq.Method = "GET";
                webReq.Timeout = 1800000;
                HttpWebResponse response;
                response = (HttpWebResponse)webReq.GetResponse();

                Stream responseStream = response.GetResponseStream();
                string responsestring = new StreamReader(responseStream).ReadToEnd();
                SMSResponse objsmsresponse = JsonConvert.DeserializeObject<SMSResponse>(responsestring);
                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "SMS API Response", JsonConvert.SerializeObject(objsmsresponse), mobileno);

                if (objsmsresponse != null)
                {
                    if (objsmsresponse.status.code == "200")
                    {
                        objresponse.response_code = APIResponseCode.Success;
                        objresponse.response_message = APIResponseCodeDesc.Success;
                    }
                    else
                    {
                        objresponse.response_code = objsmsresponse.status.code;
                        objresponse.response_message = objsmsresponse.status.reason;
                    }

                }

            }
            catch (Exception ex)
            {
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, mobileno);

                return null;
            }
            return objresponse;
        }

        public APTGenericResponse? APTPGtoPayoutTxn(PayOutPGtoPayoutTxnRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();

           // APTGenericResponse objresponse = new APTGenericResponse();
            APTPayOutTransactionResponse Resp = new APTPayOutTransactionResponse();
            AxisPaymentRequest values = new AxisPaymentRequest();
            YesBankPayRequest yesValues = new YesBankPayRequest();
            string commission = "";
            try
            {
                DataSet Dst = _payoutRepo.APTGetPGtoPayoutTxn(Request.transactionID);
                if(_commonService.IsValidDataSet(Dst))
                {
                    if (Dst.Tables[0].Rows[0][0].ToString() == "100" && Dst.Tables[0].Rows[0]["Banktypeid"].ToString() == "3")
                    {
                        values.txnPaymode = Dst.Tables[0].Rows[0]["PayModeRefID"].ToString();
                        values.txnAmount = Dst.Tables[0].Rows[0]["Amount"].ToString();
                        values.beneName = Dst.Tables[0].Rows[0]["BeneName"].ToString();
                        values.beneCode = Dst.Tables[0].Rows[0]["BeneCode"].ToString(); //49395
                        values.beneAccNum = Dst.Tables[0].Rows[0]["BeneAccountNumber"].ToString();
                        values.beneIfscCode = Dst.Tables[0].Rows[0]["BeneIFSCCode"].ToString();
                        values.beneBankName = Dst.Tables[0].Rows[0]["BeneBankName"].ToString();
                        values.transactionID = Dst.Tables[0].Rows[0]["TransactionID"].ToString();
                        commission = Dst.Tables[0].Rows[0]["CustomerFee"].ToString();
                        //APTSendOTP(values.txnAmount,Request.sendermobilenumber, values.beneName, values.transactionID);
                        CombinedResponse paymentRespnse = _axisservice.FundTransfer(values);
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Axis Payout Request Response", JsonConvert.SerializeObject(paymentRespnse), Request.transactionID + "a");
                        if (paymentRespnse.ApiResponse == "Failed")
                        {
                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed!... Something Wrong...Please Contact Admin", JsonConvert.SerializeObject(paymentRespnse), Request.transactionID);

                            objresponse.response_code = "201";
                            objresponse.response_message = "Failed";
                            objresponse.data = "Failed!... Something Wrong...Please Contact Admin";
                            return objresponse;
                        }
                        var transactionID = Dst.Tables[0].Rows[0]["TransactionID"].ToString();
                        TransferResponseAxis responseStatus = JsonConvert.DeserializeObject<TransferResponseAxis>(paymentRespnse.ApiResponse);

                        UpdateTxnStatusPayout objResponse = new UpdateTxnStatusPayout();
                        objResponse.TransactionID = transactionID;
                        objResponse.BankReferrenceNumber = "";
                        objResponse.Request = paymentRespnse.ApiRequest;
                        objResponse.Response = paymentRespnse.ApiResponse;
                        objResponse.ResponseCode = responseStatus.status;
                        objResponse.ResponseDescription = responseStatus.message;
                        if (responseStatus.status.ToUpper() == "F") { objResponse.TranStatus = "3"; } else if (responseStatus.status.ToUpper() == "S") { objResponse.TranStatus = "2"; }
                        objResponse.Flag = "1";
                        objResponse.BankTypeID = Dst.Tables[0].Rows[0]["Banktypeid"].ToString();


                        //DataSet dsts = _payoutRepo.PayoutUpdateResponse(objResponse);

                        if (responseStatus.status.ToUpper() == "F")
                        {
                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed", JsonConvert.SerializeObject(paymentRespnse), Request.transactionID);

                            Dictionary<string, string> response = new Dictionary<string, string>();
                            response.Add("result", paymentRespnse.ApiResponse);
                            response.Add("transactionID", transactionID);
                            objresponse.response_code = "201";
                            objresponse.response_message = "Failed";
                            objresponse.data = paymentRespnse.ApiResponse;
                            DataSet dsts = _payoutRepo.PayoutUpdateResponse(objResponse);
                        }
                        else if (responseStatus.status.ToUpper() == "S")
                        {
                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Error", JsonConvert.SerializeObject(paymentRespnse), Request.transactionID);

                            Thread.Sleep(30000);
                            decimal TotalAmount = Convert.ToDecimal(commission) + Convert.ToDecimal(values.txnAmount);
                            UpdateTxnStatusPayout objRequestStatus = new UpdateTxnStatusPayout();


                            CombinedResponse statusResponse = _axisservice.CheckStatus(transactionID);
                            if (statusResponse.ApiResponse == "Failed")
                            {
                                objresponse.response_code = "201";
                                objresponse.response_message = "Error";
                                objresponse.data = "Failed!... Something Wrong...Please Contact Admin";
                                return objresponse;
                            }
                            GetStatusResponseBody axisCheckResponse = JsonConvert.DeserializeObject<GetStatusResponseBody>(statusResponse.ApiResponse);
                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Error", JsonConvert.SerializeObject(axisCheckResponse), Request.transactionID);

                            if (axisCheckResponse.status.ToUpper() == "S")
                            {
                                if (axisCheckResponse.data.CUR_TXN_ENQ.Count > 0)
                                {
                                    if (axisCheckResponse.data.CUR_TXN_ENQ[0].transactionStatus == "PROCESSED")
                                    {
                                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Error", JsonConvert.SerializeObject(axisCheckResponse), Request.transactionID);

                                        objRequestStatus.TransactionID = transactionID;
                                        objRequestStatus.BankReferrenceNumber = axisCheckResponse.data.CUR_TXN_ENQ[0].utrNo;
                                        objRequestStatus.Request = statusResponse.ApiRequest;
                                        objRequestStatus.Response = statusResponse.ApiResponse;
                                        objRequestStatus.ResponseCode = axisCheckResponse.data.CUR_TXN_ENQ[0].transactionStatus;
                                        objRequestStatus.ResponseDescription = axisCheckResponse.data.CUR_TXN_ENQ[0].statusDescription;
                                        objRequestStatus.TranStatus = "2"; // Success
                                        objRequestStatus.Flag = "2";
                                        objRequestStatus.BankTypeID = Dst.Tables[0].Rows[0]["Banktypeid"].ToString();


                                        DataSet dataset = _payoutRepo.PayoutUpdateResponse(objRequestStatus);
                                        try
                                        {
                                            if (dataset.Tables[0].Rows[0]["smsflag"].ToString() == "2")
                                            {
                                                if (dataset.Tables[0].Rows.Count > 0)
                                                {
                                                    string Amount = dataset.Tables[0].Rows[0]["amount"].ToString();
                                                    string PayeeName = dataset.Tables[0].Rows[0]["payeename"].ToString();
                                                    string MobileNo = dataset.Tables[0].Rows[0]["SenderMobileNumber"].ToString();
                                                    string RefId = dataset.Tables[0].Rows[0]["Refid"].ToString();

                                                    APTSendOTP(Amount, MobileNo, PayeeName, RefId);
                                                }
                                                else
                                                {

                                                }
                                            }

                                        }
                                        catch { }
                                        Resp.transaction_date = DateTime.Now.ToString("dd/MM/yyyy");
                                        Resp.transaction_id = transactionID;
                                        Resp.rrn = axisCheckResponse.data.CUR_TXN_ENQ[0].utrNo;
                                        Resp.customer_name = values.beneName;
                                        Resp.commision = commission;
                                        Resp.bene_acnt_no = values.beneAccNum;
                                        Resp.bene_name = values.beneName;
                                        Resp.totalAmount = TotalAmount.ToString();
                                        Resp.amount = values.txnAmount;

                                        objresponse.response_code = "200";
                                        objresponse.response_message = "Success";
                                        objresponse.data = Resp;
                                    }
                                    else if (axisCheckResponse.data.CUR_TXN_ENQ[0].transactionStatus == "REJECTED" || axisCheckResponse.data.CUR_TXN_ENQ[0].transactionStatus == "RETURN" || axisCheckResponse.data.CUR_TXN_ENQ[0].transactionStatus == "FAILED")
                                    {
                                        objRequestStatus.TransactionID = transactionID;
                                        objRequestStatus.BankReferrenceNumber = axisCheckResponse.data.CUR_TXN_ENQ[0].utrNo;
                                        objRequestStatus.Request = statusResponse.ApiRequest;
                                        objRequestStatus.Response = statusResponse.ApiResponse;
                                        objRequestStatus.ResponseCode = axisCheckResponse.data.CUR_TXN_ENQ[0].transactionStatus;
                                        objRequestStatus.ResponseDescription = axisCheckResponse.data.CUR_TXN_ENQ[0].statusDescription;
                                        objRequestStatus.TranStatus = "3"; // Failed
                                        objRequestStatus.Flag = "2";
                                        objRequestStatus.BankTypeID = Dst.Tables[0].Rows[0]["Banktypeid"].ToString();

                                        DataSet dataset = _payoutRepo.PayoutUpdateResponse(objRequestStatus);

                                        objresponse.response_code = "202";
                                        objresponse.response_message = "Transaction Failed";
                                        objresponse.data = "Your payment request is FAILED";
                                    }
                                    else if (axisCheckResponse.data.CUR_TXN_ENQ[0].transactionStatus == "PENDING")
                                    {

                                        objRequestStatus.TransactionID = transactionID;
                                        objRequestStatus.BankReferrenceNumber = axisCheckResponse.data.CUR_TXN_ENQ[0].utrNo;
                                        objRequestStatus.Request = statusResponse.ApiRequest;
                                        objRequestStatus.Response = statusResponse.ApiResponse;
                                        objRequestStatus.ResponseCode = axisCheckResponse.data.CUR_TXN_ENQ[0].transactionStatus;
                                        objRequestStatus.ResponseDescription = axisCheckResponse.data.CUR_TXN_ENQ[0].statusDescription;
                                        objRequestStatus.TranStatus = "1"; // Pending
                                        objRequestStatus.Flag = "2";
                                        objRequestStatus.BankTypeID = Dst.Tables[0].Rows[0]["Banktypeid"].ToString();

                                        DataSet dataset = _payoutRepo.PayoutUpdateResponse(objRequestStatus);
                                        try
                                        {
                                            if (dataset.Tables[0].Rows[0]["smsflag"].ToString() == "2")
                                            {
                                                if (dataset.Tables[0].Rows.Count > 0)
                                                {
                                                    string Amount = dataset.Tables[0].Rows[0]["amount"].ToString();
                                                    string PayeeName = dataset.Tables[0].Rows[0]["payeename"].ToString();
                                                    string MobileNo = dataset.Tables[0].Rows[0]["SenderMobileNumber"].ToString();
                                                    string RefId = dataset.Tables[0].Rows[0]["Refid"].ToString();

                                                    APTSendOTP(Amount, MobileNo, PayeeName, RefId);
                                                }
                                                else
                                                {

                                                }
                                            }

                                        }
                                        catch { }
                                        Resp.transaction_date = DateTime.Now.ToString("dd/MM/yyyy");
                                        Resp.transaction_id = transactionID;
                                        Resp.rrn = axisCheckResponse.data.CUR_TXN_ENQ[0].utrNo;
                                        Resp.customer_name = values.beneName;
                                        Resp.commision = commission;
                                        Resp.bene_acnt_no = values.beneAccNum;
                                        Resp.bene_name = values.beneName;
                                        Resp.totalAmount = TotalAmount.ToString();
                                        Resp.amount = values.txnAmount;

                                        objresponse.response_code = "200";
                                        objresponse.response_message = "Pending";
                                        objresponse.data = Resp;
                                    }
                                }
                                else
                                {
                                    objRequestStatus.TransactionID = transactionID;
                                    objRequestStatus.BankReferrenceNumber = "";
                                    objRequestStatus.Request = statusResponse.ApiRequest;
                                    objRequestStatus.Response = statusResponse.ApiResponse;
                                    objRequestStatus.ResponseCode = axisCheckResponse.data.errorMessage;
                                    objRequestStatus.ResponseDescription = "";
                                    objRequestStatus.TranStatus = "3"; // Failed
                                    objRequestStatus.Flag = "2";
                                    objRequestStatus.BankTypeID = Dst.Tables[0].Rows[0]["Banktypeid"].ToString();

                                    DataSet dataset = _payoutRepo.PayoutUpdateResponse(objRequestStatus);

                                    objresponse.response_code = "202";
                                    objresponse.response_message = "Transaction Failed";
                                    objresponse.data = "Your payment request is FAILED";
                                }

                            }
                            else if (axisCheckResponse.status.ToUpper() == "F")
                            {
                                objRequestStatus.TransactionID = transactionID;
                                objRequestStatus.BankReferrenceNumber = axisCheckResponse.data.CUR_TXN_ENQ[0].utrNo;
                                objRequestStatus.Request = statusResponse.ApiRequest;
                                objRequestStatus.Response = statusResponse.ApiResponse;
                                objRequestStatus.ResponseCode = axisCheckResponse.data.CUR_TXN_ENQ[0].transactionStatus;
                                objRequestStatus.ResponseDescription = axisCheckResponse.data.CUR_TXN_ENQ[0].statusDescription;
                                objRequestStatus.TranStatus = "3"; // Pending
                                objRequestStatus.Flag = "2";
                                objRequestStatus.BankTypeID = Dst.Tables[0].Rows[0]["Banktypeid"].ToString();

                                DataSet dataset = _payoutRepo.PayoutUpdateResponse(objRequestStatus);

                                objresponse.response_code = "105";
                                objresponse.response_message = "Failed";
                                objresponse.data = "Your payment request is Failed";
                            }

                            return objresponse;

                        }
                        else
                        {
                            objresponse.response_code = "201";
                            objresponse.response_message = "Failed";
                            objresponse.data = paymentRespnse;
                            return objresponse;
                        }
                    }
                    // Yes bank Start
                    else if (Dst.Tables[0].Rows[0][0].ToString() == "100" && Dst.Tables[0].Rows[0]["Banktypeid"].ToString() == "4")
                    {
                        yesValues.TxnPaymode = Dst.Tables[0].Rows[0]["PayModeRefID"].ToString();
                        yesValues.TxnAmount = Dst.Tables[0].Rows[0]["Amount"].ToString();
                        yesValues.BeneName = Dst.Tables[0].Rows[0]["BeneName"].ToString();
                        yesValues.BeneAccNum = Dst.Tables[0].Rows[0]["BeneAccountNumber"].ToString();
                        yesValues.BeneIfscCode = Dst.Tables[0].Rows[0]["BeneIFSCCode"].ToString();
                        yesValues.BeneBankName = Dst.Tables[0].Rows[0]["BeneBankName"].ToString();
                        yesValues.TransactionID = Dst.Tables[0].Rows[0]["TransactionID"].ToString();
                        yesValues.EmailAddress = Dst.Tables[0].Rows[0]["Email"].ToString();
                        yesValues.Address1 = Dst.Tables[0].Rows[0]["Address1"].ToString();
                        yesValues.Address2 = Dst.Tables[0].Rows[0]["Address2"].ToString();
                        yesValues.Pincode = Dst.Tables[0].Rows[0]["Pincode"].ToString();
                        yesValues.State = Dst.Tables[0].Rows[0]["State"].ToString();
                        yesValues.PhoneNumber = Dst.Tables[0].Rows[0]["Agentmobile"].ToString(); ////Change value
                        yesValues.customername = Dst.Tables[0].Rows[0]["customername"].ToString();
                        commission = Dst.Tables[0].Rows[0]["CustomerFee"].ToString();
                        decimal TotalAmount = Convert.ToDecimal(commission) + Convert.ToDecimal(yesValues.TxnAmount);
                        // APTSendOTP(yesValues.TxnAmount, Request.sendermobilenumber, yesValues.BeneName, yesValues.TransactionID);
                        if (string.IsNullOrEmpty(yesValues.EmailAddress))
                        {
                            yesValues.EmailAddress = "customercare@accupaydtech.com";
                        }
                        CombinedResponse paymentRespnse = _yesbanksevice.TransferPayment(yesValues);
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "YPayout Response22", JsonConvert.SerializeObject(paymentRespnse), Request.transactionID + "y");
                        if (paymentRespnse.ApiResponse == "Failed")
                        {
                            objresponse.response_code = "201";
                            objresponse.response_message = "Failed";
                            objresponse.data = "Failed!... Something Wrong...Please Contact Admin";
                            return objresponse;
                        }
                        var transactionID = Dst.Tables[0].Rows[0]["TransactionID"].ToString();
                        YesBankResponse responseStatus = JsonConvert.DeserializeObject<YesBankResponse>(paymentRespnse.ApiResponse);

                        UpdateTxnStatusPayout objResponse = new UpdateTxnStatusPayout();
                        objResponse.TransactionID = transactionID;
                        objResponse.BankReferrenceNumber = responseStatus.Data.TransactionIdentification;
                        objResponse.Request = paymentRespnse.ApiRequest;
                        objResponse.Response = paymentRespnse.ApiResponse;
                        if (responseStatus.Data.Status == "Received") { objResponse.ResponseCode = "S"; } else { objResponse.ResponseCode = "F"; }

                        objResponse.ResponseDescription = "";
                        if (responseStatus.Data.Status != "Received") { objResponse.TranStatus = "3"; } else if (responseStatus.Data.Status == "Received") { objResponse.TranStatus = "2"; }
                        objResponse.Flag = "1";
                        objResponse.BankTypeID = Dst.Tables[0].Rows[0]["Banktypeid"].ToString();
                        //DataSet dsts = _payoutRepo.PayoutUpdateResponse(objResponse);

                        if (responseStatus.Data.Status != "Received")
                        {
                            Dictionary<string, string> response = new Dictionary<string, string>();
                            response.Add("result", paymentRespnse.ApiResponse);
                            response.Add("transactionID", transactionID);
                            objresponse.response_code = "201";
                            objresponse.response_message = "Failed";
                            objresponse.data = paymentRespnse.ApiResponse;
                            DataSet dsts = _payoutRepo.PayoutUpdateResponse(objResponse);
                        }
                        else if (responseStatus.Data.Status == "Received")
                        {
                            Thread.Sleep(4000);
                            UpdateTxnStatusPayout objRequestStatus = new UpdateTxnStatusPayout();

                            CombinedResponse statusResponse = _yesbanksevice.CheckStatus(transactionID);
                            if (statusResponse.ApiResponse == "Failed")
                            {
                                objresponse.response_code = "201";
                                objresponse.response_message = "Error";
                                objresponse.data = "Failed!... Something Wrong...Please Contact Admin";
                                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "YPayout Success!2", JsonConvert.SerializeObject(objresponse), Request.transactionID);
                                return objresponse;
                            }


                            YesBankStatusResponse YesCheckResponse = JsonConvert.DeserializeObject<YesBankStatusResponse>(statusResponse.ApiResponse);


                            if (YesCheckResponse.Data.Status == "SettlementCompleted")
                            {
                                objRequestStatus.TransactionID = transactionID;
                                objRequestStatus.BankReferrenceNumber = YesCheckResponse.Data.Initiation.EndToEndIdentification;
                                objRequestStatus.Request = statusResponse.ApiRequest;
                                objRequestStatus.Response = statusResponse.ApiResponse;
                                objRequestStatus.ResponseCode = YesCheckResponse.Data.Status;
                                objRequestStatus.ResponseDescription = "";
                                objRequestStatus.TranStatus = "2"; // Success
                                objRequestStatus.Flag = "2";
                                objRequestStatus.BankTypeID = Dst.Tables[0].Rows[0]["Banktypeid"].ToString();


                                DataSet dataset = _payoutRepo.PayoutUpdateResponse(objRequestStatus);
                                try
                                {
                                    if (dataset.Tables[0].Rows[0]["smsflag"].ToString() == "2")
                                    {
                                        if (dataset.Tables[0].Rows.Count > 0)
                                        {
                                            string Amount = dataset.Tables[0].Rows[0]["amount"].ToString();
                                            string PayeeName = dataset.Tables[0].Rows[0]["payeename"].ToString();
                                            string MobileNo = dataset.Tables[0].Rows[0]["SenderMobileNumber"].ToString();
                                            string RefId = dataset.Tables[0].Rows[0]["Refid"].ToString();

                                            APTSendOTP(Amount, MobileNo, PayeeName, RefId);
                                        }
                                        else
                                        {

                                        }
                                    }

                                }
                                catch { }
                                APTUpdateBeneNameYesbank_payout UpdateBeneNameReq = new APTUpdateBeneNameYesbank_payout();
                                UpdateBeneNameReq.transaction_id = transactionID;
                                UpdateBeneNameReq.rrn = YesCheckResponse.Data.Initiation.EndToEndIdentification;
                                UpdateBeneNameReq.bene_name = YesCheckResponse.Data.Initiation.CreditorAccount.Name;
                                UpdateBeneNameReq.status_update_datetime = DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt");
                                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "YPayout Success!BeneNameUpdateReq", JsonConvert.SerializeObject(UpdateBeneNameReq), Request.transactionID);
                                DataSet dtst = _payoutRepo.PayoutYesBankUpdateBeneName(UpdateBeneNameReq);
                                //_logService.WriteLog(MethodBase.GetCurrentMethod().Name, "YPayout Success!BeneNameUpdateDBResponse", JsonConvert.SerializeObject(dtst), Request.sendermobilenumber);

                                Resp.transaction_date = DateTime.Now.ToString("dd/MM/yyyy");
                                Resp.transaction_id = transactionID;
                                Resp.rrn = YesCheckResponse.Data.Initiation.EndToEndIdentification;
                                Resp.customer_name = yesValues.BeneName;
                                Resp.commision = commission;
                                Resp.bene_acnt_no = yesValues.BeneAccNum;
                                Resp.bene_name = yesValues.BeneName;
                                Resp.totalAmount = TotalAmount.ToString();
                                Resp.amount = yesValues.TxnAmount;
                                objresponse.response_code = "200";
                                objresponse.response_message = "Success";
                                objresponse.data = Resp;//"Your payment request is successfully credited to " + YesCheckResponse.Data.Initiation.CreditorAccount.Name;
                                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "YPayout Success!2", JsonConvert.SerializeObject(objresponse), Request.transactionID);
                            }
                            else if ((YesCheckResponse.Data.Status == "SettlementInProcess") || (YesCheckResponse.Data.Status == "Pending") || (YesCheckResponse.Data.Status == "Accepted"))
                            {
                                objRequestStatus.TransactionID = transactionID;
                                objRequestStatus.BankReferrenceNumber = (YesCheckResponse.Data.Status == "SettlementInProcess") ? YesCheckResponse.Data.Initiation.EndToEndIdentification : "";
                                objRequestStatus.Request = statusResponse.ApiRequest;
                                objRequestStatus.Response = statusResponse.ApiResponse;
                                objRequestStatus.ResponseCode = YesCheckResponse.Data.Status;
                                objRequestStatus.ResponseDescription = "";
                                objRequestStatus.TranStatus = "1"; // Failed
                                objRequestStatus.Flag = "2";
                                objRequestStatus.BankTypeID = Dst.Tables[0].Rows[0]["Banktypeid"].ToString();

                                DataSet dataset = _payoutRepo.PayoutUpdateResponse(objRequestStatus);
                                try
                                {
                                    if (dataset.Tables[0].Rows[0]["smsflag"].ToString() == "2")
                                    {
                                        if (dataset.Tables[0].Rows.Count > 0)
                                        {
                                            string Amount = dataset.Tables[0].Rows[0]["amount"].ToString();
                                            string PayeeName = dataset.Tables[0].Rows[0]["payeename"].ToString();
                                            string MobileNo = dataset.Tables[0].Rows[0]["SenderMobileNumber"].ToString();
                                            string RefId = dataset.Tables[0].Rows[0]["Refid"].ToString();

                                            APTSendOTP(Amount, MobileNo, PayeeName, RefId);
                                        }
                                        else
                                        {

                                        }
                                    }

                                }
                                catch { }
                                Resp.transaction_date = DateTime.Now.ToString("dd/MM/yyyy");
                                Resp.transaction_id = transactionID;
                                Resp.rrn = YesCheckResponse.Data.Initiation.EndToEndIdentification;
                                Resp.customer_name = yesValues.BeneName;
                                Resp.commision = commission;
                                Resp.bene_acnt_no = yesValues.BeneAccNum;
                                Resp.bene_name = yesValues.BeneName;
                                Resp.totalAmount = TotalAmount.ToString();
                                Resp.amount = yesValues.TxnAmount;

                                objresponse.response_code = "200";
                                objresponse.response_message = "Pending";
                                objresponse.data = Resp;
                                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "YPayout Pending!2", JsonConvert.SerializeObject(objresponse), Request.transactionID);
                            }
                            else if (YesCheckResponse.Data.Status == "FAILED")
                            {
                                objRequestStatus.TransactionID = transactionID;
                                objRequestStatus.BankReferrenceNumber = YesCheckResponse.Data.Initiation.EndToEndIdentification;
                                objRequestStatus.Request = statusResponse.ApiRequest;
                                objRequestStatus.Response = statusResponse.ApiResponse;
                                objRequestStatus.ResponseCode = YesCheckResponse.Data.Status;
                                objRequestStatus.ResponseDescription = "";
                                objRequestStatus.TranStatus = "3"; // Pending
                                objRequestStatus.Flag = "2";
                                objRequestStatus.BankTypeID = Dst.Tables[0].Rows[0]["Banktypeid"].ToString();

                                DataSet dataset = _payoutRepo.PayoutUpdateResponse(objRequestStatus);

                                objresponse.response_code = "202";
                                objresponse.response_message = "Failed";
                                objresponse.data = objRequestStatus;
                                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "YPayout Failed!2", JsonConvert.SerializeObject(objresponse), Request.transactionID);
                            }

                            return objresponse;

                        }
                        else
                        {
                            objresponse.response_code = "201";
                            objresponse.response_message = "Failed";
                            objresponse.data = paymentRespnse;
                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "YPayout Failed!!!", JsonConvert.SerializeObject(objresponse), Request.transactionID);
                            return objresponse;
                        }
                    }
                    else if (Dst.Tables[0].Rows[0][0].ToString() == "101")
                    {
                        objresponse.response_code = "205";
                        objresponse.response_message = "Insufficient Balance";
                        objresponse.data = null;
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed!!! Dataset", JsonConvert.SerializeObject(objresponse), Request.transactionID);
                    }
                }
                else
                {
                    objresponse.response_code = APIResponseCode.DatasetNull;
                    objresponse.response_message = APIResponseCodeDesc.DatasetNull;
                    objresponse.data = null;
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed!!! Dataset", JsonConvert.SerializeObject(objresponse), Request.transactionID);
                }
                
            }
            catch(Exception ex)
            {
                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Failed!!! Ex", JsonConvert.SerializeObject(ex), Request.transactionID);
            }
            return objresponse;
        }

    }
}
