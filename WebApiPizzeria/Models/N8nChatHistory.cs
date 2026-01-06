using System;
using System.Collections.Generic;

namespace WebApiPizzeria.Models;

public partial class N8nChatHistory
{
    public int Id { get; set; }

    public string SessionId { get; set; } = null!;

    public string Message { get; set; } = null!;
}
