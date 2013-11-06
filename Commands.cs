using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace CertDetails
{
    abstract class Command
    {
        public abstract int Run(IEnumerable<string> parameters);
    }

    class ShowCommand : Command
    {
        public override int Run(IEnumerable<string> parameters)
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
                return ShowDetails(details);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }
        }

        int ShowDetails(CertificateDetails details)
        {
            foreach (var detail in details.Pairs)
            {
                Console.WriteLine("{0, -20}: {1}", detail.Field, detail.Value);
            }
            return 0;
        }
    }
}
