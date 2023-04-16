using Features.Player;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Features.Perception.Input
{
    public class PlayerCharacterControllerInput : CharacterControllerInput
    {
        private PlayerControlInput _playerControlInput;
        private void Awake()
        {
            _playerControlInput = new PlayerControlInput();

            // Camera Input
            lookInput = this.UpdateAsObservable().Select(_ => _playerControlInput.Camera.Look.ReadValue<Vector2>());
            aimInput = new Subject<Unit>();
            
            _playerControlInput.Camera.Aim.started += context => { aimInput.OnNext(Unit.Default); };
            _playerControlInput.Camera.Aim.canceled += context => { aimInput.OnNext(Unit.Default); };
            
            // Locomotion Input
            moveDir = this.UpdateAsObservable()
                .Select(_ => _playerControlInput.BasicLocomotion.Movement.ReadValue<Vector2>());
            sprinting = new Subject<Unit>();
            jump = new Subject<Unit>();
            dodge = new Subject<Unit>();
            crouch = new ReactiveProperty<bool>();

            _playerControlInput.BasicLocomotion.Sprint.performed += ctx => { sprinting.OnNext(Unit.Default); };
            _playerControlInput.BasicLocomotion.Sprint.canceled += ctx => { sprinting.OnNext(Unit.Default); };
            _playerControlInput.BasicLocomotion.Jump.performed += ctx => { jump.OnNext(Unit.Default); };
            _playerControlInput.BasicLocomotion.Dodge.performed += ctx => { dodge.OnNext(Unit.Default); };
            _playerControlInput.BasicLocomotion.Crouch.performed += ctx => { crouch.Value = !crouch.Value; };
            
            // Combat Input
            attack = new Subject<Unit>();
            _playerControlInput.Combat.Attack.performed += context => { attack.OnNext(Unit.Default); };
            
            // Generic Interaction Input
            heal = new Subject<Unit>();
            interact = new Subject<Unit>();
            _playerControlInput.Other.Heal.performed += context => { heal.OnNext(Unit.Default); };
            _playerControlInput.Other.Interact.performed += context => { interact.OnNext(Unit.Default); };
            
        }

        private void OnEnable()
        {
            _playerControlInput.Camera.Enable();
            _playerControlInput.BasicLocomotion.Enable();
            _playerControlInput.Combat.Enable();
            _playerControlInput.Other.Enable();
        }

        private void OnDisable()
        {
            _playerControlInput.Camera.Disable();
            _playerControlInput.BasicLocomotion.Disable();
            _playerControlInput.Combat.Disable();
            _playerControlInput.Other.Disable();
        }
    }
}