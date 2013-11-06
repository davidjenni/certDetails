using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace CertDetails
{
    abstract class Command
    {
        public abstract int Run(IEnumerable<string> parameters, ResultWriter resultWriter);
    }

    class ShowCommand : Command
    {
        public override int Run(IEnumerable<string> parameters, ResultWriter resultWriter)
        {
            try
            {
                if (!parameters.Any())
                {
                    Console.WriteLine("Must at least specify a certificate file.");
                    return 10;
                }
                var fileAndPassword = parameters.ToArray();
                string file = fileAndPassword[0];
                string password = null;
                if (fileAndPassword.Length > 1)
                {
                    password = fileAndPassword[1];
                }
                CertificateDetails details = null;
                if (password != null)
                {
                    details = CertificateDetails.OpenFromFile(file, password);
                }
                else
                {
                    try
                    {
                        details = CertificateDetails.OpenFromFile(file);
                    }
                    catch (CryptographicException ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("file seems to be a .pfx file with a private key, please also specify the password.");
                        return 1;
                    }
                }
                resultWriter.WriteHeader(details);
                resultWriter.Write(details);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }
            return 0;
        }
    }

    class RecursiveCommand : Command
    {
        public override int Run(IEnumerable<string> parameters, ResultWriter resultWriter)
        {
            string startDirectory = Environment.CurrentDirectory;
            string pattern = "*.cer";
            bool isFirst = true;

            foreach (var file in Directory.EnumerateFiles(startDirectory, pattern, SearchOption.AllDirectories))
            {
                var details = CertificateDetails.OpenFromFile(file);
                if (isFirst)
                {
                    isFirst = false;
                    resultWriter.WriteHeader(details);
                }
                resultWriter.Write(details);
            }
            return 0;
        }
    }
}
