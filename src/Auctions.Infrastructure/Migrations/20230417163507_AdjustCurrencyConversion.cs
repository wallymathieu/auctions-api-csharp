using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Migrations
{
    /// <inheritdoc />
    public partial class AdjustCurrencyConversion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bids_Auctions_TimedAscendingAuctionAuctionId",
                table: "Bids"
            );

            migrationBuilder.RenameColumn(
                name: "TimedAscendingAuctionAuctionId",
                table: "Bids",
                newName: "AuctionId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_Bids_TimedAscendingAuctionAuctionId",
                table: "Bids",
                newName: "IX_Bids_AuctionId"
            );

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "Auctions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_Auctions_AuctionId",
                table: "Bids",
                column: "AuctionId",
                principalTable: "Auctions",
                principalColumn: "AuctionId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Bids_Auctions_AuctionId", table: "Bids");

            migrationBuilder.RenameColumn(
                name: "AuctionId",
                table: "Bids",
                newName: "TimedAscendingAuctionAuctionId"
            );

            migrationBuilder.RenameIndex(
                name: "IX_Bids_AuctionId",
                table: "Bids",
                newName: "IX_Bids_TimedAscendingAuctionAuctionId"
            );

            migrationBuilder.AlterColumn<int>(
                name: "Currency",
                table: "Auctions",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_Auctions_TimedAscendingAuctionAuctionId",
                table: "Bids",
                column: "TimedAscendingAuctionAuctionId",
                principalTable: "Auctions",
                principalColumn: "AuctionId"
            );
        }
    }
}
