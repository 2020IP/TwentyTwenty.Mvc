using System;
using System.Linq;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Http
{
    public static class QueryCollectionExtensions
    {
        public static int? ToInt(this string param)
        {
            return int.TryParse(param, out int parsed) ? parsed : (int?)null;
        }

        public static decimal? ToDecimal(this string param)
        {
            return decimal.TryParse(param, out decimal parsed) ? parsed : (decimal?)null;
        }

        public static T? ToEnum<T>(this string param)
            where T : struct
        {
            return Enum.TryParse<T>(param, true, out T parsed) ? parsed : (T?)null;
        }

        public static bool? ToBool(this string param)
        {
            if (bool.TryParse(param, out bool parsed))
            {
                return parsed;
            }
            if (string.Equals("0", param, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            if (string.Equals("1", param, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return null;
        }

        public static bool? GetBool(this IQueryCollection query, string key)
            => query.GetValue(key)?.ToBool();

        public static int? GetInt(this IQueryCollection query, string key)
            => query.GetValue(key)?.ToInt();

        public static decimal? GetDecimal(this IQueryCollection query, string key)
            => query.GetValue(key)?.ToDecimal();

        public static decimal?[] GetDecimals(this IQueryCollection query, string key)
            => query.GetValues(key)?.SelectMany(s => s?.Split(',')).Select(ToDecimal).ToArray();

        public static T? GetEnum<T>(this IQueryCollection query, string key)
            where T : struct
            => query.GetValue(key)?.ToEnum<T>();

        public static StringValues? GetValues(this IQueryCollection query, string key)
        {
            if (!query.TryGetValue(key, out StringValues val))
            {
                return null;
            }
            return val;
        }

        public static string GetValue(this IQueryCollection query, string key)
        {
            return query.GetValues(key)?.FirstOrDefault();
        }
    }
}