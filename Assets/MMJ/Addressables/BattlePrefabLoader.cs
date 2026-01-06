using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BattlePrefabLoader
{
    private readonly List<AsyncOperationHandle<GameObject>> handles = new();
    private readonly List<GameObject> instances = new();

    public async UniTask<GameObject> LoadAndSpawn(string address, Transform parent)
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(address);
        await handle.Task;

        var instance = Object.Instantiate(handle.Result, parent);

        handles.Add(handle);
        instances.Add(instance);

        return instance;
    }

    public void ReleaseAll()
    {
        foreach (var instance in instances)
        {
            if (instance != null)
                Addressables.ReleaseInstance(instance);
        }

        foreach (var handle in handles)
        {
            if (handle.IsValid())
                Addressables.Release(handle);
        }

        instances.Clear();
        handles.Clear();
    }
}
