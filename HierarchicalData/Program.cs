using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace HierarchicalData
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var context = new HierarchicalDataContext();
            await context.Database.MigrateAsync();

            var organizationPositions = await context.Positions.ToListAsync();

            var top = organizationPositions.Single(p => p.Path.GetLevel() == 0);

            var root = new Tree(top.Name).Guide(TreeGuide.Line);

            RenderChildren(root, top, organizationPositions);

            AnsiConsole.Write(root);

            #region Filter By Level

            var level = 2;
            var positions = await context.Positions.Where(p => p.Path.GetLevel() == level).ToListAsync();

            Console.WriteLine();
            Console.WriteLine("Positions on level {0}", level);
            Console.WriteLine();

            foreach (var p in positions)
            {
                Console.WriteLine(p.Name);
            }
            Console.WriteLine();

            #endregion

            #region Find Descendants

            var title = "CFO";

            var cfo = context.Positions.Single(p => p.Name == title);
            positions = await context.Positions.Where(p => p.Path.IsDescendantOf(cfo.Path) && p.Id != cfo.Id)
                                         .OrderBy(p => p.Path.GetLevel()).ThenBy(p => p.Path)
                                         .ToListAsync();

            Console.WriteLine();
            Console.WriteLine("Descendants of {0}", title);
            Console.WriteLine();

            foreach (var p in positions)
            {
                Console.WriteLine(p.Name);
            }
            Console.WriteLine();

            #endregion


            #region Find Ancestors
            title = ".Net Senior Engineer";
            var engineer = context.Positions.Single(p => p.Name == title);
            positions = await context.Positions.Where(p => engineer.Path.IsDescendantOf(p.Path) && p.Id != cfo.Id)
                                         .OrderByDescending(p => p.Path.GetLevel()).ToListAsync();

            Console.WriteLine();
            Console.WriteLine("Ancestors of {0}", title);
            Console.WriteLine();

            foreach (var p in positions)
            {
                Console.WriteLine(p.Name);
            }
            Console.WriteLine();
            #endregion

            #region Direct Ancestor

            title = "CTO";
            positions = await context.Positions
                               .Where(p => p.Path.GetAncestor(1) == context.Positions.Single(o => o.Name == title).Path).ToListAsync();
            Console.WriteLine();
            Console.WriteLine("Direct descendants of {0}", title);
            Console.WriteLine();

            foreach (var p in positions)
            {
                Console.WriteLine(p.Name);
            }
            Console.WriteLine();
            #endregion

            #region Common Ancestor

            var title1 = "Frontend Team Lead";
            var title2 = ".Net Junior Engineer";

            var frontEndTeamLeadPosition = await context.Positions.SingleAsync(p => p.Name == title1);
            var netJuniorEngineer = await context.Positions.SingleAsync(p => p.Name == title2);

            var common = await context.Positions.Where(p => frontEndTeamLeadPosition.Path.IsDescendantOf(p.Path)
                                                      && netJuniorEngineer.Path.IsDescendantOf(p.Path))
                                                  .OrderByDescending(p => p.Path).FirstAsync();

            Console.WriteLine();
            Console.WriteLine($"Common Ancestor of {title1} & {title2} is {common.Name}");

            #endregion

            #region Add Child

            title = "Chief Commercial Officer";

            var cco = await context.Positions.SingleAsync(p => p.Name == title);

            var descendants = await context.Positions.Where(p => p.Path.IsDescendantOf(cco.Path)).ToListAsync();

            var maxChild = descendants.Max(p => p.Path);
            var newPath = cco.Path.GetDescendant(maxChild, null);

            context.Positions.Add(new OrganizationPosition
            {
                Name = "Commercial Vice President",
                Path = newPath
            });
            await context.SaveChangesAsync();

            #endregion
        }

        private static void RenderChildren(IHasTreeNodes node, OrganizationPosition parent, List<OrganizationPosition> organizationPositions)
        {
            var children = organizationPositions.Where(p => p.Path.GetAncestor(1) == parent.Path)
                                            .OrderBy(p => p.Path);

            foreach (var child in children)
            {
                var treeNode = node.AddNode(child.Name);
                RenderChildren(treeNode, child, organizationPositions);
            }
        }
    }
}