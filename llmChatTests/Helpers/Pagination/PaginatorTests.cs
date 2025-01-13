using llmChat.Helpers.Pagination;
using Xunit;
using Assert = Xunit.Assert;

namespace llmChatTests.Helpers.Pagination
{
    public class PaginatorTests
    {
        private readonly Paginator _paginator;

        public PaginatorTests()
        {
            _paginator = new Paginator();
        }

        [Fact]
        public void Paginate_ShouldReturnCorrectPage_WhenPageNumberAndPageSizeAreValid()
        {
            // Arrange
            var data = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var queryPage = new QueryPage { PageNumber = 2, PageSize = 3 };
            var queryableData = data.AsQueryable();

            // Act
            var result = _paginator.Paginate(queryableData, queryPage).ToList();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal(4, result[0]);
            Assert.Equal(5, result[1]);
            Assert.Equal(6, result[2]);
        }

        [Fact]
        public void Paginate_ShouldReturnFirstPage_WhenPageNumberIsLessThanOne()
        {
            // Arrange
            var data = new List<int> { 1, 2, 3, 4, 5 };
            var queryPage = new QueryPage { PageNumber = 0, PageSize = 2 }; // Некорректный номер страницы
            var queryableData = data.AsQueryable();

            // Act
            var result = _paginator.Paginate(queryableData, queryPage).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0]);
            Assert.Equal(2, result[1]);
        }

        [Fact]
        public void Paginate_ShouldReturnEmptyList_WhenPageNumberIsOutOfRange()
        {
            // Arrange
            var data = new List<int> { 1, 2, 3, 4, 5 };
            var queryPage = new QueryPage { PageNumber = 10, PageSize = 2 }; // Страница за пределами данных
            var queryableData = data.AsQueryable();

            // Act
            var result = _paginator.Paginate(queryableData, queryPage).ToList();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Paginate_ShouldReturnAllItems_WhenPageSizeIsLargerThanDataCount()
        {
            // Arrange
            var data = new List<int> { 1, 2, 3 };
            var queryPage = new QueryPage { PageNumber = 1, PageSize = 10 }; // Размер страницы больше, чем данных
            var queryableData = data.AsQueryable();

            // Act
            var result = _paginator.Paginate(queryableData, queryPage).ToList();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal(1, result[0]);
            Assert.Equal(2, result[1]);
            Assert.Equal(3, result[2]);
        }

        [Fact]
        public void Paginate_ShouldThrowArgumentNullException_WhenQueryableIsNull()
        {
            // Arrange
            IQueryable<int> queryableData = null;
            var queryPage = new QueryPage { PageNumber = 1, PageSize = 10 };

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _paginator.Paginate(queryableData, queryPage));
            Assert.Equal("obj", exception.ParamName);
        }

        [Fact]
        public void Paginate_ShouldThrowArgumentNullException_WhenQueryPageIsNull()
        {
            // Arrange
            var data = new List<int> { 1, 2, 3 };
            var queryableData = data.AsQueryable();
            QueryPage queryPage = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _paginator.Paginate(queryableData, queryPage));
            Assert.Equal("queryPage", exception.ParamName);
        }
    }
}