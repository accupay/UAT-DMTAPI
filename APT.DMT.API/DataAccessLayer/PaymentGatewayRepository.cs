using APT.DMT.API.BusinessLogic;
using APT.DMT.API.BusinessObjects.Models;
using APT.DMT.API.Businessstrings.Models;
using APT.PaymentServices.API.BusinessObjects.Models;
using Microsoft.VisualBasic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
namespace APT.PaymentServices.API.DataAccessLayer
{
    public class PaymentGatewayRepository
    {
        private readonly string _connString;
        private readonly string _txnconnString;
        private readonly string _masterconnString;
        public static IConfiguration Configuration { get; set; }
        public static LogService _log = new LogService();
        public static CommonService _commonService = new CommonService();
        public PaymentGatewayRepository()
        {
            Configuration = _commonService.GetConfiguration();
            _connString = Configuration.GetConnectionString("DMTDBconnection");
            _masterconnString = Configuration.GetConnectionString("MasterDBConnection");
            _txnconnString = Configuration.GetConnectionString("TransactionDBConnection");
        }

        public DataSet APTPaymentGatewayInsertTraction(InsertPGTransactionRequest Request)
        {

            //default timeout 30 secs
            using (var connection = new SqlConnection(_txnconnString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_InsertCardPaymentTransaction", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("VendorRefID", 1));
                    cmd.Parameters.Add(new SqlParameter("VendorName", "CashFree"));
                    cmd.Parameters.Add(new SqlParameter("AgentRefID", Request.agent_ref_id));
                    cmd.Parameters.Add(new SqlParameter("Currency", "INR"));
                    cmd.Parameters.Add(new SqlParameter("RequestID", ""));
                    cmd.Parameters.Add(new SqlParameter("VendorOrderID", Request.order_id));
                    cmd.Parameters.Add(new SqlParameter("PaymentAmount", Request.transfer_amount));
                    cmd.Parameters.Add(new SqlParameter("ServiceID", 1));
                    cmd.Parameters.Add(new SqlParameter("ServiceDescription", "WalletLoading"));
                    cmd.Parameters.Add(new SqlParameter("Status", 1));
                    cmd.Parameters.Add(new SqlParameter("StatusDescription", "Pending"));
                    cmd.Parameters.Add(new SqlParameter("Remarks", "Transaction Payment Amount"));
                    cmd.Parameters.Add(new SqlParameter("UserRefID", Request.agent_ref_id));
                    cmd.Parameters.Add(new SqlParameter("PGCharges", "0"));
                    cmd.Parameters.Add(new SqlParameter("GSTCharges", "0"));
                    cmd.Parameters.Add(new SqlParameter("ChannelType", "1"));
                    cmd.Parameters.Add(new SqlParameter("AccountTypeRefID", Request.account_type_ref_id));
                    cmd.Parameters.Add(new SqlParameter("topupmoderefid", Request.topup_mode_ref_id));
                    cmd.Parameters.Add(new SqlParameter("MerchantMobileNo", Request.agent_mobile_number));
                    cmd.Parameters.Add(new SqlParameter("MerchantName", Request.agent_name));
                    cmd.Parameters.Add(new SqlParameter("ServiceChannelID", "1"));
                    cmd.Parameters.Add(new SqlParameter("PackageID", ""));
                    cmd.Parameters.Add(new SqlParameter("VendorPaymentSessionID", Request.payment_session_id));
                    cmd.Parameters.Add(new SqlParameter("BankRefID", "7"));
                    //cmd.Parameters.Add(new SqlParameter("serviceid", Request.pg_service_id));
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
                        _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.order_id);

                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public DataSet APTPaymentGatewayUpdateTraction(UpdatePGTransactionRequest Request)
        {

            //default timeout 30 secs
            using (var connection = new SqlConnection(_txnconnString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("Apt_UpdateCardPaymentTransaction", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("CPTransID", Request.cp_transaction_id));
                    cmd.Parameters.Add(new SqlParameter("VendorRefNo", Request.vendor_ref_no));
                    cmd.Parameters.Add(new SqlParameter("VendorPaymentID", Request.vendor_payment_id));
                    cmd.Parameters.Add(new SqlParameter("RefundPaymentID", Request.refund_payment_id));
                    cmd.Parameters.Add(new SqlParameter("Status", Request.status));
                    cmd.Parameters.Add(new SqlParameter("StatusDescription", Request.status_desc));
                    cmd.Parameters.Add(new SqlParameter("PaymentType", Request.payment_type ));
                    cmd.Parameters.Add(new SqlParameter("webhookrespone", Request.webhookrespone));
                    cmd.Parameters.Add(new SqlParameter("UserRefID", Request.agent_ref_id));
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
                        _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.cp_transaction_id);

                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public DataSet APTPaymentGatewayTopup(UpdatePGTransactionRequest Request)
        {

            //default timeout 30 secs
            using (var connection = new SqlConnection(_txnconnString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_TopUpDistributor", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("MobileNo", Request.agent_mobile_number));
                    cmd.Parameters.Add(new SqlParameter("TopupModeRefID", Request.topup_mode_ref_id));
                    cmd.Parameters.Add(new SqlParameter("DepositSlipRefID", "0"));
                    cmd.Parameters.Add(new SqlParameter("BankRefID", Request.bank_ref_id));
                    cmd.Parameters.Add(new SqlParameter("UTRNo", Request.vendor_ref_no));
                    cmd.Parameters.Add(new SqlParameter("UserRefID", Request.agent_ref_id)); 
                    cmd.Parameters.Add(new SqlParameter("TransDate", DateTime.Now.ToString()));
                    cmd.Parameters.Add(new SqlParameter("Amount", Request.transfer_amount));
                    cmd.Parameters.Add(new SqlParameter("TransactionID", Request.cp_transaction_id));
                    cmd.Parameters.Add(new SqlParameter("Remarks", Request.refund_payment_id));
                    cmd.Parameters.Add(new SqlParameter("CardRefID", Request.card_brand));
                    cmd.Parameters.Add(new SqlParameter("CardSubTypeRefID", Request.card_sub_type));
                    
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
                        _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.agent_mobile_number);

                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public DataSet GetPGTransactionDetails(string OrderId)
        {
            //default timeout 30 secs
            using (var connection = new SqlConnection(_txnconnString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_GetCardPaymentTransactionbyOrderId", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("VendorOrderId", OrderId));

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
                        _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, OrderId);

                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

        }
        public DataSet GetPGTransactionDetailsByTransID(string TransactionID)
        {
            using (var connection = new SqlConnection(_txnconnString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_GetCardPaymentTransaction", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("Transactionid", TransactionID));

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
                        _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, TransactionID);

                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }




        }

        public DataSet PGDecider(string MobileNumber, string serviceid)
        {
            //default timeout 30 secs
            using (var connection = new SqlConnection(_masterconnString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("Apt_GETPGBANK", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("Mobilenumber", MobileNumber));
                    cmd.Parameters.Add(new SqlParameter("serviceid", serviceid));
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
                        _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, MobileNumber);

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
