using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unity 직렬화(serialization)를 지원하기 위한 Dictionary wrapper 구조체.
/// 직렬화를 적용하기 위해서는 이 클래스를 상속한 서브클래스를 사용해야 한다.
/// </summary>
[Serializable]
public class SerializableDictionary<Key, Value> : ISerializationCallbackReceiver, IEnumerable<KeyValuePair<Key, Value>>
{
    [SerializeField]
    private List<Key> keys = new List<Key>();
    [SerializeField]
    private List<Value> values = new List<Value>();
    [NonSerialized]
    private Dictionary<Key, Value> dict = new Dictionary<Key, Value>();
    
    public void OnAfterDeserialize()
    {
        dict.Clear();
        for (int i = 0; i < keys.Count; i++)
            dict[keys[i]] = values[i];
    }

    public void OnBeforeSerialize()
    {
        // do nothing
    }

    public static implicit operator Dictionary<Key, Value>(SerializableDictionary<Key, Value> d)
    {
        return d.dict;
    }

    public Value this[Key key]
    {
        get
        {
            return dict[key];
        }
        set
        {
            if (!dict.ContainsKey(key))
            {
                keys.Add(key);
                values.Add(value);
            }
            else
            {
                values[keys.IndexOf(key)] = value;
            }
            dict[key] = value;
        }
    }

    IEnumerator<KeyValuePair<Key, Value>> IEnumerable<KeyValuePair<Key, Value>>.GetEnumerator()
    {
        return dict.GetEnumerator();
    }

    public IEnumerator GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<Key, Value>>)dict).GetEnumerator();
    }

    public Dictionary<Key, Value>.KeyCollection Keys
    {
        get { return dict.Keys; }
    }

    public Dictionary<Key, Value>.ValueCollection Values
    {
        get { return dict.Values; }
    }

    public bool ContainsKey(Key key)
    {
        return dict.ContainsKey(key);
    }

    public bool ContainsValue(Value value)
    {
        return dict.ContainsValue(value);
    }

    public bool TryGetValue(Key key, out Value value)
    {
        return dict.TryGetValue(key, out value);
    }

    public void Add(Key key, Value value)
    {
        if (!dict.ContainsKey(key))
        {
            keys.Add(key);
            values.Add(value);
            dict.Add(key, value);
        }
    }

    public void Clear()
    {
        dict.Clear();
        keys.Clear();
        values.Clear();
    }
}