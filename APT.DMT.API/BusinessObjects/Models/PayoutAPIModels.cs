using System.ComponentModel.DataAnnotations;

namespace APT.DMT.API.BusinessObjects.Models
{
    public class UpdateTxnStatusPayout
    {
        public string TransactionID { get; set; }
        public string BankReferrenceNumber { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }
        public string TranStatus { get; set; }
        public string Flag { get; set; }
        public string BankTypeID { get; set; }

    }

    public class CombinedResponse
    {
        public string ApiRequest { get; set; }
        public string ApiResponse { get; set; }
    }


    // AXIS Bank Model -- START --
    public class GetStatusResponseBody
    {
        public GetStatusResponseBody()
        {
            data = new data();
        }
        public data data { get; set; }
        public string message { get; set; }
        public string status { get; set; }
    }
    public class data
    {
        public data()
        {
            CUR_TXN_ENQ = new List<CURTXNENQ>();
        }
        public List<CURTXNENQ> CUR_TXN_ENQ { get; set; }
        public string errorMessage { get; set; }
        public string checksum { get; set; }
    }
    public class CURTXNENQ
    {
        public string corpCode { get; set; }
        public string statusDescription { get; set; }
        public string batchNo { get; set; }
        public string utrNo { get; set; }
        public string processingDate { get; set; }
        public string responeCode { get; set; }
        public string crn { get; set; }
        public string transactionStatus { get; set; }
    }

    public class TransferResponseAxis
    {
        public string data { get; set; }
        public string message { get; set; }
        public string status { get; set; }
    }

    public class AxisPaymentResponse
    {
        public object needtochange { get; set; }
    }
    public class PayoutStatusRequest
    {
        [Required]
        public string transactionID { get; set; }

    }
    public class CheckStatusReqEnc
    {
        public CheckStatusReqEnc()
        {
            GetStatusRequest = new GetStatusRequestEnc();
        }
        public GetStatusRequestEnc GetStatusRequest { get; set; }
    }
    public class GetStatusRequestEnc
    {
        public GetStatusRequestEnc()
        {
            SubHeader = new SubHeader();
        }
        public SubHeader SubHeader { get; set; }

        public string GetStatusRequestBodyEncrypted { get; set; }
    }
    public class GetStatusResponse
    {
        public GetStatusResponse()
        {
            SubHeader = new SubHeader();
        }
        public SubHeader SubHeader { get; set; }
        public string GetStatusResponseBodyEncrypted { get; set; }
    }
    public class CheckStatusReq
    {
        public CheckStatusReq()
        {
            GetStatusRequest = new GetStatusRequest();
        }
        public GetStatusRequest GetStatusRequest { get; set; }
    }
    public class GetStatusRequest
    {
        public GetStatusRequest()
        {
            SubHeader = new SubHeader();
            GetStatusRequestBody = new GetStatusRequestBody();
        }
        public SubHeader SubHeader { get; set; }
        public GetStatusRequestBody GetStatusRequestBody { get; set; }

    }
    public class GetStatusRequestBody
    {
        public string channelId { get; set; }
        public string corpCode { get; set; }
        public string crn { get; set; }
        public string checksum { get; set; }
    }
    public class AxisGetStatusResponse
    {
        public AxisGetStatusResponse()
        {
            GetStatusResponse = new GetStatusResponse();
        }
        public GetStatusResponse GetStatusResponse { get; set; }
    }
    public class PayoutPaymentRequest
    {
        public string sendermobilenumber { get; set; }
        public string PayeeRefID { get; set; }
        public string Amount { get; set; }
        public string PaymentTransactionTypeRefID { get; set; }
        public string PayModeRefID { get; set; }
        public string Agentmobile { get; set; }
        public string ifsccodein { get; set; }
        public string ChannelType { get; set; }
        public string LATITUDE { get; set; }
        public string Longitude { get; set; }
        public string AgentRemarks { get; set; }
        public string Remarks { get; set; }
        public string Accountnumerin { get; set; }
        public string ARefID { get; set; }
        public string location_acquirace { get; set; }
        public string agent_ip { get; set; }
        public string MacID { get; set; }

    }
    public class AxisPaymentRequest
    {
        [Required]
        public String txnPaymode { get; set; }
        [Required]
        public String txnAmount { get; set; }
        [Required]
        public String beneName { get; set; }
        [Required]
        public String beneCode { get; set; }
        [Required]
        public String beneAccNum { get; set; }
        [Required]
        public String beneIfscCode { get; set; }
        [Required]
        public String beneBankName { get; set; }
        [Required]
        public String transactionID { get; set; }

    }


