using Desafio_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Desafio_API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Pet> Pets {get;set;}

        public ApplicationDbContext(DbContextOptions <ApplicationDbContext> options) : base(options){}
    }
}