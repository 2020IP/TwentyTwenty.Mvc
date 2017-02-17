using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TwentyTwenty.Mvc.DataTables.Core;

namespace TwentyTwenty.Mvc.DataTables
{
    /// <summary>
    /// Represents a model binder for DataTables request element.
    /// </summary>
    public class ModelBinder : IModelBinder
    {
        /// <summary>
        /// Binds request data/parameters/values into a 'IDataTablesRequest' element.
        /// </summary>
        /// <param name="bindingContext">Binding context for data/parameters/values.</param>
        /// <returns>An IDataTablesRequest object or null if binding was not possible.</returns>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            BindModel(bindingContext, ParseAdditionalParameters);
            return Task.CompletedTask;
        }
        /// <summary>
        /// For internal and testing use only.
        /// Binds request data/parameters/values into a 'IDataTablesRequest' element.
        /// </summary>
        /// <param name="controllerContext">Controller context for execution.</param>
        /// <param name="bindingContext">Binding context for data/parameters/values.</param>
        /// <param name="options">DataTables.AspNet global options.</param>
        /// <returns>An IDataTablesRequest object or null if binding was not possible.</returns>
        public virtual void BindModel(ModelBindingContext bindingContext, Func<ModelBindingContext, IDictionary<string, object>> parseAditionalParameters)
        {
            // Model binding is not set, thus AspNet5 will keep looking for other model binders.
            if (bindingContext.ModelType != typeof(IDataTablesRequest))
            {
                return;
            }

            var options = bindingContext.HttpContext.RequestServices.GetRequiredService<IOptions<DataTablesOptions>>().Value;
            var values = bindingContext.ValueProvider;

            // Accordingly to DataTables docs, it is recommended to receive/return draw casted as int for security reasons.
            // This is meant to help prevent XSS attacks.
            var drawVal = values.GetValue(RequestNames.Draw);
            int draw = 0;
            if (options.IsDrawValidationEnabled && !TryParse<int>(drawVal, out draw))
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }

            var startVal = values.GetValue(RequestNames.Start);
            int start = Parse<int>(startVal);

            var lengthVal = values.GetValue(RequestNames.Length);
            int length;
            if (!TryParse<int>(lengthVal, out length))
            {
                length = options.DefaultPageLength;
            }

            var searchVal = values.GetValue(RequestNames.SearchValue);
            string searchValue = Parse<string>(searchVal);

            var searchRegexVal = values.GetValue(RequestNames.IsSearchRegex);
            bool searchRegex = Parse<bool>(searchRegexVal);

            var search = new Search(searchValue, searchRegex);

            // Parse columns & column sorting.
            var columns = ParseColumns(values).ToList();
            var sorting = ParseSorting(columns, values).ToList();

            if (parseAditionalParameters != null)
            {
                var aditionalParameters = parseAditionalParameters(bindingContext);
                var model = new DataTablesRequest(draw, start, length, search, columns, aditionalParameters);
                {
                    bindingContext.Result = ModelBindingResult.Success(model);
                    return;
                }
            }
            else
            {
                var model = new DataTablesRequest(draw, start, length, search, columns);
                {
                    bindingContext.Result = ModelBindingResult.Success(model);
                    return;
                }
            }
        }

        /// <summary>
        /// Provides custom aditional parameters processing for your request.
        /// You have to implement this to populate 'IDataTablesRequest' object with aditional (user-defined) request values.
        /// </summary>
        public Func<ModelBindingContext, IDictionary<string, object>> ParseAdditionalParameters;

        /// <summary>
        /// For internal use only.
        /// Parse column collection.
        /// </summary>
        /// <param name="values">Request parameters.</param>
        /// <returns></returns>
        private static IEnumerable<IColumn> ParseColumns(IValueProvider values)
        {
            for (int i = 0;; i++)
            {
                // Parses Field value.
                var columnFieldVal = values.GetValue(string.Format(RequestNames.ColumnField, i));
                string columnField;
                if (!TryParse<string>(columnFieldVal, out columnField))
                {
                    break;
                }

                // Parses Name value.
                var columnNameVal = values.GetValue(string.Format(RequestNames.ColumnName, i));
                string columnName = Parse<string>(columnNameVal);

                // Parses Orderable value.
                var columnSortableVal = values.GetValue(string.Format(RequestNames.IsColumnSortable, i));
                bool columnSortable = Parse<bool>(columnSortableVal);

                // Parses Searchable value.
                var columnSearchableVal = values.GetValue(string.Format(RequestNames.IsColumnSearchable, i));
                bool columnSearchable = Parse<bool>(columnSearchableVal);

                // Parsed Search value.
                var columnSearchVal = values.GetValue(string.Format(RequestNames.ColumnSearchValue, i));
                string columnSearch = Parse<string>(columnSearchVal);

                // Parses IsRegex value.
                var columnSearchRegexVal = values.GetValue(string.Format(RequestNames.IsColumnSearchRegex, i));
                bool columnSearchRegex = Parse<bool>(columnSearchRegexVal);

                var search = new Search(columnSearch, columnSearchRegex);

                // Yield a new column with parsed elements.
                yield return new Column(columnName, columnField, columnSearchable, columnSortable, search);
            }
        }

        /// <summary>
        /// For internal use only.
        /// Parse sort collection.
        /// </summary>
        /// <param name="columns">Column collection to use when parsing sort.</param>
        /// <param name="values">Request parameters.</param>
        /// <returns></returns>
        private static IEnumerable<ISort> ParseSorting(IList<IColumn> columns, IValueProvider values)
        {
            for (int i = 0; i < columns.Count; i++)
            {
                var sortFieldVal = values.GetValue(string.Format(RequestNames.SortField, i));
                int sortField;
                if (!TryParse<int>(sortFieldVal, out sortField))
                {
                    break;
                }

                var column = columns[sortField];

                var sortDirectionVal = values.GetValue(string.Format(RequestNames.SortDirection, i));
                string sortDirection = Parse<string>(sortDirectionVal);

                if (column.SetSort(i, sortDirection))
                {
                    yield return column.Sort;
                }
            }
        }

        /// <summary>
        /// Parses a possible raw value and transforms into a strongly-typed result.
        /// </summary>
        /// <typeparam name="T">The expected type for result.</typeparam>
        /// <param name="value">The possible request value.</param>
        /// <param name="result">Returns the parsing result or default value for type is parsing failed.</param>
        /// <returns>True if parsing succeeded, False otherwise.</returns>
        private static bool TryParse<T>(ValueProviderResult value, out T result)
        {
            result = default(T);

            if (value == null) return false;
            if (string.IsNullOrWhiteSpace(value.FirstValue)) return false;

            try
            {
                result = (T)Convert.ChangeType(value.FirstValue, typeof(T));
                return true;
            }
            catch { return false; }
        }

        private static T Parse<T>(ValueProviderResult value)
        {
            T val;
            TryParse<T>(value, out val);
            return val;
        }
    }
}