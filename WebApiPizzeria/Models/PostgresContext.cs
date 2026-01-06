using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebApiPizzeria.Models;

public partial class PostgresContext : DbContext
{
    public PostgresContext()
    {
    }

    public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ConversationSession> ConversationSessions { get; set; }

    public virtual DbSet<DayType> DayTypes { get; set; }

    public virtual DbSet<N8nChatHistory> N8nChatHistories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderStateType> OrderStateTypes { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductAvailabilityDay> ProductAvailabilityDays { get; set; }

    public virtual DbSet<ProductType> ProductTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder
        //    .HasPostgresEnum("auth", "aal_level", new[] { "aal1", "aal2", "aal3" })
        //    .HasPostgresEnum("auth", "code_challenge_method", new[] { "s256", "plain" })
        //    .HasPostgresEnum("auth", "factor_status", new[] { "unverified", "verified" })
        //    .HasPostgresEnum("auth", "factor_type", new[] { "totp", "webauthn", "phone" })
        //    .HasPostgresEnum("auth", "oauth_authorization_status", new[] { "pending", "approved", "denied", "expired" })
        //    .HasPostgresEnum("auth", "oauth_client_type", new[] { "public", "confidential" })
        //    .HasPostgresEnum("auth", "oauth_registration_type", new[] { "dynamic", "manual" })
        //    .HasPostgresEnum("auth", "oauth_response_type", new[] { "code" })
        //    .HasPostgresEnum("auth", "one_time_token_type", new[] { "confirmation_token", "reauthentication_token", "recovery_token", "email_change_token_new", "email_change_token_current", "phone_change_token" })
        //    .HasPostgresEnum("realtime", "action", new[] { "INSERT", "UPDATE", "DELETE", "TRUNCATE", "ERROR" })
        //    .HasPostgresEnum("realtime", "equality_op", new[] { "eq", "neq", "lt", "lte", "gt", "gte", "in" })
        //    .HasPostgresEnum("storage", "buckettype", new[] { "STANDARD", "ANALYTICS", "VECTOR" })
        //    .HasPostgresExtension("extensions", "pg_stat_statements")
        //    .HasPostgresExtension("extensions", "pgcrypto")
        //    .HasPostgresExtension("extensions", "uuid-ossp")
        //    .HasPostgresExtension("graphql", "pg_graphql")
        //    .HasPostgresExtension("vault", "supabase_vault");

        modelBuilder.Entity<ConversationSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("conversation_sessions_pkey");

            entity.ToTable("conversation_sessions");

            entity.HasIndex(e => e.UserId, "conversation_sessions_user_id_key").IsUnique();

            entity.HasIndex(e => e.LastUpdatedAt, "idx_conversation_sessions_last_updated");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.ConversationState).HasColumnName("conversation_state");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.LastUpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("last_updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<DayType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("day_types_pkey");

            entity.ToTable("day_types");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<N8nChatHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("n8n_chat_histories_pkey");

            entity.ToTable("n8n_chat_histories");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Message)
                .HasColumnType("jsonb")
                .HasColumnName("message");
            entity.Property(e => e.SessionId)
                .HasMaxLength(255)
                .HasColumnName("session_id");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orders_pkey");

            entity.ToTable("orders");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.LastUpdate)
                .HasDefaultValueSql("now()")
                .HasColumnName("last_update");
            entity.Property(e => e.OrderNumber).HasColumnName("order_number");
            entity.Property(e => e.OrderStateTypeId).HasColumnName("order_state_type_id");
            entity.Property(e => e.Total)
                .HasPrecision(10, 2)
                .HasColumnName("total");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_items_pkey");

            entity.ToTable("order_items");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("fk_order_items_order");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_order_items_product");
        });

        modelBuilder.Entity<OrderStateType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_state_type_pkey");

            entity.ToTable("order_state_type");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("products_pkey");

            entity.ToTable("products");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.ProductTypeId).HasColumnName("product_type_id");

            entity.HasOne(d => d.ProductType).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_products_product_type");
        });

        modelBuilder.Entity<ProductAvailabilityDay>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("product_availability_days_pkey");

            entity.ToTable("product_availability_days");

            entity.HasIndex(e => new { e.ProductId, e.DayTypeId }, "uq_product_day").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.DayTypeId).HasColumnName("day_type_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");

            entity.HasOne(d => d.DayType).WithMany(p => p.ProductAvailabilityDays)
                .HasForeignKey(d => d.DayTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_pad_day_type");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductAvailabilityDays)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_product_availability_days_product");
        });

        modelBuilder.Entity<ProductType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("product_types_pkey");

            entity.ToTable("product_types");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
