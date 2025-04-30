#nullable disable
using FluentMigrator;

namespace Wallymathieu.Auctions.Infrastructure.Migrations;

[Migration(20250430071300)]
public class AddAuctionTable : Migration
{
    public override void Up()
    {
        Create.Table("Auctions")
            .WithColumn("AuctionId").AsInt64().PrimaryKey().Identity()
            .WithColumn("Options_ReservePrice").AsInt64().Nullable()
            .WithColumn("Options_MinRaise").AsInt64().Nullable()
            .WithColumn("Options_TimeFrame").AsTime().Nullable()
            .WithColumn("EndsAt").AsDateTimeOffset().Nullable()
            .WithColumn("StartsAt").AsDateTimeOffset().NotNullable()
            .WithColumn("Title").AsAnsiString(200).Nullable()
            .WithColumn("Expiry").AsDateTimeOffset().NotNullable()
            .WithColumn("User").AsAnsiString(2000).NotNullable()
            .WithColumn("Currency").AsFixedLengthAnsiString(3).NotNullable()
            .WithColumn("AuctionType").AsInt32().NotNullable()
            .WithColumn("Options").AsInt32().Nullable()
            .WithColumn("OpenBidders").AsBoolean().NotNullable()
            .WithColumn("Version").AsGuid().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("Auctions");
    }
}