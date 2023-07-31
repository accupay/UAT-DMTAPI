using APT.DMT.API.BusinessLogic;
using APT.DMT.API.BusinessObjects.Models;
using APT.DMT.API.Businessstrings.Models;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace APT.DMT.API.DataAccessLayer.DMT
{
    public class DMTRepository
    {
        private readonly string _connString;
        private readonly string _txnconnString;
        private readonly string _masterconnString;
        public static IConfiguration Configuration { get; set; }
        public static LogService _log = new LogService();
        public static CommonService _commonService = new CommonService();
        public DMTRepository()
        {
            Configuration = _commonService.GetConfiguration();
            _connString = Configuration.GetConnectionString("DMTDBconnection");
            _masterconnString = Configuration.GetConnectionString("MasterDBConnection");
            _txnconnString = Configuration.GetConnectionString("TransactionDBConnection");
        }

        #region DMT
        public DataSet APTGetCustomerInfo(APTGetCustomerInfo Request)
        {

            //default timeout 30 secs
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_GetCustomerInfo", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("MobileNo", Request.mobile_no));
                    cmd.Parameters.Add(new SqlParameter("AgentRefId", Request.agent_ref_id));
                    cmd.Parameters.Add(new SqlParameter("Flag", Request.flag));
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
        public DataSet APTRegisterCustomer(APTRegisterCustomer Request)
        {

            //default timeout 30 secs
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_RegisterCustomer", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("MobileNo", Request.mobile_no));
                    cmd.Parameters.Add(new SqlParameter("AgentMobile", Request.agent_mobile_no));
                    cmd.Parameters.Add(new SqlParameter("CustomerName", Request.customer_name));
                    cmd.Parameters.Add(new SqlParameter("Address1", Request.address1));
                    cmd.Parameters.Add(new SqlParameter("Address2", Request.address2));
                    cmd.Parameters.Add(new SqlParameter("PinCode", Request.pincode));
                    cmd.Parameters.Add(new SqlParameter("EMail", Request.email));
                    cmd.Parameters.Add(new SqlParameter("StateRefID", Request.state_ref_id));
                    cmd.Parameters.Add(new SqlParameter("State", Request.state));
                    cmd.Parameters.Add(new SqlParameter("City", Request.city));
                    cmd.Parameters.Add(new SqlParameter("gender", Request.gender));
                    cmd.Parameters.Add(new SqlParameter("Dob", Request.dob));
                    cmd.Parameters.Add(new SqlParameter("lastname", Request.last_name));
                    cmd.Parameters.Add(new SqlParameter("OTPVALIDATIONFLAG", Request.otp_validation_flag));
                    cmd.Parameters.Add(new SqlParameter("AgentRefID", Request.agent_ref_id));
                    cmd.Parameters.Add(new SqlParameter("PIN", Request.pin));
                    cmd.Parameters.Add(new SqlParameter("LocationRefID", Request.location_ref_id));
                    cmd.Parameters.Add(new SqlParameter("flag", Request.flag));
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
        public DataSet APTUpdateCustomer(APTUpdateCustomerRequest Request)
        {
            //default timeout 30 secs
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_UpdateCustomer", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("MobileNo", Request.mobile_no));
                    cmd.Parameters.Add(new SqlParameter("Bankrefid", Request.bank_ref_id));
                    cmd.Parameters.Add(new SqlParameter("ActiveStatusRefID", Request.active_status_ref_id));
                    cmd.Parameters.Add(new SqlParameter("UpdatedBy", Request.updated_by));
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
        public DataSet APTRegisterCustomerWithID(APTRegisterCustomer Request)
        {

            //default timeout 30 secs
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_RegisterCustomer_withid_registration", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("MobileNo", Request.mobile_no));
                    cmd.Parameters.Add(new SqlParameter("AgentMobile", Request.agent_mobile_no));
                    cmd.Parameters.Add(new SqlParameter("CustomerName", Request.customer_name));
                    cmd.Parameters.Add(new SqlParameter("Address1", Request.address1));
                    cmd.Parameters.Add(new SqlParameter("Address2", Request.address2));
                    cmd.Parameters.Add(new SqlParameter("PinCode", Request.pincode));
                    cmd.Parameters.Add(new SqlParameter("EMail", Request.email));
                    cmd.Parameters.Add(new SqlParameter("StateRefID", Convert.ToInt32(Request.state_ref_id)));
                    cmd.Parameters.Add(new SqlParameter("State", Request.state));
                    cmd.Parameters.Add(new SqlParameter("City", Request.city));
                    cmd.Parameters.Add(new SqlParameter("gender", Request.gender));
                    cmd.Parameters.Add(new SqlParameter("Dob", Request.dob));
                    cmd.Parameters.Add(new SqlParameter("lastname", Request.last_name));
                    cmd.Parameters.Add(new SqlParameter("OTPVALIDATIONFLAG", Convert.ToInt32(Request.otp_validation_flag)));
                    cmd.Parameters.Add(new SqlParameter("AgentRefID", Convert.ToInt32(Request.agent_ref_id)));
                    cmd.Parameters.Add(new SqlParameter("PIN", Convert.ToInt32(Request.pin)));
                    cmd.Parameters.Add(new SqlParameter("LocationRefID", Convert.ToInt32(Request.location_ref_id)));
                    cmd.Parameters.Add(new SqlParameter("flag", Convert.ToInt32(Request.flag)));
                    cmd.Parameters.Add(new SqlParameter("Bankid", Convert.ToInt32(Request.bank_id)));
                    cmd.Parameters.Add(new SqlParameter("Bankrefid", Request.bank_ref_id));

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
        public DataSet APTValidateRegisterCustomerOTP(ValidateOTPRequest Request)
        {
            //default timeout 30 secs
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("ValidateOTP", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("OTP", Request.otp));
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
        public DataSet APTGetAllPayee(APTGetAllPayee Request)
        {

            //default timeout 30 secs
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_GetAllPayee", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("customerMobileNo ", Request.customer_mobile_no));
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
                        _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.customer_mobile_no);

                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }
        public DataSet APTGetPayeeDetails(string PayeeRefID, string CustMobNum)
        {

            //default timeout 30 secs
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_GetPayeeDetails", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("PayeeRefID", PayeeRefID));
                    cmd.Parameters.Add(new SqlParameter("customerMobileNo", CustMobNum));
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
                        _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, CustMobNum);

                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }
        public DataSet APTAddPayee(APTAddPayee Request)
        {

            //default timeout 30 secs
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_AddPayee", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("Payeerefid", Convert.ToInt32(Request.payee_ref_id)));
                    cmd.Parameters.Add(new SqlParameter("PayeeName", Request.payee_name));
                    cmd.Parameters.Add(new SqlParameter("PayeeMobileNo", Request.payee_mobile_no));
                    cmd.Parameters.Add(new SqlParameter("IFSCCode", Request.ifsc_code));
                    cmd.Parameters.Add(new SqlParameter("IFSCBankRefID", Convert.ToInt32(Request.ifsc_bank_ref_id)));
                    cmd.Parameters.Add(new SqlParameter("IFSCBankBranchRefID", Convert.ToInt32(Request.ifsc_bank_branch_ref_id)));
                    cmd.Parameters.Add(new SqlParameter("AccountNumber", Request.account_number));
                    cmd.Parameters.Add(new SqlParameter("ActiveStatusRefID", Convert.ToInt32(Request.active_status_ref_id)));
                    cmd.Parameters.Add(new SqlParameter("PayeeNameNPCI", Request.payee_name_npci));
                    cmd.Parameters.Add(new SqlParameter("Validated", Convert.ToInt32(Request.validated)));
                    cmd.Parameters.Add(new SqlParameter("MobileNo", Request.mobile_no));
                    cmd.Parameters.Add(new SqlParameter("BankTypeID", Convert.ToInt32(Request.bank_type_id)));
                    cmd.Parameters.Add(new SqlParameter("AgentRefId", Convert.ToInt32(Request.agent_ref_id)));
                    cmd.Parameters.Add(new SqlParameter("flag", Convert.ToInt32(Request.flag)));
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

        public DataSet APTDeletePayee(APTDeletePayee Request)
        {

            //default timeout 30 secs
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_DeletePayee", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("Payeerefid", Convert.ToInt32(Request.payee_ref_id)));
                    cmd.Parameters.Add(new SqlParameter("sendermobilenumber", Request.mobile_no));
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
        public DataSet APTAddPayeeWithID(APTAddPayee Request)
        {

            //default timeout 30 secs
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_AddPayee_withid_registration", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("Payeerefid", Request.payee_ref_id));
                    cmd.Parameters.Add(new SqlParameter("PayeeName", Request.payee_name));
                    cmd.Parameters.Add(new SqlParameter("PayeeMobileNo", Request.payee_mobile_no));
                    cmd.Parameters.Add(new SqlParameter("IFSCCode", Request.ifsc_code));
                    cmd.Parameters.Add(new SqlParameter("IFSCBankRefID", Request.ifsc_bank_ref_id));
                    cmd.Parameters.Add(new SqlParameter("IFSCBankBranchRefID", Request.ifsc_bank_branch_ref_id));
                    cmd.Parameters.Add(new SqlParameter("AccountNumber", Request.account_number));
                    cmd.Parameters.Add(new SqlParameter("ActiveStatusRefID", Request.active_status_ref_id));
                    cmd.Parameters.Add(new SqlParameter("PayeeNameNPCI", Request.payee_name_npci));
                    cmd.Parameters.Add(new SqlParameter("Validated", Request.validated));
                    cmd.Parameters.Add(new SqlParameter("MobileNo", Request.mobile_no));
                    cmd.Parameters.Add(new SqlParameter("BankTypeID", Request.bank_type_id));
                    cmd.Parameters.Add(new SqlParameter("AgentRefId", Request.agent_ref_id));
                    cmd.Parameters.Add(new SqlParameter("flag", Request.flag));
                    cmd.Parameters.Add(new SqlParameter("Bankid", Request.bank_id));
                    cmd.Parameters.Add(new SqlParameter("Bankrefid", Request.bank_ref_id));
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
        public DataSet APTUpdatePayee(APTUpdatePayeeRequest Request)
        {
            //default timeout 30 secs
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_UpdatePAyee", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("CustomerMobileNo", Request.mobile_no));
                    cmd.Parameters.Add(new SqlParameter("Bankrefid", Request.bank_ref_id));
                    cmd.Parameters.Add(new SqlParameter("ActiveStatusRefID", Request.active_status_ref_id));
                    cmd.Parameters.Add(new SqlParameter("UpdatedBy", Request.updated_by));
                    cmd.Parameters.Add(new SqlParameter("payeerefid", Request.payee_ref_id));
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

        public DataSet APTUpdateBeneValidationStatus(APTUpdatePayeeRequest Request)
        {
            //default timeout 30 secs
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_UpdateNPCIName", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("CustomerMobileNo", Request.mobile_no));
                    cmd.Parameters.Add(new SqlParameter("REFID", Request.payee_ref_id));
                    cmd.Parameters.Add(new SqlParameter("NPCIPayeeName", Request.npci_name));
                    cmd.Parameters.Add(new SqlParameter("UpdatedBy", Request.updated_by));
                    cmd.Parameters.Add(new SqlParameter("Validated", Request.validated));
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
        public DataSet APTTransaction(APTTransactionRequest Request)
        {
            //default timeout 30 secs
            using (var connection = new SqlConnection(_txnconnString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_TransferMoneyToPayee_v1", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("sendermobilenumber", Request.sender_mobile_number));
                    cmd.Parameters.Add(new SqlParameter("PayeeRefID", Request.payee_ref_id.ToString()));
                    cmd.Parameters.Add(new SqlParameter("Amount", Request.amount));
                    cmd.Parameters.Add(new SqlParameter("PaymentTransactionTypeRefID", Request.payment_transaction_type_refid.ToString()));
                    cmd.Parameters.Add(new SqlParameter("PayModeRefID", Request.pay_mode_ref_id));
                    cmd.Parameters.Add(new SqlParameter("Agentmobile", Request.agent_mobile));
                    cmd.Parameters.Add(new SqlParameter("Accountnumerin", Request.account_number_in));
                    cmd.Parameters.Add(new SqlParameter("ARefID", Request.agent_ref_id));

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
                        _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.sender_mobile_number);

                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }
        public DataSet APTUpdateTransaction(APTUpdateTransaction Request)
        {
            //default timeout 30 secs
            using (var connection = new SqlConnection(_txnconnString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_Updatepaymenttransaction", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("TransactionID", Request.transaction_id));
                    cmd.Parameters.Add(new SqlParameter("BankReferrenceNumber", Request.bank_reference_number));
                    cmd.Parameters.Add(new SqlParameter("Request", Request.request));
                    cmd.Parameters.Add(new SqlParameter("Response", Request.response));
                    cmd.Parameters.Add(new SqlParameter("ResponseCode", Request.response_code));
                    cmd.Parameters.Add(new SqlParameter("ResponseDescription", Request.response_description));
                    cmd.Parameters.Add(new SqlParameter("Bankbenename", Request.beneficiary_name));
                    //cmd.Parameters.Add(new SqlParameter("NPCIName", Request.beneficiary_name));
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
                        _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.transaction_id);

                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public DataSet APTRefundTransaction(RefundOTPRequest Request)
        {
            //default timeout 30 secs
            using (var connection = new SqlConnection(_txnconnString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_PaymentTransaction_Cashout", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("TransactionID", Request.transaction_id));

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
                        _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.transaction_id);

                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }
        public DataSet APTRefundPayOutTransaction(RefundOTPRequest Request)
        {
            //default timeout 30 secs
            using (var connection = new SqlConnection(_txnconnString))
            {
                connection.Open();
                using (var cmd = new SqlCommand("APT_PaymentTransactionPayout_Cashout", connection))
                {
                    cmd.Parameters.Add(new SqlParameter("TransactionID", Request.transaction_id));

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
                        _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.transaction_id);

                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }
        #endregion DMT




    }
}
