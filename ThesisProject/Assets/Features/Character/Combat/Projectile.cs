using UnityEngine;

namespace Features.Character.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float maxRange;
        [SerializeField] private float projectileSpeed;

        private Vector3 _initPoint;

        private void Update()
        {
           transform.position += Time.deltaTime * projectileSpeed * transform.forward;

            if (Vector3.Magnitude(_initPoint - transform.position) > maxRange)
            {
                Destroy(gameObject);
            }
        }
        public void OnCollisionEnter(Collision collision)
        {
            Debug.Log("Collision");
            Destroy(gameObject);
        }
    }
}