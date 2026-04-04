using Listings_Portal.Lib.Tools.Managers;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;

namespace Listings_Portal.Lib.Tools.Cloud
{
    public abstract class BaseClient(HttpClient client, ILogger logger)
    {
        protected readonly HttpClient client = client;
        protected readonly ILogger logger = logger;

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


        protected async Task<HttpResponseMessage> SendHttpGetResponseAsync(string endpoint, string? query, HttpMethod method, CancellationToken cancellationToken = default)
        {
            //string resource = $"{endpoint}{(string.IsNullOrEmpty(query) ? "" : $"?{query}")}";
            //return client.SendAsync(new HttpRequestMessage(method, resource), cancellationToken);

            var resource = $"{endpoint}{(string.IsNullOrEmpty(query) ? "" : $"?{query}")}";
            using var request = new HttpRequestMessage(method, resource)
            {
                Version = HttpVersion.Version11,
                VersionPolicy = HttpVersionPolicy.RequestVersionExact
            };

            var sw = Stopwatch.StartNew();

            try
            {
                logger.LogInformation("HTTP start {Method} {Resource}", method, resource);

                var response = await client.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken);

                logger.LogInformation(
                    "HTTP headers received in {ElapsedMs} ms. Status={StatusCode}",
                    sw.ElapsedMilliseconds,
                    (int)response.StatusCode);

                return response;
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                logger.LogError(
                    ex,
                    "HTTP timeout after {ElapsedMs} ms for {Method} {Resource}",
                    sw.ElapsedMilliseconds,
                    method,
                    resource);

                throw;
            }
        }

        /// <summary>
        /// Creates new encrypted API key file.
        /// This should NOT be used in production.
        /// </summary>
        /// <param name="apiKey"> API key in plaintext.</param>
        /// <param name="filename"> Filename for key file. </param>
        protected static void CreateApiKeyFile(string apiKey, string filename)
        {
            File.WriteAllBytes(Path.Combine(Paths.BinDir, filename), SecurityManager.Encrypt(apiKey));
        }

        /// <summary>
        /// Gets API key from encrypted file.
        /// </summary>
        /// <param name="filename"> Filename for key file. </param>
        /// <return> API key. </return>
        protected static string GetApiKey(string filename)
        {
            return SecurityManager.Decrypt(File.ReadAllBytes(Path.Combine(Paths.BinDir, filename)));
        }
    }
}
