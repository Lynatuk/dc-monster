using System.Collections.Generic;
using UnityEngine;

public class PopupPool
{
    private readonly Stack<FxPopupController> _stack = new();
    private readonly FxPopupController _prefab;
    private readonly Transform _parent;

    public PopupPool(FxPopupController prefab, Transform parent, int preload = 8)
    {
        _prefab = prefab;
        _parent = parent;

        for (int i = 0; i < preload; i++)
        {
            _stack.Push(Object.Instantiate(_prefab, _parent));
        }

        foreach (var v in _stack)
        {
            v.gameObject.SetActive(false);
        }
    }

    public FxPopupController Get()
    {
        var v = _stack.Count > 0 ? _stack.Pop() : Object.Instantiate(_prefab, _parent);
        v.gameObject.SetActive(true);
        return v;
    }

    public void Release(FxPopupController v)
    {
        v.gameObject.SetActive(false);
        v.transform.SetParent(_parent, false);
        _stack.Push(v);
    }
}
