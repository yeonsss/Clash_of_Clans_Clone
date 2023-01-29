using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utils;
using static Define;

public class PathVecter2Int
{
    public int x { get; set; } = -1;
    public int y { get; set; } = -1;

    public PathVecter2Int(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

struct PQNode : IComparable<PQNode>
{
    public int F;
    public int G;
    public int Y;
    public int X;

    public int CompareTo(PQNode other)
    {
        if (F == other.F) return 0;
        return F < other.F ? 1 : -1;
    }
}

public class PathManager : Singleton<PathManager>
{
    private int PosY = 0;
    private int PosX = 0;
    private int DestY = 9;
    private int DestX = 9;
    public List<PathVecter2Int> Root = new List<PathVecter2Int>();

    public void Init()
    {
        Debug.Log("��ã�� �Ŵ��� ����");
    }

    public bool CalcPathFromParent(PathVecter2Int[,] parent)
    {
        Root.Clear();
        int y = DestY;
        int x = DestX;

        while (true)
        {
            if (parent[y, x] == null)
            {
                Root.Clear();
                return false;
            }

            if (parent[y, x].y == y && parent[y, x].x == x) break;
            
            Root.Add(new PathVecter2Int(x, y));
            PathVecter2Int pos = parent[y, x];
            
            y = (int)pos.y;
            x = (int)pos.x;
        }
        Root.Add(new PathVecter2Int(y, x));
        Root.Reverse();
        return true;
    }
    
    public bool AStarPath(int crx, int cry, int desx, int desy, Vector2Int? ignoreWall)
    {
        PosY = cry;
        PosX = crx;
        DestY = desy;
        DestX = desx;
    
        // �̵� ����
        int[] deltaY = new int[] { -1, 0, 1, 0 };
        int[] deltaX = new int[] { 0, -1, 0, 1 };
        int[] cost = new int[] { 1, 1, 1, 1 };

        PathVecter2Int[,] parent = new PathVecter2Int[(int)HEIGHT, (int)WIDTH];
        ;
        // �켱���� ť
        // ���� ����Ʈ�� �ִ� ���� �� ���� ���� �ĺ��� ������ �̾Ƴ��� ���� ����
        PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();
        
        // ���� �ű��
        // F = G + H
        // F = ���� ���� (���� ���� ����, ��ο� ���� �޶���)
        // G = ���������� �ش� ��ǥ���� �̵��ϴ� �� ��� ��� (���� ���� ����, �̵��� ��ο� ���� �޶���)
        // H = ���������� �󸶳� ������� (���� ���� ����, ������ ��)
        // H�� ����� �� �߿��� ���� ���� �ִٰ� �������� �ʰ� ���������� �󸶳� ������� ����ϴ� ���̴�.
    
        // (y, x) �̹� �湮�ߴ��� ���� (�湮 = closed ����)
        // �̵��� �� ���� ��ǥ���� �ٽ� ��ã�⸦ �����ϴµ�
        // �׷��ٸ� �̹� �Դٴ� �� �� ����� ������ ��Ʈ�ϴ� ���̹Ƿ� �ٽô� ���� �ʴ´�.
        // �� close��Ŵ
        bool[,] closed = new bool[(int)HEIGHT, (int)WIDTH]; // Close List
    
        // (y, x) ���� ���� �� ���̶� �߰��ߴ� ��
        // �߰�X => MaxValue
        // �߰�O => F = G + H
        int[,] open = new int[(int)HEIGHT, (int)WIDTH];
        
        // �ִ� ������ ���� �ʱ�ȭ
        for (int y = 0; y<(int)HEIGHT; y++)
        {
            for (int x = 0; x < (int)WIDTH; x++)
            {
                open[y, x] = Int32.MaxValue;
            }
        }
    
        // ������ �߰� �� ��ã�� ����(���� ����)
        // ���� ��ǥ�� ���(G)�� 0�̴�.
        // H => Math.Abs(DestY - PosY) + Math.Abs(DestX - PosX);
        
        open[PosY, PosX] = Math.Abs(DestY - PosY) + Math.Abs(DestX - PosX);
        pq.Push(new PQNode(){F = Math.Abs(DestY - PosY) + Math.Abs(DestX - PosX), G = 0, X = PosX, Y = PosY});
        parent[PosY, PosX] = new PathVecter2Int(PosX, PosY);
        
        
        while (pq.Count() > 0)
        {
            // �켱���� ť�� F������ ���� ���� ģ���� �̾Ƴ���.
            PQNode node = pq.Pop();
            
            // ������ ��ǥ�� ���� ��η� ã�µ� �̹� �� ���� ��η� ���ؼ� �湮 �� ��� ��ŵ
            if (closed[node.Y, node.X]) continue;
            // �湮�Ѵ�.
            closed[node.Y, node.X] = true;
            // ������ ���� �� ����
            if (node.Y == DestY && node.X == DestX) break;
            
            // �����¿� �� �̵��� �� �ִ� ��ǥ���� Ȯ���ؼ� ����(open)�Ѵ�.
            for (int i = 0; i < deltaY.Length; i++)
            {
                int nextY = node.Y + deltaY[i];
                int nextX = node.X + deltaX[i];
                
                // �� ��ȿ���� üũ
                if (
                    nextX < 0 || 
                    nextX >= WIDTH || 
                    nextY < 0 || 
                    nextY >= HEIGHT
                    ) continue;
    
                var ignoreX = ignoreWall?.x ?? -1;
                var ignoreY = ignoreWall?.y ?? -1;
                    
                if (nextX != ignoreX || nextY != ignoreY)
                {
                    if (BattleManager.instance.CheckMovePossible(nextX, nextY) == false) continue;    
                }
    
                // �̹� �湮�� ���̸� ��ŵ
                if (closed[nextY, nextX]) continue;
    
                // ��� ���
                int g = node.G + cost[i];
                int h = Math.Abs(DestY - nextY) + Math.Abs(DestX - nextX) ;
                // �ٸ� ��ο��� �� ���� ���� �̹� ã������ ��ŵ
    
                if (open[nextY, nextX] < g + h) continue;
    
                open[nextY, nextX] = g + h;
                pq.Push(new PQNode(){F = g + h, G = g, Y = nextY, X = nextX});
                parent[nextY, nextX] = new PathVecter2Int(node.X, node.Y);
            }
        }
    
        return CalcPathFromParent(parent);
    }
}
