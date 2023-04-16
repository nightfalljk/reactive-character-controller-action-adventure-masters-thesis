using System;
using Features.Perception.Sensors;
using Features.StateMachines;
using UniRx;
using UnityEngine;

namespace Features.Character.Locomotion
{
    public class BasicLocomotionState : State
    {
        private LocomotionStateMachine _stateMachine;

        private CompositeDisposable _disposables = new CompositeDisposable();

        private float _currentMoveSpeed;
        private float _currentMaxSpeed;
        private Vector3 _currentMoveVec;
        private Transform _currentViewTransform;
        private bool _sprinting;
        private ObstacleInteraction _currentObstacleInteraction;

        private bool _starting;
        private bool _stopping;
        private bool _turning;
        
        //TODO: CHANGE MOVE SPEED ANIM VAL TO 0-1 THEN CONVERT BASED ON MAX MOVE SPEED TO SET VALUE (MORE FLEXIBLE)
        //TODO: REMOVE ACCELERATION
        private readonly int _runAnimHash = Animator.StringToHash("BasicLocomotion");
        private readonly int _moveSpeedAnimHash = Animator.StringToHash("MoveSpeed");
        private readonly int _runTurn = Animator.StringToHash("Running Turn 180");
        private readonly int _walkTurn = Animator.StringToHash("Walking Turn 180");
        private readonly int _runStop = Animator.StringToHash("Run To Stop");
        private readonly int _walkStop = Animator.StringToHash("Stop Walking");
        private readonly int _sprintStart = Animator.StringToHash("Idle To Sprint");
            
        

        public BasicLocomotionState(LocomotionStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            _currentMoveSpeed = 0;
            _currentMoveVec = Vector3.zero;
            _sprinting = false;
        }
        public override void Enter()
        {
            _disposables = new CompositeDisposable();
            
            _stateMachine.Animator.CrossFadeInFixedTime(_sprintStart, 0.1f);
            _starting = true;

            _stateMachine.SensorSystem.Grounded.Subscribe(grounded =>
            {
                if (!grounded)
                {
                    _stateMachine.SwitchState((int)LocomotionStates.Air);
                }
            }).AddTo(_disposables);

            // If we haven't received any movement input for the last 50 milliseconds we stop
            _stateMachine.SensorSystem.HorizontalMoveDir
                .Where(moveVec => moveVec != Vector2.zero)
                .Throttle(TimeSpan.FromMilliseconds(50))
                .Subscribe(_ =>
                {
                    if (!_stopping)
                    {
                        _stopping = true;
                        if(_currentMoveSpeed <= _stateMachine.LocomotionConfig.walkSpeed)
                            _stateMachine.Animator.CrossFadeInFixedTime(_walkStop, 0.1f);
                        else
                            _stateMachine.Animator.CrossFadeInFixedTime(_runStop, 0.1f);
                    }
                }).AddTo(_disposables);

            _stateMachine.SensorSystem.HorizontalMoveDir
                .Where(moveVec => moveVec != Vector2.zero)
                .Subscribe(moveVec =>
                {
                    if (_stopping)
                    {
                        _stateMachine.Animator.CrossFadeInFixedTime(_runAnimHash, 0.1f);
                        _stopping = false;
                    }
                    
                    Vector2 inputVelocity = moveVec;
                    if (!_sprinting)
                    {
                        var runSpeed = _stateMachine.LocomotionConfig.runSpeed;
                        _currentMaxSpeed = Mathf.Min( runSpeed * inputVelocity.magnitude, runSpeed);
                    }
                    _currentMoveVec = (new Vector3(inputVelocity.x, 0, inputVelocity.y)).normalized;

                }).AddTo(_disposables);
            
            _stateMachine.SensorSystem.HorizontalMoveDir
                .Zip(_stateMachine.SensorSystem.HorizontalMoveDir.Skip(1), (prev, curr) => new { Previous = prev, Current = curr })
                .Where(obj =>
                {
                    // Measure angle between previous and current, currently unreliable
                    if(Vector2.Angle(obj.Previous, obj.Current) > 170)
                        return true;
                    return false;
                })
                .Subscribe(_ =>
                {
                    _turning = true;
                    if(_currentMoveSpeed <= _stateMachine.LocomotionConfig.walkSpeed)
                        _stateMachine.Animator.CrossFadeInFixedTime(_walkTurn, 0.1f);
                    else
                        _stateMachine.Animator.CrossFadeInFixedTime(_runTurn, 0.1f);

                }).AddTo(_disposables);

            _stateMachine.SensorSystem.ViewTransform.Subscribe(viewTransform =>
            {
                _currentViewTransform = viewTransform;
            }).AddTo(_disposables);

            _stateMachine.SensorSystem.Sprint.Subscribe(_ =>
            {
                _sprinting = true;
                if (_sprinting)
                {
                    _currentMaxSpeed = _stateMachine.LocomotionConfig.sprintSpeed;
                }
            }).AddTo(_disposables);
            
            _stateMachine.SensorSystem.Jump.Subscribe(_ =>
            {
                switch (_stateMachine.SensorSystem.CurrentObstacleInteraction.Value)
                {
                    
                    case ObstacleInteraction.Vault:
                        _stateMachine.SwitchState((int) LocomotionStates.Vault);
                        break;
                    case ObstacleInteraction.StepUp:
                        _stateMachine.SwitchState((int) LocomotionStates.StepUp);
                        break;
                    case ObstacleInteraction.ClimbUp:
                        _stateMachine.SwitchState((int) LocomotionStates.WallRun);
                        break;
                    default:
                        _stateMachine.SwitchState((int)LocomotionStates.Jump);
                        break;
                }

            }).AddTo(_disposables);

            _stateMachine.SensorSystem.Dodge.Subscribe(_ =>
            {
                _stateMachine.SwitchState((int)LocomotionStates.Dodge);
            }).AddTo(_disposables);

            _stateMachine.SensorSystem.Crouch.Subscribe(_ =>
            {
                _currentMaxSpeed = _stateMachine.LocomotionConfig.crouchSpeed;
            }).AddTo(_disposables);

            _stateMachine.SensorSystem.AnimationFinished.Subscribe(_ =>
            {
                if (_turning)
                {
                    _turning = false;
                    _stateMachine.Animator.CrossFadeInFixedTime(_runAnimHash, 0.1f);
                }
                else if (_starting && !_stopping)
                {
                    _starting = false;
                    _stateMachine.Animator.CrossFadeInFixedTime(_runAnimHash, 0.1f);
                }
                else if (_stopping)
                {
                    _stopping = false;
                    _starting = false;
                    _stateMachine.SwitchState((int) LocomotionStates.Idle);
                }
            }).AddTo(_disposables);
        }

