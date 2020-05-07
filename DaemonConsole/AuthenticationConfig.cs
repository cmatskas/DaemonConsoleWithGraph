namespace DaemonConsole
{
    public class AuthenticationConfig
    {
        public string Scopes { get; set; } = "https://graph.microsoft.com/.default";

        public string Tenant { get; set; }

        public string ClientId { get; set; }

        public string CertificateName { get; set; }
    }
}

