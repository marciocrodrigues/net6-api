using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
using Blog.ViewModels.Posts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class PostController : ControllerBase
    {
        private readonly BlogDataContext _context;

        public PostController(BlogDataContext context)
        {
            _context = context;
        }

        [HttpGet("v1/posts")]
        public async Task<IActionResult> GetAsync(
            [FromQuery] string category,
            [FromQuery] string author,
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 25
            )
        {
            try
            {
                var count = 0;

                var posts = _context.Posts
                    .AsNoTracking()
                    .Include(x => x.Category)
                    .Include(x => x.Author)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(category))
                {
                    posts = posts.Where(x => x.Category.Name.Contains(category));
                    count = await posts.Where(x => x.Category.Name.Contains(category)).CountAsync();
                }

                if (!string.IsNullOrEmpty(author))
                {
                    posts = posts.Where(x => x.Author.Name.Contains(author));
                }

                count = await posts.CountAsync();

                var retorno = await posts.Select(x => new ListPostsViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Slug = x.Slug,
                    LastUpdateDate = x.LastUpdateDate,
                    Author = $"{x.Author.Name} ({x.Author.Email})",
                    Category = x.Category.Name
                })
                .Skip(page * pageSize)
                .Take(pageSize)
                .OrderByDescending(x => x.LastUpdateDate)
                .ToListAsync();

                return Ok(new ResultViewModel<dynamic>(new
                {
                    total = count > 0 ? count : 0,
                    page,
                    pageSize,
                    posts = retorno
                }));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<List<Post>>("Falha interna no servidor"));
            }
        }

        [HttpGet("v1/posts/{id:int}")]
        public async Task<IActionResult> DetailsAsync(
            [FromRoute] int id
            )
        {
            try
            {
                var count = await _context.Posts.AsNoTracking().CountAsync();

                var post = await _context.Posts
                    .AsNoTracking()
                    .Include(x => x.Author)
                    .ThenInclude(x => x.Roles)
                    .Include(x => x.Category)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (post is null)
                    return NotFound(new ResultViewModel<Post>("Conteúdo não encontrado"));

                return Ok(new ResultViewModel<Post>(post));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<List<Post>>("Falha interna no servidor"));
            }
        }
    }
}
