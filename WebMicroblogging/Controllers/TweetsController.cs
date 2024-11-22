using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebMicroblogging.Models;

namespace WebMicroblogging.Controllers
{
    [Authorize]
    public class TweetsController : Controller
    {
        private readonly MicrobloggingContext _context;

        public TweetsController(MicrobloggingContext context)
        {
            _context = context;
        }

        // Acción para mostrar los tweets en el índice
        public async Task<IActionResult> Index()
        {
            var tweets = await _context.Tweets
                .Include(t => t.User)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
            return View(tweets);
        }

        // GET: Tweets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tweets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTweetViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Obtener el UserId del usuario autenticado
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (Guid.TryParse(userIdString, out Guid userId))
                {
                    var tweet = new Tweet
                    {
                        Content = model.Content,
                        CreatedAt = DateTime.Now,
                        UserId = userId
                    };

                    // Manejar la imagen si se proporciona
                    if (model.Image != null && model.Image.Length > 0)
                    {
                        // Validar el tipo de archivo
                        var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var extension = Path.GetExtension(model.Image.FileName).ToLowerInvariant();

                        if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError("Image", "Tipo de archivo no permitido. Usa .jpg, .jpeg, .png, o .gif");
                            return View(model);
                        }

                        // Generar un nombre de archivo único
                        var fileName = Path.GetFileNameWithoutExtension(model.Image.FileName);
                        var uniqueFileName = $"{fileName}_{DateTime.Now.Ticks}{extension}";

                        // Ruta donde se guardará la imagen
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Crear la carpeta si no existe
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Guardar el archivo en el servidor
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.Image.CopyToAsync(stream);
                        }

                        // Almacenar la ruta relativa en el tweet
                        tweet.ImageUrl = $"/uploads/{uniqueFileName}";
                    }

                    _context.Tweets.Add(tweet);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index), "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Error al obtener el identificador de usuario.");
                }
            }

            // Si llegamos aquí, algo falló; volver a mostrar el formulario
            return View(model);
        }

        // GET: Tweets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tweet = await _context.Tweets.FindAsync(id);
            if (tweet == null)
            {
                return NotFound();
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId) || tweet.UserId != userId)
            {
                return Forbid();
            }

            var model = new EditTweetViewModel
            {
                Id = tweet.Id,
                Content = tweet.Content,
                ExistingImageUrl = tweet.ImageUrl
            };

            return View(model);
        }

        // POST: Tweets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditTweetViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var tweet = await _context.Tweets.FindAsync(id);
            if (tweet == null)
            {
                return NotFound();
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId) || tweet.UserId != userId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                tweet.Content = model.Content;

                if (model.Image != null && model.Image.Length > 0)
                {
                    var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(model.Image.FileName).ToLowerInvariant();

                    if (!permittedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("Image", "Tipo de archivo no permitido.");
                        return View(model);
                    }

                    var uniqueFileName = $"{Path.GetFileNameWithoutExtension(model.Image.FileName)}_{DateTime.Now.Ticks}{extension}";
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(stream);
                    }

                    if (!string.IsNullOrEmpty(tweet.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", tweet.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    tweet.ImageUrl = $"/uploads/{uniqueFileName}";
                }

                _context.Update(tweet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), "Home");
            }

            model.ExistingImageUrl = tweet.ImageUrl;
            return View(model);
        }

        // GET: Tweets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tweet = await _context.Tweets
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (tweet == null)
            {
                return NotFound();
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId) || tweet.UserId != userId)
            {
                return Forbid();
            }

            return View(tweet);
        }

        // POST: Tweets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tweet = await _context.Tweets
                .Include(t => t.Comments) // Incluye los comentarios relacionados
                .FirstOrDefaultAsync(m => m.Id == id);

            if (tweet == null)
            {
                return NotFound();
            }

            // Elimina los comentarios asociados
            _context.Comments.RemoveRange(tweet.Comments);

            // Ahora elimina el tweet
            _context.Tweets.Remove(tweet);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Like(int tweetId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized();
            }

            var tweet = await _context.Tweets.FindAsync(tweetId);
            if (tweet == null)
            {
                return NotFound();
            }

            // Verificar si el usuario ya dio like a este tweet
            var existingLike = await _context.Likes.FirstOrDefaultAsync(l => l.TweetId == tweetId && l.UserId == userId);
            if (existingLike == null)
            {
                var like = new Like
                {
                    TweetId = tweetId,
                    UserId = userId
                };

                _context.Likes.Add(like);
                await _context.SaveChangesAsync();

                // Devolver el recuento actualizado de likes
                var likesCount = await _context.Likes.CountAsync(l => l.TweetId == tweetId);
                return Ok(new { likesCount });
            }
            else
            {
                return BadRequest("Ya has dado like a este tweet.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Unlike(int tweetId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized();
            }

            var like = await _context.Likes.FirstOrDefaultAsync(l => l.TweetId == tweetId && l.UserId == userId);
            if (like != null)
            {
                _context.Likes.Remove(like);
                await _context.SaveChangesAsync();

                // Devolver el recuento actualizado de likes
                var likesCount = await _context.Likes.CountAsync(l => l.TweetId == tweetId);
                return Ok(new { likesCount });
            }
            else
            {
                return BadRequest("No has dado like a este tweet.");
            }
        }



        private bool TweetExists(int id)
        {
            return _context.Tweets.Any(e => e.Id == id);
        }
    }
}
