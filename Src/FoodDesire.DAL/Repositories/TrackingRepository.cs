﻿namespace FoodDesire.DAL.Repositories;
public class TrackingRepository<T> : ITrackingRepository<T> where T : TrackedEntity {
    private readonly ApplicationDbContext _context;
    private DbSet<T> entitySet => _context.Set<T>();

    public TrackingRepository(ApplicationDbContext context) {
        _context = context;
    }

    public async Task<T> Add(T entity) {
        EntityEntry<T> newEntity = await entitySet.AddAsync(entity);
        return newEntity.Entity;
    }

    public async Task<List<T>> AddAll(List<T> entities) {
        await entitySet.AddRangeAsync(entities);
        return entities.ToList();
    }

    public async Task<T> GetByID(int? id) {
        T? entity = await entitySet.SingleOrDefaultAsync(e => !e.Deleted && e.Id == id);
        return entity!;
    }

    public async Task<List<T>> GetAll() {
        List<T>? entities = await entitySet.AsNoTracking().Where(e => !e.Deleted).ToListAsync();
        return entities;
    }

    public Task SaveChanges() {
        return _context.SaveChangesAsync();
    }

    public Task<T> Update(T entity) {
        _context.Entry(entity).State = EntityState.Detached;
        var updatedEntity = entitySet.Update(entity);
        return Task.FromResult(updatedEntity.Entity);
    }
    public async Task<bool> SoftDelete(int Id) {
        T? entity = await GetByID(Id);
        if (entity == null) return false;

        entity.Deleted = true;
        T? updatedEntity = await Update(entity);
        return updatedEntity.Deleted;
    }

    public async Task<bool> Delete(int Id) {
        EntityEntry<T>? entityDeleted = entitySet.Remove(await GetByID(Id));
        return true;
    }

    public async Task<IDbContextTransaction> BeginTransaction() {
        return await _context.Database.BeginTransactionAsync();
    }

    public async Task<T> GetOne(
        Func<IQueryable<T>, IQueryable<T>> filter,
        Func<IQueryable<T>, IIncludableQueryable<T, object?>>? include = null
        ) {
        Expression<Func<T, bool>> TrackedFilter = e => !e.Deleted;

        IQueryable<T>? query = entitySet.AsNoTracking().Where(TrackedFilter);
        query = filter(query);
        if (include != null) query = include(query);

        T? entity = await query.SingleOrDefaultAsync();
        return entity!;
    }

    public async Task<List<T>> Get(
        Func<IQueryable<T>, IQueryable<T>>? filter,
        Func<IQueryable<T>, IOrderedQueryable<T>>? order = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object?>>? include = null
        ) {
        Expression<Func<T, bool>> TrackedFilter = e => !e.Deleted;

        IQueryable<T>? query = entitySet.AsNoTracking().Where(TrackedFilter);
        query = include?.Invoke(query) ?? query;
        query = filter?.Invoke(query) ?? query;
        query = order?.Invoke(query) ?? query;

        List<T> entities = await query.ToListAsync();
        return entities;
    }
}
