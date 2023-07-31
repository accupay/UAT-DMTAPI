using APT.DMT.API.Businessstrings.Models;
using System.Data.SqlClient;
using System.Data;
using APT.DMT.API.BusinessLogic;

namespace APT.DMT.API.DataAccessLayer
{
    public class ReportRepository
    {
        private readonly string _connString;
        private readonly string _txnconnString;
        private readonly string _masterconnString;
        public static IConfiguration Configuration { get; set; }
        public static DMTService _dmtService = new DMTService();
        public static CommonService _commonService = new CommonService();
        public ReportRepository()
        {
            Configuration = _commonService.GetConfiguration();
            _connString = Configuration.GetConnectionString("DMTDBconnection");
            _masterconnString = Configuration.GetConnectionString("MasterDBConnection");
            _txnconnString = Configuration.GetConnectionString("TransactionDBConnection");
        }
        #region DMTReports
        public DataSet APTTransactionLedgerReport(TransactionLedgerReportRequest Request)
        {
            //default timeout 30 secs
            using (var connection = new SqlConnection(_txnconnString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_TransactionLedgerReport", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("FromDate", Request.from_date));
                    cmd.Parameters.Add(new SqlParameter("ToDate", Request.to_date));
                    cmd.Parameters.Add(new SqlParameter("RetailerRefID", Request.agent_ref_id));

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
                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public DataSet APTGetRefundPaymentTransactionReport(GetRefundPaymentTransactionReportRequest Request)
        {
            //default timeout 30 secs
            using (var connection = new SqlConnection(_txnconnString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_GetRefundPaymentTransactionReport", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("FromDate", Request.from_date));
                    cmd.Parameters.Add(new SqlParameter("ToDate", Request.to_date));
                    cmd.Parameters.Add(new SqlParameter("RetailerRefID", Request.agent_ref_id));

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
                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public DataSet APTGetRefundPaymentTransactionReportPayOut(GetRefundPaymentTransactionReportRequest Request)
        {
            //default timeout 30 secs
            using (var connection = new SqlConnection(_txnconnString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_GetRefundPaymentTransactionPayoutReport", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("FromDate", Request.from_date));
                    cmd.Parameters.Add(new SqlParameter("ToDate", Request.to_date));
                    cmd.Parameters.Add(new SqlParameter("RetailerRefID", Request.agent_ref_id));

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
                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public DataSet APTGetRetailerPaymentTransactionReport(GetRetailerPaymentTransactionReportRequest Request)
        {
            //default timeout 30 secs
            using (var connection = new SqlConnection(_txnconnString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_GetRetailerPaymentTransactionReport", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("FromDate", Request.from_date));
                    cmd.Parameters.Add(new SqlParameter("ToDate", Request.to_date));
                    cmd.Parameters.Add(new SqlParameter("RetailerRefID", Request.agent_ref_id));
                    cmd.Parameters.Add(new SqlParameter("SearchOption", Request.search_option));
                    cmd.Parameters.Add(new SqlParameter("SearchValue", Request.search_value));

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
                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public DataSet APTGetRetailerPaymentTransactionReportPayOut(GetRetailerPaymentTransactionReportRequest Request)
        {
            //default timeout 30 secs
            using (var connection = new SqlConnection(_txnconnString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_GetRetailerPayoutPaymentTransactionReport", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("FromDate", Request.from_date));
                    cmd.Parameters.Add(new SqlParameter("ToDate", Request.to_date));
                    cmd.Parameters.Add(new SqlParameter("RetailerRefID", Request.agent_ref_id));
                    cmd.Parameters.Add(new SqlParameter("SearchOption", Request.search_option));
                    cmd.Parameters.Add(new SqlParameter("SearchValue", Request.search_value));

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
                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public DataSet APTGetRetailerTopupReport(GetRetailerTopupReportRequest Request)
        {
            //default timeout 30 secs
            using (var connection = new SqlConnection(_txnconnString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_GetRetailerTopupReport", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("FromDate", Request.from_date));
                    cmd.Parameters.Add(new SqlParameter("ToDate", Request.to_date));
                    cmd.Parameters.Add(new SqlParameter("SearchOption", Request.search_option));
                    cmd.Parameters.Add(new SqlParameter("SearchValue", Request.search_value));
                    cmd.Parameters.Add(new SqlParameter("RetailerRefID", Request.agent_ref_id));

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
                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public DataSet APTGetScrollMessage(string AgentMobileNumber)
        {
            //default timeout 30 secs
            using (var connection = new SqlConnection(_masterconnString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("Apt_ScrollMessage", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("Mobilenumber", AgentMobileNumber));

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
                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public DataSet APTPGTransactionReport(GetRetailerTopupReportRequest Request)
        {
            //default timeout 30 secs
            using (var connection = new SqlConnection(_txnconnString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_PGTransactionReport", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("FromDate", Request.from_date));
                    cmd.Parameters.Add(new SqlParameter("ToDate", Request.to_date));
                    cmd.Parameters.Add(new SqlParameter("agentrefid", Request.agent_ref_id));
                    cmd.Parameters.Add(new SqlParameter("SearchOption", Request.search_option));
                    cmd.Parameters.Add(new SqlParameter("SearchValue", Request.search_value));

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
                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public DataSet APTGetCommonDetails(GetRetailerCommonDetailsRequest Request)
        {
            //default timeout 30 secs
            using (var connection = new SqlConnection(_txnconnString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_GetCommonDetails", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("FromDate", Request.from_date));
                    cmd.Parameters.Add(new SqlParameter("ToDate", Request.to_date));
                    cmd.Parameters.Add(new SqlParameter("Status", Request.status));
                    cmd.Parameters.Add(new SqlParameter("agentrefid", Request.agent_ref_id));

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
                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public DataSet APTGetCustomerPayoutTxnReport(GetRetailerCustomerPayoutTxnReportRequest Request)
        {
            //default timeout 30 secs
            using (var connection = new SqlConnection(_txnconnString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_GetTransaction_CustomerPayout", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("FromDate", Request.from_date));
                    cmd.Parameters.Add(new SqlParameter("ToDate", Request.to_date));
                    cmd.Parameters.Add(new SqlParameter("MobileNo", Request.mobile_number));
                    cmd.Parameters.Add(new SqlParameter("TransactionStatusRefId", Request.transaction_status));

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
                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }
        #endregion DMTReports
    }
}
