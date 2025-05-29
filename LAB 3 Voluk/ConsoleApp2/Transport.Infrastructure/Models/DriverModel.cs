public class DriverModel
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;

    public BusModel? Bus { get; set; }
}