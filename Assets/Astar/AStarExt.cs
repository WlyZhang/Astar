using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using tcom.tools;

static public class AStarExt
{
    class AStarData
    {
        public Node close;
        public Node open;
        public tcom.tools.Stack stack;
    }

    static public List<Node> TSFindPath(this AStart astar, Vector3 pos0, Vector3 pos1)
    {
        return astar.TSFindPath(new AStarData(), (int)(pos0.x / astar.grdwid), (int)(pos0.z / astar.grdwid), (int)(pos1.x / astar.grdwid), (int)(pos1.z / astar.grdwid));
    }

    static private void TSGetNewPos(this AStart astar, ref int _dx, ref int _dy)
    {
        for (int i = 1; i < astar.igrodis; i++)
        {
            for (int j = _dx - i; j < (_dx + i); j++)
            {
                for (int k = _dy - i; k < (_dy + i); k++)
                {
                    if (astar.GetBlock(j, k) == 0)
                    {
                        _dx = j;
                        _dy = k;
                        Debug.Log(((int)_dx).ToString() + " | " + ((int)_dy).ToString());
                        return;
                    }
                }
            }
        }
    }

    static private int TSGetIndex(this AStart astar, int x, int y)
    {
        return (((x / 10) * astar.height) + (y / 10));
    }

    static private bool TSHasIndex(this AStart astar, int x, int y)
    {
        int index = ((x / 10) * astar.height) + (y / 10);
        if ((index < 0) || (index >= astar.indexs.Length))
        {
            return false;
        }
        return astar.indexs[index].Equals(0);
    }


    static private int TSCalcH(this AStart astar, int sx, int sy, int dx, int dy)
    {
        return (System.Math.Abs((int)(sx - dx)) + System.Math.Abs((int)(sy - dy)));
    }

    static private Node TSGetBestNode(this AStart astar, AStarData data)
    {
        Node next = null;
        if (data.open.Next != null)
        {
            next = data.open.Next;
            data.open.Next = next.Next;
            next.Next = data.close.Next;
            data.close.Next = next;
        }
        return next;
    }

    static private void TSInsert(this AStart astar, AStarData data, Node successor)
    {
        Node next = null;
        int f = 0;
        if (data.open.Next == null)
        {
            data.open.Next = successor;
        }
        else
        {
            f = successor.F;
            Node open = data.open;
            next = data.open.Next;
            while ((next != null) && (next.F < f))
            {
                open = next;
                next = next.Next;
            }
            successor.Next = next;
            open.Next = successor;
        }
    }

