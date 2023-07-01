namespace Domain.Exceptions;

public class EntityNotFoundException<TEntity> :Exception where TEntity : class
{
    public EntityNotFoundException(Guid id)
        : base($"Entity of type {typeof(TEntity).Name} with ID '{id}' was not found.")
    {
    }
}
