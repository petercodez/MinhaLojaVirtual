using Microsoft.OpenApi.Models;
using MinhaLoja.web.Components;
using MinhaLoja.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using MinhaLoja.web.Services;


var builder = WebApplication.CreateBuilder(args);

// Configurações do JWT
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true
    };
});

// Adiciona serviços para o container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar os Controllers e configurar o JSON para ignorar ciclos
builder.Services.AddControllers()
    .AddJsonOptions(options => 
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT gerado no endpoint de login."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddScoped<CarrinhoService>();

// Registra o LocalStorage (O Cofre)
builder.Services.AddBlazoredLocalStorage();

// Registra o interceptor de Token (O Entregador)
builder.Services.AddScoped<JwtDelegatingHandler>();

// Ensina o sistema a criar o HttpClient ÚNICO, já com o interceptor embutido
builder.Services.AddScoped(sp =>
{
    var handler = sp.GetRequiredService<JwtDelegatingHandler>();
    handler.InnerHandler = new HttpClientHandler(); 
    
    return new HttpClient(handler) 
    { 
        BaseAddress = new Uri("http://localhost:5269/") 
    };
});

builder.Services.AddExceptionHandler<MinhaLoja.web.Handlers.GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddAuthorizationCore(); // Habilita as tags [Authorize] no Frontend

var app = builder.Build();

// >> PIPELINE <<
app.UseExceptionHandler();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication(); // 1º: Identifica QUEM é o utilizador (Lê o Token)
app.UseAuthorization();  // 2º: Verifica O QUE o utilizador pode fazer (Permissões)

app.UseAntiforgery(); // 3º: Protege os formulários Blazor (precisa de saber quem é o utilizador)

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// app.MapControllers();

// Decidir qual app.MapControllers() será mantido no final do projeto!
app.MapControllers().DisableAntiforgery();

// Rota kamikaze para testar o GlobalExceptionHandler
// app.MapGet("/api/teste-erro", () => 
// {
//     throw new Exception("Isso é um erro gravíssimo simulado que derrubaria o sistema!"); 
// });

app.Run();
