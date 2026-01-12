namespace BackendAPI.DTO.Reports
{
    public class ReportRowDto
    {
        public string LocationName { get; set; }
        public string ChargerId { get; set; }

        public double EnergyKwh { get; set; }
        public double ChargingDurationMinutes { get; set; }
        public double UtilizationPercent { get; set; }

        public int TotalSessions { get; set; }

        public double UptimePercent { get; set; }
        public double DowntimePercent { get; set; }

        public double Co2SavedKg { get; set; }
        public double Cost { get; set; }
    }
}
