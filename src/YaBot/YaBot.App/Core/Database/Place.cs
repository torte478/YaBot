#nullable disable

namespace YaBot.App.Core.Database
{
    public partial class Place
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }
    }
}
