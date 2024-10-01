using System;
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

    public virtual DbSet<Information> Information { get; set; }

    public virtual DbSet<Recipe> Recipes { get; set; }

    public virtual DbSet<Restaurant> Restaurants { get; set; }

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
            entity.HasKey(e => e.CriticId).HasName("PK__Critics__A19EBA850321E61D");

            entity.HasOne(d => d.Rest).WithMany(p => p.Critics).HasConstraintName("FK__Critics__RestID__300424B4");

            entity.HasOne(d => d.User).WithMany(p => p.Critics).HasConstraintName("FK__Critics__UserID__2F10007B");
        });

        modelBuilder.Entity<Information>(entity =>
        {
            entity.HasKey(e => e.InfoId).HasName("PK__Informat__4DEC9D9A7CB207A1");
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.RecipeId).HasName("PK__Recipes__FDD988D02B4747B0");

            entity.HasOne(d => d.User).WithMany(p => p.Recipes).HasConstraintName("FK__Recipes__UserID__32E0915F");
        });

        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.HasKey(e => e.RestId).HasName("PK__Restaura__02F04D6A3D3FB14D");

            entity.HasOne(d => d.TypeFood).WithMany(p => p.Restaurants).HasConstraintName("FK__Restauran__TypeF__2C3393D0");

            entity.HasOne(d => d.User).WithMany(p => p.Restaurants).HasConstraintName("FK__Restauran__UserI__2B3F6F97");
        });

        modelBuilder.Entity<TypeFood>(entity =>
        {
            entity.HasKey(e => e.TypeFoodId).HasName("PK__TypeFood__D1DD6A43B844ECFF");
        });

        modelBuilder.Entity<TypeUserId>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__TypeUser__516F03955DC9FEC1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACD8CE6A46");

            entity.HasOne(d => d.Type).WithMany(p => p.Users).HasConstraintName("FK__Users__TypeID__286302EC");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
