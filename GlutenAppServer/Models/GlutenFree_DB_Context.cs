﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GlutenAppServer.Models;

public partial class GlutenFree_DB_Context : DbContext
{
    public GlutenFree_DB_Context()
    {
    }

    public GlutenFree_DB_Context(DbContextOptions<GlutenFree_DB_Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Critic> Critics { get; set; }

    public virtual DbSet<Recipe> Recipes { get; set; }

    public virtual DbSet<Restaurant> Restaurants { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<TypeFood> TypeFoods { get; set; }

    public virtual DbSet<TypeUserId> TypeUserIds { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server = (localdb)\\MSSQLLocalDB;Initial Catalog=GlutenFree_DB;User ID=AppAdminLogin;Password=Tami;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Critic>(entity =>
        {
            entity.HasKey(e => e.CriticId).HasName("PK__Critics__A19EBA85ADCE7DC3");

            entity.HasOne(d => d.Rest).WithMany(p => p.Critics).HasConstraintName("FK__Critics__RestID__32E0915F");

            entity.HasOne(d => d.User).WithMany(p => p.Critics).HasConstraintName("FK__Critics__UserID__31EC6D26");
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.RecipeId).HasName("PK__Recipes__FDD988D099F101A8");

            entity.HasOne(d => d.Status).WithMany(p => p.Recipes).HasConstraintName("FK__Recipes__StatusI__37A5467C");

            entity.HasOne(d => d.TypeFood).WithMany(p => p.Recipes).HasConstraintName("FK__Recipes__TypeFoo__35BCFE0A");

            entity.HasOne(d => d.User).WithMany(p => p.Recipes).HasConstraintName("FK__Recipes__UserID__36B12243");
        });

        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.HasKey(e => e.RestId).HasName("PK__Restaura__02F04D6AC2EF37A8");

            entity.HasOne(d => d.Status).WithMany(p => p.Restaurants).HasConstraintName("FK__Restauran__Statu__2F10007B");

            entity.HasOne(d => d.TypeFood).WithMany(p => p.Restaurants).HasConstraintName("FK__Restauran__TypeF__2E1BDC42");

            entity.HasOne(d => d.User).WithMany(p => p.Restaurants).HasConstraintName("FK__Restauran__UserI__2D27B809");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Statuses__C8EE2043DD60A97B");

            entity.Property(e => e.StatusId).ValueGeneratedNever();
        });

        modelBuilder.Entity<TypeFood>(entity =>
        {
            entity.HasKey(e => e.TypeFoodId).HasName("PK__TypeFood__D1DD6A43A03C804C");
        });

        modelBuilder.Entity<TypeUserId>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__TypeUser__516F03957040DFCB");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC2884EF8A");

            entity.HasOne(d => d.Type).WithMany(p => p.Users).HasConstraintName("FK__Users__TypeID__2A4B4B5E");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
