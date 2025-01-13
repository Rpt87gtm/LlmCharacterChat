namespace llmChat.Helpers.Pagination
{
    public class Paginator
    {
        public Paginator() { }

        public IQueryable<T> Paginate<T>(IQueryable<T> obj, QueryPage queryPage)
        {
            if (obj == null) { throw new ArgumentNullException(nameof(obj)); }
            if (queryPage == null) { throw new ArgumentNullException(nameof(queryPage)); }
            var updatedObj = obj;
            int skipNumber = (queryPage.PageNumber - 1) * queryPage.PageSize;
            return updatedObj.Skip(skipNumber).Take(queryPage.PageSize);
        }
    }
}
