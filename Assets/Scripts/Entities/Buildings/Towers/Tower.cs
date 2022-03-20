using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Entities.Buildings.Towers
{
    public class Tower : Building
    {
        SphereCollider _collider;
        List<Transform> _nearEnemies;
        TowerState _towerState;
        TimeCounter _shotCooldownTimer;
        GameObject _enemyTarget;
        float _rotateAngle = 0f;

        [Header("Tower Options")]
        [SerializeField] float _fireCooldown = 0.5f;
        [SerializeField] float _hitPower = 1f;
        [SerializeField] float _slowDownPower = 0f;
        [SerializeField] float _detectRadius = 2f;
        [SerializeField] float _rotationSpeed = 1f;
        [SerializeField] bool _alwaysRotate = false;

        [Header("Projectile")]
        [SerializeField] GameObject _projectile;
        [SerializeField] Transform _projectileSpawnPoint;
        [SerializeField] Transform _projectileGun;

        public override void Init()
        {
            if (!gameObject.TryGetComponent<SphereCollider>(out _collider))
                _collider = gameObject.AddComponent<SphereCollider>();

            _collider.isTrigger = true;
            _collider.radius = _detectRadius;
            Vector3 collCenter = _collider.center;
            collCenter.y = 1;
            _collider.center = collCenter;


            _nearEnemies = new List<Transform>();
            _enemyTarget = null;

            if (_projectile == null)
                Debug.LogError(gameObject.name + " No Projectile Assigned!");
            if (_projectileSpawnPoint == null)
                Debug.LogError(gameObject.name + " No Projectile SpawnPoint Assigned!");
            if (_projectileGun == null)
                Debug.LogError(gameObject.name + " No Projectile Gun Assigned!");

            _shotCooldownTimer = new TimeCounter(_fireCooldown);
        }

        void Update()
        {
            if (!isBlueprint)
            {
                Decision();
            }
        }

        void Decision()
        {
            switch(_towerState)
            {
                case TowerState.Idle:
                    ClearDeadEnemies();
                    if (_nearEnemies.Count > 0)
                        _towerState = TowerState.ChooseTarget;
                    if(_alwaysRotate)
                    {
                        _rotateAngle = Time.deltaTime * _rotationSpeed;
                        RotateGun(_rotateAngle);
                    }
                    _shotCooldownTimer.SetDelayedTime(_fireCooldown - 0.1f);
                    break;
                case TowerState.ChooseTarget:
                    ClearDeadEnemies();
                    if (_nearEnemies.Count > 0)
                    {
                        if (_nearEnemies[0].gameObject == null)
                        {
                            _towerState = TowerState.Idle;
                            break;
                        }
                        _enemyTarget = _nearEnemies[0].gameObject;

                        _towerState = TowerState.Shooting;
                    }
                    break;
                case TowerState.Shooting:
                    ClearDeadEnemies();
                    if (_enemyTarget == null)
                    {
                        _towerState = TowerState.Idle;
                        break;
                    }
                    if (_nearEnemies.Contains(_enemyTarget.transform))
                    {
                        GunLookAtTarget();
                        if(_shotCooldownTimer.IsTimeUp())
                        {
                            GameObject projectile = Instantiate(_projectile, _projectileSpawnPoint.position, Quaternion.identity);
                            projectile.GetComponent<Projectile>().Init(_enemyTarget, _hitPower, _mainSpeed, _slowDownPower, _fireCooldown);
                        }
                    }
                    else
                        _towerState = TowerState.Idle;
                    break;
            }
        }

        public void GunLookAtTarget()
        {
            Vector3 lookAt = _enemyTarget.transform.position;
            lookAt.y = _projectileGun.position.y;
            _projectileGun.LookAt(lookAt);
        }

        public void RotateGun(float angle)
        {
            _projectileGun.Rotate(Vector3.up, angle);
        }

        private void ClearDeadEnemies()
        {
            List<int> removeList = new List<int>();

            int i = 0;
            foreach(Transform enemy in _nearEnemies)
            {
                if (enemy == null)
                    removeList.Add(i);
                i++;
            }
            if (removeList.Count == 0)
                return;

            for(i = 0; i < removeList.Count; i++)
            {
                _nearEnemies.RemoveAt(i);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Entity entity = other.GetComponent<Enemy>();
            if(entity)
            {
                if(_type == Type.AirGround || entity.GetEntityType() == _type)
                    _nearEnemies.Add(entity.transform);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Entity entity = other.GetComponent<Enemy>();
            if (entity)
                _nearEnemies.Remove(entity.transform);
        }
    }

    enum TowerState
    {
        Idle,
        ChooseTarget,
        Shooting
    }
}