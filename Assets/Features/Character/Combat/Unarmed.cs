using UnityEngine;

namespace Features.Character.Combat
{
    public class Unarmed : MonoBehaviour, IWeapon
    {
        private WeaponType _weaponType;
        private WeaponConfig _weaponConfig;

        private int _attackAnimHash;

        private void Awake()
        {
            _attackAnimHash = Animator.StringToHash(_weaponConfig.attackAnimName);
        }

        public void Attack(Animator animator)
        {
            animator.CrossFadeInFixedTime(_attackAnimHash, 0.1f);
        }

        public void Reload()
        {
            
        }

        public WeaponType GetWeaponType()
        {
            return _weaponType;
        }
    }
}