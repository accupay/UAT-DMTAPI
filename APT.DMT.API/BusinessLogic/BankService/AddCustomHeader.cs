using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;



public class AddRequiredHeaderParameter : IOperationFilter
{
    public class SwaggerHeaderAttribute : Attribute
    {
        public string HeaderName { get; }
        public string Description { get; }
        public string DefaultValue { get; }
        public bool IsRequired { get; }

        public SwaggerHeaderAttribute(string headerName, string description = null, string defaultValue = null, bool isRequired = false)
        {
            HeaderName = headerName;
            Description = description;
            DefaultValue = defaultValue;
            IsRequired = isRequired;
        }
    }
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {







        //if (operation.Parameters == null)
        //    operation.Parameters = new List<IParameter>();

        //operation.Parameters.Add(new NonBodyParameter
        //{
        //    Name = "X-User-Token",
        //    In = "header",
        //    Type = "string",
        //    Required = false
        //});
    }

    //public void Apply(OpenApiOperation operation, OperationFilterContext context)
    //{
    //    throw new NotImplementedException();
    //}
}

