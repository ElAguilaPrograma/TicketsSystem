using System;
using System.Collections.Generic;

namespace TicketsSystem_Data;

public partial class Mcprequest
{
    public Guid McprequestId { get; set; }

    public Guid TicketId { get; set; }

    public string UseCase { get; set; } = null!;

    public string PromptVersion { get; set; } = null!;

    public DateTime RequestedAt { get; set; }

    public virtual ICollection<Mcpresponse> Mcpresponses { get; set; } = new List<Mcpresponse>();

    public virtual Ticket Ticket { get; set; } = null!;
}
