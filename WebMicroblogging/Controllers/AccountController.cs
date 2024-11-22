using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebMicroblogging.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;

namespace WebMicroblogging.Controllers
{
    public class AccountController : Controller
    {
        private readonly MicrobloggingContext _context;

        public AccountController(MicrobloggingContext context)
        {
            _context = context;
        }
        private const string RootEmail = "root@example.com";  // Cambia esto al correo root


        // Acción para mostrar el formulario de login
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            // Si el usuario ya está autenticado, redirigirlo a la página de tweets
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // Acción para procesar el login
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Buscar el usuario por nombre de usuario o email
                var user = await _context.Users.FirstOrDefaultAsync(u =>
                    (u.UserName == model.UsernameOrEmail || u.Email == model.UsernameOrEmail) && u.Activo == true);

                if (user != null)
                {
                    // Verificar la contraseña
                    if (VerifyPassword(model.Password, user.PasswordHash))
                    {
                        // Crear los claims de usuario
                        var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName)
                    };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity));

                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("", "Usuario o contraseña incorrectos");
            }
            return View(model);
        }


        // Mostrar formulario de registro
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Procesar registro
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el nombre de usuario o email ya existen
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == model.UserName || u.Email == model.Email);
                if (existingUser == null)
                {
                    // Crear nuevo usuario con un Id de tipo Guid
                    var user = new User
                    {
                        Id = Guid.NewGuid(),
                        UserName = model.UserName,
                        Email = model.Email,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                        FechaNacimiento = model.FechaNacimiento,
                        Activo = true,
                        // Asigna otros campos según sea necesario
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    // Redirigir al login o iniciar sesión automáticamente
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ModelState.AddModelError("", "El nombre de usuario o correo electrónico ya están en uso");
                }
            }
            return View(model);
        }

        // Acción para cerrar sesión
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        // Método para verificar la contraseña
        private bool VerifyPassword(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }

        // Acción para mostrar el perfil del usuario autenticado
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            if (user == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            var model = new ProfileViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                FechaNacimiento = user.FechaNacimiento
            };

            return View(model);
        }

        // Acción para mostrar el formulario de edición del perfil
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            if (user == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            var model = new EditProfileViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                FechaNacimiento = user.FechaNacimiento
            };

            return View(model);
        }

        // Acción para procesar la edición del perfil
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            if (user == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            if (ModelState.IsValid)
            {
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.FechaNacimiento = model.FechaNacimiento;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Profile");
            }

            return View(model);
        }

        // Acción para listar todos los usuarios (accesible solo por el usuario root)
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> AllUsers()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email); // Obtenemos el correo del usuario autenticado

            // Verificamos si el usuario es root
            if (userEmail != RootEmail)
            {
                return Unauthorized("No tienes permiso para ver esta página.");
            }

            // Obtener todos los usuarios registrados
            var users = await _context.Users.ToListAsync();

            return View(users);
        }
    }
 }
