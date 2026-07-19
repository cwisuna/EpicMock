using System;
using System.Collections.Generic;

namespace EpicMock.Models;

public partial class ProviderAvailability
{
    public int AvailabilityId { get; set; }

    public int ProviderId { get; set; }

    public byte DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public virtual Provider Provider { get; set; } = null!;
}
