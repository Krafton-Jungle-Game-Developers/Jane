using UnityEngine;

namespace Michsky.UI.Reach
{
    public class ExitGame : MonoBehaviour
    {
        public void Exit() 
        { 
            Application.Quit();
#if UNITY_EDITOR
            Debug.Log("<b>[Reach UI]</b> Exit function works in builds only.");
#endif
        }
    }
}