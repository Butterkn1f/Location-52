using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UniRx;
using System.Collections.Generic;

namespace Common.SceneManagement
{
    /// <summary>
    /// The main class that loads the different scenes
    /// Controls which scenes load when
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        #region variables

        [SerializeField] private SceneAssetList _sceneList;

        public LoadingScreenType CurrentLoadingScreenType;
        [HideInInspector] public ReactiveProp<SceneID> CurrentSceneID;

        [System.Serializable]
        public struct LoadScreens
        {
            public GameObject LoadScreenObj;
            public LoadingScreenType LoadScreenType;
        }

        [SerializeField] private List<LoadScreens> _loadScreens;

        public UnityEvent SceneChangeEvent;


        #endregion

        // Start is called before the first frame update
        void Start()
        {
            CurrentSceneID = new ReactiveProp<SceneID>();

            // Do error checks here
            DevUtils.AssertTrue(_sceneList == null, "There is no valid scenelist. Please make sure there is a valid sceneList", LogType.Error);
            DevUtils.AssertTrue(_sceneList.SceneList.Count == 0, "There are no valid scenes in scenelist. Please make sure there is at least one valid scene.");

            CurrentSceneID.SetValue(_sceneList.SceneList.Where(x => x.SceneName == SceneManager.GetActiveScene().name).Select(x => x.SceneAssetID).First());

            for (int i = 0; i < _loadScreens.Count(); i++)
            {
                _loadScreens[i].LoadScreenObj.SetActive(false);
            }
        }

        /// <summary>
        /// Change the scene but with loading screen
        /// </summary>
        /// <param name="newSceneID">The scene ID of the new scene</param>
        /// <param name="loadScreenType">The loading screen type to load</param>
        public void ChangeScene(SceneID newSceneID, LoadingScreenType loadScreenType)
        {
            DOTween.KillAll();

            // Check for any null cases
            if (DevUtils.AssertTrue(_sceneList.SceneList.Where(s => s.SceneAssetID == newSceneID).Count() > 0, "Requested Scene ID of " + newSceneID.ToString() + " is not found. Please make sure you specify a valid scene name in the ID"))
            {
                // If you want the loading screen to show up just set the 2nd parameter to true
                // If checked as true for loadToLoadingScreen, load the loading screen 
                StartCoroutine(LoadSceneCoroutine(newSceneID, loadScreenType));
            }
        }

        /// <summary>
        /// Default loading lolll
        /// </summary>
        public void ChangeScene(SceneID newSceneID)
        {
            DOTween.KillAll();

            // Check for any null cases
            if (DevUtils.AssertTrue(_sceneList.SceneList.Where(s => s.SceneAssetID == newSceneID).Count() > 0, "Requested Scene ID of " + newSceneID.ToString() + " is not found. Please make sure you specify a valid scene name in the ID"))
            {
                CurrentSceneID.SetValue(newSceneID);
                SceneManager.LoadScene(GetSceneName(newSceneID));
            }
        }

        private IEnumerator LoadSceneCoroutine(SceneID newSceneID, LoadingScreenType loadScreenType)
        {
            DOTween.KillAll();

            // Loads the Loading Screen 
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(GetSceneName(newSceneID));

            for (int i = 0; i < _loadScreens.Count(); i++)
            {
                _loadScreens[i].LoadScreenObj.SetActive(false);
            }

            GameObject loadingScreenGameObject = null;

            // Set the current to the 
            if (loadScreenType != LoadingScreenType.NONE)
            {
                loadingScreenGameObject = _loadScreens.Where(x => x.LoadScreenType == loadScreenType).First().LoadScreenObj;
                loadingScreenGameObject.SetActive(true);
            }

            // If the current load for the scene is not done
            while (!asyncLoad.isDone)
            {
                // wait until the asynchronous loading is done
                // 
                yield return null;
            }

            if (loadingScreenGameObject != null)
            {
                loadingScreenGameObject.SetActive(false);
            }

            // Once complete
            // Loads the expected scene
            SceneChangeEvent.Invoke();
            SceneChangeEvent.RemoveAllListeners();
            CurrentSceneID.SetValue(newSceneID);
        }

        // Returns the string name based on the given string ID
        private string GetSceneName(SceneID sceneToSearch)
        {
            return _sceneList.SceneList.Where(s => s.SceneAssetID == sceneToSearch).Select(s => s.SceneName).LastOrDefault();
        }
    }

    public enum LoadingScreenType
    {
        NONE,
        DEFAULT,
    }

}

