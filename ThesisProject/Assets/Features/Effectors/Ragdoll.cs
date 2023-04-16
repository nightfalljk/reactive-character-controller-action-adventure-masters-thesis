using Features.Effectors.Movement;
using UnityEngine;

namespace Features.Effectors
{
    public class Ragdoll : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Animator animator;
        [SerializeField] private MovementResolver movementResolver;

        private Collider[] _colliders;
        private Rigidbody[] _rigidbodies;

        private void Awake()
        {
            _colliders = GetComponentsInChildren<Collider>(true);
            _rigidbodies = GetComponentsInChildren<Rigidbody>(true);
            DisableRagdoll();
        }

        public void EnableRagdoll()
        {
            ToggleRagdoll(true);
        }

        public void DisableRagdoll()
        {
            ToggleRagdoll(false);
        }

        private void ToggleRagdoll(bool enable)
        {
            foreach (var col in _colliders)
            {
                col.enabled = enable;
            }

            foreach (var rb in _rigidbodies)
            {
                rb.isKinematic = !enable;
                rb.useGravity = enable;
            }

            animator.enabled = !enable;
            movementResolver.enabled = !enable;
        }
    }
}
