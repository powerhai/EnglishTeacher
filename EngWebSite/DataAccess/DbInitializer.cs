public static class DbInitializer
{
    public static void Initialize(FoolDbContext context)
    {
        context.Database.EnsureCreated();
        var p1 = new Publisher() { Title = "人教版" };
        context.Publishers.Add(p1);
        var b1 = new Book() { Title = "三年级 - 上", Publisher = p1 };
        context.Books.Add(b1);

        var b2 = new Book() { Title = "三年级 - 下", Publisher = p1 };
        context.Books.Add(b2);
        var p2 = new Publisher() { Title = "外研社" };
        context.Publishers.Add(p2);
        var b3 = new Book() { Title = "五年级 - 上", Publisher = p2 };
        context.Books.Add(b3);

        var b4 = new Book() { Title = "五年级 - 下", Publisher = p2 };
        context.Books.Add(b4);

        var txt = new Text(){ Book = b1, Title = "This is a book.", Body="Hello, world!"};
        context.Texts.Add(txt);

        var sen = new Sentence(){ Text= txt, English = "Hello, world!" , Chinese="你好世界!", FileName = "a.avi" };
        context.Sentences.Add(sen);
        context.SaveChanges();
    }
}