using Microsoft.Graph;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DaemonConsole
{
    public class GraphHelper
    {
        private static GraphServiceClient graphClient;
        public static void Initialize(IAuthenticationProvider authProvider)
        {
            graphClient = new GraphServiceClient(authProvider);
        }

        public static async Task<IEnumerable<User>> GetUsersAsync()
        {
            // GET /users
            return await graphClient.Users.Request().GetAsync();
        }
    }
}
