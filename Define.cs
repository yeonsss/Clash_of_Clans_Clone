using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public const float WIDTH = 50.0f;
    public const float HEIGHT = 50.0f;
    public const float Y_MIN = -1 * (int)(HEIGHT / 2);
    public const float Y_MAX = (int)(HEIGHT / 2);
    public const float X_MIN = -1 * (int)(WIDTH / 2);
    public const float X_MAX = (int)(WIDTH / 2);
    
    public enum AreaType
    {
        Empty = 0,
        NoSpawnArea = 1,
        Building = 2,
        Collision = 3,
        UnCollision = 4,
        None
    }
    
    public enum ResourceType
    {
        Gold,
        // Credit,
        // Elixir,
        None
    }
    
    public enum BuildType
    {
        Wall,
        NonWall,
        None
    }
    
    public enum CardType
    {
        BuildCard,
        MagicCard,
        MonsterCard
    }
    
    public enum UI
    {
        HANDUI,
        GAMEUI,
        DECKUI,
        BATTLEUI,
        None
    }
    
    public enum DataClassType
    {
        Building,
        Monster
    }
    
    public enum BuildProcess
    {
        Batch,
        Cool,
        Complete,
    }
    
    public enum InputEventType
    {
        MouseDown,
        MouseDrag,
        MouseUp,
        None
    }
    
    public static bool Rtow(int x, int y, int max, out float wx, out float wy) {
        try
        {
            wy = (max / 2) - y;
            wx = x - (max / 2);
            return true;
        }
        catch
        {
            wy = 0;
            wx = 0;
            return false;
        }
    }

    public static bool Wtor(float x, float y, int max, out int rx, out int ry) {
        try
        {
            ry = (int)((int)(max / 2) - y);
            rx = (int)(x + (int)(max / 2));
            return true;
        }
        catch
        {
            ry = 0;
            rx = 0;
            return false;
        }
    }
    
    public static void Swap<T>(IList<T> source, int one, int two)
    {
        T temp = source[one];
        source[one] = source[two];
        source[two] = temp;
    }
    
}
