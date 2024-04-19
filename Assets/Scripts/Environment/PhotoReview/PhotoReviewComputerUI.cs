using Environment.PhotoReview;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

public class PhotoReviewComputerUI : MonoBehaviour
{
    [SerializeField] private GameObject _mainScreen;
    [SerializeField] private GameObject _offScreen;
    [SerializeField] private GameObject _photoUploadPage;

    private void Awake()
    {
        _offScreen.SetActive(true);
        _photoUploadPage.SetActive(false);
        _mainScreen.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotoReviewManager.Instance.CurrentPhotoReviewState.GetObservable().Subscribe(newState =>
        {
            switch (newState)
            {
                case PhotoReviewState.MAIN_PAGE:
                    _offScreen.SetActive(false);
                    _mainScreen.SetActive(true);
                    break;
                case PhotoReviewState.PHOTO_SELECT:
                    // TODO: animate
                    _photoUploadPage.SetActive(true);
                    break;
            }
        });
    }

}



