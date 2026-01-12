namespace BackendAPI.DTO.Dashboard
{
    public class DashboardSummaryDto
    {
        public int TotalChargePoints { get; set; }
        public int ActiveChargePoints { get; set; }
        public int InactiveChargePoints { get; set; }

        public int LiveSessions { get; set; }
        public int Drivers { get; set; }

        public decimal EnergyMwh { get; set; }
        public decimal Co2SavedTonnes { get; set; }
    }
}
