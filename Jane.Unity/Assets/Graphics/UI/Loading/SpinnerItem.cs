using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    [ExecuteInEditMode]
    public class SpinnerItem : MonoBehaviour
    {
        [Header("Resources")]
        public List<Image> foreground = new();
        public List<Image> background = new();
        public Color spinnerColor = Color.white;

        public void UpdateValues()
        {
            for (int i = 0; i < foreground.Count; ++i)
            {
                Image currentImage = foreground[i];
                currentImage.color = spinnerColor;
            }

            for (int i = 0; i < background.Count; ++i)
            {
                Image currentImage = background[i];
                currentImage.color = new Color(spinnerColor.r, spinnerColor.g, spinnerColor.b, 0.08f);
            }
        }
    }
