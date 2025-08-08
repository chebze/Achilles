using System.Linq.Expressions;
using Achilles.Database.Abstractions;
using Achilles.Database.Models;
using Achilles.Database.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Achilles.Database.Repositories;

public abstract class InMemoryBaseRepository<TModelType, TPrimaryKeyType> : 
    IBaseRepository<TModelType, TPrimaryKeyType> where TModelType : BaseModel<TPrimaryKeyType>
{
    protected readonly HabboDbContext _context;
    
    private readonly List<TModelType> _cache = new();

    public InMemoryBaseRepository(HabboDbContext context)
    {
        this._context = context;
    }

    private async Task<List<TModelType>> GetFromDatabase(Expression<Func<TModelType, bool>> predicate)
    {
        List<TModelType> results = [];   

        List<TModelType> resultsInMemory = [];
        lock(this._cache)
        {
            resultsInMemory = this._cache.AsQueryable().Where(predicate).ToList();
        }
        results.AddRange(resultsInMemory);

        TPrimaryKeyType[] idsInMemory = resultsInMemory.Select(t => t.Id).ToArray();
        bool resultsNotInMemory = await this._context.Set<TModelType>()
            .Where(t => !idsInMemory.Contains(t.Id))
            .Where(predicate)
        .AnyAsync();

        if(resultsNotInMemory)
        {
            List<TModelType> resultsFromDatabase = await this._context.Set<TModelType>()
                .Where(t => !idsInMemory.Contains(t.Id))
                .Where(predicate)
            .ToListAsync();
            results.AddRange(resultsFromDatabase);

            lock(this._cache)
            {
                this._cache.AddRange(resultsFromDatabase);
            }
        }
        
        return results;
    }

    public async Task<TModelType?> FindAsync(TPrimaryKeyType id)
        => (await this.GetFromDatabase(t => t.Id != null && t.Id.Equals(id))).FirstOrDefault();

    public async Task<TModelType?> FindAsync(Expression<Func<TModelType, bool>> predicate)
        => (await this.GetFromDatabase(predicate)).FirstOrDefault();
    
    public async Task<List<TModelType>> RetrieveAsync(Expression<Func<TModelType, bool>> predicate)
        => await this.GetFromDatabase(predicate);
    
    public async Task<bool> CreateAsync(TModelType model)
    {
        await this._context.Set<TModelType>().AddAsync(model);
        lock(this._cache)
        {
            this._cache.Add(model);
        }
        return await this._context.SaveChangesAsync() > 0;
    }
    public async Task<bool> UpdateAsync(TModelType model)
    {
        this._context.Set<TModelType>().Update(model);
        return await this._context.SaveChangesAsync() > 0;
    }
    public async Task<bool> DeleteAsync(TModelType model)
    {
        this._context.Set<TModelType>().Remove(model);
        lock(this._cache)
        {
            this._cache.Remove(model);
        }
        return await this._context.SaveChangesAsync() > 0;
    }
    public async Task<bool> DeleteAsync(TModelType[] models)
    {
        this._context.Set<TModelType>().RemoveRange(models);
        lock(this._cache)
        {
            this._cache.RemoveAll(m => models.Contains(m));
        }
        return await this._context.SaveChangesAsync() > 0;
    }
}
