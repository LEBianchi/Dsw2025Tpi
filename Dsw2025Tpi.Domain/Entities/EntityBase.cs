namespace Dsw2025Tpi.Domain.Entities;

public abstract class EntityBase
{
    protected EntityBase()
    {
        Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }
}
