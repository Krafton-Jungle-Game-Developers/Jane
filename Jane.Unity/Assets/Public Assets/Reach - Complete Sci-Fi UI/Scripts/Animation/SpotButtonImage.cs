using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Reach
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(AspectRatioFitter))]
    public class SpotButtonImage : MonoBehaviour
    {
        [Header("Resources")]
        [SerializeField] private ButtonManager buttonManager;
        [SerializeField] private Animator animator;
        [SerializeField] private Image image;
        [SerializeField] private AspectRatioFitter aspectRatioFitter;

        [Header("Settings")]
        [SerializeField] private float verticalRatio = 0.001f;
        [SerializeField] private float horizontalRatio = 1.7777f;

        void Start()
        {
            if (buttonManager == null) { return; }
            if (animator == null) { animator = GetComponent<Animator>(); }
            if (image == null) { image = GetComponent<Image>(); }
            if (aspectRatioFitter == null) { aspectRatioFitter = GetComponent<AspectRatioFitter>(); }

            buttonManager.onHover.AddListener(delegate { DoIn(); });
            buttonManager.onLeave.AddListener(delegate { DoOut(); });
            buttonManager.onSelect.AddListener(delegate { DoIn(); });
            buttonManager.onDeselect.AddListener(delegate { DoOut(); });

            animator.enabled = false;

            if (image.sprite != null)
            {
                Texture texture = image.sprite.texture;

                if (texture.width >= texture.height) { aspectRatioFitter.aspectRatio = horizontalRatio; }
                else { aspectRatioFitter.aspectRatio = verticalRatio; }
            }
        }

        public void DoIn()
        {
            animator.enabled = true;
            animator.CrossFade("In", 0.15f);

            StopCoroutine("DisableAnimator");
            StartCoroutine("DisableAnimator");
        }

        public void DoOut()
        {
            animator.enabled = true;
            animator.CrossFade("Out", 0.15f);

            StopCoroutine("DisableAnimator");
            StartCoroutine("DisableAnimator");
        }

        IEnumerator DisableAnimator()
        {
            yield return new WaitForSecondsRealtime(ReachInternalTools.GetAnimatorClipLength(animator, "SpotImage_Out"));
            animator.enabled = false;
        }
    }
}