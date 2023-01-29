using System;
using BehaviorTree;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIComponent
{
    public Node Root { get; set; }
    
    public enum COOLID
    {
        attack = 0,
        skill = 1,
        end
    }

    public float[] CurrentCoolTime = Enumerable.Repeat<float>(0, (int)COOLID.end).ToArray<float>();
    public Dictionary<COOLID, bool> CoolCheck = new() {};
    public Dictionary<COOLID, float> CoolTime = new() { };
    public Dictionary<string, object> _dataContext = new Dictionary<string, object>();
    public MonsterController monsterData;

    public AIComponent(MonsterController data)
    {
        monsterData = data ;
        SetCoolTime();
    }

    public void SetTree(Node node)
    {
        Root = node;
    }

    public void Generate()
    {
        if (Root != null)
        {
            CoolChecker();
            Root.Evaluate();
        }
    }
    
    private void SetCoolTime()
    {
        CoolCheck.Add(COOLID.attack, false);
        CoolTime.Add(COOLID.attack, monsterData.AttackCooldown);
        
        CoolCheck.Add(COOLID.skill, false);
        CoolTime.Add(COOLID.skill, monsterData.SkillCooldown);
    }
    
    public void SetCoolFalse(COOLID id)
    {
        CoolCheck[id] = false;
    }

    public void CoolChecker()
    {
        float time = UnityEngine.Time.deltaTime;
        //printtime(CurrentCoolTime[0]);
        //printtime(CoolCheck[COOLID.attack]);
        for (int i = 0; i < CurrentCoolTime.Length; i++)
        {
            if (CoolCheck[(COOLID)i] == true) continue;

            if (CurrentCoolTime[i] + time >= CoolTime[(COOLID)i])
            {
                CoolCheck[(COOLID)i] = true;
                CurrentCoolTime[i] = 0;
            }
            else
            {
                CurrentCoolTime[i] += time;
            }
        }
    }

    public void SetData(string key, object value)
    {
        _dataContext[key] = value;
    }

    public bool CheckData(string key)
    {
        if (_dataContext.ContainsKey(key))
        {
            if (_dataContext.TryGetValue(key, out var value) == true)
            {
                return true;    
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public object GetData(string key)
    {
        object value = null;
        if (_dataContext.TryGetValue(key, out value))
            return value;

        return null;
    }

    public bool ClearData(string key)
    {
        if (_dataContext.ContainsKey(key))
        {
            _dataContext.Remove(key);
            return true;
        }

        return false;
    }
}
