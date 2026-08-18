namespace ModularMonolith.Shared.Infrastructure.Multitenancy;

public sealed record TenantSchemaDescriptor(string SchemaPrefix, Type ContextType);
