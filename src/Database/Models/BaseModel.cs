namespace Achilles.Database.Models;

public class BaseModel<TPrimaryKeyType>
{
    public TPrimaryKeyType Id { get; set; } = default!;
}
