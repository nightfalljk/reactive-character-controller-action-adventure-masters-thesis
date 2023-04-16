using Features.Character.Interaction;
using Features.Player;
using UniRx;
using UnityEngine;

namespace Features.Perception.Sensors
{
    public class EnvironmentSensor : MonoBehaviour
    {
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private Transform lowObstacleCastOrigin;
        [SerializeField] private float obstacleDetectionRange;
        [SerializeField] private float obstacleDetectionRadius;
        [SerializeField] private SensorConfig sensorConfig;
        
        [SerializeField] private Transform detectionCastOrigin;
        [SerializeField] private float detectionRange;
        [SerializeField] private float detectionRadius;
        
        [SerializeField] private Transform closeCliffDetector;
        [SerializeField] private Transform farCliffDetector;
        
        private ReactiveProperty<bool> _interactableInView;
        private IInteractable _currentInteractable;
        private ReactiveProperty<ObstacleInteraction> _currentObstacleInteraction;
        private ReactiveProperty<Vector3> _obstacleInteractionEndPos;
        
        private ReactiveProperty<GroundType> _groundType;

        private Subject<Unit> _leftStep;
        private Subject<Unit> _rightStep;
        private Subject<Unit> _landing;

        private ReactiveProperty<CliffDistance> _cliffDistance;

        public ReactiveProperty<bool> InteractableInView => _interactableInView;
        public IInteractable CurrentInteractable => _currentInteractable;
        public ReactiveProperty<ObstacleInteraction> CurrentObstacleInteraction => _currentObstacleInteraction;
        public ReactiveProperty<Vector3> ObstacleInteractionEndPos => _obstacleInteractionEndPos;

        public ReactiveProperty<GroundType> GroundType => _groundType;
        public Subject<Unit> LeftStepEvent => _leftStep;
        public Subject<Unit> RightStepEvent => _rightStep;
        public Subject<Unit> LandingEvent => _landing;

        public ReactiveProperty<CliffDistance> CliffDistance => _cliffDistance;


        private void Awake()
        {
            _interactableInView = new ReactiveProperty<bool>();
            _interactableInView.Value = false;
            
            _currentObstacleInteraction = new ReactiveProperty<ObstacleInteraction>();
            _currentObstacleInteraction.Value = ObstacleInteraction.Jump;
            _obstacleInteractionEndPos = new ReactiveProperty<Vector3>();

            _groundType = new ReactiveProperty<GroundType>();
            _groundType.Value = Sensors.GroundType.Default;
            _cliffDistance = new ReactiveProperty<CliffDistance>();
            
            _leftStep = new Subject<Unit>();
            _rightStep = new Subject<Unit>();
            _landing = new Subject<Unit>();
        }

        private void Update()
        {
            Debug.Log("Scanning environment");
            Debug.DrawRay(_obstacleInteractionEndPos.Value, Vector3.up * 10, Color.magenta );
            PollEnvironment();
            CheckForInteractable();
            CheckGroundType();
            //DetectCliff();
        }

