using Listings_Portal.Lib.Tools.Managers;
using System.Net.Http.Json;

namespace Listings_Portal.Lib.Tools.Cloud
{
    public abstract class BaseClient(string baseAddress) : IDisposable
    {
        protected readonly HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri(baseAddress)
        };

        #region IDisposable

        private bool isDisposed;
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                    client.Dispose();

                isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        public async Task<T> Get<T>(string endpoint, string? query, CancellationToken cancellationToken = default)
        {
            var content = await SendHttpGetContentAsync(endpoint, query, HttpMethod.Get, cancellationToken);
            return await content.ReadFromJsonAsync<T>(cancellationToken)
                ?? throw new NullReferenceException($"Unable to serialize '{content}'");
        }

        protected async Task<HttpContent> SendHttpGetContentAsync(string endpoint, string? query, HttpMethod method, CancellationToken cancellationToken = default)
        {
            var rsp = await SendHttpGetResponseAsync(endpoint, query, method, cancellationToken);
            if (!rsp.IsSuccessStatusCode)
                throw new HttpRequestException($"({rsp.StatusCode}): {rsp.ReasonPhrase}");
            return rsp.Content;
        }


        protected Task<HttpResponseMessage> SendHttpGetResponseAsync(string endpoint, string? query, HttpMethod method, CancellationToken cancellationToken = default)
        {
            string resource = $"{endpoint}{(string.IsNullOrEmpty(query) ? "" : $"?{query}")}";
            return client.SendAsync(new HttpRequestMessage(method, resource), cancellationToken);
        }

        /// <summary>
        /// Creates new encrypted API key file.
        /// This should NOT be used in production.
        /// </summary>
        /// <param name="apiKey"> API key in plaintext.</param>
        /// <param name="filename"> Filename for key file. </param>
        public static void CreateApiKeyFile(string apiKey, string filename)
        {
            File.WriteAllBytes(Path.Combine(Paths.BinDir, filename), SecurityManager.Encrypt(apiKey));
        }

        /// <summary>
        /// Gets API key from encrypted file.
        /// </summary>
        /// <param name="filename"> Filename for key file. </param>
        /// <return> API key. </return>
        protected string GetApiKey(string filename)
        {
            return SecurityManager.Decrypt(File.ReadAllBytes(Path.Combine(Paths.BinDir, filename)));
        }
    }
}
