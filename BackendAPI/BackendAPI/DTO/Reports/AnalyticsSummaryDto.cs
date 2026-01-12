namespace BackendAPI.DTO.Reports
{
    public class AnalyticsSummaryDto
    {
        public int TotalSessions { get; set; }
        public double UtilizationPercent { get; set; }
        public double AvgDurationMinutes { get; set; }

        public int RemoteStart { get; set; }
        public int RemoteStop { get; set; }

        public double AvgSOCStart { get; set; }
        public double AvgSOCEnd { get; set; }

        public double TotalEnergyKwh { get; set; }
        public double AvgEnergyPerSession { get; set; }

        public double TotalCost { get; set; }
        public double AvgCostPerSession { get; set; }
        public double CostPerKwh { get; set; }

        public double Co2SavedKg { get; set; }
    }
}
