using UnityEngine;

namespace Entities
{
    public class Projectile : Entity
    {
        GameObject _targetEnemy;
        float _doHitPoint;
        float _doSlowDown = 0f;

        void Update()
        {
            if (_isAlive)
            {
                if (_targetEnemy == null)
                {
                    GetKilled();
                    return;
                }
                if (Vector3.Distance(transform.position, _targetEnemy.transform.position) > 0.4f)
                {
                    Vector3 lookAt = _targetEnemy.transform.position;
                    lookAt.y = transform.position.y;
                    Vector3 direction = (_targetEnemy.transform.position - transform.position).normalized;
                    transform.Translate(direction * _mainSpeed * Time.deltaTime, Space.World);
                    transform.LookAt(lookAt);
                }
                else
                {
                    Enemy enemy = _targetEnemy.GetComponent<Enemy>();
                    enemy.GetDamage(_doHitPoint);
                    if (_doSlowDown != 0)
                    {
                        enemy.SlowDown(_doSlowDown, _slowDownDuration);
                    }
                    GetKilled();
                }
            }
        }

        public void Init(GameObject enemy, float hitPoint, float speed, float slowDown, float slowDownDuration)
        {
            _targetEnemy = enemy;
            _doHitPoint = hitPoint;
            _isAlive = true;
            _actualSpeed = speed;
            _mainSpeed = speed;
            _doSlowDown = slowDown;
            _slowDownDuration = slowDownDuration;
        }
    }
}