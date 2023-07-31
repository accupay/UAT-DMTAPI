using System.Xml.Serialization;

namespace APT.PaymentServices.API.BusinessObjects.Models
{
//    using System.Xml.Serialization;
//    XmlSerializer serializer = new XmlSerializer(typeof(validate));
//     using (StringReader reader = new StringReader(xml))
//     {
//        var test = (validate)serializer.Deserialize(reader);
//}
//}

[XmlRoot(ElementName = "validate")]
    public class validate
    {

        [XmlElement(ElementName = "customer_code")]
        public string customer_code { get; set; }

        [XmlElement(ElementName = "bene_account_no")]
        public string bene_account_no { get; set; }

        [XmlElement(ElementName = "bene_account_ifsc")]
        public string bene_account_ifsc { get; set; }

        [XmlElement(ElementName = "bene_full_name")]
        public string bene_full_name { get; set; }

        [XmlElement(ElementName = "transfer_type")]
        public string transfer_type { get; set; }

        [XmlElement(ElementName = "transfer_unique_no")]
        public string transfer_unique_no { get; set; }

        [XmlElement(ElementName = "transfer_timestamp")]
        public DateTime transfer_timestamp { get; set; }

        [XmlElement(ElementName = "transfer_ccy")]
        public string transfer_ccy { get; set; }

        [XmlElement(ElementName = "transfer_amt")]
        public double transfer_amt { get; set; }

        [XmlElement(ElementName = "rmtr_account_no")]
        public double rmtr_account_no { get; set; }

        [XmlElement(ElementName = "rmtr_account_ifsc")]
        public string rmtr_account_ifsc { get; set; }

        [XmlElement(ElementName = "rmtr_account_type")]
        public string rmtr_account_type { get; set; }

        [XmlElement(ElementName = "rmtr_full_name")]
        public string rmtr_full_name { get; set; }

        [XmlElement(ElementName = "rmtr_address")]
        public string rmtr_address { get; set; }

        [XmlElement(ElementName = "attempt_no")]
        public int attempt_no { get; set; }

        [XmlElement(ElementName = "rmtr_to_bene_note")]
        public string rmtr_to_bene_note { get; set; }
    }

    // using System.Xml.Serialization;
    // XmlSerializer serializer = new XmlSerializer(typeof(ValidateResponse));
    // using (StringReader reader = new StringReader(xml))
    // {
    //    var test = (ValidateResponse)serializer.Deserialize(reader);
    // }

    [XmlRoot(ElementName = "validateResponse")]
    public class EcollectYesBankValidateResponse
    {

        [XmlElement(ElementName = "decision")]
        public string decision { get; set; }
    }

    // using System.Xml.Serialization;
    // XmlSerializer serializer = new XmlSerializer(typeof(Notify));
    // using (StringReader reader = new StringReader(xml))
    // {
    //    var test = (Notify)serializer.Deserialize(reader);
    // }

    [XmlRoot(ElementName = "notify")]
    public class notify
    {

        [XmlElement(ElementName = "customer_code")]
        public string customer_code { get; set; }

        [XmlElement(ElementName = "bene_account_no")]
        public string bene_account_no { get; set; }

        [XmlElement(ElementName = "bene_account_ifsc")]
        public string bene_account_ifsc { get; set; }

        [XmlElement(ElementName = "bene_full_name")]
        public string bene_full_name { get; set; }

        [XmlElement(ElementName = "transfer_type")]
        public string transfer_type { get; set; }

        [XmlElement(ElementName = "transfer_unique_no")]
        public string transfer_unique_no { get; set; }

        [XmlElement(ElementName = "transfer_timestamp")]
        public DateTime transfer_timestamp { get; set; }

        [XmlElement(ElementName = "transfer_ccy")]
        public string transfer_ccy { get; set; }

        [XmlElement(ElementName = "transfer_amt")]
        public double transfer_amt { get; set; }

        [XmlElement(ElementName = "rmtr_account_no")]
        public double rmtr_account_no { get; set; }

        [XmlElement(ElementName = "rmtr_account_ifsc")]
        public string rmtr_account_ifsc { get; set; }

        [XmlElement(ElementName = "rmtr_account_type")]
        public string rmtr_account_type { get; set; }

        [XmlElement(ElementName = "rmtr_full_name")]
        public string rmtr_full_name { get; set; }

        [XmlElement(ElementName = "rmtr_address")]
        public object rmtr_address { get; set; }

        [XmlElement(ElementName = "rmtr_to_bene_note")]
        public string rmtr_to_bene_note { get; set; }

        [XmlElement(ElementName = "attempt_no")]
        public int attempt_no { get; set; }

        [XmlElement(ElementName = "status")]
        public string status { get; set; }

        [XmlElement(ElementName = "credit_acct_no")]
        public double credit_acct_no { get; set; }

