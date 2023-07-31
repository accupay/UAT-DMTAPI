using APT.DMT.API.BusinessLogic;
using APT.DMT.API.BusinessObjects.Models;
using APT.DMT.API.Businessstrings.Models;
using APT.PaymentServices.API.BusinessObjects.Models;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Xml.Linq;


namespace APT.PaymentServices.API.DataAccessLayer
{

    public class PgtoPayoutRepository
    {
        private readonly string _connString;
        private readonly string _txnconnString;
        private readonly string _masterconnString;
        private readonly string _customerconnString;
        public static IConfiguration Configuration { get; set; }
        public static CommonService _commonService = new CommonService();
        public PgtoPayoutRepository()
        {
            Configuration = _commonService.GetConfiguration();
            _connString = Configuration.GetConnectionString("DMTDBconnection");
            _masterconnString = Configuration.GetConnectionString("MasterDBConnection");
            _txnconnString = Configuration.GetConnectionString("TransactionDBConnection");
        }

        
        public DataSet InsertCustomerMaster(APTInsertCustomerRequest Request)
        {
            using (var connections = new SqlConnection(_connString))
            {
                connections.Open();
                using (var cmd = new SqlCommand("Apt_inscustomerMaster", connections))
                {
                    cmd.Parameters.Add(new SqlParameter("Mobilenumber", Request.mobile_number));
                    cmd.Parameters.Add(new SqlParameter("Emailid", Request.email_id));
                    cmd.Parameters.Add(new SqlParameter("Name", Request.name));
                    cmd.Parameters.Add(new SqlParameter("Pincode", Request.pincode));
                    cmd.Parameters.Add(new SqlParameter("state", Request.state));
                    cmd.Parameters.Add(new SqlParameter("Area", Request.area));
                    cmd.Parameters.Add(new SqlParameter("Agentcode", Request.agent_code));
                    cmd.Parameters.Add(new SqlParameter("Password", Request.password));
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

        public DataSet GetCustomer(APTGetCustomerInfoRequest Request)
        {
            using (var connections = new SqlConnection(_connString))
            {
                connections.Open();
                using (var cmd = new SqlCommand("Apt_Getcustomer", connections))
                {
                    cmd.Parameters.Add(new SqlParameter("Mobilenumber", Request.mobile_number));
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

        public DataSet UploadKYC(APTPGtoPayoutuploadkycRequest Request)
        {
            using (var connections = new SqlConnection(_connString))
            {
                connections.Open();
                using (var cmd = new SqlCommand("APT_UpdateKYCDocument_CustomerPayout", connections))
                {
                    cmd.Parameters.Add(new SqlParameter("TransactionID", Request.transaction_id));
                    cmd.Parameters.Add(new SqlParameter("idprooffilerefnumber", Request.id_prrof_ref_number));
                    cmd.Parameters.Add(new SqlParameter("idproofimageFRONTFilname", Request.id_proof_front));
                    cmd.Parameters.Add(new SqlParameter("idproofimageBACKFilname", Request.id_proof_back));
                    cmd.Parameters.Add(new SqlParameter("idproofimageFilepath", Request.id_proof_filepath));
                    cmd.Parameters.Add(new SqlParameter("UserID", Request.user_id));
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
        public DataSet UploadCreditCard(APTPGtoPayoutuploadCreditCardRequest Request)
        {
            using (var connections = new SqlConnection(_connString))
            {
                connections.Open();
                using (var cmd = new SqlCommand("APT_UpdateKYCDocument_CustomerPayout", connections))
                {
                    cmd.Parameters.Add(new SqlParameter("TransactionID", Request.transaction_id));
                    cmd.Parameters.Add(new SqlParameter("Creditcardnumber", Request.credit_card_number));
                    cmd.Parameters.Add(new SqlParameter("CreditcardimagePath", Request.credit_card_image_path));
                    cmd.Parameters.Add(new SqlParameter("CreditcardimageFrontFilname", Request.credit_card_front_filename));
                    cmd.Parameters.Add(new SqlParameter("UserID", Request.user_id));
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
