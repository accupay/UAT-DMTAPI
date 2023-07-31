using APT.DMT.API.BusinessLogic.BankService;
using APT.DMT.API.BusinessObjects;
using APT.DMT.API.BusinessObjects.Models;
using APT.DMT.API.Businessstrings.Models;
using APT.DMT.API.DataAccessLayer;
using APT.DMT.API.DataAccessLayer.DMT;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using System.Transactions;
using System.Xml.Linq;
using File = System.IO.File;
namespace APT.DMT.API.BusinessLogic
{
    public class ReportService
    {
        public static CommonService _commonService = new CommonService();
        public static LogService _log = new LogService();
        public static ReportRepository _reportRepo = new ReportRepository();
        #region Reports
        public APTGenericResponse? APTTransactionLedgerReport(TransactionLedgerReportRequest Request)
        {
            _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, "Entry", "", Request.agent_ref_id);

            APTGenericResponse objresponse = new APTGenericResponse();
            List<TransactionLedgerReportResponse> LstobjResponse = new List<TransactionLedgerReportResponse>();
            try
            {
                DataSet dst = _reportRepo.APTTransactionLedgerReport(Request);
                if (_commonService.IsValidDataSet(dst))
                {
                    for (int i = 0; i < dst.Tables[0].Rows.Count; i++)
                    {
                        TransactionLedgerReportResponse objResponse = new TransactionLedgerReportResponse();
                        objResponse.transaction_ref_id = dst.Tables[0].Rows[i]["TransactionRefID"].ToString();
                        objResponse.distributor_ref_id = dst.Tables[0].Rows[i]["DistributorRefID"].ToString();
                        objResponse.agent_ref_id = dst.Tables[0].Rows[i]["AgentRefID"].ToString();
                        objResponse.transaction_date = dst.Tables[0].Rows[i]["TransactionDate"].ToString();
                        objResponse.opening_balance = dst.Tables[0].Rows[i]["OpeningBalance"].ToString();
                        objResponse.credit = dst.Tables[0].Rows[i]["Credit"].ToString();
                        objResponse.debit = dst.Tables[0].Rows[i]["Debit"].ToString();
                        objResponse.closing_balance = dst.Tables[0].Rows[i]["ClosingBalance"].ToString();
                        objResponse.transaction_id = dst.Tables[0].Rows[i]["TransactionID"].ToString();
                        objResponse.comments = dst.Tables[0].Rows[i]["Comments"].ToString();
                        objResponse.status = dst.Tables[0].Rows[i]["Status"].ToString();
                        objResponse.created_date = dst.Tables[0].Rows[i]["CreatedDate"].ToString();
                        objResponse.updated_date = dst.Tables[0].Rows[i]["UpdatedDate"].ToString();
                        objResponse.date_key = dst.Tables[0].Rows[i]["Datekey"].ToString();
                        LstobjResponse.Add(objResponse);

                    }

                    objresponse.response_code = "200";
                    objresponse.response_message = "Success";
                    objresponse.data = LstobjResponse;

                }
                else
                {
                    objresponse.response_code = "101";
                    objresponse.response_message = "No Records Found";
                    objresponse.data = null;
                }

            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.agent_ref_id);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
            }

