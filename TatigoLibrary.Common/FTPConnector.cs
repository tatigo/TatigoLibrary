using Limilabs.FTP.Client;
using TatigoLibrary.Common.DesignByContract;
using System;
using System.Collections.Generic;
using System.IO;

namespace TatigoLibrary.Common
{
    public class FTPClient
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public FTPClient(string InHostIP, string InFolderPath, int? InPort, string InUserName, string InPass)
        {
            Check.Require(!string.IsNullOrEmpty(InHostIP));

            this.HostIP = InHostIP;
            this.FolderPath = InFolderPath;
            this.Port = InPort;
            this.UserName = InUserName;
            this.Pass = InPass;
        }

        public string RemoteHost
        {
            get
            {
                if (this.Port.HasValue)
                    return @"ftp://" + this.HostIP + ":" + this.Port.Value.ToString() + "/" + this.FolderPath;
                return @"ftp://" + this.HostIP + "/" + this.FolderPath;
            }
        }

        private string FolderPath { get; set; }
        private string HostIP { get; set; }
        private string Pass { get; set; }
        private int? Port { get; set; }
        private string UserName { get; set; }

        public void DeleteFile(string InFileName)
        {
            Check.Require(!string.IsNullOrEmpty(InFileName));

            Ftp ftp = null;
            try
            {
                using (ftp = new Ftp())
                {
                    this.FTPConnect(ftp);

                    var remoteFilePath = Path.Combine(this.FolderPath, InFileName);
                    log.Info(string.Format("Deleting from [{0}]", remoteFilePath));
                    ftp.DeleteFile(InFileName);
                }
            }
            catch (Exception ex)
            {
                log.Error((string.Format("Failed deleting file [{0}] from [{1}]", InFileName)), ex);
                throw ex;
            }
            finally
            {
                if (ftp != null)
                    ftp.Close();
            }
        }

        public void DownloadFile(string InFileName, string InLocalDestinationPath)
        {
            Check.Require(!string.IsNullOrEmpty(InFileName));
            Check.Require(!string.IsNullOrEmpty(InLocalDestinationPath));

            Ftp ftp = null;
            try
            {
                using (ftp = new Ftp())
                {
                    this.FTPConnect(ftp);

                    var remoteFilePath = Path.Combine(this.FolderPath, InFileName);
                    log.Info(string.Format("Downloading from [{0}] to [{1}]", remoteFilePath, InLocalDestinationPath));
                    ftp.Download(InFileName, InLocalDestinationPath);
                }
            }
            catch (Exception ex)
            {
                log.Error((string.Format("Failed downloading file [{0}] to [{1}]", InFileName, InLocalDestinationPath)), ex);
                throw ex;
            }
            finally
            {
                if (ftp != null)
                    ftp.Close();
            }
        }

        public void DownloadFiles(List<string> InFileNames, string InLocalDestinationFolderPath, bool InToDeleteOriginal)
        {
            Check.Require(InFileNames != null);
            Check.Require(!string.IsNullOrEmpty(InLocalDestinationFolderPath));

            Ftp ftp = null;
            try
            {
                using (ftp = new Ftp())
                {
                    this.FTPConnect(ftp);

                    foreach (var fileName in InFileNames)
                    {
                        try
                        {
                            var destinationFileNameFull = Path.Combine(InLocalDestinationFolderPath, fileName);
                            var remoteFilePath = Path.Combine(this.FolderPath, fileName);

                            log.Info(string.Format("Downloading from [{0}] to [{1}]", remoteFilePath, destinationFileNameFull));

                            if (File.Exists(destinationFileNameFull)) //Clear the temp folder from previous attempts
                                File.Delete(destinationFileNameFull);

                            ftp.Download(fileName, destinationFileNameFull);
                            Check.Ensure(File.Exists(destinationFileNameFull), string.Format("File [{0}] should exist in [{1}]", fileName, InLocalDestinationFolderPath));

                            //Delete the original file if the file was copied to destination
                            if (InToDeleteOriginal)
                            {
                                try
                                {
                                    log.Info(string.Format("Deleting from [{0}]", remoteFilePath));
                                    ftp.DeleteFile(fileName);
                                }
                                catch (Exception ex)
                                {
                                    log.Error((string.Format("Failed deleting file [{0}]", remoteFilePath)), ex);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error((string.Format("Failed downloading file [{0}] to [{1}]", fileName, InLocalDestinationFolderPath)), ex);
                            throw ex;
                        }
                    }
                }
            }
            finally
            {
                if (ftp != null)
                    ftp.Close();
            }
        }

        public List<string> ListDirectory()
        {
            Ftp ftp = null;
            try
            {
                using (ftp = new Ftp())
                {
                    this.FTPConnect(ftp);

                    var list = ftp.GetList().ConvertAll<string>(s => s.Name);

                    log.Info(string.Format("Found in directory listing [{0}]", list.Count));
                    list.ForEach(l => log.Info(l + ';'));

                    return list;
                }
            }
            catch (Exception ex)
            {
                log.Error((string.Format("Failed accessing FTP [{0}]", this.HostIP)), ex);
                throw ex;
            }
            finally
            {
                if (ftp != null)
                    ftp.Close();
            }
        }

        private void FTPConnect(Ftp ftp)
        {
            //Limilabs.FTP.Log.Enabled = true;

            ftp.Connect(this.HostIP);
            log.Info(string.Format("Connected to FTP [{0}]", this.HostIP));

            try
            {
                ftp.Login(this.UserName, this.Pass);
                log.Info(string.Format("Loggedin using [{0}] [{1}]", this.UserName, this.Pass));
            }
            catch
            {
                ftp.LoginAnonymous();
                log.Info("Login Anonymous");
            }

            if (!string.IsNullOrEmpty(this.FolderPath))
                ftp.ChangeFolder(this.FolderPath);
        }
    }
}