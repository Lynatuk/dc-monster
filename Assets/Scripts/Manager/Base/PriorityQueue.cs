using System;
using System.Collections;
using System.Collections.Generic;

public class PriorityQueue<TValue> : ICollection
{
    private readonly Dictionary<int, Queue<TValue>> _data;

    public int Count => _data.Keys.Count;

    public bool IsSynchronized => throw new NotImplementedException();

    public object SyncRoot => throw new NotImplementedException();

    public PriorityQueue() 
    {
        _data = new Dictionary<int, Queue<TValue>>();
    }
    
    public void Enqueue(int priority, TValue value) 
    {
        if (_data.ContainsKey(priority))
        {
            _data[priority].Enqueue(value);
        }
        else 
        {
            _data.Add(priority, new Queue<TValue>());
            _data[priority].Enqueue(value);
        }
    }

    public override bool Equals(object obj)
    {
        throw new Exception("Not override yet");
    }

    public TValue Dequeue()
    {
        int key = FoundMinimalPriority();
        TValue val = _data[key].Dequeue();
        if (_data[key].Count == 0) {

            _data.Remove(key);
        }
        
        return val;
    }
    
    private int FoundMinimalPriority()
    {
        int min = int.MaxValue;
        foreach (var key in _data.Keys)
        {
            if (min > key)
            {
                min = key;

            }
        }
        
        return min;
    }

    public override string ToString()
    {
        string str = "";
        foreach (var k in _data.Keys)
        {
            str += k + "  : \n";
            foreach (var elem in _data[k])
            {
                str += " " + elem + "  \n";

            }
            str += "\n";
        }
        
        return str;
    }

    public void CopyTo(Array array, int index)
    {
        throw new NotImplementedException();
    }

    public IEnumerator GetEnumerator()
    {
        throw new NotImplementedException();
    }
}