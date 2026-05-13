using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MinhaLoja.Core.DTOs;

namespace MinhaLoja.web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager; // 1. O Gerente de Cargos!
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar(LoginDTO dto)
        {
            var user = new IdentityUser { UserName = dto.Email, Email = dto.Email };
            
            // O Identity cria o usuário e já faz o Hash (criptografia) da senha automaticamente
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            // 1. Verifica se o cargo base existe. Se não, cria.
            if (!await _roleManager.RoleExistsAsync("Cliente"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Cliente"));
            }

            // 2. Todo mundo que se cadastra pela loja ganha APENAS a role de Cliente
            await _userManager.AddToRoleAsync(user, "Cliente");

            return Ok(new { Mensagem = "Usuário criado com sucesso!" });
        }

        // NOVA ROTA: Rota secreta para registrar Administradores
        [HttpPost("registrar-admin")]
        public async Task<IActionResult> RegistrarAdmin(LoginDTO dto)
        {
            // Verifica se a gaveta "Admin" já existe no banco. Se não, cria ela na hora.
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var user = new IdentityUser { UserName = dto.Email, Email = dto.Email };
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            // Vincula o usuário recém-criado ao cargo de Admin
            await _userManager.AddToRoleAsync(user, "Admin");

            return Ok(new { Mensagem = "Administrador criado com sucesso!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized(new { Mensagem = "Email ou senha inválidos." });

            // Busca no banco de dados quais são os cargos desse usuário
            var userRoles = await _userManager.GetRolesAsync(user);

            // Prepara as informações básicas do crachá
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!)
            };

            // Injeta cada cargo do usuário dentro do crachá JWT
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]!);
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(authClaims), // Agora o token carrega os cargos!
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            return Ok(new { Token = tokenHandler.WriteToken(token) });
        }
    }
}