public class RouteModel
{
    public int Id { get; set; }
    public string Destination { get; set; } = string.Empty;

    public int BusId { get; set; }
    public BusModel Bus { get; set; } = null!;
}