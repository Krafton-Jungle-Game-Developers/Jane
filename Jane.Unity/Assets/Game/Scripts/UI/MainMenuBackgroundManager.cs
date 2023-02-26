using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Jane.Unity.UI
{
    [ExecuteInEditMode]
    public class MainMenuBackgroundManager : MonoBehaviour
    {
        [SerializeField] private RawImage backgroundVideoImage;
        [SerializeField] private VideoPlayer backgroundVideoPlayer;
        [SerializeField] private Color backgroundTintColor = new(160f, 225f, 255f, 255f);
        [SerializeField] private float videoPlaySpeed = 1.25f;
        
        [Space(0.1f)]
        [SerializeField] private ParticleSystem backgroundParticle;
        [SerializeField] private Color backgroundParticleTintColor = new(160f, 225f, 255f, 255f);

        private void LateUpdate()
        {
            UpdateBackground();
        }

        private void UpdateBackground()
        {
            backgroundVideoPlayer.enabled = true;
            backgroundVideoImage.enabled = true;

            backgroundVideoImage.color = backgroundTintColor;
            backgroundVideoPlayer.playbackSpeed = videoPlaySpeed;
        }

        private void UpdateParticleColor()
        {
            var main = backgroundParticle.main;
            main.startColor = new Color(backgroundParticleTintColor.r,
                                        backgroundParticleTintColor.g,
                                        backgroundParticleTintColor.b, 
                                        main.startColor.color.a);
        }
    }
}
