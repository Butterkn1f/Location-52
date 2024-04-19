using Common.DataManagement;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MainGame
{
    public class TutorialManager : MonoBehaviour
    {
        public SaveableDataContainer GameProgressData;
        public ReactiveProp<TutorialState> CurrentTutorialState = new ReactiveProp<TutorialState>();

        // Start is called before the first frame update
        void Awake()
        {
            CurrentTutorialState.SetValue(TutorialState.INTRO_CUTSCENE);


            GameProgressData.InitialiseDataContainer();
            GameProgressData.GameData.Add(new SaveableBoolAsset("Finished Tutorial", false));
            GameProgressData.GameData.Add(new SaveableIntAsset("Day Count", 1));
        }

        public void GetNextTutorialStage()
        {
            CurrentTutorialState.SetValue((TutorialState)((int)CurrentTutorialState.GetValue() + 1));
        }
    }

    public enum TutorialState
    {
        INTRO_CUTSCENE,
        MOVEMENT_TUTORIAL,
        INTERACTION_TUTORIAL,
        INVENTORY_TUTORIAL,
        TOWN_TUTORIAL,
        FOREST_TUTORIAL,
        PHOTO_TUTORIAL,
    }
}
