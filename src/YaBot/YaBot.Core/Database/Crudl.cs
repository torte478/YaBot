namespace YaBot.Core.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using YaBot.Core.Extensions;

    public sealed class Crudl<TContext, TEntity> : ICrudl<int, TEntity>
        where TContext : DbContext
        where TEntity : class, IHasId<int>
    {
        private readonly TContext context;
        private readonly Func<TContext, DbSet<TEntity>> getEntities;

        public Crudl(TContext context, Func<TContext, DbSet<TEntity>> getEntities)
        {
            this.context = context;
            this.getEntities = getEntities;
        }

        public int Create(TEntity value)
        {
            context._(getEntities).Add(value);

            context.SaveChanges();
            
            return value.Id;
        }

        public TEntity Read(int key)
        {
            return context
                ._(getEntities)
                .FirstOrDefault(_ => _.Id == key);
        }

        public bool Update(TEntity value)
        {
            using var transaction = context.Database.BeginTransaction();

            var exists = Delete(value.Id);
            context
                ._(getEntities)
                .Add(value)
                ._(_ => context.SaveChanges());
            
            transaction.Commit();

            return exists;
        }

        public bool Delete(int key)
        {
            var item = Read(key);

            context
                ._(getEntities)
                .Remove(item);

            context.SaveChanges();
            
            return item != null;
        }

        public IEnumerable<TEntity> Enumerate()
        {
            return context._(getEntities);
        }
    }
}