namespace tcom.tools
{
    using System;

    public class Stack
    {
        private tcom.tools.Stack next;
        private tcom.tools.Node node;

        public tcom.tools.Stack Next
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

        public tcom.tools.Node Node
        {
            get
            {
                return this.node;
            }
            set
            {
                this.node = value;
            }
        }
    }
}

