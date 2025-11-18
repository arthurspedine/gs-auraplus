using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuraPlus.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddReconhecimento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "numero_indicacoes",
                table: "t_arp_relatorio_pessoa",
                type: "NUMBER(5,0)",
                precision: 5,
                scale: 0,
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(short),
                oldType: "NUMBER(5,0)",
                oldPrecision: 5,
                oldDefaultValue: (short)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short>(
                name: "numero_indicacoes",
                table: "t_arp_relatorio_pessoa",
                type: "NUMBER(5,0)",
                precision: 5,
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(int),
                oldType: "NUMBER(5,0)",
                oldPrecision: 5,
                oldScale: 0,
                oldDefaultValue: 0);
        }
    }
}
