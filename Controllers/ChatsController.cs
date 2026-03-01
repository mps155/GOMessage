using GOMessage.Data;
using GOMessage.Hubs;
using GOMessage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace GOMessage.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatsController : ControllerBase
{
    private readonly ChatDbContext _context;
    private readonly IHubContext<ChatHub> _hubContext;

    public ChatsController(ChatDbContext context, IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    [HttpPost]
    public async Task<IActionResult> CreateChat([FromBody] CreateChatDto dto)
    {
        var chat = new Chat
        {
            Title = dto.Title,
            IsGroupChat = dto.IsGroupChat
        };

        _context.Chats.Add(chat);
        await _context.SaveChangesAsync();

        return Ok(new { Id = chat.Id.ToString(), chat.Title, chat.IsGroupChat });
    }

    [HttpPost("{chatId}/join")]
    public async Task<IActionResult> JoinChat(string chatId, [FromBody] JoinChatDto dto)
    {
        var alreadyExists = await _context.UserChats
            .AnyAsync(uc => uc.ChatId == chatId && uc.UserId == dto.UserId);

        if (alreadyExists)
        {
            return Conflict("O usuário já faz parte deste chat.");
        }

        var userChat = new UserChat
        {
            ChatId = chatId,
            UserId = dto.UserId,
            Role = dto.Role
        };

        _context.UserChats.Add(userChat);
        await _context.SaveChangesAsync();

        return Ok("Usuário adicionado ao chat com sucesso.");
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserChats(string userId)
    {
        var userChatLinks = await _context.UserChats
            .Where(uc => uc.UserId == userId)
            .ToListAsync();

        var chatIds = userChatLinks.Select(uc => uc.ChatId).ToList();

        var chats = await _context.Chats
            .Where(c => chatIds.Contains(c.Id.ToString()))
            .ToListAsync();

        var result = chats.Select(c => new { Id = c.Id.ToString(), c.Title, c.IsGroupChat });
        return Ok(result);
    }

    [HttpPost("{chatId}/messages")]
    public async Task<IActionResult> SendMessage(string chatId, [FromBody] SendMessageDto dto)
    {
        var hasAccess = await _context.UserChats
            .AnyAsync(uc => uc.ChatId == chatId && uc.UserId == dto.SenderId);

        if (!hasAccess)
        {
            return Forbid("O usuário não tem acesso para enviar mensagens neste chat.");
        }

        var message = new Message
        {
            ChatId = chatId,
            SenderId = dto.SenderId,
            Content = dto.Content
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        var messageToReturn = new
        {
            Id = message.Id.ToString(),
            message.SenderId,
            message.Content,
            message.Timestamp
        };

        await _hubContext.Clients.Group(chatId).SendAsync("ReceiveMessage", messageToReturn);

        return Ok(messageToReturn);
    }

    [HttpGet("{chatId}/messages")]
    public async Task<IActionResult> GetChatMessages(string chatId, [FromQuery] string userId)
    {
        var hasAccess = await _context.UserChats
            .AnyAsync(uc => uc.ChatId == chatId && uc.UserId == userId);

        if (!hasAccess)
        {
            return Forbid("O usuário não tem acesso para ler as mensagens deste chat.");
        }

        var messages = await _context.Messages
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.Timestamp)
            .ToListAsync();

        var result = messages.Select(m => new
        {
            Id = m.Id.ToString(),
            m.SenderId,
            m.Content,
            m.Timestamp
        });

        return Ok(result);
    }
}

public class CreateChatDto
{
    public string Title { get; set; } = string.Empty;
    public bool IsGroupChat { get; set; }
}

public class JoinChatDto
{
    public string UserId { get; set; } = string.Empty;
    public string Role { get; set; } = "Member";
}

public class SendMessageDto
{
    public string SenderId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}