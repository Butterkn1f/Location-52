using Characters.Player;
using Game.Application;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Environment.Room
{
    public class RoomDoor : MonoBehaviour, IInteractable
    {
        public bool IsInteractable => true;

        [SerializeField] private UnityEvent _noCameraEvent;

        public void Hover(Vector3 CameraPosition, Vector3 hitPoint)
        {
            Debug.Log("Hover on the door");
        }

        public void Interact(Vector3 CameraPosition, Vector3 hitPoint)
        {
            Debug.Log("Interacted with the door");

            if (InventoryManager.Instance.CheckInventoryHasCamera())
            {
                // Add scene transition here
                PlayerManager.Instance.CurrentSpawnPointID = SpawnPointID.HOME_DOOR;
                ApplicationManager.Instance.Loader.ChangeScene(Common.SceneManagement.SceneID.TOWN_SCENE, Common.SceneManagement.LoadingScreenType.DEFAULT);
            }
            else
            {
                _noCameraEvent.Invoke();
            }
        }

        public void StopHover()
        {
        }
    }
}
