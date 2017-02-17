using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TwentyTwenty.Mvc.DataTables.Core;

namespace TwentyTwenty.Mvc.DataTables
{
    public static class ControllerExtensions
    {
        /// <summary>
        /// Creates a new response instance.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <param name="totalRecords">Total record count (total records available on database).</param>
        /// <param name="totalRecordsFiltered">Filtered record count (total records available after filtering).</param>
        /// <param name="data">Data object (collection).</param>
        /// <returns>The response object.</returns>
        public static DataTablesResult DataTables(this ControllerBase controller, IDataTablesRequest request, 
            long totalRecords, long totalRecordsFiltered, object data)
            => DataTables(controller, request, totalRecords, totalRecordsFiltered, data, null);

        /// <summary>
        /// Creates a new response instance.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <param name="totalRecords">Total record count (total records available on database).</param>
        /// <param name="totalRecordsFiltered">Filtered record count (total records available after filtering).</param>
        /// <param name="data">Data object (collection).</param>
        /// <param name="additionalParameters">Aditional parameters for response.</param>
        /// <returns>The response object.</returns>
        public static DataTablesResult DataTables(this ControllerBase controller, IDataTablesRequest request, 
            long totalRecords, long totalRecordsFiltered, object data, IDictionary<string, object> additionalParameters)
        {
            // When request is null, there should be no response (null response).
            if (request == null) return null;

            var options = controller.HttpContext.RequestServices.GetRequiredService<IOptions<DataTablesOptions>>();

            if (options.Value.IsDrawValidationEnabled)
            {
                // When draw validation is in place, response must have a draw value equals to or greater than 1.
                // Any other value besides that represents an invalid draw request and response should be null.

                if (request.Draw < 1) return null;
            }

            return new DataTablesResult(request.Draw, totalRecords, totalRecordsFiltered, data, additionalParameters);
        }
        /// <summary>
        /// Creates a new response instance.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <param name="errorMessage">Error message.</param>
        /// <returns>The response object.</returns>
        public static DataTablesResult DataTables(this ControllerBase controller, IDataTablesRequest request, string errorMessage)
            => DataTables(controller, request, errorMessage, null);

        /// <summary>
        /// Creates a new response instance.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <param name="errorMessage">Error message.</param>
        /// <returns>The response object.</returns>
        public static DataTablesResult DataTables(this ControllerBase controller, IDataTablesRequest request, string errorMessage, IDictionary<string, object> additionalParameters)
        {
            // When request is null, there should be no response (null response).
            if (request == null) return null;

            var options = controller.HttpContext.RequestServices.GetRequiredService<IOptions<DataTablesOptions>>();

            if (options.Value.IsDrawValidationEnabled)
            {
                // When draw validation is in place, response must have a draw value equals to or greater than 1.
                // Any other value besides that represents an invalid draw request and response should be null.

                if (request.Draw < 1) return null;
            }

            return new DataTablesResult(request.Draw, errorMessage, additionalParameters);
        }
    }
}