        public override void Tick(float deltaTime)
        {
            if (_turning || _stopping) return;
            if(_currentMoveVec == Vector3.zero)
                return;
            CalculateMoveSpeed(deltaTime);
            var moveVec = CalculateMoveVec();
            if(!_starting)
                _stateMachine.MovementResolver.Move(moveVec, _currentMoveSpeed, deltaTime);
            //TODO: ROTATION BASED ON MOVE SPEED -> LESS ROTATE WHEN FAST
            _stateMachine.Avatar.rotation = Quaternion.Lerp(_stateMachine.Avatar.rotation,
                Quaternion.LookRotation(moveVec), deltaTime * 10);
            
            _stateMachine.Animator.SetFloat(_moveSpeedAnimHash, _currentMoveSpeed, 0.1f, deltaTime);
        }

        private void CalculateMoveSpeed(float deltaTime)
        {
            float acceleration = _stateMachine.LocomotionConfig.acceleration;
            if (_currentMoveSpeed <= _currentMaxSpeed)
            {
                _currentMoveSpeed = Mathf.Min(_currentMoveSpeed + acceleration * deltaTime, _currentMaxSpeed);
            }
            else
            {
                _currentMoveSpeed = Mathf.Max(_currentMoveSpeed - acceleration * deltaTime, _currentMaxSpeed);
            }
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
            _stateMachine.PreviousState = LocomotionStates.BasicMove;
            _stateMachine.MovementResolver.ResetMoveSpeed();
            _sprinting = false;
            _turning = false;
            _starting = false;
            _stopping = false;
            _currentMoveVec = Vector3.zero;
            _disposables.Dispose();
        }
    }
}