using DG.Tweening;
using Game.Application;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UniRx;
using UnityEngine;

namespace Menus
{
    public class MainMenu : MonoBehaviour
    {
        [System.Serializable]
        public class MainMenuPhase
        {
            public MainMenuState CurrentState;
            public GameObject ComputerScreenDisplay;
            public GameObject CameraPosition;
        }

        public List<MainMenuPhase> PhaseList;
        public GameObject CameraGO;
        public GameObject InitialCameraPosition;

        public ReactiveProp<MainMenuState> CurrentMainMenuState = new ReactiveProp<MainMenuState>();

        // Start is called before the first frame update
        void Start()
        {
            CameraGO.transform.position = InitialCameraPosition.transform.position;
            CameraGO.transform.rotation = InitialCameraPosition.transform.rotation;

            CurrentMainMenuState.GetObservable().Subscribe(newVal =>
            {
                ChangeMainMenuState(newVal);
            });

            CurrentMainMenuState.SetValue(MainMenuState.TITLE_SCREEN);
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void ChangeMainMenuState(MainMenuState newState)
        {
            foreach (var item in PhaseList)
            {
                // Deactivate all the screens in the computer first
                item.ComputerScreenDisplay.SetActive(false);
            }

            MainMenuPhase CurrentPhase = PhaseList.Where(x => x.CurrentState == newState).First();
            CameraGO.transform.DOMove(CurrentPhase.CameraPosition.transform.position, 0.5f);
            CameraGO.transform.DORotateQuaternion(CurrentPhase.CameraPosition.transform.rotation, 0.5f);

            // TODO: add delay here, perchance?
            CurrentPhase.ComputerScreenDisplay.SetActive(true);
        }

        public void GoToModeSelect()
        {
            CurrentMainMenuState.SetValue(MainMenuState.MODE_SELECT);
        }

        public void ViewSettings()
        {
            CurrentMainMenuState.SetValue(MainMenuState.SETTINGS);
        }

        public void GoToTitle()
        {
            CurrentMainMenuState.SetValue(MainMenuState.TITLE_SCREEN);
        }

        public void GoToMainMenu()
        {
            CurrentMainMenuState.SetValue(MainMenuState.MAIN_MENU);
        }

        public void StartGame()
        {
            CurrentMainMenuState.SetValue(MainMenuState.TRANSITION);

            Sequence seq = DOTween.Sequence();
            seq.PrependInterval(0.5f);
            seq.AppendCallback(() => { ApplicationManager.Instance.Loader.ChangeScene(Common.SceneManagement.SceneID.ROOM_SCENE, Common.SceneManagement.LoadingScreenType.DEFAULT); });
        }
    }

    public enum MainMenuState
    {
        TITLE_SCREEN,
        SETTINGS,
        MODE_SELECT,
        TRANSITION,
        MAIN_MENU
    }
}
