using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;
using Generator;
using Entities.Buildings;
using Managers;

namespace BuildSystem
{
    public class Build : MonoBehaviour
    {
        public static Build instance { get; private set; }

        [SerializeField] Object _selectedBuilding;
        [SerializeField] GameObject _showBuildingBlueprint;
        [SerializeField] Tile _selectedTile;
        [SerializeField] Vector3 _tilePos;
        [SerializeField] bool _showBlueprint;

        [SerializeField] Object _buildEffect;
        LayerMask _tileLayer;

        private void Awake()
        {
            instance = this;
        }

        void Start()
        {
            _tileLayer = MapGenerator.instance._tileLayer;
            _showBlueprint = false;
        }

        void Update()
        {
            if(_showBlueprint)
            {
                if (Input.GetButtonDown("Fire2"))
                {
                    CancelBuild();
                    return;
                }

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 1000, _tileLayer))
                {
                    if (hit.collider.CompareTag("Tile"))
                    {
                        _tilePos = hit.collider.transform.position;
                        _selectedTile = MapGenerator.instance.GetTile((int)_tilePos.x, (int)_tilePos.z);

                        if (_selectedTile != null)
                        {
                            if (_selectedTile.isEmpty && _selectedTile.type == TileType.Buildable)
                            {
                                _showBuildingBlueprint.transform.position = _tilePos;
                                _showBuildingBlueprint.SetActive(true);

                                if (Input.GetButtonDown("Fire1"))
                                {
                                    _showBuildingBlueprint.SetActive(false);
                                    DoneBuild();
                                    return;
                                }
                            }
                            else
                            {
                                _showBuildingBlueprint.SetActive(false);
                            }
                        }
                        else
                            _showBuildingBlueprint.SetActive(false);
                    }
                }
                else
                    _showBuildingBlueprint.SetActive(false);
            }
        }

        public void SelectBuilding(Object prefab)
        {
            if (prefab == null)
            {
                Debug.LogError("Building Prefab is null!");
                return;
            }

            if (!(GameManager.instance.gameState == GameState.Play))
                return;

            _selectedBuilding = prefab;
            _showBuildingBlueprint = (GameObject)Instantiate(prefab);
            _showBuildingBlueprint.SetActive(false);
            Building building = _showBuildingBlueprint.GetComponent<Building>();
            building.isBlueprint = true;
            if (building.GetPrice() <= GameManager.instance.Gold)
                ShowBlueprint();
            else
                CancelBuild();
        }

        public void DoneBuild()
        {
            if (_selectedBuilding == null)
            {
                Debug.LogError("Building Prefab is null!");
                return;
            }
            CancelBuild();

            _selectedTile.obstacleObj = (GameObject)Instantiate(_selectedBuilding, _selectedTile.position, Quaternion.identity) ;
            _selectedTile.obstacleObj.GetComponent<Building>().Init();
            _selectedTile.isEmpty = false;

            EventManager.instance.onBuildingDone((int)_selectedTile.obstacleObj.GetComponent<Building>().GetPrice());
        }

        public void CancelBuild()
        {
            HideBlueprint();
            DestroyImmediate(_showBuildingBlueprint);
        }

        public void ShowBlueprint()
        {
            _showBlueprint = true;
        }

        public void HideBlueprint()
        {
            _showBlueprint = false;
        }
    }
}