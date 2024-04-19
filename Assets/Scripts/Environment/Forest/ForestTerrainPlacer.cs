using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Environment.Forest
{
    /// <summary>
    /// A simple class to ensure all the trees / bushes and stuff are placed on the correct Y axis
    /// </summary>
    public class ForestTerrainPlacer : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            List<GameObject> forestGameObjects = GameObject.FindGameObjectsWithTag("ForestItem").ToList();

            for (int i = 0; i < forestGameObjects.Count; i++)
            {
                forestGameObjects[i].transform.position = new Vector3(forestGameObjects[i].transform.position.x, Terrain.activeTerrain.SampleHeight(forestGameObjects[i].transform.position), forestGameObjects[i].transform.position.z);
            }
        }
    }
}
