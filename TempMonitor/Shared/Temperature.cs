using System;
using CsvHelper.Configuration.Attributes;
namespace TempMonitor.Shared
{
    public class Temperature
    {
        public DateTime Date => dateTime;
        [Index(0)]
        public DateTime dateTime { get; set; }
        [Index(1)]
        public double InsideTemp { get; set; }
        [Index(2)]
        public double Humidity { get; set; }
    }

    public class Period
    {
        public string Name { get; set; }
        public int Value { get; set; }  
    }
}
