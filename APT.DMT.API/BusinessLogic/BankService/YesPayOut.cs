using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using APT.DMT.API.BusinessObjects.Models;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Reflection;

namespace APT.DMT.API.BusinessLogic.BankService
{
    public class YesPayOut
    {
        private readonly string YESFundTransfer;
        private readonly string CertName;
        private readonly string CertPWD;
        private readonly string ClientId;
        private readonly string SecretKey;
        private readonly string ConsentId;
        private readonly string Identification;
        private readonly string SecondaryIdentification;
        private readonly string Reference;
        private readonly string YESCheckStatus;
        private readonly string basicAuthusername;
        private readonly string basicAuthPWD;
        public static IConfiguration Configuration { get; set; }
        public static LogService _logService = new LogService();
        public YesPayOut()
        {
            Configuration = GetConfigurations();
            YESFundTransfer = Configuration["YesBankValues:YESFundTransfer"];
            CertName = Configuration["YesBankValues:CertName"];
            CertPWD = Configuration["YesBankValues:CertNamePWD"];
            ClientId = Configuration["YesBankValues:ClientId"];
            SecretKey = Configuration["YesBankValues:SecretKey"];
            ConsentId = Configuration["YesBankValues:ConsentId"];
            SecondaryIdentification = Configuration["YesBankValues:SecondaryIdentification"];
            Identification = Configuration["YesBankValues:Identification"];
            Reference = Configuration["YesBankValues:Reference"];
            basicAuthusername = Configuration["YesBankValues:basicAuthusername"];
            basicAuthPWD = Configuration["YesBankValues:basicAuthPWD"];
            YESCheckStatus = Configuration["YesBankValues:YESCheckStatus"];
        }
        public CombinedResponse TransferPayment(YesBankPayRequest Request)
        {
            string lstrReturn = string.Empty;
            string lstrRequest = string.Empty;

            DataSet ds = new DataSet();
            DataSet dsupd = new DataSet();
            SqlCommand objCMD = new SqlCommand();
            HttpResponseMessage Response = new HttpResponseMessage();
            CombinedResponse data = new CombinedResponse();
            YBPTRequest yesbankPaymentRequest = new YBPTRequest();
            try
            {
                yesbankPaymentRequest.Data.ConsentId = ConsentId;
                yesbankPaymentRequest.Data.Initiation.ClearingSystemIdentification = Request.TxnPaymode;
                yesbankPaymentRequest.Data.Initiation.InstructionIdentification = Request.TransactionID;// ReferenceId
                yesbankPaymentRequest.Data.Initiation.EndToEndIdentification = "";
                yesbankPaymentRequest.Data.Initiation.InstructedAmount.Amount = Request.TxnAmount;
                yesbankPaymentRequest.Data.Initiation.InstructedAmount.Currency = "INR";
                yesbankPaymentRequest.Data.Initiation.DebtorAccount.Identification = Identification;  //001190100016907
                yesbankPaymentRequest.Data.Initiation.DebtorAccount.SecondaryIdentification = SecondaryIdentification;
                yesbankPaymentRequest.Data.Initiation.CreditorAccount.SchemeName = Request.BeneIfscCode;
                yesbankPaymentRequest.Data.Initiation.CreditorAccount.Identification = Request.BeneAccNum;
                yesbankPaymentRequest.Data.Initiation.CreditorAccount.Name = Request.BeneName;
                yesbankPaymentRequest.Data.Initiation.CreditorAccount.Unstructured.ContactInformation.EmailAddress = Request.EmailAddress;
                yesbankPaymentRequest.Data.Initiation.CreditorAccount.Unstructured.ContactInformation.MobileNumber = Request.PhoneNumber;
                yesbankPaymentRequest.Data.Initiation.RemittanceInformation.Reference = Reference;
                yesbankPaymentRequest.Data.Initiation.RemittanceInformation.Unstructured.CreditorReferenceInformation = Request.customername;

                if (Request.BeneBankName.ToUpper() == "YES BANK")
                {
                    Request.TxnPaymode = "FT"; // YES to YES
                    yesbankPaymentRequest.Data.Initiation.ClearingSystemIdentification = "FT";
                }
                else
                {
                    if (Request.TxnPaymode == "1")
                    {
                        Request.TxnPaymode = "IMPS"; //IMPS
                        yesbankPaymentRequest.Data.Initiation.ClearingSystemIdentification = "IMPS";
                    }
                    else if (Request.TxnPaymode == "2")
                    {
                        Request.TxnPaymode = "NEFT"; // NEFT
                        yesbankPaymentRequest.Data.Initiation.ClearingSystemIdentification = "NEFT";
                    }
                    else if (Request.TxnPaymode == "3")
                    {
                        Request.TxnPaymode = "RTGS"; // NEFT
                        yesbankPaymentRequest.Data.Initiation.ClearingSystemIdentification = "RTGS"; // RTGS
                    }
                }
                yesbankPaymentRequest.Risk.DeliveryAddress.AddressLine.Add(Request.Address1);
                yesbankPaymentRequest.Risk.DeliveryAddress.AddressLine.Add(Request.Address2);
                yesbankPaymentRequest.Risk.DeliveryAddress.StreetName = Request.Address1;
                yesbankPaymentRequest.Risk.DeliveryAddress.BuildingNumber = "1";
                yesbankPaymentRequest.Risk.DeliveryAddress.PostCode = Request.Pincode;
                yesbankPaymentRequest.Risk.DeliveryAddress.TownName = "sda";
                yesbankPaymentRequest.Risk.DeliveryAddress.CountySubDivision.Add(Request.State);
                yesbankPaymentRequest.Risk.DeliveryAddress.Country = "IN";

                string encBankReq = JsonConvert.SerializeObject(yesbankPaymentRequest);

                string lstrBasicAuth = FormingBasicAuth(basicAuthusername, basicAuthPWD);
                string payoutResponse = POSTData(YESFundTransfer, lstrBasicAuth, encBankReq, "application/json");
                //string payoutResponse = "{\"Data\":{\"ConsentId\":\"453733\",\"TransactionIdentification\":\"f8ea1eb89be211ed87670afa92520000\",\"Status\":\"Received\",\"CreationDateTime\":\"2023-01-24T18:01:05.736+05:30\",\"StatusUpdateDateTime\":\"2023-01-24T18:01:05.736+05:30\",\"Initiation\":{\"InstructionIdentification\":\"YESPOUTB30241801\",\"EndToEndIdentification\":\"\",\"InstructedAmount\":{\"Amount\":\"1000\",\"Currency\":\"INR\"},\"DebtorAccount\":{\"Identification\":\"000190600017042\",\"SecondaryIdentification\":\"453733\"},\"CreditorAccount\":{\"SchemeName\":\"YESB0000262\",\"Identification\":\"026291800001191\",\"Name\":\"Chinnakannan Ajith R\",\"Unstructured\":{\"ContactInformation\":{\"EmailAddress\":\"chinnakannanajithr@gmail.com\",\"MobileNumber\":\"9943535355\"}}},\"RemittanceInformation\":{\"Reference\":\"FRESCO-101\",\"Unstructured\":{\"CreditorReferenceInformation\":\"RemeToBeneInfo\"}},\"ClearingSystemIdentification\":\"FT\"}},\"Risk\":{\"DeliveryAddress\":{\"AddressLine\":[\"test\",\"test\"],\"StreetName\":\"test\",\"BuildingNumber\":\"1\",\"PostCode\":\"41234\",\"TownName\":\"sda\",\"CountySubDivision\":[\"TN\"],\"Country\":\"IN\"}},\"Links\":{\"Self\":\"https:\\/\\/olyuatesbtrans.yesbank.in:7085\\/api-banking\\/v2.0\\/domestic-payments\\/payment-details\"}}";
                if (payoutResponse.IsNullOrEmpty())
                {
                    data.ApiRequest = encBankReq;
                    data.ApiResponse = "Failed";
                    return data;
                }
                data.ApiRequest = encBankReq;
                data.ApiResponse = payoutResponse;
                return data;


            }
            catch (System.Exception ex)
            {
                data.ApiRequest = "Error";
                data.ApiResponse = "Error";
                return data;
            }

        }
        public CombinedResponse CheckStatus(String transactionID)
        {

            string lstrReturn = string.Empty;
            string lstrRequest = string.Empty;

            DataSet ds = new DataSet();
            DataSet dsupd = new DataSet();
            SqlCommand objCMD = new SqlCommand();
            HttpResponseMessage Response = new HttpResponseMessage();
            datas jsonRequest = new datas();
            CombinedResponse data = new CombinedResponse();
            CheckStatus jsonRequests = new CheckStatus();

            try
            {
                jsonRequest.InstrId = transactionID;
                jsonRequest.ConsentId = ConsentId;//"453733";
                jsonRequest.SecondaryIdentification = SecondaryIdentification;//"453733";

                jsonRequests.Data = jsonRequest;
                string encBankReq = JsonConvert.SerializeObject(jsonRequests);
                string lstrBasicAuth = FormingBasicAuth(basicAuthusername, basicAuthPWD);
                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "YPayout ChkSts!2", JsonConvert.SerializeObject(jsonRequest), jsonRequest.SecondaryIdentification);
                string ststusResponse = POSTData(YESCheckStatus, lstrBasicAuth, encBankReq, "application/json");
                //string ststusResponse = "{\"Data\":{\"ConsentId\":\"453733\",\"TransactionIdentification\":\"faec7b3e9be211ed87670afa92520000\",\"Status\":\"SettlementCompleted\",\"CreationDateTime\":\"2023-01-24T18:01:08.000+05:30\",\"StatusUpdateDateTime\":\"2023-01-24T18:01:09.000+05:30\",\"Initiation\":{\"InstructionIdentification\":\"YESPOUTB30241801\",\"EndToEndIdentification\":\"1387420220402000200053974\",\"InstructedAmount\":{\"Amount\":1E+3,\"Currency\":\"INR\"},\"DebtorAccount\":{\"Identification\":\"000190600017042\",\"SecondaryIdentification\":\"453733\"},\"CreditorAccount\":{\"SchemeName\":\"YESB0000262\",\"Identification\":\"026291800001191\",\"Name\":\"Chinnakannan Ajith R\",\"BeneficiaryCode\":null,\"Unstructured\":{\"ContactInformation\":{\"EmailAddress\":\"chinnakannanajithr@gmail.com\",\"MobileNumber\":\"9943535355\"}},\"RemittanceInformation\":{\"Unstructured\":{\"CreditorReferenceInformation\":\"RemeToBeneInfo\"}},\"ClearingSystemIdentification\":\"FT\"}}},\"Risk\":{\"PaymentContextCode\":null,\"DeliveryAddress\":{\"AddressLine\":\"test,test\",\"StreetName\":\"test\",\"BuildingNumber\":\"1\",\"PostCode\":\"41234\",\"TownName\":\"sda\",\"CountySubDivision\":\"TN\",\"Country\":\"IN\"}},\"Links\":{\"Self\":\"https:\\/\\/olyuatesbtrans.yesbank.in:7085\\/api-banking\\/v2.0\\/domestic-payments\\/payment-details\"}}";

                if (ststusResponse.IsNullOrEmpty())
                {
                    data.ApiRequest = encBankReq;
                    data.ApiResponse = "Failed";
                    _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "YPayout ChkSts!Failed", JsonConvert.SerializeObject(data), jsonRequest.SecondaryIdentification);
                    return data;
                }
                data.ApiRequest = encBankReq;
                data.ApiResponse = ststusResponse;
                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "YPayout ChkSts!Succe", JsonConvert.SerializeObject(data), jsonRequest.SecondaryIdentification);
                return data;


            }
            catch (Exception ex)
            {
                data.ApiRequest = "Error";
                data.ApiResponse = "Error";
                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "YPayout ChkSts!err", JsonConvert.SerializeObject(ex), jsonRequest.SecondaryIdentification);
                return data;

            }

        }
        private string FormingBasicAuth(string lstrUserName, string lstrPassword)
        {
            try
            {
                return "Basic " + Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(lstrUserName + ":" + lstrPassword));
            }
            catch (Exception ex)
            {
                return "";
            }
            finally
            {

            }
        }
        public string POSTData(string URL, string APIKEY, string body, string ContentType)
        {

            string lstrOut = string.Empty;
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls |
                    SecurityProtocolType.Tls11;
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.Timeout = 40 * 1000;
                httpWebRequest.ContentType = ContentType;// "application/json";
                httpWebRequest.Method = "POST";
                byte[] bytes = Encoding.UTF8.GetBytes(body);
                httpWebRequest.Headers.Add("Authorization", APIKEY);
                httpWebRequest.Headers.Add("X-IBM-Client-Id", ClientId);
                httpWebRequest.Headers.Add("X-IBM-Client-Secret", SecretKey);



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

        public CombinedResponse TransferPayment_vignesh(string Amount ,string Paymode,string txnid)
        {
            CombinedResponse data = new CombinedResponse();
            try
            {
                YBPTRequest yesbankPaymentRequest = new YBPTRequest();
                yesbankPaymentRequest = JsonConvert.DeserializeObject<YBPTRequest>(File.ReadAllText("E:\\RequestFile\\YesReq.txt"));
                if(Paymode == "1")
                {
                    yesbankPaymentRequest.Data.Initiation.ClearingSystemIdentification = "IMPS";
                }
                else if(Paymode == "2")
                {
                    yesbankPaymentRequest.Data.Initiation.ClearingSystemIdentification = "NEFT";
                }
                else if(Paymode == "3")
                {
                    yesbankPaymentRequest.Data.Initiation.ClearingSystemIdentification = "RTGS";
                }
                
                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Entry", Amount + "---" + Paymode, txnid);
                yesbankPaymentRequest.Data.Initiation.InstructionIdentification = txnid;// ReferenceId
               
                yesbankPaymentRequest.Data.Initiation.InstructedAmount.Amount =Amount;

                string encBankReq = JsonConvert.SerializeObject(yesbankPaymentRequest);
                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Request --"+ encBankReq, Amount + "---" + Paymode, txnid);
                string lstrBasicAuth = FormingBasicAuth(basicAuthusername, basicAuthPWD);
                string payoutResponse = POSTData(YESFundTransfer, lstrBasicAuth, encBankReq, "application/json");
                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Request --" + encBankReq + "----Response --" + payoutResponse, Amount + "---" + Paymode, txnid);
                //string payoutResponse = "{\"Data\":{\"ConsentId\":\"453733\",\"TransactionIdentification\":\"f8ea1eb89be211ed87670afa92520000\",\"Status\":\"Received\",\"CreationDateTime\":\"2023-01-24T18:01:05.736+05:30\",\"StatusUpdateDateTime\":\"2023-01-24T18:01:05.736+05:30\",\"Initiation\":{\"InstructionIdentification\":\"YESPOUTB30241801\",\"EndToEndIdentification\":\"\",\"InstructedAmount\":{\"Amount\":\"1000\",\"Currency\":\"INR\"},\"DebtorAccount\":{\"Identification\":\"000190600017042\",\"SecondaryIdentification\":\"453733\"},\"CreditorAccount\":{\"SchemeName\":\"YESB0000262\",\"Identification\":\"026291800001191\",\"Name\":\"Chinnakannan Ajith R\",\"Unstructured\":{\"ContactInformation\":{\"EmailAddress\":\"chinnakannanajithr@gmail.com\",\"MobileNumber\":\"9943535355\"}}},\"RemittanceInformation\":{\"Reference\":\"FRESCO-101\",\"Unstructured\":{\"CreditorReferenceInformation\":\"RemeToBeneInfo\"}},\"ClearingSystemIdentification\":\"FT\"}},\"Risk\":{\"DeliveryAddress\":{\"AddressLine\":[\"test\",\"test\"],\"StreetName\":\"test\",\"BuildingNumber\":\"1\",\"PostCode\":\"41234\",\"TownName\":\"sda\",\"CountySubDivision\":[\"TN\"],\"Country\":\"IN\"}},\"Links\":{\"Self\":\"https:\\/\\/olyuatesbtrans.yesbank.in:7085\\/api-banking\\/v2.0\\/domestic-payments\\/payment-details\"}}";
                if (payoutResponse.IsNullOrEmpty())
                {
                    data.ApiRequest = encBankReq;
                    data.ApiResponse = "Failed";
                    return data;
                }
                data.ApiRequest = encBankReq;
                data.ApiResponse = payoutResponse;
                return data;
            }
            catch(Exception ex)
            {
                data.ApiRequest = "Failed";
                data.ApiResponse = "Failed";
                _logService.WriteLog(MethodBase.GetCurrentMethod().Name, "Exception --" + ex.ToString, Amount + "---" + Paymode, txnid);
                return data;
            }
        }
        public IConfiguration GetConfigurations()
        {
            Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();
            return Configuration;
        }


    }
}
