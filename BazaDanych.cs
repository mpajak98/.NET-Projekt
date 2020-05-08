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
    /// <summary>
    /// Tabele bazy danych
    /// </summary>
    /// <remarks>
    /// W tej klasie znajdują się tabele Utworów, Wykonawców oraz Albumów
    /// </remarks>
    class BazaDanych : DbContext
    {
        /// <value>Tabela  utworów</value>
        public DbSet<Utwor> Utwory { get; set; }
        /// <value>Tabela  wykonawców</value>
        public DbSet<Wykonawca> Wykonawcy { get; set; }
        /// <value>Tabela  albumów</value>
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
                    LiczbaUtworowWAlumie = 0
                }
            ) ;

            modelBuilder.Entity<Album>()
                .Property(b => b.WykonawcaId)
                .HasDefaultValue(1);
            modelBuilder.Entity<Album>()
                .Property(b => b.Rok)
                .HasDefaultValue("nieznany");

            modelBuilder.Entity<Wykonawca>().HasData(
                new Wykonawca
                {
                    WykonawcaId = 1,
                    Nazwa = "nieznany",
                    LiczbaUtworowWykonawcy = 0
                }
            );

        }
    }

    /// <summary>
    /// Klasa definiująca atrybuty encji Utwór
    /// </summary>
    public class Utwor
    {
        /// <value>Zwraca, ustawia ID utworu</value>
        public int UtworId { get; set; }

        /// <value>Zwraca, ustawia nazwę utworu</value>
        public string Nazwa { get; set; }

        /// <value>Zwraca, ustawia ID wykonawcy utworu</value>
        public int WykonawcaId { get; set; }

        /// <value>Zwraca, ustawia ID albumu utworu</value>
        public int AlbumId { get; set; }

        /// <value>Zwraca, ustawia czy utwór jest ulubiony</value>
        public bool CzyUlubione { get; set; }

        /// <value>Zwraca, ustawia datę dodania utworu do bazy</value>
        public DateTime DataDodania { get; set; }

        /// <value>Zwraca, ustawia rok wydania utworu</value>
        public string Rok { get; set; }

        /// <value>Zwraca, ustawia link do utworu</value>
        public string UrlPath { get; set; }
    }

    /// <summary>
    /// Klasa definiująca atrybuty encji Wykonawca
    /// </summary>
    public class Wykonawca
    {
        /// <value>Zwraca, ustawia ID wykonawcy</value>
        public int WykonawcaId { get; set; }

        /// <value>Zwraca, ustawia nazwę wykonawcy</value>
        public string Nazwa { get; set; }

        /// <value>Zwraca, ustawia liczbę utworów przypisanych wykonawcy </value>
        public int LiczbaUtworowWykonawcy { get; set; }

    }

    /// <summary>
    /// Klasa definiująca atrybuty encji Album
    /// </summary>
    public class Album
    {
        /// <value>Zwraca, ustawia ID albumu</value>
        public int AlbumId { get; set; }

        /// <value>Zwraca, ustawia nazwę albumu</value>
        public string Nazwa { get; set; }

        /// <value>Zwraca, ustawia rok wydania albumu</value>
        public string Rok { get; set; }

        /// <value>Zwraca, ustawia ID wykonawcy albumu</value>
        public int WykonawcaId { get; set; }

        /// <value>Zwraca, ustawia liczbę utworów przypisanych albumowi </value>
        public int LiczbaUtworowWAlumie { get; set; }

    }


}