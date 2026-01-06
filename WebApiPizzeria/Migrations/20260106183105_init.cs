using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApiPizzeria.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "conversation_sessions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    conversation_state = table.Column<string>(type: "text", nullable: false),
                    last_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("conversation_sessions_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "day_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("day_types_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "n8n_chat_histories",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    session_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    message = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("n8n_chat_histories_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "order_state_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("order_state_type_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "product_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("product_types_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    order_number = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    total = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    order_state_type_id = table.Column<int>(type: "integer", nullable: true),
                    last_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("orders_pkey", x => x.id);
                    table.ForeignKey(
                        name: "FK_orders_order_state_type_order_state_type_id",
                        column: x => x.order_state_type_id,
                        principalTable: "order_state_type",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    description = table.Column<string>(type: "text", nullable: false),
                    product_type_id = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("products_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_products_product_type",
                        column: x => x.product_type_id,
                        principalTable: "product_types",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "order_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    order_id = table.Column<int>(type: "integer", nullable: false),
                    product_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("order_items_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_order_items_order",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_order_items_product",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "product_availability_days",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    product_id = table.Column<int>(type: "integer", nullable: false),
                    day_type_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("product_availability_days_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_pad_day_type",
                        column: x => x.day_type_id,
                        principalTable: "day_types",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_product_availability_days_product",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "conversation_sessions_user_id_key",
                table: "conversation_sessions",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_conversation_sessions_last_updated",
                table: "conversation_sessions",
                column: "last_updated_at");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_order_id",
                table: "order_items",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_product_id",
                table: "order_items",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_order_state_type_id",
                table: "orders",
                column: "order_state_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_availability_days_day_type_id",
                table: "product_availability_days",
                column: "day_type_id");

            migrationBuilder.CreateIndex(
                name: "uq_product_day",
                table: "product_availability_days",
                columns: new[] { "product_id", "day_type_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_products_product_type_id",
                table: "products",
                column: "product_type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "conversation_sessions");

            migrationBuilder.DropTable(
                name: "n8n_chat_histories");

            migrationBuilder.DropTable(
                name: "order_items");

            migrationBuilder.DropTable(
                name: "product_availability_days");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "day_types");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "order_state_type");

            migrationBuilder.DropTable(
                name: "product_types");
        }
    }
}
