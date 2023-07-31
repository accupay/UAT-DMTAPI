namespace APT.DMT.API.BusinessObjects
{
    public struct APIResponseCode
    {
        public const string Success = "200";
        public const string Failed = "400";
        public const string DBSuccess = "100";
        public const string RequestNull = "401";
        public const string Exception = "500";
        public const string DatasetNull = "400";
        public const string InvalidRequest = "401";
    }
    public struct APIResponseCodeDesc
    {
        public const string Success = "Success";
        public const string Failed = "Failed";
        public const string DatasetNull = "Connectivity issue. Please contact administrator for further support ";
        public const string RequestNull = "Request is null, Please pass valid request";
        public const string Exception = "An error occured. Please contact administrator for further support";
        public const string InvalidRequest = "Invalid request parameter. Please validate the request";
    }
    public struct APIConstants
    {
        public const string Countrycode = "91";
        public const string BeneDeletion = "BeneDeletion";
    }
}
