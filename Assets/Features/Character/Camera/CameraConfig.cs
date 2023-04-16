using UnityEngine;

namespace Features.Character.Camera
{
    [CreateAssetMenu(fileName = "Camera/CameraConfig")]
    public class CameraConfig : ScriptableObject
    {
        public float upAngle;
        public float downAngle;
        public float rotateSpeed;
        public float aimRotateSpeed;
        public float minDefaultCamDistance;
        public float maxDefaultCamDistance;
        public float defaultCamDistance;
        public float defaultCamZoomSpeed;
        public float defaultPosReturnDuration;
    }
}