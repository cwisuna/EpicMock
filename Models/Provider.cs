using System;
using System.Collections.Generic;

namespace EpicMock.Models;

public partial class Provider
{
    public int ProviderId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Specialty { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<ProviderAvailability> ProviderAvailabilities { get; set; } = new List<ProviderAvailability>();
}
