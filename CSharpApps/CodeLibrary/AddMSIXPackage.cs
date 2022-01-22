using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using System.Text;
using System.Threading;
using System.Xml;

namespace CodeLibrary
{
    class AddMSIXPackage
    {
        public void Test()
        {
            //Thread t1 = new Thread(ProcessAction01);
            //t1.Name = "t1";
            //t1.Start();
            //Thread t2 = new Thread(ProcessAction012);
            //t2.Name = "t2";
            //t2.Start();

            //t1.Join();
            //t2.Join();

            
            //bool sideLoadNeeded = IsSideLoadNeeded();

            //string baseDirPath = AppDomain.CurrentDomain.BaseDirectory;
            //baseDirPath = baseDirPath.Replace(@"\bin\Debug", "");

            //string testDataPath = baseDirPath + @"\TestData\msixFiles";
            //ExecuteAsAdmin();

            //CheckMSIXExistsAndRemove();

        }

        private static void ProcessAction01()
        {
            StatTest.Increment();
            for (int i = 0; i < 10; i++)
            {
                StatTest.Increment();
                Thread.Sleep(1000);
                StatTest.Decrement();
            }
            StatTest.Decrement();
        }

        private static void ProcessAction012()
        {
            StatTest.Increment();
            Thread.Sleep(1000);
            StatTest.Decrement();
            
        }

        private static bool IsSideLoadNeeded()
        {
            bool sideloadneeded = false;
            bool oscheck = OSVersionCheck();
            bool appmodelkey = AppModelUnlockRegistryCheck();
            if(OSVersionCheck() && !AppModelUnlockRegistryCheck())
            {
                sideloadneeded = true;
            }
            return sideloadneeded;
        }
        private static bool AppModelUnlockRegistryCheck()
        {
            bool sideLoadNotNeeded = true;


            RegistryKey appModelUnlockKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\AppModelUnlock",true);
            if(appModelUnlockKey!=null)
            {
                
                object allowAllTrustedApps=appModelUnlockKey.GetValue("AllowAllTrustedApps");
                //appModelUnlockKey.SetValue("ja", 1, RegistryValueKind.DWord);
                if(allowAllTrustedApps!=null)
                {
                    if(Convert.ToInt32(allowAllTrustedApps)==1)
                    {
                        sideLoadNotNeeded = false;
                    }
                }
            }
            

            return sideLoadNotNeeded;
        }
        private static bool OSVersionCheck()
        {
            bool sideloadneeded = false;

            string osversion = GetOSVersionString();
            if (!string.IsNullOrEmpty(osversion))
            {
                string[] os = osversion.Split('.');
                if (os.Length > 3)
                {
                    string major = os[0];
                    string build = os[2];
                    if (Convert.ToInt32(major) >= 10 && Convert.ToInt32(build) <= 2004)
                    {
                        sideloadneeded = true;
                    }
                }
            }

            return sideloadneeded;
        }

