﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class FixedSizedQueue<T> : Queue<T>
{
    private readonly object syncObject = new object();

    public int Size { get; private set; }

    public FixedSizedQueue(int size)
    {
        Size = size;
    }

    public new void Enqueue(T obj)
    {
        base.Enqueue(obj);
        lock (syncObject)
        {
            while (base.Count > Size)
            {
                T outObj = base.Dequeue();
            }
        }
    }
}

