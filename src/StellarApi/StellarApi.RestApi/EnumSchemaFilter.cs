using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace StellarApi.RestApi
{
    /// <summary>
    /// Filter to add enum names to the schema.
    /// </summary>
    public class EnumSchemaFilter : ISchemaFilter
    {
        /// <summary>
        /// Apply the schema filter.
        /// </summary>
        /// <param name="schema">The schema to apply the filter to.</param>
        /// <param name="context">The context of the schema filter.</param>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                var array = new OpenApiArray();
                array.AddRange(Enum.GetNames(context.Type).Select(n => new OpenApiString(n)));
                // NSwag
                schema.Extensions.Add("x-enumNames", array);
                // Openapi-generator
                schema.Extensions.Add("x-enum-varnames", array);
            }
        }
    }
}
