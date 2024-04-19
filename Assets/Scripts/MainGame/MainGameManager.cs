using Characters.Player;
using Common.DataManagement;
using Common.DesignPatterns;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MainGame
{
    public class MainGameManager : SingletonPersistent<MainGameManager>
    {
        public SaveableDataContainer GameProgressData;

        private ReactiveProp<GameState> CurrentGameState = new ReactiveProp<GameState>();

        // An indicator for the game to recognise the most impactful changes recently
        public EventID RecentGameEvent;

        // Start is called before the first frame update
        void Start()
        {
            CurrentGameState.GetObservable().Subscribe(newState => { ChangeGameState(newState); });

            GameProgressData.InitialiseDataContainer();
            GameProgressData.GameData.Add(new SaveableBoolAsset("Finished Tutorial", false));
            GameProgressData.GameData.Add(new SaveableIntAsset("Day Count", 1));
        }
        
        public void ChangeGameState(GameState newGameState)
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    public enum GameState
    {
        ROOM_SCENE,
        TOWN_WALK,
        MAIN_GAMEPLAY,
        PHOTO_SCORING,
        FINAL_RESULT,
    }

    public enum EventID
    {
        NONE,

        // Event Dialogue lines
        NOBODY_MOVED,
        VIRAL,
        NEWS,
        WALMART
    }
}
