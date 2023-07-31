namespace APT.DMT.API.BusinessObjects.Models
{
    public class SessionValidation
    {
        public string SID { get; set; } = string.Empty;
        public string MacID { get; set; } = string.Empty;
    }
    public class LoginResponse
    {
        public string session_id { get; set; } = string.Empty;
        public string token_id { get; set; } = string.Empty;
        public string SID { get; set; } = string.Empty;
    }

    public class APTGenericResponse
    {
        public string response_code { get; set; } = string.Empty;
        public string response_message { get; set; } = string.Empty;
        public LoginResponse session_details { get; set; }
        public object data { get; set; }
    }

    public class TopupInfoResponse
    {
        public string bank_name { get; set; } = string.Empty;
        public string account_number { get; set; } = string.Empty;
        public string ifsc_code { get; set; } = string.Empty;

    }

    public class DistributorInfo
    {
        public string distributor_info { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string mobile_number { get; set; } = string.Empty;
    }

    public class StaffInfo
    {
        public string staff_info { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string mobile_number { get; set; } = string.Empty;
    }

    public class CompanyInfo
    {
        public string company_info { get; set; } = string.Empty;
        public string email_id { get; set; } = string.Empty;
        public string mobile_number { get; set; } = string.Empty;
    }

    public class SupportInfoResponse
    {
        public DistributorInfo dist_info { get; set; }
        public StaffInfo staff_info { get; set; } 
        public CompanyInfo company_info { get; set; }
    }
}
