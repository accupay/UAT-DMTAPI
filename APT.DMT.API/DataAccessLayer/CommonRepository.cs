using APT.DMT.API.BusinessLogic;
using APT.DMT.API.BusinessObjects.Models;
using APT.DMT.API.Businessstrings.Models;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace APT.PaymentServices.API.DataAccessLayer
{
  
    public class CommonRepository
    {
        private readonly string _connString;
        private readonly string _txnconnString;
        private readonly string _masterconnString;
        public static IConfiguration Configuration { get; set; }
        public static LogService _log = new LogService();
        public static CommonService _commonService = new CommonService();
        public CommonRepository()
        {
            Configuration = _commonService.GetConfiguration();
            _connString = Configuration.GetConnectionString("DMTDBconnection");
            _masterconnString = Configuration.GetConnectionString("MasterDBConnection");
            _txnconnString = Configuration.GetConnectionString("TransactionDBConnection");
        }

        public DataSet APTTopupInfo()
        {

            //default timeout 30 secs
            using (var connection = new SqlConnection(_masterconnString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("Apt_topupinfo", connection))
                {
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
                        _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, "Topup info");

                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public DataSet APTSupportInfo(string AgentRefID)
        {

            //default timeout 30 secs
            using (var connection = new SqlConnection(_masterconnString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("Apt_Supportinfo", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("agentrefid", AgentRefID));

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
                        _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, AgentRefID);

                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public DataSet APTInsertOTP(APTInsertOTPRequest Request)
        {
            //default timeout 30 secs
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_insotp", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("Mobilenumber", Request.mobile_no));
                    cmd.Parameters.Add(new SqlParameter("Accounttype", (string.IsNullOrEmpty(Request.account_type) ? "" : Request.account_type).ToString()));
                    cmd.Parameters.Add(new SqlParameter("Accountrefid", Request.account_ref_id));
                    cmd.Parameters.Add(new SqlParameter("OTP", (string.IsNullOrEmpty(Request.otp) ? "" : Request.otp).ToString()));
                    cmd.Parameters.Add(new SqlParameter("otptyperefid", Request.otp_type_ref_id));

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
                        _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.mobile_no);

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
