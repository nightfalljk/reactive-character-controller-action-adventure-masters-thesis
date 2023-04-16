using UniRx;
using UnityEngine;

namespace Features.Effectors.Movement
{
    public class CharacterControllerMovementResolver : MovementResolver
    {
        private const float FallThreshold = 2;
        
        private CharacterController _characterController;
        
        private float _verticalVelocity;
        private bool _gravityEnabled;
        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _verticalVelocity = Physics.gravity.y;
            velocity = new ReactiveProperty<Vector3>();
            distToGround = new ReactiveProperty<float>();
            velocity.Value = _characterController.velocity;
            fallHeight = new ReactiveProperty<float>();
            grounded = new ReactiveProperty<bool>();
            moveSpeed = new ReactiveProperty<float>();

            _gravityEnabled = true;


            distToGround
                .Zip(distToGround.Skip(1), (prev, curr) => new {Previous = prev, Current = curr})
                .Subscribe(obj =>
                {
                    if (obj.Current - obj.Previous > FallThreshold)
                    {
                        fallHeight.Value = obj.Current;
                    }
                })
                .AddTo(this);
        }

        private void Update()
        {
            velocity.Value = _characterController.velocity;
            RaycastHit hit = new RaycastHit();
            Physics.Raycast(transform.position, Vector3.down, out hit, maxGroundCastDistance, groundLayer);
            distToGround.Value = hit.distance;
            grounded.Value = distToGround.Value < groundedThreshold;
            
            if(!_gravityEnabled) return;

            if (_verticalVelocity < 0 && IsGrounded())
            {
                _verticalVelocity = Physics.gravity.y * Time.deltaTime;
            }
            else
            {
                _verticalVelocity += Physics.gravity.y * Time.deltaTime;
            }
        }

        public override void Move(Vector3 moveVec, float moveSpeed, float deltaTime)
        {
            base.moveSpeed.Value = moveSpeed;
            Vector3 move = moveVec * moveSpeed + Vector3.up  * _verticalVelocity;
            _characterController.Move(move * deltaTime);
        }

        public override void Jump(float jumpForce)
        {
            _verticalVelocity += jumpForce;
        }

        public override void ResetMoveSpeed()
        {
            moveSpeed.Value = 0;
        }

        public override void ToggleGravity()
        {
            if (_gravityEnabled)
            {
                _verticalVelocity = 0;
            }
            else
            {
                _verticalVelocity = Physics.gravity.y * Time.deltaTime;
            }
            _gravityEnabled = !_gravityEnabled;
        }

        protected override bool IsGrounded()
        {
            return grounded.Value;
        }

        private void OnEnable()
        {
            _characterController.detectCollisions = true;
            _characterController.enabled = true;
        }

        private void OnDisable()
        {
            _characterController.detectCollisions = false;
            _characterController.enabled = false;
        }
    }
}