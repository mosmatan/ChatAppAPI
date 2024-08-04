using Microsoft.EntityFrameworkCore;
using ChatAPI.Models;

namespace ChatAPI.Data
{
    public class ChatDbContext: DbContext
    {
        public DbSet<UserDTO> Users { get; set; }
        public DbSet<ContactRequest> ContactRequests { get; set; }
        public DbSet<ConversationDTO> Conversations { get; set; }
        public DbSet<MessageDTO> Massages { get; set; }

        public ChatDbContext(DbContextOptions<ChatDbContext> i_Options) : base(i_Options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserDTO>().HasKey(user => user.UserId);
            modelBuilder.Entity<ContactRequest>().HasKey(request => request.Id);
            modelBuilder.Entity<ConversationDTO>().HasKey(converstion => converstion.ConversationId);
            modelBuilder.Entity<MessageDTO>().HasKey(message => message.MessageId);

            base.OnModelCreating(modelBuilder);
            // Additional configuration if needed
        }
    }
}
