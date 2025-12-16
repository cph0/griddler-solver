using Griddlers.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Griddlers.Library;

public class Tree
{
    public TreeNode Root { get; set; }

    public Tree()
    {
        Root = new TreeNode("Start");
    }

    public Tree(IEnumerable<Point> points)
    {
        Root = new TreeNode("Start", points);
    }
}

public class TreeNode
{
    //private List<TreeNode> Excludes = new List<TreeNode>() { }; 
    [JsonIgnore]
    public TreeNode? Parent { get; set; }
    public int CurrentNodePos { get; set; }

    public string Name { get; private set; }

    public Dictionary<(int, int), Point> Points { get; private set; }

    public List<TreeNode> Children { get; private set; }

    public TreeNode(string name)
    {
        Name = name;
        Points = new Dictionary<(int, int), Point>() { };
        Children = new List<TreeNode>() { };
    }

    public TreeNode(string name, IEnumerable<Point> points) : this(name)
    {
        SetNodes(points);
    }

    public TreeNode(Point grp, Dictionary<(int, int), Point> points) : this(Enum.GetName(grp.Action)!)
    {
        Points = points;
    }

    private bool NodeEqual(TreeNode node)
    {
        return Points.Count == node.Points.Count && !Points.Keys.Except(node.Points.Keys).Any();
    }

    public void SetNodes(IEnumerable<Point> points, IList<TreeNode>? excludes = null)
    {
        //if (excludes != null)
        //    Excludes = excludes;

        IEnumerable<TreeNode> AllNodes = (from p in points
                                          group p by p.Grp into grp
                                          select new TreeNode(grp.First(), grp.ToDictionary(k => (k.X, k.Y))));

        foreach (TreeNode Node in AllNodes)
        {
            bool Remove = false;

            foreach (TreeNode Exclude in Children)
            {
                if (Node.NodeEqual(Exclude))
                {
                    Remove = true;
                    Exclude.Name = $"{Exclude.Name}\n{Node.Name}";
                    break;
                }
            }

            if (!Remove)
                Children.Add(Node);
        }
    }
}
