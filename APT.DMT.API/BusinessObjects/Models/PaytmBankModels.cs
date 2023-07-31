namespace APT.DMT.API.BusinessObjects.Models
{
    public class PaytmPrevalidateCustomerResponse
    {
        public string status { get; set; } = string.Empty;
        public string response_code { get; set; } = string.Empty;
        public string firstName { get; set; } = string.Empty;
        public string lastName { get; set; } = string.Empty;
        public string customerMobile { get; set; } = string.Empty;
        public string limitLeft { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
        public string txn_id { get; set; } = string.Empty;




    }

    public class PaytmSendOTPRequest
    {
        public string customerMobile { get; set; } = string.Empty;
        public string otpType { get; set; } = "registrationOtp";
        public BeneOTPDetails properties { get; set; }
    }
    public class BeneOTPDetails
    {
        public string benAccNum { get; set; } = string.Empty;
    }

    public class PaytmSendOTPResponse
    {
        public string status { get; set; } = string.Empty;
        public string state { get; set; } = string.Empty;
        public string response_code { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
        public string txn_id { get; set; } = string.Empty;
        public string errorCode { get; set; } = string.Empty;
        public string errorMessage { get; set; } = string.Empty;

    }


    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class PaytmCustomerRegistrationAddress
    {
        public string name { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string pin { get; set; }
        public string mobile { get; set; }
    }

    public class PaytmCustomerRegistrationName
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
    }

    public class PaytmCustomerRegistrationRequest
    {
        public string customerMobile { get; set; }
        public string otp { get; set; }
        public string state { get; set; }
        public PaytmCustomerRegistrationName name { get; set; }
        public PaytmCustomerRegistrationAddress address { get; set; }
    }

    public class PaytmCustomerRegistrationResponse
    {
        public string status { get; set; } = string.Empty;
        public string response_code { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
        public string txn_id { get; set; } = string.Empty;
        public string customerMobile { get; set; } = string.Empty;

    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class BeneficiaryDetails
    {
        public string accountNumber { get; set; }
        public string bankName { get; set; }
        public string benIfsc { get; set; }
        public string name { get; set; }
        public string nickName { get; set; }
    }

    public class PaytmAddBeneRequest
    {
        public BeneficiaryDetails beneficiaryDetails { get; set; }
        public string customerMobile { get; set; }
        public string otp { get; set; }
        public string state { get; set; }
    }

    public class PaytmAddBeneResponse
    {
        public string status { get; set; } = string.Empty;
        public string response_code { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
        public string txn_id { get; set; } = string.Empty;
        public string beneficiaryId { get; set; } = string.Empty;
        public string customerMobile { get; set; } = string.Empty;


    }
    public class BeneValidateDetails
    {
        public string accountNumber { get; set; }
        public string bankName { get; set; }
        public string benIfsc { get; set; }
    }

    public class BeneValidateRequest
    {
        public BeneValidateDetails beneficiaryDetails { get; set; }
        public string channel { get; set; } = "S2S";
        public string customerMobile { get; set; }
        public string transactionType { get; set; } = "CORPORATE_PENNY_DROP";
        public string txnReqId { get; set; }
    }

    public class BeneValidateExtraInfo
    {
        public string beneficiaryName { get; set; }
    }

    public class BeneValidateResponse
    {
        public string status { get; set; }
        public string response_code { get; set; }
        public string txn_id { get; set; }
        public string message { get; set; }
        public string customerMobile { get; set; }
        public decimal amount { get; set; }
        public string mw_txn_id { get; set; }
        public BeneValidateExtraInfo extra_info { get; set; }
        public string rrn { get; set; }
        public string transactionDate { get; set; }
    }
    public class PaytmDMTTransactionRequest
    {
        public decimal amount { get; set; }
        public string beneficiaryId { get; set; }
        public string channel { get; set; } = "S2S";
        public string customerMobile { get; set; }
        public string transactionType { get; set; } = "CORPORATE_DOMESTIC_REMITTANCE";
        public string txnReqId { get; set; }
        public string mode { get; set; }
        public string ifscBased { get; set; }
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class PaytmDMTTransactionResponseExtraInfo
    {
        public string totalAmount { get; set; }
        public string commission { get; set; }
        public string mode { get; set; }
        public string beneficiaryName { get; set; }
    }

    public class PaytmDMTTransactionResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public string customerMobile { get; set; }
        public decimal amount { get; set; }
        public int response_code { get; set; }
        public string txn_id { get; set; }
        public string mw_txn_id { get; set; }
        public PaytmDMTTransactionResponseExtraInfo extra_info { get; set; }
        public string rrn { get; set; }
        public string transactionDate { get; set; }
    }

    public class GetCustomerResponse
    {
        public string customer_ref_id { get; set; } = string.Empty;
        public string customer_name { get; set; } = string.Empty;
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class GetBeneListResponseAccountDetail
    {
        public string ifscCode { get; set; }
        public string bankName { get; set; }
        public string accountHolderName { get; set; }
        public string accountNumber { get; set; }
    }

    public class GetBeneListResponseBeneficiary
    {
        public string beneficiaryId { get; set; }
        public GetBeneListResponseAccountDetail accountDetail { get; set; }
    }

    public class GetBeneListResponse
    {
        public string status { get; set; }
        public string response_code { get; set; }
        public string txn_id { get; set; }
        public string message { get; set; }
        public string customerMobile { get; set; }
        public int totalCount { get; set; }
        public List<GetBeneListResponseBeneficiary> beneficiaries { get; set; }
    }
    public class PaytmDeletePayeeRequest
    {
        public string beneficiaryId { get; set; }
        public string client { get; set; }
        public string otp { get; set; }
        public string state { get; set; }
        public string customerMobile { get; set; }
    }

    public class PaytmDeletePayeeResponse
    {
        public string status { get; set; }
        public int response_code { get; set; }
        public string customerMobile { get; set; }
    }

    public class PaytmGetStatusResponse
    {
        public string status { get; set; }
        public string mode { get; set; }
        public int response_code { get; set; }
        public string txn_id { get; set; }
        public string mw_txn_id { get; set; }
        public string rrn { get; set; }
    }


}
