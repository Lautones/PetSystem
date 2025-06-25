using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgendamentoCuidados.Models;
using Microsoft.EntityFrameworkCore;

namespace AgendamentoCuidados.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Agendamento> Agendamentos {get;set;}

        public ApplicationDbContext(DbContextOptions <ApplicationDbContext> options) : base(options){}
    }
}