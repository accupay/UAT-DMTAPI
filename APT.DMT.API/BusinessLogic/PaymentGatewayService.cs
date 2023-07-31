using APT.DMT.API.BusinessLogic.BankService;
using APT.DMT.API.BusinessLogic;
using APT.DMT.API.DataAccessLayer.DMT;
using System.Data;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using File = System.IO.File;
using APT.DMT.API.BusinessObjects.Models;
using APT.PaymentServices.API.BusinessObjects.Models;
using Newtonsoft.Json;
using APT.PaymentServices.API.BusinessLogic.BankService;
using System;
using APT.PaymentServices.API.DataAccessLayer;
using APT.DMT.API.BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;
using System.Buffers.Text;
using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc.Razor;
using Razorpay.Api;
using System.Net.Security;
using System.Data.SqlClient;
using System.Net.Http.Headers;
using static System.Net.Mime.MediaTypeNames;

namespace APT.PaymentServices.API.BusinessLogic
{
    public class PaymentGatewayService
    {

        private readonly string SessionValidationURL;
        private readonly string IFSCLookupURL;

        public static IConfiguration Configuration { get; set; }
        public static CommonService _commonService = new CommonService();

        public static PaymentGatewayRepository _pgRepo = new PaymentGatewayRepository();
        public static PGCashFreeAPI _cashFreeAPI = new PGCashFreeAPI();

        public static LogService _logService = new LogService();
        private string SecretKey { get; set; } = string.Empty;
        private string RazorpaySecretKey { get; set; } = string.Empty;
        private string RazorpaySecretId { get; set; } = string.Empty;
        public PaymentGatewayService()
        {
            Configuration = _commonService.GetConfiguration();
            SessionValidationURL = Configuration["SessionValidation:Domain"] + Configuration["SessionValidation:URL"];
            IFSCLookupURL = Configuration["Cache:Domain"] + Configuration["Cache:IFSCLookupURL"];
            SecretKey = Configuration["CashFreePGValues:SecretKey"];
            RazorpaySecretKey = Configuration["RazorpayValues:SecretKey"];
            RazorpaySecretId = Configuration["RazorpayValues:SecretID"];
        }

        public APTGenericResponse? APTCreatePaymentGatewayOrder(InsertPGTransactionRequest Request)
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.customer_phone);

