namespace APT.DMT.API.Businessstrings.Models
{
    public class TransactionLedgerReportRequest
    {
        public string from_date { get; set; } = string.Empty;
        public string to_date { get; set; } = string.Empty;
        public string agent_ref_id { get; set; } = string.Empty;
    }
    public class TransactionLedgerReportResponse
    {
        public string transaction_ref_id { get; set; } = string.Empty;
        public string distributor_ref_id { get; set; } = string.Empty;
        public string agent_ref_id { get; set; } = string.Empty;
        public string transaction_date { get; set; } = string.Empty;
        public string opening_balance { get; set; } = string.Empty;
        public string credit { get; set; } = string.Empty;
        public string debit { get; set; } = string.Empty;
        public string closing_balance { get; set; } = string.Empty;
        public string transaction_id { get; set; } = string.Empty;
        public string comments { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public string created_date { get; set; } = string.Empty;
        public string updated_date { get; set; } = string.Empty;
        public string date_key { get; set; } = string.Empty;
    }

    public class GetRefundPaymentTransactionReportRequest
    {
        public string from_date { get; set; } = string.Empty;
        public string to_date { get; set; } = string.Empty;
        public string agent_ref_id { get; set; } = string.Empty;
    }

    public class GetRefundPaymentTransactionReportResponse
    {
        // Root myDeserializedClass = JsonConvert.Deserializestring<Root>(myJsonResponse);

        public string paymenttransactionrefid { get; set; }
        public string paymode { get; set; }
        public string transactionid { get; set; }
        public string transactionstatus { get; set; }
        public string request { get; set; }
        public string response { get; set; }
        public string statuscode { get; set; }
        public string statusdescription { get; set; }
        public string responsecode { get; set; }
        public string responsedescription { get; set; }
        public string displaymessage { get; set; }
        public string bankreferencenumber { get; set; }
        public string amount { get; set; }
        public string customerfee { get; set; }
        public string agentcomm { get; set; }
        public string distcomm { get; set; }
        public string createddate { get; set; }
        public string updateddate { get; set; }
        public string datekey { get; set; }
        public string cashoutdttime { get; set; }
        public string cashoutdatekey { get; set; }
        public string reversaldatetime { get; set; }
        public string reversaldatekey { get; set; }
        public string bankname { get; set; }
        public string customermobileno { get; set; }
        public string customername { get; set; }
        public string payeename { get; set; }
        public string payeeaccountnumer { get; set; }
        public string amobileno { get; set; }
        public string agencyname { get; set; }
        public string agentname { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zone { get; set; }
        public string distmobileno { get; set; }
        public string distributorname { get; set; }
        public string distcompanyname { get; set; }
        public string agentip { get; set; }
        public string latitude { get; set; }
        public string longtitude { get; set; }
        public string channeltype { get; set; }
        public string channeltypedesc { get; set; }
        public string agentcommwithouttds { get; set; }
        public string distcommwithouttds { get; set; }
    }

    public class GetRetailerPaymentTransactionReportRequest
    {
        public string from_date { get; set; } = string.Empty;
        public string to_date { get; set; } = string.Empty;
        public string agent_ref_id { get; set; } = string.Empty;
        public string search_option { get; set; } = string.Empty;
        public string search_value { get; set; } = string.Empty;
    }
    public class GetRetailerPaymentTransactionReportResponse
    {
        public string paymenttransactionrefid { get; set; }
        public string paymode { get; set; }
        public string transactionid { get; set; }
        public string transactionstatus { get; set; }
        public string request { get; set; }
        public string response { get; set; }
        public string statuscode { get; set; }
        public string statusdescription { get; set; }
        public string responsecode { get; set; }
        public string responsedescription { get; set; }
        public string displaymessage { get; set; }
        public string bankreferencenumber { get; set; }
        public string amount { get; set; }
        public string customerfee { get; set; }
        public string agentcomm { get; set; }
        public string distcomm { get; set; }
        public string createddate { get; set; }
        public string updateddate { get; set; }
        public string datekey { get; set; }
        public string cashoutdttime { get; set; }
        public string cashoutdatekey { get; set; }
        public string reversaldatetime { get; set; }
        public string reversaldatekey { get; set; }
        public string bankname { get; set; }
        public string customermobileno { get; set; }
        public string customername { get; set; }
        public string payeename { get; set; }
        public string payeeaccountnumer { get; set; }
        public string amobileno { get; set; }
        public string agencyname { get; set; }
        public string agentname { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zone { get; set; }
        public string distmobileno { get; set; }
        public string distributorname { get; set; }
        public string distcompanyname { get; set; }
        public string agentip { get; set; }
        public string latitude { get; set; }
        public string longtitude { get; set; }
        public string agentcommwithouttds { get; set; }
        public string distcommwithouttds { get; set; }
        public string IFSC { get; set; }
    }

