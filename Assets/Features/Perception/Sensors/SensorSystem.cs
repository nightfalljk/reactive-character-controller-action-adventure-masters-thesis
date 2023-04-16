using System;
using Features.Character.Stats;
using Features.Effectors.Animator;
using Features.Effectors.Camera;
using Features.Effectors.Movement;
using Features.Perception.Input;
using UniRx;
using UnityEngine;

namespace Features.Perception.Sensors
{
    public class SensorSystem : MonoBehaviour
    {
        [SerializeField] private CharacterControllerInput characterControllerInput;
        [SerializeField] private MovementResolver movementResolver;
        [SerializeField] private AnimationResolver animationResolver;
        [SerializeField] private EnvironmentSensor environmentSensor;
        [SerializeField] private CameraResolver cameraResolver;
        [SerializeField] private Health health;

        private RaycastHit _closestSensorObject;

        private ReactiveProperty<Vector3> _characterForward;
        private ReactiveProperty<Vector3> _characterRight;

        private ReactiveProperty<bool> _locomotionLock;
        private ReactiveProperty<bool> _combatLock;

        public IObservable<Vector2> HorizontalMoveDir => characterControllerInput.MoveDir;
        public ReactiveProperty<float> MoveSpeed => movementResolver.MoveSpeed;
        public ReactiveProperty<bool> Grounded => movementResolver.Grounded;
        public ReactiveProperty<float> FallHeight => movementResolver.FallHeight;
        public IObservable<Vector2> LookInput => characterControllerInput.LookInput;
        public Subject<Unit> Aim => characterControllerInput.Aiming;
        public IObservable<Transform> ViewTransform => cameraResolver.ViewTransform;
        public ReactiveProperty<Vector3> CharacterForward => _characterForward;
        public ReactiveProperty<Vector3> CharacterRight => _characterRight;
        public Subject<Unit> Sprint => characterControllerInput.Sprinting;
        public Subject<Unit> Jump => characterControllerInput.Jump;
        public Subject<Unit> Dodge => characterControllerInput.Dodge;
        public Subject<Unit> Attack => characterControllerInput.Attack;
        public ReactiveProperty<bool> Crouch => characterControllerInput.Crouch;
        public ReactiveProperty<int> Health => health.GetHealth;
        public Subject<Unit> Death => health.Death;
        public Subject<Unit> Heal => characterControllerInput.Heal;
        public Subject<Unit> Interact => characterControllerInput.Interact;
        public ReactiveProperty<bool> InteractableInView => environmentSensor.InteractableInView;
        public ReactiveProperty<Vector3> ObstacleInteractionEndPos => environmentSensor.ObstacleInteractionEndPos;
        public ReactiveProperty<ObstacleInteraction> CurrentObstacleInteraction => environmentSensor.CurrentObstacleInteraction;
        public ReactiveProperty<bool> LocomotionLock => _locomotionLock;
        public ReactiveProperty<bool> CombatLock => _combatLock;
        public ReactiveProperty<GroundType> GroundType => environmentSensor.GroundType;
        public ReactiveProperty<CliffDistance> CliffDistance => environmentSensor.CliffDistance;
        public Subject<Unit> AnimationFinished => animationResolver.AnimationFinished;
        public Subject<Unit> LeftStep => environmentSensor.LeftStepEvent;
        public Subject<Unit> RightStep => environmentSensor.RightStepEvent;
        public Subject<Unit> Landing => environmentSensor.LandingEvent;

        private void Awake()
        {
            _characterForward = new ReactiveProperty<Vector3>();
            _characterRight = new ReactiveProperty<Vector3>();

            _combatLock = new ReactiveProperty<bool>();
            _locomotionLock = new ReactiveProperty<bool>();
        }

        private void Start()
        {
            Interact.Subscribe(_ => { environmentSensor.CurrentInteractable?.Interact(); }).AddTo(this);
        }

        private void Update()
        {
            var characterTransform = transform;
            _characterForward.Value = characterTransform.forward;
            _characterRight.Value = characterTransform.right;
        }
        
    }
}
