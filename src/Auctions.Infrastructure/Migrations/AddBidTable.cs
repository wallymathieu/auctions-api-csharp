#nullable disable
using FluentMigrator;

namespace Wallymathieu.Auctions.Infrastructure.Migrations;

[Migration(20250430071800)]
public class AddBidTable : Migration
{
    public override void Up()
    {
        Create.Table("Bids")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("User").AsAnsiString(2000).NotNullable()
            .WithColumn("Amount").AsInt64().NotNullable()
            .WithColumn("At").AsDateTimeOffset().NotNullable()
            .WithColumn("AuctionId").AsInt64().NotNullable()
                .ForeignKey("FK_Bids_Auctions_AuctionId", "Auctions", "AuctionId");

    }

    public override void Down()
    {
        Delete.Table("Bids");
    }
}