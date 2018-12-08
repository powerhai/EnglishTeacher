using Microsoft.EntityFrameworkCore;
public class FoolDbContext : DbContext
{
    public FoolDbContext(DbContextOptions<FoolDbContext> options)  : base(options)
    {
     
    }
   
    public DbSet<Sentence> Sentences {get;set;}
    public DbSet<Word> Words{get;set;}
    public DbSet<Publisher> Publishers{get;set;}
    public DbSet<Book> Books{get;set;} 
    public DbSet<Text> Texts{get;set;}
}