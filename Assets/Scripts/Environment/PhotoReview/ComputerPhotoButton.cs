using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Environment.PhotoReview
{
    [RequireComponent(typeof(Button))]
    public class ComputerPhotoButton : MonoBehaviour
    {
        public Image Outline;
        public Image Photo;

        // Start is called before the first frame update
        void Start()
        {
            SelectPhoto(false);
        }

        public void SetImageSprite(Sprite newSprite)
        {
            Photo.sprite = newSprite;
            Photo.preserveAspect = true;
        }

        public void SelectPhoto(bool isSelected)
        {
            Outline.gameObject.SetActive(isSelected);
        }
    }
}
