#nullable disable

namespace YaBot.App.Core.Database
{
    using YaBot.Core;
    using YaBot.Core.Database;

    public partial class Place : IHasId<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }
    }
}
