using System;
using UniRx;
using UnityEngine;

namespace Features.Perception.Input
{
    public abstract class CharacterControllerInput : MonoBehaviour
    {
        protected IObservable<Vector2> lookInput;
        protected Subject<Unit> aimInput;

        public IObservable<Vector2> LookInput => lookInput;
        public Subject<Unit> Aiming => aimInput;
        
        
        protected Subject<Unit> attack;
        public Subject<Unit> Attack => attack;
        
        
        protected Subject<Unit> interact;
        protected Subject<Unit> heal;

        public Subject<Unit> Interact => interact;
        public Subject<Unit> Heal => heal;
        
        
        protected IObservable<Vector2> moveDir;
        protected Subject<Unit> sprinting;
        protected Subject<Unit> jump;
        protected Subject<Unit> dodge;
        protected ReactiveProperty<bool> crouch;

        public IObservable<Vector2> MoveDir => moveDir;

        public Subject<Unit> Sprinting => sprinting;

        public Subject<Unit> Jump => jump;
        public Subject<Unit> Dodge => dodge;

        public ReactiveProperty<bool> Crouch => crouch;
    }
}