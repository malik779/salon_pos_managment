using Microsoft.EntityFrameworkCore;
using Salon.BuildingBlocks.Abstractions;

namespace Salon.BuildingBlocks.Data;

public abstract class ServiceDbContextBase : DbContext, IUnitOfWork
{
    protected ServiceDbContextBase(DbContextOptions options) : base(options)
    {
    }
}
