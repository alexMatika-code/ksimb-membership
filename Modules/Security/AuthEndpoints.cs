using System.Security.Claims;
using ksimb_membership.Modules.Members;
using Microsoft.AspNetCore.Authentication;

namespace ksimb_membership.Modules.Security;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
            app.MapGet("/auth/sign-out",
                async (HttpContext httpContext) =>
                {
                    await httpContext.SignOutAsync("KsimbAuth");

                    return Results.Redirect("/login");
                });

        app.MapPost("/auth/admin-sign-in",
            async (
                HttpContext httpContext,
                ISecurityService securityService,
                IMembersService membersService) =>
            {
                var form = await httpContext.Request.ReadFormAsync();

                var memberIdString = form["MemberId"].ToString();
                var secret = form["Secret"].ToString();

                if (!Guid.TryParse(memberIdString, out var memberId))
                {
                    return Results.BadRequest();
                }

                var member =
                    await membersService.GetMemberById(memberId);

                if (member is null || !member.IsAdmin)
                {
                    return Results.Unauthorized();
                }

                var valid =
                    await securityService.VerifyAdminSecret(secret);

                if (!valid)
                {
                    return Results.Unauthorized();
                }

                var claims = new List<Claim>
                {
                    new(
                        ClaimTypes.NameIdentifier,
                        member.Id.ToString()),

                    new(
                        ClaimTypes.Role,
                        "Admin")
                };

                var identity = new ClaimsIdentity(
                    claims,
                    "KsimbAuth");

                var principal = new ClaimsPrincipal(identity);

                await httpContext.SignInAsync(
                    "KsimbAuth",
                    principal);

                return Results.Redirect(
                    $"/profile/{member.Id}");
            });
    }
}