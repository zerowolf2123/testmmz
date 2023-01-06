using db.Requests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging.Configuration;
using System.Data;
using System.Security.Claims;
using TestMMZ.Model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opions => opions.LoginPath = "/login");
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
    builder.AddDebug();
});
ILogger logger = loggerFactory.CreateLogger("StarWork");

var mainPage = app.MapGroup("/");
var loginPage = app.MapGroup("/login");

mainPage.MapGet("", [Authorize] async (HttpContext context, IConfiguration appConfig) =>
{
    logger.LogInformation("Загрузка главной странице");
    context.Response.StatusCode = 200;
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync("wwwroot/html/index.html");
});
mainPage.MapGet("/get-all-products", [Authorize] (IConfiguration appConfig) =>
{
    logger.LogInformation("Возвращаем список всей продукции");
    string? connectionString = appConfig["ConnectionStrings:DefaultConnection"];
    if (connectionString != null)
    {
        var mainRequests = new MainRequests(connectionString);
        var products = mainRequests.GetAllProducts();
        return Results.Json(products);
    }
    logger.LogError("Неудалось найти данный для подключения");
    return Results.BadRequest();
});
mainPage.MapGet("get-all-details/{oboz}", [Authorize] (string oboz, IConfiguration appConfig) =>
{
    logger.LogInformation("Возвращаем список деталей, этой продукции");
    string? connectionString = appConfig["ConnectionStrings:DefaultConnection"];
    if (connectionString != null)
    {
        var mainRequests = new MainRequests(connectionString);
        int productId = mainRequests.GetProductId(oboz);
        if (productId != 0)
        {
            var detailsId = mainRequests.GetDetailsId(productId);
            if (detailsId != null)
            {
                var details = mainRequests.GetDetails(detailsId);
                return Results.Json(details);
            }
        }
    }
    logger.LogError("Неудалось найти данный для подключения");
    return Results.BadRequest();
});

loginPage.MapGet("", async (string? returnUrl, HttpContext context) =>
{
    logger.LogInformation("Загрузка страницы авторизации");
    context.Response.StatusCode = 200;
    context.Response.ContentType = "text/html; charset=utf-8";
    context.Response.Cookies.Append("returnUrl", returnUrl);
    await context.Response.SendFileAsync("wwwroot/html/login.html");
});
loginPage.MapPost("", async (User user, HttpContext context, IConfiguration appConfig) =>
{
    logger.LogInformation("Передача аутентификационных данных на авторизацию");
    string? connectionString = appConfig["ConnectionStrings:DefaultConnection"];
    if (connectionString != null)
    {
        var userRequests = new UserRequests(connectionString);
        int id = userRequests.SearchUserId(user.Login, user.Password);
        if (id != 0)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Login)
            };
            var claimIdentity = new ClaimsIdentity(claims, "Cookies");
            var claimPrincipal = new ClaimsPrincipal(claimIdentity);

            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal);

            return Results.Json(context.Request.Cookies["returnUrl"]);
        }
        logger.LogError("Неудалось найти пользователя");
    }
    logger.LogError("Неудалось найти данный для подключения");
    return Results.BadRequest();
});

app.Run();
