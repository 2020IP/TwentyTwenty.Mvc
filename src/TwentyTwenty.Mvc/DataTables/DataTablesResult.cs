using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
        
        public override async Task ExecuteResultAsync(ActionContext context)
        {
            var options = context.HttpContext.RequestServices.GetRequiredService<IOptions<DataTablesOptions>>();

            var response = context.HttpContext.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.OK;

            using (var writer = new HttpResponseStreamWriter(response.Body, Encoding.UTF8))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                jsonWriter.CloseOutput = false;

                // Start json object.
                await jsonWriter.WriteStartObjectAsync();

                // Draw
                await jsonWriter.WritePropertyNameAsync(ResponseNames.Draw, true);
                await jsonWriter.WriteValueAsync(Draw);

                if (IsSuccessResponse())
                {
                    // TotalRecords
                    await jsonWriter.WritePropertyNameAsync(ResponseNames.TotalRecords, true);
                    await jsonWriter.WriteValueAsync(TotalRecords);

                    // TotalRecordsFiltered
                    await jsonWriter.WritePropertyNameAsync(ResponseNames.TotalRecordsFiltered, true);
                    await jsonWriter.WriteValueAsync(TotalRecordsFiltered);

                    // Data
                    await jsonWriter.WritePropertyNameAsync(ResponseNames.Data, true);
                    SerializeData(jsonWriter, Data);
                }
                else
                {
                    // Error
                    await jsonWriter.WritePropertyNameAsync(ResponseNames.Error, true);
                    await jsonWriter.WriteValueAsync(Error);
                }

                // AdditionalParameters
                if (options.Value.IsResponseAdditionalParametersEnabled && AdditionalParameters != null)
                {
                    foreach(var keypair in AdditionalParameters)
                    {
                        await jsonWriter.WritePropertyNameAsync(keypair.Key, true);
                        await jsonWriter.WriteValueAsync(keypair.Value);
                    }
                }

                // End json object
                await jsonWriter.WriteEndObjectAsync();

                await jsonWriter.FlushAsync();
            }
        }

        public override void ExecuteResult(ActionContext context)
        {
            var options = context.HttpContext.RequestServices.GetRequiredService<IOptions<DataTablesOptions>>();

            var response = context.HttpContext.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.OK;

            using (var writer = new HttpResponseStreamWriter(response.Body, Encoding.UTF8))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                jsonWriter.CloseOutput = false;

                // Start json object.
                jsonWriter.WriteStartObject();

                // Draw
                jsonWriter.WritePropertyName(ResponseNames.Draw, true);
                jsonWriter.WriteValue(Draw);

                if (IsSuccessResponse())
                {
                    // TotalRecords
                    jsonWriter.WritePropertyName(ResponseNames.TotalRecords, true);
                    jsonWriter.WriteValue(TotalRecords);

                    // TotalRecordsFiltered
                    jsonWriter.WritePropertyName(ResponseNames.TotalRecordsFiltered, true);
                    jsonWriter.WriteValue(TotalRecordsFiltered);

                    // Data
                    jsonWriter.WritePropertyName(ResponseNames.Data, true);
                    SerializeData(jsonWriter, Data);
                }
                else
                {
                    // Error
                    jsonWriter.WritePropertyName(ResponseNames.Error, true);
                    jsonWriter.WriteValue(Error);
                }

                // AdditionalParameters
                if (options.Value.IsResponseAdditionalParametersEnabled && AdditionalParameters != null)
                {
                    foreach(var keypair in AdditionalParameters)
                    {
                        jsonWriter.WritePropertyName(keypair.Key, true);
                        jsonWriter.WriteValue(keypair.Value);
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
        /// Transforms a data object into a json element using Json.Net library.
        /// Can be overriten when needed.
        /// 
        /// Data will be serialized with camelCase convention by default, since it's a JavaScript standard.
        /// This should not interfere with DataTables' CamelCase X HungarianNotation issue.
        /// </summary>
        /// <param name="data">Data object to be transformed to json.</param>
        /// <returns>A json representation of your data.</returns>
        public virtual void SerializeData(JsonTextWriter writer, object data)
        {
            var settings = new JsonSerializerSettings() 
            { 
                ContractResolver = new CamelCasePropertyNamesContractResolver() 
            };

            var serializer = JsonSerializer.Create(settings);
            
            serializer.Serialize(writer, data);
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