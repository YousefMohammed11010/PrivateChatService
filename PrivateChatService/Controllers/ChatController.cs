using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PrivateChatService.Data;
using PrivateChatService.Models;
using PrivateChatService.DTOs;
using System.Security.Claims;


[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class ChatController : ControllerBase
{
    private readonly AppDbContext _context;

    public ChatController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("messages")]
    public IActionResult GetMessages([FromBody] WithUserDto dto)
    {
        var myEmail = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(myEmail))
            return Unauthorized("Email not found in token.");

        var messages = _context.Messages
            .Where(m => (m.SenderEmail == myEmail && m.ReceiverEmail == dto.WithUser) ||
                        (m.SenderEmail == dto.WithUser && m.ReceiverEmail == myEmail))
            .OrderBy(m => m.Timestamp)
            .ToList();

        return Ok(messages);
    }


    [HttpPost("send")]
    public IActionResult SendMessage([FromBody] Message msg)
    {
        var myEmail = User.FindFirstValue(ClaimTypes.Email);
        if (myEmail == null || myEmail != msg.SenderEmail)
        {
            return Unauthorized("Invalid sender.");
        }

        msg.Timestamp = DateTime.UtcNow;
        _context.Messages.Add(msg);
        _context.SaveChanges();
        return Ok(msg);
    }
}
