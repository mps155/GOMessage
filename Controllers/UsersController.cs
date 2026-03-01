using GOMessage.Data;
using GOMessage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GOMessage.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ChatDbContext _context;

    public UsersController(ChatDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _context.Users.ToListAsync();

        var result = users.Select(u => new
        {
            Id = u.Id.ToString(),
            u.Name,
            u.Email,
            u.CreatedAt
        });

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == id);

        if (user == null)
        {
            return NotFound("Usuário não encontrado.");
        }

        return Ok(new
        {
            Id = user.Id.ToString(),
            user.Name,
            user.Email,
            user.CreatedAt
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Email))
        {
            return BadRequest("O Nome e o Email são obrigatórios.");
        }

        var emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
        if (emailExists)
        {
            return Conflict("Já existe um usuário cadastrado com este e-mail.");
        }

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = user.Id.ToString() }, new
        {
            Id = user.Id.ToString(),
            user.Name,
            user.Email,
            user.CreatedAt
        });
    }
}

public class CreateUserDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}