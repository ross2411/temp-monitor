using System;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;
using Microsoft.Extensions.Logging;

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
    }

    public  class CustomDateTimeConverter: TypeConverter
    {
        //private readonly ILogger<CustomDateTimeConverter> _logger;
        //public CustomDateTimeConverter(
        //    ILogger<CustomDateTimeConverter> logger)
        //{
        //    _logger = logger;
        //}

        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            //_logger.LogInformation("About to parse{0)", text);
            if (DateTime.TryParseExact(text, "d / M / y HH:m:s", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out var dt))
            {
                return dt;
            }
            else if (DateTime.TryParseExact(text, "d/M/y HH:m:s", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out var dt2))
            {
                return dt2;
            }
            //DateTime.Parse(text, )
            ///"Unable to parse date: 11/10/20 13:00:10"
            throw new Exception($"Unable to parse date: {text}");
            //var dt = DateTime.ParseExact(text, "d/M/y h:m:s", CultureInfo.InvariantCulture);
            //return dt;
            //return base.ConvertFromString(text, row, memberMapData);
        }

    }

    public class Period
    {
        public string Name { get; set; }
        public int Value { get; set; }  
    }
}