    public class InvoiceDetail
    {
        public string invoiceAmount { get; set; }
        public string invoiceNumber { get; set; }
        public string invoiceDate { get; set; }
        public string cashDiscount { get; set; }
        public string tax { get; set; }
        public string netAmount { get; set; }
    }
    public class PaymentDetail
    {
        public PaymentDetail()
        {
            invoiceDetails = new List<InvoiceDetail>();
        }
        public string txnPaymode { get; set; }
        public string custUniqRef { get; set; }
        public string corpAccNum { get; set; }
        public string valueDate { get; set; }
        public string txnAmount { get; set; }
        public string beneLEI { get; set; }
        public string beneName { get; set; }
        public string beneCode { get; set; }
        public string beneAccNum { get; set; }
        public string beneAcType { get; set; }
        public string beneAddr1 { get; set; }
        public string beneAddr2 { get; set; }
        public string beneAddr3 { get; set; }
        public string beneCity { get; set; }
        public string beneState { get; set; }
        public string benePincode { get; set; }
        public string beneIfscCode { get; set; }
        public string beneBankName { get; set; }
        public string baseCode { get; set; }
        public string chequeNumber { get; set; }
        public string chequeDate { get; set; }
        public string payableLocation { get; set; }
        public string printLocation { get; set; }
        public string beneEmailAddr1 { get; set; }
        public string beneMobileNo { get; set; }
        public string productCode { get; set; }
        public string txnType { get; set; }
        public List<InvoiceDetail> invoiceDetails { get; set; }
        public string enrichment1 { get; set; }
        public string enrichment2 { get; set; }
        public string enrichment3 { get; set; }
        public string enrichment4 { get; set; }
        public string enrichment5 { get; set; }
        public string senderToReceiverInfo { get; set; }
    }
    public class TransferPaymentRequestBody
    {
        public TransferPaymentRequestBody()
        {
            paymentDetails = new List<PaymentDetail>();
        }
        public string channelId { get; set; }
        public string corpCode { get; set; }
        public List<PaymentDetail> paymentDetails { get; set; }
        public string checksum { get; set; }
    }
    public class SubHeader
    {
        public string requestUUID { get; set; }
        public string serviceRequestId { get; set; }
        public string serviceRequestVersion { get; set; }
        public string channelId { get; set; }
    }
    public class TransferPaymentRequest
    {
        public TransferPaymentRequest()
        {
            SubHeader = new SubHeader();
            TransferPaymentRequestBody = new TransferPaymentRequestBody();
        }
        public SubHeader SubHeader { get; set; }
        public TransferPaymentRequestBody TransferPaymentRequestBody { get; set; }
    }
    public class FundTransferBankRequest
    {
        public FundTransferBankRequest()
        {
            TransferPaymentRequest = new TransferPaymentRequest();
        }
        public TransferPaymentRequest TransferPaymentRequest { get; set; }
    }
    public class TransferPaymentEncRequest
    {
        public TransferPaymentEncRequest()
        {
            SubHeader = new SubHeader();
        }
        public SubHeader SubHeader { get; set; }
        public string TransferPaymentRequestBodyEncrypted { get; set; }
    }
    public class FundTransferEncRequest
    {
        public FundTransferEncRequest()
        {
            TransferPaymentRequest = new TransferPaymentEncRequest();
        }
        public TransferPaymentEncRequest TransferPaymentRequest { get; set; }

    }
    public class TransferPaymentResponse
    {
        public TransferPaymentResponse()
        {
            SubHeader = new SubHeader();
        }
        public SubHeader SubHeader { get; set; }
        public string TransferPaymentResponseBodyEncrypted { get; set; }
    }
    public class AxisResponse
    {
        public AxisResponse()
        {
            TransferPaymentResponse = new TransferPaymentResponse();
        }
        public TransferPaymentResponse TransferPaymentResponse { get; set; }
    }

    // AXIS Bank Model -- END --
    //==============================================================================
    // YES Bank Model -- START --

    public class YBPTRequest
    {
        public YBPTRequest()
        {
            Data = new Data();
            Risk = new Risk();
        }
        public Data Data { get; set; }
        public Risk Risk { get; set; }

    }
    public class Risk
    {
        public Risk()
        {
            DeliveryAddress = new DeliveryAddress();
        }
        public DeliveryAddress DeliveryAddress { get; set; }
    }
    public class DeliveryAddress
    {

