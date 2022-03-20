using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Generator;
using Utility;

namespace Entities
{
    public class Enemy : Entity
    {
        [SerializeField] List<Vector3> _path;
        [SerializeField] int _pathTargetPointIndex;
        [SerializeField] Vector3 _direction;
        [SerializeField] EnemyState _enemyState;
        [SerializeField] int _gold;
        [SerializeField] int _score;
        [SerializeField] int _damage;
        
        void Start()
        {
            _enemyState = EnemyState.Spawned;
            _pathTargetPointIndex = 0;

            _path = new List<Vector3>(MapGenerator.instance.Path);
        }

        void Update()
        {
            Decision();
        }

        void Decision()
        {
            switch(_enemyState)
            {
                case EnemyState.Spawned:
                    transform.position = _path[0];
                    SetState(EnemyState.FindingDirection);
                    break;
                case EnemyState.FindingDirection:
                    _pathTargetPointIndex++;
                    _direction = (_path[_pathTargetPointIndex] - transform.position).normalized;
                    transform.LookAt(_path[_pathTargetPointIndex]);
                    SetState(EnemyState.Moving);
                    break;
                case EnemyState.Moving:
                    transform.Translate(_direction * _actualSpeed * Time.deltaTime, Space.World);
                    if (_slowDownActive)
                    {
                        if (_slowDownCounter.IsTimeUp())
                        {
                            SetSpeedNormal();
                        }
                    }
                    if (Vector3.Distance(transform.position, _path[_pathTargetPointIndex]) <= 0.2f)
                    {   
                        if (_pathTargetPointIndex == _path.Count - 1)
                        {
                            SetState(EnemyState.HitBase);
                        }
                        else
                        {
                            SetState(EnemyState.FindingDirection);
                        }
                    }
                    if(_health <= 0f)
                    {
                        SetState(EnemyState.Dead);
                    }
                    break;
                case EnemyState.HitBase:
                    EventManager.instance.onBaseGetHit?.Invoke(_damage);
                    GetKilled();
                    break;
                case EnemyState.Dead:
                    EventManager.instance.onEnemyKilled?.Invoke(_score, _gold);
                    GetKilled();
                    break;
            }
        }

        public void SetState(EnemyState state)
        {
            _enemyState = state;
        }

        public override void SlowDown(float speed, float duration)
        {
            base.SlowDown(speed, duration);
            _slowDownCounter = new TimeCounter(duration, false);
            _slowDownCounter.Start();
        }
    }

    public enum EnemyState
    {
        Spawned,
        FindingDirection,
        Moving,
        Dead,
        HitBase
    }
}