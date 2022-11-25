using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
public class TwoDirectionDictionary<T, U> : IEnumerable<KeyValuePair<T, U>>
{
    private ConcurrentDictionary<T, U> _forward = new ConcurrentDictionary<T, U>();
    private ConcurrentDictionary<U, T> _backward = new ConcurrentDictionary<U, T>();

    public TwoDirectionDictionary()
    {
        _forward = new ConcurrentDictionary<T, U>(_forward);
        _backward = new ConcurrentDictionary<U, T>(_backward);
    }
    public int Count()
    {
        return _forward.Count;
    }
    public ICollection<T> Keys
    {
        get
        {
            return _forward.Keys;
        }
    }
    public ICollection<U> Values
    {
        get
        {
            return _backward.Keys;
        }
    }
    public void Clear()
    {
        _forward.Clear();
        _backward.Clear();
    }
    public bool ContainsKey(T key)
    {
        return _forward.ContainsKey(key);
    }
    public bool ContainsKey(U key)
    {
        return _backward.ContainsKey(key);
    }
    public bool TryAdd(T a, U b)
    {
        if (a != null && b != null)
        {
            if (!_forward.ContainsKey(a) && !_backward.ContainsKey(b))
            {
                return _forward.TryAdd(a, b) && _backward.TryAdd(b, a);
            }
        }
        return false;
    }
    public bool TryAdd(U b, T a)
    {
        if (a != null && b != null)
        {
            if (!_forward.ContainsKey(a) && !_backward.ContainsKey(b))
            {
                return _forward.TryAdd(a, b) && _backward.TryAdd(b, a);
            }
        }
        return false;
    }
    public bool TryRemove(T a)
    {
        U b = default(U);
        return _forward.TryRemove(a, out b) && _backward.TryRemove(b, out a);
    }
    public bool TryRemove(U b)
    {
        T a = default(T);
        return _backward.TryRemove(b, out a) && _forward.TryRemove(a, out b);
    }

    public IEnumerator<KeyValuePair<T, U>> GetEnumerator()
    {
        return GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _forward.GetEnumerator();
    }

    public T this[U index]
    {
        get { return _backward.ContainsKey(index) ? _backward[index] : default(T); }
        set
        {
            if (_backward.ContainsKey(index))
            {
                _backward[index] = value;
            }
        }
    }
    public U this[T index]
    {
        get { return _forward.ContainsKey(index) ? _forward[index] : default(U); }
        set
        {
            if (_forward.ContainsKey(index))
            {
                _forward[index] = value;
            }
        }
    }
}
