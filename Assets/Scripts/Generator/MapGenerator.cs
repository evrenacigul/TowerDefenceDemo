using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;
using Utility;

namespace Generator
{
    [ExecuteInEditMode]
    public class MapGenerator : MonoBehaviour
    {
        public static MapGenerator instance { get; private set; }

        [Header("Map Size(Tile Count)")]
        [Range(8, 128)]
        [SerializeField] int _mapWidthX = 16;
        [Range(8, 128)]
        [SerializeField] int _mapWidthZ = 16;

        [Header("Path Options")]
        [Range(2, 8)]
        [SerializeField] int _minimumPathLength = 3;
        [Range(2, 8)]
        [SerializeField] int _pathCornerCount = 4;

        [Header("Tile Prefabs")]
        [SerializeField] Object _roadPathTilePrefab;
        [SerializeField] Object _buildableTilePrefab;

        [Header("Building Prefabs")]
        [SerializeField] Object _mainHouseObjPrefab;
        [SerializeField] Object _enemySpawnObjPrefab;

        [Header("Obstacle Options")]
        [SerializeField] List<Object> _obstaclePrefabs;
        [Range(0, 150)]
        [SerializeField] int _maxObstacles = 0;

        GameObject _terrainParent;
        Vector2Int _pathStartPoint;
        Vector2Int _pathEndPoint;
        Vector2Int _pathCurrentPoint;

        public Vector2Int GetSpawnPoint { get { return _pathStartPoint; } }
        public Vector2Int GetHomePoint { get { return _pathEndPoint; } }

        [SerializeField] List<Vector2Int> _pathPoints;
        [SerializeField] List<Vector2Int> _pathCornerPoints;

        public List<Vector3> Path { get { return tileGrid.GetPath(); }}
        [SerializeField] private List<Vector3> _path;
        public int GetMapWidthX { get { return _mapWidthX; } }
        public int GetMapWidthZ { get { return _mapWidthZ; } }

        public LayerMask _tileLayer;

        [SerializeField] private TileGrid tileGrid;
        private TileGrid tileGridClone;

        private void Awake() { instance = this; }
        private void Start()
        {
            tileGridClone = tileGrid.Clone();
            Vector3 startPoint = tileGridClone.GetPath()[0];
            _pathStartPoint = new Vector2Int((int)startPoint.x, (int)startPoint.z);
        }

        public void Generate()
        {
            GenerateTerrainParent();
            GenerateTileGrid();
            GeneratePath();
            GenerateObstacles();

            for (int x = 0; x < _mapWidthX; x++)
            {
                for(int z = 0; z < _mapWidthZ; z++)
                {
                    Tile tile = tileGrid.GetTile(x, z);
                    GameObject temp = null;

                    if (tile.type == TileType.Buildable)
                    {
                        temp = (GameObject)Instantiate(_buildableTilePrefab, new Vector3(x, 0, z), Quaternion.identity, _terrainParent.transform);
                        AddBoxColliderAndTag(temp, "Tile");
                    }
                    else if (tile.type == TileType.Path)
                    {
                        Vector3 tempPos = new Vector3(x, 0, z);
                        temp = (GameObject)Instantiate(_roadPathTilePrefab, tempPos, Quaternion.identity, _terrainParent.transform);
                        AddBoxColliderAndTag(temp, "Path");
                    }
                    else if (tile.type == TileType.Home)
                    {
                        temp = (GameObject)Instantiate(_mainHouseObjPrefab, new Vector3(x, 0, z), Quaternion.identity, _terrainParent.transform);
                        Vector2 lookAtPos = _pathPoints[_pathPoints.Count - 2];
                        temp.transform.LookAt(new Vector3(lookAtPos.x, 0, lookAtPos.y));
                        AddBoxColliderAndTag(temp, "Home");
                    }
                    else if (tile.type == TileType.SpawnPoint)
                    {
                        temp = (GameObject)Instantiate(_enemySpawnObjPrefab, new Vector3(x, 0, z), Quaternion.identity, _terrainParent.transform);
                        AddBoxColliderAndTag(temp, "Spawn");
                    }
                    else
                        Debug.LogError("Tile type not found!");

                    tile.tileObj = temp;
                    if(tile.isEmpty)
                        tile.obstacleObj = null;

                    temp.layer = (int)Mathf.Log(_tileLayer.value, 2);
                }
            }
        }

        void AddBoxColliderAndTag(GameObject obj, string tag)
        {
            if (obj != null)
            {
                BoxCollider collider;
                collider = obj.GetComponent<BoxCollider>();
                if (collider == null)
                {
                    collider = obj.AddComponent<BoxCollider>();
                }

                collider.isTrigger = true;
                obj.tag = tag;
            }
        }

        void GenerateTerrainParent()
        {
            _terrainParent = GameObject.Find("Terrain Tiles");
            if (_terrainParent != null)
            {
                DestroyImmediate(_terrainParent);
            }
            _terrainParent = new GameObject("Terrain Tiles");
        }

