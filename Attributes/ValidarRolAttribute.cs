using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;



public class ValidarRolAttribute : Attribute, IAuthorizationFilter
{
    private readonly int[] _rolesPermitidos;

    public ValidarRolAttribute(params int[] roles)
    {
        _rolesPermitidos = roles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var rol = context.HttpContext.Session.GetInt32("IdRol");

        if (rol == null || !_rolesPermitidos.Contains(rol.Value))
        {
            context.Result = new RedirectToActionResult("AccesoDenegado", "General", null);
        }
    }
}

