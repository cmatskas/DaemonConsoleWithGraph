using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Threading.Tasks;

namespace DaemonConsole
{
    class Program
    {
        private const string AppSettingsFile = "appsettings.json";

        static async Task Main(string[] args)
        {
            var config = LoadAppSettings();
            var authProvider = new ClientCredentialsAuthProvider(config);

            //authenticate
            await GetAccessToken(authProvider);

            //grab the Graph data
            await GetDirectoryUsers(authProvider);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static AuthenticationConfig LoadAppSettings()
        {
            var config = new ConfigurationBuilder()
                                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                                .AddJsonFile(AppSettingsFile)
                                .Build();

            return config.Get<AuthenticationConfig>();
        }

        private static async Task GetDirectoryUsers(IAuthenticationProvider authProvider)
        {
            Console.WriteLine("Acquiring directory users...");

            try
            {
                GraphHelper.Initialize(authProvider);
                var users = await GraphHelper.GetUsersAsync();
                if(users == null)
                {
                    return;
                }

                Console.ForegroundColor = ConsoleColor.DarkBlue;
                foreach (var user in users)
                {
                    Console.WriteLine($"Found user {user.DisplayName} with email address: {user.Mail}");
                }
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Unable to retrieve directory data...");
                Console.WriteLine(ex.Message);
            }
            
            Console.ResetColor();
        }

        private static async Task GetAccessToken(ClientCredentialsAuthProvider authProvider)
        {
            try
            {
                var result = await authProvider.GetAccessTokenAsync();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Auth token acquired :)");
                // Console.WriteLine(result);
                Console.WriteLine(string.Empty);
            }
            catch (MsalServiceException ex) when (ex.Message.Contains("AADSTS70011"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
            }

            Console.ResetColor();
        }
    }
}
