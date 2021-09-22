using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CodeLibrary
{
    class TestCert
    {
        public void Test()
        {
            //CertificateNames();
            string pfxFilePath = @"C:\work\data\userstory\msix\cert\mycert.pfx";
            string certFilePath = @"C:\work\data\userstory\msix\cert\mycert.cer";
            CreateCertificate(certFilePath,pfxFilePath);
            X509Store store = new X509Store("teststore", StoreLocation.LocalMachine);
            X509Certificate2 certificate = new X509Certificate2(certFilePath);
            AddCertToStore(certFilePath,ref store,ref certificate);
            RemoveCertificateFromStore(ref store, ref certificate);
            
        }

        private static void CertificateNames()
        {
            Console.WriteLine("\r\nExists Certs Name and Location");
            Console.WriteLine("------ ----- -------------------------");

            foreach (StoreLocation storeLocation in (StoreLocation[])
                Enum.GetValues(typeof(StoreLocation)))
            {
                foreach (StoreName storeName in (StoreName[])
                    Enum.GetValues(typeof(StoreName)))
                {
                    X509Store store = new X509Store(storeName, storeLocation);

                    try
                    {
                        store.Open(OpenFlags.OpenExistingOnly);

                        Console.WriteLine("Yes   {0,4}  {1}, {2}", store.Certificates.Count, store.Name, store.Location);
                    }
                    catch (CryptographicException)
                    {
                        Console.WriteLine("No {0}, {1}", store.Name, store.Location);
                    }
                }
                Console.WriteLine();
            }
        }

        static void CreateCertificate(string certfilepath, string pfxfilepath)
        {
            var ecdsa = ECDsa.Create(); // generate asymmetric key pair
            CertificateRequest certificateRequest = new CertificateRequest("cn=testcert99", ecdsa, HashAlgorithmName.SHA256);
            X509Certificate2 cert = certificateRequest.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(5));
            
            // Create PFX (PKCS #12) with private key
            File.WriteAllBytes(pfxfilepath, cert.Export(X509ContentType.Pfx, "passw0rd"));

            // Create Base 64 encoded CER (public key only)
            File.WriteAllText(certfilepath,
                "-----BEGIN CERTIFICATE-----\r\n"
                + Convert.ToBase64String(cert.Export(X509ContentType.Cert), Base64FormattingOptions.InsertLineBreaks)
                + "\r\n-----END CERTIFICATE-----");
        }

        private static void AddCertToStore(string certFilePath,ref X509Store store,ref X509Certificate2 certificate)
        {
            
            store.Open(OpenFlags.ReadWrite);
                                  

            //Add certificates to the store.
            store.Add(certificate);

            X509Certificate2Collection storecollection = (X509Certificate2Collection)store.Certificates;
            Console.WriteLine("Store name: {0}", store.Name);
            Console.WriteLine("Store location: {0}", store.Location);
            foreach (X509Certificate2 x509 in storecollection)
            {
                Console.WriteLine("certificate name: {0}", x509.Subject);
            }
                   
            //Close the store.
            store.Close();
            
        }

        private static void RemoveCertificateFromStore(ref X509Store store, ref X509Certificate2 certificate)
        {
            store.Open(OpenFlags.ReadWrite);
            store.Remove(certificate);
            X509Certificate2Collection storecollection2 = (X509Certificate2Collection)store.Certificates;
            Console.WriteLine("{1}Store name: {0}", store.Name, Environment.NewLine);
            foreach (X509Certificate2 x509 in storecollection2)
            {
                Console.WriteLine("certificate name: {0}", x509.Subject);
            }
            store.Close();
        }
    }
    
    
}
