using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookReviews.API.Migrations
{
    /// <inheritdoc />
    public partial class addFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Rating",
                schema: "public",
                table: "reviews",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                schema: "public",
                table: "reviews",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoverImagePath",
                schema: "public",
                table: "books",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Isbn",
                schema: "public",
                table: "books",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                schema: "public",
                table: "books",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Pages",
                schema: "public",
                table: "books",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PublishedYear",
                schema: "public",
                table: "books",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Publisher",
                schema: "public",
                table: "books",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverImagePath",
                schema: "public",
                table: "books");

            migrationBuilder.DropColumn(
                name: "Isbn",
                schema: "public",
                table: "books");

            migrationBuilder.DropColumn(
                name: "Language",
                schema: "public",
                table: "books");

            migrationBuilder.DropColumn(
                name: "Pages",
                schema: "public",
                table: "books");

            migrationBuilder.DropColumn(
                name: "PublishedYear",
                schema: "public",
                table: "books");

            migrationBuilder.DropColumn(
                name: "Publisher",
                schema: "public",
                table: "books");

            migrationBuilder.AlterColumn<int>(
                name: "Rating",
                schema: "public",
                table: "reviews",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                schema: "public",
                table: "reviews",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);
        }
    }
}
