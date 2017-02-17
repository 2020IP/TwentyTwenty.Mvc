namespace TwentyTwenty.Mvc.DataTables.Core
{
    public static class RequestNames
    {
        public const string 
            Draw = "draw",
            Start = "start",
            Length = "length",
            SearchValue = "search[value]",
            IsSearchRegex = "search[regex]",
            SortField = "order[{0}][column]",
            SortDirection = "order[{0}][dir]",
            ColumnField = "columns[{0}][data]",
            ColumnName = "columns[{0}][name]",
            IsColumnSearchable = "columns[{0}][searchable]",
            IsColumnSortable = "columns[{0}][orderable]",
            ColumnSearchValue = "columns[{0}][search][value]",
            IsColumnSearchRegex = "columns[{0}][search][regex]",
            SortAscending = "asc",
            SortDescending = "desc";
    }

    public static class ResponseNames
    {
        public const string 
            Draw = "draw",
            TotalRecords = "recordsTotal",
            TotalRecordsFiltered = "recordsFiltered",
            Data = "data",
            Error = "error";
    }
}