using Microsoft.AspNetCore.Authorization;

namespace ReceitasApi.Attributes
{
    public class RolesAttribute : AuthorizeAttribute
    {  
        public RolesAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }        
    }
}