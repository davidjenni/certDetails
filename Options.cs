using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertDetails
{
    class Options
    {
        List<string> parameters = new List<string>();

        public string Command { get; private set; }
        public IEnumerable<string> Parameters { get { return this.parameters; } }

        public bool EmitToStdout
        {
            get { return string.IsNullOrEmpty(CsvFile); }
        }

        public string CsvFile { get; set; }

        public static Options Parse(string[] args)
        {
            if (args.Length < 1)
            {
                Options.Usage();
            }
            Options options = new Options();
            options.Command = args[0];
            options.parameters.AddRange(args.Skip(1));

            int i = 0;
            while (i < options.parameters.Count)
            {
                var parameter = options.parameters[i].Trim();
                char lead = parameter[0];
                if (lead == '-' || lead == '/')
                {
                    options.parameters.RemoveAt(i);
                    var flagAndValue = parameter.Substring(1).Split(new char[] { ':' }, 2);
                    var flag = flagAndValue[0];
                    switch (flag.ToUpperInvariant())
                    {
                        case "CSV":
                            options.CsvFile = flagAndValue[1];
                            break;
                        default:
                            Console.WriteLine("Unknown flag: '{0}'", flag);
                            Usage();
                            break;
                    }
                }
                else
                {
                    i++;
                }
            }
            return options;
        }

        public static void Usage(int errorCode = 10)
        {
            Console.WriteLine(@"
Usage:
CertDetails command_verb [parameters]
  Command verbs: SHOW, RECURSIVE

SHOW path_to_cert_file [password]
      path_to_cert_file: .cer or .pfx file
      password:          password if .pfx file with private key
  displays details of a given certificate file

RECURSIVE 
  walks recursively directory tree and displays details of all .cer files

global options:
  -csv:file         emit result to CSV file (default: emit to stdout)
            ");
            Environment.Exit(errorCode);
        }
    }
}
