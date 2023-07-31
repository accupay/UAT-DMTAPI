using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace APT.PaymentServices.API.BusinessObjects.Models
{
    public class APTGetAllPayeeCustomerAppReq
    {
        public string Customer_ref_id { get; set; }
    }

    public class APTInsertPayeeCustomerAppReq
    {
        public string mobile_number { get; set; }
        public string payee_name { get; set; }
        public string mail_id { get; set; }
        public string account_number { get; set; }
        public string account_type { get; set; }
        public string ifsc_code { get; set; }
        public string upi_id { get; set; }
        public string gst_number { get; set; }
        public string customer_ref_id { get; set; }
        public string Customer_mobile_number { get; set; }
        public string created_by { get; set; }
        public string bank_name { get; set; }
        public string branch_name { get; set; }
       
    }

    public class APTGetAllPayeeCustAppResponse
    {
        public string payee_ref_id { get; set; } = string.Empty;
        public string payee_name { get; set; } = string.Empty;
        public string account_number { get; set; } = string.Empty;
        public string mobile_no { get; set; } = string.Empty;
        public string bank_ref_id { get; set; } = string.Empty;
        public string bank_name { get; set; } = string.Empty;
        public string ifsc_code { get; set; } = string.Empty;
        public string mail_id { get; set; } = string.Empty;
        public string account_type { get; set; } = string.Empty;
        public string branch_name { get; set; } = string.Empty;


    }

    public class APTDashboardRequest
    {
        public string mobile_number { get; set; }

    }

    public class APTDashboardResponse
    {
        public string settlement_balance { get; set; }

        public List<APTDashboardReport> last5txns { get; set; }

    }
     public class APTDashboardReport
    {
        public string transaction_id { get; set; }
        public string created_date { get; set; }
        public string transaction_date { get; set; }
        public string bank_name { get; set; }
        public string branch_name { get; set; }
        public string account_number { get; set; }
        public string customer_name { get; set; }
        public string payee_name { get; set; }
        public string customer_mobile_number { get; set; }
        public string payee_mobile_number { get; set; }
    }

    public class APTCustomerAppTransactionRequest
    {
        public string payment_transaction_type_ref_id { get; set; }
        public string Pay_mode_ref_id { get; set; }
        public string payee_ref_id { get; set; }
        public string remarks { get; set; }
        public string account_number { get; set; }
        public string customer_ref_id { get; set; }
        public string ifsc { get; set; }
        public string channel_type { get; set; }
        public string amount { get; set; }
        public string payment_category_type { get; set; }
        public string settlement_type { get; set; }
        public string payment_mode { get; set; }
        public string agent_ref_id { get; set; }
        public string user_ref_id { get; set; }
        public string agent_mobile { get; set; }
        public string bank_name { get; set; }
        public string branch_name { get; set; }
        public string vendor_payment_sessionid { get; set; }
        public string vendor_ref_id { get; set; }
        public string vendor_name { get; set; }
        public string pg_transaction_id { get; set; }
        public string bank_type_id { get; set; }
        public string pg_plateform_bank_id { get; set; }
        public string customer_mobile_number { get; set; }
    }


}
