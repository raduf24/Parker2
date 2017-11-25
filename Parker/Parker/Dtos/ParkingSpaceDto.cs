public class ParkingSpaceDto
{
    public ParkingSpaceDto()
    {        
    }

    public string Name { get; set; }
    public byte Threshold { get; set; }
    public string LocationMask { get; set; }
    public string SensorMask { get; set; }    
}