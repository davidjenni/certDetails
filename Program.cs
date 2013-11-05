using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CertDetails
{
    class Program
    {
        static int Main(string[] args)
        {
            var app = new Program();

            return app.Run(args);
        }

        int Run(string[] args)
        {
            X509Certificate2 certificate = null;
            if (args.Length == 2)
            {
                certificate = OpenCertificateFile(args[0], args[1]);
            }
            else if (args.Length == 1)
            {
                try
                {
                    certificate = OpenCertificateFile(args[0]);
                }
                catch (CryptographicException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("file seems to be a .pfx file with a private key, please also specify the password.");
                    Environment.Exit(1);
                }
            }
            else
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("CertDetails path_to_cert_file [password]");
                Console.WriteLine("  path_to_cert_file: .cer or .pfx file");
                Console.WriteLine("  password:          password if .pfx file with private key");
                Environment.Exit(10);
            }
            if (certificate == null)
            {
                return 1;
            }
            return ShowDetails(certificate);
        }

        int ShowDetails(X509Certificate2 certificate)
        {
            Console.WriteLine("Thumbprint:          {0}, SHA1", certificate.Thumbprint);
            Console.WriteLine("Serial:              {0}", certificate.SerialNumber);
            var publicKey = certificate.PublicKey;
            Console.WriteLine("Key:                 {0} {1} bits", publicKey.Oid.FriendlyName, publicKey.Key.KeySize);
            Console.WriteLine("Private:             {0}", certificate.HasPrivateKey);
            Console.WriteLine("Subject:             {0}", certificate.Subject);
            Console.WriteLine("Issuer:              {0}", certificate.Issuer);
            Console.WriteLine("Signature algorithm: {0}", certificate.SignatureAlgorithm.FriendlyName);
            Console.WriteLine("Not before:          {0} UTC", certificate.NotBefore.ToUniversalTime().ToString());
            Console.WriteLine("Not after:           {0} UTC", certificate.NotAfter.ToUniversalTime().ToString());
            // TODO: enumerate Extensions
            return 0;
        }

        static X509Certificate2 OpenCertificateFile(string file)
        {
            return OpenCertificateFile(file, null);
        }

        static X509Certificate2 OpenCertificateFile(string file, string password)
        {
            if (!File.Exists(file))
            {
                Console.WriteLine("Cannot find certificate file: '{0}'", file);
                return null;
            }
            byte[] certData;
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                certData = new byte[stream.Length];
                stream.Read(certData, 0, certData.Length);
            }
            Console.WriteLine("Opening {1} certificate file '{0}'", file, X509Certificate2.GetCertContentType(certData));
            if (password == null)
            {
                return new X509Certificate2(certData);
            }
            else
            {
                return new X509Certificate2(certData, password);
            }
        }
    }
}
