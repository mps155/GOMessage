using GOMessage.Data;
using GOMessage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MinhaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly ChatDbContext _context;

    public MessagesController(ChatDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetMessages()
    {
        var messages = await _context.Messages
            .OrderBy(m => m.Timestamp)
            .ToListAsync();

        var result = messages.Select(m => new
        {
            Id = m.Id.ToString(),
            m.Content,
            m.SenderId,
            m.Timestamp
        });

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> PostMessage([FromBody] CreateMessageDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Content) || string.IsNullOrWhiteSpace(dto.SenderId))
        {
            return BadRequest("Conteúdo e Remetente são obrigatórios.");
        }

        if (string.IsNullOrWhiteSpace(dto.ReceiverrId))
        {
            return BadRequest("Destinatário é obrigatório.");
        }

        var message = new Message
        {
            Content = dto.Content,
            SenderId = dto.SenderId,
            ChatId = dto.ChatId,
            Timestamp = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMessages), new { id = message.Id.ToString() }, new
        {
            Id = message.Id.ToString(),
            message.Content,
            message.SenderId,
            message.Timestamp
        });
    }
}

public class CreateMessageDto
{
    public string Content { get; set; } = string.Empty;
    public string SenderId { get; set; } = string.Empty;
    public string ReceiverrId { get; set; } = string.Empty;
    public string ChatId { get; set; } = string.Empty;
}