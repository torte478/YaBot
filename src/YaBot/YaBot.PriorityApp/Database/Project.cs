#nullable disable

namespace YaBot.PriorityApp.Database
{
    using System.Collections.Generic;
    using YaBot.Core;

    public partial class Project : IHasId<int>
    {
        public Project()
        {
            Objectives = new HashSet<Objective>();
        }

        public int Id { get; set; }
        public int Parent { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Objective> Objectives { get; set; }
    }
}
