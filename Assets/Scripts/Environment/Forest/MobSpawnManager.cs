using Characters.Player;
using Common.DesignPatterns;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Environment.Forest
{
    /// <summary>
    /// This class spawns the mobs
    /// </summary>
    public class MobSpawnManager : Singleton<MobSpawnManager>
    {
        [Header("Mob variables")]
        [SerializeField] private MobList _mobList;
        [SerializeField, Range(5, 20)] private int _anomalyCap = 15;
        [SerializeField, Range(1, 5)] private int _staticAnomaliesCapPerZone = 1;
        [SerializeField, Range(5, 20)] private int _mobCap = 30;

        [Space]
        public BoxCollider SpawnArea;
        [SerializeField] private GameObject _midAreaMarker;
        [SerializeField] private GameObject _deepAreaMarker;

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

        private System.Random _random = new System.Random();

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
                SpawnMobs();
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
                    GameObject mobToDestroy = _spawnedMobs[spawnMobCounter];
                    _spawnedMobs.RemoveAt(spawnMobCounter);
                    Destroy(mobToDestroy);
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
                    GameObject mobToDestroy = _spawnedAnomalies_NonStatic[anomalySpawnCounter];
                    _spawnedAnomalies_NonStatic.RemoveAt(anomalySpawnCounter);
                    Destroy(mobToDestroy);
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

        private GameObject SpawnGameObjectOnTerrain(GameObject currentGOToSpawn, GameObject spawnPosition)
        {
            GameObject spawnedGameObject = Instantiate(currentGOToSpawn, spawnPosition.transform.position, spawnPosition.transform.rotation);
            foreach (Transform item in spawnedGameObject.GetComponentInChildren<Transform>())
            {
                item.position = new Vector3(item.position.x, Terrain.activeTerrain.SampleHeight(item.position), item.position.z);
            }

            return spawnedGameObject;

        }

        private GameObject SpawnGameObjectOnTerrain(GameObject currentGOToSpawn, Vector3 spawnPosition)
        {
            GameObject spawnedGameObject = Instantiate(currentGOToSpawn, spawnPosition, Quaternion.identity);
            foreach (Transform item in spawnedGameObject.GetComponentInChildren<Transform>())
            {
                item.position = new Vector3(item.position.x, Terrain.activeTerrain.SampleHeight(item.position), item.position.z);
            }

            return spawnedGameObject;
        }

        private void SpawnMobs()
        {
            // Spawn anomalies
            if (_spawnedAnomalies_NonStatic.Count < _anomalyCap)
            {
                for (int i = _spawnedAnomalies_NonStatic.Count; i < _anomalyCap; i++)
                {
                    // Generate a random point around the player between the given coordinates 
                    Vector3 randomPosition = new Vector3(Random.Range(SpawnArea.bounds.min.x, SpawnArea.bounds.max.x), 0, Random.Range(SpawnArea.bounds.min.z, SpawnArea.bounds.max.z));

                    Debug.Log(randomPosition);

                    // Check if point is valid,, then check if point is between the given coordinates
                    float distanceFromPlayer = Vector3.Distance(new Vector3(PlayerManager.Instance.Movement.gameObject.transform.position.x, 0, PlayerManager.Instance.Movement.gameObject.transform.position.z), randomPosition);

                    if (distanceFromPlayer >= MobSpawnAvoidanceRadius && distanceFromPlayer <= MobSpawnRadius)
                    {
                        // is at a valid radius
                        // determine the type to spawn

                        int RandomChance = Random.Range(0, 100);
                        DangerLevel dangerLevelOfMob = DangerLevel.NON_HOSTILE;
;
                        if (randomPosition.z < _midAreaMarker.transform.position.z)
                        {
                            // Forest edge 
                            if (RandomChance < 10)
                            {
                                // Spawn medium danger mob
                                dangerLevelOfMob = DangerLevel.MODERATE;
                            }
                            else
                            {
                                // Spawn non dangerous mob
                                dangerLevelOfMob = DangerLevel.NON_HOSTILE;
                            }
                        }
                        else if (randomPosition.z < _deepAreaMarker.transform.position.z)
                        {
                            // Medium forest (ruins) 
                            if (RandomChance < 10)
                            {
                                // Spawn extreme danger mob
                                dangerLevelOfMob = DangerLevel.EXTREME;
                            }
                            else if (RandomChance < 50)
                            {
                                // Spawn non dangerous mob
                                dangerLevelOfMob = DangerLevel.NON_HOSTILE;
                            }
                            else
                            {
                                // Spawn medium danger mob
                                dangerLevelOfMob = DangerLevel.MODERATE;
                            }
                        }
                        else
                        {
                            // Medium forest (ruins) 
                            if (RandomChance < 10)
                            {
                                // Spawn extreme danger mob
                                dangerLevelOfMob = DangerLevel.NON_HOSTILE;
                            }
                            else if (RandomChance < 50)
                            {
                                // Spawn non dangerous mob
                                dangerLevelOfMob = DangerLevel.MODERATE;
                            }
                            else
                            {
                                // Spawn medium danger mob
                                dangerLevelOfMob = DangerLevel.EXTREME;
                            }
                        }

                        GameObject mobToSpawn = _mobList.Anomalies.Where(x => x.AnomalyDangerLevel == dangerLevelOfMob).OrderBy(_ => _random.Next()).First().MobPrefab;

                        GameObject spawnedObj = SpawnGameObjectOnTerrain(mobToSpawn, randomPosition);
                        _spawnedAnomalies_NonStatic.Add(spawnedObj);
                    }

                }
            }

            // Spawn all mobs
            if (_spawnedMobs.Count < _mobCap)
            {
                for (int i = _spawnedMobs.Count; i < _mobCap; i++)
                {
                    // Generate a random point around the player between the given coordinates 
                    Vector3 randomPosition = new Vector3(Random.Range(SpawnArea.bounds.min.x, SpawnArea.bounds.max.x), 0, Random.Range(SpawnArea.bounds.min.z, SpawnArea.bounds.max.z));

                    // Check if point is valid,, then check if point is between the given coordinates
                    float distanceFromPlayer = Vector3.Distance(new Vector3(PlayerManager.Instance.Movement.gameObject.transform.position.x, 0, PlayerManager.Instance.Movement.gameObject.transform.position.z), randomPosition);

                    if (distanceFromPlayer >= MobSpawnAvoidanceRadius && distanceFromPlayer <= MobSpawnRadius)
                    {
                        // is at a valid radius
                        // determine the type to spawn

                        int RandomChance = Random.Range(0, 100);
                        DangerLevel dangerLevelOfMob = DangerLevel.NON_HOSTILE;
                        ;
                        if (randomPosition.z < _midAreaMarker.transform.position.z)
                        {
                            // Forest edge 
                            if (RandomChance < 30)
                            {
                                // Spawn medium danger mob
                                dangerLevelOfMob = DangerLevel.MODERATE;
                            }
                            else
                            {
                                // Spawn non dangerous mob
                                dangerLevelOfMob = DangerLevel.NON_HOSTILE;
                            }
                        }
                        else if (randomPosition.z < _deepAreaMarker.transform.position.z)
                        {
                            // Medium forest (ruins) 
                            if (RandomChance < 50)
                            {
                                // Spawn non dangerous mob
                                dangerLevelOfMob = DangerLevel.NON_HOSTILE;
                            }
                            else
                            {
                                // Spawn medium danger mob
                                dangerLevelOfMob = DangerLevel.MODERATE;
                            }
                        }
                        else
                        {
                            // Medium forest (ruins) 
                            if (RandomChance < 30)
                            {
                                // Spawn extreme danger mob
                                dangerLevelOfMob = DangerLevel.NON_HOSTILE;
                            }
                            else
                            {
                                // Spawn non dangerous mob
                                dangerLevelOfMob = DangerLevel.MODERATE;
                            }
                        }

                        GameObject mobToSpawn = _mobList.Mobs.Where(x => x.AnomalyDangerLevel == dangerLevelOfMob).OrderBy(_ => _random.Next()).First().MobPrefab;

                        GameObject spawnedMob = SpawnGameObjectOnTerrain(mobToSpawn, randomPosition);

                        _spawnedMobs.Add(spawnedMob);
                    }

                }
            }
        }
    }
}
