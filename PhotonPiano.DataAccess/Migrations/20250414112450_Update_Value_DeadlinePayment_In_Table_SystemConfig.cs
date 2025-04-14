using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotonPiano.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Update_Value_DeadlinePayment_In_Table_SystemConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE ""SystemConfig""
                SET ""ConfigValue"" = '4',
                    ""UpdatedAt"" = CURRENT_TIMESTAMP
                WHERE ""ConfigName"" = 'Hạn chót thanh toán học phí'
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
