using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using TatigoLibrary.Common.DesignByContract;

namespace TatigoLibrary.Common
{
    public static class Utils
    {
        private static Random random;

        static Utils()
        {
            Utils.random = new Random();
        }

        public static string CreateJson(IDictionary<string, string> InPropertyValues)
        {
            Check.Require(InPropertyValues != null, "InPropertyValues must be provided");
            JObject jObject = new JObject();
            foreach (KeyValuePair<string, string> inPropertyValue in InPropertyValues)
            {
                jObject.Add(inPropertyValue.Key, inPropertyValue.Value);
            }
            return jObject.ToString();
        }

        public static MailMessage CreateMailMessage(string InSenderAddress, string InToAddress, string InToBCC, string InMailSubject, string InMailBody)
        {
            if (string.IsNullOrEmpty(InSenderAddress))
            {
                throw new ArgumentNullException("InSenderAddress");
            }
            if (string.IsNullOrEmpty(InMailSubject))
            {
                throw new ArgumentNullException("InMailSubject");
            }
            if (string.IsNullOrEmpty(InMailBody))
            {
                throw new ArgumentNullException("InMailBody");
            }
            MailMessage mailMessage = new MailMessage()
            {
                From = new MailAddress(InSenderAddress),
                Sender = new MailAddress(InSenderAddress)
            };
            if (!string.IsNullOrEmpty(InToAddress))
            {
                mailMessage.To.Add(InToAddress);
            }
            if (!string.IsNullOrEmpty(InToBCC))
            {
                mailMessage.Bcc.Add(InToBCC);
            }
            mailMessage.SubjectEncoding = Encoding.UTF8;
            mailMessage.Subject = Utils.StripWhiteSpace(InMailSubject, true);
            mailMessage.BodyEncoding = Encoding.UTF8;
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(Utils.HTMLConvertToText(InMailBody), null, "text/plain");
            mailMessage.AlternateViews.Add(alternateView);
            if (mailMessage.IsBodyHtml)
            {
                AlternateView alternateView1 = AlternateView.CreateAlternateViewFromString(InMailBody, null, "text/html");
                mailMessage.AlternateViews.Add(alternateView1);
            }
            return mailMessage;
        }

        public static string DecryptPassword(string password)
        {
            byte[] numArray = null;
            try
            {
                numArray = Convert.FromBase64String(password);
            }
            catch
            {
                string str = password.Trim().Replace('-', '+');
                numArray = Convert.FromBase64String(str.Replace('\u005F', '/'));
            }
            PasswordDeriveBytes passwordDeriveByte = new PasswordDeriveBytes("SSIEncryptionKey", new byte[] { 73, 118, 97, 110, 32, 77, 101, 100, 118, 101, 100, 101, 118 });
            MemoryStream memoryStream = new MemoryStream();
            Rijndael bytes = Rijndael.Create();
            bytes.Key = passwordDeriveByte.GetBytes(32);
            bytes.IV = passwordDeriveByte.GetBytes(16);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, bytes.CreateDecryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(numArray, 0, (int)numArray.Length);
            cryptoStream.Close();
            byte[] array = memoryStream.ToArray();
            return Encoding.Unicode.GetString(array);
        }

        public static string EncryptPassword(string password)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(password);
            PasswordDeriveBytes passwordDeriveByte = new PasswordDeriveBytes("SSIEncryptionKey", new byte[] { 73, 118, 97, 110, 32, 77, 101, 100, 118, 101, 100, 101, 118 });
            MemoryStream memoryStream = new MemoryStream();
            Rijndael rijndael = Rijndael.Create();
            rijndael.Key = passwordDeriveByte.GetBytes(32);
            rijndael.IV = passwordDeriveByte.GetBytes(16);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndael.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(bytes, 0, (int)bytes.Length);
            cryptoStream.Close();
            return Convert.ToBase64String(memoryStream.ToArray());
        }
       
