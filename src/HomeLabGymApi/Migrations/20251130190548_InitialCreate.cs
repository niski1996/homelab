using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HomeLabGymApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "exercises",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    exercise_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    additional_settings = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercises", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tags", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "workout_templates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workout_templates", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "exercise_links",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    exercise_id = table.Column<Guid>(type: "uuid", nullable: false),
                    url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercise_links", x => x.id);
                    table.ForeignKey(
                        name: "FK_exercise_links_exercises_exercise_id",
                        column: x => x.exercise_id,
                        principalTable: "exercises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "exercise_tags",
                columns: table => new
                {
                    exercise_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tag_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercise_tags", x => new { x.exercise_id, x.tag_id });
                    table.ForeignKey(
                        name: "FK_exercise_tags_exercises_exercise_id",
                        column: x => x.exercise_id,
                        principalTable: "exercises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_exercise_tags_tags_tag_id",
                        column: x => x.tag_id,
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "template_exercises",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    workout_template_id = table.Column<Guid>(type: "uuid", nullable: false),
                    exercise_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_index = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_template_exercises", x => x.id);
                    table.ForeignKey(
                        name: "FK_template_exercises_exercises_exercise_id",
                        column: x => x.exercise_id,
                        principalTable: "exercises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_template_exercises_workout_templates_workout_template_id",
                        column: x => x.workout_template_id,
                        principalTable: "workout_templates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workout_sessions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    workout_template_id = table.Column<Guid>(type: "uuid", nullable: true),
                    session_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workout_sessions", x => x.id);
                    table.ForeignKey(
                        name: "FK_workout_sessions_workout_templates_workout_template_id",
                        column: x => x.workout_template_id,
                        principalTable: "workout_templates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "template_sets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    template_exercise_id = table.Column<Guid>(type: "uuid", nullable: false),
                    set_number = table.Column<int>(type: "integer", nullable: false),
                    metrics = table.Column<JsonDocument>(type: "jsonb", nullable: false, defaultValueSql: "'{}'::jsonb")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_template_sets", x => x.id);
                    table.ForeignKey(
                        name: "FK_template_sets_template_exercises_template_exercise_id",
                        column: x => x.template_exercise_id,
                        principalTable: "template_exercises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "session_exercises",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    workout_session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    exercise_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_index = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_session_exercises", x => x.id);
                    table.ForeignKey(
                        name: "FK_session_exercises_exercises_exercise_id",
                        column: x => x.exercise_id,
                        principalTable: "exercises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_session_exercises_workout_sessions_workout_session_id",
                        column: x => x.workout_session_id,
                        principalTable: "workout_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workout_sets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    session_exercise_id = table.Column<Guid>(type: "uuid", nullable: false),
                    set_number = table.Column<int>(type: "integer", nullable: false),
                    completed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    metrics = table.Column<JsonDocument>(type: "jsonb", nullable: false, defaultValueSql: "'{}'::jsonb"),
                    notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workout_sets", x => x.id);
                    table.ForeignKey(
                        name: "FK_workout_sets_session_exercises_session_exercise_id",
                        column: x => x.session_exercise_id,
                        principalTable: "session_exercises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "exercises",
                columns: new[] { "id", "additional_settings", "category", "created_at", "description", "exercise_type", "name", "updated_at" },
                values: new object[,]
                {
                    { new Guid("6ed4004d-7b48-4542-a892-2c20d99c6b96"), null, "chest", new DateTimeOffset(new DateTime(2025, 11, 30, 19, 5, 46, 465, DateTimeKind.Unspecified).AddTicks(2560), new TimeSpan(0, 0, 0, 0, 0)), "Horizontal pressing movement", "Strength", "Bench Press", new DateTimeOffset(new DateTime(2025, 11, 30, 19, 5, 46, 465, DateTimeKind.Unspecified).AddTicks(2560), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("7fb65bec-fb9e-41c8-82c4-66a4ab15148b"), null, "shoulders", new DateTimeOffset(new DateTime(2025, 11, 30, 19, 5, 46, 465, DateTimeKind.Unspecified).AddTicks(2544), new TimeSpan(0, 0, 0, 0, 0)), "Overhead push press with legs drive", "Strength", "Push Press", new DateTimeOffset(new DateTime(2025, 11, 30, 19, 5, 46, 465, DateTimeKind.Unspecified).AddTicks(2545), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("969dc17e-ca07-4c1e-b482-12167ba2a015"), null, "legs", new DateTimeOffset(new DateTime(2025, 11, 30, 19, 5, 46, 465, DateTimeKind.Unspecified).AddTicks(2574), new TimeSpan(0, 0, 0, 0, 0)), "Barbell back squat", "Strength", "Back Squat", new DateTimeOffset(new DateTime(2025, 11, 30, 19, 5, 46, 465, DateTimeKind.Unspecified).AddTicks(2574), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.InsertData(
                table: "tags",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { new Guid("0372d9c0-8b12-43b3-ac63-b5269fb10521"), "legs" },
                    { new Guid("45b9aa4c-82aa-4eb5-83e0-7d670d6cf865"), "pull" },
                    { new Guid("9c56ffd6-3aed-4420-9dc5-70e5b6db414e"), "push" }
                });

            migrationBuilder.InsertData(
                table: "exercise_tags",
                columns: new[] { "exercise_id", "tag_id" },
                values: new object[,]
                {
                    { new Guid("6ed4004d-7b48-4542-a892-2c20d99c6b96"), new Guid("9c56ffd6-3aed-4420-9dc5-70e5b6db414e") },
                    { new Guid("7fb65bec-fb9e-41c8-82c4-66a4ab15148b"), new Guid("9c56ffd6-3aed-4420-9dc5-70e5b6db414e") },
                    { new Guid("969dc17e-ca07-4c1e-b482-12167ba2a015"), new Guid("0372d9c0-8b12-43b3-ac63-b5269fb10521") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_exercise_links_exercise_id",
                table: "exercise_links",
                column: "exercise_id");

            migrationBuilder.CreateIndex(
                name: "IX_exercise_tags_tag_id",
                table: "exercise_tags",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "idx_exercise_type",
                table: "exercises",
                column: "exercise_type");

            migrationBuilder.CreateIndex(
                name: "IX_session_exercises_exercise_id",
                table: "session_exercises",
                column: "exercise_id");

            migrationBuilder.CreateIndex(
                name: "IX_session_exercises_workout_session_id",
                table: "session_exercises",
                column: "workout_session_id");

            migrationBuilder.CreateIndex(
                name: "idx_tag_name_unique",
                table: "tags",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_template_exercises_exercise_id",
                table: "template_exercises",
                column: "exercise_id");

            migrationBuilder.CreateIndex(
                name: "IX_template_exercises_workout_template_id",
                table: "template_exercises",
                column: "workout_template_id");

            migrationBuilder.CreateIndex(
                name: "IX_template_sets_template_exercise_id",
                table: "template_sets",
                column: "template_exercise_id");

            migrationBuilder.CreateIndex(
                name: "idx_workout_session_date",
                table: "workout_sessions",
                column: "session_date");

            migrationBuilder.CreateIndex(
                name: "IX_workout_sessions_workout_template_id",
                table: "workout_sessions",
                column: "workout_template_id");

            migrationBuilder.CreateIndex(
                name: "IX_workout_sets_session_exercise_id",
                table: "workout_sets",
                column: "session_exercise_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "exercise_links");

            migrationBuilder.DropTable(
                name: "exercise_tags");

            migrationBuilder.DropTable(
                name: "template_sets");

            migrationBuilder.DropTable(
                name: "workout_sets");

            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.DropTable(
                name: "template_exercises");

            migrationBuilder.DropTable(
                name: "session_exercises");

            migrationBuilder.DropTable(
                name: "exercises");

            migrationBuilder.DropTable(
                name: "workout_sessions");

            migrationBuilder.DropTable(
                name: "workout_templates");
        }
    }
}
