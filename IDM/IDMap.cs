﻿namespace KryptonM.IDM;

public class IDMap<T> where T : IDHolder
{

    private readonly Queue<Action> delayedRegistry = new Queue<Action>();

    public List<T> IdList = new List<T>();
    public Dictionary<ID, T> IdMap = new Dictionary<ID, T>();

    public int NextId = 1;

    public T this[int index] => Get(index);
    public T this[ID idt] => Get(idt);
    public T this[string idt] => Get(idt);
    public T DefaultValue => IdList[0];

    public T RegisterDefaultValue(string idt, T o)
    {
        return RegisterDefaultValue(new ID(idt), o);
    }

    public T RegisterDefaultValue(ID idt, T o)
    {
        o.Uid = new ID(idt.Space, idt.Key, 0);
        IdList.Add(o);
        IdMap[o.Uid] = o;

        return o;
    }

    public T Register(string idt, T o)
    {
        return Register(new ID(idt), o);
    }

    public T Register(ID idt, T o)
    {
        o.Uid = new ID(idt.Space, idt.Key, NextId);
        IdList.Add(o);
        IdMap[o.Uid] = o;
        NextId++;

        return o;
    }

    public T Get(int id)
    {
        return IdList[id];
    }

    public T Get(in ID idt)
    {
        return IdMap.GetValueOrDefault(idt, DefaultValue);
    }

    public T Get(string key)
    {
        return Get(new ID(key));
    }

    public List<T> List()
    {
        return IdList;
    }

    public Dictionary<ID, T> Mapped()
    {
        return IdMap;
    }

}