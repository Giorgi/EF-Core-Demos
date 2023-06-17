using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace HierarchicalData
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var context = new HierarchicalDataContext();
            var organizationPositions = context.Positions.ToList();

            var top = organizationPositions.Single(p => p.Path.GetLevel() == 0);

            var root = new Tree(top.Name).Guide(TreeGuide.Line);

            AddChildren(root, top, organizationPositions);

            AnsiConsole.Write(root);

            #region Filter By Level

            var level = 2;
            var positions = context.Positions.Where(p => p.Path.GetLevel() == level).ToList();

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
            positions = context.Positions.Where(p => p.Path.IsDescendantOf(cfo.Path) && p.Id != cfo.Id)
                                         .OrderBy(p => p.Path.GetLevel()).ThenBy(p => p.Path)
                                         .ToList();

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
            positions = context.Positions.Where(p => engineer.Path.IsDescendantOf(p.Path) && p.Id != cfo.Id)
                                         .OrderByDescending(p => p.Path.GetLevel()).ToList();

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
            positions = context.Positions
                               .Where(p => p.Path.GetAncestor(1) == context.Positions.Single(o => o.Name == title).Path).ToList();
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

            var frontEndTeamLeadPosition = context.Positions.Single(p => p.Name == title1);
            var netJuniorEngineer = context.Positions.Single(p => p.Name == title2);

            var common = context.Positions.Where(p => frontEndTeamLeadPosition.Path.IsDescendantOf(p.Path)
                                                      && netJuniorEngineer.Path.IsDescendantOf(p.Path))
                                                  .OrderByDescending(p => p.Path).First();

            Console.WriteLine();
            Console.WriteLine($"Common Ancestor of {title1} & {title2} is {common.Name}");

            #endregion
        }

        private static void AddChildren(IHasTreeNodes node, OrganizationPosition parent, List<OrganizationPosition> organizationPositions)
        {
            var children = organizationPositions.Where(p => p.Path.GetAncestor(1) == parent.Path)
                                            .OrderBy(p => p.Path);

            foreach (var child in children)
            {
                var treeNode = node.AddNode(child.Name);
                AddChildren(treeNode, child, organizationPositions);
            }
        }
    }
}