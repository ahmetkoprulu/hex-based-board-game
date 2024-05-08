using System;
using System.Collections.Generic;
using System.Linq;

public class DescendingComparer<T> : IComparer<T> where T : IComparable<T>
{
    public int Compare(T x, T y)
    {
        return y.CompareTo(x);
    }
}

public class AscendingComparer<T> : IComparer<T> where T : IComparable<T>
{
    public int Compare(T x, T y)
    {
        return x.CompareTo(y);
    }
}

public class PriorityQueue<TKey, TValue> : SortedDictionary<TKey, Queue<TValue>>
{
    public PriorityQueue(IComparer<TKey> keyComparer) : base(keyComparer) { }

    public void Enqueue(TKey priority, TValue value)
    {
        if (!ContainsKey(priority))
        {
            this[priority] = new Queue<TValue>();
        }
        this[priority].Enqueue(value);
    }

    public TValue Dequeue()
    {
        if (Count == 0)
        {
            throw new InvalidOperationException("The queue is empty.");
        }

        var maxPriority = Keys.First();
        var dequeuedValue = this[maxPriority].Dequeue();

        // Remove the key if the queue is empty for that priority
        if (this[maxPriority].Count == 0)
        {
            Remove(maxPriority);
        }

        return dequeuedValue;
    }

    public TValue Peek()
    {
        if (Count == 0)
        {
            throw new InvalidOperationException("The queue is empty.");
        }

        var maxPriority = Keys.First();
        return this[maxPriority].Peek();
    }
}