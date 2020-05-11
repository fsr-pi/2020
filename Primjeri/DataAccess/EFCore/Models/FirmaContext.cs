using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace EFCore.Models
{
  public partial class FirmaContext : DbContext
  {
    public virtual DbSet<Artikl> Artikl { get; set; }
    public virtual DbSet<Dokument> Dokument { get; set; }
    public virtual DbSet<Drzava> Drzava { get; set; }
    public virtual DbSet<Mjesto> Mjesto { get; set; }
    public virtual DbSet<Osoba> Osoba { get; set; }
    public virtual DbSet<Partner> Partner { get; set; }
    public virtual DbSet<Stavka> Stavka { get; set; }
    public virtual DbSet<Tvrtka> Tvrtka { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      var config = new ConfigurationBuilder()
                .AddUserSecrets("Firma")
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
      string connString = config["ConnectionStrings:Firma"];
      connString = connString.Replace("sifra", config["FirmaSqlPassword"]);
      optionsBuilder.UseSqlServer(connString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Artikl>(entity =>
      {
        entity.HasKey(e => e.SifArtikla)
            .HasName("pk_Artikl")
            .IsClustered(false);

        entity.HasIndex(e => e.NazArtikla)
            .HasName("ix_Artikl_NazArtikla")
            .IsUnique();

        entity.Property(e => e.SifArtikla)
            .HasComment("Šifra artikla")
            .ValueGeneratedNever();

        entity.Property(e => e.CijArtikla)
            .HasColumnType("money")
            .HasComment("Cijena artikla");

        entity.Property(e => e.JedMjere)
            .IsRequired()
            .HasMaxLength(5)
            .HasDefaultValueSql("('kom')")
            .HasComment("Jedinica mjere");

        entity.Property(e => e.NazArtikla)
            .IsRequired()
            .HasMaxLength(255)
            .HasComment("Naziv artikla");

        entity.Property(e => e.SlikaChecksum).HasComputedColumnSql("(checksum([SlikaArtikla]))");

        entity.Property(e => e.ZastUsluga).HasComment("Check box, usluge nemaju sliku");
      });

      modelBuilder.Entity<Dokument>(entity =>
      {
        entity.HasKey(e => e.IdDokumenta)
            .HasName("pk_Dokument")
            .IsClustered(false);

        entity.Property(e => e.IdDokumenta).HasComment("Identifikator dokumenta");

        entity.Property(e => e.BrDokumenta).HasComment("Broj dokumenta");

        entity.Property(e => e.DatDokumenta)
            .HasColumnType("datetime")
            .HasComment("Datum dokumenta");

        entity.Property(e => e.IdPartnera).HasComment("Partner");

        entity.Property(e => e.IdPrethDokumenta).HasComment("Prethodni dokument");

        entity.Property(e => e.IznosDokumenta)
            .HasColumnType("money")
            .HasComment("Ukupno stavke s porezom");

        entity.Property(e => e.PostoPorez)
            .HasColumnType("decimal(4, 2)")
            .HasComment("Postotak poreza");

        entity.Property(e => e.VrDokumenta)
            .IsRequired()
            .HasMaxLength(20)
            .HasComment("Vrsta dokumenta");

        entity.HasOne(d => d.IdPartneraNavigation)
            .WithMany(p => p.Dokument)
            .HasForeignKey(d => d.IdPartnera)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_Partner_Dokument");

        entity.HasOne(d => d.IdPrethDokumentaNavigation)
            .WithMany(p => p.InverseIdPrethDokumentaNavigation)
            .HasForeignKey(d => d.IdPrethDokumenta)
            .HasConstraintName("fk_Dokument_Dokument");
      });

      modelBuilder.Entity<Drzava>(entity =>
      {
        entity.HasKey(e => e.OznDrzave)
            .HasName("pk_Drzava")
            .IsClustered(false);

        entity.HasIndex(e => e.NazDrzave)
            .HasName("ix_Drzava_NazDrzave")
            .IsUnique();

        entity.Property(e => e.OznDrzave)
            .HasMaxLength(3)
            .HasComment("Oznaka države");

        entity.Property(e => e.Iso3drzave)
            .HasColumnName("ISO3Drzave")
            .HasMaxLength(3);

        entity.Property(e => e.NazDrzave)
            .IsRequired()
            .HasMaxLength(255)
            .HasComment("Naziv države");

        entity.Property(e => e.SifDrzave).HasDefaultValueSql("((0))");
      });

      modelBuilder.Entity<Mjesto>(entity =>
      {
        entity.HasKey(e => e.IdMjesta)
            .HasName("pk_Mjesto")
            .IsClustered(false);

        entity.HasIndex(e => e.NazMjesta)
            .HasName("ix_Mjesto_NazMjesta");

        entity.HasIndex(e => e.OznDrzave)
            .HasName("ix_Mjesto_OznDrzave");

        entity.Property(e => e.IdMjesta).HasComment("Identifikator mjesta");

        entity.Property(e => e.NazMjesta)
            .IsRequired()
            .HasMaxLength(40)
            .HasComment("Naziv mjesta");

        entity.Property(e => e.OznDrzave)
            .IsRequired()
            .HasMaxLength(3)
            .HasComment("Oznaka države");

        entity.Property(e => e.PostBrMjesta).HasComment("Poštanski broj mjesta");

        entity.Property(e => e.PostNazMjesta)
            .HasMaxLength(50)
            .HasComment("Naziv dostavne pošte");

        entity.HasOne(d => d.OznDrzaveNavigation)
            .WithMany(p => p.Mjesto)
            .HasForeignKey(d => d.OznDrzave)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_Drzava_Mjesto");
      });

      modelBuilder.Entity<Osoba>(entity =>
      {
        entity.HasKey(e => e.IdOsobe)
            .HasName("pk_Osoba")
            .IsClustered(false);

        entity.Property(e => e.IdOsobe)
            .HasComment("Identifikator osobe")
            .ValueGeneratedNever();

        entity.Property(e => e.ImeOsobe)
            .IsRequired()
            .HasMaxLength(20)
            .HasComment("Ime osobe");

        entity.Property(e => e.PrezimeOsobe)
            .IsRequired()
            .HasMaxLength(20)
            .HasComment("Prezime osobe");

        entity.HasOne(d => d.IdOsobeNavigation)
            .WithOne(p => p.Osoba)
            .HasForeignKey<Osoba>(d => d.IdOsobe)
            .HasConstraintName("fk_Partner_Osoba");
      });

      modelBuilder.Entity<Partner>(entity =>
      {
        entity.HasKey(e => e.IdPartnera)
            .HasName("pk_Partner")
            .IsClustered(false);

        entity.HasIndex(e => e.Oib)
            .HasName("ix_Partner_OIB")
            .IsUnique();

        entity.Property(e => e.IdPartnera).HasComment("Identifikator partnera");

        entity.Property(e => e.AdrIsporuke)
            .HasMaxLength(50)
            .HasComment("Adresa isporuke");

        entity.Property(e => e.AdrPartnera)
            .HasMaxLength(50)
            .HasComment("Adresa partnera");

        entity.Property(e => e.IdMjestaIsporuke).HasComment("Identifikator mjesta isporuke");

        entity.Property(e => e.IdMjestaPartnera).HasComment("Identifikator mjesta partnera");

        entity.Property(e => e.Oib)
            .HasColumnName("OIB")
            .HasMaxLength(50);

        entity.Property(e => e.TipPartnera)
            .IsRequired()
            .HasMaxLength(1)
            .HasComment("Tip partnera (F-fizička, P-pravna)");

        entity.HasOne(d => d.IdMjestaIsporukeNavigation)
            .WithMany(p => p.PartnerIdMjestaIsporukeNavigation)
            .HasForeignKey(d => d.IdMjestaIsporuke)
            .HasConstraintName("fk_Mjesto_Partner_Isporuka");

        entity.HasOne(d => d.IdMjestaPartneraNavigation)
            .WithMany(p => p.PartnerIdMjestaPartneraNavigation)
            .HasForeignKey(d => d.IdMjestaPartnera)
            .HasConstraintName("fk_Mjesto_Partner_Sjediste");
      });

      modelBuilder.Entity<Stavka>(entity =>
      {
        entity.HasKey(e => e.IdStavke)
            .HasName("pk_Stavka")
            .IsClustered(false);

        entity.HasIndex(e => e.SifArtikla)
            .HasName("ix_Stavka_SifArtikla");

        entity.HasIndex(e => new { e.IdDokumenta, e.SifArtikla })
            .HasName("IX_Stavka_SifArtikla_IdDokumenta")
            .IsUnique();

        entity.Property(e => e.IdDokumenta).HasComment("Identifikator dokumenta");

        entity.Property(e => e.JedCijArtikla)
            .HasColumnType("money")
            .HasComment("Cijena jediničnog artikla bez poreza. Inicijalno cijena iz tablice Artikl");

        entity.Property(e => e.KolArtikla)
            .HasColumnType("decimal(18, 5)")
            .HasComment("Količina artikla (za pojedine jedinice mjere može biti decimalni broj)");

        entity.Property(e => e.PostoRabat)
            .HasColumnType("decimal(4, 2)")
            .HasComment("Postotak popusta za pojedinu stavku");

        entity.Property(e => e.SifArtikla).HasComment("Šifra artikla");

        entity.HasOne(d => d.IdDokumentaNavigation)
            .WithMany(p => p.Stavka)
            .HasForeignKey(d => d.IdDokumenta)
            .HasConstraintName("fk_Dokument_Stavka");

        entity.HasOne(d => d.SifArtiklaNavigation)
            .WithMany(p => p.Stavka)
            .HasForeignKey(d => d.SifArtikla)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_Artikl_Stavka");
      });

      modelBuilder.Entity<Tvrtka>(entity =>
      {
        entity.HasKey(e => e.IdTvrtke)
            .HasName("pk_Tvrtka")
            .IsClustered(false);

        entity.HasIndex(e => e.MatBrTvrtke)
            .HasName("ix_Tvrtka_MatBrTvrtke")
            .IsUnique();

        entity.HasIndex(e => e.NazivTvrtke)
            .HasName("ix_Tvrtka_NazivTvrtke");

        entity.Property(e => e.IdTvrtke)
            .HasComment("Identifikator tvrtke")
            .ValueGeneratedNever();

        entity.Property(e => e.MatBrTvrtke)
            .IsRequired()
            .HasMaxLength(30)
            .HasComment("Matični broj tvrtke");

        entity.Property(e => e.NazivTvrtke)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("Naziv tvrtke");

        entity.HasOne(d => d.IdTvrtkeNavigation)
            .WithOne(p => p.Tvrtka)
            .HasForeignKey<Tvrtka>(d => d.IdTvrtke)
            .HasConstraintName("fk_Partner_Tvrtka");
      });

      OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
  }
}