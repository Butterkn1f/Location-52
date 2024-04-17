using Characters.Player;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEditor.Progress;

namespace Environment.Forest
{
    /// <summary>
    /// This class spawns the mobs
    /// </summary>
    public class MobSpawnManager : MonoBehaviour
    {
        [Header("Mob variables")]
        [SerializeField] private MobList _mobList;
        [SerializeField, Range(5, 20)] private int _anomalyCap = 5;
        [SerializeField, Range(1, 5)] private int _staticAnomaliesCapPerZone = 1;
        [SerializeField, Range(5, 20)] private int _mobCap = 20;

        [System.Serializable]
        private class StaticMobSpawnPoints
        {
            public GameObject SpawnPoint;
            public AnomalySize SpawnPointSize;
        }

        [SerializeField] private List<StaticMobSpawnPoints> _staticAnomalySpawnPoints;

        private List<GameObject> _spawnedMobs = new List<GameObject>();
        private List<GameObject> _spawnedAnomalies_NonStatic = new List<GameObject>();

        [Header("Mob Spawning Variables")]
        [SerializeField, Range(0.1f, 1.0f)] private float MobSpawnTick = 0.25f;
        private float counter = 0.0f;

        // This is the radius in which normal mobs cannot spawn from the player
        [SerializeField, Range(1, 100)] private int MobSpawnAvoidanceRadius = 25;

        // This is the radius in which normal mobs spawn from the player
        // Beyond this distance, the mobs despawn 
        [SerializeField, Range(1, 100)] private int MobSpawnRadius = 50;
        [SerializeField] private TerrainData _terrainData;

        #region privateVariables

        private System.Random _random;

        #endregion

        // Start is called before the first frame update
        void Start()
        {
            //SpawnStaticAnomalies();
        }

        // Update is called once per frame
        void Update()
        {
            counter += Time.deltaTime;

            if (counter >= MobSpawnTick)
            {
                counter = 0;

                DespawnMobs();
                
            }
        }

        /// <summary>
        /// Despawn mobs if they're too far away
        /// </summary>
        private void DespawnMobs()
        {
            GameObject playerObject = PlayerManager.Instance.Movement.gameObject;

            int spawnMobCounter = 0;
            while (spawnMobCounter < _spawnedMobs.Count)
            {
                if (Vector3.SqrMagnitude(_spawnedMobs[spawnMobCounter].transform.position - playerObject.transform.position) > Mathf.Pow(MobSpawnRadius, 2))
                {
                    _spawnedMobs.RemoveAt(spawnMobCounter);  
                }
                else
                {
                    spawnMobCounter++;
                }
            }

            int anomalySpawnCounter = 0;
            while (anomalySpawnCounter < _spawnedAnomalies_NonStatic.Count)
            {
                if (Vector3.SqrMagnitude(_spawnedAnomalies_NonStatic[anomalySpawnCounter].transform.position - playerObject.transform.position) > Mathf.Pow(MobSpawnRadius, 2))
                {
                    _spawnedAnomalies_NonStatic.RemoveAt(anomalySpawnCounter);
                }
                else
                {
                    anomalySpawnCounter++;
                }
            }
        }

        private void SpawnStaticAnomalies()
        {
            // Spawn anomalies
            // Get a list of random static anomalies
            List<StaticAnomalyObject> _tempStaticAnomalyList = _mobList.StaticAnomalies.OrderBy(x => _random.Next()).ToList();

            for (int i = 0; i < 3; i++)
            {
                AnomalySize currentSize = AnomalySize.SMALL;
                switch (i)
                {
                    case 0:
                        currentSize = AnomalySize.SMALL;
                        break;
                    case 1:
                        currentSize = AnomalySize.MEDIUM;
                        break;
                    case 2:
                        currentSize = AnomalySize.LARGE;
                        break;
                }

                List<StaticMobSpawnPoints> randomSpawnPoints = _staticAnomalySpawnPoints.Where(x => x.SpawnPointSize == currentSize).OrderBy(x => _random.Next()).ToList();

                for (int j = 0; j < _staticAnomaliesCapPerZone; j++)
                {
                    if (_tempStaticAnomalyList.Where(x => x.AnomalySize == currentSize).ToList()[j].MobPrefab != null || randomSpawnPoints[j].SpawnPoint != null)
                    {
                        GameObject _currentSpawnPoint = randomSpawnPoints[j].SpawnPoint;
                        SpawnGameObjectOnTerrain(_tempStaticAnomalyList.Where(x => x.AnomalySize == currentSize).ToList()[j].MobPrefab, _currentSpawnPoint);
                    }
                }

                // Replace the rest of the 
            }
        }

        private void SpawnGameObjectOnTerrain(GameObject currentGOToSpawn, GameObject spawnPosition)
        {
            GameObject spawnedGameObject = Instantiate(currentGOToSpawn, spawnPosition.transform.position, spawnPosition.transform.rotation);
            foreach (Transform item in spawnedGameObject.GetComponentInChildren<Transform>())
            {
                item.position = new Vector3(item.position.x, Terrain.activeTerrain.SampleHeight(item.position), item.position.z);
            }

        }

        private void SpawnMobs()
        {
            // Spawn mobs
            if (_spawnedAnomalies_NonStatic.Count < _anomalyCap)
            {
                for (int i = _spawnedAnomalies_NonStatic.Count; i < _anomalyCap; i++)
                {
                    int randomDistance = Random.Range(MobSpawnAvoidanceRadius, MobSpawnRadius);

                }
            }

            // Spawn all anomalies
        }
    }
}
