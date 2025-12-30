using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private readonly Stack<PooledObject> objStack = new();

    private PooledObject prefab;
    private int poolSize = 15;

    public void CreatePool(PooledObject obj, int size = 15)
    {
        if (objStack.Count > 0)
        {
            foreach (var o in objStack)
            {
                Destroy(o.gameObject);
            }
            objStack.Clear();
        }
        
        poolSize = size;
        prefab = obj;
        
        for (int i = 0; i < size; i++)
        {
            var go = Instantiate(obj, transform);
            go.gameObject.SetActive(false);
            go.returnPool = this;
            objStack.Push(go);
        }
    }

    public PooledObject GetObject()
    {
        if (objStack.Count > 0) return objStack.Pop();
        
        var go = Instantiate(prefab, transform);
        go.returnPool = this;
        
        return go;

    }

    public void ReturnObject(PooledObject obj)
    {
        obj.gameObject.SetActive(false);
        objStack.Push(obj);
    }
    
    public void ClearPool()
    {
        foreach(var o in objStack) Destroy(o.gameObject);
        objStack.Clear();
    }
}
