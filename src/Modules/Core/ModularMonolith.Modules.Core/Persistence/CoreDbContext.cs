using Microsoft.EntityFrameworkCore;

namespace ModularMonolith.Modules.Core.Persistence;

public sealed class CoreDbContext(DbContextOptions<CoreDbContext> options) : DbContext(options);
