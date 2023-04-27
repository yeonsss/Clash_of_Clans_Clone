using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Utils;
using static Define;

public class PathVecter2Int
{
    public int X { get; set; } = -1;
    public int Y { get; set; } = -1;

    public PathVecter2Int(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }
}

struct PQNode : IComparable<PQNode>
{
    public float F;
    public float G;
    public int Y;
    public int X;

    public int CompareTo(PQNode other)
    {
        if ((F - other.F) < 0.1) return 0;
        return F < other.F ? 1 : -1;
    }
}

public class PathManager : Singleton<PathManager>
{
    private int PosY = 0;
    private int PosX = 0;
    private int DestY = 9;
    private int DestX = 9;
    public int  pathCost= 0;
    public List<PathVecter2Int> Root = new List<PathVecter2Int>();
    public List<GameObject> targetList = new List<GameObject>();

    public override void Init()
    {
        Debug.Log("��ã�� �Ŵ��� ����");
    }

    // If the decimal point of x and y is 0.5, width and height are even numbers.
    // If x and y are integers, height and width are odd numbers.
    // width and height are always the same.
    // x and y are always of the same form.
    private bool PosInSquareArea(float x, float y, int centerPosX, int centerPosY, int width, int height)
    {

        var minX = Mathf.Ceil(centerPosX - (width / 2));
        var maxX = Mathf.Floor(centerPosX + (width / 2));
        var minY = Mathf.Ceil(centerPosY - (height / 2));
        var maxY = Mathf.Floor(centerPosY + (height / 2));

        if (minX > x || maxX < x) return false;
        if (minY > y || maxY < y) return false;
        
        return true;
    }
    
    private bool PosInSquareArea(float x, float y, GameObject target)
    {
        var centerPos = target.transform.position;
        Wtor(centerPos.x, centerPos.z, (int)WIDTH, out var rx, out var ry);
        
        // TODO: 총 크기가 3이고 반지름은 1.5인데 왜 공격을 안하는가?
        
        var width = target.GetComponent<Build>().XSize + 2;
        var height = target.GetComponent<Build>().YSize + 2;
        
        var minX = Mathf.Ceil(rx - (width / 2));
        var maxX = Mathf.Floor(rx + (width / 2));
        var minY = Mathf.Ceil(ry - (height / 2));
        var maxY = Mathf.Floor(ry + (height / 2));

        if (minX > x || maxX < x) return false;
        if (minY > y || maxY < y) return false;
        
        return true;
    }
    
    public void CalcPathIgnoreWall(PathVecter2Int[,] parent, GameObject origin, GameObject target)
    {
        // 철거비용 계산해서 리턴
        Root.Clear();
        targetList.Clear();
        pathCost = 0;
        int y = DestY;
        int x = DestX;

        while (true)
        {
            if (parent[y, x] == null)
            {
                return;
            }

            if (parent[y, x].Y == y && parent[y, x].X == x) break;
            
            Root.Add(new PathVecter2Int(x, y));
            pathCost += 1;

            PathVecter2Int pos = parent[y, x];
            
            y = (int)pos.Y;
            x = (int)pos.X;
        }
        
        Root.Add(new PathVecter2Int(x, y));
        pathCost += 1;

        targetList.Reverse();
        targetList.Add(target);
        Root.Reverse();
    }
    
