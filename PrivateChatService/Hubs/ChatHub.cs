using Microsoft.AspNetCore.SignalR;
using PrivateChatService.Data;
using PrivateChatService.Models;

using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;
using PrivateChatService.Models;
using PrivateChatService.Data;
using Microsoft.EntityFrameworkCore;


namespace PrivateChatService.Hubs
{
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;

        public ChatHub(AppDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string senderEmail, string receiverEmail, string message)
        {
            var chatMessage = new Message
            {
                SenderEmail = senderEmail,
                ReceiverEmail = receiverEmail,
                Content = message,
                Timestamp = DateTime.Now
            };

            _context.Messages.Add(chatMessage);
            await _context.SaveChangesAsync();

            await Clients.User(receiverEmail).SendAsync("ReceiveMessage", senderEmail, message);
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
    }
}
