using System.Collections.Generic;
using static Define;
using Random = System.Random;

public class Deck
{
    // int => CardCode
    public List<int> _cardList = new();

    public void RemoveCard(int code)
    {
        _cardList.Remove(code);
    }

    public void AddCard(int code)
    {
        _cardList.Add(code);
    }
    
    public void Shuffle()
    {
        int count = _cardList.Count;
        if (count < 2) return;
        Random rnd = new Random();
        for (int i = 0; i < 100; i++)
        {
            Swap(_cardList, rnd.Next(0, count), rnd.Next(0, count));
        }
    }
    
    public int[] Draw()
    {
        int[] selected = new int[3]; 
        if (_cardList.Count < 3)
        {
            Random rnd = new Random();
            
            for (int k = 0; k < 3; k++)
            {
                if (k > _cardList.Count - 1)
                {
                    var idx = rnd.Next(0, _cardList.Count);
                    selected[k] = _cardList[idx];
                }
                else
                {
                    selected[k] = _cardList[k];
                }
                
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                selected[i] = _cardList[i];
            }    
        }
    
        return selected;
    }



}
