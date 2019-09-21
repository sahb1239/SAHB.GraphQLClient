using GraphQL.Types;

namespace SAHB.GraphQL.Client.Testserver.Schemas
{
    public class GenericQuerySchema<T> : Schema
        where T : ObjectGraphType, new()
    {
        public GenericQuerySchema()
        {
            Query = new T();
        }
    }
}
