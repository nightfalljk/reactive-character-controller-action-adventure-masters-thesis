using System.Collections.Generic;
using Cinemachine;
using Features.Character.Locomotion;
using Features.Effectors.Camera;
using Features.Perception.Sensors;
using Features.StateMachines;
using UnityEngine;

namespace Features.Character.Camera
{
    public class CameraStateMachine : StateMachine
    {
        [SerializeField] private CameraResolver cameraResolver;

        [SerializeField] private CinemachineVirtualCamera aimCam;
        [SerializeField] private CinemachineVirtualCamera defaultCam;
        
        [SerializeField] private SensorSystem sensorSystem;

        [SerializeField] private CameraConfig cameraConfig;
        [SerializeField] private LocomotionStateMachine locomotionStateMachine;
        [SerializeField] private LocomotionConfig locomotionConfig;
        [SerializeField] private GameObject crossHair;

        

        private new void Awake()
        {
            base.Awake();
            stateDict = new Dictionary<int, State>()
            {
                { (int)CameraStates.Default, new DefaultCameraState(this) },
                { (int)CameraStates.Aim, new AimCameraState(this) }
            };
        }
        private void Start()
        {
            SwitchState((int)CameraStates.Default);
        }

        public CameraResolver CameraResolver => cameraResolver;
        public CinemachineVirtualCamera DefaultCam => defaultCam;
        public CinemachineVirtualCamera AimCam => aimCam;

        public SensorSystem SensorSystem => sensorSystem;

        public CameraConfig CameraConfig => cameraConfig;

        public LocomotionConfig LocomotionConfig => locomotionConfig;

        public GameObject CrossHair => crossHair;

        public LocomotionStateMachine LocomotionStateMachine => locomotionStateMachine;

    }
}