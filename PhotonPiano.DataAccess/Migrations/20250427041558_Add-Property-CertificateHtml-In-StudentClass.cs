using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyCertificateHtmlInStudentClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CertificateHtml",
                table: "StudentClass",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasCertificateHtml",
                table: "StudentClass",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CertificateHtml",
                table: "StudentClass");

            migrationBuilder.DropColumn(
                name: "HasCertificateHtml",
                table: "StudentClass");
        }
    }
}