    public class GetRetailerTopupReportRequest
    {
        public string from_date { get; set; } = string.Empty;
        public string to_date { get; set; } = string.Empty;
        public string agent_ref_id { get; set; } = string.Empty;
        public string search_option { get; set; } = string.Empty;
        public string search_value { get; set; } = string.Empty;
    }
    
    public class GetRetailerTopupReportResponse
    {
        public string topup_ref_id { get; set; } = string.Empty;
        public string account_type_ref_id { get; set; } = string.Empty;
        public string account_ref_id { get; set; } = string.Empty;
        public string account_name { get; set; } = string.Empty;
        public string account_mobile_number { get; set; } = string.Empty;
        public string date { get; set; } = string.Empty;
        public string amount { get; set; } = string.Empty;
        public string service_charge { get; set; } = string.Empty;
        public string flat_fee { get; set; } = string.Empty;
        public string dist_service_tax { get; set; } = string.Empty;
        public string UTR_number { get; set; } = string.Empty;
        public string distributor_ref_id { get; set; } = string.Empty;
        public string Retailer_name { get; set; } = string.Empty;
        public string retailer_mobile_number { get; set; } = string.Empty;
        public string t_bankstatement_id { get; set; } = string.Empty;
        public string bankstatement_id { get; set; } = string.Empty;
        public string state { get; set; } = string.Empty;
        public string city { get; set; } = string.Empty;
        public string agent_ref_id { get; set; } = string.Empty;
        public string bank_ref_id { get; set; } = string.Empty;
        public string topup_type_ref_id { get; set; } = string.Empty;
        public string topup_status_ref_id { get; set; } = string.Empty;
        public string approval_reason_ref_id { get; set; } = string.Empty;
        public string maker_ref_id { get; set; } = string.Empty;
        public string topup_mode { get; set; } = string.Empty;
        public string currency_code { get; set; } = string.Empty;
        public string topup_transaction_id { get; set; } = string.Empty;
        public string transaction_id { get; set; } = string.Empty;
        public string approval_transaction_id { get; set; } = string.Empty;
        public string comments { get; set; } = string.Empty;
        public string bank_reference_number { get; set; } = string.Empty;
        public string checker_ref_id { get; set; } = string.Empty;
        public string checker_transdate { get; set; } = string.Empty;
        public string checker_remarks { get; set; } = string.Empty;
        public string created_by_type_ref_id { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public string created_by { get; set; } = string.Empty;
        public string created_date { get; set; } = string.Empty;
        public string updated_by { get; set; } = string.Empty;
        public string updated_date { get; set; } = string.Empty;
        public string new_row { get; set; } = string.Empty;
        public string updated_row { get; set; } = string.Empty;
        public string date_key { get; set; } = string.Empty;
        public string image { get; set; } = string.Empty;
        public string deposit_create_date { get; set; } = string.Empty;
        public string reversal_remarks { get; set; } = string.Empty;
        public string company_name { get; set; } = string.Empty;
        
        public string deposit_slip_ref_id { get; set; } = string.Empty;
        public string channel_ref_id { get; set; } = string.Empty;
        public string pg_Receipt_link { get; set; } = string.Empty;
        public string pg_link { get; set; } = string.Empty;
        public string LinkprocessedDate { get; set; } = string.Empty;
        public string LinkCreateddate { get; set; } = string.Empty;
    }

    public class GetScrollMessageResponse
    {
        public string Message { get; set; }
    }

    public class GetScrollMessageRequest
    {
        public string MobileNumber { get; set; }
    }
    public class GetPGTransactionReportResponse
    {
       
        public string amount { get; set; } = string.Empty;
        public string service_charge { get; set; } = string.Empty;
        public string agent_ref_id { get; set; } = string.Empty;
        public string pg_Receipt_link { get; set; } = string.Empty;
        public string pg_link { get; set; } = string.Empty;
        public string LinkprocessedDate { get; set; } = string.Empty;
        public string LinkCreateddate { get; set; } = string.Empty;
        public string transaction_date { get; set; } = string.Empty;
        public string pg_status { get; set; } = string.Empty;
        public string mobile { get; set; } = string.Empty;
        public string order_id { get; set; } = string.Empty;
        public string payment_id { get; set; } = string.Empty;
        public string transaction_id { get; set; } = string.Empty;
        public string merchant_name { get; set; } = string.Empty;
        public string vendor_payment_session_id { get; set; } = string.Empty;
        public string webhook_response { get; set; } = string.Empty;
        public string total_fees { get; set; } = string.Empty;
        public string card_name { get; set; } = string.Empty;
        public string card_sub_type { get; set; } = string.Empty;
        public string vendor_ref_id { get; set; } = string.Empty;
        public string UTR_number { get; set; } = string.Empty;

    }

    public class GetRetailerCommonDetailsRequest
    {
        public string from_date { get; set; } = string.Empty;
        public string to_date { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public string agent_ref_id { get; set; } = string.Empty;

    }

