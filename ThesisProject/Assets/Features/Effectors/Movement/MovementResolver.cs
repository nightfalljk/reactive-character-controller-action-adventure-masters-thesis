using UniRx;
using UnityEngine;

namespace Features.Effectors.Movement
{
    public abstract class MovementResolver : MonoBehaviour
    {
        [SerializeField] protected float groundedThreshold;
        [SerializeField] protected float maxGroundCastDistance;
        [SerializeField] protected LayerMask groundLayer;

        protected ReactiveProperty<Vector3> velocity;
        protected ReactiveProperty<float> distToGround;
        protected ReactiveProperty<float> fallHeight;
        protected ReactiveProperty<bool> grounded;
        protected ReactiveProperty<float> moveSpeed;

        public abstract void Move(Vector3 moveVec, float moveSpeed, float deltaTime);
        public abstract void Jump(float jumpForce);
        public abstract void ResetMoveSpeed();
        public abstract void ToggleGravity();
    
        public ReactiveProperty<Vector3> Velocity => velocity;
        public ReactiveProperty<float> MoveSpeed => moveSpeed;
        public ReactiveProperty<float> DistToGround => distToGround;
        public ReactiveProperty<float> FallHeight => fallHeight;
        public ReactiveProperty<bool> Grounded => grounded;
        protected abstract bool IsGrounded();
    }
}
