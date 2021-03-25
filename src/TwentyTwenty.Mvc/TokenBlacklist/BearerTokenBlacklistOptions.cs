namespace TwentyTwenty.Mvc.TokenBlacklist
{
    public class BearerTokenBlacklistOptions
    {
        /// <summary>
        /// Sets the format string used when creating the cache key
        /// </summary>
        public string CacheKeyFormatString { get; set; } = "{0}";
    }
}