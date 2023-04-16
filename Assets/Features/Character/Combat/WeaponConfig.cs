using UnityEngine;

namespace Features.Character.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Create new Weapon")]
    public class WeaponConfig : ScriptableObject
    {
        public string attackAnimName;
        public float damage;
    }
}