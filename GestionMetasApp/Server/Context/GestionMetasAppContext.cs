using System;
using System.Collections.Generic;
using GestionMetasApp.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionMetasApp.Server.Context;

public partial class GestionMetasAppContext : DbContext
{
    public GestionMetasAppContext()
    {
    }

    public GestionMetasAppContext(DbContextOptions<GestionMetasAppContext> options)
        : base(options)
    {
    }

    public virtual DbSet<MetaMod> Metas { get; set; }

    public virtual DbSet<TareaMod> Tareas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MetaMod>(entity =>
        {
            entity.HasKey(e => e.IdMeta).HasName("PK__METAS__472B2D442512E2A8");

            entity.ToTable("METAS");

            entity.Property(e => e.IdMeta).HasColumnName("ID_META");
            entity.Property(e => e.FechaCreacion)
                .HasColumnType("datetime")
                .HasColumnName("FECHA_CREACION");
            entity.Property(e => e.Nombre)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");
            entity.Property(e => e.Porcentaje)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("PORCENTAJE");
            entity.Property(e => e.TotalTareas).HasColumnName("TOTAL_TAREAS");
        });

        modelBuilder.Entity<TareaMod>(entity =>
        {
            entity.HasKey(e => e.IdTarea).HasName("PK__TAREA__3484F0F986E94F5E");

            entity.ToTable("TAREA");

            entity.Property(e => e.IdTarea).HasColumnName("ID_TAREA");
            entity.Property(e => e.Estado)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("ESTADO");
            entity.Property(e => e.FechaCreacion)
                .HasColumnType("datetime")
                .HasColumnName("FECHA_CREACION");
            entity.Property(e => e.IdMeta).HasColumnName("ID_META");
            entity.Property(e => e.Importante).HasColumnName("IMPORTANTE");
            entity.Property(e => e.Nombre)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");

            entity.HasOne(d => d.meta).WithMany(p => p.Tareas)
                .HasForeignKey(d => d.IdMeta)
                .HasConstraintName("FK__TAREA__ID_META__5441852A");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
