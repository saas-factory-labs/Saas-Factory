using AppBlueprint.Infrastructure.DatabaseContexts.Modules.Chat.Models;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Modules.Chat.Partials;

public partial class ChatModuleDbContext
{
    public DbSet<ChatModel> Chats { get; set; }
    public DbSet<ChatMessageModel> ChatMessages { get; set; }

    partial void OnModelCreating_Chat(ModelBuilder modelBuilder)
    {
        // ChatModel entity configuration
        modelBuilder.Entity<ChatModel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.OwnerId).IsRequired();

            // Relationship with Owner (UserEntity)
            entity.HasOne(e => e.Owner)
                  .WithMany()
                  .HasForeignKey(e => e.OwnerId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Relationship with ChatMessages
            entity.HasMany(e => e.ChatMessages)
                  .WithOne(m => m.Chat)
                  .HasForeignKey(m => m.ChatId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.ToTable("Chats");
        });

        // ChatMessageModel entity configuration
        modelBuilder.Entity<ChatMessageModel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.OwnerId).IsRequired();

            // Relationship with Chat
            entity.HasOne(e => e.Chat)
                  .WithMany(c => c.ChatMessages)
                  .HasForeignKey(e => e.ChatId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Relationship with Sender (UserEntity)
            entity.HasOne(e => e.Sender)
                  .WithMany()
                  .HasForeignKey(e => e.SenderId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Relationship with Owner (UserEntity)
            entity.HasOne(e => e.Owner)
                  .WithMany()
                  .HasForeignKey(e => e.OwnerId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable("ChatMessages");
        });
    }
}
