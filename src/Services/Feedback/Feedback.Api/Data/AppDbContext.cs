namespace Feedback.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<FeedbackEntry> Feedbacks => Set<FeedbackEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Feedback");

        modelBuilder.Entity<FeedbackEntry>(e =>
        {
            e.ToTable("Feedbacks");
            e.HasKey(f => f.Id);
            e.Property(f => f.Message).HasMaxLength(500).IsRequired();
        });
    }
}
