using System.IdentityModel.Tokens.Jwt;

namespace EduConnect_Front.Utilities
{
    public class JwtUtility
    {
        public static int? ObtenerIdUsuarioDesdeToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // En tu backend, agregaste el claim "NameIdentifier" con el IdUsuario
            var claim = jwtToken.Claims.FirstOrDefault(c => c.Type == "IdUsuario");


            if (claim != null && int.TryParse(claim.Value, out int idUsuario))
                return idUsuario;

            return null;
        }
    }
}