            APTGenericResponse objresponse = new APTGenericResponse();
            CashFreeOrderDetailsResponse objAPIResponse = new CashFreeOrderDetailsResponse();
            InsertPGTransactionResponse objInserttxnResponse = new InsertPGTransactionResponse();
            try
            {
                DataSet dst = _pgRepo.PGDecider(Request.agent_mobile_number, Request.pg_service_id.ToString());
                if (dst != null && dst.Tables.Count > 0 && dst.Tables[0].Rows != null)
                {
                    if (dst.Tables[0].Rows[0][0].ToString() == "7")
                    {
                        CashFreePGCreateOrderRequest objRequest = new CashFreePGCreateOrderRequest();
                        objRequest.customer_details = new CustomerDetails();
                        objRequest.customer_details.customer_phone = Request.customer_phone;
                        objRequest.customer_details.customer_id = Request.customer_phone;
                        objRequest.order_meta = new OrderMeta();
                        objRequest.order_meta.return_url = Configuration["CashFreePGValues:CashFreeAdminDomain"] + Configuration["CashFreePGValues:returnURL"];
                        objRequest.order_amount = Request.transfer_amount;
                        objRequest.order_currency = "INR";
                        objRequest.order_id = "CF" + DateTime.UtcNow.Date.ToString("ddMMyyyy") + GenerateRandomNo().ToString();

                        objAPIResponse = _cashFreeAPI.CreateOrder(objRequest);
                        if (objAPIResponse != null)
                        {
                            Request.order_id = objAPIResponse.order_id;
                            Request.payment_session_id = objAPIResponse.payment_session_id;
                        }
                        objInserttxnResponse.payment_url = Configuration["CashFreePGValues:CashFreeAdminDomain"] + Configuration["CashFreePGValues:PaymentURL"];


                    }
                    else if (dst.Tables[0].Rows[0][0].ToString() == "2")
                    {
                        // Converting amount to paise for razorpay eg rs 1 is 100 paise  --start
                        Int64 Amt;
                        decimal RPAmount = Convert.ToDecimal(Request.transfer_amount);
                        string ActulAmount = string.Format("{0:0.00}", RPAmount);
                        var my = string.Format("{0:0.00}", RPAmount);
                        var strArr = my.ToString().Split('.').ToArray();
                        string finalString = strArr[0].PadLeft(3, '0') + strArr[1];

                        RPAmount = Convert.ToDecimal(finalString);
                        Amt = Convert.ToInt64(RPAmount);
                        // Converting amount to paise for razorpay eg rs 1 is 100 paise  --start

                        string searchref = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // TLS 1.2
                        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
                        Dictionary<string, object> input = new Dictionary<string, object>();
                        input.Add("amount", Amt); // this amount should be same as transaction amount
                        input.Add("currency", "INR");
                        input.Add("receipt", searchref);
                        input.Add("payment_capture", 1);
                        try
                        {
                            try
                            {
                                string orderId = RazorPayGenerateOrderId(RazorpaySecretId, RazorpaySecretKey, input);
                                if (string.IsNullOrEmpty(orderId))
                                {
                                    objresponse.response_code = "112";
                                    objresponse.response_message = "Failed At Razorpay End to create Order Id";
                                    return objresponse;
                                }
                                Request.order_id = orderId;
                                objInserttxnResponse.payment_url = Configuration["RazorpayValues:RazorPayAdminDomain"] + Configuration["RazorpayValues:PaymentURL"];

                            }
                            catch (WebException webEx)
                            {
                                try
                                {
                                    WebResponse response = webEx.Response;
                                    Stream stream = response.GetResponseStream();
                                    String responseMessage = new StreamReader(stream).ReadToEnd();
                                    objresponse.response_code = "111";
                                    objresponse.response_message = responseMessage;
                                    return objresponse;
                                }
                                catch (Exception ex)
                                {
                                    objresponse.response_code = "111";
                                    objresponse.response_message = "Failed At Razorpay End to create Order Id";
                                    return objresponse;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            objresponse.response_code = "111";
                            objresponse.response_message = "Failed At Razorpay End to create Order Id";
                            return objresponse;
                        }
                    }

                }


                if (objInserttxnResponse.payment_url != null)
                {
                    Request.agent_ref_id = Request.agent_ref_id;
                    Request.transfer_amount = Request.transfer_amount;
                    Request.channel_type_ref_id = Request.channel_type_ref_id;
                    Request.account_type_ref_id = Request.account_type_ref_id;
                    Request.agent_mobile_number = Request.agent_mobile_number;
                    Request.agent_name = Request.agent_name;
                    Request.transfer_amount = Request.transfer_amount;


                    DataSet Insert_dst = _pgRepo.APTPaymentGatewayInsertTraction(Request);
                    if (_commonService.IsValidDataSet(Insert_dst))
                    {
                        if (Insert_dst.Tables[0].Rows[0][0].ToString() == "100")
                        {
                            objInserttxnResponse.cp_transaction_id = Insert_dst.Tables[0].Rows[0][2].ToString();
                            objInserttxnResponse.payment_url = objInserttxnResponse.payment_url + objInserttxnResponse.cp_transaction_id;
                            objresponse.response_code = APIResponseCode.Success;
                            objresponse.response_message = APIResponseCodeDesc.Success;
                            objresponse.data = objInserttxnResponse;
                            return objresponse;
                        }
                        else
                        {
                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Issue in Inserting Customer to DB", JsonConvert.SerializeObject(Request), Request.agent_mobile_number);

                            objresponse.response_code = Insert_dst.Tables[0].Rows[0][0].ToString();
                            objresponse.response_message = Insert_dst.Tables[0].Rows[0][1].ToString();
                            objresponse.data = null;
                        }
                    }
                    else
                    {
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Not a Valid Dataset", JsonConvert.SerializeObject(Request), Request.agent_mobile_number);

                        objresponse.response_code = APIResponseCode.DatasetNull;
                        objresponse.response_message = APIResponseCodeDesc.DatasetNull;
                        objresponse.data = null;
                    }
                }
                else
                {
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "APTCreatePaymentGatewayOrder Failed Response", JsonConvert.SerializeObject(objAPIResponse), Request.agent_mobile_number);

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
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.agent_mobile_number);

            }

            return objresponse;
        }

        public APTGenericResponse? APTUpdatePaymentGatewayOrder(UpdatePGTransactionRequest Request)
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", "", Request.vendor_payment_id);