        [XmlElement(ElementName = "credited_at")]
        public DateTime credited_at { get; set; }
    }

    // using System.Xml.Serialization;
    // XmlSerializer serializer = new XmlSerializer(typeof(NotifyResult));
    // using (StringReader reader = new StringReader(xml))
    // {
    //    var test = (NotifyResult)serializer.Deserialize(reader);
    // }

    [XmlRoot(ElementName = "notifyResult")]
    public class NotifyResult
    {

        [XmlElement(ElementName = "result")]
        public string result { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class ECollectYesbankRequest
    {
        public ECollectYesbankRequestValidate validate { get; set; }
    }

    public class ECollectYesbankRequestValidate
    {
        public string customer_code { get; set; }
        public string bene_account_no { get; set; }
        public string bene_account_ifsc { get; set; }
        public string bene_full_name { get; set; }
        public string transfer_type { get; set; }
        public string transfer_unique_no { get; set; }
        public string transfer_timestamp { get; set; }
        public string transfer_ccy { get; set; }
        public string transfer_amt { get; set; }
        public string rmtr_account_no { get; set; }
        public string rmtr_account_ifsc { get; set; }
        public string rmtr_account_type { get; set; }
        public string rmtr_full_name { get; set; }
        public string rmtr_address { get; set; }
        public string rmtr_to_bene_note { get; set; }
        public int attempt_no { get; set; }
    }

    public class ECollectNewResponse
    {
        public ECollectNewResponseValidateResponse validateResponse {get;set;}
    }

    public class ECollectNewResponseValidateResponse
    {
        public string decision { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class YesbankECNewRequestNotify
    {
        public string customer_code { get; set; }
        public string bene_account_no { get; set; }
        public string bene_account_ifsc { get; set; }
        public string bene_full_name { get; set; }
        public string transfer_type { get; set; }
        public string transfer_unique_no { get; set; }
        public string transfer_timestamp { get; set; }
        public string transfer_ccy { get; set; }
        public string transfer_amt { get; set; }
        public string rmtr_account_no { get; set; }
        public string rmtr_account_ifsc { get; set; }
        public string rmtr_account_type { get; set; }
        public string rmtr_full_name { get; set; }
        public string rmtr_to_bene_note { get; set; }
        public int attempt_no { get; set; }
        public string status { get; set; }
        public string credit_acct_no { get; set; }
        public string credited_at { get; set; }
    }

    public class YesbankECNewRequest
    {
        public YesbankECNewRequestNotify notify { get; set; }
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class YesbankECNewNotifyResult
    {
        public string result { get; set; }
    }

    public class YesbankECNewResponse
    {
        public YesbankECNewNotifyResult notifyResult { get; set; }
    }
    public class ECollectDataInsertRequest
    {
        public string IFSCCode { get; set; }
        public string AccountNumber { get; set; }
        public string RemitterName { get; set; }
        public string Amount { get; set; }
        public string MobileNumber { get; set; }
    }
    public class ECOllectAccountAdditionRequest
    {
        public string agent_mobile { get; set; }
        public string bank_account_number { get; set; }
        public string account_holder_name { get; set; }
        public string bank_name { get; set; }
        public string bank_ref_id { get; set; }
        public string ifsc_code { get; set; }
        public string bank_file_name { get; set; }
        public string bank_copy_file { get; set; }
    }
    public class ECOllectAccountAdditionResponse
    {
        public string bank_account_id { get; set; }
        public string account_ref_id { get; set; }
       
    }

    public class ECOllectViewAccountRequest
    {
        public string agent_ref_id { get; set; }

    }

    public class ECOllectViewAccountResponse
    {
        public string agent_mobile { get; set; }
        public string agent_name { get; set; }
        public string agency_name { get; set; }
        public string bank_name { get; set; }
        public string Bank_account_number { get; set; }
        public string account_holder_name { get; set; }
        public string ifsc_code { get; set; }
        public string bank_copy_file { get; set; }
        public string bank_account_id { get; set; }
        public string account_validation_status { get; set; }
        public string transfer_amount { get; set; }
        public string paymode { get; set; }
        public string created_date { get; set; }
        public string updated_date { get; set; }
        public string npci_name { get; set; }
        public string name_match_status { get; set; }
        
    }

    public class DepositSlipUploadRequest
    {
        public string account_type_ref_id { get; set; }
        public string account_ref_id { get; set; }
        public string mobile_number { get; set; }
        public string bank_transaction_id { get; set; }
        public string amount { get; set; }
        public string deposited_date { get; set; }
        public string topup_type_refid { get; set; }
        public string deposited_bank { get; set; }
        public string image { get; set; }
        public string comments { get; set; }

    }

    public class DepositSlipUploadResponse
    {
        public string deposit_slip_refid { get; set; }

    }
}
