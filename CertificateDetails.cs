using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace CertDetails
{
    struct Detail
    {
        readonly string field;
        readonly object value;

        public Detail(string field, object value)
        {
            this.field = field;
            this.value = value;
        }
        public string Field { get { return this.field; } }
        public object Value { get { return this.value; } }
    }

    class CertificateDetails
    {
        readonly X509Certificate2 certificate;
        List<Detail> details;

        public IEnumerable<string> FieldNames
        {
            get
            {
                foreach (var detail in this.details)
                {
                    yield return detail.Field;
                }
            }
        }

        public IEnumerable<Detail> Pairs
        {
            get
            {
                return this.details;
            }
        }

        public CertificateDetails(X509Certificate2 certificate)
            : this(certificate, null)
        {
        }

        public CertificateDetails(X509Certificate2 certificate, string file)
        {
            this.certificate = certificate;
            PopulateDetails(file);
        }

        public static CertificateDetails OpenFromFile(string file)
        {
            return OpenFromFile(file, null);
        }

        public static CertificateDetails OpenFromFile(string file, string password)
        {
            if (!File.Exists(file))
            {
                throw new FileNotFoundException(string.Format("Cannot find certificate file: '{0}'", file));
            }
            byte[] certData;
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                certData = new byte[stream.Length];
                stream.Read(certData, 0, certData.Length);
            }
            X509Certificate2 certificate;
            if (password == null)
            {
                certificate = new X509Certificate2(certData);
            }
            else
            {
                certificate = new X509Certificate2(certData, password);
            }
            return new CertificateDetails(certificate, file);
        }

        void PopulateDetails(string origin)
        {
            if (this.details == null)
            {
                var list = new List<Detail>();
                if (origin != null)
                {
                    list.Add(new Detail("Origin", origin));
                }
                list.Add(new Detail("Type", X509Certificate2.GetCertContentType(certificate.RawData)));
                list.Add(new Detail("Thumbprint", certificate.Thumbprint));
                list.Add(new Detail("Serial", certificate.SerialNumber));
                var publicKey = certificate.PublicKey;
                list.Add(new Detail("KeyAlgorithm", publicKey.Oid.FriendlyName));
                list.Add(new Detail("KeyLength", publicKey.Key.KeySize));
                list.Add(new Detail("Private", certificate.HasPrivateKey));
                list.Add(new Detail("Subject", certificate.Subject));
                list.Add(new Detail("Issuer", certificate.Issuer));
                list.Add(new Detail("Signature algorithm", certificate.SignatureAlgorithm.FriendlyName));
                list.Add(new Detail("Not before", certificate.NotBefore.ToUniversalTime()));
                list.Add(new Detail("Not after", certificate.NotAfter.ToUniversalTime()));
                list.Add(new Detail("Extensions count", certificate.Extensions.Count));
                // TODO: enumerate Extensions
                this.details = list;
            }
        }
    }
}
