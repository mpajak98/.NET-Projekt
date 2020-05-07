using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace BibliotekaMultimediow
{

    class BazaDanych : DbContext
    {
        public DbSet<Utwor> Utwory { get; set; }
        public DbSet<Wykonawca> Wykonawcy { get; set; }
        public DbSet<Album> Albumy { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=bazadanych.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Utwor>()
                .Property(b => b.WykonawcaId)
                .HasDefaultValue(1);
            modelBuilder.Entity<Utwor>()
                .Property(b => b.AlbumId)
                .HasDefaultValue(1);
            modelBuilder.Entity<Utwor>()
                .Property(b => b.Rok)
                .HasDefaultValue("nieznany");
            modelBuilder.Entity<Utwor>()
                .Property(b => b.CzyUlubione)
                .HasDefaultValue(false);

            modelBuilder.Entity<Album>().HasData(
                new Album
                {
                    AlbumId = 1,
                    Nazwa = "nieznany",
                    Rok = "nieznany",
                    WykonawcaId = 1,
                    CzyUlubione = false
                }
            ) ;

            modelBuilder.Entity<Album>()
                .Property(b => b.WykonawcaId)
                .HasDefaultValue(1);
            modelBuilder.Entity<Album>()
                .Property(b => b.CzyUlubione)
                .HasDefaultValue(false);
            modelBuilder.Entity<Album>()
                .Property(b => b.Rok)
                .HasDefaultValue("nieznany");

            modelBuilder.Entity<Wykonawca>().HasData(
                new Wykonawca
                {
                    WykonawcaId = 1,
                    Nazwa = "nieznany",
                    CzyUlubione = false
                }
            );

            modelBuilder.Entity<Wykonawca>()
                .Property(b => b.CzyUlubione)
                .HasDefaultValue(false);


        }
    }

    public class Utwor
    {
        public int UtworId { get; set; }
        public string Nazwa { get; set; }
        public int WykonawcaId { get; set; }
        public int AlbumId { get; set; }

        public bool CzyUlubione { get; set; }
        public DateTime DataDodania { get; set; }

        public string Rok { get; set; }
        public string UrlPath { get; set; }
    }

    public class Wykonawca
    {
        public int WykonawcaId { get; set; }
        public string Nazwa { get; set; }
        public bool CzyUlubione { get; set; }

    }

    public class Album
    {
        public int AlbumId { get; set; }
        public string Nazwa { get; set; }
        public string Rok { get; set; }
        public int WykonawcaId { get; set; }
        public bool CzyUlubione { get; set; }

    }


}