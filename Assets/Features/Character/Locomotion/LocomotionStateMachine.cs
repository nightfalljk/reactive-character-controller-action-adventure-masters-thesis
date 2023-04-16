using System.Collections.Generic;
using Features.Character.Stats;
using Features.Effectors;
using Features.Effectors.Animator;
using Features.Effectors.Movement;
using Features.Perception.Sensors;
using Features.StateMachines;
using UniRx;
using UnityEngine;

namespace Features.Character.Locomotion
{
    public class LocomotionStateMachine : StateMachine
    {
        [SerializeField] private SensorSystem sensorSystem;
        [SerializeField] private MovementResolver movementResolver;
        [SerializeField] private AnimationResolver animationResolver;
        [SerializeField] private Transform avatar;
        [SerializeField] private Health health;
        [SerializeField] private Ragdoll ragdoll;
        [SerializeField] private LocomotionConfig locomotionConfig;

        private new void Awake()
        {
            base.Awake();
            stateDict = new Dictionary<int, State>()
            {
                { (int)LocomotionStates.Idle, new IdleState(this) },
                { (int)LocomotionStates.BasicMove, new BasicLocomotionState(this) },
                { (int)LocomotionStates.Jump, new TakeOffState(this) },
                { (int)LocomotionStates.Air, new AirState(this) },
                { (int)LocomotionStates.Landing, new LandingState(this) },
                { (int)LocomotionStates.Dodge, new DodgeState(this) },
                { (int)LocomotionStates.Dead, new DeathState(this) },
                { (int)LocomotionStates.Vault, new VaultState(this) },
                { (int)LocomotionStates.StepUp, new StepUpState(this) },
                { (int)LocomotionStates.WallRun, new WallRunState(this) },
                { (int)LocomotionStates.Strafe, new StrafeState(this) }
            };
        }
        
        private void Start()
        {
            SwitchState((int)LocomotionStates.Idle);
            SensorSystem.Death.Subscribe(_ =>
            {
                SwitchState((int)LocomotionStates.Dead);
            }).AddTo(this);
        }

        private new void Update()
        {
            if(sensorSystem.LocomotionLock.Value) return;
            base.Update();
        }
        
        public void LockOtherStateMachines()
        {
            sensorSystem.CombatLock.Value = true;
        }

        public void UnlockOtherStateMachines()
        {
            sensorSystem.CombatLock.Value = false;
        }


        public SensorSystem SensorSystem => sensorSystem;

        public Animator Animator => animationResolver.Animator;

        public MovementResolver MovementResolver => movementResolver;

        public Transform Avatar => avatar;

        public LocomotionStates PreviousState { get; set; }

        public Health Health => health;

        public Ragdoll Ragdoll => ragdoll;

        public LocomotionConfig LocomotionConfig => locomotionConfig;

        public AnimationResolver AnimationResolver => animationResolver;
    }
}