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
    if (!dbContext.Card.Any())
    {
        dbContext.Database.EnsureCreated();

        var card1 = new Card("Mcano", 123, "123456", "123", 2000, 3, false, DateTimeOffset.Now);
        var card2 = new Card("LBabington", 1234, "1234564", "11", 4000, 0, false, DateTimeOffset.Now);
        var operation1 = new Operation(Entities.Enums.OperationType.WithDrawal, 6000, 4000, card1.CardId);
        var operation2 = new Operation(Entities.Enums.OperationType.Transfer, 4000, 5000, card1.CardId);

        dbContext.Card.AddRange(card1, card2);
        dbContext.Operation.AddRange(operation1, operation2);
        dbContext.SaveChanges();
    }
}

builder.Services.AddScoped<ICardRepository, CardRepository>();
builder.Services.AddScoped<IOperationRepository, OperationRepository>();
builder.Services.AddScoped<IValidateCardService, ValidateCardService>();
builder.Services.AddScoped<IGetBalanceInfo, GetBalanceInfo>();
builder.Services.AddScoped<IGetOperation, GetOperations>();
builder.Services.AddScoped<ILogUser, LogUser>();
builder.Services.AddScoped<IWithdrawal, Withdrawal>();
builder.Services.AddRouting();
builder.Services.AddSwaggerGen(c =>
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
builder.Services.AddEndpointsApiExplorer();

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
    return tokenHandler.WriteToken(token);
});

app.MapPost("api/withdrawal", async (ClaimsPrincipal user, string numberCard, int amount, UC.IWithdrawal useCase) =>
{
    await useCase.DoAsync(numberCard, amount);
}).RequireAuthorization();

app.MapGet("api/balance", async (string numberCard,UC.IGetBalanceInfo useCase) =>
{
    return await useCase.DoAsync(numberCard);
}).RequireAuthorization();

app.MapPost("api/operations", async (GetOperationsPaginated getOperationPaginated, UC.IGetOperation useCase) =>
{
    return await useCase.DoAsync(getOperationPaginated);
}).RequireAuthorization();
app.Run();