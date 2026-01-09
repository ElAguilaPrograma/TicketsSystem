using System;
using System.Collections.Generic;

namespace TicketsSystem_Data;

public partial class TicketHistory
{
    public Guid HistoryId { get; set; }

    public Guid TicketId { get; set; }

    public Guid ChangedByUserId { get; set; }

    public string FieldName { get; set; } = null!;

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public DateTime ChangedAt { get; set; }

    public virtual User ChangedByUser { get; set; } = null!;

    public virtual Ticket Ticket { get; set; } = null!;
}
