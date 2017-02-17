using System;
using Microsoft.AspNetCore.Http;

namespace TwentyTwenty.BaseLine
{
    public static class HttpRequestExtensions
    {
        public static int GetPage(this HttpRequest request)
        {
            var page = request.Query.GetInt("page");
            return page.HasValue ? page.Value : 1;
        }

        public static int GetPageSize(this HttpRequest request)
        {
            var perPage = request.Query.GetInt("pageSize");
            return perPage.HasValue ? perPage.Value : 100;
        }

        public static SortSpec GetSortSpec(this HttpRequest request)
        {
            var sortSeg = request.Query.GetValue("sort")
                ?.Split('-', ' ');

            if (sortSeg != null && sortSeg.Length > 0)
            {
                var sortDirection = ListSortDirection.Ascending;

                if (sortSeg.Length > 1 && string.Equals(sortSeg[1], "desc", StringComparison.OrdinalIgnoreCase))
                {
                    sortDirection = ListSortDirection.Descending;
                }

                return new SortSpec(sortSeg[0], sortDirection);
            }
            return null;
        }
    }
}