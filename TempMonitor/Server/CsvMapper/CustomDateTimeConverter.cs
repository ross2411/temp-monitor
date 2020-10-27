using System;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace TempMonitor.Server.CsvMapper
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

        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return value is DateTime time ? time.ToString("dd/MM/yy HH:mm:ss") : base.ConvertToString(value, row, memberMapData);
        }
    }
}
