namespace tcom.tools
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class AStart
    {
        private tcom.tools.Node close;
        public static tcom.tools.AStart g_inst;
        public float grdwid = 1f;
        public int height = 50;
        public int igrodis = 5;
        public int[] indexs;
        private tcom.tools.Node open;
        public Vector3[] result;
        private tcom.tools.Stack stack;
        public int width = 50;

        public AStart(int w, int h)
        {
            this.SetMapSize(w, h);
            g_inst = this;
        }

        private int CalcH(int sx, int sy, int dx, int dy)
        {
            return (Math.Abs((int) (sx - dx)) + Math.Abs((int) (sy - dy)));
        }

        private tcom.tools.Node CheckClose(int index)
        {
            tcom.tools.Node next = null;
            next = this.close.Next;
            while (next != null)
            {
                if (next.Index.Equals(index))
                {
                    return next;
                }
                next = next.Next;
            }
            return next;
        }

        private tcom.tools.Node CheckOpen(int index)
        {
            tcom.tools.Node next = null;
            next = this.open.Next;
            while (next != null)
            {
                if (next.Index.Equals(index))
                {
                    return next;
                }
                next = next.Next;
            }
            return next;
        }

        public List<tcom.tools.Node> FindPath(Vector3 pos0, Vector3 pos1)
        {
            return this.FindPath((int) (pos0.x / this.grdwid), (int) (pos0.z / this.grdwid), (int) (pos1.x / this.grdwid), (int) (pos1.z / this.grdwid));
        }

        private List<tcom.tools.Node> FindPath(int _sx, int _sy, int _dx, int _dy)
        {
            this.open = new tcom.tools.Node();
            this.close = new tcom.tools.Node();
            this.stack = new tcom.tools.Stack();
            Debug.Log("---");
            if (this.GetBlock(_sx, _sy) > 0)
            {
                Debug.LogWarning("初始点在阻挡点上");
                return new List<tcom.tools.Node>();
            }
               Debug.Log("---");
            if (this.GetBlock(_dx, _dy) > 0)
            {
                Debug.LogWarning("目标点在阻挡点上");
                this.GetNewPos(ref _dx, ref _dy);
                if (this.GetBlock(_dx, _dy) > 0)
                {
                    return new List<tcom.tools.Node>();
                }
            }
            Debug.Log("---");

            int x = _sx * 10;
            int y = _sy * 10;
            int num3 = _dx * 10;
            int num4 = _dy * 10;
            List<tcom.tools.Node> list = new List<tcom.tools.Node>();
            tcom.tools.Node bestnode = null;
            int index = this.GetIndex(num3, num4);
            tcom.tools.Node node = new tcom.tools.Node(x, y) {
                G = 0,
                H = this.CalcH(x, y, num3, num4)
            };
            node.F = node.H + node.G;
            node.Index = this.GetIndex(x, y);
            this.open.Next = node;
            int length = this.indexs.Length;
            while (length > 0)
            {
                length--;
                bestnode = this.GetBestNode();
                if ((bestnode == null) || bestnode.Index.Equals(index))
                {
                    break;
                }
                this.GenerateSuccessors(bestnode, num3, num4);
            }
            if (length <= 0)
            {
                Debug.LogWarning("no road");
                return list;
            }
            this.ShowPath(list, bestnode);
            return list;
        }

        private void GenerateSucc(tcom.tools.Node bestnode, int x, int y, int dx, int dy, int step)
        {
            int index = 0;
            int num = bestnode.G + step;
            int num2 = this.GetIndex(x, y);
            tcom.tools.Node successor = null;
            tcom.tools.Node node = this.CheckOpen(num2);
            if (node == null)
            {
                node = this.CheckClose(num2);
                if (node == null)
                {
                    successor = new tcom.tools.Node(x, y) {
                        Parent = bestnode,
                        G = num,
                        H = this.CalcH(x, y, dx, dy)
                    };
                    successor.F = num + successor.H;
                    successor.Index = num2;
                    this.Insert(successor);
                    index = 0;
                    while (index < 8)
                    {
                        if (bestnode.Child[index] == null)
                        {
                            break;
                        }
                        index++;
                    }
                    bestnode.Child[index] = successor;
                    return;
                }
                for (index = 0; index < 8; index++)
                {
                    if (bestnode.Child[index] == null)
                    {
                        break;
                    }
                }
            }
            else
            {
                index = 0;
                while (index < 8)
                {
                    if (bestnode.Child[index] == null)
                    {
                        break;
                    }
                    index++;
                }
                bestnode.Child[index] = node;
                if (num < node.G)
                {
                    node.Parent = bestnode;
                    node.G = num;
                    node.F = num + node.H;
                }
                tcom.tools.Node next = null;
                tcom.tools.Node node4 = null;
                tcom.tools.Node node5 = null;
                next = this.open.Next;
                node5 = this.open.Next;
                while ((next != null) && (next.Index != node.Index))
                {
                    node4 = next;
                    next = next.Next;
                }
                if (next.Index == node.Index)
                {
                    if (next == this.open.Next)
                    {
                        node5 = node5.Next;
                        this.open.Next = node5;
                    }
                    else
                    {
                        node4.Next = next.Next;
                    }
                }
                this.Insert(node);
                return;
            }
            bestnode.Child[index] = node;
            if (num < node.G)
            {
                node.Parent = bestnode;
                node.G = num;
                node.F = num + node.H;
                this.PropagateDown(node, step);
            }
        }

        private void GenerateSuccessors(tcom.tools.Node bestnode, int dx, int dy)
        {
            int y = 0;
            for (int i = 0; i < 8; i++)
            {
                int x;
                switch (i)
                {
                    case 0:
                        x = bestnode.X + 10;
                        y = bestnode.Y;
                        if (this.HasIndex(x, y))
                        {
                            this.GenerateSucc(bestnode, x, y, dx, dy, 10);
                        }
                        break;

                    case 1:
                        x = bestnode.X;
                        y = bestnode.Y + 10;
                        if (this.HasIndex(x, y))
                        {
                            this.GenerateSucc(bestnode, x, y, dx, dy, 10);
                        }
                        break;

                    case 2:
                        x = bestnode.X - 10;
                        y = bestnode.Y;
                        if (this.HasIndex(x, y))
                        {
                            this.GenerateSucc(bestnode, x, y, dx, dy, 10);
                        }
                        break;

                    case 3:
                        x = bestnode.X;
                        y = bestnode.Y - 10;
                        if (this.HasIndex(x, y))
                        {
                            this.GenerateSucc(bestnode, x, y, dx, dy, 10);
                        }
                        break;

                    case 4:
                        x = bestnode.X + 10;
                        y = bestnode.Y - 10;
                        if (this.HasIndex(x, y))
                        {
                            this.GenerateSucc(bestnode, x, y, dx, dy, 14);
                        }
                        break;

                    case 5:
                        x = bestnode.X + 10;
                        y = bestnode.Y + 10;
                        if (this.HasIndex(x, y))
                        {
                            this.GenerateSucc(bestnode, x, y, dx, dy, 14);
                        }
                        break;

                    case 6:
                        x = bestnode.X - 10;
                        y = bestnode.Y + 10;
                        if (this.HasIndex(x, y))
                        {
                            this.GenerateSucc(bestnode, x, y, dx, dy, 14);
                        }
                        break;

                    case 7:
                        x = bestnode.X - 10;
                        y = bestnode.Y - 10;
                        if (this.HasIndex(x, y))
                        {
                            this.GenerateSucc(bestnode, x, y, dx, dy, 14);
                        }
                        break;
                }
            }
        }

        private tcom.tools.Node GetBestNode()
        {
            tcom.tools.Node next = null;
            if (this.open.Next != null)
            {
                next = this.open.Next;
                this.open.Next = next.Next;
                next.Next = this.close.Next;
                this.close.Next = next;
            }
            return next;
        }

        public int GetBlock(int x, int z)
        {
            if (((x >= 0) && (z >= 0)) && ((x < this.width) && (z < this.height)))
            {
                return this.indexs[(x * this.height) + z];
            }
            return 0x3e8;
        }

        private int GetIndex(int x, int y)
        {
            return (((x / 10) * this.height) + (y / 10));
        }

        public static tcom.tools.AStart GetInst()
        {
            return g_inst;
        }

        private void GetNewPos(ref int _dx, ref int _dy)
        {
            for (int i = 1; i < this.igrodis; i++)
            {
                for (int j = _dx - i; j < (_dx + i); j++)
                {
                    for (int k = _dy - i; k < (_dy + i); k++)
                    {
                        if (this.GetBlock(j, k) == 0)
                        {
                            _dx = j;
                            _dy = k;
                            Debug.Log(((int) _dx).ToString() + " | " + ((int) _dy).ToString());
                            return;
                        }
                    }
                }
            }
        }

        private bool HasIndex(int x, int y)
        {
            int index = ((x / 10) * this.height) + (y / 10);
            if ((index < 0) || (index >= this.indexs.Length))
            {
                return false;
            }
            return this.indexs[index].Equals(0);
        }

        private void Inivate()
        {
            int num;
            this.open = new tcom.tools.Node();
            this.close = new tcom.tools.Node();
            this.stack = new tcom.tools.Stack();
            int index = 0;
            for (num = 0; num < (this.width * this.height); num++)
            {
                this.indexs[num] = 0;
            }
            for (num = 0; num < this.height; num++)
            {
                this.indexs[num] = 1;
            }
            for (num = 1; num < (this.width - 1); num++)
            {
                index += this.height;
                this.indexs[index] = 1;
                this.indexs[(index + this.height) - 1] = 1;
            }
            index += this.width - 1;
            for (num = index; num < (this.width * this.height); num++)
            {
                this.indexs[num] = 1;
            }
        }

        private void Insert(tcom.tools.Node successor)
        {
            tcom.tools.Node next = null;
            int f = 0;
            if (this.open.Next == null)
            {
                this.open.Next = successor;
            }
            else
            {
                f = successor.F;
                tcom.tools.Node open = this.open;
                next = this.open.Next;
                while ((next != null) && (next.F < f))
                {
                    open = next;
                    next = next.Next;
                }
                successor.Next = next;
                open.Next = successor;
            }
        }

        private tcom.tools.Node Pop()
        {
            tcom.tools.Node node = null;
            tcom.tools.Stack next = null;
            next = this.stack.Next;
            node = next.Node;
            this.stack.Next = next.Next;
            return node;
        }

        private void PropagateDown(tcom.tools.Node old, int step)
        {
            tcom.tools.Node node;
            int g = 0;
            tcom.tools.Node node2 = null;
            g = old.G;
            int index = 0;
            while (index < 8)
            {
                node = old.Child[index];
                if (node == null)
                {
                    break;
                }
                if ((g + step) < node.G)
                {
                    node.G = g + step;
                    node.F = node.G + node.H;
                    node.Parent = old;
                    this.Push(node);
                }
                index++;
            }
            while (this.stack.Next != null)
            {
                node2 = this.Pop();
                for (index = 0; index < 8; index++)
                {
                    node = node2.Child[index];
                    if (node == null)
                    {
                        continue;
                    }
                    if ((node2.G + step) < node.G)
                    {
                        node.G = node2.G + step;
                        node.F = node.G + node.H;
                        node.Parent = node2;
                        this.Push(node);
                    }
                }
            }
        }

        private void Push(tcom.tools.Node node)
        {
            tcom.tools.Stack stack = new tcom.tools.Stack {
                Node = node,
                Next = this.stack.Next
            };
            this.stack.Next = stack;
        }

        public void SetBlock(int x, int z, int val)
        {
            if (((x < 0) || (z < 0)) || ((x >= this.width) || (z >= this.height)))
            {
                Debug.LogError("wrong par x=" + x.ToString() + " z=" + z.ToString());
            }
            else
            {
                this.indexs[(x * this.height) + z] = val;
            }
        }

        public void SetMapSize(int w, int h)
        {
            this.width = w;
            this.height = h;
            this.indexs = new int[this.width * this.height];
            this.Inivate();
        }

        private void ShowPath(List<tcom.tools.Node> list, tcom.tools.Node bestnode)
        {
            if (bestnode != null)
            {
                if (bestnode.Parent != null)
                {
                    this.ShowPath(list, bestnode.Parent);
                }
                list.Add(bestnode);
            }
        }
    }
}

