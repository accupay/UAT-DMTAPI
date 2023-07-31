using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using APT.DMT.API.BusinessObjects.Models;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using System.Reflection;

namespace APT.DMT.API.BusinessLogic.BankService
{
    public class AxisPayOut
    {
        public readonly string AESKEY;
        public readonly string serviceRequestId;
        private readonly string AXISFundTransfer;
        private readonly string CertName;
        private readonly string CertPWD;
        private readonly string channelId;
        private readonly string corpCode;
        private readonly string corpAccNum;
        private readonly string AXISCheckStatus;
        public static IConfiguration Configuration { get; set; }
        public static LogService _logService = new LogService();
        public AxisPayOut()
        {
            Configuration = GetConfigurations();
            AXISFundTransfer = Configuration["AxisValues:AXISFundTransfer"];
            CertName = Configuration["AxisValues:CertName"];
            CertPWD = Configuration["AxisValues:CertNamePWD"];
            AESKEY = Configuration["AxisValues:AESKey"];
            serviceRequestId = Configuration["AxisValues:serviceRequestId"];
            channelId = Configuration["AxisValues:channelId"];
            corpCode = Configuration["AxisValues:corpCode"];
            corpAccNum = Configuration["AxisValues:corpAccNum"];
            AXISCheckStatus = Configuration["AxisValues:AXISCheckStatus"];
        }
        public CombinedResponse FundTransfer(AxisPaymentRequest axisPaymentRequest)
        {

            FundTransferBankRequest objRequest = new FundTransferBankRequest();
            FundTransferEncRequest objBankEncReq = new FundTransferEncRequest();
            PaymentDetail paymentDetails = new PaymentDetail();
            CombinedResponse data = new CombinedResponse();

            string emptyData = string.Empty;
            string lstrCheckSum = string.Empty;
            try
            {
                string lstrUUID = axisPaymentRequest.transactionID;
                objRequest.TransferPaymentRequest.SubHeader.requestUUID = lstrUUID;
                objRequest.TransferPaymentRequest.SubHeader.serviceRequestId = serviceRequestId;
                objRequest.TransferPaymentRequest.SubHeader.serviceRequestVersion = "1.0";
                objRequest.TransferPaymentRequest.SubHeader.channelId = channelId;
                objRequest.TransferPaymentRequest.TransferPaymentRequestBody.channelId = channelId;
                objRequest.TransferPaymentRequest.TransferPaymentRequestBody.corpCode = corpCode;

                if (axisPaymentRequest.beneBankName.ToUpper() == "AXIS BANK")
                {
                    paymentDetails.txnPaymode = "FT"; // AXIS to AXIS
                }
                else
                {
                    if (axisPaymentRequest.txnPaymode == "1")
                    {
                        paymentDetails.txnPaymode = "PA"; //IMPS
                    }
                    else if (axisPaymentRequest.txnPaymode == "2")
                    {
                        paymentDetails.txnPaymode = "NE"; // NEFT
                    }
                    else if (axisPaymentRequest.txnPaymode == "3")
                    {
                        paymentDetails.txnPaymode = "RT"; // RTGS
                    }
                }
                paymentDetails.custUniqRef = axisPaymentRequest.transactionID;
                paymentDetails.corpAccNum = corpAccNum;
                paymentDetails.valueDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
                paymentDetails.txnAmount = axisPaymentRequest.txnAmount;
                paymentDetails.beneLEI = emptyData;
                paymentDetails.beneName = axisPaymentRequest.beneName;
                paymentDetails.beneCode = axisPaymentRequest.beneCode;
                paymentDetails.beneAccNum = axisPaymentRequest.beneAccNum;
                paymentDetails.beneAcType = emptyData;
                paymentDetails.beneAddr1 = emptyData;
                paymentDetails.beneAddr2 = emptyData;
                paymentDetails.beneAddr3 = emptyData;
                paymentDetails.beneCity = emptyData;
                paymentDetails.beneState = emptyData;
                paymentDetails.benePincode = emptyData;
                paymentDetails.beneIfscCode = axisPaymentRequest.beneIfscCode;
                paymentDetails.beneBankName = axisPaymentRequest.beneBankName;
                paymentDetails.baseCode = emptyData;
                paymentDetails.chequeNumber = emptyData;
                paymentDetails.chequeDate = emptyData;
                paymentDetails.payableLocation = emptyData;
                paymentDetails.printLocation = emptyData;
                paymentDetails.beneEmailAddr1 = emptyData;
                paymentDetails.beneMobileNo = emptyData;
                paymentDetails.productCode = emptyData;
                paymentDetails.txnType = emptyData;
                paymentDetails.invoiceDetails = new List<InvoiceDetail>
            {
                new InvoiceDetail
                {
                    invoiceAmount="0.00",
                    invoiceNumber=GetTransID(),
                    invoiceDate=DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    cashDiscount="0.00",
                    tax="0.00",
                    netAmount="0.00"
                }

            };
                paymentDetails.enrichment1 = emptyData;
                paymentDetails.enrichment2 = emptyData;
                paymentDetails.enrichment3 = emptyData;
                paymentDetails.enrichment4 = emptyData;
                paymentDetails.enrichment5 = emptyData;
                paymentDetails.senderToReceiverInfo = emptyData;
                objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails.Add(paymentDetails);

                if (1 == 0)
                {
                    lstrCheckSum = objRequest.TransferPaymentRequest.TransferPaymentRequestBody.channelId + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].txnPaymode + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].custUniqRef + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].corpAccNum + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].valueDate + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].txnAmount + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].beneLEI + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].beneName + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].beneCode +
                                        objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].beneAccNum + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].beneAcType + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].beneAddr1 + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].beneAddr2 + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].beneAddr3 + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].beneCity + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].beneState + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].benePincode + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].beneIfscCode + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].beneBankName +
                                        objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].baseCode + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].chequeNumber + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].chequeDate + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].payableLocation + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].printLocation + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].beneEmailAddr1 + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].beneMobileNo + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].productCode + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].txnType + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].invoiceDetails[0].invoiceAmount +
                                        objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].invoiceDetails[0].invoiceNumber + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].invoiceDetails[0].invoiceDate + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].invoiceDetails[0].cashDiscount + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].invoiceDetails[0].tax + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].invoiceDetails[0].netAmount + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].enrichment1 + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].enrichment2 + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].enrichment3 + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].enrichment4 +
                                        objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].enrichment5 + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].senderToReceiverInfo;

                }
                else
                {

                    lstrCheckSum = objRequest.TransferPaymentRequest.TransferPaymentRequestBody.channelId + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.corpCode + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].txnPaymode + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].custUniqRef + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].corpAccNum + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].valueDate + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].txnAmount + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].beneLEI + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].beneName + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].beneCode +
                                            objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].beneAccNum + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].beneAcType + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].beneAddr1 + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].beneAddr2 + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].beneAddr3 + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].beneCity + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].beneState + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].benePincode + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].beneIfscCode + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].beneBankName +
                                            objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].baseCode + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].chequeNumber + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].chequeDate + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].payableLocation + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].printLocation + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].beneEmailAddr1 + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].beneMobileNo + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].productCode + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].txnType + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].invoiceDetails[0].invoiceAmount +
                                            objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].invoiceDetails[0].invoiceNumber + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].invoiceDetails[0].invoiceDate + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].invoiceDetails[0].cashDiscount + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].invoiceDetails[0].tax + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].invoiceDetails[0].netAmount + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].enrichment1 + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].enrichment2 + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].enrichment3 + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].enrichment4 +
                                            objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].enrichment5 + objRequest.TransferPaymentRequest.TransferPaymentRequestBody.paymentDetails[0].senderToReceiverInfo;
                }


                objRequest.TransferPaymentRequest.TransferPaymentRequestBody.checksum = GetCheckSum(lstrCheckSum);
                string req = JsonConvert.SerializeObject(objRequest);
                string PlainReqBody = JsonConvert.SerializeObject(objRequest.TransferPaymentRequest.TransferPaymentRequestBody);
                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Axis Request", PlainReqBody, "Axis payout");
                string EncReqBody = AES_ENCRYPT(PlainReqBody, AESKEY);
                objBankEncReq.TransferPaymentRequest.SubHeader.requestUUID = lstrUUID;
                objBankEncReq.TransferPaymentRequest.SubHeader.serviceRequestId = serviceRequestId;
                objBankEncReq.TransferPaymentRequest.SubHeader.serviceRequestVersion = "1.0";
                objBankEncReq.TransferPaymentRequest.SubHeader.channelId = channelId;
                objBankEncReq.TransferPaymentRequest.TransferPaymentRequestBodyEncrypted = EncReqBody;
                string encBankReq = JsonConvert.SerializeObject(objBankEncReq);
                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Axis Request enc", encBankReq, "Axis payout");
                string lstrRet = POSTData(AXISFundTransfer, encBankReq, "application/json");

                if (lstrRet.IsNullOrEmpty())
                {
                    data.ApiRequest = PlainReqBody;
                    data.ApiResponse = "Failed";
                    return data;
                }
                AxisResponse objResponse = JsonConvert.DeserializeObject<AxisResponse>(lstrRet);
                string DecResp = AES_DECRYPT(objResponse.TransferPaymentResponse.TransferPaymentResponseBodyEncrypted.ToString(), AESKEY);

                data.ApiRequest = PlainReqBody;
                data.ApiResponse = DecResp;
                return data;


            }
            catch (System.Exception ex)
            {
                data.ApiRequest = "Error";
                data.ApiResponse = "Error";
                return data;
            }
        }
        public CombinedResponse CheckStatus(string transactionID)
        {
            string lstrReturn = string.Empty;
            string lstrRequest = string.Empty;
            SqlCommand objCMD = new SqlCommand();
            HttpResponseMessage Response = new HttpResponseMessage();
            Dictionary<string, string> objResp = new Dictionary<string, string>();
            CheckStatusReq objRequest = new CheckStatusReq();
            CombinedResponse data = new CombinedResponse();
            string lstrResponse = string.Empty;
            string lstrAPIKEY = string.Empty;
            string lstrCustomerID = string.Empty;
            string lstrConsumerSecret = string.Empty;
            string lstrAESKey = string.Empty;
            string lstrErrMsg = string.Empty;
            string lstrDecRequest = string.Empty;
            string empty = string.Empty;
            try
            {
                string lstrUUID = "PAY" + GetTransID();
                objRequest.GetStatusRequest.SubHeader.requestUUID = lstrUUID;
                objRequest.GetStatusRequest.SubHeader.serviceRequestId = serviceRequestId;
                objRequest.GetStatusRequest.SubHeader.serviceRequestVersion = "1.0";
                objRequest.GetStatusRequest.SubHeader.channelId = channelId;
                objRequest.GetStatusRequest.GetStatusRequestBody.channelId = channelId;
                objRequest.GetStatusRequest.GetStatusRequestBody.corpCode = corpCode;
                objRequest.GetStatusRequest.GetStatusRequestBody.crn = transactionID;
                string lstrCheckSum = objRequest.GetStatusRequest.GetStatusRequestBody.channelId + objRequest.GetStatusRequest.GetStatusRequestBody.corpCode + objRequest.GetStatusRequest.GetStatusRequestBody.crn;
                objRequest.GetStatusRequest.GetStatusRequestBody.checksum = GetCheckSum(lstrCheckSum);

                string req = JsonConvert.SerializeObject(objRequest);

                string PlainReqBody = JsonConvert.SerializeObject(objRequest.GetStatusRequest.GetStatusRequestBody);

                string EncReqBody = AES_ENCRYPT(PlainReqBody, AESKEY);

                CheckStatusReqEnc objStatusEncReq = new CheckStatusReqEnc();

                objStatusEncReq.GetStatusRequest.SubHeader.requestUUID = lstrUUID;
                objStatusEncReq.GetStatusRequest.SubHeader.serviceRequestId = "OpenAPI";
                objStatusEncReq.GetStatusRequest.SubHeader.serviceRequestVersion = "1.0";
                objStatusEncReq.GetStatusRequest.SubHeader.channelId = "ACCUPAYD";
                objStatusEncReq.GetStatusRequest.GetStatusRequestBodyEncrypted = EncReqBody;
                string encBankReq = JsonConvert.SerializeObject(objStatusEncReq);
                string lstrRet = POSTData(AXISCheckStatus, encBankReq, "application/json");
                if (lstrRet.IsNullOrEmpty())
                {
                    data.ApiRequest = PlainReqBody;
                    data.ApiResponse = "Failed";
                    return data;
                }
                AxisGetStatusResponse objResponse = JsonConvert.DeserializeObject<AxisGetStatusResponse>(lstrRet);
                string DecResp = AES_DECRYPT(objResponse.GetStatusResponse.GetStatusResponseBodyEncrypted.ToString(), AESKEY);

                data.ApiRequest = PlainReqBody;
                data.ApiResponse = DecResp;
                return data;

            }
            catch (System.Exception ex)
            {
                data.ApiRequest = "Error";
                data.ApiResponse = "Error";
                return data;
            }

        }
        public string GetCheckSum(string value)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(value));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
        }
        public string AES_ENCRYPT(string clearText, string EncryptionKey)
        {
            try
            {
                byte[] bIV = new byte[] { (byte)0x8E, 0x12, 0x39, (byte)0x9C, 0x07, 0x72, 0x6F, 0x5A, (byte)0x8E, 0x12, 0x39, (byte)0x9C, 0x07, 0x72, 0x6F, 0x5A };

                byte[] clearBytes = Encoding.ASCII.GetBytes(clearText);
                byte[] bENVVALUE;
                using (Aes encryptor = Aes.Create())
                {
                    byte[] bKey = HexStringToByte(EncryptionKey);//Encoding.ASCII.GetBytes(EncryptionKey);//

                    encryptor.Key = bKey;
                    encryptor.IV = bIV;
                    encryptor.Mode = CipherMode.CBC;
                    encryptor.Padding = PaddingMode.PKCS7;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        bENVVALUE = ms.ToArray();
                        byte[] bFinal = new byte[bIV.Length + bENVVALUE.Length];
                        Array.Copy(bIV, bFinal, bIV.Length);
                        Array.Copy(bENVVALUE, 0, bFinal, bIV.Length, bENVVALUE.Length);
                        clearText = Convert.ToBase64String(bFinal);//util.ByteArrayToHexString(bENVVALUE);   //Convert.ToBase64String(bENVVALUE);
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }
            finally
            {

            }
            return clearText;
        }
        public string AES_DECRYPT(string cipherText, string EncryptionKey)
        {
            try
            {
                byte[] bIV;
                byte[] bDECVALUE;
                byte[] cipherBytes = Convert.FromBase64String(cipherText); //util.HexStringToByte(cipherText);
                using (Aes encryptor = Aes.Create())
                {
                    bIV = new byte[16];
                    Array.Copy(cipherBytes, bIV, 16);

                    byte[] bData = new byte[cipherBytes.Length - 16];

                    Array.Copy(cipherBytes, 16, bData, 0, cipherBytes.Length - 16);

                    byte[] bKey = HexStringToByte(EncryptionKey);  //Encoding.ASCII.GetBytes(EncryptionKey);// 
                    encryptor.Key = bKey;
                    encryptor.IV = bIV;
                    encryptor.Mode = CipherMode.CBC;
                    encryptor.Padding = PaddingMode.PKCS7;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(bData, 0, bData.Length);
                            cs.Close();
                        }
                        bDECVALUE = ms.ToArray();
                        cipherText = Encoding.ASCII.GetString(ms.ToArray());
                    }
                }
                return cipherText;
            }
            catch (Exception)
            {
                return "";
            }
        }
        public static string ConvertToJulian()
        {
            DateTime firstJan = new DateTime(DateTime.Now.Year, 1, 1);
            string daysSinceFirstJan = Convert.ToString((DateTime.Now - firstJan).Days + 1);
            return DateTime.Now.Year.ToString().Substring(3, 1) + daysSinceFirstJan.PadLeft(3, '0') + DateTime.Now.ToString("HH");
        }
        public static bool IsNumeric(string input)
        {
            int test;
            return int.TryParse(input, out test);
        }
        public static string GetStan()
        {
            string lstrString = "";
            string lstrNumber = "";
            int lintCnt = 0;

            lstrString = Guid.NewGuid().ToString();
            for (lintCnt = 1; lintCnt <= lstrString.Length - 1; lintCnt++)
            {
                if (IsNumeric(lstrString.Substring(lintCnt, 1)) == true)
                {
                    lstrNumber = lstrNumber + lstrString.Substring(lintCnt, 1);
                    if (lstrNumber.Length > 5)
                    {
                        break;
                    }
                }
            }
            return lstrNumber.PadRight(6, '0').ToString();
        }
        public static string GetTransID()
        {
            return ConvertToJulian() + GetStan() + DateTime.Now.ToString("ssffff");
        }
        private static byte[] HexStringToByte(string hexString)
        {
            try
            {
                int bytesCount = (hexString.Length) / 2;
                byte[] bytes = new byte[bytesCount];
                for (int x = 0; x < bytesCount; ++x)
                {
                    bytes[x] = Convert.ToByte(hexString.Substring(x * 2, 2), 16);
                }
                return bytes;
            }
            catch
            {
                throw;
            }
        }
        public IConfiguration GetConfigurations()
        {
            Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();
            return Configuration;
        }
        public string POSTData(string URL, string body, string ContentType)
        {
            string lstrOut = string.Empty;
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.Timeout = 40 * 1000;
                httpWebRequest.ContentType = ContentType;// "application/json";
                httpWebRequest.Method = "POST";
                byte[] bytes = Encoding.UTF8.GetBytes(body);
                httpWebRequest.Headers.Add("X-IBM-Client-Id", "626b9f48-4743-46c7-9484-d48730dabfe4");
                httpWebRequest.Headers.Add("X-IBM-Client-Secret", "E3iN0oM7fP4aD3yH5kG5rK5jN3nP4mU3iF7vH1oU5cA5kL2kK4");
                string AxisCertificate = CertName;
                string lstrPassword = CertPWD;
                X509Certificate2 privateCert1 = new X509Certificate2(AxisCertificate, lstrPassword);
                httpWebRequest.ClientCertificates.Add(privateCert1);
                httpWebRequest.ProtocolVersion = HttpVersion.Version10;

                using (System.IO.Stream sendStream = httpWebRequest.GetRequestStream())
                {
                    sendStream.Write(bytes, 0, bytes.Length);
                    sendStream.Close();
                }

                HttpWebResponse myHttpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    lstrOut = myHttpWebResponse.StatusDescription;
                }
                System.IO.Stream ReceiveStream = myHttpWebResponse.GetResponseStream();
                using (System.IO.StreamReader sr = new System.IO.StreamReader(ReceiveStream, Encoding.UTF8))
                {
                    lstrOut = sr.ReadToEnd();
                }

                return lstrOut;
            }
            catch (WebException ex)
            {

                using (WebResponse response = ex.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        lstrOut = reader.ReadToEnd();
                    }
                }
                lstrOut = "";
                return lstrOut;
            }
            catch (Exception ex)
            {
                lstrOut = "";
                return lstrOut;
            }
        }


    }

}
