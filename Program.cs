using ksimb_membership.Components;
using ksimb_membership.Modules;
using ksimb_membership.Modules.Members;
using ksimb_membership.Modules.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddAntiforgery();

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddAuthentication("KsimbAuth")
    .AddCookie("KsimbAuth", options =>
    {
        options.Cookie.Name = "ksimb_auth";

        options.LoginPath = "/login";
        options.AccessDeniedPath = "/access-denied";

        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;

        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<IMembersService, MembersService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ISecurityService, SecurityService>();
builder.Services.AddScoped<IPasswordHasher<SecuritySettings>, PasswordHasher<SecuritySettings>>();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

await DatabaseInitializer.InitializeAsync(app.Services);

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
app.MapAuthEndpoints();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapGet("/admin/members/export",
        async (
            IMembersService exportService) =>
        {
            var bytes = await exportService.ExportMembers();

            return Results.File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"clanovi-{DateTime.Now:yyyy-MM-dd}.xlsx");
        })
    .RequireAuthorization(policy =>
        policy.RequireRole("Admin"));
app.Run();