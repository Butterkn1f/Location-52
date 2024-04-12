using Environment.PhotoReview;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

public class PhotoReviewComputerUI : MonoBehaviour
{
    [System.Serializable]
    public class ComputerReviewScreens
    {
        public PhotoReviewState state;
        public GameObject ComputerScreen;
    }

    public List<ComputerReviewScreens> ComputerScreensList;

    private void Awake()
    {
        // Deactivate all screens at the start.. it causes the animations to not work unfortunately
        foreach (var item in ComputerScreensList)
        {
            // Deactivate all the screens in the computer first
            item.ComputerScreen.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotoReviewManager.Instance.CurrentPhotoReviewState.GetObservable().Subscribe(newState =>
        {
            foreach (var item in ComputerScreensList)
            {
                // Deactivate all the screens in the computer first
                item.ComputerScreen.SetActive(false);
            }

            ComputerReviewScreens CurrentPhase = ComputerScreensList.Where(x => x.state == newState).First();

            // TODO: add delay here, perchance?
            CurrentPhase.ComputerScreen.SetActive(true);
        });
    }

}



