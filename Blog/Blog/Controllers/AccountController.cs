using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Blog.ViewModels.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Blog.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class AccountController : ControllerBase
    {
        private readonly BlogDataContext _context;
        private readonly TokenService _tokenService;
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;

        public AccountController(TokenService tokenService, BlogDataContext context, EmailService emailService, IConfiguration configuration)
        {
            _tokenService = tokenService;
            _context = context;
            _emailService = emailService;
            _configuration = configuration;
        }

        [HttpPost("v1/accounts/")]
        public async Task<IActionResult> Post([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<User>(ModelState.GetErrors()));

            try
            {
                var user = new User
                {
                    Name = model.Name,
                    Email = model.Email,
                    Slug = model.Email.Replace("@", "-").Replace(".", "-"),
                    PasswordHash = AuthService.ComputeSha256Hash(model.Password)
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                _emailService.Send(user.Name, user.Email, "Bem vindo ao blog!", $"<strong>Sua senha é {model.Password}</strong>");

                return Ok(new ResultViewModel<dynamic>(new
                {
                    user = user.Email,
                    model.Password
                }));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(400, new ResultViewModel<string>("Este E-mail já está cadastrado"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("Falha interna no servidor"));
            }

        }

        [HttpPost("v1/login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            try
            {
                var passwordHash = AuthService.ComputeSha256Hash(model.Password);

                var user = await _context
                    .Users
                    .AsNoTracking()
                    .Include(x => x.Roles)
                    .FirstOrDefaultAsync(x => x.Email == model.Email);

                if (user is null)
                    return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));

                if (user.PasswordHash != passwordHash)
                    return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));

                var token = _tokenService.GenerateToken(user);

                return Ok(new ResultViewModel<string>(token, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("Falha interna no servidor"));
            }
        }

        [Authorize]
        [HttpPost("v1/accounts/upload-image")]
        public async Task<IActionResult> UploadImage([FromBody] UploadImageViewModel model)
        {
            var fileName = $"{Guid.NewGuid().ToString()}.jpg";
            var data = new Regex(@"^data:image\/[a-z]+;base64,")
                .Replace(model.Base64Image, "");
            var bytes = Convert.FromBase64String(data);

            try
            {
                await System.IO.File.WriteAllBytesAsync($"wwwroot/images/{fileName}", bytes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Falha interna no servidor");
            }

            var user = await _context
                .Users
                .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

            if (user is null)
                return NotFound(new ResultViewModel<User>("Usuário não encontrado"));

            user.Image = $"{_configuration.GetValue<string>("UrlImagens")}{fileName}";

            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Falha interna no servidor");
            }

            return Ok(new ResultViewModel<string>("Imagem alterada com sucesso!", null));
        }
    }
}
