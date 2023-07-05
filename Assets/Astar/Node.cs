namespace tcom.tools
{
    using System;

    public class Node
    {
        private tcom.tools.Node[] child;
        private int f;
        private int g;
        private int h;
        private int index;
        private tcom.tools.Node next;
        private tcom.tools.Node parent;
        private int x;
        private int y;

        public Node()
        {
            this.child = new tcom.tools.Node[8];
        }

        public Node(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.child = new tcom.tools.Node[8];
        }

        public tcom.tools.Node[] Child
        {
            get
            {
                return this.child;
            }
            set
            {
                this.child = value;
            }
        }

        public int F
        {
            get
            {
                return this.f;
            }
            set
            {
                this.f = value;
            }
        }

        public int G
        {
            get
            {
                return this.g;
            }
            set
            {
                this.g = value;
            }
        }

        public int H
        {
            get
            {
                return this.h;
            }
            set
            {
                this.h = value;
            }
        }

        public int Index
        {
            get
            {
                return this.index;
            }
            set
            {
                this.index = value;
            }
        }

        public tcom.tools.Node Next
        {
            get
            {
                return this.next;
            }
            set
            {
                this.next = value;
            }
        }

        public tcom.tools.Node Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                this.parent = value;
            }
        }

        public int X
        {
            get
            {
                return this.x;
            }
            set
            {
                this.x = value;
            }
        }

        public int Y
        {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;
            }
        }
    }
}

