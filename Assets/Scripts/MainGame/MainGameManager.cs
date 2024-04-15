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
        [SerializeField] private SaveableDataContainer GameProgressData;

        private ReactiveProp<GameState> CurrentGameState = new ReactiveProp<GameState>();

        // Start is called before the first frame update
        void Start()
        {

        }
        
        public void ChangeGameState(GameState newGameState)
        {
            switch (newGameState)
            {
                case GameState.PHOTO_SCORING:
                    // Player does not need to walk anymore lol
                    Destroy(PlayerManager.Instance.gameObject);
                    break;
            }
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
}
