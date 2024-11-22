using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebMicroblogging.Models;

namespace WebMicroblogging.Controllers
{
    public class HomeController : Controller
    {
        private readonly MicrobloggingContext _context;

        public HomeController(MicrobloggingContext context)
        {
            _context = context;
        }

        // Acci�n para mostrar todos los Tweets
        [Authorize]
        public async Task<IActionResult> Index()
        {
            // Obtener todos los tweets ordenados por fecha de creaci�n descendente
            var tweets = await _context.Tweets
                .Include(t => t.User) // Incluir informaci�n del usuario que public� el tweet
                .Include(t => t.Comments)
                .Include(t => t.Likes)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(tweets);
        }
    }
}
