using System;
using System.Collections.Generic;

namespace PrivateChatService.Models;

public partial class Message
{
    public int Id { get; set; }

    public string SenderEmail { get; set; } = null!;

    public string ReceiverEmail { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime Timestamp { get; set; }
}
