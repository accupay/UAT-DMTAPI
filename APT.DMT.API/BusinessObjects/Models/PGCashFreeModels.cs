namespace APT.PaymentServices.API.BusinessObjects.Models
{

    public class CustomerDetails
    {
        public string customer_id { get; set; }
        public string customer_email { get; set; }
        public string customer_phone { get; set; }
    }

    public class OrderMeta
    {
        public string return_url { get; set; }
        public string notify_url { get; set; }
    }



    public class CashFreePGCreateOrderRequest
    {
        public string order_id { get; set; }
        public string order_amount { get; set; }
        public string order_currency { get; set; }
        public CustomerDetails customer_details { get; set; }
        public OrderMeta order_meta { get; set; }
    }

    public class CashFreeErrorClass
    {
        public string message { get; set; }
        public string code { get; set; }
        public string type { get; set; }
    }


    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class CashFreeCustomerDetails
    {
        public string customer_id { get; set; }
        public object customer_name { get; set; }
        public string customer_email { get; set; }
        public string customer_phone { get; set; }
    }

    public class CashFreeOrderMeta
    {
        public string return_url { get; set; }
        public string notify_url { get; set; }
        public string payment_methods { get; set; }
    }

    public class CashFreeOrderTags
    {
        public string newKey { get; set; }
    }

    public class Payments
    {
        public string url { get; set; }
    }

    public class Refunds
    {
        public string url { get; set; }
    }

    public class CashFreeOrderDetailsResponse
    {
        public int cf_order_id { get; set; }
        public DateTime created_at { get; set; }
        public CashFreeCustomerDetails customer_details { get; set; }
        public string entity { get; set; }
        public string order_amount { get; set; }
        public string order_currency { get; set; }
        public DateTime order_expiry_time { get; set; }
        public string order_id { get; set; }
        public CashFreeOrderMeta order_meta { get; set; }
        public object order_note { get; set; }
        public List<object> order_splits { get; set; }
        public string order_status { get; set; }
        public CashFreeOrderTags order_tags { get; set; }
        public string payment_session_id { get; set; }
        public Payments payments { get; set; }
        public Refunds refunds { get; set; }
        public Settlements settlements { get; set; }
        public object terminal_data { get; set; }
    }

    public class Settlements
    {
        public string url { get; set; }
    }


    public class PaymentDetailsPaymentMethod
    {
        public PaymentDetailsCard card { get; set; }
        public PaymentDetailsUpi upi { get; set; }
    }

    public class PaymentDetailsCard
    {
        public string channel { get; set; }
        public string card_number { get; set; }
        public string card_network { get; set; }
        public string card_type { get; set; }
        public string card_country { get; set; }
        public string card_bank_name { get; set; }
        public string card_display { get; set; }
    }
    public class ErrorDetails
    {
        public string error_code { get; set; }
        public string error_description { get; set; }
        public string error_reason { get; set; }
        public string error_source { get; set; }
        public string error_code_raw { get; set; }
        public string error_description_raw { get; set; }
    }
    public class PaymentDetailsResponse
    {
        public ErrorDetails error_details { get; set; }

        public string auth_id { get; set; }
        public string authorization { get; set; }
        public string bank_reference { get; set; }
        public string cf_payment_id { get; set; }
        public string entity { get; set; }
        public bool is_captured { get; set; }
        public string order_amount { get; set; }
        public string order_id { get; set; }
        public string payment_amount { get; set; }
        public DateTime payment_completion_time { get; set; }
        public string payment_currency { get; set; }
        public string payment_gateway_details { get; set; }
        public string payment_group { get; set; }
        public string payment_message { get; set; }
        public PaymentDetailsPaymentMethod payment_method { get; set; }
        public List<object> payment_offers { get; set; }
        public string payment_status { get; set; }
        public DateTime payment_time { get; set; }
    }

    public class PaymentDetailsUpi
    {
        public string channel { get; set; }
        public string upi_id { get; set; }
    }


}
