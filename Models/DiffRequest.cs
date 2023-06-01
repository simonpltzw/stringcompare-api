using Swashbuckle.AspNetCore.Annotations;

namespace StringCompare.API.Models
{
    [SwaggerSchema("Text difference request payload model.", Required = new[] { nameof(Text1), nameof(Text2) })]
    public class DiffRequest
    {
        [SwaggerSchema("The original text.", WriteOnly = true)]
        public string? Text1 { get; set; }

        [SwaggerSchema("The changed text.", WriteOnly = true)]
        public string? Text2 { get; set; }
    }
}
