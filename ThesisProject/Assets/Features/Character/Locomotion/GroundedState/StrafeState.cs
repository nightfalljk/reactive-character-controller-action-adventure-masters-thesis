using Features.StateMachines;
using UniRx;
using UnityEngine;

namespace Features.Character.Locomotion
{
    public class StrafeState : State
    {
        private LocomotionStateMachine _stateMachine;
        
        private readonly int _strafeAnimHash = Animator.StringToHash("Strafe");
        private readonly int _moveSpeedAnimHash = Animator.StringToHash("MoveSpeed");
        private readonly int _forwardAnimHash = Animator.StringToHash("Forward");
        private readonly int _rightAnimHash = Animator.StringToHash("Right");
        
        private CompositeDisposable _disposables;
        
        private float _currentMoveSpeed;
        private float _currentMaxSpeed;
        private Vector3 _currentMoveVec;
        private Transform _currentViewTransform;
        
        public StrafeState(LocomotionStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        
        public override void Enter()
        {
            _disposables = new CompositeDisposable();
            
            _stateMachine.Animator.CrossFadeInFixedTime(_strafeAnimHash, 0.1f);
            
            _stateMachine.SensorSystem.HorizontalMoveDir.Subscribe(moveInput =>
            {
                _stateMachine.Animator.SetFloat(_forwardAnimHash, moveInput.y);
                _stateMachine.Animator.SetFloat(_rightAnimHash, moveInput.x);

                if (moveInput == Vector2.zero)
                {
                    _currentMoveVec = Vector3.zero;
                    _currentMaxSpeed = 0f;
                    _stateMachine.Animator.SetFloat(_moveSpeedAnimHash, 0);
                    return;
                }
                
                Vector2 inputVelocity = moveInput;
                var strafeSpeed = _stateMachine.LocomotionConfig.strafeSpeed;
                _currentMaxSpeed = Mathf.Min( strafeSpeed * inputVelocity.magnitude, strafeSpeed);
                
                _currentMoveVec = (new Vector3(inputVelocity.x, 0, inputVelocity.y)).normalized;

            }).AddTo(_disposables);
            
            _stateMachine.SensorSystem.ViewTransform.Subscribe(viewTransform =>
            {
                _currentViewTransform = viewTransform;
            }).AddTo(_disposables);
        }

        public override void Tick(float deltaTime)
        {
            //TODO: RESTRICT THIS ROTATION HORIZONTALLY; MAYBE ADD MOVING ARM FOR AIM OR SOMETHING?
            if(_currentViewTransform != null)
                _stateMachine.Avatar.rotation = Quaternion.Lerp(_stateMachine.Avatar.rotation,
                    Quaternion.LookRotation(_currentViewTransform.forward), deltaTime * 10);
            if(_currentMoveVec == Vector3.zero)
                return;
            CalculateMoveSpeed(deltaTime);
            var moveVec = CalculateMoveVec();
            _stateMachine.MovementResolver.Move(moveVec, _currentMoveSpeed, deltaTime);
            
            _stateMachine.Animator.SetFloat(_moveSpeedAnimHash, _currentMoveSpeed, 0.1f, deltaTime);
        }

        private void CalculateMoveSpeed(float deltaTime)
        {
            float acceleration = _stateMachine.LocomotionConfig.acceleration;
            _currentMoveSpeed = Mathf.MoveTowards(_currentMoveSpeed, _currentMaxSpeed, acceleration * deltaTime);
        }

        private Vector3 CalculateMoveVec()
        {
            Vector3 viewForward = _currentViewTransform.forward.normalized;
            Vector3 viewRight = _currentViewTransform.right.normalized;
            viewForward.y = 0;
            viewRight.y = 0;
            var moveVec = (viewForward * _currentMoveVec.z + viewRight * _currentMoveVec.x).normalized;
            return moveVec;
        }

        public override void Exit()
        {
            _disposables.Dispose();
        }
    }
}