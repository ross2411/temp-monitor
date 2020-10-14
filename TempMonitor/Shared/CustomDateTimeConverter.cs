using System;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace TempMonitor.Shared
{
    public class CustomDateTimeConverter : TypeConverter
    {

        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (DateTime.TryParseExact(text, "d / M / y HH:m:s", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out var dt))
            {
                return dt;
            }
            else if (DateTime.TryParseExact(text, "d/M/y HH:m:s", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out var dt2))
            {
                return dt2;
            }
            throw new Exception($"Unable to parse date: {text}");
        }

    }
}
