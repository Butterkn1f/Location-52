using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace Environment.PhotoReview
{
    public class PhotoSelect : MonoBehaviour
    {
        [SerializeField] private GameObject _imageSpawnParent;
        [SerializeField] private GameObject _imagePrefab;
        [SerializeField] private GameObject _noImagesFoundText;
        [SerializeField] private List<GameObject> _spawnedImages;

        private int _numPhotosSelected;

        [SerializeField] private TextMeshProUGUI _photoCounter;


        // Start is called before the first frame update
        void Start()
        {
            List<Photo> photos = PhotoManager.Instance.Photos;

            // TODO: replace with actual photos in
            int numPictures = photos.Count;

            if (numPictures == 0)
            {
                _noImagesFoundText.SetActive(true);
            }
            else
            {
                _noImagesFoundText.SetActive(false);

                // Simulate loading photos in
                for (int i = 0; i < numPictures; i++)
                {
                    GameObject tempPhoto = Instantiate(_imagePrefab, _imageSpawnParent.transform);
                    tempPhoto.SetActive(false);
                    tempPhoto.GetComponent<Button>().onClick.RemoveAllListeners();
                    tempPhoto.GetComponent<Button>().onClick.AddListener(() => SelectPhoto(tempPhoto));

                    Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false, true);
                    byte[] bytes = File.ReadAllBytes(Application.dataPath + photos[i].TextureSaveName);
                    texture.LoadImage(bytes);

                    tempPhoto.GetComponent<ComputerPhotoButton>().SetImageSprite(texture);
                    tempPhoto.GetComponent<ComputerPhotoButton>().photoRef = photos[i];
                    _spawnedImages.Add(tempPhoto);
                }

                PhotoReviewManager.Instance._selectedImages = new List<GameObject>();
                _photoCounter.text = "Photos Selected: <b>" + PhotoReviewManager.Instance._selectedImages.Count + "/10";

                StartCoroutine(PhotoSpawn());
            }
        }

        public void SelectPhoto(GameObject selectedPhoto)
        {
            if (PhotoReviewManager.Instance._selectedImages.Contains(selectedPhoto))
            {
                // Photo is currently selected
                selectedPhoto.GetComponent<ComputerPhotoButton>().SelectPhoto(false);
                PhotoReviewManager.Instance._selectedImages.Remove(selectedPhoto);
                _photoCounter.text = "Photos Selected: <b>" + PhotoReviewManager.Instance._selectedImages.Count + "/10";
            }
            else
            {
                if (PhotoReviewManager.Instance._selectedImages.Count < 10)
                {
                    // Photo is currently selected
                    selectedPhoto.GetComponent<ComputerPhotoButton>().SelectPhoto(true);
                    PhotoReviewManager.Instance._selectedImages.Add(selectedPhoto);
                    _photoCounter.text = "Photos Selected: <b>" + PhotoReviewManager.Instance._selectedImages.Count + "/10";
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
            if (PhotoReviewManager.Instance._selectedImages.Count >= 1)
            {
                // Calculate score
                // Get the grade
                PhotoReviewManager.Instance.UploadPost();
                this.gameObject.SetActive(false);
            }
        }
    }
}
