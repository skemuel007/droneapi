using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class add_unique_constraint_dronerequestcode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DroneRequest_Drones_DroneId",
                table: "DroneRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Medication",
                table: "Medication");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DroneRequest",
                table: "DroneRequest");

            migrationBuilder.RenameTable(
                name: "Medication",
                newName: "Medications");

            migrationBuilder.RenameTable(
                name: "DroneRequest",
                newName: "DroneRequests");

            migrationBuilder.RenameIndex(
                name: "IX_Medication_Code",
                table: "Medications",
                newName: "IX_Medications_Code");

            migrationBuilder.RenameIndex(
                name: "IX_DroneRequest_DroneId",
                table: "DroneRequests",
                newName: "IX_DroneRequests_DroneId");

            migrationBuilder.AlterColumn<string>(
                name: "DroneRequestCode",
                table: "DroneRequests",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Medications",
                table: "Medications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DroneRequests",
                table: "DroneRequests",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DroneRequests_DroneRequestCode",
                table: "DroneRequests",
                column: "DroneRequestCode",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DroneRequests_Drones_DroneId",
                table: "DroneRequests",
                column: "DroneId",
                principalTable: "Drones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DroneRequests_Drones_DroneId",
                table: "DroneRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Medications",
                table: "Medications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DroneRequests",
                table: "DroneRequests");

            migrationBuilder.DropIndex(
                name: "IX_DroneRequests_DroneRequestCode",
                table: "DroneRequests");

            migrationBuilder.RenameTable(
                name: "Medications",
                newName: "Medication");

            migrationBuilder.RenameTable(
                name: "DroneRequests",
                newName: "DroneRequest");

            migrationBuilder.RenameIndex(
                name: "IX_Medications_Code",
                table: "Medication",
                newName: "IX_Medication_Code");

            migrationBuilder.RenameIndex(
                name: "IX_DroneRequests_DroneId",
                table: "DroneRequest",
                newName: "IX_DroneRequest_DroneId");

            migrationBuilder.AlterColumn<string>(
                name: "DroneRequestCode",
                table: "DroneRequest",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Medication",
                table: "Medication",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DroneRequest",
                table: "DroneRequest",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DroneRequest_Drones_DroneId",
                table: "DroneRequest",
                column: "DroneId",
                principalTable: "Drones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
