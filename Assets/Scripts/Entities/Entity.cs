using UnityEngine;
using Utility;

namespace Entities
{
    public abstract class Entity : MonoBehaviour
    {
        [SerializeField] protected float _health;
        [SerializeField] protected float _actualSpeed;
        [SerializeField] protected float _mainSpeed;
        [SerializeField] protected bool _isAlive;
        [SerializeField] protected string _entityName;
        [SerializeField] protected Type _type;
        [SerializeField] protected bool _slowDownActive;
        [SerializeField] protected float _slowDownDuration;
        protected TimeCounter _slowDownCounter;

        public virtual void Init(float health, float actualSpeed, float mainSpeed, bool isAlive, string name)
        {
            _health = health; _actualSpeed = actualSpeed ; _mainSpeed = mainSpeed; _isAlive = isAlive; _entityName = name;
        }
        public virtual void GetKilled() { _isAlive = false; Destroy(gameObject); }
        public virtual void GetDamage(float damage) { _health -= damage; }
        public virtual void Heal(float health) { _health += health; }
        public virtual void SlowDown(float speed, float duration)
        { _actualSpeed = _mainSpeed - speed; _slowDownActive = true; _slowDownDuration = duration; }
        public virtual void SpeedUp(float speed) { _actualSpeed += speed; }
        public virtual void SetSpeedNormal() { _actualSpeed = _mainSpeed; }
        public virtual Type GetEntityType() { return _type; }
    }

    public enum Type
    {
        Ground,
        Air,
        AirGround
    }
}