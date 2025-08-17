using MegisEbys.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MegisEbys.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Birim> Birimler { get; set; }
    public DbSet<EvrakTuru> EvrakTurleri { get; set; }
    public DbSet<Kullanici> Kullanicilar { get; set; } // Yeni eklendi
    public DbSet<Evrak> Evraklar { get; set; }
    public DbSet<Kurum> Kurumlar { get; set; } // Yeni eklendi
    public DbSet<DisKullanici> DisKullanicilar { get; set; } // Yeni eklendi
    public DbSet<DisDepartman> DisDepartmanlar { get; set; }
    public DbSet<IzinTalebi> IzinTalepleri { get; set; }
    public DbSet<IslemGecmisi> Islemler { get; set; }
    public DbSet<Sablon> Sablonlar { get; set; }
}