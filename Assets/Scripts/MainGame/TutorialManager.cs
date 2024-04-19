using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MainGame
{
    public class TutorialManager : MonoBehaviour
    {
        private ReactiveProp<TutorialState> _currentTutorialState = new ReactiveProp<TutorialState>();

        // Start is called before the first frame update
        void Start()
        {
            _currentTutorialState.SetValue(TutorialState.INTRO_CUTSCENE);
            GetNextTutorialStage();
            Debug.Log(_currentTutorialState.GetValue().ToString());
        }

        public void GetNextTutorialStage()
        {
            _currentTutorialState.SetValue((TutorialState)((int)_currentTutorialState.GetValue() + 1));
        }
    }

    public enum TutorialState
    {
        INTRO_CUTSCENE,
        MOVEMENT_TUTORIAL,
        INTERACTION_TUTORIAL,
        TOWN_TUTORIAL,
        FOREST_TUTORIAL,
        PHOTO_TUTORIAL,
    }
}