        public DeliveryAddress()
        {
            AddressLine = new List<string>();
            CountySubDivision = new List<string>();
        }

        public List<string> AddressLine { get; set; }
        public string StreetName { get; set; }
        public string BuildingNumber { get; set; }
        public string PostCode { get; set; }
        public string TownName { get; set; }
        public List<string> CountySubDivision { get; set; }
        public string Country { get; set; }
    }
    public class Data
    {
        public string ConsentId { get; set; }
        public Data()
        {
            Initiation = new Initiation();
        }
        public Initiation Initiation { get; set; }

    }
    public class Initiation
    {
        public string InstructionIdentification { get; set; }
        public string EndToEndIdentification { get; set; }
        public Initiation()
        {
            InstructedAmount = new InsAmt();
            DebtorAccount = new DebAcc();
            CreditorAccount = new CreAcc();
            RemittanceInformation = new RemInfo();

        }
        public InsAmt InstructedAmount { get; set; }
        public DebAcc DebtorAccount { get; set; }
        public CreAcc CreditorAccount { get; set; }
        public RemInfo RemittanceInformation { get; set; }

        public string ClearingSystemIdentification { get; set; }
    }
    public class InsAmt
    {
        public string Amount { get; set; }
        public string Currency { get; set; }
    }
    public class DebAcc
    {
        public string Identification { get; set; }
        public string SecondaryIdentification { get; set; }

    }
    public class CreAcc
    {
        public string SchemeName { get; set; }
        public string Identification { get; set; }
        public string Name { get; set; }
        public CreAcc()
        {
            Unstructured = new unstruct();
        }
        public unstruct Unstructured { get; set; }
    }
    public class unstruct
    {
        public unstruct()
        {
            ContactInformation = new ContactInformation();
        }
        public ContactInformation ContactInformation { get; set; }
    }
    public class ContactInformation
    {
        public string EmailAddress { get; set; }
        public string MobileNumber { get; set; }
    }
    public class RemInfo
    {
        public string Reference { get; set; }
        public RemInfo()
        {
            Unstructured = new Unstructured();
        }
        public Unstructured Unstructured { get; set; }

    }
    public class Unstructured
    {
        public string CreditorReferenceInformation { get; set; }
    }
    public class YesBankPayRequest
    {
        [Required]
        public String TransactionID { get; set; }
        [Required]
        public string TxnAmount { get; set; }
        [Required]
        public string BeneIfscCode { get; set; }
        [Required]
        public string BeneAccNum { get; set; }
        [Required]
        public string TxnPaymode { get; set; }
        [Required]
        public string BeneBankName { get; set; }
        [Required]
        public string BeneName { get; set; }
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Address1 { get; set; }
        [Required]
        public string Address2 { get; set; }
        [Required]
        public string Pincode { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string customername { get; set; }


    }
    public class CheckStatus
    {
        public datas Data { get; set; }

    }
    public class datas
    {
        public string InstrId { get; set; }
        public string ConsentId { get; set; }
        public string SecondaryIdentification { get; set; }
    }
    public class CheckStatusRequest
    {
        public string referenceID { get; set; }


    }

    public class CreditorAccount
    {
        public string SchemeName { get; set; }
        public string Identification { get; set; }
        public string Name { get; set; }
        public object BeneficiaryCode { get; set; }
        public Unstructured Unstructured { get; set; }
        public RemittanceInformation RemittanceInformation { get; set; }
        public string ClearingSystemIdentification { get; set; }
    }

    public class DataDUP
    {
        public string ConsentId { get; set; }
        public string TransactionIdentification { get; set; }
        public string Status { get; set; }
        public DateTime CreationDateTime { get; set; }
        public DateTime StatusUpdateDateTime { get; set; }
        public InitiationDUP Initiation { get; set; }
    }

    public class DebtorAccount
    {
        public string Identification { get; set; }
        public string SecondaryIdentification { get; set; }
    }

    public class DeliveryAddressDUP
    {
        public List<string> AddressLine { get; set; }
        public string StreetName { get; set; }
        public string BuildingNumber { get; set; }
        public string PostCode { get; set; }
        public string TownName { get; set; }
        public List<string> CountySubDivision { get; set; }
        public string Country { get; set; }
    }

