namespace BackendAPI.DTO.Reports
{
    public class QueueTimeDto
    {
        public string LocationName { get; set; }
        public string ChargerId { get; set; }

        public double AvgWaitMinutes { get; set; }
        public double MaxWaitMinutes { get; set; }

        public double AvgQueueLength { get; set; }   // vehicles waiting
        public double ThroughputPerHour { get; set; } // vehicles/hour
    }
}
