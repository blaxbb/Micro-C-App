using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace micro_c_app.Models.Reference
{
    public class ReferenceTree : IReferenceItem
    {
        public string Name { get; set; }
        public List<IReferenceItem> Nodes { get; set; }

        public ReferenceTree()
        {
            Nodes = new List<IReferenceItem>();
        }

        public ReferenceTree(string name) : this()
        {
            Name = name;
        }

        public void SortNodes()
        {
            var trees = Nodes.Where(n => n is ReferenceTree).OrderBy(n => n.Name).ToList();
            var other = Nodes.Where(n => !(n is ReferenceTree)).OrderBy(n => n.Name).ToList();
            Nodes.Clear();
            Nodes.AddRange(trees);
            Nodes.AddRange(other);

            foreach(var t in trees)
            {
                if(t is ReferenceTree tree)
                {
                    tree.SortNodes();
                }
            }
        }

        public ReferenceTree CreateRoute(IEnumerable<string> path)
        {
            if (path.Count() > 1)
            {
                var name = path.ElementAt(0);
                var node = Nodes.FirstOrDefault(n => n.Name == name);
                if(node == null)
                {
                    var next = new ReferenceTree()
                    {
                        Name = name
                    };
                    Nodes.Add(next);
                    return next.CreateRoute(path.Skip(1));
                }
                if (node is ReferenceTree tree)
                {
                    return tree.CreateRoute(path.Skip(1));
                }
            }

            return this;
        }

        public IReferenceItem GetNode(IEnumerable<string> path)
        {
            var part = path.FirstOrDefault();
            if(part == null)
            {
                return this;
            }

            var node = Nodes.FirstOrDefault(n => n.Name == part);
            if(node == null)
            {
                return this;
            }

            if (node is ReferenceTree tree)
            {
                return tree.GetNode(path.Skip(1));
            }
            else
            {
                return node;
            }
        }
    }
}
