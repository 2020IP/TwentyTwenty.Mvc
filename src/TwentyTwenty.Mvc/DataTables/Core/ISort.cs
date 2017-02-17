namespace TwentyTwenty.Mvc.DataTables.Core
{
    public interface ISort
    {
        /// <summary>
        /// Indicates the sort order for composed (multi) sorting.
        /// </summary>
        int Order { get; }
        /// <summary>
        /// Ordering direction for this column.
        /// It will be 'Ascending' or 'Descending' to indicate ordering direction.
        /// </summary>
        SortDirection Direction { get; }
    }

    /// <summary>
    /// Defines directions for column sorting.
    /// </summary>
    public enum SortDirection
    {
        /// <summary>
        /// Represents an ascendant sorting (default).
        /// </summary>
        Ascending = 0,
        /// <summary>
        /// Represents a descendant sorting.
        /// </summary>
        Descending = 1
    }
}