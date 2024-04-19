using Characters.Player;
using Game.Application;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment.Town
{
    public class TownHouseDoor : MonoBehaviour, IInteractable
    {
        public bool IsInteractable => true;

        public void Hover(Vector3 CameraPosition, Vector3 hitPoint)
        {
            Debug.Log("Hover on the house door");
        }

        public void Interact(Vector3 CameraPosition, Vector3 hitPoint)
        {
            Debug.Log("Interacted with the house door");

            // Add scene transition here
            PlayerManager.Instance.CurrentSpawnPointID = SpawnPointID.ROOM_AFTER;
            ApplicationManager.Instance.Loader.ChangeScene(Common.SceneManagement.SceneID.PHOTO_REVIEW_SCENE, Common.SceneManagement.LoadingScreenType.DEFAULT);
        }

        public void StopHover()
        {
        }
    }
}