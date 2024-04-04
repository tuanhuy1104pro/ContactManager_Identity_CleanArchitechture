using CoreLayer.Domain.IdentityEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class BaseProjectDbContext:IdentityDbContext<ApplicationUser,ApplicationRole,Guid>
    {
        public BaseProjectDbContext(DbContextOptions option) :base(option) 
        {
            
        }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Person> Persons { get; set; } // table

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            //modelBuilder.Entity<Country>().ToTable("Countries");
            //modelBuilder.Entity<Person>().ToTable("Persons");
            //Configure table name thoi

            //Seed data to Countries
            //modelBuilder.Entity<Country>().HasData(new Country()
            //{ CountryID = Guid.NewGuid(),CountryName = "Sample"}); // cách thông thường thủ công

            //Seed data from Json file => seeding data bằng file json
            string countriesJson = System.IO.File.ReadAllText("countries.json");
            List<Country> countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesJson);

            foreach (Country country in countries)
            {
                modelBuilder.Entity<Country>().HasData(country);
            }

            //seed data for persons
            string personjon = System.IO.File.ReadAllText("persons.json");
            List<Person> persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personjon);
            foreach (Person person in persons)
            {
                modelBuilder.Entity<Person>().HasData(person);
            }

        }

        public List<Person> sp_GetAllPersons()
        {
            return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
        }
        public int sp_InsertPerson(Person person)
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@PersonID",person.PersonID),
                new SqlParameter("@PersonName", person.PersonName),
                new SqlParameter("@Email", person.Email),
                new SqlParameter("@DateOfBirth", person.DateOfBirth),
                new SqlParameter("@Gender", person.Gender),
                new SqlParameter("@CountryID", person.CountryID),
                new SqlParameter("@Address", person.Address),
                new SqlParameter("@ReceiveNewsLetters", person.ReceiveNewsLetters)
            };
            return Database.ExecuteSqlRaw("EXECUTE [dbo].[InsertPerson] @PersonID, @PersonName, @Email, @DateOfBirth, @Gender, @CountryID, @Address, @ReceiveNewsLetters", sp);
        }
    }
}
