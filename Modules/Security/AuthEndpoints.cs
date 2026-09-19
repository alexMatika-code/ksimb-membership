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

                if (member is null)
                {
                    return Results.Problem("MEMBER_NOT_FOUND", statusCode: 401);
                }

                if (!member.IsAdmin)
                {
                    return Results.Problem("MEMBER_NOT_ADMIN", statusCode: 403);
                }

                var valid = await securityService.VerifyAdminSecret(secret);

                if (!valid)
                {
                    return Results.Problem("INVALID_ADMIN_SECRET", statusCode: 401);
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

        app.MapPost("/auth/user-sign-in",
            async (
                HttpContext httpContext,
                IMembersService membersService) =>
            {
                var form = await httpContext.Request.ReadFormAsync();

                var oibString = form["OIB"].ToString();

                var member =
                    await membersService.GetMemberByPersonalId(oibString);

                if (member is null)
                {
                    return Results.Problem(
                        "MEMBER_NOT_FOUND",
                        statusCode: 401);
                }

                if (member.IsAdmin)
                {
                    return Results.Redirect(
                        $"/security-check/{member.Id}");
                }

                var claims = new List<Claim>
                {
                    new(
                        ClaimTypes.NameIdentifier,
                        member.Id.ToString()),

                    new(
                        ClaimTypes.Role,
                        "User")
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