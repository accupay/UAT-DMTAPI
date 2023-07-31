using APT.DMT.API.BusinessLogic.BankService;
using APT.DMT.API.DataAccessLayer.DMT;

namespace APT.DMT.API.BusinessLogic
{
    public class LogService
    {



      
        #region LogWriting
        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                    sw.WriteLine("-----------------------------------------------------------------------------------------------------");
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                    sw.WriteLine("-----------------------------------------------------------------------------------------------------");

                }
            }
        }

        public void WriteLog(string MethodName, string Message, string data, string MobileNo)
        {
            try
            {
                WriteToFile(DateTime.Now + " Method Name : " + MethodName + " Message : " + Message + " Data :" + data + " . Mobile No : " + MobileNo);
                // WriteToFile("-----------------------------------------------------------------------------------------------------");
            }
            catch(Exception ex)
            {

            }
        }

        public void WriteAPILog(string MethodName, string Message, string URL, string Request, string Response)
        {
            try
            {
                WriteToFile(DateTime.Now + " Method Name : " + MethodName + " Message : " + Message + " URL : " + URL + " Request :" + Request + " . Response : " + Response);
                // WriteToFile("-----------------------------------------------------------------------------------------------------");
            }
            catch (Exception ex)
            {

            }
        }
        public void WriteExceptionLog(string MethodName, string ExMessage, string StackTrace, string MobileNo)
        {
            try
            {
                WriteToFile(DateTime.Now + "Exception Method Name : " + MethodName + " Exception Message : " + ExMessage + " StackTrace :" + StackTrace + " . Mobile No : " + MobileNo);
                // WriteToFile("-----------------------------------------------------------------------------------------------------");
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }
}
