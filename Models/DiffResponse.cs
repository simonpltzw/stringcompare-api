using Swashbuckle.AspNetCore.Annotations;

namespace StringCompare.API.Models
{
    [SwaggerSchema("Text difference response payload model.")]
    public class DiffResponseElement
    {
        [SwaggerSchema("One of: INSERT, DELTE or EQUAL.", Nullable = false, ReadOnly = true)]
        public string Operation { get; set; } = string.Empty;

        [SwaggerSchema("The text associated with this diff operation.", Nullable = false, ReadOnly = true)]
        public string Text { get; set; } = string.Empty;
    }
}
