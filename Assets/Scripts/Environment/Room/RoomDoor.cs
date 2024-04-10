using Characters.Player;
using Game.Application;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment.Room
{
    public class RoomDoor : MonoBehaviour, IInteractable
    {
        public bool IsInteractable => true;

        public void Hover(Vector3 CameraPosition, Vector3 hitPoint)
        {
            Debug.Log("Hover on the door");
        }

        public void Interact(Vector3 CameraPosition, Vector3 hitPoint)
        {
            Debug.Log("Interacted with the door");

            // Add scene transition here
            PlayerManager.Instance.CurrentSpawnPointID = SpawnPointID.HOME_DOOR;
            ApplicationManager.Instance.Loader.ChangeScene(Common.SceneManagement.SceneID.TOWN_SCENE, Common.SceneManagement.LoadingScreenType.DEFAULT);
        }

        public void StopHover()
        {
        }
    }
}
