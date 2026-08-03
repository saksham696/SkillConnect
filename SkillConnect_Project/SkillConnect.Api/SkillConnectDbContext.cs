using Microsoft.EntityFrameworkCore;
public class SkillConnectDbContext : DbContext
{
    public SkillConnectDbContext(DbContextOptions<SkillConnectDbContext> options) : base(options)
    {
        
    }
    public DbSet<User> Users { get; set; }   

    public DbSet<Job> Jobs { get; set; }

    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<JobApplication> JobApplications { get; set; }
    
    
}