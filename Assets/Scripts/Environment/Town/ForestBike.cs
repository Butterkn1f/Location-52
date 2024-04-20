using Characters.Player;
using Game.Application;
using MainGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment.Town
{
    public class ForestBike : MonoBehaviour, IInteractable
    {
        public bool IsInteractable => true;

        public void Hover(Vector3 CameraPosition, Vector3 hitPoint)
        {
            Debug.Log("Hover on the bike");
        }

        public void Interact(Vector3 CameraPosition, Vector3 hitPoint)
        {
            Debug.Log("Interacted with the bike");

            // Add scene transition here
            ApplicationManager.Instance.Loader.ChangeScene(Common.SceneManagement.SceneID.TOWN_SCENE, Common.SceneManagement.LoadingScreenType.DEFAULT);
            PlayerManager.Instance.CurrentSpawnPointID = SpawnPointID.TOWN_BIKE;
        }

        public void StopHover()
        {
        }
    }
}