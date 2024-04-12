using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Environment.PhotoReview
{
    public class PhotoSelect : MonoBehaviour
    {
        [SerializeField] private GameObject _imageSpawnParent;
        [SerializeField] private GameObject _imagePrefab;
        [SerializeField] private List<GameObject> _spawnedImages;

        private List<GameObject> _selectedImages;
        private int _numPhotosSelected;

        [SerializeField] private TextMeshProUGUI _photoCounter;


        // Start is called before the first frame update
        void Start()
        {
            // TODO: replace with actual photos in
            int numPictures = 5;

            // Simulate loading photos in
            for (int i = 0; i < numPictures; i++)
            {
                GameObject tempPhoto = Instantiate(_imagePrefab, _imageSpawnParent.transform);
                tempPhoto.SetActive(false);
                tempPhoto.GetComponent<Button>().onClick.RemoveAllListeners();
                tempPhoto.GetComponent<Button>().onClick.AddListener(() => SelectPhoto(tempPhoto));
                _spawnedImages.Add(tempPhoto);
            }

            _selectedImages = new List<GameObject>();
            _photoCounter.text = "Photos Selected: <b>" + _selectedImages.Count + "/10";

            StartCoroutine(PhotoSpawn());
        }

        public void SelectPhoto(GameObject selectedPhoto)
        {
            if (_selectedImages.Contains(selectedPhoto))
            {
                // Photo is currently selected
                selectedPhoto.GetComponent<ComputerPhotoButton>().SelectPhoto(false);
                _selectedImages.Remove(selectedPhoto);
                _photoCounter.text = "Photos Selected: <b>" + _selectedImages.Count + "/10";
            }
            else
            {
                if (_selectedImages.Count < 10)
                {
                    // Photo is currently selected
                    selectedPhoto.GetComponent<ComputerPhotoButton>().SelectPhoto(true);
                    _selectedImages.Add(selectedPhoto);
                    _photoCounter.text = "Photos Selected: <b>" + _selectedImages.Count + "/10";
                }
            }
        }

        public IEnumerator PhotoSpawn()
        {
            for (int i = 0; i < _spawnedImages.Count; i++)
            {
                Sequence seq = DOTween.Sequence();
                _spawnedImages[i].SetActive(true);
                _spawnedImages[i].GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);
                seq.Append(_spawnedImages[i].GetComponent<RectTransform>().DOScale(1.05f, 0.75f));
                seq.Append(_spawnedImages[i].GetComponent<RectTransform>().DOScale(1f, 0.25f));

                yield return new WaitForSeconds(0.25f);
            }
        }

        public void UploadPhotos()
        {
            // Calculate score

            
            if (_selectedImages.Count >= 1)
            {
                // Calculate score
                // Get the grade
                PhotoReviewManager.Instance.PlayerResult = Grade.MEGA_VIRAL;
                PhotoReviewManager.Instance.CurrentPhotoReviewState.SetValue(PhotoReviewState.PHOTO_UPLOAD_SEQUENCE);
            }
        }
    }
}