        void GenerateTileGrid()
        {
            tileGrid.Init(_mapWidthX, _mapWidthZ);
        }

        void GeneratePath()
        {
            if (_pathPoints != null)
                _pathPoints.Clear();
            else
                _pathPoints = new List<Vector2Int>();

            if (_pathCornerPoints != null)
                _pathCornerPoints.Clear();
            else
                _pathCornerPoints = new List<Vector2Int>();

            if (_path == null)
            {
                _path = new List<Vector3>();
            }
            _path.Clear();
            //tileGrid.GetPath().Clear();

            // Randomize beginning and finish points of path
            _pathStartPoint = new Vector2Int(Random.Range(_minimumPathLength, _mapWidthX - _minimumPathLength), 0);
            _pathCurrentPoint = _pathStartPoint;
            _pathEndPoint = new Vector2Int(Random.Range(_minimumPathLength, _mapWidthX - _minimumPathLength), _mapWidthZ - _minimumPathLength);

            // Add first path point(spawn point) to corner connection list
            _pathPoints.Add(_pathCurrentPoint);

            int zOffset = _mapWidthZ / (_pathCornerCount - 1);

            for (int i = 0; i < _pathCornerCount; i++)
            {
                _pathCornerPoints.Add(new Vector2Int(Random.Range(_minimumPathLength, _mapWidthX - 1), (zOffset * (i + 1) / 2)));
            }
            // Add last path point(base) to corner connection list
            _pathCornerPoints.Add(_pathEndPoint);

            // Connect whole points step by step
            for (int i = 0; i < _pathCornerPoints.Count; i++)
            {
                // Move path in Y axis first
                if (_pathCurrentPoint.y < _pathCornerPoints[i].y)
                {
                    while (_pathCurrentPoint.y < _pathCornerPoints[i].y)
                    {
                        _pathCurrentPoint += MovePath.moveForward;
                        _pathPoints.Add(_pathCurrentPoint);
                    }
                }
                else if (_pathCurrentPoint.y > _pathCornerPoints[i].y)
                {
                    while (_pathCurrentPoint.y > _pathCornerPoints[i].y)
                    {
                        _pathCurrentPoint += MovePath.moveBackward;
                        _pathPoints.Add(_pathCurrentPoint);
                    }
                }

                // Then move path in X axis
                if(_pathCurrentPoint.x < _pathCornerPoints[i].x)
                {
                    while (_pathCurrentPoint.x < _pathCornerPoints[i].x)
                    {
                        _pathCurrentPoint += MovePath.moveRight;
                        _pathPoints.Add(_pathCurrentPoint);
                    }
                }
                else if (_pathCurrentPoint.x > _pathCornerPoints[i].x)
                {
                    while (_pathCurrentPoint.x > _pathCornerPoints[i].x)
                    {
                        _pathCurrentPoint += MovePath.moveLeft;
                        _pathPoints.Add(_pathCurrentPoint);
                    }
                }
            }

            // Set type of points as Path
            for (int i = 0; i < _pathPoints.Count; i++)
            {
                tileGrid.SetTileType(_pathPoints[i].x, _pathPoints[i].y, TileType.Path);
                _path.Add(new Vector3(_pathPoints[i].x, 0, _pathPoints[i].y));
            }

            tileGrid.SetPath(_path);

            // Set type of other points
            tileGrid.SetTileType(_pathStartPoint.x, _pathStartPoint.y, TileType.SpawnPoint);
            tileGrid.SetTileType(_pathEndPoint.x, _pathEndPoint.y, TileType.Home);
        }

        void GenerateObstacles()
        {
            if (_obstaclePrefabs == null)
                return;
            if (_obstaclePrefabs.Count == 0)
                return;

            for(int i = 0; i < _maxObstacles; i++)
            {
                int selectObstacle = Random.Range(0, _obstaclePrefabs.Count);
                Tile tile = tileGrid.GetRandomTile();

                if(tile.type == TileType.Buildable && tile.isEmpty)
                {
                    GameObject obstacle = (GameObject)Instantiate(_obstaclePrefabs[selectObstacle]);
                    Vector3 euler = obstacle.transform.rotation.eulerAngles;
                    euler.y = Random.Range(0, 180);

                    tileGrid.SetTileObstacleObj(tile.position.x, tile.position.z, obstacle, euler, _terrainParent.transform);
                }
            }
        }

        public Tile GetTile(int posX, int posZ)
        {
            return tileGridClone.GetTile(posX, posZ);
        }
    }

    public static class MovePath
    {
        public static Vector2Int moveRight { get { return new Vector2Int(1, 0); } }
        public static Vector2Int moveLeft { get { return new Vector2Int(-1, 0); } }
        public static Vector2Int moveForward { get { return new Vector2Int(0, 1); } }
        public static Vector2Int moveBackward { get { return new Vector2Int(0, -1); } }
    }
}