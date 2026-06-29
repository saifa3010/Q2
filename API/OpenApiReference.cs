using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;

internal class BaseOpenApiReference : OpenApiReference
{
    public ReferenceType Type { get; set; }
    public string Id { get; set; }
}