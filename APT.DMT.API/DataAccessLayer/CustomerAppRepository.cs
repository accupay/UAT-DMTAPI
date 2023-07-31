using APT.DMT.API.BusinessLogic;
using APT.DMT.API.BusinessObjects.Models;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using APT.PaymentServices.API.BusinessObjects.Models;

namespace APT.PaymentServices.API.DataAccessLayer
{
    public class CustomerAppRepository
    {
        private readonly string _connString;
        private readonly string _txnconnString;
        private readonly string _masterconnString;
        public static IConfiguration Configuration { get; set; }
        public static LogService _log = new LogService();
        public static CommonService _commonService = new CommonService();

        public CustomerAppRepository()
        {
            Configuration = _commonService.GetConfiguration();
            _connString = Configuration.GetConnectionString("DMTDBconnection");
            _masterconnString = Configuration.GetConnectionString("MasterDBConnection");
            _txnconnString = Configuration.GetConnectionString("TransactionDBConnection");
        }

        public DataSet APTGetAllPayee(APTGetAllPayeeCustomerAppReq Request)
        {

            //default timeout 30 secs
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_GetPayeeMaster", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("CustomerRefID ", Request.Customer_ref_id));
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var dataAdaper = new SqlDataAdapter(cmd);
                        DataSet dataSet = new DataSet();
                        dataAdaper.Fill(dataSet);

                        return dataSet;
                    }
                    catch (Exception ex)
                    {
                        _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.Customer_ref_id);

                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public DataSet APTInsertPayee(APTInsertPayeeCustomerAppReq Request)
        {

            //default timeout 30 secs
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_INSPayeeMaster", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("MobileNumber ", Request.mobile_number));
                    cmd.Parameters.Add(new SqlParameter("PayeeName ", Request.payee_name));
                    cmd.Parameters.Add(new SqlParameter("EmailID ", Request.mail_id));
                    cmd.Parameters.Add(new SqlParameter("AccountNumber ", Request.account_number));
                    cmd.Parameters.Add(new SqlParameter("AccountType ", Request.account_type));
                    cmd.Parameters.Add(new SqlParameter("IFSCCode ", Request.ifsc_code));
                    cmd.Parameters.Add(new SqlParameter("UPIID ", Request.upi_id));
                    cmd.Parameters.Add(new SqlParameter("GSTNumber ", Request.gst_number));
                    cmd.Parameters.Add(new SqlParameter("CustomerRefID ", Request.customer_ref_id));
                    cmd.Parameters.Add(new SqlParameter("CustomerMobileNo ", Request.Customer_mobile_number));
                    cmd.Parameters.Add(new SqlParameter("CreatedBy ", Request.created_by));
                    cmd.Parameters.Add(new SqlParameter("BankName ", Request.bank_name));
                    cmd.Parameters.Add(new SqlParameter("BranchName ", Request.branch_name));
                    
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var dataAdaper = new SqlDataAdapter(cmd);
                        DataSet dataSet = new DataSet();
                        dataAdaper.Fill(dataSet);

                        return dataSet;
                    }
                    catch (Exception ex)
                    {
                        _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.Customer_mobile_number);

                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public DataSet APTDashboardAPI(APTDashboardRequest Request)
        {

            //default timeout 30 secs
            using (var connection = new SqlConnection(_txnconnString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_DashboardCustomerPayout", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("MobileNumber ", Request.mobile_number));

                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var dataAdaper = new SqlDataAdapter(cmd);
                        DataSet dataSet = new DataSet();
                        dataAdaper.Fill(dataSet);

                        return dataSet;
                    }
                    catch (Exception ex)
                    {
                        _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.mobile_number);

                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public DataSet APTCustomerPaymentTransaction(APTCustomerAppTransactionRequest Request)
        {

            //default timeout 30 secs
            using (var connection = new SqlConnection(_txnconnString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_TransferMoneytoPayee_CustomerPayout", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("PaymentTransactionTypeRefID", Request.payment_transaction_type_ref_id));
                    cmd.Parameters.Add(new SqlParameter("PayModeRefID", Request.Pay_mode_ref_id));
                    cmd.Parameters.Add(new SqlParameter("PayeeRefID", Request.payee_ref_id));
                    cmd.Parameters.Add(new SqlParameter("Remarks", Request.remarks));
                    cmd.Parameters.Add(new SqlParameter("Accountnumerin", Request.account_number));
                    cmd.Parameters.Add(new SqlParameter("CustomerRefID", Request.customer_ref_id));
                    cmd.Parameters.Add(new SqlParameter("IFSCCode", Request.ifsc));
                    cmd.Parameters.Add(new SqlParameter("ChannelType", Request.channel_type));
                    cmd.Parameters.Add(new SqlParameter("Amount", Request.amount));
                    cmd.Parameters.Add(new SqlParameter("PGPlatformBankID", Request.pg_plateform_bank_id));
                    cmd.Parameters.Add(new SqlParameter("PaymentCategoryType", Request.payment_category_type));
                    cmd.Parameters.Add(new SqlParameter("SettlementType", Request.settlement_type));
                    cmd.Parameters.Add(new SqlParameter("PaymentMode", Request.payment_mode));
                    cmd.Parameters.Add(new SqlParameter("UserRefID", Request.user_ref_id));
                    cmd.Parameters.Add(new SqlParameter("Agentmobile", Request.agent_mobile));
                    cmd.Parameters.Add(new SqlParameter("BankName", Request.bank_name));
                    cmd.Parameters.Add(new SqlParameter("BranchName", Request.branch_name));
                    cmd.Parameters.Add(new SqlParameter("VendorPaymentSessionID", Request.vendor_payment_sessionid));
                    cmd.Parameters.Add(new SqlParameter("VendorRefID", Request.vendor_ref_id));
                    cmd.Parameters.Add(new SqlParameter("VendorName", Request.vendor_name));
                    cmd.Parameters.Add(new SqlParameter("PGTransactionID", Request.pg_transaction_id));
                    cmd.Parameters.Add(new SqlParameter("BankTypeId", Request.bank_type_id));
                    

                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var dataAdaper = new SqlDataAdapter(cmd);
                        DataSet dataSet = new DataSet();
                        dataAdaper.Fill(dataSet);

                        return dataSet;
                    }
                    catch (Exception ex)
                    {
                        _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.agent_mobile);

                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

    }
}
