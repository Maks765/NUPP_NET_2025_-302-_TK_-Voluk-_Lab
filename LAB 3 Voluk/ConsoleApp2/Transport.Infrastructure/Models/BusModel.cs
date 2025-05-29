public class BusModel
{
    public int Id { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public int Capacity { get; set; }

    public int DriverId { get; set; }
    public DriverModel Driver { get; set; } = null!;

    public ICollection<RouteModel> Routes { get; set; } = new List<RouteModel>();
}