using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    namespace DesignPatterns
    {
        /// <summary>
        /// Sample object pool functionality
        /// </summary>
        public class ObjectPooler : MonoBehaviour
        {
            [Header("Object to pool")]
            public GameObject ObjectToPool;
            [Min(2)] public int AmountToPool;

            [Header("Object Pool settings")]
            public bool ShouldExpand = true;
            public int ExpansionAmount = 1;
            public bool TakeFirstIfFull = false;

            // A list of items we have already
            public List<GameObject> pooledObjects;

            // Start is called before the first frame update
            void Start()
            {
                // Error checking, if false then run code
                if (DevUtils.AssertFalse(ObjectToPool == null, "There is no valid gameobject to pool. Please make sure you add a valid Gameobject."))
                {
                    // Initialise objects
                    pooledObjects = new List<GameObject>();
                    InitialiseItem(AmountToPool);
                }
            }

            private void InitialiseItem(int amountToInitialise)
            {
                for (int i = 0; i < amountToInitialise; i++)
                {
                    GameObject obj = (GameObject)Instantiate(ObjectToPool);
                    obj.SetActive(false);
                    obj.transform.SetParent(this.transform);
                    pooledObjects.Add(obj);
                }
            }

            public GameObject GetObject()
            {
                // Some items in the pooled objects are not full, take from there
                for (int i = 0; i < pooledObjects.Count; i++)
                {
                    if (pooledObjects[i] == null) { pooledObjects.Remove(pooledObjects[i]); }
                    if (!pooledObjects[i].activeInHierarchy)
                    {
                        pooledObjects[i].SetActive(true);
                        return pooledObjects[i];
                    }
                }

                if (ShouldExpand == true)
                {
                    // Add new items and initialise them
                    InitialiseItem(ExpansionAmount);
                    return GetObject();
                }
                else if (TakeFirstIfFull == true)
                {
                    GameObject temp = pooledObjects[0];
                    pooledObjects.Remove(temp);
                    pooledObjects.Add(temp);
                    return temp;
                }

                Debug.LogError("Object pool is returning a null error. This is normally not supposed to happen. Please check it thanks");
                return null;
            }

            public void returnObject(GameObject obj)
            {
                obj.SetActive(false);
            }
        }
    }
}
