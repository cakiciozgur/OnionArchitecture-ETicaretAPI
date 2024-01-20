using NpgsqlTypes;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;

namespace ETicaretAPI.API.Configurations.ColumnWriters
{
    public class UsernameColumnWriter : ColumnWriterBase
    {
        public UsernameColumnWriter() : base(NpgsqlDbType.Varchar)
        {

        }

        public override object GetValue(LogEvent logEvent, IFormatProvider formatProvider = null)
        {
            logEvent.Properties.TryGetValue("user_name", out var username); // program cs te middleware ile log contextine eklenmiş user_name adında bir propertyi var ise yakalayıp geri döndürür

            return username?.ToString() ?? null;
        }
    }
}
