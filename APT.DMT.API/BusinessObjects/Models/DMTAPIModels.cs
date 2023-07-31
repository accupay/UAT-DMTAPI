using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace APT.DMT.API.BusinessObjects.Models
{
    public class APTRegisterCustomer
    {
        public string mobile_no { get; set; } = string.Empty;
        public string agent_mobile_no { get; set; } = string.Empty;
        public string customer_name { get; set; } = string.Empty;
        public string address1 { get; set; } = string.Empty;
        public string address2 { get; set; } = string.Empty;
        public string pincode { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string state_ref_id { get; set; } = string.Empty;
        public string state { get; set; } = string.Empty;
        public string city { get; set; } = string.Empty;
        public string gender { get; set; } = string.Empty;
        public string dob { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;
        public string otp_validation_flag { get; set; } = string.Empty;
        public string agent_ref_id { get; set; } = string.Empty;
        public string pin { get; set; } = string.Empty;
        public string location_ref_id { get; set; } = string.Empty;

        public string flag { get; set; } = string.Empty;
        public string bank_id { get; set; } = string.Empty;
        public string bank_ref_id { get; set; } = string.Empty;
        public string account_type { get; set; } = string.Empty;
        // public SessionValidation session { get; set; }

    }

    public class APTRegisterCustomerResponse
    {
        public bool is_internal { get; set; } = false;
        public string otp_state { get; set; } = "";
    }
    public class APTGetCustomerInfo
    {
        public string mobile_no { get; set; } = string.Empty;
        public string agent_ref_id { get; set; } = string.Empty;
        public string flag { get; set; } = string.Empty;
        [JsonIgnore]
        public bool is_internal { get; set; } = false;
        // public SessionValidation session { get; set; }


    }

    public class APTDeletePayee
    {
        public string mobile_no { get; set; } = string.Empty;
        public string payee_ref_id { get; set; } = string.Empty;
        public string otp { get; set; } = string.Empty;
        public string otp_state { get; set; } = string.Empty;
        public string beneficiary_id { get; set; } = string.Empty;
    }
    public class APTAddPayee
    {
        [JsonIgnore]
        public string payee_ref_id { get; set; } = string.Empty;
        public string payee_name { get; set; } = string.Empty;
        public string payee_mobile_no { get; set; } = string.Empty;
        public string ifsc_code { get; set; } = string.Empty;
        public string ifsc_bank_ref_id { get; set; } = string.Empty;
        public string ifsc_bank_branch_ref_id { get; set; } = string.Empty;
        public string account_number { get; set; } = string.Empty;
        public string active_status_ref_id { get; set; } = string.Empty;
        public string payee_name_npci { get; set; } = string.Empty;
        [JsonIgnore]
        public string validated { get; set; } = "0";
        public string mobile_no { get; set; } = string.Empty;
        public string agent_ref_id { get; set; } = string.Empty;
        public string bank_type_id { get; set; } = string.Empty;
        public string flag { get; set; } = string.Empty;
        [JsonIgnore]
        public string bank_id { get; set; } = string.Empty;
        [JsonIgnore]
        public string bank_ref_id { get; set; } = string.Empty;
        //  public SessionValidation session { get; set; }

    }

    public class APTAddPayeeResponse
    {
        public string payee_ref_id { get; set; } = string.Empty;
        public string otp_state { get; set; } = string.Empty;
    }

    public class APTGetAllPayee
    {
        public string customer_mobile_no { get; set; } = string.Empty;
        public string agent_ref_id { get; set; } = string.Empty;
    }
        public class APTGetAllPayeeResponse
    {
        public string payee_ref_id { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public string payee_name { get; set; } = string.Empty;
        public string account_number { get; set; } = string.Empty;
        public string mobile_no { get; set; } = string.Empty;
        public string global_ifsc_code { get; set; } = string.Empty;
        public string bank_ref_id { get; set; } = string.Empty;
        public string bank_name { get; set; } = string.Empty;
        public string regn_date { get; set; } = string.Empty;
        public string activation_date { get; set; } = string.Empty;
        public string current_balance { get; set; } = string.Empty;
        public string ifsc_code { get; set; } = string.Empty;
        public string ifsc_bank_branch_ref_id { get; set; } = string.Empty;
        public string n_bin { get; set; } = string.Empty;
        public string imps_status_ref_id { get; set; } = string.Empty;
        public string neft_status_ref_id { get; set; } = string.Empty;
        public string validated { get; set; } = string.Empty;
        public string npci_payee_name { get; set; } = string.Empty;
        public string active_status_ref_id { get; set; } = string.Empty;
        public string beneficiaryId { get; set; } = string.Empty;

        public string isBankflowNeeded { get; set; } = string.Empty;
    }





    public class APTPrevalidateCustomerRequest
    {
        public string customer_mobile_no { get; set; } = string.Empty;
    }

    public class APTGetPincodeDetailsRequest
    {
        public string pincode { get; set; } = string.Empty;
    }
    //Master Data's
    public class APTGetPincodeDetailsResponse
    {
        public string location_ref_id { get; set; } = string.Empty;
        public string tier { get; set; } = string.Empty;
        public string category { get; set; } = string.Empty;
        public string area { get; set; } = string.Empty;
        public string pincode { get; set; } = string.Empty;
        public string sub_district { get; set; } = string.Empty;
        public string district { get; set; } = string.Empty;
        public string state { get; set; } = string.Empty;
        public string zone { get; set; } = string.Empty;
        public string state_ref_id { get; set; } = string.Empty;
        public string zone_ref_id { get; set; } = string.Empty;
        public string dist_ref_id { get; set; } = string.Empty;
        public string new_category { get; set; } = string.Empty;
    }

    public class APTGetCustomerInfoResponse
    {
        public string customer_ref_id { get; set; } = string.Empty;
        public string agent_ref_id { get; set; } = string.Empty;
        public string account_type_ref_id { get; set; } = string.Empty;
        public string customer_name { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;
        public string mobile_number { get; set; } = string.Empty;
        public string pin { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string address1 { get; set; } = string.Empty;
        public string address2 { get; set; } = string.Empty;
        public string pincode { get; set; } = string.Empty;
        public string dob { get; set; } = string.Empty;
        public string paytm_ref_id { get; set; } = string.Empty;
        public string balance { get; set; } = string.Empty;

        public string bank_1_current_balance { get; set; } = string.Empty;
        public string bank_1_monthly_transcount { get; set; } = string.Empty;
        public string active_status_ref_id { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public string customer_status { get; set; } = string.Empty;
        public string city { get; set; } = string.Empty;
        public string bank_1_monthly_balance { get; set; } = string.Empty;
        public string new_current_balance { get; set; } = string.Empty;
        public string entity_id { get; set; } = string.Empty;
        public string gender { get; set; } = string.Empty;
        public string state { get; set; } = string.Empty;
        public bool is_internal { get; set; }
    }
    public class GetBankTypeAPIResponse
    {
        public string bank { get; set; } = string.Empty;
        public string bank_type { get; set; } = string.Empty;
    }
    public class IFSCBanksResponse
    {
        public int response_code { get; set; }
        public string response_message { get; set; }
        public int bank_ref_id { get; set; }
        public string bank_name { get; set; }
        public bool imps_available { get; set; }
        public bool is_gramin_bank { get; set; }
        public int imps_status_ref_id { get; set; }
        public int neft_status_ref_id { get; set; }
        public string global_ifsc { get; set; }
        public string four_digit_code { get; set; }
        public string nbin { get; set; }
        public bool aeps_available { get; set; }
        public string aeps_bin { get; set; }
        public string min_digit_account_number { get; set; }
        public string max_digit_account_number { get; set; }
    }
    public class APTGetBanksRequest
    {
        public string bank_type { get; set; }
        public string active_banks { get; set; }
    }

    public class APTInsertOTPRequest
    {
        public string mobile_no { get; set; } = string.Empty;
        public string account_ref_id { get; set; } = string.Empty;
        public string account_type { get; set; } = string.Empty;
        public string otp { get; set; } = string.Empty;
        public string otp_type_ref_id { get; set; } = "1";
    }

    public class SMSResponse
    {
        public string req_id { get; set; }
        public string req_time { get; set; }
        public SMSStatus status { get; set; }
    }

    public class SMSStatus
    {
        public string reason { get; set; }
        public string code { get; set; }
        public string info { get; set; }
    }

    public class ValidateOTPRequest
    {
        public string otp { get; set; } = string.Empty;
        public string otp_state { get; set; } = string.Empty;
        public string mobile_no { get; set; } = string.Empty;
        public string agent_ref_id { get; set; } = string.Empty;
        public bool is_internal { get; set; }
        public string customer_mobile { get; set; } = string.Empty;
        public bool isRetailer { get; set; } = false;
        //public SessionValidation session { get; set; }
    }


    public class ResendOTPRequest
    {
        public string mobile_no { get; set; } = string.Empty;
        public string agent_ref_id { get; set; } = string.Empty;
        public string account_type { get; set; } = string.Empty;
        public bool is_internal { get; set; }
        public int flag { get; set; }
    }

    public class APTUpdateCustomerRequest
    {
        public string mobile_no { get; set; } = string.Empty;
        public string bank_ref_id { get; set; } = string.Empty;
        public string active_status_ref_id { get; set; } = string.Empty;
        public string updated_by { get; set; } = string.Empty;

    }




    public class APTUpdatePayeeRequest
    {
        public string agent_ref_id { get; set; } = string.Empty;
        public string payee_ref_id { get; set; } = string.Empty;
        public string npci_name { get; set; } = string.Empty;
        public string validated { get; set; } = string.Empty;
        public string otp { get; set; } = string.Empty;
        public string otp_state { get; set; } = string.Empty;
        public string mobile_no { get; set; } = string.Empty;
        [JsonIgnore]
        public string bank_ref_id { get; set; } = string.Empty;
        [JsonIgnore]
        public string active_status_ref_id { get; set; } = string.Empty;
        [JsonIgnore]
        public string updated_by { get; set; } = string.Empty;
        public SessionValidation session { get; set; }

    }


    public class APTTransactionRequest
    {

        public string sender_mobile_number { get; set; } = string.Empty;
        public string payee_ref_id { get; set; } = string.Empty;

        public string amount { get; set; } = string.Empty;
        public string payment_transaction_type_refid { get; set; } = string.Empty;
        public string pay_mode_ref_id { get; set; } = string.Empty;
        public string agent_mobile { get; set; } = string.Empty;
        public string account_number_in { get; set; } = string.Empty;
        public string agent_ref_id { get; set; } = string.Empty;
        public string bank_name { get; set; } = string.Empty;
        public string ifsc_code { get; set; } = string.Empty;
        // public SessionValidation session { get; set; }

    }
    public class APTTransactionResponse
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

    public class APTUpdateTransaction
    {
        public string transaction_id { get; set; } = string.Empty;
        public string bank_reference_number { get; set; } = string.Empty;
        public string request { get; set; } = string.Empty;
        public string response { get; set; } = string.Empty;
        public string response_code { get; set; } = string.Empty;
        public string response_description { get; set; } = string.Empty;
        public string trans_status { get; set; } = string.Empty;
        public string beneficiary_name { get; set; } = string.Empty;

    }

    public class BanksListResponse
    {
        public string response_code { get; set; }
        public string response_message { get; set; }
        public int bank_ref_id { get; set; }
        public string bank_branch_ref_id { get; set; }

        public string bank_name { get; set; }
        public bool imps_available { get; set; }
        public bool is_gramin_bank { get; set; }
        public int imps_status_ref_id { get; set; }
        public int neft_status_ref_id { get; set; }
        public string global_ifsc { get; set; }
        public string four_digit_code { get; set; }
        public string nbin { get; set; }
        public bool aeps_available { get; set; }
        public string aeps_bin { get; set; }
        public string min_digit_account_number { get; set; }
        public string max_digit_account_number { get; set; }

        public int state_ref_id { get; set; }
        public int city_ref_id { get; set; }
        public string ifsc_code { get; set; }
        public string bank_branch { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }

    }
    public class APTBeneValidateResponse
    {
        public string customerMobile { get; set; }
        public string beneficiaryName { get; set; }
        public string transactionDate { get; set; }
    }

    public class RefundOTPRequest
    {
        public string otp { get; set; } = string.Empty;
        public string mobile_no { get; set; } = string.Empty;
        public string agent_ref_id { get; set; } = string.Empty;
        public string transaction_id { get; set; } = string.Empty;
        public string payment_type { get; set; } = string.Empty;
    }

    public class PaytmStatusRequest
    {
        [Required]
        public string transactionID { get; set; }

    }




}
