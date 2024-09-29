using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyMovie.Models;
using System.Net.Sockets;

namespace MyMovie.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        // movie.cs will map to a movie table in database
        public DbSet<Movie> Movies { get; set; }

        // viewer.ca will map to a customer table in database
        public DbSet<Viewer> Viewers { get; set; }

        // ticket.ca will map to a customer table in database
        public DbSet<Ticket> Tickets { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
