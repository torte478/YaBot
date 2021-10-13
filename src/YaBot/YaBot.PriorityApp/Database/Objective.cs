#nullable disable

namespace YaBot.PriorityApp.Database
{
    using YaBot;
    using YaBot.Database;

    public partial class Objective : IHasId<int>
    {
        public int Id { get; set; }
        public int Value { get; set; }
        public string Text { get; set; }
        public int Project { get; set; }

        public virtual Project ProjectNavigation { get; set; }
    }
}
