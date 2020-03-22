using System;
using System.Threading;
using System.Threading.Tasks;
using SAHB.GraphQLClient.Builder;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Introspection;

namespace SAHB.GraphQLClient
{
    public static class GraphQLClientExtensions
    {
        #region Requests

        public static IGraphQLHttpRequest<T> Query<T>(this IGraphQLClient client) where T : class
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            return client.CreateHttpRequest<T>(GraphQLOperationType.Query);
        }

        public static IGraphQLHttpRequest<dynamic> Query(this IGraphQLClient client, Action<IGraphQLBuilder> queryBuilder)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (queryBuilder is null)
            {
                throw new ArgumentNullException(nameof(queryBuilder));
            }

            return client.CreateHttpRequest(GraphQLOperationType.Query, queryBuilder);
        }

        public static IGraphQLHttpRequest<T> Mutate<T>(this IGraphQLClient client) where T : class
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            return client.CreateHttpRequest<T>(GraphQLOperationType.Mutation);
        }

        public static IGraphQLHttpRequest<dynamic> Mutate(this IGraphQLClient client, Action<IGraphQLBuilder> queryBuilder)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (queryBuilder is null)
            {
                throw new ArgumentNullException(nameof(queryBuilder));
            }

            return client.CreateHttpRequest(GraphQLOperationType.Mutation, queryBuilder);
        }

        public static IGraphQLSubscriptionRequest<T> Subscribe<T>(this IGraphQLClient client) where T : class
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            return client.CreateSubscriptionRequest<T>(GraphQLOperationType.Subscription);
        }

        public static IGraphQLSubscriptionRequest<dynamic> Subscribe(this IGraphQLClient client, Action<IGraphQLBuilder> queryBuilder)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (queryBuilder is null)
            {
                throw new ArgumentNullException(nameof(queryBuilder));
            }

            return client.CreateSubscriptionRequest(GraphQLOperationType.Subscription, queryBuilder);
        }

        public static IGraphQLBatchRequest QueryBatch(this IGraphQLClient client)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            return client.CreateBatchRequest(GraphQLOperationType.Query);
        }

        public static IGraphQLBatchRequest MutateBatch(this IGraphQLClient client)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            return client.CreateBatchRequest(GraphQLOperationType.Mutation);
        }

        public static IGraphQLBatchRequest SubscribeBatch(this IGraphQLClient client)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            return client.CreateBatchRequest(GraphQLOperationType.Subscription);
        }

        #endregion

        #region Introspection

        public static async Task<GraphQLIntrospectionSchema> GetIntrospectionSchema(this IGraphQLClient client, CancellationToken cancellationToken = default)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            var request = client.Query<GraphQLIntrospectionQuery>().SetShouldThrowIfQueryIsInvalid(false);
            var response = await request.GetData(cancellationToken).ConfigureAwait(false);

            return response.Schema;
        }

        #endregion
    }
}
