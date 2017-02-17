
namespace TwentyTwenty.Mvc.DataTables.Core
{
    /// <summary>
    /// Represents a configuration object for DataTables.AspNet.
    /// </summary>
    public class DataTablesOptions
    {
        /// <summary>
        /// Gets default page length when parameter is not set.
        /// </summary>
        public int DefaultPageLength { get; set; } = 10;
        /// <summary>
        /// Gets an indicator if draw parameter should be validated.
        /// </summary>
        public bool IsDrawValidationEnabled { get; set; } = true;
        /// <summary>
        /// Gets an indicator whether response adicional parameters parsing is enabled or not.
        /// </summary>
        public bool IsResponseAdditionalParametersEnabled { get; set; } = false;
    }
}