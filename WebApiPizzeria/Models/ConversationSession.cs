using System;
using System.Collections.Generic;

namespace WebApiPizzeria.Models;

public partial class ConversationSession
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public string ConversationState { get; set; } = null!;

    public DateTime LastUpdatedAt { get; set; }

    public DateTime CreatedAt { get; set; }
}
