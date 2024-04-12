using Common.DataManagement;
using Common.DesignPatterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainGame
{
    public class MainGameManager : SingletonPersistent<MainGameManager>
    {
        [SerializeField] private SaveableDataContainer GameProgressData;

        // Start is called before the first frame update
        void Start()
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
}
