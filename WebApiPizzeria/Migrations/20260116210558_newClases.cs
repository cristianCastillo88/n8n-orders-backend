using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApiPizzeria.Migrations
{
    /// <inheritdoc />
    public partial class newClases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentOrderItemId",
                table: "order_items",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExtraTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtraTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemExtras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderItemId = table.Column<int>(type: "integer", nullable: false),
                    ExtraTypeId = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemExtras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItemExtras_ExtraTypes_ExtraTypeId",
                        column: x => x.ExtraTypeId,
                        principalTable: "ExtraTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItemExtras_order_items_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "order_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_order_items_ParentOrderItemId",
                table: "order_items",
                column: "ParentOrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemExtras_ExtraTypeId",
                table: "OrderItemExtras",
                column: "ExtraTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemExtras_OrderItemId",
                table: "OrderItemExtras",
                column: "OrderItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_order_items_order_items_ParentOrderItemId",
                table: "order_items",
                column: "ParentOrderItemId",
                principalTable: "order_items",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_order_items_order_items_ParentOrderItemId",
                table: "order_items");

            migrationBuilder.DropTable(
                name: "OrderItemExtras");

            migrationBuilder.DropTable(
                name: "ExtraTypes");

            migrationBuilder.DropIndex(
                name: "IX_order_items_ParentOrderItemId",
                table: "order_items");

            migrationBuilder.DropColumn(
                name: "ParentOrderItemId",
                table: "order_items");
        }
    }
}
