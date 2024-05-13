using FluentMigrator;

namespace Route256.Week5.Workshop.PriceCalculator.Dal.Migrations;

[Migration(20241504154200, TransactionBehavior.None)]
public class UpdateTaskCommentsTable : Migration
{
    public override void Up()
    {
        Alter.Table("task_comments")
            .AddColumn("modified_at").AsDateTimeOffset().Nullable()
            .AddColumn("deleted_at").AsDateTimeOffset().Nullable();
    }

    public override void Down()
    {
        Delete.Column("modified_at")
            .Column("deleted_at")
            .FromTable("task_comments");
    }
}