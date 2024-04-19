using Characters.Player;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.DesignPatterns;

namespace Environment
{
    public class PlayerSpawnHelper : Singleton<PlayerSpawnHelper>
    {
        [System.Serializable]
        public class PlayerSpawnContainer
        {
            public string Header;
            public SpawnPointID ID;
            public GameObject spawnTransform;
        }

        [SerializeField] private List<PlayerSpawnContainer> _spawnPoints;

        private void Start()
        {
            if (PlayerManager.Instance != null)
            {
                GameObject SpawnPoint;

                if (_spawnPoints.Where(x => x.ID == PlayerManager.Instance.CurrentSpawnPointID).Count() == 0)
                {
                    SpawnPoint = _spawnPoints[0].spawnTransform;
                }
                else
                {
                    // Move player to correct spawn point
                    SpawnPoint = _spawnPoints.Where(x => x.ID == PlayerManager.Instance.CurrentSpawnPointID).FirstOrDefault().spawnTransform;
                }
                PlayerManager.Instance.Movement.MovePlayer(SpawnPoint.transform.position, SpawnPoint.transform.rotation);
            }
        }

        public void MovePlayerLocal(SpawnPointID newID)
        {
            GameObject SpawnPoint = _spawnPoints.Where(x => x.ID == newID).First().spawnTransform;
            PlayerManager.Instance.Movement.MovePlayer(SpawnPoint.transform.position, SpawnPoint.transform.rotation);
        }
    }

    public enum SpawnPointID
    {
        HOME_DOOR,
        TOWN_BIKE,
        ROOM_AFTER
    }
}
