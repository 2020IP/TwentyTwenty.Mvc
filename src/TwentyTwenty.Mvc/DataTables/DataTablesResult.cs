using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TwentyTwenty.Mvc.DataTables.Core;

namespace TwentyTwenty.Mvc.DataTables
{
    /// <summary>
    /// Represents a response for DataTables.
    /// </summary>
    public class DataTablesResult : ActionResult, IDataTablesResult
    {
        /// <summary>
        /// Gets draw count for validation and async ordering.
        /// </summary>
        public int Draw { get; protected set; }
        /// <summary>
        /// Gets error message, if not successful.
        /// Should only be available for DataTables 1.10 and above.
        /// </summary>
        public string Error { get; protected set; }
        /// <summary>
        /// Gets total record count (total records available on database).
        /// </summary>
        public long TotalRecords { get; protected set; }
        /// <summary>
        /// Gets filtered record count (total records available after filtering).
        /// </summary>
        public long TotalRecordsFiltered { get; protected set; }
        /// <summary>
        /// Gets data object (collection).
        /// </summary>
        public object Data { get; protected set; }
        /// <summary>
        /// Gets aditional parameters for response.
        /// </summary>
        public IDictionary<string, object> AdditionalParameters { get; protected set; }

        private JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        
        public override async Task ExecuteResultAsync(ActionContext context)
        {
            var options = context.HttpContext.RequestServices.GetRequiredService<IOptions<DataTablesOptions>>();

            var response = context.HttpContext.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.OK;

            await using (var jsonWriter = new Utf8JsonWriter(context.HttpContext.Response.BodyWriter))
            {
                // Start json object.
                jsonWriter.WriteStartObject();

                // Draw
                jsonWriter.WriteNumber(ResponseNames.Draw, Draw);

                if (IsSuccessResponse())
                {
                    // TotalRecords
                    jsonWriter.WriteNumber(ResponseNames.TotalRecords, TotalRecords);

                    // TotalRecordsFiltered
                    jsonWriter.WriteNumber(ResponseNames.TotalRecordsFiltered, TotalRecordsFiltered);

                    // Data
                    jsonWriter.WritePropertyName(ResponseNames.Data);
                    JsonSerializer.Serialize(jsonWriter, Data, _jsonOptions);
                }
                else
                {
                    // Error
                    jsonWriter.WriteString(ResponseNames.Error, Error);
                }

                // AdditionalParameters
                if (options.Value.IsResponseAdditionalParametersEnabled && AdditionalParameters != null)
                {
                    foreach(var keypair in AdditionalParameters)
                    {
                        jsonWriter.WritePropertyName(keypair.Key);
                        JsonSerializer.Serialize(jsonWriter, keypair.Value, _jsonOptions);
                    }
                }

                // End json object
                jsonWriter.WriteEndObject();

                await jsonWriter.FlushAsync();
            }
        }

        public override void ExecuteResult(ActionContext context)
        {
            var options = context.HttpContext.RequestServices.GetRequiredService<IOptions<DataTablesOptions>>();

            var response = context.HttpContext.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.OK;

            using (var jsonWriter = new Utf8JsonWriter(context.HttpContext.Response.BodyWriter))
            {
                // Start json object.
                jsonWriter.WriteStartObject();

                // Draw
                jsonWriter.WriteNumber(ResponseNames.Draw, Draw);

                if (IsSuccessResponse())
                {
                    // TotalRecords
                    jsonWriter.WriteNumber(ResponseNames.TotalRecords, TotalRecords);

                    // TotalRecordsFiltered
                    jsonWriter.WriteNumber(ResponseNames.TotalRecordsFiltered, TotalRecordsFiltered);

                    // Data
                    jsonWriter.WritePropertyName(ResponseNames.Data);
                    JsonSerializer.Serialize(jsonWriter, Data, _jsonOptions);
                }
                else
                {
                    // Error
                    jsonWriter.WriteString(ResponseNames.Error, Error);
                }

                // AdditionalParameters
                if (options.Value.IsResponseAdditionalParametersEnabled && AdditionalParameters != null)
                {
                    foreach(var keypair in AdditionalParameters)
                    {
                        jsonWriter.WritePropertyName(keypair.Key);
                        JsonSerializer.Serialize(jsonWriter, keypair.Value, _jsonOptions);
                    }
                }

                // End json object
                jsonWriter.WriteEndObject();

                jsonWriter.Flush();
            }
        }        
        
        /// <summary>
        /// For private use only.
        /// Gets an indicator if this is a success response or an error response.
        /// </summary>
        /// <returns>True if it's a success response, False if it's an error response.</returns>
        private bool IsSuccessResponse()
        {
            return Data != null && string.IsNullOrWhiteSpace(Error);
        }

        /// <summary>
        /// For internal use only.
        /// Creates a new response instance.
        /// </summary>
        /// <param name="draw">Draw count from request object.</param>
        /// <param name="errorMessage">Error message.</param>
        public DataTablesResult(int draw, string errorMessage)
            : this(draw, errorMessage, null)
        { }
        /// <summary>
        /// For internal use only.
        /// Creates a new response instance.
        /// </summary>
        /// <param name="draw">Draw count from request object.</param>
        /// <param name="errorMessage">Error message.</param>
        public DataTablesResult(int draw, string errorMessage, IDictionary<string, object> additionalParameters)
        {
            Draw = draw;
            Error = errorMessage;
            AdditionalParameters = additionalParameters;
        }
        /// <summary>
        /// For internal use only.
        /// Creates a new response instance.
        /// </summary>
        /// <param name="draw">Draw count from request object.</param>
        /// <param name="totalRecords">Total record count (total records available on database).</param>
        /// <param name="totalRecordsFiltered">Filtered record count (total records available after filtering).</param>
        /// <param name="data">Data object (collection).</param>
        public DataTablesResult(int draw, long totalRecords, long totalRecordsFiltered, object data)
            : this(draw, totalRecords, totalRecordsFiltered, data, null)
        { }
        /// <summary>
        /// For internal use only.
        /// Creates a new response instance.
        /// </summary>
        /// <param name="draw">Draw count from request object.</param>
        /// <param name="totalRecords">Total record count (total records available on database).</param>
        /// <param name="totalRecordsFiltered">Filtered record count (total records available after filtering).</param>
        /// <param name="additionalParameters">Aditional parameters for response.</param>
        /// <param name="data">Data object (collection).</param>
        public DataTablesResult(int draw, long totalRecords, long totalRecordsFiltered, object data, IDictionary<string, object> additionalParameters)
        {
            Draw = draw;
            TotalRecords = totalRecords;
            TotalRecordsFiltered = totalRecordsFiltered;
            Data = data;

            AdditionalParameters = additionalParameters;
        }
    }
}