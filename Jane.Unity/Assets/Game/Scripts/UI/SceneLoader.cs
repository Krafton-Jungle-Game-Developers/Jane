using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

namespace Jane.Unity.UI
{
    public class SceneLoader : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public async UniTaskVoid LoadSceneAsync(string sceneName)
        {
            await SceneManager.LoadSceneAsync(sceneName);
        }
    }
}
