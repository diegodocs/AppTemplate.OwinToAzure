using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace AppTemplate.OwinWebApp.StartApp
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {       

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            await Task.Run(() =>
            {
                string codigoCliente;
                string segredoCliente;

                if (!context.TryGetBasicCredentials(out codigoCliente, out segredoCliente))
                {
                    context.TryGetFormCredentials(out codigoCliente, out segredoCliente);
                }

                if (context.ClientId != null)
                {
                    context.OwinContext.Set("client_id", codigoCliente);
                    context.OwinContext.Set("AllowedOrigin", "true");
                    context.OwinContext.Set("RefreshTokenLifeTime", "1");
                }

                context.Validated();
            });
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            await Task.Run(() =>
            {
                var usuario = new UsuarioContexto() { Login = "admin", Senha="123qwe" };

                if (usuario ==null)
                {
                    context.SetError("invalid_grant", "Usuário ou senha inválidos");
                    return;
                }

                var identidade = new ClaimsIdentity(context.Options.AuthenticationType);

                identidade.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                identidade.AddClaim(new Claim(ClaimTypes.Email, context.ClientId));

                var principal = new GenericPrincipal(identidade, new[] { "" });
                Thread.CurrentPrincipal = principal;

                var propriedadeAutenticacao = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {"client_id", context.ClientId}
                });

                var ticket = new AuthenticationTicket(identidade, propriedadeAutenticacao);
                context.Validated(ticket);
            });
        }

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var codigoClienteOriginal = context.Ticket.Properties.Dictionary["client_id"];
            var codigoClienteAtual = context.OwinContext.Get<string>("client_id");

            if (codigoClienteOriginal == codigoClienteAtual)
            {
                var novoCodigo = new ClaimsIdentity(context.Ticket.Identity);
                novoCodigo.AddClaim(new Claim("newClaim", "refreshToken"));

                var newTicket = new AuthenticationTicket(novoCodigo, context.Ticket.Properties);
                context.Validated(newTicket);
            }
            else
            {
                context.Rejected();
            }

            return Task.FromResult<object>(null);
        }

        public class UsuarioContexto
        {
            public string Login { get; set; }
            public string Senha { get; set; }
        }
    }
}