using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MovieAPI.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSubscriptionPlanDurationEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "SubscriptionPlans");

            migrationBuilder.AddColumn<int>(
                name: "DurationInDays",
                table: "SubscriptionPlans",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationInDays",
                table: "SubscriptionPlans");

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "SubscriptionPlans",
                type: "text",
                nullable: false,
                defaultValue: "");

            
        }
    }
}
