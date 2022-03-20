using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;

namespace Entities.Buildings
{
    public abstract class Building : Entity
    {
        [SerializeField] protected float _price;
        protected Tile _tileInfo;
        public bool isBlueprint { get; set; }

        public virtual void Init() { isBlueprint = false; }
        public virtual void SetPrice(float price) { _price = price; }
        public virtual float GetPrice() { return _price; }
        protected virtual void SetTile(Tile tile) { _tileInfo = tile; }
    }
}