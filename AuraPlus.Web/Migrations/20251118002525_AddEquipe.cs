using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuraPlus.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_arp_equipe",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    nm_time = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    descricao = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_arp_equipe", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "t_arp_relatorio_equipe",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    data = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    sentimento_medio = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    descritivo = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    id_equipe = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_arp_relatorio_equipe", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_arp_relatorio_equipe_t_arp_equipe_id_equipe",
                        column: x => x.id_equipe,
                        principalTable: "t_arp_equipe",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_arp_users",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    nome = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    senha = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    role = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    cargo = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    data_admissao = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    ativo = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: false, defaultValue: "1"),
                    id_equipe = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_arp_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_arp_users_t_arp_equipe_id_equipe",
                        column: x => x.id_equipe,
                        principalTable: "t_arp_equipe",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "t_arp_reconhecimento",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    titulo = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    descricao = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    data = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    id_reconhecedor = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    id_reconhecido = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_arp_reconhecimento", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_arp_reconhecimento_t_arp_users_id_reconhecedor",
                        column: x => x.id_reconhecedor,
                        principalTable: "t_arp_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_arp_reconhecimento_t_arp_users_id_reconhecido",
                        column: x => x.id_reconhecido,
                        principalTable: "t_arp_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_arp_relatorio_pessoa",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    numero_indicacoes = table.Column<int>(type: "NUMBER(5,0)", precision: 5, scale: 0, nullable: false, defaultValue: 0),
                    data = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    descritivo = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    id_usuario = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_arp_relatorio_pessoa", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_arp_relatorio_pessoa_t_arp_users_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "t_arp_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_arp_sentimentos",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    nome_sentimento = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    valor_pontuacao = table.Column<decimal>(type: "DECIMAL(5,2)", precision: 5, scale: 2, nullable: true),
                    data = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    descricao = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    id_usuario = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_arp_sentimentos", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_arp_sentimentos_t_arp_users_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "t_arp_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_arp_reconhecimento_id_reconhecedor",
                table: "t_arp_reconhecimento",
                column: "id_reconhecedor");

            migrationBuilder.CreateIndex(
                name: "IX_t_arp_reconhecimento_id_reconhecido",
                table: "t_arp_reconhecimento",
                column: "id_reconhecido");

            migrationBuilder.CreateIndex(
                name: "IX_t_arp_relatorio_equipe_id_equipe",
                table: "t_arp_relatorio_equipe",
                column: "id_equipe");

            migrationBuilder.CreateIndex(
                name: "IX_t_arp_relatorio_pessoa_id_usuario",
                table: "t_arp_relatorio_pessoa",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_t_arp_sentimentos_id_usuario",
                table: "t_arp_sentimentos",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_t_arp_users_email",
                table: "t_arp_users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_arp_users_id_equipe",
                table: "t_arp_users",
                column: "id_equipe");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_arp_reconhecimento");

            migrationBuilder.DropTable(
                name: "t_arp_relatorio_equipe");

            migrationBuilder.DropTable(
                name: "t_arp_relatorio_pessoa");

            migrationBuilder.DropTable(
                name: "t_arp_sentimentos");

            migrationBuilder.DropTable(
                name: "t_arp_users");

            migrationBuilder.DropTable(
                name: "t_arp_equipe");
        }
    }
}
