using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataAccessLibrary.Data
{
    public class ApplicationDbContext : IdentityDbContext<ProfileEntity>
    {
        private readonly IConfiguration _config;

        public DbSet<ProfileEntity> Profiles { get; set; }
        public DbSet<TopicEntity> Topics { get; set; }
        public DbSet<UserFile> UserFiles { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<UserItem> UserItems { get; set; }

        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration config)
            : base(options)
        {
            _config = config;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=tcp:vutor-server.database.windows.net,1433;Initial Catalog=vutor_db;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication= Active Directory Default",
                options => options.EnableRetryOnFailure().MaxBatchSize(100));
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProfileEntity>(e =>
            {
                e.Property(e => e.Name)
                    .HasMaxLength(250);

                e.Property(e => e.Surname)
                    .HasMaxLength(250);

                e.Property(e => e.CourseInfo)
                    .HasConversion(
                        v => v.ToString(),
                        v => new CourseData(v))
                    .HasMaxLength(250);

                e.Property(e => e.CreationDate)
                    .HasConversion(
                        v => v.ToString(),
                        v => new profileCreationDate(v))
                    .HasMaxLength(250);

                e.HasMany(p => p.TopicsToLearn)
                    .WithMany(t => t.LearningProfiles)
                    .UsingEntity(j => j.ToTable("ProfilesLearningTopics"));

                e.HasMany(p => p.TopicsToTeach)
                    .WithMany(t => t.TeachingProfiles)
                    .UsingEntity(j => j.ToTable("ProfilesTeachingTopics"));
            });

            modelBuilder.Entity<TopicEntity>(e =>
            {
                e.HasKey(e => e.Id);

                e.Property(e => e.Title)
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<Folder>()
                .HasMany(e => e.SubFolders)
                .WithOne(e => e.ParentFolder)
                .HasForeignKey(e => e.ParentFolderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserItem>(e =>
            { 
                e.HasOne(e => e.Profile)
                    .WithMany(e => e.UserItems)
                    .HasForeignKey(e => e.ProfileId);

                 e.Property(e => e.CreationDate)
                      .HasConversion(
                          v => v.ToString(),
                          v => new profileCreationDate(v))
                      .HasMaxLength(250);
            });

            modelBuilder.Entity<UserFile>(e =>
            { 
                e.HasOne(e => e.Folder)
                    .WithMany(e => e.Files)
                    .HasForeignKey(e => e.FolderId)
                    .IsRequired();

                e.HasMany(e=> e.Topics)
                    .WithMany(e => e.UserFiles)
                    .UsingEntity(j => j.ToTable("TopicToFiles"));

                e.Property(e => e.BlobUri)
                    .HasMaxLength(500);
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}