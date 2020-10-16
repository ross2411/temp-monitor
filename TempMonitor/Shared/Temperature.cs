using System;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;

namespace TempMonitor.Shared
{
    public class Temperature
    {
        public DateTime Date => dateTime;
        [Index(0)]
        [TypeConverter(typeof(CustomDateTimeConverter))]
        public DateTime dateTime { get; set; }
        [Index(1)]
        public double InsideTemp { get; set; }
        [Index(2)]
        public double Humidity { get; set; }
        [Index(3)]
        public double? OutsideTemp { get; set; }
        [Index(4)]
        public string WeatherDescription { get; set; }
    }
}