        private static string GetOSMappingVersion()
        {

            RegistryKey hKey = null;
            string osMappedVersion = string.Empty;
            try
            {
                hKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Novell\\ZCM");

                if (hKey != null)
                {
                    osMappedVersion = Convert.ToString(hKey.GetValue("OSMappedVersion"));
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                if (hKey != null)
                {
                    hKey.Close();
                }
            }
            return osMappedVersion;
        }

        public static string GetOSVersionString()
        {
            Version OSVersion = Environment.OSVersion.Version;
            string OSVersionString = OSVersion.ToString();
            string Win10_20H2 = "20H2";

            if (OSVersion.Major == 6 && OSVersion.Minor == 2)
            {
                // It may be 6.3, check registry
                RegistryKey currentVersionKey = null;
                try
                {

                    currentVersionKey = Registry.LocalMachine.OpenSubKey(
                          @"SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion");
                    if (currentVersionKey != null)
                    {
                        string MajorOSVersion;
                        string MinorOSVersion;
                        string currentVersion;

                        MajorOSVersion = Convert.ToString(currentVersionKey.GetValue("CurrentMajorVersionNumber"));

                        if (!String.IsNullOrEmpty(MajorOSVersion))
                        {
                            MinorOSVersion = Convert.ToString(currentVersionKey.GetValue("CurrentMinorVersionNumber"));
                            if (String.IsNullOrEmpty(MinorOSVersion))
                            {
                                MinorOSVersion = "0";
                            }
                            OSVersionString = MajorOSVersion + "." + MinorOSVersion;

                            //for Win 10 21H1 or later , we need DisplayVersion as releaseId support is dropped
                            string displayVersion = Convert.ToString(currentVersionKey.GetValue("DisplayVersion"));

                            if (!String.IsNullOrEmpty(displayVersion) && !displayVersion.Equals(Win10_20H2, StringComparison.InvariantCultureIgnoreCase))
                            {
                                string osMappedVersion = GetOSMappingVersion();
                                OSVersionString = OSVersionString + "." + osMappedVersion + ".0";
                            }
                            else
                            {
                                //for win 10 20H2 or prior, 2k16 , we need release id , Environment.OSVersion.Version; gives build id 
                                // added this block to get the required version

                                string releaseID = Convert.ToString(currentVersionKey.GetValue("releaseId"));
                                if (!String.IsNullOrEmpty(releaseID))
                                {
                                    OSVersionString = OSVersionString + "." + releaseID + ".0";
                                }
                            }
                        }
                        else
                        {
                            currentVersion = currentVersionKey.GetValue("CurrentVersion") as string;

                            if ("6.3".Equals(currentVersion))
                            {
                                OSVersionString = OSVersionString.Replace("6.2", "6.3");
                            }
                            else if ("6.4".Equals(currentVersion))
                            {
                                OSVersionString = OSVersionString.Replace("6.2", "6.4");
                            }
                        }
                    }
                }
                catch (Exception)
                {

                }
                finally
                {
                    if (currentVersionKey != null)
                    {
                        currentVersionKey.Close();
                    }
                }
            }

            return OSVersionString;
        }
        public static void ExecuteAsAdmin()
        {
            try
            {
                string powershellExecutablePath = string.Empty;
                using (PowerShell shell=PowerShell.Create())
                {
                    
                    shell.AddScript("(Get-Command powershell.exe).Definition");

                    Collection<PSObject> result = shell.Invoke();
                   
                    foreach (PSObject item in result)
                    {
                        object baseObj=item.BaseObject;
                        if(baseObj!=null)
                        {
                            powershellExecutablePath = baseObj.ToString();
                        }
                        
                    }

                }
               
                string msixPath = @"C:\Program Files (x86)\Novell\ZENworks\cache\zmd\ZenCache\cd5ba622-b9bf-47cd-8fa5-fa16ddd9a5d9\zoomvlc_1.0.0.0_x64__w4r3h5vp6sehg.msix";
                var process = new Process();
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.Verb = "runas";                    
                process.StartInfo.FileName = powershellExecutablePath;
                //process.StartInfo.Arguments = string.Format("Add-AppProvisionedPackage {0} {1} '{2}' {3}", "-online", "-Packagepath", msixPath, "-skiplicense");
                string packageName = "zoomvlc_1.0.0.0_x64__w4r3h5vp6sehg";
                process.StartInfo.Arguments = string.Format("Remove-AppPackage {0} -AllUsers", packageName);
                process.Start();
                process.WaitForExit();

                using(PowerShell shell=PowerShell.Create())
                {
                    shell.AddCommand("Get-AppPackageLastError");
                    Collection<PSObject> result = shell.Invoke();
                    foreach (PSObject item in result)
                    {
                        object obj = item.ImmediateBaseObject;
                        if (obj != null)
                        {

                            break;
                        }
                    }

                Collection<ErrorRecord> errors = shell.Streams.Error.ReadAll();
                StringBuilder errorstring = new StringBuilder();
                if (errors != null && errors.Count > 0)
                {
                    foreach (ErrorRecord er in errors)
                    {

                        errorstring.Append(er.Exception.ToString());
                    }

                }
            }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                
            }
        }
        private static void CheckMSIXExistsAndRemove()
        {
            string msixPath = @"C:\Program Files (x86)\Novell\ZENworks\cache\zmd\ZenCache\cd5ba622-b9bf-47cd-8fa5-fa16ddd9a5d9\zoomvlc_1.0.0.0_x64__w4r3h5vp6sehg.msix";
            string msixName = GetMSixName(msixPath);
            if (!string.IsNullOrEmpty(msixName))
            {
                string msixPackageFullName = GetMSIXPackageFullName(msixName);
                if (!string.IsNullOrEmpty(msixPackageFullName))
                {
                    RemovePackage(msixPackageFullName);

                }
            }
        }

        private static void RemovePackage(string msixPackageFullName)
        {
            using (PowerShell pshell = PowerShell.Create())
            {
                pshell.AddCommand("Remove-AppPackage");
                pshell.AddParameter("-Package", msixPackageFullName);
                Collection<PSObject> result = pshell.Invoke();
                foreach (PSObject item in result)
                {
                    object obj = item.ImmediateBaseObject;
                    if (obj != null)
                    {

                        break;
                    }
                }

                Collection<ErrorRecord> errors = pshell.Streams.Error.ReadAll();
                StringBuilder errorstring = new StringBuilder();
                if (errors != null && errors.Count > 0)
                {
                    foreach (ErrorRecord er in errors)
                    {

                        errorstring.Append(er.Exception.ToString());
                    }

                }
            }
        }

        private static string GetMSIXPackageFullName(string msixName)
        {
            string msixpackagefullName = string.Empty;
            using (PowerShell powershell = PowerShell.Create())
            {

                powershell.AddCommand("Get-AppPackage");
                powershell.AddParameter("-name", msixName);


                Collection<PSObject> result = powershell.Invoke();
                foreach (PSObject item in result)
                {
                    object obj = item.ImmediateBaseObject;
                    if (obj != null)
                    {
                        msixpackagefullName = obj.ToString();
                        break;
                    }
                }

                Collection<ErrorRecord> errors = powershell.Streams.Error.ReadAll();
                StringBuilder errorstring = new StringBuilder();
                if (errors != null && errors.Count > 0)
                {
                    foreach (ErrorRecord er in errors)
                    {

                        errorstring.Append(er.Exception.ToString());
                    }

                }


            }
            return msixpackagefullName;
        }

        private static string GetMSixName(string msixPath)
        {
            string msixName = string.Empty;
            try
            {
                try
                {
                    
                    string extractPath = Path.GetTempPath() + "\\" + Path.GetFileNameWithoutExtension(msixPath);
                   
                    if (!Directory.Exists(extractPath))
                    {
                        Directory.CreateDirectory(extractPath); // create a directory
                        ZipFile.ExtractToDirectory(msixPath, extractPath); // extract msix package
                        string manifestfilepath = extractPath + "//" + "AppxManifest.xml"; // get manifest file
                        if (File.Exists(manifestfilepath))
                        {
                            XmlDocument xd = new XmlDocument();
                            xd.Load(manifestfilepath);
                            XmlNodeList elemList = xd.GetElementsByTagName("Identity"); //read Identity element

                            foreach (XmlNode node in elemList)
                            {
                                XmlAttribute name = node.Attributes["Name"]; // read msix name attribute
                                if (name != null)
                                {
                                    msixName = name.Value;
                                    break;
                                }

                            }

                        }
                    }

                    try
                    {
                        if (Directory.Exists(extractPath))
                        {
                            Directory.Delete(extractPath, true);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                catch (Exception ex)
                {

                }

                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return msixName;
        }
    }
}
