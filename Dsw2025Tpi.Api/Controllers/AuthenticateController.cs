using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Application.Dtos;
using Microsoft.Extensions.Logging;

namespace Dsw2025Tpi.Api.Controllers
{
    [ApiController]
    [Route("/api/authenticate")]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly JwtTokenService _jwtTokenService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthenticateController> _logger;
        public AuthenticateController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            JwtTokenService jwtTokenService,
            RoleManager<IdentityRole> roleManager,
            ILogger<AuthenticateController> logger)

        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel request)
        {
            _logger.LogInformation("Recibida solicitud de login para el usuario: {Username}", request.Username);
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                _logger.LogWarning("Login fallido. Usuario no encontrado: {Username}", request.Username);
                return Unauthorized("Usuario o contraseña incorrectos.");
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Login fallido. Contraseña incorrecta para el usuario: {Username}", request.Username);
                return Unauthorized("Usuario o contraseña incorrectos.");
            }
            var userRoles = await _userManager.GetRolesAsync(user);
            var role = userRoles.FirstOrDefault();

            if (role == null)
            {
                _logger.LogError("Error critico: El usuario {Username} no tiene un rol asignado.", user.UserName);
                return StatusCode(500, "Error del sistema: El usuario autenticado no tiene un rol asignado.");
            }
            var token = _jwtTokenService.GenerateToken(request.Username, role);
            _logger.LogInformation("Usuario {Username} autenticado exitosamente.", request.Username);
            return Ok(new { Token = token, Message = "Secion iniciada con Exito." });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            _logger.LogInformation("Recibida solicitud de registro para el usuario: {Username}", model.Username);
            var user = new IdentityUser { UserName = model.Username, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Fallo el registro para el usuario {Username}. Errores: {Errors}", model.Username, string.Join(", ", result.Errors.Select(e => e.Description)));
                return BadRequest(result.Errors);
            }

            string userRole = "User";
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                _logger.LogInformation("El rol '{Role}' no existe, creándolo...", userRole);
                await _roleManager.CreateAsync(new IdentityRole(userRole));
            }

            await _userManager.AddToRoleAsync(user, "User");

            var userRoles = await _userManager.GetRolesAsync(user);
            var role = userRoles.FirstOrDefault();

            if (role == null)
            {
                _logger.LogError("Se produjo un error en el sistema y no se pudo asignar un rol al nuevo usuario");
                return StatusCode(500, "Error interno: No se pudo asignar el rol 'User' al nuevo usuario.");
            }

            _logger.LogInformation("Usuario {Username} registrado y asignado al rol '{Role}'.", model.Username, userRole);
            var token = _jwtTokenService.GenerateToken(model.Username, role);

            return Ok(new { Token = token, Message = "Usuario registrado y sesión iniciada con éxito." });
        }
    }
}
