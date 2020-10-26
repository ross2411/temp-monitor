using CsvHelper.Configuration;
using TempMonitor.Shared;

namespace TempMonitor.Server.CsvMapper
{
    public sealed class TemperatureCsvMapper: ClassMap<Temperature>
    {
        internal TemperatureCsvMapper()
        {
            Map(m => m.dateTime).Index(0)
                .TypeConverter(new CustomDateTimeConverter());
            Map(m => m.InsideTemp).Index(1);
            Map(m => m.Humidity).Index(2);
            Map(m => m.OutsideTemp).Index(3);
            Map(m => m.WeatherDescription).Index(4);
        }
    }
}