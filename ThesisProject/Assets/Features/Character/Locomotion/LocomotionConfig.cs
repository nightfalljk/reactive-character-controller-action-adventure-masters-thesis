using UnityEngine;

namespace Features.Character.Locomotion
{
    [CreateAssetMenu(fileName = "LocomotionConfig", menuName = "Character/Locomotion/New LocomotionConfig")]
    public class LocomotionConfig : ScriptableObject
    {
        public float walkSpeed;
        public float runSpeed;
        public float sprintSpeed;
        public float crouchSpeed;
        public float acceleration;
        public float strafeSpeed;

        public float groundedThreshold;

        public float airSpeed;
        public float deadlyFallHeight;
        public float rollLandingFallHeight;
        public float heavyLandingFallHeight;

    }
}