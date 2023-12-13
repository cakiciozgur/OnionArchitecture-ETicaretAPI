using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Entities.Common;
using ETicaretAPI.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Repositories
{
    public class ReadRepository<T> : IReadRepository<T> where T : BaseEntity
    {
        private readonly ETicaretAPIDbContext _context;

        public ReadRepository(ETicaretAPIDbContext context)
        {
            _context = context;
        }

        public DbSet<T> Table => _context.Set<T>();

        public IQueryable<T> GetAll(bool tracking = true)
        {
            var query = Table.AsQueryable();
            if(!tracking)
                query = query.AsNoTracking();

            return query;
        }

        public IQueryable<T> GetWhere(Expression<Func<T, bool>> expression, bool tracking = true)
        {
            var query = Table.AsQueryable();
            if (!tracking)
                query = query.AsNoTracking();

            return query.Where(expression);
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> expression, bool tracking = true)
        {
            var query = Table.AsQueryable();
            if (!tracking)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(expression);
        }

        public async Task<T> GetByIdAsync(string id, bool tracking = true)
        {
            var query = Table.AsQueryable();
            if (!tracking)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(x => x.Id == Guid.Parse(id));
        }

        //public async Task<T> GetByIdAsync(string id) // other choice
        //=> await Table.FindAsync(id);

        //private IQueryable<T> GetQueryTracking(DbSet<T> table, bool tracking = true) // tracking mekanizmasına karar vermek için aşağıdaki şekilde optimize edilebilir
        //{
        //    var query = table.AsQueryable();
        //    if (!tracking)
        //        query = query.AsNoTracking();

        //    return query;
        //}
    }
}
