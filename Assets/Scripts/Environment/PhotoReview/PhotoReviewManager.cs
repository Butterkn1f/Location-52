using Common.DesignPatterns;
using DG.Tweening;
using Game.Application;
using MainGame;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Environment.PhotoReview
{
    public class PhotoReviewManager : Singleton<PhotoReviewManager>
    {
        [Space]
        [SerializeField] private GameObject CameraStartPos;
        [SerializeField] private GameObject CameraComPos;

        [SerializeField] private PastPhotos _pastPhotos;


        public ReactiveProp<PhotoReviewState> CurrentPhotoReviewState = new ReactiveProp<PhotoReviewState>();
        public List<GameObject> _selectedImages;

        public Grade PlayerResult;

        // Start is called before the first frame update
        void Start()
        {
            //Sequence seq = DOTween.Sequence();
            //seq.PrependInterval(0.5f);
            //seq.Append(CameraObject.transform.DOMove(CameraComPos.transform.position, 0.5f));
            //seq.Join(CameraObject.transform.DORotateQuaternion(CameraComPos.transform.rotation, 0.5f));
            //seq.AppendCallback(() => { });

            CurrentPhotoReviewState.SetValue(PhotoReviewState.DEFAULT);
        }

        public void StartComputer()
        {
            CurrentPhotoReviewState.SetValue(PhotoReviewState.MAIN_PAGE);
        }

        public void SelectPhotos()
        {
            CurrentPhotoReviewState.SetValue(PhotoReviewState.PHOTO_SELECT);
        }

        public void UploadPost()
        {
            PhotoReviewManager.Instance.CurrentPhotoReviewState.SetValue(PhotoReviewState.PHOTO_UPLOAD_SEQUENCE);
            _pastPhotos.GenerateNewPost();
        }

        /// <summary>
        /// Marks the end of the day
        /// </summary>
        public void EndDay()
        {
            if (MainGameManager.Instance.GameProgressData.GetDataBoolByID("Finished Tutorial").GetValue() == false)
            {
                // Mark tutorial as completed
                MainGameManager.Instance.GameProgressData.GetDataBoolByID("Finished Tutorial").SetValue(true);
                Debug.Log("Tutorial Marked as completed");
            }

            // Increment day counter
            MainGameManager.Instance.GameProgressData.GetDataIntByID("Day Count").SetValue(MainGameManager.Instance.GameProgressData.GetDataIntByID("Day Count").GetValue() + 1);
            Debug.Log("Moving on to day " + MainGameManager.Instance.GameProgressData.GetDataIntByID("Day Count").GetValue());

            // Change Scene
            ApplicationManager.Instance.Loader.ChangeScene(Common.SceneManagement.SceneID.ROOM_SCENE);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    public enum PhotoReviewState
    {
        DEFAULT,
        MAIN_PAGE = 1,
        PHOTO_SELECT = 2, 
        PHOTO_UPLOAD_SEQUENCE = 3,
        NEWS_SECTION = 4,
        RESULTS_PAGE = 5
    }

    /// <summary>
    /// This is the 
    /// </summary>public enum Grade
    public enum Grade
    {
        NOT_IMPRESSED = 0,
        DECENT = 1,
        VIRAL = 2,
        MEGA_VIRAL = 3,
        EXTREMELY_VIRAL = 4
    }
    
}
