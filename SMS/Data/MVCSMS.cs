#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SMS.Models;

    public class MVCSMS : DbContext
    {
        public MVCSMS (DbContextOptions<MVCSMS> options)
            : base(options)
        {
        }

        public DbSet<SMS.Models.Person> Person { get; set; }

        public DbSet<SMS.Models.Seminar> Seminar { get; set; }

        public DbSet<SMS.Models.Admin> Admin { get; set; }

        public DbSet<SMS.Models.Organizer> Organizer { get; set; }

        public DbSet<SMS.Models.Registration> Registration { get; set; }
    }
