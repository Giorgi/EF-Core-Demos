using NpgsqlTypes;

namespace FTS;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public string Author { get; set; }
    public string Tags { get; set; }

    public NpgsqlTsVector SearchDocument { get; set; }
}