using UnityEngine;

namespace Features.Character.Combat
{
    public interface IWeapon
    {


        public void Attack(Animator animator);
        public void Reload();

        public WeaponType GetWeaponType();
    }
}
