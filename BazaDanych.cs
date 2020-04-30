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
        public DbSet<Utwor> Muzyka { get; set; }
        public DbSet<Wykonawca> Wykonawcy { get; set; }
        public DbSet<Album> Albumy { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=bazadanych.db");
    }

    public class Utwor
    {
        public int UtworId { get; set; }
        public string Nazwa { get; set; }
        public int CzasTrwania { get; set; }
        public int WykonawcaId { get; set; }
        public int AlbumId { get; set; }

        [Range(0, 5)]
        public int Ocena { get; set; }
        public bool CzyUlubione { get; set; }
        public DateTime DataDodania { get; set; }
        [Range(1900, 2100)]
        public int Rok { get; set; }
        public string UrlPath { get; set; }
    }

    public class Wykonawca
    {
        public int WykonawcaId { get; set; }
        public string Nazwa { get; set; }

    }

    public class Album
    {
        public int AlbumId { get; set; }
        public string Nazwa { get; set; }
        public int Rok { get; set; }

    }


}