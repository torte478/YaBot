#nullable disable

namespace YaBot.App.Core.Database
{
    using YaBot;
    using YaBot.Database;

    public partial class Place : IHasId<int>
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual byte[] Image { get; set; }
    }
}
