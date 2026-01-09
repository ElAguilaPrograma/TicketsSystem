using System;
using System.Collections.Generic;

namespace TicketsSystem_Data;

public partial class TicketPriority
{
    public int PriorityId { get; set; }

    public string Name { get; set; } = null!;

    public int Level { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
