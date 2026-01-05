namespace Chargersimulator.State;

public class ChargerState
{
    public string ChargerId { get; set; } = "";
    public string TenantId { get; set; } = "";

    public ChargerStatus Status { get; set; } = ChargerStatus.Available;

    public string? ActiveSessionId { get; set; }
    public string? ActiveUserId { get; set; }

    public string ActiveVin { get; set; }

    public double ActiveSoc { get; set; }




    public decimal TotalEnergyKwh { get; set; } = 0;
    public CancellationTokenSource? MeteringCts { get; set; }
}


public enum ChargerStatus
{
    Available,
    Preparing,
    Connected,
    Charging,
    Faulted
}
