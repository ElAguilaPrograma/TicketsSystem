using System;
using System.Collections.Generic;

namespace TicketsSystem_Data;

public partial class User
{
    public Guid UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Ticket> TicketAssignedToUsers { get; set; } = new List<Ticket>();

    public virtual ICollection<TicketComment> TicketComments { get; set; } = new List<TicketComment>();

    public virtual ICollection<Ticket> TicketCreatedByUsers { get; set; } = new List<Ticket>();

    public virtual ICollection<TicketHistory> TicketHistories { get; set; } = new List<TicketHistory>();
}