            APTGenericResponse objresponse = new APTGenericResponse();
            CashFreeOrderDetailsResponse objAPIResponse = new CashFreeOrderDetailsResponse();
            UpdatePGTransactionRequest updateTxnReq = new UpdatePGTransactionRequest();
            try
            {


                objAPIResponse = _cashFreeAPI.GetOrderDetails(Request.order_id);
                if (objAPIResponse != null)
                {
                    List<PaymentDetailsResponse> objPayResponse = new List<PaymentDetailsResponse>();
                    objPayResponse = _cashFreeAPI.GetPaymentDetails(Request.order_id);
                    for (int i = 0; i < objPayResponse.Count; i++)
                    {
                        string status = objPayResponse[i].payment_status;
                        if (status == "SUCCESS")
                        {
                            Request.payment_type = objPayResponse[i].payment_group;
                            if (Request.payment_type.Contains("card"))
                            {

                                Request.card_brand = objPayResponse[i].payment_method.card.card_network;
                                Request.card_sub_type = objPayResponse[i].payment_method.card.card_type;
                            }
                            break;
                        }
                    }


                    Request.vendor_payment_id = objAPIResponse.cf_order_id.ToString();
                    Request.transfer_amount = objAPIResponse.order_amount.ToString();

                    //Request.payment_type = objAPIResponse.order_meta.payment_methods;


                    if (objAPIResponse.order_status == "PAID")
                    {
                        Request.status = "2";
                        Request.status_desc = objAPIResponse.order_status;
                        objresponse.response_code = APIResponseCode.Success;
                        objresponse.response_message = "Transaction Successful";
                        objresponse.data = Request.vendor_payment_id;
                        return objresponse;
                    }
                    else
                    {
                        Request.status = "1";
                        Request.status_desc = objAPIResponse.order_status;
                    }
                    DataSet updateTxn = _pgRepo.APTPaymentGatewayUpdateTraction(Request);
                    if (_commonService.IsValidDataSet(updateTxn))
                    {
                        if (updateTxn.Tables[0].Rows[0][0].ToString() == "100")
                        {
                            if (objAPIResponse.order_status == "PAID")
                            {
                                objresponse.response_code = APIResponseCode.Success;
                                objresponse.response_message = "Transaction Successful";
                                objresponse.data = Request.vendor_payment_id;
                                if (Request.payment_type == "upi")
                                {
                                    Request.topup_mode_ref_id = "20";
                                }

                                else if (Request.payment_type == "credit_card")
                                {
                                    Request.topup_mode_ref_id = "15";
                                }
                                else if (Request.payment_type == "debit_card")
                                {
                                    Request.topup_mode_ref_id = "16";
                                }
                                else if (Request.payment_type == "netbanking")
                                {
                                    Request.topup_mode_ref_id = "23";
                                }
                                Request.bank_ref_id = "7";
                                DataSet topup_dst = _pgRepo.APTPaymentGatewayTopup(Request);
                                return objresponse;
                            }
                            else
                            {
                                objresponse.response_code = APIResponseCode.Success;
                                objresponse.response_message = objAPIResponse.order_status;
                                objresponse.data = Request.vendor_payment_id;
                                return objresponse;
                            }
                        }
                        else
                        {
                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Issue in Inserting Customer to DB", JsonConvert.SerializeObject(Request), Request.agent_mobile_number);

                            objresponse.response_code = updateTxn.Tables[0].Rows[0][0].ToString();
                            objresponse.response_message = updateTxn.Tables[0].Rows[0][1].ToString();
                            objresponse.data = null;
                        }
                    }
                    else
                    {
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Not a API Response", JsonConvert.SerializeObject(Request), Request.agent_mobile_number);

                        objresponse.response_code = APIResponseCode.DatasetNull;
                        objresponse.response_message = APIResponseCodeDesc.DatasetNull;
                        objresponse.data = null;
                    }
                }
                else
                {
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Prevalidate Failed Response", JsonConvert.SerializeObject(objAPIResponse), Request.agent_mobile_number);

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
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.agent_mobile_number);

            }