        public void CheckGroundType()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out var groundHit, 1f, obstacleLayer))
            {
                switch (groundHit.transform.tag)
                {
                    case "Stone":
                        _groundType.Value = Sensors.GroundType.Stone;
                        break;
                    default:
                        _groundType.Value = Sensors.GroundType.Default;
                        break;
                }
            }
                
        }

        /*private void DetectCliff()
        {
            CliffDistance cliffDistance = Perception.Sensors.CliffDistance.None;
            RaycastHit groundHit;
            if (!Physics.SphereCast(farCliffDetector.position, 0.2f, Vector3.down, out groundHit, 1, obstacleLayer))
            {
                cliffDistance = Perception.Sensors.CliffDistance.Far;
            }
            if (!Physics.SphereCast(closeCliffDetector.position, 0.2f, Vector3.down, out groundHit, 1, obstacleLayer))
            {
                if(cliffDistance == Perception.Sensors.CliffDistance.Far)
                    cliffDistance = Perception.Sensors.CliffDistance.Close;
            }
            
            _cliffDistance.Value = cliffDistance;
            Debug.Log("Cliff Distance: " + cliffDistance);
        }*/

        public void LeftStep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight < 0.5f) return;
            _leftStep.OnNext(Unit.Default);

        }
        
        public void RightStep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight < 0.5f) return;
            _rightStep.OnNext(Unit.Default);
        }

        public void Landing()
        {
            _landing.OnNext(Unit.Default);
        }
        
        private void PollEnvironment()
        {
            if (Physics.SphereCast(lowObstacleCastOrigin.position, obstacleDetectionRadius,
                    lowObstacleCastOrigin.forward, out var obstacleHit, obstacleDetectionRange, obstacleLayer))
            {
                //TODO: Target distances based on movespeed
                //TODO: Cast Distance based on movespeed 
                //TODO: Put this in a class
                Vector3 characterPosition = transform.position;
                Vector3 obstacleTraverseDirection = -obstacleHit.normal * 0.5f;

                Vector3 traverseOrigin = characterPosition + Vector3.up * sensorConfig.vaultHeight;
                Debug.DrawRay(traverseOrigin, obstacleTraverseDirection * sensorConfig.vaultDistance, Color.red);
                
                Vector3 obstacleOrigin = obstacleHit.point;
                obstacleOrigin.y = characterPosition.y;
                //Max vault height cast
                
                
                if (Physics.Raycast(traverseOrigin, obstacleTraverseDirection, out var obstacleCast, 2, obstacleLayer))
                {
                    Debug.Log("Hit wall");
                    //TODO: Check height of wall and climb up if close enough
                    Vector3 obstacleMaxClimbHeightPos = obstacleOrigin + Vector3.up * sensorConfig.maxClimbHeight;
                    if (Physics.Raycast(obstacleMaxClimbHeightPos, Vector3.down, out obstacleCast, sensorConfig.maxClimbHeight, obstacleLayer))
                    {
                        float hitY = obstacleCast.point.y;
                        float diff = hitY - characterPosition.y;
                        if (diff < sensorConfig.maxClimbHeight && diff > 0)
                        {
                            SetUpClimbInteraction(obstacleCast);
                            _obstacleInteractionEndPos.Value += obstacleTraverseDirection;
                            return;
                        }
                        Debug.Log("Wall is not climbable");
                    }

                }
                else
                {
                    Vector3 obstacleMaxVaultHeightPos = obstacleOrigin + Vector3.up * sensorConfig.vaultHeight;
                    obstacleMaxVaultHeightPos += obstacleTraverseDirection * sensorConfig.vaultDistance;
                    Debug.DrawRay(traverseOrigin, -Vector3.up * sensorConfig.vaultHeight, Color.blue);
                    if (Physics.Raycast(obstacleMaxVaultHeightPos, Vector3.down, out obstacleCast, sensorConfig.vaultHeight+0.1f, obstacleLayer))
                    {
                        Debug.DrawRay(obstacleMaxVaultHeightPos, Vector3.down * sensorConfig.vaultHeight, Color.green);
                        float hitY = obstacleCast.point.y;
                        float diff = hitY - characterPosition.y;

                        if (Mathf.Abs(diff) < sensorConfig.vaultGroundHeightThreshold)
                        {
                            SetUpVaultInteraction(obstacleCast, obstacleOrigin, obstacleHit);
                            return;
                        }
                        Debug.Log("Checking step up");
                        if (diff > 0 && diff < sensorConfig.maxStepUpHeight)
                        {
                            SetUpStepUpInteraction(obstacleOrigin, obstacleHit);
                            return;
                        }
                    }
                }
            }

            _currentObstacleInteraction.Value = ObstacleInteraction.Jump;
        }

        private void SetUpClimbInteraction(RaycastHit obstacleCast)
        {
            Debug.Log("Wall is climbable");
            //TODO: THis should be same height as hand and foot positions => use this as IK target
            //TODO: Additionally use wall contact at chest height for foot IK
            _obstacleInteractionEndPos.Value = obstacleCast.point;
            _currentObstacleInteraction.Value = ObstacleInteraction.ClimbUp;
            return;
        }

        private void SetUpStepUpInteraction(Vector3 obstacleOrigin, RaycastHit obstacleHit)
        {
            Vector3 handPositionCastOrigin = obstacleOrigin + Vector3.up * sensorConfig.vaultHeight;
            handPositionCastOrigin -= obstacleHit.normal * 0.1f;
            Debug.DrawRay(handPositionCastOrigin, Vector3.down, Color.magenta);
            if (Physics.Raycast(handPositionCastOrigin, Vector3.down, out var handCast, sensorConfig.vaultHeight * 2,
                    obstacleLayer))
            {
                _obstacleInteractionEndPos.Value = handCast.point;
            }

            Debug.Log("Can step up");
            _currentObstacleInteraction.Value = ObstacleInteraction.StepUp;
            return;
        }

        private void SetUpVaultInteraction(RaycastHit obstacleCast, Vector3 obstacleOrigin, RaycastHit obstacleHit)
        {
            Debug.Log("Can vault");
            _obstacleInteractionEndPos.Value = obstacleCast.point;
            _currentObstacleInteraction.Value = ObstacleInteraction.Vault;

            Vector3 handPositionCastOrigin = obstacleOrigin + Vector3.up * sensorConfig.vaultHeight;
            handPositionCastOrigin -= obstacleHit.normal * 0.1f;
            Debug.DrawRay(handPositionCastOrigin, Vector3.down, Color.magenta);
            if (Physics.Raycast(handPositionCastOrigin, Vector3.down, out var handCast, sensorConfig.vaultHeight * 2,
                    obstacleLayer))
            {
                //TODO: ADD MATCH TARGET FOR ANIM
            }

            return;
        }

        private void CheckForInteractable()
        {
            RaycastHit interactableHit = new RaycastHit();
            Debug.DrawRay(detectionCastOrigin.position, detectionCastOrigin.forward);
            if (Physics.SphereCast(detectionCastOrigin.position, detectionRadius, detectionCastOrigin.forward,
                    out interactableHit, detectionRange))
            {
                _currentInteractable = interactableHit.transform.gameObject.GetComponent<IInteractable>();
                if (_currentInteractable != null)
                {
                    InteractableInView.Value = true;
                    return;
                }
            }

            _currentInteractable = null;
            InteractableInView.Value = false;


        }
    }
    

}