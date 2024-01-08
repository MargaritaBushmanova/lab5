using System.Collections.Generic;

namespace lab5
{
    public class Composition
    {
        public string author { get; set; }
        public string song { get; set; }
        public Composition()
        {
            author = string.Empty;
            song = string.Empty;
        }
        public Composition(string _author, string _song)
        {
            author = _author;
            song = _song;
        }
        public override string ToString()
        {
            return author + " - " + song;
        }
    }


/*  public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Composition> Compositions { get; set; }
    }
*/



}