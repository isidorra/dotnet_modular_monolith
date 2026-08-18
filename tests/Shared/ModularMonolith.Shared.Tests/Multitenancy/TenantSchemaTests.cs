using ModularMonolith.Shared.Infrastructure.Multitenancy;

namespace ModularMonolith.Shared.Tests.Multitenancy;

public sealed class TenantSchemaTests
{
    private static readonly Guid TenantId = Guid.Parse("01a01672-43e5-7de8-98fb-3597d4e38de1");

    [Fact]
    public void For_composes_the_prefix_and_the_hyphenated_tenant_id()
    {
        TenantSchema.For("auth", TenantId).ShouldBe("auth_01a01672-43e5-7de8-98fb-3597d4e38de1");
    }

    [Theory]
    [InlineData("auth")]
    [InlineData("core")]
    public void For_produces_a_name_that_is_illegal_unquoted(string prefix)
    {
        TenantSchema.For(prefix, TenantId).ShouldContain("-");
    }

    [Fact]
    public void Quote_wraps_the_identifier_in_double_quotes()
    {
        TenantSchema.Quote("auth_schema").ShouldBe("\"auth_schema\"");
    }

    [Fact]
    public void Quote_doubles_embedded_double_quotes()
    {
        TenantSchema.Quote("we\"ird").ShouldBe("\"we\"\"ird\"");
    }

    [Fact]
    public void Quote_of_a_tenant_schema_yields_a_legal_quoted_identifier()
    {
        TenantSchema.Quote(TenantSchema.For("core", TenantId))
            .ShouldBe("\"core_01a01672-43e5-7de8-98fb-3597d4e38de1\"");
    }
}