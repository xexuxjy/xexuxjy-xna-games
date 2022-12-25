using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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


public class LongBounds
{
    private LongVector2 m_min;
    private LongVector2 m_max;
    private LongVector2 m_center;
    private long m_distance;
    public LongBounds(LongVector2 center,long x,long y)
    {
        m_center = center;
        m_min = new LongVector2(center.X-x,center.Y-y);
        m_max = new LongVector2(center.X+x,center.Y+y);
    }

    public bool Encapsulates(LongBounds bounds)
    {
        return m_min.X <= bounds.m_min.X &&
             m_min.Y <= bounds.m_min.Y &&
             m_max.X >= bounds.m_max.X &&
             m_max.Y >= bounds.m_max.Y;
    }

    public bool Intersects(LongBounds bounds)
    {
        return (m_min.X <= bounds.m_max.X) && (m_max.X >= bounds.m_min.X) &&
                (m_min.Y <= bounds.m_max.Y) && (m_max.Y >= bounds.m_min.Y);
    }

    public bool Contains(long x,long y)
    {
        return (x >= m_min.X && x <= m_max.X) && (y >= m_min.Y && y <= m_max.Y);
    }
}

