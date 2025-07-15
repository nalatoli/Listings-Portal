using Listings_Portal.Lib.Tools.Cloud;

namespace Listings_Portal.Lib.Tools.Cloud.RentCast
{
    public class RentCastClient : BaseClient
    {
        private const string keyFilename = "rckey.txt";

        public RentCastClient() : base("https://api.rentcast.io/v1/")
        {
            client.DefaultRequestHeaders.Add("X-Api-Key", GetApiKey(keyFilename));
        }

        public static void CreateApiKeyFile(string apiKey)
        {
            CreateApiKeyFile(apiKey, keyFilename);
        }
    }
}
