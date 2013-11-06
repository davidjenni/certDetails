using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertDetails
{
    abstract class ResultWriter : IDisposable
    {
        public abstract void WriteHeader(CertificateDetails details);
        public abstract void Write(CertificateDetails details);
        protected abstract void Close();

        public void Dispose()
        {
            Close();
        }
    }

    class StdoutResultWriter : ResultWriter
    {
        public override void WriteHeader(CertificateDetails details)
        {
        }

        public override void Write(CertificateDetails details)
        {
            foreach (var detail in details.Pairs)
            {
                Console.WriteLine("{0, -20}: {1}", detail.Field, detail.Value);
            }
        }

        protected override void Close()
        {
        }
    }

    class CsvResultWriter : ResultWriter
    {
        const char separator = ',';
        TextWriter writer;
        FileStream file;

        public CsvResultWriter(string csvFile)
        {
            if (csvFile == "-")
            {
                this.writer = Console.Out;
            }
            else
            {
                this.file = new FileStream(csvFile, FileMode.Create, FileAccess.Write, FileShare.Read);
                this.writer = new StreamWriter(this.file);
            }
        }

        public override void WriteHeader(CertificateDetails details)
        {
            var enumerator = details.FieldNames.GetEnumerator();
            bool hasMore = enumerator.MoveNext();
            while (hasMore)
            {
                this.writer.Write(Wrapped(enumerator.Current));
                hasMore = enumerator.MoveNext();
                if (hasMore)
                {
                    this.writer.Write(separator);
                }
            }
            this.writer.WriteLine();
        }

        public override void Write(CertificateDetails details)
        {
            var enumerator = details.Pairs.GetEnumerator();
            bool hasMore = enumerator.MoveNext();
            while (hasMore)
            {
                this.writer.Write(Wrapped(enumerator.Current.Value.ToString()));
                hasMore = enumerator.MoveNext();
                if (hasMore)
                {
                    this.writer.Write(separator);
                }
            }
            this.writer.WriteLine();
        }

        protected override void Close()
        {
            if (this.file != null)
            {
                this.writer.Close();
                this.file.Close();
                this.file = null;
            }
        }

        string Wrapped(string plain)
        {
            if (plain.Contains(separator))
            {
                return "\"" + plain + "\"";
            }
            else
            {
                return plain;
            }
        }
    }
}
