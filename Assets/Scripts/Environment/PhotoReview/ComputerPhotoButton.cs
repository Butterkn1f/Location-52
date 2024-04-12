using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Environment.PhotoReview
{
    [RequireComponent(typeof(Button))]
    public class ComputerPhotoButton : MonoBehaviour
    {
        public Image Outline;
        public Image Photo;

        [System.Serializable]
        public class ComputerStars
        {
            public Image StarImage;
            public TextMeshProUGUI StarText;
        }

        // Stars
        [Header("Stars")]
        [SerializeField] private List<ComputerStars> starList; 

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
