using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace DaemonConsole
{
    public class ClientCredentialsAuthProvider : IAuthenticationProvider
    {
        private readonly IConfidentialClientApplication msalClient;
        private readonly string[] scopes;

        public ClientCredentialsAuthProvider(AuthenticationConfig config)
        {
            scopes = new string[] { config.Scopes };

            msalClient = ConfidentialClientApplicationBuilder
                .Create(config.ClientId)
                .WithCertificate(ReadCertificateLocal(config.CertificateName))
                .WithAuthority(AadAuthorityAudience.AzureAdMyOrg, true)
                .WithTenantId(config.Tenant)
                .Build();
        }

        public async Task<string> GetAccessTokenAsync()
        {
            try
            {
                var result = await msalClient.AcquireTokenForClient(scopes)
                    .ExecuteAsync();

                return result.AccessToken;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error getting access token: {exception.Message}");
                return null;
            }

        }

        // This is the required function to implement IAuthenticationProvider
        // The Graph SDK will call this function each time it makes a Graph
        // call.
        public async Task AuthenticateRequestAsync(HttpRequestMessage requestMessage)
        {
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("bearer", await GetAccessTokenAsync());
        }

        private X509Certificate2 ReadCertificateLocal(string certificateName)
        {
            X509Certificate2 cert = null;

            using (X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certCollection = store.Certificates;
                X509Certificate2Collection currentCerts = certCollection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
                X509Certificate2Collection signingCert = currentCerts.Find(X509FindType.FindBySubjectDistinguishedName, certificateName, false);
                cert = signingCert.OfType<X509Certificate2>().OrderByDescending(c => c.NotBefore).FirstOrDefault();
            }
            return cert;
        }
    }
}
