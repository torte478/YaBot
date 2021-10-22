namespace YaBot.PriorityApp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using YaBot;
    using YaBot.Extensions;
    using YaBot.IO;
    using YaBot.PriorityApp.Database;
    using YaBot.PriorityApp.Tree;
    using Project = YaBot.PriorityApp.Database.Project;

    internal sealed class Service
    {
        private const int Undefined = -1;

        private readonly Random random = new Random(DateTime.Now.Millisecond);
        
        private readonly ICrudl<int, Project> projects;
        private readonly ICrudl<int, Objective> objectives;
        private readonly Func<int, IProject> openProject;

        private IProject current;
        private int node = Undefined;
        private string added;

        public Service(ICrudl<int, Project> projects, ICrudl<int, Objective> objectives, Func<int, IProject> openProject)
        {
            this.projects = projects;
            this.objectives = objectives;
            this.openProject = openProject;
        }

        public IOutput Process(IInput input)
        {
            string result;
            try
            {
                result = InnerProcess(input);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                result = $"ERROR: {ex.Message}";
            }

            return result != null
                ? new Output {Text = result}
                : null;
        }

        private string InnerProcess(IInput input)
        {
            var command = input.Text.Split(' ');
            
            return command[0].ToLower() switch
            {
                "status" => GetStatus(),

                "create" => projects
                    .Create(new Project {Name = command[1]})
                    ._(_ => GetStatus()),

                "remove" => Remove(command),
                
                "open" => Open(command),
                
                "update" => current
                    .Update(command[1]._(int.Parse), command[2])
                    ._(_ => GetStatus()),
                
                "close" => Close(),
                
                "add" or "доб" => Add(command),
                
                "y" or "д" => TryAdd(true),
                
                "n" or "н" => TryAdd(false),
                
                "r" or "?" => TryAdd(random.Next(2) > 0),

                _ => null
            };
        }

        private string TryAdd(bool greater)
        {
            var result = current.TryAdd(node, greater, added);
            if (result.added)
            {
                node = Undefined;
                
                var items = current
                    .ToPriorityList()
                    .ToArray();

                if (items.Length == 0) return "{}";

                var sb = new StringBuilder();
                var limit = items.Length > 20;
                if (limit)
                    sb.AppendLine($"20 of {items.Length}");
                
                sb = items
                    .Take(20)
                    .Select(_ => $"{_.id}: {_.text}")
                    .Aggregate(sb, (acc, x) => acc.AppendLine(x));

                if (limit)
                    sb.Append("...");

                return sb.ToString();
            }
            else
            {
                node = result.next;
                return Ask();
            }
        }

        private string Ask()
        {
            return objectives.Read(node).Text
                ._(_ => string.Format("{2}{0}is greater then{0}{1}{0}?", Environment.NewLine, _, added));
        }

        private string Add(string[] command)
        {
            added = command.Skip(1)._(_ =>  string.Join(' ', _));
            node = current.FindRoot() ?? Undefined;
            return node != Undefined
                ? Ask()
                : TryAdd(true);
        }

        private string Remove(string[] command)
        {
            var id = command[1]._(int.Parse);

            if (current == null)
            {
                return projects
                    .Delete(id)
                    ._(_ => GetStatus());
            }
            else
            {
                return current
                    .Remove(id)
                    ._(_ => GetStatus());
            }
            
        }

        private string Close()
        {
            current = null;
            return GetStatus();
        }

        private string Open(string[] command)
        {
            current = command[1]._(int.Parse)._(openProject);
            return GetStatus();
        }

        private string GetStatus()
        {
            if (current == null)
            {
                return projects
                    .All()
                    .Select(_ => $"{_.Id}: {_.Name}")
                    ._(ConcatToString);    
            }
            else
            {
                return current
                    .ToPriorityList()
                    .Select(_ => $"{_.id}: {_.text}")
                    ._(ConcatToString);
            }
        }

        private static string ConcatToString(IEnumerable<string> lines)
        {
            var arr = lines.ToArray();
            
            return arr.Length > 0
                ? string.Join(Environment.NewLine, arr)
                : "{}";
        }
    }
}