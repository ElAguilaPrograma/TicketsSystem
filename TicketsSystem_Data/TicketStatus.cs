using System;
using System.Collections.Generic;

namespace TicketsSystem_Data;

public partial class TicketStatus
{
    public int StatusId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
