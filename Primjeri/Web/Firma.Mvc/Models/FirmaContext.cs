﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Firma.Mvc.Models
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
    public virtual DbSet<ViewPartner> vw_Partner { get; set; }
    public virtual DbSet<ViewDokumentInfo> ViewDokumentInfo { get; set; }
    public virtual DbSet<StavkaDenorm> StavkaDenorm { get; set; }

    public FirmaContext(DbContextOptions<FirmaContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<ViewPartner>(entity =>
      {
        entity.HasKey(e => e.IdPartnera);
      });
      modelBuilder.Entity<ViewDokumentInfo>(entity =>
      {
        entity.HasKey(e => e.IdDokumenta);
      });
      modelBuilder.Entity<StavkaDenorm>(entity =>
      {
        entity.HasKey(e => e.IdStavke);
      });
      modelBuilder.Entity<Artikl>(entity =>
      {
        entity.HasKey(e => e.SifArtikla)
            .ForSqlServerIsClustered(false);

        entity.HasIndex(e => e.NazArtikla)
            .HasName("ix_Artikl_NazArtikla")
            .IsUnique();

        entity.Property(e => e.SifArtikla).HasDefaultValueSql("((0))");

        entity.Property(e => e.CijArtikla)
            .HasColumnType("money")
            .HasDefaultValueSql("((0))");

        entity.Property(e => e.JedMjere)
            .IsRequired()
            .HasMaxLength(5)
            .HasDefaultValueSql("('kom')");

        entity.Property(e => e.NazArtikla)
            .IsRequired()
            .HasMaxLength(255);

        entity.Property(e => e.SlikaArtikla).HasColumnType("image");

        entity.Property(e => e.SlikaChecksum).HasComputedColumnSql("(checksum([SlikaArtikla]))");
      });

      modelBuilder.Entity<Dokument>(entity =>
      {
        entity.HasKey(e => e.IdDokumenta)
            .ForSqlServerIsClustered(false);

        entity.Property(e => e.BrDokumenta).HasDefaultValueSql("((0))");

        entity.Property(e => e.DatDokumenta).HasColumnType("datetime");

        entity.Property(e => e.IznosDokumenta)
            .HasColumnType("money")
            .HasDefaultValueSql("((0))");

        entity.Property(e => e.PostoPorez)
            .HasColumnType("decimal(4, 2)")
            .HasDefaultValueSql("((0))");

        entity.Property(e => e.VrDokumenta)
            .IsRequired()
            .HasMaxLength(20);

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
            .ForSqlServerIsClustered(false);

        entity.HasIndex(e => e.NazDrzave)
            .HasName("ix_Drzava_NazDrzave")
            .IsUnique();

        entity.Property(e => e.OznDrzave)
            .HasMaxLength(3)
            .ValueGeneratedNever();

        entity.Property(e => e.Iso3drzave)
            .HasColumnName("ISO3Drzave")
            .HasMaxLength(3);

        entity.Property(e => e.NazDrzave)
            .IsRequired()
            .HasMaxLength(255);

        entity.Property(e => e.SifDrzave).HasDefaultValueSql("((0))");
      });

      modelBuilder.Entity<Mjesto>(entity =>
      {
        entity.HasKey(e => e.IdMjesta)
            .ForSqlServerIsClustered(false);

        entity.HasIndex(e => e.NazMjesta)
            .HasName("ix_Mjesto_NazMjesta");

        entity.HasIndex(e => e.OznDrzave)
            .HasName("ix_Mjesto_OznDrzave");

        entity.Property(e => e.NazMjesta)
            .IsRequired()
            .HasMaxLength(40);

        entity.Property(e => e.OznDrzave)
            .IsRequired()
            .HasMaxLength(3);

        entity.Property(e => e.PostNazMjesta).HasMaxLength(50);

        entity.HasOne(d => d.OznDrzaveNavigation)
            .WithMany(p => p.Mjesto)
            .HasForeignKey(d => d.OznDrzave)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_Drzava_Mjesto");
      });

      modelBuilder.Entity<Osoba>(entity =>
      {
        entity.HasKey(e => e.IdOsobe)
            .ForSqlServerIsClustered(false);

        entity.Property(e => e.IdOsobe).ValueGeneratedNever();

        entity.Property(e => e.ImeOsobe)
            .IsRequired()
            .HasMaxLength(20);

        entity.Property(e => e.PrezimeOsobe)
            .IsRequired()
            .HasMaxLength(20);

        entity.HasOne(d => d.IdOsobeNavigation)
            .WithOne(p => p.Osoba)
            .HasForeignKey<Osoba>(d => d.IdOsobe)
            .HasConstraintName("fk_Partner_Osoba");
      });

      modelBuilder.Entity<Partner>(entity =>
      {
        entity.HasKey(e => e.IdPartnera)
            .ForSqlServerIsClustered(false);

        entity.HasIndex(e => e.Oib)
            .HasName("ix_Partner_OIB")
            .IsUnique();

        entity.Property(e => e.AdrIsporuke).HasMaxLength(50);

        entity.Property(e => e.AdrPartnera).HasMaxLength(50);

        entity.Property(e => e.Oib)
            .HasColumnName("OIB")
            .HasMaxLength(50);

        entity.Property(e => e.TipPartnera)
            .IsRequired()
            .HasMaxLength(1);

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
            .ForSqlServerIsClustered(false);

        entity.HasIndex(e => e.SifArtikla)
            .HasName("ix_Stavka_SifArtikla");

        entity.HasIndex(e => new { e.IdDokumenta, e.SifArtikla })
            .HasName("IX_Stavka_SifArtikla_IdDokumenta")
            .IsUnique();       

        entity.Property(e => e.JedCijArtikla)
            .HasColumnType("money")
            .HasDefaultValueSql("((0))");

        entity.Property(e => e.KolArtikla).HasColumnType("decimal(18, 5)");

        entity.Property(e => e.PostoRabat)
            .HasColumnType("decimal(4, 2)")
            .HasDefaultValueSql("((0))");

        entity.Property(e => e.SifArtikla).HasDefaultValueSql("((0))");

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
            .ForSqlServerIsClustered(false);

        entity.HasIndex(e => e.MatBrTvrtke)
            .HasName("ix_Tvrtka_MatBrTvrtke")
            .IsUnique();

        entity.HasIndex(e => e.NazivTvrtke)
            .HasName("ix_Tvrtka_NazivTvrtke");

        entity.Property(e => e.IdTvrtke).ValueGeneratedNever();

        entity.Property(e => e.MatBrTvrtke)
            .IsRequired()
            .HasMaxLength(30);

        entity.Property(e => e.NazivTvrtke)
            .IsRequired()
            .HasMaxLength(50);

        entity.HasOne(d => d.IdTvrtkeNavigation)
            .WithOne(p => p.Tvrtka)
            .HasForeignKey<Tvrtka>(d => d.IdTvrtke)
            .HasConstraintName("fk_Partner_Tvrtka");
      });
    }
  }
}
