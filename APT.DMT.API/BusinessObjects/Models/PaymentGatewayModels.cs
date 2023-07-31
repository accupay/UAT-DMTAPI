using System.Text.Json.Serialization;

namespace APT.PaymentServices.API.BusinessObjects.Models
{
    public class GetTransactionDetails
    {
        public string transaction_id { get; set; } = string.Empty;
    }
    public class TransactionDatasetResponse
    {
        public List<Table> Table { get; set; }
    }

    public class Table
    {
        public int CPTransID { get; set; }
        public int VendorRefID { get; set; }
        public string VendorName { get; set; }
        public int AgentRefID { get; set; }
        public string Currency { get; set; }
        public int RequestID { get; set; }
        public string VendorOrderID { get; set; }
        public double PaymentAmount { get; set; }
        public object VendorRefNo { get; set; }
        public object VendorPaymentID { get; set; }
        public object RefundPaymentID { get; set; }
        public string PNBTransactionID { get; set; }
        public object PaymentType { get; set; }
        public int ServiceID { get; set; }
        public string ServiceDescription { get; set; }
        public int Status { get; set; }
        public string StatusDescription { get; set; }
        public string Remarks { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public object UpdatedDate { get; set; }
        public object UpdatedBy { get; set; }
        public int Datekey { get; set; }
        public double PGCharges { get; set; }
        public double GSTCharges { get; set; }
        public int ChannelType { get; set; }
        public int AccountTypeRefId { get; set; }
        public int Modetyperefid { get; set; }
        public object Modetype { get; set; }
        public object Requeryrefid { get; set; }
        public object requerystatus { get; set; }
        public object Request { get; set; }
        public object Response { get; set; }
        public string MerchantMobileno { get; set; }
        public string MerchantName { get; set; }
        public int ServiceChannelID { get; set; }
        public int PackageID { get; set; }
        public string VendorPaymentSessionID { get; set; }
        public int Bankrefid { get; set; }
        public object webhookrespone { get; set; }
    }
    public class AcquirerData
    {
        public string auth_code { get; set; }
    }

    public class Attributes
    {
        public string id { get; set; }
        public string entity { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string status { get; set; }
        public string order_id { get; set; }
        public string invoice_id { get; set; }
        public string international { get; set; }
        public string method { get; set; }
        public string amount_refunded { get; set; }
        public object refund_status { get; set; }
        public string captured { get; set; }
        public string description { get; set; }
        public string card_id { get; set; }
        public Card card { get; set; }
        public string bank { get; set; }
        public object wallet { get; set; }
        public object vpa { get; set; }
        public string email { get; set; }
        public string contact { get; set; }
        public Notes notes { get; set; }
        public string fee { get; set; }
        public string tax { get; set; }
        public string error_code { get; set; }
        public string error_description { get; set; }
        public string error_source { get; set; }
        public string error_step { get; set; }
        public string error_reason { get; set; }
        public AcquirerData acquirer_data { get; set; }
        public string created_at { get; set; }
        public string authorized_at { get; set; }
        public string auto_captured { get; set; }
        public string captured_at { get; set; }
        public string late_authorized { get; set; }
    }

    public class Card
    {
        public string id { get; set; }
        public string entity { get; set; }
        public string name { get; set; }
        public string last4 { get; set; }
        public string network { get; set; }
        public string type { get; set; }
        public string issuer { get; set; }
        public string international { get; set; }
        public string emi { get; set; }
        public string sub_type { get; set; }
        public object token_iin { get; set; }
    }

    public class Notes
    {
        public string address { get; set; }
    }