        public static byte[] FromBase64String(string In64BaseString)
        {
            char chr = '=';
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;
            object[] objArray = new object[] { chr };
            string str = string.Format(invariantCulture, "{0}{0}", objArray);
            char chr1 = '+';
            char chr2 = '/';
            char chr3 = '-';
            char chr4 = '\u005F';
            string in64BaseString = In64BaseString;
            in64BaseString = in64BaseString.Replace(chr3, chr1);
            in64BaseString = in64BaseString.Replace(chr4, chr2);
            switch (in64BaseString.Length % 4)
            {
                case 0:
                    {
                        break;
                    }
                case 1:
                    {
                        throw new ArgumentException("Illegal base64url string!", In64BaseString);
                    }
                case 2:
                    {
                        in64BaseString = string.Concat(in64BaseString, str);
                        break;
                    }
                case 3:
                    {
                        in64BaseString = string.Concat(in64BaseString, chr);
                        break;
                    }
                default:
                    {
                        throw new ArgumentException("Illegal base64url string!", In64BaseString);
                    }
            }
            return Convert.FromBase64String(in64BaseString);
        }

        public static string GenerateExistingFileNameWindowsWay(string InFilePath)
        {
            Check.Require(!string.IsNullOrEmpty(InFilePath), "InFilePath must be provided");

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(InFilePath);
            string extension = Path.GetExtension(InFilePath);
            string directoryName = Path.GetDirectoryName(InFilePath);

            string i;
            string str = null;
            int num = 1;
            for (i = InFilePath; File.Exists(i); i = Path.Combine(directoryName, string.Concat(str, extension)))
            {
                var num1 = num;
                num = num1 + 1;
                str = string.Format("{0}({1})", fileNameWithoutExtension, num1);
            }
            return i;
        }

        public static string GenerateYearMonthDaySubfolder(string InRootDirectory, DateTime InDate)
        {
            return Path.Combine(Path.Combine(Path.Combine(InRootDirectory, InDate.Year.ToString()), InDate.Month.ToString()), InDate.Day.ToString());
        }

        public static byte[] GetBytes(string InStr)
        {
            byte[] numArray = new byte[InStr.Length * 2];
            Buffer.BlockCopy(InStr.ToCharArray(), 0, numArray, 0, (int)numArray.Length);
            return numArray;
        }

        public static string GetString(byte[] bytes)
        {
            char[] chrArray = new char[(int)bytes.Length / 2];
            Buffer.BlockCopy(bytes, 0, chrArray, 0, (int)bytes.Length);
            return new string(chrArray);
        }

        public static string GetText(byte[] InBase64Bytes)
        {
            return Encoding.UTF8.GetString(InBase64Bytes);
        }

        public static byte[] GetTextBytes(string InStr)
        {
            return Encoding.UTF8.GetBytes(InStr);
        }

        public static bool GuidTryParse(string InGuidString, out Guid OutGuidResult)
        {
            bool flag;
            if (!string.IsNullOrEmpty(InGuidString))
            {
                if ((new Regex("^[A-Fa-f0-9]{32}$|^({|\\()?[A-Fa-f0-9]{8}-([A-Fa-f0-9]{4}-){3}[A-Fa-f0-9]{12}(}|\\))?$|^({)?[0xA-Fa-f0-9]{3,10}(, {0,1}[0xA-Fa-f0-9]{3,6}){2}, {0,1}({)([0xA-Fa-f0-9]{3,4}, {0,1}){7}[0xA-Fa-f0-9]{3,4}(}})$")).Match(InGuidString).Success)
                {
                    OutGuidResult = new Guid(InGuidString);
                    flag = true;
                    return flag;
                }
            }
            OutGuidResult = Guid.Empty;
            flag = false;
            return flag;
        }

        public static string HTMLConvertToText(string InHTML)
        {
            string str = InHTML.Replace("<br />", Environment.NewLine);
            str = str.Replace("<br>", Environment.NewLine);
            str = Regex.Replace(str, "\\s*<\\s*[bB][rR]\\s*/\\s*>\\s*", Environment.NewLine);
            return Regex.Replace(str, "<[^>]*>", " ");
        }

        public static IDictionary<string, string> ParseJson(string InJson, ICollection<string> InProperties)
        {
            Check.Require(!string.IsNullOrEmpty(InJson), "InJson must be provided");
            Check.Require(InProperties != null, "InProperties must be provided");
            if (InJson.Contains("\r\n"))
            {
                InJson = InJson.Replace("\r\n", string.Empty);
            }
            JToken jToken = JObject.Parse(InJson);
            Check.Assert(jToken != null, "jtoken must be created");
            Dictionary<string, string> strs = new Dictionary<string, string>();
            foreach (string inProperty in InProperties)
            {
                strs.Add(inProperty, (string)jToken.SelectToken(inProperty));
            }
            return strs;
        }

