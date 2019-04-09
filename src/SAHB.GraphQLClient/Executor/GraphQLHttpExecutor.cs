using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SAHB.GraphQLClient.Exceptions;
using SAHB.GraphQLClient.Result;

namespace SAHB.GraphQLClient.Executor
{
    // ReSharper disable once InconsistentNaming
    /// <inheritdoc />
    public class GraphQLHttpExecutor : IGraphQLHttpExecutor
    {
        public HttpClient Client { get; }

        public HttpMethod DefaultMethod { get; set; }
        
        /// <summary>
        /// Initializes a new instance of a GraphQL executor which executes a query against a http GraphQL server
        /// </summary>
        public GraphQLHttpExecutor()
        {
            // Add httpClient
            Client = new HttpClient();
            Client.DefaultRequestHeaders.Connection.Add("keep-alive");
            DefaultMethod = HttpMethod.Post;
        }

        /// <inheritdoc />
        public async Task<string> ExecuteQuery(string query, string url = null, HttpMethod method = null, string authorizationToken = null, string authorizationMethod = "Bearer", IDictionary<string, string> headers = null)
        {
            // Check parameters for null
            if (query == null) throw new ArgumentNullException(nameof(query));

            // Find method (set to default if null)
            method = method ?? DefaultMethod;
            if (method == null) throw new ArgumentNullException(nameof(method));

            // Logging
            if (Logger != null && Logger.IsEnabled(LogLevel.Information))
            {
                Logger.LogInformation($"Sending query {query} to GraphQL server on {url} with method {method}");
            }

            // Initializes request message
            var requestMessage = new HttpRequestMessage(method, url)
            {
                Content = new StringContent(query, Encoding.UTF8, "application/json")
            };

            // Add headers
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    requestMessage.Headers.Add(header.Key, header.Value);
                }
            }

            // Add authorization info
            if (authorizationToken != null)
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
            }

            // Send request
            HttpResponseMessage response = await Client.SendAsync(requestMessage).ConfigureAwait(false);

            // Detect if response was not successfully
            if (!response.IsSuccessStatusCode)
            {
                string errorResponse = null;

                // Try to read response
                try
                {
                    errorResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    throw new GraphQLHttpExecutorServerErrorStatusCodeException(response.StatusCode, query, errorResponse, "Response from server was not successfully", ex);
                }

                throw new GraphQLHttpExecutorServerErrorStatusCodeException(response.StatusCode, query, errorResponse, "Response from server was not successfully");
            }

            // Get response
            string stringResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            
            // Logging
            if (Logger != null && Logger.IsEnabled(LogLevel.Information))
            {
                Logger.LogInformation($"Response: {stringResponse}");
            }

            return stringResponse;
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
