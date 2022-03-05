using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCompression
{
    class Node
    {
        Object data;
        List<Node> par;
        List<Node> ch;

        public Node(Object data, Node parent)
        {
            this.par = new List<Node>();
            this.ch = new List<Node>();
            this.data = data;
            parent.Children.Add(this);
        }

        public Node(Object data)
        {
            this.par = new List<Node>();
            this.ch = new List<Node>();
            this.data = data;
        }

        public Node()
        {
            this.par = new List<Node>();
            this.ch = new List<Node>();
            this.data = null;
        }

        public bool IsLeaf()
        {
            return Children.Count == 0;
        }

        public List<Node> Children
        {
            get => ch;
        }

        public void AddChild(Node child)
        {
            this.ch.Add(child);
        } 

        public Object Data
        {
            get => data;
            set => data = value;
        }

        public override String ToString()
        {
            return (data).ToString();
        }
        
    }
}