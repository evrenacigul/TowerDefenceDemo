using UnityEngine;
using System.Collections.Generic;

namespace GridSystem
{
    [CreateAssetMenu(fileName = "TileGrid Data", menuName = "TowerDefence Demo/MapGenerator/Create TileGrid Data", order = 0)]
    public class TileGrid : ScriptableObject
    {
        [SerializeField] private List<Tile> _tiles;
        [SerializeField] private List<Vector3> _path;

        [SerializeField] private int _width;
        [SerializeField] private int _height;
        public void Init(int width, int height)
        {
            _width = width; _height = height;
            _tiles = new List<Tile>();

            for (int x = 0; x < _width; x++)
            {
                for (int z = 0; z < _height; z++)
                {
                    Tile temp = new Tile();
                    temp.isEmpty = true;
                    temp.position = new Vector3Int(x, 0, z);
                    temp.obstacleObj = null;
                    temp.tileObj = null;
                    temp.type = TileType.Buildable;
                    _tiles.Add(temp);
                }
            }
        }

        public void SetTilePos(int posX, int posY, int posZ)
        {
            Tile tile = GetTile(posX, posZ);
            tile.position = new Vector3Int(posX, posY, posZ);
        }

        public void SetTileObj(int posX, int posZ, GameObject tileObj)
        {
            Tile tile = GetTile(posX, posZ);
            tile.tileObj = tileObj;
        }

        public void SetTileType(int posX, int posZ, TileType type)
        {
            Tile tile = GetTile(posX, posZ);
            tile.type = type;
        }

        public void SetTile(int posX, int posY, int posZ, GameObject tileObj, TileType type)
        {
            Tile tile = GetTile(posX, posZ);
            tile.position = new Vector3Int(posX, posY, posZ);
            tile.tileObj = tileObj;
            tile.type = type;
        }

        public void SetTileObstacleObj(int posX, int posZ, GameObject obstacleObj, Vector3 euler, Transform parent = null)
        {
            Tile tile = GetTile(posX, posZ);
            tile.obstacleObj = obstacleObj;
            tile.isEmpty = false;
            obstacleObj.transform.parent = parent;
            obstacleObj.transform.position = tile.position;
            obstacleObj.transform.rotation = Quaternion.Euler(euler);
        }

        public void ClearTileObstacleObj(int posX, int posZ)
        {
            Tile tile = GetTile(posX, posZ);
            GameObject.Destroy(tile.obstacleObj);
            tile.obstacleObj = null;
            tile.isEmpty = true;
        }

        public Tile GetTile(int posX, int posZ)
        {
            for (int i = 0; i < _width * _height; i++)
            {
                if (_tiles[i].position.x == posX && _tiles[i].position.z == posZ)
                    return _tiles[i];
            }
            return default(Tile);
        }

        public Tile GetRandomTile()
        {
            int x = Random.Range(1, _width - 1);
            int z = Random.Range(1, _height - 1);
            return GetTile(x, z);
        }

        public void SetPath(List<Vector3> path)
        {
            _path = path;
        }

        public List<Vector3> GetPath()
        {
            return _path;
        }
    }
}