            return objresponse;
        }

        public APTGenericResponse? APTGetRefundPaymentTransactionReport(GetRefundPaymentTransactionReportRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, "Entry", "", Request.agent_ref_id);
            List<GetRefundPaymentTransactionReportResponse> LstobjResponse = new List<GetRefundPaymentTransactionReportResponse>();

            try
            {
                DataSet dst = _reportRepo.APTGetRefundPaymentTransactionReport(Request);
                if (_commonService.IsValidDataSet(dst))
                {
                    for (int i = 0; i < dst.Tables[0].Rows.Count; i++)
                    {
                        GetRefundPaymentTransactionReportResponse objResponse = new GetRefundPaymentTransactionReportResponse();
                        objResponse.paymenttransactionrefid = dst.Tables[0].Rows[i]["PaymentTransactionRefID"].ToString();
                        objResponse.paymode = dst.Tables[0].Rows[i]["PayMode"].ToString();
                        objResponse.transactionid = dst.Tables[0].Rows[i]["TransactionID"].ToString();
                        objResponse.transactionstatus = dst.Tables[0].Rows[i]["TransactionStatus"].ToString();
                        objResponse.request = dst.Tables[0].Rows[i]["Request"].ToString();
                        objResponse.response = dst.Tables[0].Rows[i]["Response"].ToString();
                        objResponse.statuscode = dst.Tables[0].Rows[i]["statuscode"].ToString();
                        objResponse.statusdescription = dst.Tables[0].Rows[i]["statusdescription"].ToString();
                        objResponse.responsecode = dst.Tables[0].Rows[i]["Responsecode"].ToString();
                        objResponse.responsedescription = dst.Tables[0].Rows[i]["ResponseDescription"].ToString();
                        objResponse.displaymessage = dst.Tables[0].Rows[i]["Displaymessage"].ToString();
                        objResponse.bankreferencenumber = dst.Tables[0].Rows[i]["Bankreferencenumber"].ToString();
                        objResponse.amount = dst.Tables[0].Rows[i]["Amount"].ToString();
                        objResponse.customerfee = dst.Tables[0].Rows[i]["CustomerFee"].ToString();
                        objResponse.agentcomm = dst.Tables[0].Rows[i]["AgentComm"].ToString();
                        objResponse.distcomm = dst.Tables[0].Rows[i]["Distcomm"].ToString();
                        objResponse.createddate = dst.Tables[0].Rows[i]["Createddate"].ToString();
                        objResponse.updateddate = dst.Tables[0].Rows[i]["Updateddate"].ToString();
                        objResponse.datekey = dst.Tables[0].Rows[i]["Datekey"].ToString();
                        objResponse.cashoutdttime = dst.Tables[0].Rows[i]["CashoutDttime"].ToString();
                        objResponse.cashoutdatekey = dst.Tables[0].Rows[i]["Cashoutdatekey"].ToString();
                        objResponse.reversaldatetime = dst.Tables[0].Rows[i]["Reversaldatetime"].ToString();
                        objResponse.reversaldatekey = dst.Tables[0].Rows[i]["Reversaldatekey"].ToString();
                        objResponse.bankname = dst.Tables[0].Rows[i]["Bankname"].ToString();
                        objResponse.customermobileno = dst.Tables[0].Rows[i]["Customermobileno"].ToString();
                        objResponse.customername = dst.Tables[0].Rows[i]["Customername"].ToString();
                        objResponse.payeename = dst.Tables[0].Rows[i]["Payeename"].ToString();
                        objResponse.payeeaccountnumer = dst.Tables[0].Rows[i]["Payeeaccountnumer"].ToString();
                        objResponse.amobileno = dst.Tables[0].Rows[i]["Amobileno"].ToString();
                        objResponse.agencyname = dst.Tables[0].Rows[i]["Agencyname"].ToString();
                        objResponse.agentname = dst.Tables[0].Rows[i]["Agentname"].ToString();
                        objResponse.city = dst.Tables[0].Rows[i]["City"].ToString();
                        objResponse.state = dst.Tables[0].Rows[i]["state"].ToString();
                        objResponse.zone = dst.Tables[0].Rows[i]["Zone"].ToString();
                        objResponse.distmobileno = dst.Tables[0].Rows[i]["Distmobileno"].ToString();
                        objResponse.distributorname = dst.Tables[0].Rows[i]["DistributorName"].ToString();
                        objResponse.distcompanyname = dst.Tables[0].Rows[i]["Distcompanyname"].ToString();
                        objResponse.agentip = dst.Tables[0].Rows[i]["agentip"].ToString();
                        objResponse.latitude = dst.Tables[0].Rows[i]["latitude"].ToString();
                        objResponse.longtitude = dst.Tables[0].Rows[i]["longtitude"].ToString();
                        objResponse.channeltype = dst.Tables[0].Rows[i]["Channeltype"].ToString();
                        objResponse.channeltypedesc = dst.Tables[0].Rows[i]["Channeltypedesc"].ToString();
                        objResponse.agentcommwithouttds = dst.Tables[0].Rows[i]["AgentCommWithoutTDS"].ToString();
                        objResponse.distcommwithouttds = dst.Tables[0].Rows[i]["DistCommWithoutTDS"].ToString();
                        LstobjResponse.Add(objResponse);
                    }

                    objresponse.response_code = "200";
                    objresponse.response_message = "Success";
                    objresponse.data = LstobjResponse;

                }
                else
                {
                    objresponse.response_code = "101";
                    objresponse.response_message = "No Records Found";
                    objresponse.data = null;
                }

            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.agent_ref_id);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
            }

            return objresponse;
        }

        public APTGenericResponse? APTGetRefundPaymentTransactionReportPayOut(GetRefundPaymentTransactionReportRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, "Entry", "", Request.agent_ref_id);
            List<GetRefundPaymentTransactionReportResponse> LstobjResponse = new List<GetRefundPaymentTransactionReportResponse>();

            try
            {
                DataSet dst = _reportRepo.APTGetRefundPaymentTransactionReportPayOut(Request);
                if (_commonService.IsValidDataSet(dst))
                {
                    for (int i = 0; i < dst.Tables[0].Rows.Count; i++)
                    {
                        GetRefundPaymentTransactionReportResponse objResponse = new GetRefundPaymentTransactionReportResponse();
                        objResponse.paymenttransactionrefid = dst.Tables[0].Rows[i]["PaymentTransactionRefID"].ToString();
                        objResponse.paymode = dst.Tables[0].Rows[i]["PayMode"].ToString();
                        objResponse.transactionid = dst.Tables[0].Rows[i]["TransactionID"].ToString();
                        objResponse.transactionstatus = dst.Tables[0].Rows[i]["TransactionStatus"].ToString();
                        objResponse.request = dst.Tables[0].Rows[i]["Request"].ToString();
                        objResponse.response = dst.Tables[0].Rows[i]["Response"].ToString();
                        objResponse.statuscode = dst.Tables[0].Rows[i]["statuscode"].ToString();
                        objResponse.statusdescription = dst.Tables[0].Rows[i]["statusdescription"].ToString();
                        objResponse.responsecode = dst.Tables[0].Rows[i]["Responsecode"].ToString();
                        objResponse.responsedescription = dst.Tables[0].Rows[i]["ResponseDescription"].ToString();
                        objResponse.displaymessage = dst.Tables[0].Rows[i]["Displaymessage"].ToString();
                        objResponse.bankreferencenumber = dst.Tables[0].Rows[i]["Bankreferencenumber"].ToString();
                        objResponse.amount = dst.Tables[0].Rows[i]["Amount"].ToString();
                        objResponse.customerfee = dst.Tables[0].Rows[i]["CustomerFee"].ToString();
                        objResponse.agentcomm = dst.Tables[0].Rows[i]["AgentComm"].ToString();
                        objResponse.distcomm = dst.Tables[0].Rows[i]["Distcomm"].ToString();
                        objResponse.createddate = dst.Tables[0].Rows[i]["Createddate"].ToString();
                        objResponse.updateddate = dst.Tables[0].Rows[i]["Updateddate"].ToString();
                        objResponse.datekey = dst.Tables[0].Rows[i]["Datekey"].ToString();
                        objResponse.cashoutdttime = dst.Tables[0].Rows[i]["CashoutDttime"].ToString();
                        objResponse.cashoutdatekey = dst.Tables[0].Rows[i]["Cashoutdatekey"].ToString();
                        objResponse.reversaldatetime = dst.Tables[0].Rows[i]["Reversaldatetime"].ToString();
                        objResponse.reversaldatekey = dst.Tables[0].Rows[i]["Reversaldatekey"].ToString();
                        objResponse.bankname = dst.Tables[0].Rows[i]["Bankname"].ToString();
                        objResponse.customermobileno = dst.Tables[0].Rows[i]["Customermobileno"].ToString();
                        objResponse.customername = dst.Tables[0].Rows[i]["Customername"].ToString();
                        objResponse.payeename = dst.Tables[0].Rows[i]["Payeename"].ToString();
                        objResponse.payeeaccountnumer = dst.Tables[0].Rows[i]["Payeeaccountnumer"].ToString();
                        objResponse.amobileno = dst.Tables[0].Rows[i]["Amobileno"].ToString();
                        objResponse.agencyname = dst.Tables[0].Rows[i]["Agencyname"].ToString();
                        objResponse.agentname = dst.Tables[0].Rows[i]["Agentname"].ToString();
                        objResponse.city = dst.Tables[0].Rows[i]["City"].ToString();
                        objResponse.state = dst.Tables[0].Rows[i]["state"].ToString();
                        objResponse.zone = dst.Tables[0].Rows[i]["Zone"].ToString();
                        objResponse.distmobileno = dst.Tables[0].Rows[i]["Distmobileno"].ToString();
                        objResponse.distributorname = dst.Tables[0].Rows[i]["DistributorName"].ToString();
                        objResponse.distcompanyname = dst.Tables[0].Rows[i]["Distcompanyname"].ToString();
                        objResponse.agentip = dst.Tables[0].Rows[i]["agentip"].ToString();
                        objResponse.latitude = dst.Tables[0].Rows[i]["latitude"].ToString();
                        objResponse.longtitude = dst.Tables[0].Rows[i]["longtitude"].ToString();
                        objResponse.channeltype = dst.Tables[0].Rows[i]["Channeltype"].ToString();
                        objResponse.channeltypedesc = dst.Tables[0].Rows[i]["Channeltypedesc"].ToString();
                        objResponse.agentcommwithouttds = dst.Tables[0].Rows[i]["AgentCommWithoutTDS"].ToString();
                        objResponse.distcommwithouttds = dst.Tables[0].Rows[i]["DistCommWithoutTDS"].ToString();
                        LstobjResponse.Add(objResponse);
                    }

                    objresponse.response_code = "200";
                    objresponse.response_message = "Success";
                    objresponse.data = LstobjResponse;

                }
                else
                {
                    objresponse.response_code = "101";
                    objresponse.response_message = "No Records Found";
                    objresponse.data = null;
                }

            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.agent_ref_id);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
            }

            return objresponse;
        }

        public APTGenericResponse? APTGetRetailerPaymentTransactionReport(GetRetailerPaymentTransactionReportRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
           _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, "Entry", "", Request.agent_ref_id);
            List<GetRetailerPaymentTransactionReportResponse> LstobjResponse = new List<GetRetailerPaymentTransactionReportResponse>();
            try
            {
                DataSet dst = _reportRepo.APTGetRetailerPaymentTransactionReport(Request);
                if (_commonService.IsValidDataSet(dst))
                {
                    for (int i = 0; i < dst.Tables[0].Rows.Count; i++)
                    {
                        GetRetailerPaymentTransactionReportResponse objResponse = new GetRetailerPaymentTransactionReportResponse();
                        objResponse.paymenttransactionrefid = dst.Tables[0].Rows[i]["PaymentTransactionRefID"].ToString();
                        objResponse.paymode = dst.Tables[0].Rows[i]["PayMode"].ToString();
                        objResponse.transactionid = dst.Tables[0].Rows[i]["TransactionID"].ToString();
                        objResponse.transactionstatus = dst.Tables[0].Rows[i]["TransactionStatus"].ToString();
                        objResponse.request = dst.Tables[0].Rows[i]["Request"].ToString();
                        objResponse.response = dst.Tables[0].Rows[i]["Response"].ToString();
                        objResponse.statuscode = dst.Tables[0].Rows[i]["statuscode"].ToString();
                        objResponse.statusdescription = dst.Tables[0].Rows[i]["statusdescription"].ToString();
                        objResponse.responsecode = dst.Tables[0].Rows[i]["Responsecode"].ToString();
                        objResponse.responsedescription = dst.Tables[0].Rows[i]["ResponseDescription"].ToString();
                        objResponse.displaymessage = dst.Tables[0].Rows[i]["Displaymessage"].ToString();
                        objResponse.bankreferencenumber = dst.Tables[0].Rows[i]["Bankreferencenumber"].ToString();
                        objResponse.amount = dst.Tables[0].Rows[i]["Amount"].ToString();
                        objResponse.customerfee = dst.Tables[0].Rows[i]["CustomerFee"].ToString();
                        objResponse.agentcomm = dst.Tables[0].Rows[i]["AgentComm"].ToString();
                        objResponse.distcomm = dst.Tables[0].Rows[i]["Distcomm"].ToString();
                        objResponse.createddate = dst.Tables[0].Rows[i]["Createddate"].ToString();
                        objResponse.updateddate = dst.Tables[0].Rows[i]["Updateddate"].ToString();
                        objResponse.datekey = dst.Tables[0].Rows[i]["Datekey"].ToString();
                        objResponse.cashoutdttime = dst.Tables[0].Rows[i]["CashoutDttime"].ToString();
                        objResponse.cashoutdatekey = dst.Tables[0].Rows[i]["Cashoutdatekey"].ToString();
                        objResponse.reversaldatetime = dst.Tables[0].Rows[i]["Reversaldatetime"].ToString();
                        objResponse.reversaldatekey = dst.Tables[0].Rows[i]["Reversaldatekey"].ToString();
                        objResponse.bankname = dst.Tables[0].Rows[i]["Bankname"].ToString();
                        objResponse.customermobileno = dst.Tables[0].Rows[i]["Customermobileno"].ToString();
                        objResponse.customername = dst.Tables[0].Rows[i]["Customername"].ToString();
                        objResponse.payeename = dst.Tables[0].Rows[i]["Payeename"].ToString();
                        objResponse.payeeaccountnumer = dst.Tables[0].Rows[i]["Payeeaccountnumer"].ToString();
                        objResponse.amobileno = dst.Tables[0].Rows[i]["Amobileno"].ToString();
                        objResponse.agencyname = dst.Tables[0].Rows[i]["Agencyname"].ToString();
                        objResponse.agentname = dst.Tables[0].Rows[i]["Agentname"].ToString();
                        objResponse.city = dst.Tables[0].Rows[i]["City"].ToString();
                        objResponse.state = dst.Tables[0].Rows[i]["state"].ToString();
                        objResponse.zone = dst.Tables[0].Rows[i]["Zone"].ToString();
                        objResponse.distmobileno = dst.Tables[0].Rows[i]["Distmobileno"].ToString();
                        objResponse.distributorname = dst.Tables[0].Rows[i]["DistributorName"].ToString();
                        objResponse.distcompanyname = dst.Tables[0].Rows[i]["Distcompanyname"].ToString();
                        objResponse.agentip = dst.Tables[0].Rows[i]["agentip"].ToString();
                        objResponse.latitude = dst.Tables[0].Rows[i]["latitude"].ToString();
                        objResponse.longtitude = dst.Tables[0].Rows[i]["longtitude"].ToString();
                        objResponse.agentcommwithouttds = dst.Tables[0].Rows[i]["AgentCommWithoutTDS"].ToString();
                        objResponse.distcommwithouttds = dst.Tables[0].Rows[i]["DistCommWithoutTDS"].ToString();
                        objResponse.IFSC = dst.Tables[0].Rows[i]["IFSCCode"].ToString();
                        LstobjResponse.Add(objResponse);
                    }

                    objresponse.response_code = "200";
                    objresponse.response_message = "Success";
                    objresponse.data = LstobjResponse;

                }
                else
                {
                    objresponse.response_code = "101";
                    objresponse.response_message = "No Records Found";
                    objresponse.data = null;
                }

            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.agent_ref_id);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
            }

            return objresponse;
        }

        public APTGenericResponse? APTGetRetailerPaymentTransactionReportPayOut(GetRetailerPaymentTransactionReportRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, "Entry", "", Request.agent_ref_id);
            List<GetRetailerPaymentTransactionReportResponse> LstobjResponse = new List<GetRetailerPaymentTransactionReportResponse>();
            try
            {
                DataSet dst = _reportRepo.APTGetRetailerPaymentTransactionReportPayOut(Request);
                if (_commonService.IsValidDataSet(dst))
                {
                    for (int i = 0; i < dst.Tables[0].Rows.Count; i++)
                    {
                        GetRetailerPaymentTransactionReportResponse objResponse = new GetRetailerPaymentTransactionReportResponse();
                        objResponse.paymenttransactionrefid = dst.Tables[0].Rows[i]["PaymentTransactionRefID"].ToString();
                        objResponse.paymode = dst.Tables[0].Rows[i]["PayMode"].ToString();
                        objResponse.transactionid = dst.Tables[0].Rows[i]["TransactionID"].ToString();
                        objResponse.transactionstatus = dst.Tables[0].Rows[i]["TransactionStatus"].ToString();
                        objResponse.request = dst.Tables[0].Rows[i]["Request"].ToString();
                        objResponse.response = dst.Tables[0].Rows[i]["Response"].ToString();
                        objResponse.statuscode = dst.Tables[0].Rows[i]["statuscode"].ToString();
                        objResponse.statusdescription = dst.Tables[0].Rows[i]["statusdescription"].ToString();
                        objResponse.responsecode = dst.Tables[0].Rows[i]["Responsecode"].ToString();
                        objResponse.responsedescription = dst.Tables[0].Rows[i]["ResponseDescription"].ToString();
                        objResponse.displaymessage = dst.Tables[0].Rows[i]["Displaymessage"].ToString();
                        objResponse.bankreferencenumber = dst.Tables[0].Rows[i]["Bankreferencenumber"].ToString();
                        objResponse.amount = dst.Tables[0].Rows[i]["Amount"].ToString();
                        objResponse.customerfee = dst.Tables[0].Rows[i]["CustomerFee"].ToString();
                        objResponse.agentcomm = dst.Tables[0].Rows[i]["AgentComm"].ToString();
                        objResponse.distcomm = dst.Tables[0].Rows[i]["Distcomm"].ToString();
                        objResponse.createddate = dst.Tables[0].Rows[i]["Createddate"].ToString();
                        objResponse.updateddate = dst.Tables[0].Rows[i]["Updateddate"].ToString();
                        objResponse.datekey = dst.Tables[0].Rows[i]["Datekey"].ToString();
                        objResponse.cashoutdttime = dst.Tables[0].Rows[i]["CashoutDttime"].ToString();
                        objResponse.cashoutdatekey = dst.Tables[0].Rows[i]["Cashoutdatekey"].ToString();
                        objResponse.reversaldatetime = dst.Tables[0].Rows[i]["Reversaldatetime"].ToString();
                        objResponse.reversaldatekey = dst.Tables[0].Rows[i]["Reversaldatekey"].ToString();
                        objResponse.bankname = dst.Tables[0].Rows[i]["Bankname"].ToString();
                        objResponse.customermobileno = dst.Tables[0].Rows[i]["Customermobileno"].ToString();
                        objResponse.customername = dst.Tables[0].Rows[i]["Customername"].ToString();
                        objResponse.payeename = dst.Tables[0].Rows[i]["Payeename"].ToString();
                        objResponse.payeeaccountnumer = dst.Tables[0].Rows[i]["Payeeaccountnumer"].ToString();
                        objResponse.amobileno = dst.Tables[0].Rows[i]["Amobileno"].ToString();
                        objResponse.agencyname = dst.Tables[0].Rows[i]["Agencyname"].ToString();
                        objResponse.agentname = dst.Tables[0].Rows[i]["Agentname"].ToString();
                        objResponse.city = dst.Tables[0].Rows[i]["City"].ToString();
                        objResponse.state = dst.Tables[0].Rows[i]["state"].ToString();
                        objResponse.zone = dst.Tables[0].Rows[i]["Zone"].ToString();
                        objResponse.distmobileno = dst.Tables[0].Rows[i]["Distmobileno"].ToString();
                        objResponse.distributorname = dst.Tables[0].Rows[i]["DistributorName"].ToString();
                        objResponse.distcompanyname = dst.Tables[0].Rows[i]["Distcompanyname"].ToString();
                        objResponse.agentip = dst.Tables[0].Rows[i]["agentip"].ToString();
                        objResponse.latitude = dst.Tables[0].Rows[i]["latitude"].ToString();
                        objResponse.longtitude = dst.Tables[0].Rows[i]["longtitude"].ToString();
                        objResponse.agentcommwithouttds = dst.Tables[0].Rows[i]["AgentCommWithoutTDS"].ToString();
                        objResponse.distcommwithouttds = dst.Tables[0].Rows[i]["DistCommWithoutTDS"].ToString();
                        objResponse.IFSC = dst.Tables[0].Rows[i]["IFSCCode"].ToString();
                        LstobjResponse.Add(objResponse);
                    }

                    objresponse.response_code = "200";
                    objresponse.response_message = "Success";
                    objresponse.data = LstobjResponse;

                }
                else
                {
                    objresponse.response_code = "101";
                    objresponse.response_message = "No Records Found";
                    objresponse.data = null;
                }

            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.agent_ref_id);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
            }

            return objresponse;
        }

        public APTGenericResponse? APTGetRetailerTopupReport(GetRetailerTopupReportRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, "Entry", "", Request.agent_ref_id);
            List<GetRetailerTopupReportResponse> LstobjResponse = new List<GetRetailerTopupReportResponse>();
            try
            {
                DataSet dst = _reportRepo.APTGetRetailerTopupReport(Request);
                if (_commonService.IsValidDataSet(dst))
                {
                    for (int i = 0; i < dst.Tables[0].Rows.Count; i++)
                    {

                        
                        
                        GetRetailerTopupReportResponse objResponse = new GetRetailerTopupReportResponse();
                        objResponse.topup_ref_id = dst.Tables[0].Rows[i]["TopupRefID"].ToString();
                        objResponse.account_type_ref_id = dst.Tables[0].Rows[i]["AccountTypeRefID"].ToString();
                        objResponse.account_ref_id = dst.Tables[0].Rows[i]["AccountRefID"].ToString();
                        objResponse.account_name = dst.Tables[0].Rows[i]["AccountName"].ToString();
                        objResponse.account_mobile_number = dst.Tables[0].Rows[i]["AccountMobileNo"].ToString();
                        objResponse.date = dst.Tables[0].Rows[i]["Date"].ToString();
                        objResponse.amount = dst.Tables[0].Rows[i]["Amount"].ToString();
                        objResponse.service_charge = dst.Tables[0].Rows[i]["ServiceCharge"].ToString();
                        objResponse.flat_fee = dst.Tables[0].Rows[i]["FlatFee"].ToString();
                        objResponse.dist_service_tax = dst.Tables[0].Rows[i]["DistServiceTax"].ToString();
                        objResponse.UTR_number = dst.Tables[0].Rows[i]["UTRNo"].ToString();
                        objResponse.deposit_slip_ref_id = dst.Tables[0].Rows[i]["DepositSlipRefID"].ToString();
                        objResponse.agent_ref_id = dst.Tables[0].Rows[i]["AgentRefID"].ToString();
                        objResponse.bank_ref_id = dst.Tables[0].Rows[i]["BankRefID"].ToString();
                        objResponse.topup_type_ref_id = dst.Tables[0].Rows[i]["TopUpTypeRefID"].ToString();
                        objResponse.topup_status_ref_id = dst.Tables[0].Rows[i]["TopupStatusRefID"].ToString();
                        objResponse.approval_reason_ref_id = dst.Tables[0].Rows[i]["ApprovalReasonRefID"].ToString();
                        objResponse.maker_ref_id = dst.Tables[0].Rows[i]["MakerRefID"].ToString();
                        objResponse.topup_mode = dst.Tables[0].Rows[i]["TopupMode"].ToString();
                        objResponse.currency_code = dst.Tables[0].Rows[i]["CurrencyCode"].ToString();
                        objResponse.topup_transaction_id = dst.Tables[0].Rows[i]["TopupTransactionID"].ToString();
                        objResponse.transaction_id = dst.Tables[0].Rows[i]["TransactionID"].ToString();
                        objResponse.approval_transaction_id = dst.Tables[0].Rows[i]["ApprovalTransactionID"].ToString();
                        objResponse.comments = dst.Tables[0].Rows[i]["Comments"].ToString();
                        objResponse.bank_reference_number = dst.Tables[0].Rows[i]["BankReferenceNo"].ToString();
                        objResponse.checker_ref_id = dst.Tables[0].Rows[i]["CheckerRefID"].ToString();
                        objResponse.checker_transdate = dst.Tables[0].Rows[i]["CheckerTransDate"].ToString();
                        objResponse.checker_remarks = dst.Tables[0].Rows[i]["CheckerRemarks"].ToString();
                        objResponse.created_by_type_ref_id = dst.Tables[0].Rows[i]["CreatedByTypeRefID"].ToString();
                        objResponse.status = dst.Tables[0].Rows[i]["Status"].ToString();
                        objResponse.created_by = dst.Tables[0].Rows[i]["CreatedBy"].ToString();
                        objResponse.created_date = dst.Tables[0].Rows[i]["CreatedDate"].ToString();
                        objResponse.updated_by = dst.Tables[0].Rows[i]["UpdatedBy"].ToString();
                        objResponse.updated_date = dst.Tables[0].Rows[i]["UpdatedDate"].ToString();
                        objResponse.new_row = dst.Tables[0].Rows[i]["NewRow"].ToString();
                        objResponse.updated_row = dst.Tables[0].Rows[i]["UpdatedRow"].ToString();
                        objResponse.date_key = dst.Tables[0].Rows[i]["Datekey"].ToString();
                        objResponse.image = dst.Tables[0].Rows[i]["image"].ToString();
                        objResponse.deposit_create_date = dst.Tables[0].Rows[i]["DepositeCreateDate"].ToString();
                        objResponse.reversal_remarks = dst.Tables[0].Rows[i]["ReversalRemarks"].ToString();
                        objResponse.company_name = dst.Tables[0].Rows[i]["Companyname"].ToString();
                        objResponse.Retailer_name = dst.Tables[0].Rows[i]["Retailername"].ToString();
                        objResponse.retailer_mobile_number = dst.Tables[0].Rows[i]["RetMobileno"].ToString();
                        objResponse.t_bankstatement_id = dst.Tables[0].Rows[i]["TBankStatementID"].ToString();
                        objResponse.bankstatement_id = dst.Tables[0].Rows[i]["BankStatementId"].ToString();
                        objResponse.state = dst.Tables[0].Rows[i]["State"].ToString();
                        objResponse.city = dst.Tables[0].Rows[i]["City"].ToString();
                        objResponse.LinkCreateddate = dst.Tables[0].Rows[i]["LinkCreateddate"].ToString();
                        objResponse.LinkprocessedDate = dst.Tables[0].Rows[i]["LinkprocessedDate"].ToString();
                        string orderid = dst.Tables[0].Rows[i]["VendorReferenceid"].ToString();
                        if (orderid.StartsWith("order") || orderid.StartsWith("pay"))
                        {
                            objResponse.UTR_number = dst.Tables[0].Rows[i]["VendorReferenceid"].ToString();
                            objResponse.pg_Receipt_link = "https://login.mypay.biz/PG/PGPages/Razorpaypayment_gateway?TransactionID=" + objResponse.transaction_id + "&VendorPaymentID=" + dst.Tables[0].Rows[i]["VendorReferenceid"].ToString();
                            objResponse.pg_link = "https://login.mypay.biz/PG/PGPages/Razorpaypayment_gateway?TransactionID=" + objResponse.transaction_id;
                        }
                        else if (orderid.ToUpper().StartsWith("CF"))
                        {
                            objResponse.pg_Receipt_link = "https://retailer.vyabari.in/PG/PGPages/payment_gateway?TransactionID=" + objResponse.transaction_id + "&order_id=" + dst.Tables[0].Rows[i]["VendorReferenceid"].ToString();
                            objResponse.pg_link = "https://retailer.vyabari.in/PG/PGPages/payment_gateway?TransactionID=" + objResponse.transaction_id;
                        }
                        
                        //if()
                        //{

                        //}
                        LstobjResponse.Add(objResponse);
                    }

                    objresponse.response_code = "200";
                    objresponse.response_message = "Success";
                    objresponse.data = LstobjResponse;

                }
                else
                {
                    objresponse.response_code = "101";
                    objresponse.response_message = "No Records Found";
                    objresponse.data = null;
                }

            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.agent_ref_id);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
            }

            return objresponse;
        }

        public APTGenericResponse? APTPGTransactionReport(GetRetailerTopupReportRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, "Entry", "", Request.agent_ref_id);
            List<GetPGTransactionReportResponse> LstobjResponse = new List<GetPGTransactionReportResponse>();
            try
            {
                DataSet dst = _reportRepo.APTPGTransactionReport(Request);
                if (_commonService.IsValidDataSet(dst))
                {
                    for (int i = 0; i < dst.Tables[0].Rows.Count; i++)
                    {

                        GetPGTransactionReportResponse objResponse = new GetPGTransactionReportResponse();
                        objResponse.transaction_date = dst.Tables[0].Rows[i]["TransactionDate"].ToString();
                        objResponse.pg_status = dst.Tables[0].Rows[i]["PGStatus"].ToString();
                        objResponse.mobile = dst.Tables[0].Rows[i]["Mobile"].ToString();
                        objResponse.amount = dst.Tables[0].Rows[i]["Amount"].ToString();
                        objResponse.order_id = dst.Tables[0].Rows[i]["OrderID"].ToString();
                        objResponse.payment_id = dst.Tables[0].Rows[i]["PaymentID"].ToString();
                        objResponse.transaction_id = dst.Tables[0].Rows[i]["APTTransactionID"].ToString();
                        objResponse.merchant_name = dst.Tables[0].Rows[i]["MerchantName"].ToString();
                        objResponse.vendor_payment_session_id = dst.Tables[0].Rows[i]["VendorPaymentSessionID"].ToString();
                        objResponse.webhook_response = dst.Tables[0].Rows[i]["webhookrespone"].ToString();
                        objResponse.service_charge = dst.Tables[0].Rows[i]["Servicecharge"].ToString();
                        objResponse.total_fees = dst.Tables[0].Rows[i]["Total Fees"].ToString();
                        objResponse.card_name = dst.Tables[0].Rows[i]["Card Name"].ToString();
                        objResponse.card_sub_type = dst.Tables[0].Rows[i]["Card SubType Name"].ToString();
                        objResponse.vendor_ref_id = dst.Tables[0].Rows[i]["VendorReferenceid"].ToString();
                        objResponse.LinkCreateddate = dst.Tables[0].Rows[i]["LinkCreateddate"].ToString();
                        objResponse.LinkprocessedDate = dst.Tables[0].Rows[i]["LinkprocessedDate"].ToString();
                        string orderid = dst.Tables[0].Rows[i]["VendorReferenceid"].ToString();
                        if (orderid.StartsWith("order") || orderid.StartsWith("pay"))
                        {
                            objResponse.UTR_number = dst.Tables[0].Rows[i]["VendorReferenceid"].ToString();
                            objResponse.pg_Receipt_link = "https://login.mypay.biz/PG/PGPages/Razorpaypayment_gateway?TransactionID=" + objResponse.transaction_id + "&VendorPaymentID=" + dst.Tables[0].Rows[i]["VendorReferenceid"].ToString();
                            objResponse.pg_link = "https://login.mypay.biz/PG/PGPages/Razorpaypayment_gateway?TransactionID=" + objResponse.transaction_id;
                        }
                        else if (orderid.ToUpper().StartsWith("CF"))
                        {
                            objResponse.pg_Receipt_link = "https://retailer.vyabari.in/PG/PGPages/payment_gateway?TransactionID=" + objResponse.transaction_id + "&order_id=" + dst.Tables[0].Rows[i]["VendorReferenceid"].ToString();
                            objResponse.pg_link = "https://retailer.vyabari.in/PG/PGPages/payment_gateway?TransactionID=" + objResponse.transaction_id;
                        }
                        LstobjResponse.Add(objResponse);
                    }

                    objresponse.response_code = "200";
                    objresponse.response_message = "Success";
                    objresponse.data = LstobjResponse;

                }
                else
                {
                    objresponse.response_code = "101";
                    objresponse.response_message = "No Records Found";
                    objresponse.data = null;
                }

            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, Request.agent_ref_id);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
            }

            return objresponse;
        }

        public APTGenericResponse? APTGetCommonDetails(GetRetailerCommonDetailsRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, "Entry", "", "");
            List<GetRetailerCommonDetailsResponse> LstobjResponse = new List<GetRetailerCommonDetailsResponse>();
            try
            {
                DataSet dst = _reportRepo.APTGetCommonDetails(Request);
                if (_commonService.IsValidDataSet(dst))
                {
                    for (int i = 0; i < dst.Tables[0].Rows.Count; i++)
                    {

                        GetRetailerCommonDetailsResponse objResponse = new GetRetailerCommonDetailsResponse();
                        objResponse.agent_ref_id = dst.Tables[0].Rows[i]["Agentrefid"].ToString();
                        objResponse.Customer_name = dst.Tables[0].Rows[i]["Sendername"].ToString();
                        objResponse.Customer_mobile = dst.Tables[0].Rows[i]["Sendermobilenumber"].ToString();
                        objResponse.created_date = dst.Tables[0].Rows[i]["CreatedDate"].ToString();
                        objResponse.transaction_id = dst.Tables[0].Rows[i]["Transactionid"].ToString();
                        objResponse.transaction_type = dst.Tables[0].Rows[i]["Transactiontype"].ToString();
                        objResponse.payment_mode = dst.Tables[0].Rows[i]["Paymentmode"].ToString();
                        objResponse.transaction_amount = dst.Tables[0].Rows[i]["TransactionAmount"].ToString();
                        objResponse.fees = dst.Tables[0].Rows[i]["FEES"].ToString();
                        objResponse.date_key = dst.Tables[0].Rows[i]["Datekey"].ToString();
                        objResponse.settlement_amount = dst.Tables[0].Rows[i]["SettlementAmount"].ToString();
                        objResponse.opening_balance = dst.Tables[0].Rows[i]["OpeningBalance"].ToString();
                        objResponse.transaction_mode = dst.Tables[0].Rows[i]["TransactionMode"].ToString();
                        objResponse.transaction_status = dst.Tables[0].Rows[i]["Status"].ToString();
                        objResponse.closing_balance = dst.Tables[0].Rows[i]["ClosingBalance"].ToString();

                        LstobjResponse.Add(objResponse);
                    }

                    objresponse.response_code = "200";
                    objresponse.response_message = "Success";
                    objresponse.data = LstobjResponse;

                }
                else
                {
                    objresponse.response_code = "101";
                    objresponse.response_message = "No Records Found";
                    objresponse.data = null;
                }

            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, "");

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
            }

            return objresponse;
        }

        public APTGenericResponse? APTGetScrollMesssage(string AgentMobileNumber)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, "Entry", "", AgentMobileNumber);
            GetScrollMessageResponse objResponse = new GetScrollMessageResponse();
            try
            {
                DataSet dst = _reportRepo.APTGetScrollMessage(AgentMobileNumber);
                if (_commonService.IsValidDataSet(dst))
                {
                    objResponse.Message = dst.Tables[0].Rows[0]["Message"].ToString();

                    objresponse.response_code = "200";
                    objresponse.response_message = "Success";
                    objresponse.data = objResponse;

                }
                else
                {
                    objresponse.response_code = "101";
                    objresponse.response_message = "No Scroll message";
                    objresponse.data = null;
                }

            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, AgentMobileNumber);

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
            }

            return objresponse;
        }

        public APTGenericResponse? APTGetCustomerPayoutTxnReport(GetRetailerCustomerPayoutTxnReportRequest Request)
        {
            APTGenericResponse objresponse = new APTGenericResponse();
            _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, "Entry", "", "");
            List<GetRetailerCustomerPayoutTxnReportResponse> LstobjResponse = new List<GetRetailerCustomerPayoutTxnReportResponse>();
            try
            {
                DataSet dst = _reportRepo.APTGetCustomerPayoutTxnReport(Request);
                if (_commonService.IsValidDataSet(dst))
                {
                    for (int i = 0; i < dst.Tables[0].Rows.Count; i++)
                    {

                        GetRetailerCustomerPayoutTxnReportResponse objResponse = new GetRetailerCustomerPayoutTxnReportResponse();
                        objResponse.customer_name = dst.Tables[0].Rows[i]["CustomerName"].ToString();
                        objResponse.customer_mobile = dst.Tables[0].Rows[i]["CustomerMobileNumber"].ToString();
                        objResponse.amount = dst.Tables[0].Rows[i]["Amount"].ToString();
                        objResponse.payee_name = dst.Tables[0].Rows[i]["PayeeName"].ToString();
                        objResponse.account_number = dst.Tables[0].Rows[i]["AccountNumber"].ToString();
                        objResponse.ifsc = dst.Tables[0].Rows[i]["ifsccode"].ToString();
                        objResponse.transaction_id = dst.Tables[0].Rows[i]["TransactionID"].ToString();
                        objResponse.payment_mode = dst.Tables[0].Rows[i]["PaymentMode"].ToString();
                        objResponse.payment_category_type = dst.Tables[0].Rows[i]["PaymentCategoryType"].ToString();
                        objResponse.settlement_type = dst.Tables[0].Rows[i]["SettlementType"].ToString();
                        objResponse.bank_name = dst.Tables[0].Rows[i]["BankName"].ToString();
                        objResponse.branch_name = dst.Tables[0].Rows[i]["BranchName"].ToString();
                        objResponse.transaction_status = dst.Tables[0].Rows[i]["TransactionStatus"].ToString();

                        objResponse.payment_transaction_ref_id = dst.Tables[0].Rows[i]["PaymentTransactionRefID"].ToString();
                        objResponse.transaction_id = dst.Tables[0].Rows[i]["TransactionID"].ToString();
                        objResponse.amount = dst.Tables[0].Rows[i]["Amount"].ToString();
                        objResponse.platform_charge = dst.Tables[0].Rows[i]["PlatformCharge"].ToString();
                        objResponse.settlement_charge = dst.Tables[0].Rows[i]["SettlementCharge"].ToString();
                        objResponse.pg_platform_bank_id = dst.Tables[0].Rows[i]["PGPlatformBankID"].ToString();
                        objResponse.payment_category_type = dst.Tables[0].Rows[i]["PaymentCategoryType"].ToString();
                        objResponse.settlement_type = dst.Tables[0].Rows[i]["SettlementType"].ToString();
                        objResponse.payment_mode = dst.Tables[0].Rows[i]["PaymentMode"].ToString();
                        objResponse.transaction_status_ref_id = dst.Tables[0].Rows[i]["TransactionStatusRefID"].ToString();
                        objResponse.approval_status_ref_id = dst.Tables[0].Rows[i]["ApprovalStatusRefID"].ToString();
                        objResponse.pg_status_ref_id = dst.Tables[0].Rows[i]["PGStatusRefID"].ToString();
                        objResponse.remarks = dst.Tables[0].Rows[i]["Remarks"].ToString();
                        objResponse.pg_transaction_id = dst.Tables[0].Rows[i]["PGTransactionID"].ToString();
                        objResponse.approval_date = dst.Tables[0].Rows[i]["ApprovalDate"].ToString();
                        objResponse.pg_updated_date = dst.Tables[0].Rows[i]["PGUpdatedDate"].ToString();
                        objResponse.customer_ref_id = dst.Tables[0].Rows[i]["CutomerRefID"].ToString();
                        objResponse.payee_ref_id = dst.Tables[0].Rows[i]["PayeeRefID"].ToString();
                        objResponse.agent_code = dst.Tables[0].Rows[i]["AgentCode"].ToString();
                        objResponse.created_by = dst.Tables[0].Rows[i]["CreatedBy"].ToString();
                        objResponse.created_date = dst.Tables[0].Rows[i]["CreatedDate"].ToString();
                        objResponse.updated_by = dst.Tables[0].Rows[i]["UpdatedBy"].ToString();
                        objResponse.updated_date = dst.Tables[0].Rows[i]["UpdatedDate"].ToString();
                        objResponse.ifsc = dst.Tables[0].Rows[i]["ifsccode"].ToString();
                        objResponse.agency_name = dst.Tables[0].Rows[i]["Agencyname"].ToString();
                        objResponse.agent_name = dst.Tables[0].Rows[i]["Agentname"].ToString();
                        objResponse.city_ref_id = dst.Tables[0].Rows[i]["CityRefid"].ToString();
                        objResponse.city = dst.Tables[0].Rows[i]["City"].ToString();
                        objResponse.state_ref_id = dst.Tables[0].Rows[i]["Staterefid"].ToString();
                        objResponse.state = dst.Tables[0].Rows[i]["state"].ToString();
                        objResponse.zone_ref_id = dst.Tables[0].Rows[i]["Zonerefid"].ToString();
                        objResponse.zone = dst.Tables[0].Rows[i]["Zone"].ToString();
                        objResponse.dist_ref_id = dst.Tables[0].Rows[i]["Distributorrefid"].ToString();
                        objResponse.dist_mobile_number = dst.Tables[0].Rows[i]["Distmobileno"].ToString();
                        objResponse.dist_name = dst.Tables[0].Rows[i]["DistributorName"].ToString();
                        objResponse.dist_company_name = dst.Tables[0].Rows[i]["Distcompanyname"].ToString();
                        objResponse.request = dst.Tables[0].Rows[i]["Request"].ToString();
                        objResponse.response = dst.Tables[0].Rows[i]["Response"].ToString();
                        objResponse.bank_name = dst.Tables[0].Rows[i]["BankName"].ToString();
                        objResponse.branch_name = dst.Tables[0].Rows[i]["BranchName"].ToString();
                        objResponse.date_key = dst.Tables[0].Rows[i]["Datekey"].ToString();
                        objResponse.customer_name = dst.Tables[0].Rows[i]["CustomerName"].ToString();
                        objResponse.payee_name = dst.Tables[0].Rows[i]["PayeeName"].ToString();
                        objResponse.customer_mobiel_number = dst.Tables[0].Rows[i]["CustomerMobileNumber"].ToString();
                        objResponse.payee_mobile_number = dst.Tables[0].Rows[i]["PayeeMobileNumber"].ToString();
                        objResponse.account_number = dst.Tables[0].Rows[i]["AccountNumber"].ToString();
                        objResponse.vendor_ref_id = dst.Tables[0].Rows[i]["VendorRefID"].ToString();
                        objResponse.vendor_name = dst.Tables[0].Rows[i]["VendorName"].ToString();
                        objResponse.vendor_order_id = dst.Tables[0].Rows[i]["VendorOrderID"].ToString();
                        objResponse.service_id = dst.Tables[0].Rows[i]["ServiceID"].ToString();
                        objResponse.service_description = dst.Tables[0].Rows[i]["ServiceDescription"].ToString();
                        objResponse.vendor_payment_session_id = dst.Tables[0].Rows[i]["VendorPaymentSessionID"].ToString();
                        objResponse.webhook_response = dst.Tables[0].Rows[i]["WebhookResponse"].ToString();
                        objResponse.vendor_ref_number = dst.Tables[0].Rows[i]["VendorRefNo"].ToString();
                        objResponse.vendor_Payment_id = dst.Tables[0].Rows[i]["VendorPaymentID"].ToString();
                        objResponse.bank_reference_number = dst.Tables[0].Rows[i]["BankReferrenceNumber"].ToString();
                        objResponse.id_proof_file_number = dst.Tables[0].Rows[i]["idprooffilerefnumber"].ToString();
                        objResponse.id_proof_image_filepath = dst.Tables[0].Rows[i]["idproofimageFilepath"].ToString();
                        objResponse.id_proof_front_file = dst.Tables[0].Rows[i]["idproofimageFRONTFilname"].ToString();
                        objResponse.id_proof_back_file = dst.Tables[0].Rows[i]["idproofimageBACKFilname"].ToString();
                        objResponse.credit_card_number = dst.Tables[0].Rows[i]["Creditcardnumber"].ToString();
                        objResponse.credit_card_image = dst.Tables[0].Rows[i]["CreditcardimagePath"].ToString();
                        objResponse.credit_card_image_front = dst.Tables[0].Rows[i]["CreditcardimageFrontFilname"].ToString();
                        objResponse.transaction_status = dst.Tables[0].Rows[i]["TransactionStatus"].ToString();
                        objResponse.kyc_status = dst.Tables[0].Rows[i]["Kycstatus"].ToString();


                        LstobjResponse.Add(objResponse);
                    }

                    objresponse.response_code = "200";
                    objresponse.response_message = "Success";
                    objresponse.data = LstobjResponse;

                }
                else
                {
                    objresponse.response_code = "101";
                    objresponse.response_message = "No Records Found";
                    objresponse.data = null;
                }

            }
            catch (Exception ex)
            {
                _log.WriteExceptionLog(MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace, "");

                objresponse.response_code = APIResponseCode.Exception;
                objresponse.response_message = APIResponseCodeDesc.Exception;
                objresponse.data = null;
            }

            return objresponse;
        }
        #endregion Reports
    }
}