    static private Node TSCheckClose(this AStart astar, AStarData data, int index)
    {
        Node next = null;
        next = data.close.Next;
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

    static private Node TSCheckOpen(this AStart astar, AStarData data, int index)
    {
        Node next = null;
        next = data.open.Next;
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

    static private Node TSPop(this AStart astar, AStarData data)
    {
        Node node = null;
        tcom.tools.Stack next = null;
        next = data.stack.Next;
        node = next.Node;
        data.stack.Next = next.Next;
        return node;
    }

    static private void TSPush(this AStart astar, AStarData data, Node node)
    {
        tcom.tools.Stack stack = new tcom.tools.Stack
        {
            Node = node,
            Next = data.stack.Next
        };
        data.stack.Next = stack;
    }

    static private void TSPropagateDown(this AStart astar, AStarData data, Node old, int step)
    {
        Node node;
        int g = 0;
        Node node2 = null;
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
                astar.TSPush(data, node);
            }
            index++;
        }
        while (data.stack.Next != null)
        {
            node2 = astar.TSPop(data);
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
                    astar.TSPush(data,node);
                }
            }
        }
    }


    static private void TSGenerateSucc(this AStart astar, AStarData data, Node bestnode, int x, int y, int dx, int dy, int step)
    {
        int index = 0;
        int num = bestnode.G + step;
        int num2 = astar.TSGetIndex(x, y);
        Node successor = null;
        Node node = astar.TSCheckOpen(data, num2);
        if (node == null)
        {
            node = astar.TSCheckClose(data, num2);
            if (node == null)
            {
                successor = new Node(x, y)
                {
                    Parent = bestnode,
                    G = num,
                    H = astar.TSCalcH(x, y, dx, dy),
                    F = num + astar.TSCalcH(x, y, dx, dy),
                    Index = num2
                };
                astar.TSInsert(data, successor);
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
            Node next = null;
            Node node4 = null;
            Node node5 = null;
            next = data.open.Next;
            node5 = data.open.Next;
            while ((next != null) && (next.Index != node.Index))
            {
                node4 = next;
                next = next.Next;
            }
            if (next.Index == node.Index)
            {
                if (next == data.open.Next)
                {
                    node5 = node5.Next;
                    data.open.Next = node5;
                }
                else
                {
                    node4.Next = next.Next;
                }
            }
            astar.TSInsert(data,node);
            return;
        }
        bestnode.Child[index] = node;
        if (num < node.G)
        {
            node.Parent = bestnode;
            node.G = num;
            node.F = num + node.H;
            astar.TSPropagateDown(data, node, step);
        }
    }

    static private void TSGenerateSuccessors(this AStart astar, AStarData data, Node bestnode, int dx, int dy)
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
                    if (astar.TSHasIndex(x, y))
                    {
                        astar.TSGenerateSucc(data, bestnode, x, y, dx, dy, 10);
                    }
                    break;

                case 1:
                    x = bestnode.X;
                    y = bestnode.Y + 10;
                    if (astar.TSHasIndex(x, y))
                    {
                        astar.TSGenerateSucc(data, bestnode, x, y, dx, dy, 10);
                    }
                    break;

                case 2:
                    x = bestnode.X - 10;
                    y = bestnode.Y;
                    if (astar.TSHasIndex(x, y))
                    {
                        astar.TSGenerateSucc(data, bestnode, x, y, dx, dy, 10);
                    }
                    break;

                case 3:
                    x = bestnode.X;
                    y = bestnode.Y - 10;
                    if (astar.TSHasIndex(x, y))
                    {
                        astar.TSGenerateSucc(data, bestnode, x, y, dx, dy, 10);
                    }
                    break;

                case 4:
                    x = bestnode.X + 10;
                    y = bestnode.Y - 10;
                    if (astar.TSHasIndex(x, y))
                    {
                        astar.TSGenerateSucc(data, bestnode, x, y, dx, dy, 14);
                    }
                    break;

                case 5:
                    x = bestnode.X + 10;
                    y = bestnode.Y + 10;
                    if (astar.TSHasIndex(x, y))
                    {
                        astar.TSGenerateSucc(data, bestnode, x, y, dx, dy, 14);
                    }
                    break;

                case 6:
                    x = bestnode.X - 10;
                    y = bestnode.Y + 10;
                    if (astar.TSHasIndex(x, y))
                    {
                        astar.TSGenerateSucc(data, bestnode, x, y, dx, dy, 14);
                    }
                    break;

                case 7:
                    x = bestnode.X - 10;
                    y = bestnode.Y - 10;
                    if (astar.TSHasIndex(x, y))
                    {
                        astar.TSGenerateSucc(data, bestnode, x, y, dx, dy, 14);
                    }
                    break;
            }
        }
    }

    static private void TSShowPath(this AStart astar, List<Node> list, Node bestnode)
    {
        if (bestnode != null)
        {
            if (bestnode.Parent != null)
            {
                astar.TSShowPath(list, bestnode.Parent);
            }
            list.Add(bestnode);
        }
    }


    static private List<Node> TSFindPath(this AStart astar, AStarData data, int _sx, int _sy, int _dx, int _dy)
    {
        Node node;
        data.open = new Node();
        data.close = new Node();
        data.stack = new tcom.tools.Stack();
        if (astar.GetBlock(_sx, _sy) > 0)
        {
            Debug.LogWarning("初始点在阻挡点上");
            return new List<Node>();
        }
        if (astar.GetBlock(_dx, _dy) > 0)
        {
            Debug.LogWarning("目标点在阻挡点上");
            astar.TSGetNewPos(ref _dx, ref _dy);
            if (astar.GetBlock(_dx, _dy) > 0)
            {
                return new List<Node>();
            }
        }
        int x = _sx * 10;
        int y = _sy * 10;
        int num3 = _dx * 10;
        int num4 = _dy * 10;
        List<Node> list = new List<Node>();
        Node bestnode = null;
        int index = astar.TSGetIndex(num3, num4);

        var valG = 0;
        var valH = astar.TSCalcH(x, y, num3, num4);
        node = new Node(x, y)
        {
            G = valG,
            H = valH,
            F = valG + valH,
            Index = astar.TSGetIndex(x, y)
        };
        data.open.Next = node;
        int length = astar.indexs.Length;
        while (length > 0)
        {
            length--;
            bestnode = astar.TSGetBestNode(data);
            if ((bestnode == null) || bestnode.Index.Equals(index))
            {
                break;
            }
            astar.TSGenerateSuccessors(data, bestnode, num3, num4);
        }
        if (length <= 0)
        {
            Debug.LogWarning("no road");
            return list;
        }
        astar.TSShowPath(list, bestnode);
        return list;
    }
}
