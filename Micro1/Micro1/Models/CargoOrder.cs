namespace CargoTransportApi.Models
{
    public class CargoOrder
    {
        public int Id { get; set; }
        public string? SenderName { get; set; }
        public string? ReceiverName { get; set; }
        public string? OriginCity { get; set; }
        public string? DestinationCity { get; set; }
        public double Weight { get; set; }
        public string? CargoDescription { get; set; }
        public DateTime ShipmentDate { get; set; }
        public string? Status { get; set; }
    }
}