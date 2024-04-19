using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment.Forest
{
    /// <summary>
    /// This script keeps track of what mobs exist !!
    /// </summary>
    [CreateAssetMenu(menuName = "Custom data containers/Mob List")]
    public class MobList : ScriptableObject
    {
        public List<MobObject> Mobs;
        public List<AnomalyObject> Anomalies;
        public List<StaticAnomalyObject> StaticAnomalies;
    }

    [System.Serializable]
    public class MobObject
    {
        public string MobName;
        public GameObject MobPrefab;
        public DangerLevel AnomalyDangerLevel;
    }

    [System.Serializable]
    public class AnomalyObject : MobObject 
    {
        public string AnomalyDescription;
    }

    [System.Serializable]
    public class StaticAnomalyObject : AnomalyObject
    {
        public AnomalySize AnomalySize;
    }

    public enum DangerLevel
    {
        STATIC,
        NON_HOSTILE,
        MODERATE,
        EXTREME,
    }

    public enum AnomalySize
    {
        SMALL,
        MEDIUM,
        LARGE,
    }
}
