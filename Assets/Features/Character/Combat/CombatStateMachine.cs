using System.Collections.Generic;
using Features.Character.Locomotion;
using Features.Effectors.Animator;
using Features.Perception.Sensors;
using Features.StateMachines;
using UnityEngine;

namespace Features.Character.Combat
{
    public class CombatStateMachine : StateMachine
    {
        [SerializeField] private SensorSystem sensorSystem;
        [SerializeField] private AnimationResolver animationResolver;
        [SerializeField] private LocomotionStateMachine locomotionStateMachine;
        [SerializeField] private Pistol currentWeapon;
        private new void Awake()
        {
            base.Awake();
            stateDict = new Dictionary<int, State>()
            {
                { (int)CombatStates.Melee, new MeleeCombatState(this) },
                { (int)CombatStates.Ranged, new RangedCombatState(this) }
            };
        }

        private void Start()
        {
            SwitchState((int) CombatStates.Melee);
        }

        private new void Update()
        {
            if(sensorSystem.CombatLock.Value) return;
            base.Update();
        }

        public void LockOtherStateMachines()
        {
            sensorSystem.LocomotionLock.Value = true;
        }

        public void UnlockOtherStateMachines()
        {
            sensorSystem.LocomotionLock.Value = false;
        }

        public SensorSystem SensorSystem => sensorSystem;

        public AnimationResolver AnimationResolver => animationResolver;

        public Animator Animator => animationResolver.Animator;

        public LocomotionStateMachine LocomotionStateMachine => locomotionStateMachine;

        public IWeapon CurrentWeapon => currentWeapon;
    }
}