        public static string RandomCharacter(int MinCharCode, int MaxCharCode)
        {
            char chr = Convert.ToChar(Utils.RandomNumber(MinCharCode, MaxCharCode));
            return chr.ToString();
        }

        public static int RandomNumber(int MinValue, int MaxValue)
        {
            int num = checked((int)Math.Round((double)((float)MinValue + (float)Convert.ToInt32(Utils.random.NextDouble() * (double)((float)(checked(checked(MaxValue - MinValue) + 1)))))));
            return num;
        }

        public static string RandomString(int MinLength, int MaxLength)
        {
            int num = checked((int)Math.Round((double)((float)MinLength + (float)Convert.ToInt32(Utils.random.NextDouble() * (double)((float)(checked(checked(MaxLength - MinLength) + 1)))))));
            int num1 = 0;
            string empty = string.Empty;
            while (num1 < num)
            {
                empty = string.Concat(empty, ((Utils.random.NextDouble() > 0.5 ? Utils.RandomCharacter(65, 90) : Utils.RandomCharacter(97, 122))).ToString());
                num1++;
            }
            return empty;
        }

        public static string ReplacePlaceHolders(string sourceText, object InObject)
        {
            PropertyInfo[] properties = InObject.GetType().GetProperties();
            for (int i = 0; i < (int)properties.Length; i++)
            {
                PropertyInfo propertyInfo = properties[i];
                string str = string.Concat("${", propertyInfo.Name, "}");
                if (sourceText.ToLower().Contains(str.ToLower()))
                {
                    object value = propertyInfo.GetValue(InObject, null);
                    if (value != null)
                    {
                        sourceText = sourceText.Replace(str, value.ToString());
                    }
                }
            }
            return sourceText;
        }

        public static string SendEmail(MailMessage InMailMessage, string InSMTPServer, NetworkCredential InNetworkCredentials, bool InIsEnableSSL = false, IEnumerable<Attachment> InMailAttachments = null)
        {
            string str;
            if (InMailMessage == null)
            {
                throw new ArgumentNullException("InMailMessage");
            }
            if (string.IsNullOrEmpty(InSMTPServer))
            {
                throw new ArgumentNullException("InSMTPServer");
            }
            if (InMailAttachments != null)
            {
                foreach (Attachment inMailAttachment in InMailAttachments)
                {
                    InMailMessage.Attachments.Add(inMailAttachment);
                }
            }
            try
            {
                try
                {
                    SmtpClient smtpClient = new SmtpClient();
                    string[] strArrays = InSMTPServer.Split(new char[] { ':' });
                    smtpClient.Host = strArrays[0];
                    if ((int)strArrays.Length > 1)
                    {
                        smtpClient.Port = Convert.ToInt32(strArrays[1]);
                    }
                    if (InNetworkCredentials == null)
                    {
                        smtpClient.UseDefaultCredentials = true;
                    }
                    else
                    {
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Credentials = InNetworkCredentials;
                    }
                    smtpClient.EnableSsl = InIsEnableSSL;
                    smtpClient.Send(InMailMessage);
                    str = "";
                }
                catch (SmtpFailedRecipientException smtpFailedRecipientException1)
                {
                    SmtpFailedRecipientException smtpFailedRecipientException = smtpFailedRecipientException1;
                    str = string.Format("FailedRecipient {0}", smtpFailedRecipientException.FailedRecipient);
                    throw new Exception(str, smtpFailedRecipientException);
                }
                catch (SmtpException smtpException1)
                {
                    SmtpException smtpException = smtpException1;
                    str = string.Format("SMTPConfigurationProblem {0}", smtpException.Message);
                    throw new Exception(str, smtpException);
                }
                catch (Exception exception1)
                {
                    Exception exception = exception1;
                    str = (exception.InnerException == null ? exception.Message : string.Concat(exception.Message, Environment.NewLine, exception.InnerException.Message));
                    throw new Exception(str, exception);
                }
            }
            finally
            {
                InMailMessage.Dispose();
            }
            return str;
        }

