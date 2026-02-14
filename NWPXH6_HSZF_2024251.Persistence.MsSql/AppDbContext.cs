using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NWPXH6_HSZF_2024251.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWPXH6_HSZF_2024251.Persistence.MsSql
{
    public class AppDbContext : DbContext
    {
        public DbSet<Person> PersonsDb { get; set; }
        public DbSet<Payment> PaymentsDb { get; set; }

        public AppDbContext()
        {
            this.Database.EnsureDeleted();
            this.Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connStr = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=KorsoavatoNyilvantartasDB;Integrated Security=True;MultipleActiveResultSets=true";
            optionsBuilder.UseSqlServer(connStr);
            base.OnConfiguring(optionsBuilder);

            //Server=localhost\SQLEXPRESS;Database=master;Trusted_Connection=True;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Egy a több kapcsolat
            modelBuilder.Entity<Person>()
                        .HasMany(p => p.Payment)            // Egy személynek több fizetése lehet
                        .WithOne(p => p.Person)             // Minden fizetés egy személyhez tartozik
                        .HasForeignKey(p => p.Person_Id)    // A Payment osztályban a Person_Id az idegen kulcs
                        .OnDelete(DeleteBehavior.Cascade);  // Törlés esetén a fizetések is törlődnek

            /*

            // Egy az egy-hez kapcsolat esetén
            modelBuilder.Entity<Person>()
                .HasOne(e => e.Payment)                     // Egy Person-hoz egy Payment tartozik
                .WithOne(e => e.Person)                     // Egy Payment-hez egy Person tartozik
                .HasForeignKey<Payment>(e => e.Person_Id)   // A Person_Id az idegen kulcs
                .OnDelete(DeleteBehavior.Cascade);          // People törlés esetén a hozzá tartozó Payment is törlődik
            
             De mivel a person osztályban a payment ICollection<Payment> ezért 1 az 1 hez kapcsolat nem hozható létre.
             Mert ha a ICollection<Payment> típusúnak van definiálva, akkor csak egy a többhöz vagy több a több-höz kapcsolatra használható gyűjtemény típus.

             Egy az egy-hez kapcsolat esetén
             a Person pszályban public virtual Payment Payment { get; set; } kellene használni de akkor a json filet kell módosítani mert a beolvasás sikertelen lesz...
             
             */


            // Egy az egy-hez kapcsolat beállítása
            /*
            modelBuilder.Entity<Person>()
                        .HasOne(p => p.Payment)
                        .WithOne(p => p.Person)
                        .HasForeignKey<Payment>(p => p.Person_Id)
                        .OnDelete(DeleteBehavior.Cascade);

            */
            base.OnModelCreating(modelBuilder);
        }

        
    }
}