    public class GetRetailerCommonDetailsResponse
    {
        public string agent_ref_id { get; set; } = string.Empty;
        public string Customer_name { get; set; } = string.Empty;
        public string Customer_mobile { get; set; } = string.Empty;
        public string transaction_id { get; set; } = string.Empty;
        public string transaction_type { get; set; } = string.Empty;
        public string payment_mode { get; set; } = string.Empty;
        public string transaction_amount { get; set; } = string.Empty;
        public string fees { get; set; } = string.Empty;
        public string date_key { get; set; } = string.Empty;
        public string settlement_amount { get; set; } = string.Empty;
        public string opening_balance { get; set; } = string.Empty;
        public string transaction_mode { get; set; } = string.Empty;
        public string created_date { get; set; } = string.Empty;
        public string transaction_status { get; set; } = string.Empty;
        public string closing_balance { get; set; } = string.Empty;

    }

    public class GetRetailerCustomerPayoutTxnReportRequest
    {
        public string from_date { get; set; } = string.Empty;
        public string to_date { get; set; } = string.Empty;
        public string mobile_number { get; set; } = string.Empty;
        public string transaction_status { get; set; } = string.Empty;

    }

    public class GetRetailerCustomerPayoutTxnReportResponse
    {
        public string customer_name { get; set; } = string.Empty;
        public string customer_mobile { get; set; } = string.Empty;
        public string amount { get; set; } = string.Empty;
        public string payee_name { get; set; } = string.Empty;
        public string account_number { get; set; } = string.Empty;
        public string ifsc { get; set; } = string.Empty;
        public string transaction_id { get; set; } = string.Empty;
        public string payment_mode { get; set; } = string.Empty;
        public string payment_category_type { get; set; } = string.Empty;
        public string settlement_type { get; set; } = string.Empty;
        public string bank_name { get; set; } = string.Empty;
        public string branch_name { get; set; } = string.Empty;
        public string transaction_status { get; set; } = string.Empty;

        public string payment_transaction_ref_id { get; set; } = string.Empty;
        public string platform_charge { get; set; } = string.Empty;
        public string settlement_charge { get; set; } = string.Empty;
        public string pg_platform_bank_id { get; set; } = string.Empty;
        public string transaction_status_ref_id { get; set; } = string.Empty;
        public string approval_status_ref_id { get; set; } = string.Empty;
        public string pg_status_ref_id { get; set; } = string.Empty;
        public string remarks { get; set; } = string.Empty;
        public string pg_transaction_id { get; set; } = string.Empty;
        public string approval_date { get; set; } = string.Empty;
        public string pg_updated_date { get; set; } = string.Empty;
        public string customer_ref_id { get; set; } = string.Empty;
        public string payee_ref_id { get; set; } = string.Empty;
        public string agent_code { get; set; } = string.Empty;
        public string created_by { get; set; } = string.Empty;
        public string created_date { get; set; } = string.Empty;
        public string updated_by { get; set; } = string.Empty;
        public string updated_date { get; set; } = string.Empty;
        public string agency_name { get; set; } = string.Empty;
        public string agent_name { get; set; } = string.Empty;
        public string city_ref_id { get; set; } = string.Empty;
        public string city { get; set; } = string.Empty;
        public string state_ref_id { get; set; } = string.Empty;
        public string state { get; set; } = string.Empty;
        public string zone_ref_id { get; set; } = string.Empty;
        public string zone { get; set; } = string.Empty;
        public string dist_ref_id { get; set; } = string.Empty;
        public string dist_mobile_number { get; set; } = string.Empty;
        public string dist_name { get; set; } = string.Empty;
        public string dist_company_name { get; set; } = string.Empty;
        public string request { get; set; } = string.Empty;
        public string response { get; set; } = string.Empty;
        public string date_key { get; set; } = string.Empty;
        public string customer_mobiel_number { get; set; } = string.Empty;
        public string payee_mobile_number { get; set; } = string.Empty;
        public string vendor_ref_id { get; set; } = string.Empty;
        public string vendor_name { get; set; } = string.Empty;
        public string vendor_order_id { get; set; } = string.Empty;
        public string service_id { get; set; } = string.Empty;
        public string service_description { get; set; } = string.Empty;
        public string vendor_payment_session_id { get; set; } = string.Empty;
        public string webhook_response { get; set; } = string.Empty;
        public string vendor_ref_number { get; set; } = string.Empty;
        public string vendor_Payment_id { get; set; } = string.Empty;


        public string bank_reference_number { get; set; } = string.Empty;
        public string id_proof_file_number { get; set; } = string.Empty;
        public string id_proof_image_filepath { get; set; } = string.Empty;
        public string id_proof_front_file { get; set; } = string.Empty;
        public string id_proof_back_file { get; set; } = string.Empty;
        public string credit_card_number { get; set; } = string.Empty;
        public string credit_card_image { get; set; } = string.Empty;
        public string credit_card_image_front { get; set; } = string.Empty;
        public string kyc_status { get; set; } = string.Empty;
        
    }

}
