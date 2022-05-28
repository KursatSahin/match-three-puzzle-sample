using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Lean.Pool;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace UI.Loading
{
    public class LoadingScreenManager : MonoBehaviour
    {
        [SerializeField] List<LeanGameObjectPool> _pools;
        
        async void Start()
        {
            // Wait until the loading screen create all pool objects
            foreach (var pool in _pools)
            {
                await UniTask.WaitUntil(() => pool.Preload == pool.Despawned);
            }
        
            LoadGameScene();
        }
    
        private async void LoadGameScene()
        {
            // Load game scene async
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(Strings.Scenes.Game);
            
            // Check if the asynchronous operation is set to complete before continuing
            await UniTask.WaitUntil(() => asyncOperation.isDone);
        }
    }
}
