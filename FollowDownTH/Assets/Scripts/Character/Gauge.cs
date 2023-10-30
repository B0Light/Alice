using System;
using UnityEngine;

[System.Serializable]
public class Gauge<T> where T : IComparable<T> {

    [SerializeField] T _maxValue;
    [SerializeField] T _curValue;

    public T Value {
        get {
            return _curValue;
        }
        set {
            _curValue = value;
            if (_curValue.CompareTo(_maxValue) > 0) _curValue = _maxValue;
        }
    }

    public T MaxValue {
        get { return _maxValue; }
        set { _maxValue = value; _curValue = _maxValue; }
    }

    public Gauge()
    {
        _maxValue = default(T);
        _curValue = default(T);

    }

    public Gauge(T maxValue)
    {
        _maxValue = maxValue;
        _curValue = maxValue;
    }

    public Gauge(T maxValue, T curValue)
    {
        _maxValue = maxValue;
        _curValue = curValue;
    }

    public override string ToString()
    {
        return _curValue.ToString() + " / " + _maxValue.ToString();
    }

    
}
