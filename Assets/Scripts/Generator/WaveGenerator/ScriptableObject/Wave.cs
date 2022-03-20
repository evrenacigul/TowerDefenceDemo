using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generator.Waves
{
    [CreateAssetMenu(fileName = "WaveName", menuName = "TowerDefence Demo/Add Wave", order = 1)]
    [System.Serializable]
    public class Wave : ScriptableObject
    {
        public List<WaveInfo> waves;
    }

    [System.Serializable]
    public struct WaveInfo
    {
        public List<CreatureInfo> waveCreatures;
        public float timeBetweenSpawns;
        public float timeToWaveBegins;
    }

    [System.Serializable]
    public struct CreatureInfo
    {
        public int count;
        public Object prefab;
    }
}