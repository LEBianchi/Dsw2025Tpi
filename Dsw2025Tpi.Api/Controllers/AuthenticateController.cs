using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Domain.Interfaces;
using Dsw2025Tpi.Domain.Entities;

namespace Dsw2025Tpi.Api.Controllers
{
    [ApiController]
    [Route("/api/auth")]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly JwtTokenService _jwtTokenService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthenticateController> _logger;
        private readonly IRepository _repository;

        public AuthenticateController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            JwtTokenService jwtTokenService,
            RoleManager<IdentityRole> roleManager,
            ILogger<AuthenticateController> logger,
            IRepository repository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
            _roleManager = roleManager;
            _logger = logger;
            _repository = repository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null) return Unauthorized("Usuario incorrecto.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded) return Unauthorized("Contraseña incorrecta.");

            var userRoles = await _userManager.GetRolesAsync(user);
            // OJO: Asegúrate de que los roles en BD coincidan con mayúsculas/minúsculas
            var role = userRoles.FirstOrDefault() ?? "USER";

            Guid? customerId = null;
            string customerName = user.UserName;

            // Comparamos ignorando mayúsculas para evitar errores tontos
            if (role.ToUpper() == "USER")
            {
                var customer = await _repository.First<Customer>(c => c.Email == user.Email);

                if (customer != null)
                {
                    customerId = customer.Id;
                    customerName = customer.Name;
                }
                else
                {
                    _logger.LogWarning($"El usuario {user.Email} existe en Identity pero no tiene registro en Customers.");
                }
            }

            // Aquí pasamos el ID al token. Si es null, pasamos cadena vacía.
            var token = _jwtTokenService.GenerateToken(request.Username, role, customerId?.ToString() ?? "");

            return Ok(new
            {
                Token = token,
                UserInfo = new
                {
                    Email = user.Email,
                    IdentityId = user.Id,
                    CustomerId = customerId,
                    Name = customerName,
                    Role = role
                }
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var existingCustomer = await _repository.First<Customer>(c => c.Email == model.Email);
            if (existingCustomer != null)
            {
                return Conflict("El email ya está registrado en el sistema.");
            }

            var user = new IdentityUser { UserName = model.Username, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            string userRole = "USER";
            if (!await _roleManager.RoleExistsAsync(userRole))
            {
                await _roleManager.CreateAsync(new IdentityRole(userRole));
            }
            await _userManager.AddToRoleAsync(user, userRole);

            try
            {
                var newCustomer = new Customer
                {
                    Email = model.Email,
                    Name = model.Username,
                    // PhoneNumber es opcional ahora, así que no hace falta ponerlo
                };

                await _repository.Add(newCustomer);
            }
            catch (Exception ex)
            {
                await _userManager.DeleteAsync(user);
                _logger.LogError(ex, "Error al crear la entidad Customer. Rollback del usuario realizado.");
                return StatusCode(500, "Error creando el perfil del cliente.");
            }

            // --- CAMBIO IMPORTANTE AQUÍ ---

            
            var createdCustomer = await _repository.First<Customer>(c => c.Email == model.Email);

            // 2. Ahora sí generamos el token PASÁNDOLE EL ID (argumento 3)
            // Esto es crucial para que pueda comprar apenas se registra
            var token = _jwtTokenService.GenerateToken(
                model.Username,
                userRole,
                createdCustomer?.Id.ToString() ?? ""
            );

            return Ok(new
            {
                Token = token,
                Message = "Usuario registrado con éxito.",
                CustomerId = createdCustomer?.Id
            });
        }
    }
}