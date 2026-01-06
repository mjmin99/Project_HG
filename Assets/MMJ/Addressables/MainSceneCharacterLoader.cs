using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MainSceneCharacterLoader
{
    private readonly List<AsyncOperationHandle<GameObject>> handles = new();
    private readonly List<GameObject> instances = new();

    public async UniTask<GameObject> Spawn(string address, Vector3 pos, Transform parent)
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(address);
        await handle.Task;

        var obj = Object.Instantiate(handle.Result, pos, Quaternion.identity, parent);

        handles.Add(handle);
        instances.Add(obj);

        return obj;
    }

    public void ReleaseAll()
    {
        foreach (var inst in instances)
            Addressables.ReleaseInstance(inst);

        foreach (var handle in handles)
            if (handle.IsValid())
                Addressables.Release(handle);

        instances.Clear();
        handles.Clear();
    }
}
