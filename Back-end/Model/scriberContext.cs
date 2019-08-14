using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Back_end.Model
{
    public partial class scriberContext : DbContext
    {
        public scriberContext()
        {
        }

        public scriberContext(DbContextOptions<scriberContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Transcription> Transcription { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=tcp:scriber-hgs.database.windows.net,1433;Initial Catalog=scriber;Persist Security Info=False;User ID=admin-hgs;Password=scriber-7890;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<Transcription>(entity =>
            {
                entity.Property(e => e.Phrase).IsUnicode(false);

                entity.HasOne(d => d.Video)
                    .WithMany(p => p.Transcription)
                    .HasForeignKey(d => d.VideoId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("VideoId");
            });

        }
    }
}
