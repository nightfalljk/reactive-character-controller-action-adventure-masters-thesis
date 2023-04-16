using UnityEngine;

namespace Features.Perception.Sensors
{
    [CreateAssetMenu(fileName = "SensorConfig", menuName = "Character/Sensors/New Sensor Config", order = 0)]
    public class SensorConfig : ScriptableObject
    {
        public float maxClimbHeight;
        public float maxStepUpHeight;
        public float vaultGroundHeightThreshold;
        public float vaultHeight;
        public float vaultDistance;

        public float detectionRange;
        public float detectionRangeMoveSpeedModifier;
        public float detectionCastRadius;
    }
}