using ModularMonolith.SharedKernel.Pagination;

namespace ModularMonolith.Shared.Tests.Pagination;

public sealed class PagedResultTests
{
    [Theory]
    [InlineData(0, 20, 0)]
    [InlineData(1, 20, 1)]
    [InlineData(19, 20, 1)]
    [InlineData(20, 20, 1)]
    [InlineData(21, 20, 2)]
    [InlineData(40, 20, 2)]
    [InlineData(41, 20, 3)]
    public void TotalPages_rounds_a_partial_page_up(int totalCount, int pageSize, int expected)
    {
        Page(pageSize, totalCount).TotalPages.ShouldBe(expected);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void TotalPages_is_zero_when_the_page_size_is_not_positive(int pageSize)
    {
        Page(pageSize, 100).TotalPages.ShouldBe(0);
    }

    [Fact]
    public void Carries_the_items_and_paging_values_it_was_given()
    {
        var result = new PagedResult<string>(["a", "b"], 2, 20, 42);

        result.Items.ShouldBe(["a", "b"]);
        result.Page.ShouldBe(2);
        result.PageSize.ShouldBe(20);
        result.TotalCount.ShouldBe(42);
    }

    private static PagedResult<string> Page(int pageSize, int totalCount)
    {
        return new PagedResult<string>([], PageDefaults.Number, pageSize, totalCount);
    }
}
