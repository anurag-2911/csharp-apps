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
            AddCertToStore(certFilePath);
            //AddCert();
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
            var req = new CertificateRequest("cn=testcert", ecdsa, HashAlgorithmName.SHA256);
            var cert = req.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(5));

            // Create PFX (PKCS #12) with private key
            File.WriteAllBytes(pfxfilepath, cert.Export(X509ContentType.Pfx, "P@55w0rd"));

            // Create Base 64 encoded CER (public key only)
            File.WriteAllText(certfilepath,
                "-----BEGIN CERTIFICATE-----\r\n"
                + Convert.ToBase64String(cert.Export(X509ContentType.Cert), Base64FormattingOptions.InsertLineBreaks)
                + "\r\n-----END CERTIFICATE-----");
        }

        private static void AddCertToStore(string certFilePath)
        {
            //Create new X509 store called teststore from the local certificate store.
            X509Store store = new X509Store("teststore", StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            X509Certificate2 certificate = new X509Certificate2();

            //Create certificates from certificate files.
            
            X509Certificate2 certificate1 = new X509Certificate2(certFilePath);
            //X509Certificate2 certificate2 = new X509Certificate2(certFilePath);
            //X509Certificate2 certificate3 = new X509Certificate2(certFilePath);

            //Create a collection and add two of the certificates.
            //X509Certificate2Collection collection = new X509Certificate2Collection();
            //collection.Add(certificate2);
            //collection.Add(certificate3);

            //Add certificates to the store.
            store.Add(certificate1);
            //store.AddRange(collection);

            X509Certificate2Collection storecollection = (X509Certificate2Collection)store.Certificates;
            Console.WriteLine("Store name: {0}", store.Name);
            Console.WriteLine("Store location: {0}", store.Location);
            foreach (X509Certificate2 x509 in storecollection)
            {
                Console.WriteLine("certificate name: {0}", x509.Subject);
            }

            //Remove a certificate.
            store.Remove(certificate1);
            X509Certificate2Collection storecollection2 = (X509Certificate2Collection)store.Certificates;
            Console.WriteLine("{1}Store name: {0}", store.Name, Environment.NewLine);
            foreach (X509Certificate2 x509 in storecollection2)
            {
                Console.WriteLine("certificate name: {0}", x509.Subject);
            }

            //Remove a range of certificates.
            //store.RemoveRange(collection);
            X509Certificate2Collection storecollection3 = (X509Certificate2Collection)store.Certificates;
            Console.WriteLine("{1}Store name: {0}", store.Name, Environment.NewLine);
            if (storecollection3.Count == 0)
            {
                Console.WriteLine("Store contains no certificates.");
            }
            else
            {
                foreach (X509Certificate2 x509 in storecollection3)
                {
                    Console.WriteLine("certificate name: {0}", x509.Subject);
                }
            }

            //Close the store.
            store.Close();
        }
    }
    
    
}
