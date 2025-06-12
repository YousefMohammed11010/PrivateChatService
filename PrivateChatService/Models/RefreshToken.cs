using System;
using System.Collections.Generic;

namespace PrivateChatService.Models;

public partial class RefreshToken
{
    public string ApplicationUserId { get; set; } = null!;

    public int Id { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public virtual AspNetUser ApplicationUser { get; set; } = null!;
}
