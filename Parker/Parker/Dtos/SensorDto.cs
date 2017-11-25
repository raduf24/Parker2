using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class SensorDto
{
    public SensorDto()
    {
    }

    public SensorDto(string jsonPath)
    {

    }

    public string Name { get; set; }
    public string Address { get; set; }
    public string Coordinates { get; set; }
    public string MapUrl { get; set; }
    public string EmptyImg { get; set; }
    public string inputSample { get; set; }
    public string SensorMask { get; set; }
    public ParkingSpaceDto[] ParkingSpaces { get; set; }

}