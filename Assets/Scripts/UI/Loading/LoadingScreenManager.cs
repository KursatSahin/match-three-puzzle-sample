using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Lean.Pool;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class LoadingScreenManager : MonoBehaviour
{
    [SerializeField] List<LeanGameObjectPool> _pools;
    // Start is called before the first frame update
    async void Start()
    {
        foreach (var pool in _pools)
        {
            await UniTask.WaitUntil(() => pool.Preload == pool.Despawned);
        }
        
        LoadGameScene();
    }
    
    private async void LoadGameScene()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(Strings.Scenes.Game);
        await UniTask.WaitUntil(() => asyncOperation.isDone);
    }
}
