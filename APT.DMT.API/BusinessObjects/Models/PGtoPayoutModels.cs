using System.Text.Json.Serialization;

namespace APT.PaymentServices.API.BusinessObjects.Models
{

    public class APTInsertCustomerRequest
    {
      
        public string mobile_number { get; set; }=string.Empty;
        public string email_id { get; set; } = string.Empty;

        public string name { get; set; } = string.Empty;

        public string pincode { get; set; } = string.Empty;
        public string state { get; set; } = string.Empty;
        public string area { get; set; } = string.Empty;
        public string agent_code { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;

    }

    public class APTGetCustomerInfoRequest
    {
        public string mobile_number { get; set; } = string.Empty;
    }

    public class APTGetCustInfoResponse
    {
        public string customer_ref_id { get; set; } = string.Empty;
        public string email_id { get; set; } = string.Empty;
        public string agent_code { get; set; } = string.Empty;
        public string mobile_number { get; set; } = string.Empty;
        public string agent_name { get; set; } = string.Empty;
    }

    public class APTPGtoPayoutuploadCreditCardRequest
    {
        public string transaction_id { get; set; } = string.Empty;
        public string credit_card_number { get; set; } = string.Empty;
        public string credit_card_image_path { get; set; } = string.Empty;
        public string credit_card_front_filename { get; set; } = string.Empty;
        public string user_id { get; set; } = string.Empty;

    }

    public class APTPGtoPayoutuploadkycRequest
    {
        public string transaction_id { get; set; } = string.Empty;
        public string id_prrof_ref_number { get; set; } = string.Empty;
        public string id_proof_front { get; set; } = string.Empty;
        public string id_proof_back { get; set; } = string.Empty;
        public string id_proof_filepath { get; set; } = string.Empty;
        public string user_id { get; set; } = string.Empty;

    }
}
