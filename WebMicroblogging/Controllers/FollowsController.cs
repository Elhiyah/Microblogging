using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebMicroblogging.Models;

namespace WebMicroblogging.Controllers
{
    [Authorize]
    public class FollowsController : Controller
    {
        private readonly MicrobloggingContext _context;

        public FollowsController(MicrobloggingContext context)
        {
            _context = context;
        }

        // Acción para mostrar los usuarios que el usuario autenticado está siguiendo
        [HttpGet]
        public async Task<IActionResult> Following()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(currentUserId, out Guid userId))
            {
                return Unauthorized();
            }

            // Obtener la lista de usuarios que el usuario actual sigue
            var following = await _context.Follows
                .Where(f => f.FollowerId == userId)
                .Include(f => f.User)
                .Select(f => f.User)
                .ToListAsync();

            return View(following);
        }

        // Acción para seguir a un usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Follow(Guid userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(currentUserId, out Guid followerId))
            {
                return Unauthorized();
            }

            // Verificar que no se esté siguiendo a sí mismo
            if (userId == followerId)
            {
                return BadRequest("No puedes seguirte a ti mismo.");
            }

            // Verificar si ya lo sigue
            var existingFollow = await _context.Follows
                .FirstOrDefaultAsync(f => f.UserId == userId && f.FollowerId == followerId);
            if (existingFollow == null)
            {
                var follow = new Follow
                {
                    UserId = userId,
                    FollowerId = followerId
                };
                _context.Follows.Add(follow);
                await _context.SaveChangesAsync();
            }

            return Ok(new { success = true });
        }

        // Acción para dejar de seguir a un usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unfollow(Guid userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(currentUserId, out Guid followerId))
            {
                return Unauthorized();
            }

            var follow = await _context.Follows
                .FirstOrDefaultAsync(f => f.UserId == userId && f.FollowerId == followerId);
            if (follow != null)
            {
                _context.Follows.Remove(follow);
                await _context.SaveChangesAsync();
            }

            return Ok(new { success = true });
        }
    }
}
