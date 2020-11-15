﻿using Microsoft.EntityFrameworkCore;

namespace Gss.Infrastructure
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }
    }
}
