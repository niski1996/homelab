using Microsoft.EntityFrameworkCore;
using HomeLabGymApi.Models;
using System.Text.Json;

namespace HomeLabGymApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<ExerciseTag> ExerciseTags { get; set; }
    public DbSet<ExerciseLink> ExerciseLinks { get; set; }
    public DbSet<WorkoutTemplate> WorkoutTemplates { get; set; }
    public DbSet<TemplateExercise> TemplateExercises { get; set; }
    public DbSet<TemplateSet> TemplateSets { get; set; }
    public DbSet<WorkoutSession> WorkoutSessions { get; set; }
    public DbSet<SessionExercise> SessionExercises { get; set; }
    public DbSet<WorkoutSet> WorkoutSets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Exercise configuration
        modelBuilder.Entity<Exercise>(entity =>
        {
            entity.ToTable("exercises");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Category).HasColumnName("category").HasMaxLength(100);
            entity.Property(e => e.ExerciseType).HasColumnName("exercise_type").IsRequired().HasMaxLength(50);
            entity.Property(e => e.AdditionalSettings).HasColumnName("additional_settings").HasColumnType("jsonb");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");

            entity.HasIndex(e => e.ExerciseType).HasDatabaseName("idx_exercise_type");
        });

        // Tag configuration
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("tags");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
            
            entity.HasIndex(e => e.Name).IsUnique().HasDatabaseName("idx_tag_name_unique");
        });

        // ExerciseTag configuration (many-to-many)
        modelBuilder.Entity<ExerciseTag>(entity =>
        {
            entity.ToTable("exercise_tags");
            entity.HasKey(et => new { et.ExerciseId, et.TagId });
            entity.Property(et => et.ExerciseId).HasColumnName("exercise_id");
            entity.Property(et => et.TagId).HasColumnName("tag_id");

            entity.HasOne(et => et.Exercise)
                  .WithMany(e => e.ExerciseTags)
                  .HasForeignKey(et => et.ExerciseId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(et => et.Tag)
                  .WithMany(t => t.ExerciseTags)
                  .HasForeignKey(et => et.TagId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ExerciseLink configuration
        modelBuilder.Entity<ExerciseLink>(entity =>
        {
            entity.ToTable("exercise_links");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.ExerciseId).HasColumnName("exercise_id");
            entity.Property(e => e.Url).HasColumnName("url").IsRequired().HasMaxLength(500);
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(200);

            entity.HasOne(el => el.Exercise)
                  .WithMany(e => e.Links)
                  .HasForeignKey(el => el.ExerciseId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // WorkoutTemplate configuration
        modelBuilder.Entity<WorkoutTemplate>(entity =>
        {
            entity.ToTable("workout_templates");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Category).HasColumnName("category").HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");
        });

        // TemplateExercise configuration
        modelBuilder.Entity<TemplateExercise>(entity =>
        {
            entity.ToTable("template_exercises");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.WorkoutTemplateId).HasColumnName("workout_template_id");
            entity.Property(e => e.ExerciseId).HasColumnName("exercise_id");
            entity.Property(e => e.OrderIndex).HasColumnName("order_index").HasDefaultValue(0);

            entity.HasOne(te => te.WorkoutTemplate)
                  .WithMany(wt => wt.TemplateExercises)
                  .HasForeignKey(te => te.WorkoutTemplateId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(te => te.Exercise)
                  .WithMany(e => e.TemplateExercises)
                  .HasForeignKey(te => te.ExerciseId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // TemplateSet configuration
        modelBuilder.Entity<TemplateSet>(entity =>
        {
            entity.ToTable("template_sets");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.TemplateExerciseId).HasColumnName("template_exercise_id");
            entity.Property(e => e.SetNumber).HasColumnName("set_number");
            entity.Property(e => e.Metrics).HasColumnName("metrics").HasColumnType("jsonb").HasDefaultValueSql("'{}'::jsonb");

            entity.HasOne(ts => ts.TemplateExercise)
                  .WithMany(te => te.TemplateSets)
                  .HasForeignKey(ts => ts.TemplateExerciseId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // WorkoutSession configuration
        modelBuilder.Entity<WorkoutSession>(entity =>
        {
            entity.ToTable("workout_sessions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.WorkoutTemplateId).HasColumnName("workout_template_id");
            entity.Property(e => e.SessionDate).HasColumnName("session_date");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.IsCompleted).HasColumnName("is_completed").HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");

            entity.HasIndex(e => e.SessionDate).HasDatabaseName("idx_workout_session_date");

            entity.HasOne(ws => ws.WorkoutTemplate)
                  .WithMany(wt => wt.WorkoutSessions)
                  .HasForeignKey(ws => ws.WorkoutTemplateId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // SessionExercise configuration
        modelBuilder.Entity<SessionExercise>(entity =>
        {
            entity.ToTable("session_exercises");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.WorkoutSessionId).HasColumnName("workout_session_id");
            entity.Property(e => e.ExerciseId).HasColumnName("exercise_id");
            entity.Property(e => e.OrderIndex).HasColumnName("order_index").HasDefaultValue(0);

            entity.HasOne(se => se.WorkoutSession)
                  .WithMany(ws => ws.SessionExercises)
                  .HasForeignKey(se => se.WorkoutSessionId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(se => se.Exercise)
                  .WithMany(e => e.SessionExercises)
                  .HasForeignKey(se => se.ExerciseId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // WorkoutSet configuration
        modelBuilder.Entity<WorkoutSet>(entity =>
        {
            entity.ToTable("workout_sets");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.SessionExerciseId).HasColumnName("session_exercise_id");
            entity.Property(e => e.SetNumber).HasColumnName("set_number");
            entity.Property(e => e.Completed).HasColumnName("completed").HasDefaultValue(false);
            entity.Property(e => e.Metrics).HasColumnName("metrics").HasColumnType("jsonb").HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.Notes).HasColumnName("notes");

            entity.HasOne(ws => ws.SessionExercise)
                  .WithMany(se => se.WorkoutSets)
                  .HasForeignKey(ws => ws.SessionExerciseId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed some initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Tags
        var pushTag = new Tag { Id = Guid.NewGuid(), Name = "push" };
        var pullTag = new Tag { Id = Guid.NewGuid(), Name = "pull" };
        var legsTag = new Tag { Id = Guid.NewGuid(), Name = "legs" };

        modelBuilder.Entity<Tag>().HasData(pushTag, pullTag, legsTag);

        // Seed Exercises
        var pushPress = new Exercise
        {
            Id = Guid.NewGuid(),
            Name = "Push Press",
            Description = "Overhead push press with legs drive",
            Category = "shoulders",
            ExerciseType = "Strength",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        var benchPress = new Exercise
        {
            Id = Guid.NewGuid(),
            Name = "Bench Press",
            Description = "Horizontal pressing movement",
            Category = "chest",
            ExerciseType = "Strength",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        var squat = new Exercise
        {
            Id = Guid.NewGuid(),
            Name = "Back Squat",
            Description = "Barbell back squat",
            Category = "legs",
            ExerciseType = "Strength",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        modelBuilder.Entity<Exercise>().HasData(pushPress, benchPress, squat);

        // Seed ExerciseTags
        modelBuilder.Entity<ExerciseTag>().HasData(
            new ExerciseTag { ExerciseId = pushPress.Id, TagId = pushTag.Id },
            new ExerciseTag { ExerciseId = benchPress.Id, TagId = pushTag.Id },
            new ExerciseTag { ExerciseId = squat.Id, TagId = legsTag.Id }
        );
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity is Exercise exercise)
            {
                if (entry.State == EntityState.Added)
                    exercise.CreatedAt = DateTimeOffset.UtcNow;
                exercise.UpdatedAt = DateTimeOffset.UtcNow;
            }
            else if (entry.Entity is WorkoutTemplate template)
            {
                if (entry.State == EntityState.Added)
                    template.CreatedAt = DateTimeOffset.UtcNow;
                template.UpdatedAt = DateTimeOffset.UtcNow;
            }
            else if (entry.Entity is WorkoutSession session)
            {
                if (entry.State == EntityState.Added)
                    session.CreatedAt = DateTimeOffset.UtcNow;
                session.UpdatedAt = DateTimeOffset.UtcNow;
            }
        }
    }
}
