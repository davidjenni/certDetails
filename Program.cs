using System;
using System.Collections.Generic;
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
                Options.Usage(errorCode);
            }
            return 0;
        }

        int Run(string[] args)
        {
            var options = Options.Parse(args);
            using (var resultWriter = CreateWriter(options))
            {
                switch (options.Command.ToUpperInvariant())
                {
                    case "SHOW":
                        return new ShowCommand().Run(options.Parameters, resultWriter);
                    case "RECURSIVE":
                        return new RecursiveCommand().Run(options.Parameters, resultWriter);
                    default:
                        Options.Usage();
                        break;
                }
            }
            return 99;
        }

        ResultWriter CreateWriter(Options options)
        {
            if (options.EmitToStdout)
            {
                return new StdoutResultWriter();
            }
            else if (!string.IsNullOrEmpty(options.CsvFile))
            {
                return new CsvResultWriter(options.CsvFile);
            }
            else
            {
                throw new NotImplementedException("Unknown result write");
            }
        }
    }
}
