using System;
using System.Collections.Generic;

namespace TicketsSystem_Data;

public partial class Ticket
{
    public Guid TicketId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int StatusId { get; set; }

    public int PriorityId { get; set; }

    public Guid CreatedByUserId { get; set; }

    public Guid? AssignedToUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    public virtual User? AssignedToUser { get; set; }

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual ICollection<Mcprequest> Mcprequests { get; set; } = new List<Mcprequest>();

    public virtual TicketPriority Priority { get; set; } = null!;

    public virtual TicketStatus Status { get; set; } = null!;

    public virtual ICollection<TicketComment> TicketComments { get; set; } = new List<TicketComment>();

    public virtual ICollection<TicketHistory> TicketHistories { get; set; } = new List<TicketHistory>();
}
