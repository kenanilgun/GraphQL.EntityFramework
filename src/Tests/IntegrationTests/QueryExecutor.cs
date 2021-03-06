using System.Threading.Tasks;
using GraphQL;
using GraphQL.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

static class QueryExecutor
{
    public static async Task<object> ExecuteQuery(string query, ServiceCollection services, DbContext dbContext, Inputs inputs, GlobalFilters filters)
    {
        query = query.Replace("'", "\"");
        EfGraphQLConventions.RegisterInContainer(services, dbContext.Model, filters);
        using (var provider = services.BuildServiceProvider())
        using (var schema = new Schema(new FuncDependencyResolver(provider.GetRequiredService)))
        {
            var documentExecuter = new EfDocumentExecuter();

            var executionOptions = new ExecutionOptions
            {
                Schema = schema,
                Query = query,
                UserContext = dbContext,
                Inputs = inputs
            };

            var executionResult = await documentExecuter.ExecuteWithErrorCheck(executionOptions);
            return executionResult.Data;
        }
    }
}