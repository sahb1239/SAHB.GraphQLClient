using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SAHB.GraphQLClient.Result;

namespace SAHB.GraphQLClient.Executor
{
    // ReSharper disable once InconsistentNaming
    /// <inheritdoc />
    public class GraphQLHttpExecutor : IGraphQLHttpExecutor
    {
        private readonly HttpClient _client;

        /// <summary>
        /// Initilizes a new instance of a GraphQL executor which executes a query against a http GraphQL server
        /// </summary>
        public GraphQLHttpExecutor()
        {
            // Add httpClient
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Connection.Add("keep-alive");
        }

        /// <inheritdoc />
        public async Task<GraphQLDataResult<T>> ExecuteQuery<T>(string query, string url, HttpMethod method, string authorizationToken = null, string authorizationMethod = "Bearer") where T : class
        {
            // Check parameters for null
            if (query == null) throw new ArgumentNullException(nameof(query));
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (method == null) throw new ArgumentNullException(nameof(method));

            // Logging
            if (Logger != null && Logger.IsEnabled(LogLevel.Information))
            {
                Logger.LogInformation($"Sending query {query} to GraphQL server on {url} with method {method}");
            }

            // Initilizes request message
            var requestMessage = new HttpRequestMessage(method, url)
            {
                Content = new StringContent(query, Encoding.UTF8, "application/json")
            };

            // Add authorization info
            if (authorizationToken != null)
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
            }

            // Send request
            HttpResponseMessage response = await _client.SendAsync(requestMessage);

            // Ensure success response
            response.EnsureSuccessStatusCode();

            // Get response
            string stringResponse = await response.Content.ReadAsStringAsync();
            
            // Logging
            if (Logger != null && Logger.IsEnabled(LogLevel.Information))
            {
                Logger.LogInformation($"Response: {stringResponse}");
            }

            // Deserilize response
            var result = JsonConvert.DeserializeObject<GraphQLDataResult<T>>(stringResponse);

            // Logging errors
            if (result.ContainsErrors && Logger != null && Logger.IsEnabled(LogLevel.Error))
            {
                Logger.LogError($"GraphQL error from query {query} and url {url}", result.Errors);
            }

            return result;
        }
        
        #region Logging

        private ILoggerFactory _loggerFactory;

        /// <summary>
        /// Contains a logger factory for the GraphQLHttpClient
        /// </summary>
        public ILoggerFactory LoggerFactory
        {
            internal get { return _loggerFactory; }
            set
            {
                _loggerFactory = value;
                if (_loggerFactory != null)
                {
                    Logger = _loggerFactory.CreateLogger<GraphQLHttpExecutor>();
                }
            }
        }

        /// <summary>
        /// Contains the logger for the class
        /// </summary>
        private ILogger<GraphQLHttpExecutor> Logger { get; set; }

        #endregion
    }
}
