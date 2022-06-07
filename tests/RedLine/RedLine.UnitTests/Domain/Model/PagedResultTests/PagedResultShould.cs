using System;
using FluentAssertions;
using RedLine.Domain.Model;
using Xunit;

namespace RedLine.UnitTests.Domain.Model.PagedResultTests
{
    [Collection(nameof(UnitTestCollection))]
    public class PagedResultShould
    {
        [Fact]
        public void CalculateTotalPages()
        {
            var items = new[] { "item1", "item2", "item3" };
            var result = new PageableResult<string>(items, 1, 2, 3);
            result.TotalPages.Should().Be(2);
        }

        [Fact]
        public void SetDefaultValuesIfNotProvided()
        {
            var items = new[] { "item1", "item2", "item3" };
            var result = new PageableResult<string>(items);

            result.Page.Should().Be(1);
            result.PageSize.Should().Be(items.Length);
            result.TotalPages.Should().Be(1);
            result.TotalItems.Should().Be(items.Length);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(1, 1)]
        [InlineData(0, 0)]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(null, 1)]
        [InlineData(1, null)]
        public void ValidateCtorArguments(int? page, int? pageSize)
        {
            var items = new[] { "item1", "item2", "item3" };

            if (page == null && pageSize == null)
            {
                var result = new PageableResult<string>(items, page, pageSize, items.Length);
                result.Page.Should().Be(1);
                result.PageSize.Should().Be(items.Length);
                result.TotalItems.Should().Be(items.Length);

                result = new PageableResult<string>(items, page, pageSize, null);
                result.Page.Should().Be(1);
                result.PageSize.Should().Be(items.Length);
                result.TotalItems.Should().Be(items.Length);
            }
            else if (page > 0 && pageSize > 0)
            {
                var result = new PageableResult<string>(items, page, pageSize, items.Length);
                result.Page.Should().Be(page);
                result.PageSize.Should().Be(pageSize);
                result.TotalItems.Should().Be(items.Length);

                result = new PageableResult<string>(items, page, pageSize, null);
                result.Page.Should().Be(page);
                result.PageSize.Should().Be(pageSize);
                result.TotalItems.Should().Be(items.Length);

                Assert.Throws<ArgumentException>(() => new PageableResult<string>(items, page, pageSize, items.Length - 1));
            }
            else if (page == 0 || pageSize == 0)
            {
                Assert.Throws<ArgumentException>(() => new PageableResult<string>(items, page, pageSize, items.Length));
            }
            else if (page == null || pageSize != null)
            {
                Assert.Throws<ArgumentException>(() => new PageableResult<string>(items, page, pageSize, items.Length));
            }
            else if (page != null || pageSize == null)
            {
                Assert.Throws<ArgumentException>(() => new PageableResult<string>(items, page, pageSize, items.Length));
            }
        }
    }
}
