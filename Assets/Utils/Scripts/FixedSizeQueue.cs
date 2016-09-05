using UnityEngine;
using System.Collections.Generic;

public class FixedSizeQueue<T> : Queue<T> {

	private int limit = -1;

    public int Limit
    {
        get { return limit; }
        set { limit = value; }
    }

    public FixedSizeQueue(int limit) : base(limit)
    {
        this.Limit = limit;
    }

    public new T Enqueue(T item)
    {
        T head = default(T);
        if (this.Count >= this.Limit) {
            head = this.Dequeue();
        }
        base.Enqueue(item);
        return head;
    }
}
