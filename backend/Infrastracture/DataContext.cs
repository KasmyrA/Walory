using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastracture
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Walor> Walors { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }

        public DbSet<UserFriend> UserFriends { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserFriend>()
                .HasKey(uf => new { uf.UserId, uf.FriendId });

            modelBuilder.Entity<UserFriend>()
                .HasOne(uf => uf.User)
                .WithMany(u => u.Friends)
                .HasForeignKey(uf => uf.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserFriend>()
                .HasOne(uf => uf.Friend)
                .WithMany()
                .HasForeignKey(uf => uf.FriendId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Walor>()
                .Property(i => i.Content)
                .HasColumnType("jsonb"); // PostgreSQL JSON

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany()
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Collection)
                .WithMany(coll => coll.Comments)
                .HasForeignKey(c => c.CollectionId)
                .OnDelete(DeleteBehavior.SetNull); 

            modelBuilder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Like>()
                .HasOne(l => l.Collection)
                .WithMany(w => w.Likes)
                .HasForeignKey(l => l.CollectionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Walor>()
                .HasOne(w => w.Collection)
                .WithMany(c => c.Walors)
                .HasForeignKey(w => w.CollectionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Collection>()
                .HasOne(c => c.Owner)          
                .WithMany(u => u.Collections)  
                .HasForeignKey(c => c.OwnerId)  
                .OnDelete(DeleteBehavior.Cascade); 


            modelBuilder.Entity<Collection>()
                .HasMany(c => c.Walors)         
                .WithOne(w => w.Collection)      
                .HasForeignKey(w => w.CollectionId) 
                .OnDelete(DeleteBehavior.Cascade); 


            modelBuilder.Entity<Collection>()
                .HasMany(c => c.Comments)         
                .WithOne(cm => cm.Collection)     
                .HasForeignKey(cm => cm.CollectionId) 
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<Collection>()
                .HasMany(c => c.Likes)          
                .WithOne(l => l.Collection)     
                .HasForeignKey(l => l.CollectionId)  
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
