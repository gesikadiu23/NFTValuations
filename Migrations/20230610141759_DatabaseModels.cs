using Microsoft.EntityFrameworkCore.Migrations;

namespace NFTValuations.Migrations
{
    public partial class DatabaseModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DatabaseModels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ExternalUrl = table.Column<string>(nullable: true),
                    Media = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatabaseModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertyModel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category = table.Column<string>(nullable: true),
                    Property = table.Column<string>(nullable: true),
                    DatabaseModelId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyModel_DatabaseModels_DatabaseModelId",
                        column: x => x.DatabaseModelId,
                        principalTable: "DatabaseModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyModel_DatabaseModelId",
                table: "PropertyModel",
                column: "DatabaseModelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyModel");

            migrationBuilder.DropTable(
                name: "DatabaseModels");
        }
    }
}