            return objresponse;
        }

        public APTGenericResponse? APTWebhookUpdate(WebhookReqCashFree objRequest, List<string> lstDetails,string rawJson)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            UpdatePGTransactionRequest Request = new UpdatePGTransactionRequest();
            try
            {

                if (lstDetails != null && lstDetails.Count > 1)
                {
                    bool IsSignatureVerified = CheckSignature(rawJson, lstDetails);
                    if (IsSignatureVerified)
                    {
                        DataSet Dst = _pgRepo.GetPGTransactionDetails(objRequest.data.order.order_id);
                        if (Dst != null && Dst.Tables.Count > 0 && Dst.Tables[0].Rows.Count > 0)
                        {
                            Request.cp_transaction_id = Dst.Tables[0].Rows[0]["CPTransID"].ToString();
                            Request.agent_mobile_number = Dst.Tables[0].Rows[0]["MerchantMobileno"].ToString();
                            Request.agent_name = Dst.Tables[0].Rows[0]["MerchantName"].ToString();
                            Request.agent_ref_id = Dst.Tables[0].Rows[0]["AgentRefID"].ToString();
                            Request.order_id = objRequest.data.order.order_id;
                        }

                        if (objRequest.data.payment.payment_status == "SUCCESS")
                        {
                            Request.status = "2";
                            Request.status_desc = objRequest.data.payment.payment_status;
                        }
                        else if (objRequest.data.payment.payment_status == "FAILED")
                        {
                            Request.status = "3";
                            Request.status_desc = objRequest.data.payment.payment_status;
                        }
                        else
                        {
                            Request.status = "1";
                            Request.status_desc = objRequest.data.payment.payment_status;
                        }
                        Request.vendor_ref_no = objRequest.data.payment.bank_reference;
                        Request.vendor_payment_id = objRequest.data.payment.cf_payment_id.ToString();
                        Request.payment_type = objRequest.data.payment.payment_group;
                        Request.webhookrespone = JsonConvert.SerializeObject(objRequest);
                        DataSet updateTxn = _pgRepo.APTPaymentGatewayUpdateTraction(Request);
                        if (_commonService.IsValidDataSet(updateTxn))
                        {
                            if (updateTxn.Tables[0].Rows[0][0].ToString() == "100")
                            {
                                if (objRequest.data.payment.payment_status == "SUCCESS")
                                {
                                    Request.transfer_amount = objRequest.data.payment.payment_amount.ToString();
                                    if (objRequest.data.payment.payment_group == "upi")
                                    {
                                        Request.topup_mode_ref_id = "20";
                                    }
                                    else if (objRequest.data.payment.payment_group == "credit_card")
                                    {
                                        Request.topup_mode_ref_id = "15";
                                        if (objRequest.data.payment.payment_method.card.card_network.ToUpper() == "VISA")
                                        {
                                            Request.card_brand = "2";
                                        }
                                        else if (objRequest.data.payment.payment_method.card.card_network.ToUpper() == "MASTERCARD")
                                        {
                                            Request.card_brand = "1";
                                        }
                                        else if (objRequest.data.payment.payment_method.card.card_network.ToUpper() == "RUPAY")
                                        {
                                            Request.card_brand = "3";
                                        }
                                        else if (objRequest.data.payment.payment_method.card.card_network.ToUpper() == "AMERICAN EXPRESS" || objRequest.data.payment.payment_method.card.card_network.ToUpper() == "AMEX")
                                        {
                                            Request.card_brand = "4";
                                        }
                                        else if (objRequest.data.payment.payment_method.card.card_network.ToUpper() == "UPI")
                                        {
                                            Request.card_brand = "5";
                                        }
                                        else if (objRequest.data.payment.payment_method.card.card_network.ToUpper() == "PAYLATER")
                                        {
                                            Request.card_brand = "6";
                                        }
                                        else if (objRequest.data.payment.payment_method.card.card_network.ToUpper() == "WALLET")
                                        {
                                            Request.card_brand = "7";
                                        }
                                        if (objRequest.data.payment.payment_method.card.card_sub_type != null)
                                        {
                                            if (objRequest.data.payment.payment_method.card.card_sub_type.ToUpper() == "R")
                                            {
                                                Request.card_sub_type = "1";
                                            }
                                            else if (objRequest.data.payment.payment_method.card.card_sub_type.ToUpper() == "P")
                                            {
                                                Request.card_sub_type = "1";
                                            }
                                            else
                                            {
                                                Request.card_sub_type = "3";
                                            }
                                        }
                                        else
                                        {
                                            Request.card_sub_type = "3";
                                        }


                                    }
                                    else if (objRequest.data.payment.payment_group == "debit_card")
                                    {
                                        Request.topup_mode_ref_id = "16";
                                    }
                                    else if (objRequest.data.payment.payment_group == "netbanking")
                                    {
                                        Request.topup_mode_ref_id = "23";
                                    }
                                    Request.bank_ref_id = "7";
                                    DataSet topup_dst = _pgRepo.APTPaymentGatewayTopup(Request);
                                    if (_commonService.IsValidDataSet(topup_dst))
                                    {
                                        if (topup_dst.Tables[0].Rows[0][0].ToString() == "100")
                                        {
                                            objresponse.response_code = APIResponseCode.Success;
                                            objresponse.response_message = "Transaction Successful";
                                            objresponse.data = Request.vendor_payment_id;
                                            return objresponse;
                                        }
                                        else
                                        {
                                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Not a API Response - topup fail dst", JsonConvert.SerializeObject(Request), Request.agent_mobile_number);
                                            objresponse.response_code = topup_dst.Tables[0].Rows[0][0].ToString();
                                            objresponse.response_message = topup_dst.Tables[0].Rows[0][1].ToString();
                                            objresponse.data = null;
                                            return objresponse;
                                        }
                                    }
                                    else
                                    {
                                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Not a API Response - topup fail", JsonConvert.SerializeObject(Request), Request.agent_mobile_number);

                                        objresponse.response_code = APIResponseCode.DatasetNull;
                                        objresponse.response_message = APIResponseCodeDesc.DatasetNull;
                                        objresponse.data = null;
                                    }
                                    return objresponse;
                                }
                                else
                                {
                                    objresponse.response_code = APIResponseCode.Success;
                                    objresponse.response_message = objRequest.data.payment.payment_status;
                                    objresponse.data = Request.vendor_payment_id;
                                    return objresponse;
                                }
                            }
                            else
                            {
                                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Issue in Inserting Customer to DB", JsonConvert.SerializeObject(Request), Request.agent_mobile_number);

                                objresponse.response_code = updateTxn.Tables[0].Rows[0][0].ToString();
                                objresponse.response_message = updateTxn.Tables[0].Rows[0][1].ToString();
                                objresponse.data = null;
                                return objresponse;
                            }
                        }
                        else
                        {
                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Not a API Response", JsonConvert.SerializeObject(Request), Request.agent_mobile_number);

                            objresponse.response_code = APIResponseCode.DatasetNull;
                            objresponse.response_message = APIResponseCodeDesc.DatasetNull;
                            objresponse.data = null;
                            return objresponse;
                        }
                    }
                    else
                    {
                        objresponse.response_code = "401";
                        objresponse.response_message = "Signature Verification Failed";
                        objresponse.data = null;
                        return objresponse;
                    }
                }
                else
                {
                    objresponse.response_code = "401";
                    objresponse.response_message = "Signature Details Not Present";
                    objresponse.data = null;
                    return objresponse;
                }
            }
            catch (Exception ex)
            {
                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Webhook Ex - " + ex.StackTrace.ToString(), JsonConvert.SerializeObject(ex), Request.agent_mobile_number);
                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                return objresponse;
            }

        }
        public int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }



        


        public bool CheckSignature(string objRequest, List<string> lstDetails)
        {

            string timestamp = lstDetails[0]; 
            string payload = objRequest;
            

            string signatureData = string.Concat(timestamp, payload);
            byte[] message = Encoding.UTF8.GetBytes(signatureData);
            byte[] secretkey = Encoding.UTF8.GetBytes(SecretKey); //Get Secret_Key from Cashfree Merchant Dashboard

            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "signatureData", signatureData, "");

            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "secretkey", SecretKey, "");
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Header Signature", lstDetails[1], "");

            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Header Timestamp", lstDetails[0], "");
            using (HMACSHA256 hmac = new HMACSHA256(secretkey))
            {

                byte[] signatureBytes = hmac.ComputeHash(message);
                string computedSignature = Convert.ToBase64String(signatureBytes);
                //return computedSignature; //compare with "x-webhook-signature"

                if (computedSignature == lstDetails[1])
                {
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "CheckSignature Success", "Signature : " + lstDetails[1] + " | EncryptedSignature : " + computedSignature, "");

                    return true;
                }
                else
                {
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "CheckSignature Failure", "Signature : " + lstDetails[1] + " | EncryptedSignature : " + computedSignature, "");

                    return true;
                }


            }


          

        }

        public string Encrypt(string data)
        {
            string encData = null;
            byte[][] keys = GetHashKeys(SecretKey);

            try
            {
                encData = EncryptStringToBytes_Aes(data, keys[0], keys[1]);
            }
            catch (CryptographicException) { }
            catch (ArgumentNullException) { }

            return encData;
        }

        private byte[][] GetHashKeys(string key)
        {
            byte[][] result = new byte[2][];
            Encoding enc = Encoding.UTF8;

            SHA256 sha2 = new SHA256CryptoServiceProvider();

            byte[] rawKey = enc.GetBytes(key);
            byte[] rawIV = enc.GetBytes(key);

            byte[] hashKey = sha2.ComputeHash(rawKey);
            byte[] hashIV = sha2.ComputeHash(rawIV);

            Array.Resize(ref hashIV, 16);

            result[0] = hashKey;
            result[1] = hashIV;

            return result;
        }

        private static string EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            byte[] encrypted;

            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt =
                            new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(encrypted);
        }


        public APTGenericResponse? APTGetTransactionDetails(GetTransactionDetails Request)
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.transaction_id);

            APTGenericResponse objresponse = new APTGenericResponse();
            try
            {

                DataSet dst = _pgRepo.GetPGTransactionDetailsByTransID(Request.transaction_id);
                if (!_commonService.IsValidDataSet(dst))
                {
                    objresponse.response_code = APIResponseCode.DatasetNull;
                    objresponse.response_message = APIResponseCodeDesc.DatasetNull;
                    objresponse.data = null;
                }
                else
                {
                    objresponse.response_code = APIResponseCode.Success;
                    objresponse.response_message = APIResponseCodeDesc.Success;
                    objresponse.data = JsonConvert.SerializeObject(dst);
                    TransactionDatasetResponse myDeserializedClass = JsonConvert.DeserializeObject<TransactionDatasetResponse>(JsonConvert.SerializeObject(dst));

                    objresponse.data = myDeserializedClass;
                }
            }

            catch (Exception ex)
            {
                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.transaction_id);

            }

            return objresponse;
        }

        public string RazorPayGenerateOrderId(string RazorPayKey, string RazorPaySecret, Dictionary<string, object> input)
        {
            string orderId = "";
            try
            {
                //NLogWrapper.LogAPIData("Controller", "RazorPay", "RazorPayGenerateOrderId", "RazorPayKey:" + RazorPayKey, "", input.ToString() + "Pnb_Search_ID:" + input["receipt"].ToString());
                try
                {
                    string key = RazorPayKey;
                    string secret = RazorPaySecret;
                    RazorpayClient client = new RazorpayClient(key, secret);
                    Razorpay.Api.Order order = client.Order.Create(input);
                    orderId = order["id"].ToString();
                    //NLogWrapper.LogAPIData("Controller", "RazorPay", "RazorPayGenerateOrderId", "orderId:" + orderId, "", "Pnb_Search_ID:" + input["receipt"].ToString());
                    return orderId;
                }
                catch (WebException webEx)
                {
                    try
                    {
                        //get the response stream
                        WebResponse response = webEx.Response;
                        Stream stream = response.GetResponseStream();
                        String responseMessage = new StreamReader(stream).ReadToEnd();
                        //NLogWrapper.LogError("Controller", "RazorPay", "RazorPayGenerateOrderId - Error1", responseMessage, webEx.StackTrace, "Pnb_Search_ID:" + input["receipt"].ToString());
                        return orderId;
                    }
                    catch (Exception ex)
                    {
                        //NLogWrapper.LogError("Controller", "RazorPay", "RazorPayGenerateOrderId - Error2", ex.Message, ex.StackTrace, "Pnb_Search_ID:" + input["receipt"].ToString());
                        return orderId;
                    }
                }
            }
            catch (Exception ex)
            {
                // NLogWrapper.LogError("Controller", "RazorPay", "RazorPayGenerateOrderId - Error3", ex.Message, ex.StackTrace, "Pnb_Search_ID:" + input["receipt"].ToString());
                return orderId;
            }
        }

        public APTGenericResponse? APTUpdatePaymentGatewayOrderRazorpay(UpdatePGTransactionRequest Request)
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", "", Request.order_id);

            APTGenericResponse objresponse = new APTGenericResponse();
            UpdatePGTransactionRequest updateTxnReq = new UpdatePGTransactionRequest();
            try
            {

                string payment_id = Request.vendor_payment_id;//"pay_LtdwQ6TEk222Hg";

                string key = RazorpaySecretKey;  // key
                string secret = RazorpaySecretId; //secret id
                RazorpayClient client = new RazorpayClient(secret, key);
                Razorpay.Api.Payment payment = client.Payment.Fetch(payment_id);
                string obj = JsonConvert.SerializeObject(payment);

                PaymentDetails objPayResponse = JsonConvert.DeserializeObject<PaymentDetails>(obj);
                if (objPayResponse != null)
                {

                    Request.payment_type = objPayResponse.Attributes.method;
                    if (Request.payment_type.Contains("card"))
                    {
                        if (objPayResponse.Attributes.card.type.Contains("credit"))
                        {
                            Request.payment_type = "credit_card";
                        }
                        if (objPayResponse.Attributes.card != null)
                        {
                            Request.card_brand = objPayResponse.Attributes.card.network;
                            Request.card_sub_type = objPayResponse.Attributes.card.sub_type;
                        }
                    }


                }

                Request.vendor_payment_id = objPayResponse.Attributes.id.ToString();
                Request.transfer_amount = objPayResponse.Attributes.amount.ToString();



                DataSet Dst = _pgRepo.GetPGTransactionDetails(Request.order_id);
                if (Dst != null && Dst.Tables.Count > 0 && Dst.Tables[0].Rows.Count > 0)
                {
                    Request.cp_transaction_id = Dst.Tables[0].Rows[0]["CPTransID"].ToString();
                    Request.agent_mobile_number = Dst.Tables[0].Rows[0]["MerchantMobileno"].ToString();
                    Request.agent_name = Dst.Tables[0].Rows[0]["MerchantName"].ToString();
                    Request.agent_ref_id = Dst.Tables[0].Rows[0]["AgentRefID"].ToString();
                }

                if (objPayResponse.Attributes.status.ToUpper() == "CAPTURED")
                {
                    Request.status = "2";
                    Request.status_desc = objPayResponse.Attributes.status.ToUpper();
                }
                else if (objPayResponse.Attributes.status.ToUpper() == "FAILED")
                {
                    Request.status = "3";
                    Request.status_desc = objPayResponse.Attributes.status.ToUpper();
                }
                else
                {
                    Request.status = "1";
                    Request.status_desc = objPayResponse.Attributes.status.ToUpper();
                }
                Request.vendor_ref_no = objPayResponse.Attributes.card_id;


                Request.webhookrespone = JsonConvert.SerializeObject(objPayResponse);
                DataSet updateTxn = _pgRepo.APTPaymentGatewayUpdateTraction(Request);
                if (_commonService.IsValidDataSet(updateTxn))
                {
                    if (updateTxn.Tables[0].Rows[0][0].ToString() == "100")
                    {
                        if (objPayResponse.Attributes.status.ToUpper() == "CAPTURED")
                        {
                            if (Request.payment_type == "upi")
                            {
                                Request.topup_mode_ref_id = "20";
                            }
                            else if (Request.payment_type == "credit_card")
                            {
                                Request.topup_mode_ref_id = "15";
                                if (Request.card_brand.ToUpper() == "VISA")
                                {
                                    Request.card_brand = "2";
                                }
                                else if (Request.card_brand.ToUpper() == "MASTERCARD")
                                {
                                    Request.card_brand = "1";
                                }
                                else if (Request.card_brand.ToUpper() == "RUPAY")
                                {
                                    Request.card_brand = "3";
                                }
                                else if (Request.card_brand.ToUpper() == "AMERICAN EXPRESS")
                                {
                                    Request.card_brand = "4";
                                }
                                else if (Request.card_brand.ToUpper() == "UPI")
                                {
                                    Request.card_brand = "5";
                                }
                                else if (Request.card_brand.ToUpper() == "PAYLATER")
                                {
                                    Request.card_brand = "6";
                                }
                                else if (Request.card_brand.ToUpper() == "WALLET")
                                {
                                    Request.card_brand = "7";
                                }
                                if (Request.card_sub_type != null)
                                {
                                    if (Request.card_sub_type.ToUpper() == "CONSUMER")
                                    {
                                        Request.card_sub_type = "1";
                                    }
                                    else if (Request.card_sub_type.ToUpper() == "PREMIUM")
                                    {
                                        Request.card_sub_type = "1";
                                    }
                                    else
                                    {
                                        Request.card_sub_type = "3";
                                    }
                                }
                                else
                                {
                                    Request.card_sub_type = "3";
                                }


                            }
                            else if (Request.payment_type == "debit_card")
                            {
                                Request.topup_mode_ref_id = "16";
                            }
                            else if (Request.payment_type == "netbanking")
                            {
                                Request.topup_mode_ref_id = "23";
                            }
                            Request.bank_ref_id = "2";
                            DataSet topup_dst = _pgRepo.APTPaymentGatewayTopup(Request);
                            if (_commonService.IsValidDataSet(topup_dst))
                            {
                                if (topup_dst.Tables[0].Rows[0][0].ToString() == "100")
                                {
                                    objresponse.response_code = APIResponseCode.Success;
                                    objresponse.response_message = "Transaction Successful";
                                    objresponse.data = Request.vendor_payment_id;
                                    return objresponse;
                                }
                                else
                                {
                                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Not a API Response - topup fail dst", JsonConvert.SerializeObject(Request), Request.agent_mobile_number);
                                    objresponse.response_code = topup_dst.Tables[0].Rows[0][0].ToString();
                                    objresponse.response_message = topup_dst.Tables[0].Rows[0][1].ToString();
                                    objresponse.data = null;
                                    return objresponse;
                                }
                            }
                            else
                            {
                                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Not a API Response - topup fail", JsonConvert.SerializeObject(Request), Request.agent_mobile_number);

                                objresponse.response_code = APIResponseCode.DatasetNull;
                                objresponse.response_message = APIResponseCodeDesc.DatasetNull;
                                objresponse.data = null;
                            }
                            return objresponse;
                        }
                        else
                        {
                            objresponse.response_code = APIResponseCode.Failed;
                            objresponse.response_message = objPayResponse.Attributes.status.ToUpper();
                            objresponse.data = Request.vendor_payment_id;
                            return objresponse;
                        }
                    }
                    else
                    {
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Issue in Inserting Customer to DB", JsonConvert.SerializeObject(Request), Request.agent_mobile_number);

                        objresponse.response_code = updateTxn.Tables[0].Rows[0][0].ToString();
                        objresponse.response_message = updateTxn.Tables[0].Rows[0][1].ToString();
                        objresponse.data = null;
                        return objresponse;
                    }
                }
                else
                {
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Not a API Response", JsonConvert.SerializeObject(Request), Request.agent_mobile_number);

                    objresponse.response_code = APIResponseCode.DatasetNull;
                    objresponse.response_message = APIResponseCodeDesc.DatasetNull;
                    objresponse.data = null;
                    return objresponse;
                }



            }

            catch (Exception ex)
            {
                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.agent_mobile_number);

            }

            return objresponse;
        }

        public APTGenericResponse? PGCheckStatus(RazorpayServiceRequest Request)
        {
            APTGenericResponse Response = new APTGenericResponse();
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", "", Request.OrderId);
            try
            {
                string RazorpayURL = "https://api.razorpay.com/v1/orders/" + Request.OrderId+ "/payments";
                string response = RazorpayRequerryAPICall(RazorpayURL);
                if(response != "error")
                {
                    GetRPPaymentByOrderResponse RazorpayResponse = new GetRPPaymentByOrderResponse();
                    
                    RazorpayResponse = JsonConvert.DeserializeObject<GetRPPaymentByOrderResponse>(response);
                    if(RazorpayResponse.count > 0)
                    {
                       
                        for (int i=0;i<RazorpayResponse.count;i++)
                        {
                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "RPGotService", JsonConvert.SerializeObject(RazorpayResponse), Request.OrderId);
                            if (RazorpayResponse.items[i].captured)
                            {
                                string paymentid = RazorpayResponse.items[i].id;
                                UpdatePGTransactionRequest updateRequest = new UpdatePGTransactionRequest();
                                updateRequest.order_id = RazorpayResponse.items[i].order_id;
                                updateRequest.vendor_payment_id = paymentid;
                                APTUpdatePaymentGatewayOrderRazorpay(updateRequest);
                                Response.response_code = "200";
                                Response.response_message = "Updated Successfully";
                                Response.data = RazorpayResponse;
                                break;
                            }
                        }
                    }
                    else
                    {

                    }

                }
                else
                {

                }
                
            }
            catch (Exception ex)
            {
                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Exception", JsonConvert.SerializeObject(ex), Request.OrderId);
            }
            return Response;
        }

        public string RazorpayRequerryAPICall(string URL)
        {
            try
            {
                var plainTextBytes = Encoding.UTF8.GetBytes($"{RazorpaySecretId}:{RazorpaySecretKey}");
                string val = Convert.ToBase64String(plainTextBytes);
                HttpWebRequest webReq = WebRequest.Create(URL) as HttpWebRequest;
                webReq.Method = "GET";
                string Authorization = "Basic cnpwX2xpdmVfT3gyVllJdG9Gd3hicU46anJ4N2dMdjRPYkRuMzkxWHRvYjU3bmVx";//"Basic " + val;
                webReq.Headers.Add("Authorization", Authorization);
                webReq.Timeout = 1800000;

                HttpWebResponse response;
                response = (HttpWebResponse)webReq.GetResponse();
                Stream responseStream = response.GetResponseStream();
                string responsestring = new StreamReader(responseStream).ReadToEnd();
                return responsestring;
                
            }
            catch (Exception ex)
            {
                //Utilities.LogAPIData("Windowservice", "RazorpayRequery", "Razorpay_RequerryAPICall_Exception", "", ex.Message + "----" + ex.StackTrace, "");
                return "error";
            }

        }

        public APTGenericResponse? APTCreatePaymentGatewayOrder_CustPayout(InsertPGTransactionRequest Request)
        {
            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", JsonConvert.SerializeObject(Request), Request.customer_phone);

            APTGenericResponse objresponse = new APTGenericResponse();
            CashFreeOrderDetailsResponse objAPIResponse = new CashFreeOrderDetailsResponse();
            InsertPGTransactionResponse objInserttxnResponse = new InsertPGTransactionResponse();
            try
            {
                DataSet dst = _pgRepo.PGDecider(Request.agent_mobile_number, Request.pg_service_id.ToString());
                if (dst != null && dst.Tables.Count > 0 && dst.Tables[0].Rows != null)
                {
                    if (dst.Tables[0].Rows[0][0].ToString() == "7")
                    {
                        CashFreePGCreateOrderRequest objRequest = new CashFreePGCreateOrderRequest();
                        objRequest.customer_details = new CustomerDetails();
                        objRequest.customer_details.customer_phone = Request.customer_phone;
                        objRequest.customer_details.customer_id = Request.customer_phone;
                        objRequest.order_meta = new OrderMeta();
                        objRequest.order_meta.return_url = Configuration["CashFreePGValues:CashFreeAdminDomain"] + Configuration["CashFreePGValues:returnURL"];
                        objRequest.order_amount = Request.transfer_amount;
                        objRequest.order_currency = "INR";
                        objRequest.order_id = "CF" + DateTime.UtcNow.Date.ToString("ddMMyyyy") + GenerateRandomNo().ToString();

                        objAPIResponse = _cashFreeAPI.CreateOrder(objRequest);
                        if (objAPIResponse != null)
                        {
                            Request.order_id = objAPIResponse.order_id;
                            Request.payment_session_id = objAPIResponse.payment_session_id;
                        }
                        objInserttxnResponse.payment_url = Configuration["CashFreePGValues:CashFreeAdminDomain"] + Configuration["CashFreePGValues:PaymentURL"];


                    }
                    else if (dst.Tables[0].Rows[0][0].ToString() == "2")
                    {
                        // Converting amount to paise for razorpay eg rs 1 is 100 paise  --start
                        Int64 Amt;
                        decimal RPAmount = Convert.ToDecimal(Request.transfer_amount);
                        string ActulAmount = string.Format("{0:0.00}", RPAmount);
                        var my = string.Format("{0:0.00}", RPAmount);
                        var strArr = my.ToString().Split('.').ToArray();
                        string finalString = strArr[0].PadLeft(3, '0') + strArr[1];

                        RPAmount = Convert.ToDecimal(finalString);
                        Amt = Convert.ToInt64(RPAmount);
                        // Converting amount to paise for razorpay eg rs 1 is 100 paise  --start

                        string searchref = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // TLS 1.2
                        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
                        Dictionary<string, object> input = new Dictionary<string, object>();
                        input.Add("amount", Amt); // this amount should be same as transaction amount
                        input.Add("currency", "INR");
                        input.Add("receipt", searchref);
                        input.Add("payment_capture", 1);
                        try
                        {
                            try
                            {
                                string orderId = RazorPayGenerateOrderId(RazorpaySecretId, RazorpaySecretKey, input);
                                if (string.IsNullOrEmpty(orderId))
                                {
                                    objresponse.response_code = "112";
                                    objresponse.response_message = "Failed At Razorpay End to create Order Id";
                                    return objresponse;
                                }
                                Request.order_id = orderId;
                                objInserttxnResponse.payment_url = Configuration["RazorpayValues:RazorPayAdminDomain"] + Configuration["RazorpayValues:PaymentURL"];

                            }
                            catch (WebException webEx)
                            {
                                try
                                {
                                    WebResponse response = webEx.Response;
                                    Stream stream = response.GetResponseStream();
                                    String responseMessage = new StreamReader(stream).ReadToEnd();
                                    objresponse.response_code = "111";
                                    objresponse.response_message = responseMessage;
                                    return objresponse;
                                }
                                catch (Exception ex)
                                {
                                    objresponse.response_code = "111";
                                    objresponse.response_message = "Failed At Razorpay End to create Order Id";
                                    return objresponse;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            objresponse.response_code = "111";
                            objresponse.response_message = "Failed At Razorpay End to create Order Id";
                            return objresponse;
                        }
                    }

                }


                if (objInserttxnResponse.payment_url != null)
                {
                    Request.agent_ref_id = Request.agent_ref_id;
                    Request.transfer_amount = Request.transfer_amount;
                    Request.channel_type_ref_id = Request.channel_type_ref_id;
                    Request.account_type_ref_id = Request.account_type_ref_id;
                    Request.agent_mobile_number = Request.agent_mobile_number;
                    Request.agent_name = Request.agent_name;
                    Request.transfer_amount = Request.transfer_amount;


                    DataSet Insert_dst = _pgRepo.APTPaymentGatewayInsertTraction(Request);
                    if (_commonService.IsValidDataSet(Insert_dst))
                    {
                        if (Insert_dst.Tables[0].Rows[0][0].ToString() == "100")
                        {
                            objInserttxnResponse.cp_transaction_id = Insert_dst.Tables[0].Rows[0][2].ToString();
                            objInserttxnResponse.order_id = Request.order_id;
                            objInserttxnResponse.vendor_session_id = Request.payment_session_id;
                            objInserttxnResponse.bank_id = dst.Tables[0].Rows[0][0].ToString();
                            objInserttxnResponse.payment_url = objInserttxnResponse.payment_url + objInserttxnResponse.cp_transaction_id;
                            objresponse.response_code = APIResponseCode.Success;
                            objresponse.response_message = APIResponseCodeDesc.Success;
                            objresponse.data = objInserttxnResponse;
                            return objresponse;
                        }
                        else
                        {
                            _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Issue in Inserting Customer to DB", JsonConvert.SerializeObject(Request), Request.agent_mobile_number);

                            objresponse.response_code = Insert_dst.Tables[0].Rows[0][0].ToString();
                            objresponse.response_message = Insert_dst.Tables[0].Rows[0][1].ToString();
                            objresponse.data = null;
                        }
                    }
                    else
                    {
                        _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Not a Valid Dataset", JsonConvert.SerializeObject(Request), Request.agent_mobile_number);

                        objresponse.response_code = APIResponseCode.DatasetNull;
                        objresponse.response_message = APIResponseCodeDesc.DatasetNull;
                        objresponse.data = null;
                    }
                }
                else
                {
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "APTCreatePaymentGatewayOrder Failed Response", JsonConvert.SerializeObject(objAPIResponse), Request.agent_mobile_number);

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
                _logService.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.agent_mobile_number);

            }

            return objresponse;
        }

    }
}
