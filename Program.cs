using System;
using System.Linq;

namespace CertDetails
{
    class Program
    {
        static int Main(string[] args)
        {
            var app = new Program();

            int errorCode = app.Run(args);
            if (errorCode != 0)
            {
                app.Usage();
            }
            return errorCode;
        }

        int Run(string[] args)
        {
            if (args.Length < 1)
            {
                Usage();
                Environment.Exit(10);
            }
            var parameters = args.Skip(1);
            switch (args[0].ToUpperInvariant())
            {
                case "SHOW":
                    return new ShowCommand().Run(parameters);
                default:
                    Usage();
                    Environment.Exit(10);
                    break;
            }
            return 99;
        }

        void Usage()
        {
            Console.WriteLine(@"
Usage:
CertDetails command_verb [parameters]
  Command verbs: show, 

show path_to_cert_file [password]
  path_to_cert_file: .cer or .pfx file
  password:          password if .pfx file with private key
            ");
        }

    }
}