    // public void CalcPathRangeType(PathVecter2Int[,] parent, GameObject origin, GameObject target)
    // {
    //     // 철거비용 계산해서 리턴
    //     Root.Clear();
    //     targetList.Clear();
    //     pathCost = 0;
    //     int y = DestY;
    //     int x = DestX;
    //
    //     var attackRange = origin.GetComponent<MonsterController>().AttackRange;
    //
    //     while (true)
    //     {
    //         if (parent[y, x] == null)
    //         {
    //             return;
    //         }
    //
    //         if (parent[y, x].Y == y && parent[y, x].X == x) break;
    //
    //         var originPos = origin.transform.position;
    //         var targetPos = target.transform.position;
    //
    //         if ((targetPos - originPos).sqrMagnitude < Mathf.Pow(attackRange, 2))
    //         {
    //             pathCost += 0;
    //             targetList.Clear();
    //             targetList.Add(target);
    //             Root.Reverse();
    //             return;
    //         }
    //
    //         Root.Add(new PathVecter2Int(x, y));
    //         if (BattleManager.instance.wallMap[y, x] != null)
    //         {
    //             targetList.Add(BattleManager.instance.wallMap[y, x]);
    //             pathCost += BattleManager.instance.GetDestroyCost(origin, x, y);
    //         }
    //         else
    //         {
    //             pathCost += 1;
    //         }
    //
    //         PathVecter2Int pos = parent[y, x];
    //     
    //         y = (int)pos.Y;
    //         x = (int)pos.X;
    //         
    //     }
    //     
    //     Root.Add(new PathVecter2Int(x, y));
    //     if (BattleManager.instance.wallMap[y, x] != null)
    //     {
    //         targetList.Add(BattleManager.instance.wallMap[y, x]);   
    //         pathCost += BattleManager.instance.GetDestroyCost(origin, x, y);
    //     }
    //     else
    //     {
    //         pathCost += 1;
    //     }
    //
    //     targetList.Reverse();
    //     targetList.Add(target);
    //     Root.Reverse();
    // }

    public void CalcPath(PathVecter2Int[,] parent, GameObject origin, GameObject target)
    {
        // 철거비용 계산해서 리턴
        Root.Clear();
        targetList.Clear();
        pathCost = 0;
        int y = DestY;
        int x = DestX;

        while (true)
        {
            if (parent[y, x] == null)
            {
                return;
            }

            if (parent[y, x].Y == y && parent[y, x].X == x) break;
            
            Root.Add(new PathVecter2Int(x, y));
            if (BattleManager.instance.wallMap[y, x] != null)
            {
                targetList.Add(BattleManager.instance.wallMap[y, x]);
                pathCost += BattleManager.instance.GetDestroyCost(origin, x, y);
            }
            else
            {
                pathCost += 1;
            }

            PathVecter2Int pos = parent[y, x];
            
            y = (int)pos.Y;
            x = (int)pos.X;
        }
        
        Root.Add(new PathVecter2Int(x, y));
        if (BattleManager.instance.wallMap[y, x] != null)
        {
            targetList.Add(BattleManager.instance.wallMap[y, x]);   
            pathCost += BattleManager.instance.GetDestroyCost(origin, x, y);
        }
        else
        {
            pathCost += 1;
        }

        targetList.Reverse();
        targetList.Add(target);
        Root.Reverse();
    }

    // origin => monster, target => building
    public void AStarRoot(GameObject origin, GameObject target)
    {
        Debug.Log("Find Path");

        // TODO: 반올림
        var position = origin.transform.position;

        // var roundX = 0.0f;
        // var roundY = 0.0f;
        //
        // Wtor(roundX, roundY, (int)WIDTH, out var rx, out var ry);
        Wtor((int)position.x, (int)position.z, (int)WIDTH, out var rx, out var ry);
        PosY = ry;
        PosX = rx;
        
        var position1 = target.transform.position;
        Wtor((int)position1.x, (int)position1.z, (int)WIDTH, out var drx, out var dry);
        DestY = dry;
        DestX = drx;
        
        // int[] deltaY = { -1, 0, 1, 0 };
        // int[] deltaX = { 0, -1, 0, 1 };
        // float[] cost = { 1.0f, 1.0f, 1.0f, 1.0f };

        int[] deltaY = { -1, 0, 1, 0, 1, 1, -1, -1 };
        int[] deltaX = { 0, -1, 0, 1, 1, -1, 1, -1 };
        float[] cost = { 1.0f, 1.0f, 1.0f, 1.0f, 1.4f, 1.4f, 1.4f, 1.4f };
        
        PathVecter2Int[,] parent = new PathVecter2Int[(int)HEIGHT, (int)WIDTH];
        
        PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();
        
        bool[,] closed = new bool[(int)HEIGHT, (int)WIDTH];
        
        float[,] open = new float[(int)HEIGHT, (int)WIDTH];
        
        for (int y = 0; y<(int)HEIGHT; y++)
        {
            for (int x = 0; x < (int)WIDTH; x++)
            {
                open[y, x] = float.MaxValue;
            }
        }

        open[PosY, PosX] = Math.Abs(DestY - PosY) + Math.Abs(DestX - PosX);
        pq.Push(new PQNode()
        {
            F = Math.Abs(DestY - PosY) + Math.Abs(DestX - PosX) + BattleManager.instance.GetDestroyCost(origin, PosX, PosY), 
            G = 0, 
            X = PosX, 
            Y = PosY
        });
        parent[PosY, PosX] = new PathVecter2Int(PosX, PosY);
        
        
        while (pq.Count() > 0)
        {
            PQNode node = pq.Pop();
            if (closed[node.Y, node.X]) continue;
            closed[node.Y, node.X] = true;
            
            // TODO: 길찾기 알고리즘 체크
            // goal!! 
            if (node.Y == DestY && node.X == DestX) break;

            for (int i = 0; i < deltaY.Length; i++)
            {
                int nextY = node.Y + deltaY[i];
                int nextX = node.X + deltaX[i];

                if (
                    nextX < 0 || 
                    nextX >= WIDTH || 
                    nextY < 0 || 
                    nextY >= HEIGHT
                    ) continue;

                if (BattleManager.instance.CheckMovePossible(nextX, nextY, DestX, DestY) == false) continue;

                if (closed[nextY, nextX]) continue;
                
                float g = node.G + cost[i];
                // 이쪽에 조건 변경
                int h = Math.Abs(DestY - nextY) + Math.Abs(DestX - nextX) + BattleManager.instance.GetDestroyCost(origin, nextX, nextY);

                if (open[nextY, nextX] < g + h) continue;
    
                open[nextY, nextX] = g + h;
                pq.Push(new PQNode(){F = g + h, G = g, Y = nextY, X = nextX});
                parent[nextY, nextX] = new PathVecter2Int(node.X, node.Y);
            }
        }

        CalcPath(parent, origin, target);
    }
    