    public class PaymentDetails
    {
        public Attributes Attributes { get; set; }
    }
    public class InsertPGTransactionRequest
    {
        public string agent_ref_id { get; set; } = string.Empty;
        public string channel_type_ref_id { get; set; } = string.Empty;
        public string account_type_ref_id { get; set; } = string.Empty;
        public string agent_mobile_number { get; set; } = string.Empty;
        public string agent_name { get; set; } = string.Empty;
        public int pg_service_id { get; set; }
        [JsonIgnore]
        public string customer_id { get; set; } = string.Empty;
        public string customer_phone { get; set; } = string.Empty;
        public string transfer_amount { get; set; } = string.Empty;
        [JsonIgnore]
        public string order_id { get; set; } = string.Empty;
        [JsonIgnore]
        public string payment_session_id { get; set; } = string.Empty;
        [JsonIgnore]
        public string topup_mode_ref_id { get; set; } = string.Empty;
    }
    public class UpdatePGTransactionRequest
    {
        public string order_id { get; set; } = string.Empty;
        public string bank_ref_id { get; set; } = string.Empty;
        [JsonIgnore]
        public string vendor_ref_no { get; set; } = string.Empty;
        public string cp_transaction_id { get; set; } = string.Empty;
       
        public string vendor_payment_id { get; set; } = string.Empty;
        [JsonIgnore]
        public string refund_payment_id { get; set; } = string.Empty;
        [JsonIgnore]
        public string status { get; set; } = string.Empty;
        [JsonIgnore]
        public string status_desc { get; set; } = string.Empty;
        [JsonIgnore]
        public string payment_type { get; set; } = string.Empty;
        public string agent_ref_id { get; set; } = string.Empty;
        public string agent_mobile_number { get; set; } = string.Empty;
        public string account_type_ref_id { get; set; } = string.Empty;
        public string agent_name { get; set; } = string.Empty;
        [JsonIgnore]
        public string transfer_amount { get; set; } = string.Empty;

        [JsonIgnore]

        public string topup_mode_ref_id { get; set; } = string.Empty;

        public string card_sub_type { get; set; } = string.Empty;
        public string card_brand { get; set; } = string.Empty;
        [JsonIgnore]
        public string webhookrespone { get; set; } = string.Empty;
    }

    public class InsertPGTransactionResponse
    {
        public string payment_url { get; set; } = string.Empty;
        public string cp_transaction_id { get; set; } = string.Empty;
        public string order_id { get; set; } = string.Empty;
        public string vendor_session_id { get; set; } = string.Empty;
        public string bank_id { get; set; } = string.Empty;
        public string param1 { get; set; } = string.Empty;
    }
    //--Webhook
    public class WebhookReqCard
    {
        public object channel { get; set; }
        public string card_number { get; set; }
        public string card_network { get; set; }
        public string card_type { get; set; }
        public string card_sub_type { get; set; }
        public string card_country { get; set; }
        public string card_bank_name { get; set; }
    }

    public class WebhookReqCustomerDetails
    {
        public string customer_name { get; set; }
        public string customer_id { get; set; }
        public string customer_email { get; set; }
        public string customer_phone { get; set; }
    }

    public class WebhookReqData
    {
        public WebhookReqOrder order { get; set; }
        public WebhookReqPayment payment { get; set; }
        public WebhookReqCustomerDetails customer_details { get; set; }
    }

    public class WebhookReqNetbanking
    {
        public object channel { get; set; }
        public string netbanking_bank_code { get; set; }
        public string netbanking_bank_name { get; set; }
    }

    public class WebhookReqOrder
    {
        public string order_id { get; set; }
        public double order_amount { get; set; }
        public string order_currency { get; set; }
        public object order_tags { get; set; }
    }

    public class WebhookReqPayment
    {
        public int cf_payment_id { get; set; }
        public string payment_status { get; set; }
        public double payment_amount { get; set; }
        public string payment_currency { get; set; }
        public string payment_message { get; set; }
        public DateTime payment_time { get; set; }
        public string bank_reference { get; set; }
        public object auth_id { get; set; }
        public WebhookReqPaymentMethod payment_method { get; set; }
        public string payment_group { get; set; }
    }

    public class WebhookReqPaymentMethod
    {
        public WebhookReqCard card { get; set; }
        public WebhookReqNetbanking netbanking { get; set; }
        public WebhookReqUpi upi { get; set; }
    }

