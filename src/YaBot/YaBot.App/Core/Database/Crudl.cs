namespace YaBot.App.Core.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using Microsoft.EntityFrameworkCore;

    internal sealed class Crudl<T> : ICrudl<int, T> where T : class, IHasId<int>
    {
        private readonly Context context;
        private readonly Func<Context, DbSet<T>> getEntities;

        public Crudl(Context context, Func<Context, DbSet<T>> getEntities)
        {
            this.context = context;
            this.getEntities = getEntities;
        }

        public int Create(T value)
        {
            var key = context
                ._(getEntities)
                .Add(value)
                .Entity
                .Id;

            context.SaveChanges();
            
            return key;
        }

        public T Read(int key)
        {
            return context
                ._(getEntities)
                .FirstOrDefault(_ => _.Id == key); // TODO : to First()
        }

        public bool Update(T value)
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
            
            return item != null;
        }

        public IEnumerable<T> Enumerate()
        {
            return context._(getEntities);
        }
    }
}