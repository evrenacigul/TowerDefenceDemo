using UnityEngine;

namespace GridSystem
{
    [System.Serializable]
    public class Tile
    {
        public Vector3Int position;
        public TileType type;
        public GameObject tileObj;
        public GameObject obstacleObj;
        public bool isEmpty;
    }

    public enum TileType
    {
        Buildable,
        Path,
        Home,
        SpawnPoint,
        Empty
    }
}