        public static string SerializeToJson(object InDataObj)
        {
            Check.Require(InDataObj != null, "InDataObj must be provided");
            return JsonConvert.SerializeObject(InDataObj);
        }

        public static string StripWhiteSpace(string InString, bool InRetainSpace)
        {
            string str;
            string str1;
            str = (!InRetainSpace ? "" : " ");
            str1 = (!string.IsNullOrEmpty(InString) ? Regex.Replace(InString, "\\s+", str) : "");
            return str1;
        }

        #region Streams
        public static byte[] StreamToByteArrayUsingMemoryStream(Stream InStream)
        {
            byte[] array;
            byte[] numArray = new byte[32768];
            MemoryStream memoryStream = new MemoryStream();
            try
            {
                while (true)
                {
                    int num = InStream.Read(numArray, 0, (int)numArray.Length);
                    if (num <= 0)
                    {
                        break;
                    }
                    memoryStream.Write(numArray, 0, num);
                }
                array = memoryStream.ToArray();
            }
            finally
            {
                if (memoryStream != null)
                {
                    ((IDisposable)memoryStream).Dispose();
                }
            }
            return array;
        }

        public static byte[] StreamToByteArrayUsingBuffer(Stream InStream)
        {
            byte[] numArray;
            long position = (long)0;
            if (InStream.CanSeek)
            {
                position = InStream.Position;
                InStream.Position = (long)0;
            }
            try
            {
                byte[] numArray1 = new byte[4096];
                int num = 0;
                while (true)
                {
                    int num1 = InStream.Read(numArray1, num, (int)numArray1.Length - num);
                    int num2 = num1;
                    if (num1 <= 0)
                    {
                        break;
                    }
                    num = num + num2;
                    if (num == (int)numArray1.Length)
                    {
                        int num3 = InStream.ReadByte();
                        if (num3 != -1)
                        {
                            byte[] numArray2 = new byte[(int)numArray1.Length * 2];
                            Buffer.BlockCopy(numArray1, 0, numArray2, 0, (int)numArray1.Length);
                            Buffer.SetByte(numArray2, num, (byte)num3);
                            numArray1 = numArray2;
                            num++;
                        }
                    }
                }
                byte[] numArray3 = numArray1;
                if ((int)numArray1.Length != num)
                {
                    numArray3 = new byte[num];
                    Buffer.BlockCopy(numArray1, 0, numArray3, 0, num);
                }
                numArray = numArray3;
            }
            finally
            {
                if (InStream.CanSeek)
                {
                    InStream.Position = position;
                }
            }
            return numArray;
        }

        public static void StreamWriteBytes(Stream from, Stream to)
        {
            for (int a = from.ReadByte(); a != -1; a = from.ReadByte())
                to.WriteByte((byte)a);
        }

        public static void StreamToFileByBytes(Stream stream, string filepath)
        {
            using (Stream destination = File.Create(filepath))
                StreamWriteBytes(stream, destination);
        }

        public static void StreamToFile(Stream stream, string filepath)
        {
            byte[] buffer = new byte[2048];

            using (var fileStream = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                int bytesRead = 0;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fileStream.Write(buffer, 0, bytesRead);
                }
            }
        }

        public static List<string> StreamToStrings(Stream stream)
        {
            var result = new List<string>();

            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                    result.Add(reader.ReadLine());
            }

            return result;
        }
        #endregion

        #region uses dynamic
        //public static dynamic DeserializeJson(string InJson)
        //{
        //    Check.Require(!string.IsNullOrEmpty(InJson), "InJson must be provided");
        //    if (InJson.Contains("\r\n"))
        //    {
        //        InJson = InJson.Replace("\r\n", string.Empty);
        //    }
        //    return JsonConvert.DeserializeObject(InJson);
        //}

        //public static dynamic ParseJWTEncodedToken(string InEncodedToken)
        //{
        //    Check.Require(!string.IsNullOrEmpty(InEncodedToken), "InEncodedToken must be provided");
        //    string[] strArrays = InEncodedToken.Split(new char[] { '.' });
        //    Check.Assert((int)strArrays.Length == 3, "JWT must have three parts.");
        //    byte[] numArray = Utils.FromBase64String(strArrays[1]);
        //    return JsonConvert.DeserializeObject(Utils.GetText(numArray));
        //}
        #endregion
    }
}
