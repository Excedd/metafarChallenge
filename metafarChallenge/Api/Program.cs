using Domain.DTO;
using Domain.Repositories;
using Domain.Services;
using Domain.UseCases;
using Entities;
using Infraestructura;
using Infraestructura.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using UC = Domain.UseCases;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

void InitializeData(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
    if (!dbContext.Card.Any())
    {
        var cards = new List<Card>()
        {
            new Card("Mcano", 123, "123456", "123", 2000, 3, false, DateTimeOffset.Now),
            new Card("LBabington", 12345, "1234", "11", 4000, 0, false, DateTimeOffset.Now),
            new Card("ONoemi", 1234, "123", "123", 40000, 4, false, DateTimeOffset.Now)
        };

        var random = new Random();
        for (int i = 0; i < 12; i++)
        {
            var operation = new Operation(
                Entities.Enums.OperationType.WithDrawal,
                (decimal)random.NextDouble() * 1000,
                (decimal)random.NextDouble() * 2000,
                cards[2].CardId,
                DateTimeOffset.Now);

            dbContext.Operation.Add(operation);
        }

        dbContext.Card.AddRange(cards);
        dbContext.SaveChanges();
    }
}

builder.Services.AddScoped<ICardRepository, CardRepository>()
    .AddScoped<IOperationRepository, OperationRepository>()
    .AddScoped<IValidateCardService, ValidateCardService>()
    .AddScoped<IGetBalanceInfo, GetBalanceInfo>()
    .AddScoped<IGetOperation, GetOperations>()
    .AddScoped<ILogUser, LogUser>()
    .AddScoped<IWithdrawal, Withdrawal>()
    .AddRouting()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "MetafarChallenge", Version = "v1" });
        var securityScheme = new OpenApiSecurityScheme
        {
            Name = "test",
            Description = "test",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Reference = new OpenApiReference
            {
                Id = JwtBearerDefaults.AuthenticationScheme,
                Type = ReferenceType.SecurityScheme
            }
        };
        c.AddSecurityDefinition("Bearer", securityScheme);
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { securityScheme, Array.Empty<string>() }
        });
    });

var key = "00112233445566778899AABBCCDDEEFF";
builder.Services.AddAuthorization();
builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuerSigningKey = true,
    };
});

var app = builder.Build();

InitializeData(app.Services);

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("../swagger/v1/swagger.json", "v1");
    });

app.MapPost("api/login", async (string numberCard, string pin, UC.ILogUser useCase) =>
{
    try
    {
        await useCase.DoAsync(numberCard, pin);
        var tokenHandler = new JwtSecurityTokenHandler();
        var byteKey = Encoding.UTF8.GetBytes(key);
        var tokenDes = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Name,key),
            }),
            Expires = DateTime.UtcNow.AddMinutes(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(byteKey), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDes);
        return Results.Ok(tokenHandler.WriteToken(token));
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPost("api/withdrawal", async (ClaimsPrincipal user, string numberCard, int amount, UC.IWithdrawal useCase) =>
{
    try
    {
        var result = await useCase.DoAsync(numberCard, amount);
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
}).RequireAuthorization();

app.MapGet("api/balance", async (string numberCard, UC.IGetBalanceInfo useCase) =>
{
    try
    {
        var result = await useCase.DoAsync(numberCard);
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
}).RequireAuthorization();

app.MapPost("api/operations", async (GetOperationsPaginated getOperationPaginated, UC.IGetOperation useCase) =>
{
    try
    {
        var result = await useCase.DoAsync(getOperationPaginated);
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
}).RequireAuthorization();
app.Run();