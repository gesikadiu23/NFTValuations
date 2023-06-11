using Microsoft.EntityFrameworkCore.Migrations;

namespace NFTValuations.Migrations
{
    public partial class addContractTokenId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContractTokenId",
                table: "DatabaseModels",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContractTokenId",
                table: "DatabaseModels");
        }
    }
}
