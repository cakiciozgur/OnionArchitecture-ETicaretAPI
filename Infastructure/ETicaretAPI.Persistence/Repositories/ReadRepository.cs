﻿using ETicaretAPI.Application.Repositories;
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

        public IQueryable<T> GetAll()
        => Table;

        public IQueryable<T> GetWhere(Expression<Func<T, bool>> expression)
        => Table.Where(expression);

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> expression)
        => await Table.FirstOrDefaultAsync(expression);

        public async Task<T> GetByIdAsync(string id)
        => await Table.FirstOrDefaultAsync(x => x.Id == Guid.Parse(id));

        //public async Task<T> GetByIdAsync(string id) // other choice
        //=> await Table.FindAsync(id);
    }
}
