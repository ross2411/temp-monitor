using System;

namespace TempMonitor.Shared
{
    public class Temperature
    {
        public DateTime Date => dateTime;
       
        public DateTime dateTime { get; set; }
        public double InsideTemp { get; set; }
        public double Humidity { get; set; }
        public double? OutsideTemp { get; set; }
        public string WeatherDescription { get; set; }
    }
}
