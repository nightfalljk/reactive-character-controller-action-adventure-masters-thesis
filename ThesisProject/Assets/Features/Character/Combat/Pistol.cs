using UnityEngine;

namespace Features.Character.Combat
{
    public class Pistol : MonoBehaviour, IWeapon
    {
        [SerializeField] private Transform projectileSpawn;
        [SerializeField] private GameObject projectile;
        [SerializeField] private WeaponType weaponType;
        [SerializeField] private int maxAmmo;
        [SerializeField] private WeaponConfig weaponConfig;

        private int _attackAnimHash;
        private int _currentAmmo;

        private void Awake()
        {
            _attackAnimHash = Animator.StringToHash(weaponConfig.attackAnimName);
            _currentAmmo = maxAmmo;
        }

        public void Attack(Animator animator)
        {
            animator.CrossFadeInFixedTime(_attackAnimHash, 0.1f);
            if (_currentAmmo > 0)
            {
                Vector3 shootVec = projectileSpawn.position - transform.position;
                //Quaternion shootDir = Quaternion.FromToRotation(projectile.transform., shootVec);
                Instantiate(projectile, projectileSpawn.position, projectileSpawn.rotation);
            }
        }

        public void Reload()
        {
            _currentAmmo = maxAmmo;
        }

        public WeaponType GetWeaponType()
        {
            return weaponType;
        }
    }
}