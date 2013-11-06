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
            switch (options.Command.ToUpperInvariant())
            {
                case "SHOW":
                    return new ShowCommand().Run(options.Parameters);
                default:
                    Options.Usage();
                    break;
            }
            return 99;
        }
    }
}
