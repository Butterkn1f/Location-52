using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Environment.PhotoReview
{
    [RequireComponent(typeof(Button))]
    public class ComputerPhotoButton : MonoBehaviour
    {
        public Image Outline;
        public Image Photo;

        // Start is called before the first frame update
        void Awake()
        {
            SelectPhoto(false);
        }

        public void SetImageSprite(Sprite newSprite)
        {
            Photo.sprite = newSprite;
            Photo.preserveAspect = true;
        }

        public void SetImageSprite(Texture2D newTexture2D)
        {
            Rect rec = new Rect(0, 0, newTexture2D.width, newTexture2D.height);
            Sprite newSprite = Sprite.Create(newTexture2D, rec, new Vector2(0, 0), 1);
            Photo.sprite = newSprite;
            Photo.preserveAspect = true;
        }

        public void SelectPhoto(bool isSelected)
        {
            Outline.gameObject.SetActive(isSelected);
        }
    }
}
