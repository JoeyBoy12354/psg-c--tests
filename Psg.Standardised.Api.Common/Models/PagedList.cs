namespace Psg.Standardised.Api.Common.Models
{
    public class PagedList<T> : List<T>
    {
        public PagingData PagingData { get; private set; }

        internal PagedList(IEnumerable<T> items, PagingData pagingData) : base(pagingData.PageSize)
        {
            AddRange(items);
            PagingData = pagingData;
        }
    }
}
