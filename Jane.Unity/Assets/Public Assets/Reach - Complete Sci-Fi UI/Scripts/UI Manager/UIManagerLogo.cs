using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Reach
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [AddComponentMenu("Reach UI/UI Manager/UI Manager Logo")]
    public class UIManagerLogo : MonoBehaviour
    {
        [Header("Resources")]
        [SerializeField] private UIManager UIManagerAsset;
        private Image objImage;

        [Header("Settings")]
        [SerializeField] private LogoType logoType = LogoType.GameLogo;
        [SerializeField] private bool useCustomColor;
        [SerializeField] private bool useCustomAlpha;

        public enum LogoType { GameLogo, BrandLogo }

        void Awake()
        {
            this.enabled = true;

            if (UIManagerAsset == null) { UIManagerAsset = Resources.Load<UIManager>("Reach UI Manager"); }
            if (objImage == null) { objImage = GetComponent<Image>(); }
            if (UIManagerAsset.enableDynamicUpdate == false) { UpdateImage(); this.enabled = false; }
        }

        void Update()
        {
            if (UIManagerAsset == null) { return; }
            if (UIManagerAsset.enableDynamicUpdate == true) { UpdateImage(); }
        }


        void UpdateImage()
        {
            if (objImage == null || useCustomColor == true)
                return;

            if (logoType == LogoType.GameLogo) 
            { 
                objImage.sprite = UIManagerAsset.gameLogo;

                if (useCustomColor == true) { return; }
                if (useCustomAlpha == false) { objImage.color = UIManagerAsset.gameLogoColor; }
                else { objImage.color = new Color(UIManagerAsset.gameLogoColor.r, UIManagerAsset.gameLogoColor.g, UIManagerAsset.gameLogoColor.b, objImage.color.a); }
            }
            
            else if (logoType == LogoType.BrandLogo) 
            { 
                objImage.sprite = UIManagerAsset.brandLogo;

                if (useCustomColor == true) { return; }
                if (useCustomAlpha == false) { objImage.color = UIManagerAsset.brandLogoColor; }
                else { objImage.color = new Color(UIManagerAsset.brandLogoColor.r, UIManagerAsset.brandLogoColor.g, UIManagerAsset.brandLogoColor.b, objImage.color.a); }
            }
        }
    }
}