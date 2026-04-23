using Microsoft.AspNetCore.Mvc;

namespace TaskManagementApi.Filters;

public class RequireApiKeyAttribute : TypeFilterAttribute
{
    public RequireApiKeyAttribute() : base(typeof(RequireApiKeyFilter))
    {
    }
}