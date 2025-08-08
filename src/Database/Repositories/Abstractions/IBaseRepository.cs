using System.Linq.Expressions;
using Achilles.Database.Models;

namespace Achilles.Database.Repositories.Abstractions;

public interface IBaseRepository<TModelType, TPrimaryKeyType>
    where TModelType : BaseModel<TPrimaryKeyType>
{
    Task<TModelType?> FindAsync(TPrimaryKeyType id);
    Task<TModelType?> FindAsync(Expression<Func<TModelType, bool>> predicate);

    Task<List<TModelType>> RetrieveAsync(Expression<Func<TModelType, bool>> predicate);

    Task<bool> CreateAsync(TModelType model);
    Task<bool> UpdateAsync(TModelType model);
    Task<bool> DeleteAsync(TModelType model);
    Task<bool> DeleteAsync(TModelType[] models);
}   