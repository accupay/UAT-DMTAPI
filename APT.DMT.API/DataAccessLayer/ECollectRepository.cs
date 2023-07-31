using APT.DMT.API.BusinessLogic;
using APT.DMT.API.BusinessObjects.Models;
using System.Data.SqlClient;
using System.Data;
using APT.PaymentServices.API.BusinessObjects.Models;

namespace APT.PaymentServices.API.DataAccessLayer
{
    public class ECollectRepository
    {
        private readonly string _connString;
        private readonly string _txnconnString;
        private readonly string _masterconnString;
        private readonly string _authconnString;
        public static IConfiguration Configuration { get; set; }
       // public static CommonService _commonService = new CommonService();
        public static LogService _log = new LogService();
        public ECollectRepository()
        {
            Configuration = GetConfiguration();
            _connString = Configuration.GetConnectionString("DMTDBconnection");
            _masterconnString = Configuration.GetConnectionString("MasterDBConnection");
            _txnconnString = Configuration.GetConnectionString("TransactionDBConnection");
            _authconnString = Configuration.GetConnectionString("AuthDBconnection");
        }
        public IConfiguration GetConfiguration()
        {
            Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();
            return Configuration;
        }
        public DataSet ECollectInsert(ECollectDataInsertRequest Request)
        {
            using (var connections = new SqlConnection(_txnconnString))
            {
                connections.Open();
                using (var cmd = new SqlCommand("ins_EcollectData", connections))
                {
                    cmd.Parameters.Add(new SqlParameter("Ifsccode", Request.IFSCCode));
                    cmd.Parameters.Add(new SqlParameter("Accountnumber", Request.AccountNumber));
                    cmd.Parameters.Add(new SqlParameter("Remittername", Request.RemitterName));
                    cmd.Parameters.Add(new SqlParameter("Amount", Request.Amount));
                    cmd.Parameters.Add(new SqlParameter("mobileno", Request.MobileNumber));
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
                        connections.Close();
                    }
                }
            }
        }

        public DataSet ECollectAccountAddition(ECOllectAccountAdditionRequest Request)
        {
            using (var connections = new SqlConnection(_txnconnString))
            {
                connections.Open();
                using (var cmd = new SqlCommand("APT_InsertAgentPayee", connections))
                {
                    cmd.Parameters.Add(new SqlParameter("AgentMobileNo", Request.agent_mobile));
                    cmd.Parameters.Add(new SqlParameter("BankAccountNo", Request.bank_account_number));
                    cmd.Parameters.Add(new SqlParameter("AccountHolderName", Request.account_holder_name));
                    cmd.Parameters.Add(new SqlParameter("BankName", Request.bank_name));
                    cmd.Parameters.Add(new SqlParameter("BankRefID", Request.bank_ref_id));

                    cmd.Parameters.Add(new SqlParameter("IFSCCode", Request.ifsc_code));
                    cmd.Parameters.Add(new SqlParameter("Bankfilename", Request.bank_file_name));
                   // cmd.Parameters.Add(new SqlParameter("BankCopyfile", Request.bank_copy_file));
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
                        connections.Close();
                    }
                }
            }
        }

        public DataSet ECollectViewAccountAddition(ECOllectViewAccountRequest Request)
        {
            using (var connections = new SqlConnection(_txnconnString))
            {
                connections.Open();
                using (var cmd = new SqlCommand("APT_GetAgentPayeeofAgent", connections))
                {
                    cmd.Parameters.Add(new SqlParameter("AgentRefID", Request.agent_ref_id));
                   
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
                        connections.Close();
                    }
                }
            }
        }

        public DataSet DepositSlipUpload(DepositSlipUploadRequest Request)
        {
            using (var connections = new SqlConnection(_txnconnString))
            {
                connections.Open();
                using (var cmd = new SqlCommand("APT_DepositslipUpload", connections))
                {
                    cmd.Parameters.Add(new SqlParameter("AccountTypeRefID", Request.account_type_ref_id));
                    cmd.Parameters.Add(new SqlParameter("AccountRefID", Request.account_ref_id));
                    cmd.Parameters.Add(new SqlParameter("MobileNumber", Request.mobile_number));
                    cmd.Parameters.Add(new SqlParameter("BankTransactionID", Request.bank_transaction_id));
                    cmd.Parameters.Add(new SqlParameter("Amount", Request.amount));
                    cmd.Parameters.Add(new SqlParameter("DepositedDate", Request.deposited_date));
                    cmd.Parameters.Add(new SqlParameter("TopupTypeRefID", Request.topup_type_refid));
                    cmd.Parameters.Add(new SqlParameter("DepositedBank", Request.deposited_bank));
                    cmd.Parameters.Add(new SqlParameter("Image", Request.image));
                    cmd.Parameters.Add(new SqlParameter("Comments", Request.comments));

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
                        connections.Close();
                    }
                }
            }
        }

    }
}