    public void AStarIgnoreWall(GameObject origin, GameObject target)
    {
        Debug.Log("Find Path");

        // TODO: 반올림
        var position = origin.transform.position;

        // var roundX = 0.0f;
        // var roundY = 0.0f;
        //
        // Wtor(roundX, roundY, (int)WIDTH, out var rx, out var ry);
        Wtor((int)position.x, (int)position.z, (int)WIDTH, out var rx, out var ry);
        PosY = ry;
        PosX = rx;
        
        var position1 = target.transform.position;
        Wtor((int)position1.x, (int)position1.z, (int)WIDTH, out var drx, out var dry);
        DestY = dry;
        DestX = drx;
        
        // int[] deltaY = { -1, 0, 1, 0 };
        // int[] deltaX = { 0, -1, 0, 1 };
        // float[] cost = { 1.0f, 1.0f, 1.0f, 1.0f };

        int[] deltaY = { -1, 0, 1, 0, 1, 1, -1, -1 };
        int[] deltaX = { 0, -1, 0, 1, 1, -1, 1, -1 };
        float[] cost = { 1.0f, 1.0f, 1.0f, 1.0f, 1.4f, 1.4f, 1.4f, 1.4f };
        
        PathVecter2Int[,] parent = new PathVecter2Int[(int)HEIGHT, (int)WIDTH];
        
        PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();
        
        bool[,] closed = new bool[(int)HEIGHT, (int)WIDTH];
        
        float[,] open = new float[(int)HEIGHT, (int)WIDTH];
        
        for (int y = 0; y<(int)HEIGHT; y++)
        {
            for (int x = 0; x < (int)WIDTH; x++)
            {
                open[y, x] = float.MaxValue;
            }
        }

        open[PosY, PosX] = Math.Abs(DestY - PosY) + Math.Abs(DestX - PosX);
        pq.Push(new PQNode()
        {
            F = Math.Abs(DestY - PosY) + Math.Abs(DestX - PosX), 
            G = 0, 
            X = PosX, 
            Y = PosY
        });
        parent[PosY, PosX] = new PathVecter2Int(PosX, PosY);

        while (pq.Count() > 0)
        {
            PQNode node = pq.Pop();
            if (closed[node.Y, node.X]) continue;
            closed[node.Y, node.X] = true;
            
            // TODO: 길찾기 알고리즘 체크
            // goal!! 
            if (node.Y == DestY && node.X == DestX) break;

            for (int i = 0; i < deltaY.Length; i++)
            {
                int nextY = node.Y + deltaY[i];
                int nextX = node.X + deltaX[i];

                if (
                    nextX < 0 || 
                    nextX >= WIDTH || 
                    nextY < 0 || 
                    nextY >= HEIGHT
                    ) continue;

                if (closed[nextY, nextX]) continue;
                
                float g = node.G + cost[i];
                int h = Math.Abs(DestY - nextY) + Math.Abs(DestX - nextX);

                if (open[nextY, nextX] < g + h) continue;
    
                open[nextY, nextX] = g + h;
                pq.Push(new PQNode(){F = g + h, G = g, Y = nextY, X = nextX});
                parent[nextY, nextX] = new PathVecter2Int(node.X, node.Y);
            }
        }

        CalcPathIgnoreWall(parent, origin, target);
    }
    
