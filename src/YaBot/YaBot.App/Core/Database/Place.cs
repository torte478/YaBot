#nullable disable

namespace YaBot.App.Core.Database
{
    using YaBot.Core;

    public partial class Place : IHasId<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }
    }
}
