﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
	/// <inheritdoc />
	public partial class InitialCreate : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Products",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
					InventoryCount = table.Column<int>(type: "int", nullable: false),
					Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
					Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Products", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Users",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Users", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Orders",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
					BuyerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Quantity = table.Column<int>(type: "int", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Orders", x => x.Id);
					table.ForeignKey(
						name: "FK_Orders_Products_ProductId",
						column: x => x.ProductId,
						principalTable: "Products",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_Orders_Users_BuyerId",
						column: x => x.BuyerId,
						principalTable: "Users",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_Orders_BuyerId",
				table: "Orders",
				column: "BuyerId");

			migrationBuilder.CreateIndex(
				name: "IX_Orders_ProductId",
				table: "Orders",
				column: "ProductId");

			AddSampleUsers(migrationBuilder);
		}

		private static void AddSampleUsers(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(@"INSERT INTO [dbo].[Users]
								   ([Id]
								   ,[Name])
							 VALUES
								   (newid(),'Hamid'),
								   (newid(),'Ali'),
								   (newid(),'Sina'),
								   (newid(),'Reza')");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Orders");

			migrationBuilder.DropTable(
				name: "Products");

			migrationBuilder.DropTable(
				name: "Users");
		}
	}
}
