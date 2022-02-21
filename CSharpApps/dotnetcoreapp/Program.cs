using System;
using System.Security.Cryptography.X509Certificates;

namespace dotnetcoreapp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            AddCertificateToTrustStore();

        }

        private static void AddCertificateToTrustStore()
        {
            string certPemFilePath = @"C:\work\learn\projects\node-apps\test-server\certificates\cert.pem";
            string keyPemFilePath = @"C:\work\learn\projects\node-apps\test-server\certificates\key.pem";
            X509Certificate2 x509Certificate2 = X509Certificate2.CreateFromPemFile(certPemFilePath, keyPemFilePath);
            try
            {
                X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadWrite);
                store.Add(x509Certificate2);

                store.Close();
            }
            catch
            {

            }
        }

    }
}
