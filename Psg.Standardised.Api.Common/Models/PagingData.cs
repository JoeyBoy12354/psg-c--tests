namespace Psg.Standardised.Api.Common.Models
{
    public class PagingData
    {
        internal PagingData(int totalCount, int currentPage, int pageSize, int totalPages, bool hasPrevious, bool hasNext)
        {
            TotalCount = totalCount;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            HasPrevious = hasPrevious;
            HasNext = hasNext;
        }

        public int TotalCount { get; private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public bool HasPrevious { get; private set; }
        public bool HasNext { get; private set; }
    }
}