    public void AStarRootVerArea(GameObject origin, GameObject target, float range)
    {
        Debug.Log("Find Path");

        // TODO: 반올림
        var position = origin.transform.position;

        // var roundX = 0.0f;
        // var roundY = 0.0f;
        //
        // Wtor(roundX, roundY, (int)WIDTH, out var rx, out var ry);
        Wtor((int)position.x, (int)position.z, (int)WIDTH, out var rx, out var ry);
        PosY = ry;
        PosX = rx;
        
        var position1 = target.transform.position;
        Wtor((int)position1.x, (int)position1.z, (int)WIDTH, out var drx, out var dry);
        DestY = dry;
        DestX = drx;
        
        // int[] deltaY = { -1, 0, 1, 0 };
        // int[] deltaX = { 0, -1, 0, 1 };
        // float[] cost = { 1.0f, 1.0f, 1.0f, 1.0f };

        int[] deltaY = { -1, 0, 1, 0, 1, 1, -1, -1 };
        int[] deltaX = { 0, -1, 0, 1, 1, -1, 1, -1 };
        float[] cost = { 1.0f, 1.0f, 1.0f, 1.0f, 1.4f, 1.4f, 1.4f, 1.4f };
        
        PathVecter2Int[,] parent = new PathVecter2Int[(int)HEIGHT, (int)WIDTH];
        
        PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();
        
        bool[,] closed = new bool[(int)HEIGHT, (int)WIDTH];
        
        float[,] open = new float[(int)HEIGHT, (int)WIDTH];
        
        for (int y = 0; y<(int)HEIGHT; y++)
        {
            for (int x = 0; x < (int)WIDTH; x++)
            {
                open[y, x] = float.MaxValue;
            }
        }

        open[PosY, PosX] = Math.Abs(DestY - PosY) + Math.Abs(DestX - PosX);
        pq.Push(new PQNode()
        {
            F = Math.Abs(DestY - PosY) + Math.Abs(DestX - PosX) + BattleManager.instance.GetDestroyCost(origin, PosX, PosY), 
            G = 0, 
            X = PosX, 
            Y = PosY
        });
        parent[PosY, PosX] = new PathVecter2Int(PosX, PosY);
        
        
        while (pq.Count() > 0)
        {
            PQNode node = pq.Pop();
            if (closed[node.Y, node.X]) continue;
            closed[node.Y, node.X] = true;
            
            // TODO: 길찾기 알고리즘 체크
            // goal!! 
            if (node.Y == DestY && node.X == DestX) break;
            // if (Vector3.Distance(origin.transform.position, target.transform.position) <= range)
            // {
            //     break;
            // }

            for (int i = 0; i < deltaY.Length; i++)
            {
                int nextY = node.Y + deltaY[i];
                int nextX = node.X + deltaX[i];

                if (
                    nextX < 0 || 
                    nextX >= WIDTH || 
                    nextY < 0 || 
                    nextY >= HEIGHT
                    ) continue;

                if (BattleManager.instance.CheckMovePossible(nextX, nextY, DestX, DestY) == false) continue;

                if (closed[nextY, nextX]) continue;
                
                float g = node.G + cost[i];
                // 이쪽에 조건 변경
                int h = Math.Abs(DestY - nextY) + Math.Abs(DestX - nextX) + BattleManager.instance.GetDestroyCost(origin, nextX, nextY);

                if (open[nextY, nextX] < g + h) continue;
    
                open[nextY, nextX] = g + h;
                pq.Push(new PQNode(){F = g + h, G = g, Y = nextY, X = nextX});
                parent[nextY, nextX] = new PathVecter2Int(node.X, node.Y);
            }
        }

        CalcPath(parent, origin, target);
    }
}
