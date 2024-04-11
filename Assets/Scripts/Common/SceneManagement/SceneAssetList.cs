using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.SceneManagement
{
    // Create a menu thing to show
    [CreateAssetMenu(menuName = "Custom app data containers/Scene Asset List", fileName = "New Scene list", order = 10)]
    public class SceneAssetList : ScriptableObject
    {
        // Physical scene list
        public List<Scene> SceneList;
    }

    [System.Serializable]
    public class Scene
    {
        // A unique identifier of the current scene asset
        public SceneID SceneAssetID;
        public string SceneName;
    }

    [System.Serializable]
    // This enumerator gives scenes a unique identification code
    // Scene changes are made by calls based on the ID
    public enum SceneID
    {
        MAIN_MENU = 0,
        ROOM_SCENE = 1,
        TOWN_SCENE = 2,
        FOREST_SCENE = 3,
        PHOTO_REVIEW_SCENE = 4,

        SAMPLE_SCENE = 100,
    }
}
