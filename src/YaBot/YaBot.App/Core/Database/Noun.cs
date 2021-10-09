namespace YaBot.App.Core.Database
{
    using YaBot.Core;
    using YaBot.Core.Database;

    public partial class Noun : IHasId<int>
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }
}