    public class WebhookReqCashFree
    {
        public WebhookReqData data { get; set; }
        public DateTime event_time { get; set; }
        public string type { get; set; }
    }

    public class WebhookReqUpi
    {
        public object channel { get; set; }
        public string upi_id { get; set; }
    }

    public class RazorpayServiceRequest
    {
        public string OrderId { get; set; }
    }

    //public class GetRPPaymentByOrderAcquirerData
    //{
    //    public object auth_code = new object();
    //}

    //public class GetRPPaymentByOrderItem
    //{
    //    public string id { get; set; }
    //    public string entity { get; set; }
    //    public int amount { get; set; }
    //    public string currency { get; set; }
    //    public string status { get; set; }
    //    public string order_id { get; set; }
    //    public object invoice_id = new object();
    //    public bool international { get; set; }
    //    public string method { get; set; }
    //    public int amount_refunded { get; set; }
    //    public object refund_status = new object();
    //    public bool captured { get; set; }
    //    public string description { get; set; }
    //    public string card_id { get; set; }
    //    public object bank = new object();
    //    public object wallet = new object();
    //    public object vpa = new object();
    //    public string email { get; set; }
    //    public string contact { get; set; }
    //    public string customer_id { get; set; }
    //    public string token_id { get; set; }
    //    public List<object> notes = new List<object>();
    //    public int fee { get; set; }
    //    public int tax { get; set; }
    //    public object error_code = new object();
    //    public object error_description = new object();
    //    public object error_source = new object();
    //    public object error_step = new object();
    //    public object error_reason = new object();
    //    public GetRPPaymentByOrderAcquirerData acquirer_data = new GetRPPaymentByOrderAcquirerData();
    //    public int created_at { get; set; }
    //}

    //public class GetRPPaymentByOrderResponse
    //{
    //    public string entity { get; set; }
    //    public int count { get; set; }
    //    public List<GetRPPaymentByOrderItem> items = new List<GetRPPaymentByOrderItem>();
    //}

    public class GEtRPAcquirerData
    {
        public string auth_code { get; set; }
        public string rrn { get; set; }
    }

    public class GEtRPCard
    {
        public string id { get; set; }
        public string entity { get; set; }
        public string name { get; set; }
        public string last4 { get; set; }
        public string network { get; set; }
        public string type { get; set; }
        public string issuer { get; set; }
        public bool international { get; set; }
        public bool emi { get; set; }
        public string sub_type { get; set; }
        public object token_iin { get; set; }
    }

    public class Item
    {
        public string id { get; set; }
        //public string entity { get; set; }
        //public int amount { get; set; }
        //public string currency { get; set; }
        //public string status { get; set; }
        public string order_id { get; set; }
       // public object invoice_id { get; set; }
        //public bool international { get; set; }
        //public string method { get; set; }
        //public int amount_refunded { get; set; }
        //public object refund_status { get; set; }
        public bool captured { get; set; }
        //public string description { get; set; }
        //public string card_id { get; set; }
        //public GEtRPCard card { get; set; }
        //public object bank { get; set; }
        //public object wallet { get; set; }
       // public object vpa { get; set; }
       // public string email { get; set; }
       // public string contact { get; set; }
       // public GEtRPNotes notes { get; set; }
       // public int fee { get; set; }
       // public int tax { get; set; }
        //public object error_code { get; set; }
       // public object error_description { get; set; }
       // public object error_source { get; set; }
       // public object error_step { get; set; }
       // public object error_reason { get; set; }
        //public AcquirerData acquirer_data { get; set; }
       // public int created_at { get; set; }
       // public int authorized_at { get; set; }
       // public bool auto_captured { get; set; }
       // public int captured_at { get; set; }
       // public bool late_authorized { get; set; }
    }

    public class GEtRPNotes
    {
        public string address { get; set; }
    }

    public class GetRPPaymentByOrderResponse
    {
        public string entity { get; set; }
        public int count { get; set; }
        public List<Item> items { get; set; }
    }

}