    public class InitiationDUP
    {
        public string InstructionIdentification { get; set; }
        public string EndToEndIdentification { get; set; }
        public InstructedAmount InstructedAmount { get; set; }
        public DebtorAccount DebtorAccount { get; set; }
        public CreditorAccount CreditorAccount { get; set; }
        public RemittanceInformation RemittanceInformation { get; set; }
        public string ClearingSystemIdentification { get; set; }
    }

    public class InstructedAmount
    {
        public string Amount { get; set; }
        public string Currency { get; set; }
    }

    public class Links
    {
        public string Self { get; set; }
    }

    public class RemittanceInformation
    {
        public string Reference { get; set; }
        public UnstructuredDUP Unstructured { get; set; }
    }

    public class RiskDUP
    {
        public object PaymentContextCode { get; set; }
        public DeliveryAddressDUP DeliveryAddress { get; set; }
    }

    public class YesBankResponse
    {
        public DataDUP Data { get; set; }
        public RiskDUP Risk { get; set; }
        public Links Links { get; set; }
    }

    public class UnstructuredDUP
    {
        public ContactInformation ContactInformation { get; set; }
        public string CreditorReferenceInformation { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class YesBankPayoutErrorResponse
    {
        public string Code { get; set; }
        public string Id { get; set; }
        public string Message { get; set; }
        public string ActionCode { get; set; }
        public string ActionDescription { get; set; }
    }






    public class CreditorAccountDUP
    {
        public string SchemeName { get; set; }
        public string Identification { get; set; }
        public string Name { get; set; }
        public object BeneficiaryCode { get; set; }
        public Unstructured Unstructured { get; set; }
        public RemittanceInformation RemittanceInformation { get; set; }
        public string ClearingSystemIdentification { get; set; }
    }



    public class DeliveryAddressDup
    {
        public string AddressLine { get; set; }
        public string StreetName { get; set; }
        public string BuildingNumber { get; set; }
        public string PostCode { get; set; }
        public string TownName { get; set; }
        public string CountySubDivision { get; set; }
        public string Country { get; set; }
    }

    public class InitiationDup
    {
        public string InstructionIdentification { get; set; }
        public string EndToEndIdentification { get; set; }
        public InstructedAmount InstructedAmount { get; set; }
        public DebtorAccount DebtorAccount { get; set; }
        public CreditorAccountDUP CreditorAccount { get; set; }
    }


    public class RiskDup
    {
        public object PaymentContextCode { get; set; }
        public DeliveryAddressDup DeliveryAddress { get; set; }
    }

    public class YesBankStatusResponse
    {
        public DataDUP Data { get; set; }
        public RiskDup Risk { get; set; }
        public Links Links { get; set; }
    }







    // YES Bank Model -- END --
    public class APTPayOutTransactionResponse
    {

        public string transaction_id { get; set; } = string.Empty;
        public string rrn { get; set; } = "";
        public string transaction_date { get; set; } = string.Empty;
        public string commision { get; set; } = string.Empty;
        public string amount { get; set; } = string.Empty;
        public string totalAmount { get; set; } = string.Empty;
        public string bene_name { get; set; } = string.Empty;
        public string bene_acnt_no { get; set; } = string.Empty;
        public string customer_name { get; set; } = string.Empty;

    }

    public class APTUpdateBeneNameYesbank_payout
    {
        public string transaction_id { get; set; } = string.Empty;
        public string rrn { get; set; } = "";
        public string bene_name { get; set; } = string.Empty;
        public string status_update_datetime { get; set; } = string.Empty;
    }

    public class Getpin_PayoutRequest
    {
        public string retailer_number { get; set; } = string.Empty;
        
    }
    public class Insertpin_PayoutRequest
    {
        public string retailer_number { get; set; } = string.Empty;
        public string new_pin { get; set; } = string.Empty;
        public string otp { get; set; } = string.Empty;
    }

    public class Getpin_PayoutResponse
    {
        public string retailer_number { get; set; } = string.Empty;
        public string pin_Required { get; set; } = string.Empty;

    }
    public class Verifypin_PayoutRequest
    {
        public string retailer_number { get; set; } = string.Empty;
        public string pin { get; set; } = string.Empty;

    }
    public class Verifypin_PayoutResponse
    {
        public string retailer_number { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;

    }
    public class APTUpdateCustomerKYCPayoutRequest
    {
        public string transaction_id { get; set; } = string.Empty;
        public string approval_status { get; set; } = string.Empty;
        public string user_ref_id { get; set; } = string.Empty;

    }

    public class PayOutPGtoPayoutTxnRequest
    {
        public string transactionID { get; set; }

    }
}
