using GOMessage.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace GOMessage.Data;

public class ChatDbContext : DbContext
{
    public DbSet<User> Users { get; init; }
    public DbSet<Chat> Chats { get; init; }
    public DbSet<UserChat> UserChats { get; init; }
    public DbSet<Message> Messages { get; init; }

    public ChatDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().ToCollection("users");
        modelBuilder.Entity<Chat>().ToCollection("chats");
        modelBuilder.Entity<UserChat>().ToCollection("userChats");
        modelBuilder.Entity<Message>().ToCollection("messages");
    }
}