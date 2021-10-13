namespace YaBot.App.Core.Database
{
    using YaBot;
    using YaBot.Database;

    public partial class Noun : IHasId<int>
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }
}