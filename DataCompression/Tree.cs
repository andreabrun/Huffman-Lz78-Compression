using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCompression
{
    class Tree
    {
        private Node root;
        private int height;

        public Tree(Node root, int height)
        {
            this.root = root;
            this.height = height;
        }

        public Tree(Node root)
        {
            this.root = root;
            this.height = GetHeight(root);
        }

        public Tree()
        {
            this.root = new Node();
            this.height = 1;
        }

        public Node[] GetLeafs()
        {
            List<Node> res = new List<Node>();
            List<Node> tmp = new List<Node>();

            tmp.Add(root);
            while(tmp.Count > 0)
            {
                if(tmp.ElementAt(0).IsLeaf())
                {
                    res.Add(tmp.ElementAt(0));
                }
                else
                {
                    foreach(Node item in tmp.ElementAt(0).Children)
                    {
                        tmp.Add(item);
                    }
                }
                tmp.RemoveAt(0);
            }

            return res.ToArray();
        }

        public int Height
        {
            get => height;
        }

        public Node Root
        {
            get => root;
        }

        public String ToMatlab()
        {
            // tree structure
            String res = "nodes = [0";
            List<Node> list = new List<Node>();
            list.Add(root);
            int i = 1;
            while(list.Count > 0)
            {
                foreach(Node item in list.ElementAt(0).Children)
                {
                    res = res + " " + i;
                    list.Add(item);
                }
                i++;
                list.RemoveAt(0);
            }
            res += "];\n";

            // labels
            res += "[x,y] = treelayout(nodes);\ntreeplot(nodes)\nlb = [";
            list.Add(root);
            res += "\"" + root.Data.ToString() + "\"";
            while(list.Count > 0)
            {
                foreach(Node item in list.ElementAt(0).Children)
                {
                    res = res + " \"" + item.Data.ToString() + "\"";
                    list.Add(item);
                }
                list.RemoveAt(0);
            }
            res += "];\n";
            res += "for i=1:length(x)\n\ttext(x(i),y(i),lb(i))\nend\n";
            return res;
        }

        public override string ToString()
        {
            return "Tree Height: " + GetHeight(root);
        }

        private static int GetHeight(Node root)
        {
            int h = 0;
            if(!root.IsLeaf())
            {
                int t = 0;
                foreach (Node item in root.Children)
                {
                    t = GetHeight(item);
                    if(t > h) h = t;
                }
            }
            return h + 1;
        }
    }
}