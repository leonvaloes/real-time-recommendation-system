namespace CleanArch.Application.Mappings;

public interface IMapping<TEntity, TDto>
{
    TDto ToDto(TEntity entity);
    TEntity ToEntity(TDto dto);
}
