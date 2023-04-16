using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Features.Effectors.Animator
{
    public class AnimationResolver : MonoBehaviour
    {
        struct IKData
        {
            public float positionWeight;
            public Vector3 position;
            public float rotationWeight;
            public Quaternion rotation;
        }
    
        [SerializeField] private UnityEngine.Animator animator;

        private Subject<Unit> _animationFinished;

        private bool _useInverseKinematics;


        private IKData _leftHandIKData;
        private IKData _leftFootIKData;
        private IKData _rightHandIKData;
        private IKData _rightFootIKData;
        private Dictionary<AvatarIKGoal, IKData> _ikData;


        public UnityEngine.Animator Animator => animator;
        public AnimatorClipInfo[] ClipInfos => animator.GetCurrentAnimatorClipInfo(0);
    
        public bool UseInverseKinematics => _useInverseKinematics;

        public void FreezeAnimation(float duration)
        {
            animator.speed = 0;
        }

        private void Awake()
        {
            _useInverseKinematics = true;
            _ikData = new Dictionary<AvatarIKGoal, IKData>()
            {
                { AvatarIKGoal.LeftHand, _leftHandIKData },
                { AvatarIKGoal.LeftFoot, _leftFootIKData },
                { AvatarIKGoal.RightHand, _rightHandIKData },
                { AvatarIKGoal.RightFoot, _rightFootIKData }
            };
            InitIKData();
            _animationFinished = new Subject<Unit>();
        }

        private void OnAnimationFinished()
        {
            _animationFinished.OnNext(Unit.Default);
        }

        public void QueueAnimationMatchTarget()
        {
            
        }

        public void EnableIK()
        {
            _useInverseKinematics = true;
        }

        public void DisableIK()
        {
            _useInverseKinematics = false;
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if(!_useInverseKinematics) return;

            foreach (var ikGoal in _ikData.Keys)
            {
                animator.SetIKPositionWeight(ikGoal, _ikData[ikGoal].positionWeight);
                animator.SetIKPosition(ikGoal, _ikData[ikGoal].position);
                animator.SetIKRotationWeight(ikGoal, _ikData[ikGoal].rotationWeight);
                animator.SetIKRotation(ikGoal, _ikData[ikGoal].rotation);
            }
        
        
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0 );
        }

        public void EnableIKPosition(AvatarIKGoal ikGoal, float weight, Vector3 position)
        {
            var ikData = _ikData[ikGoal];
            ikData.positionWeight = weight;
            ikData.position = position;
            _ikData[ikGoal] = ikData;
        }
    
        public void EnableIKRotation(AvatarIKGoal ikGoal, float weight, Quaternion rotation)
        {
            var ikData = _ikData[ikGoal];
            ikData.rotationWeight = weight;
            ikData.rotation = rotation;
            _ikData[ikGoal] = ikData;
        }

        public void DisableIK(AvatarIKGoal ikGoal)
        {
            var ikData = _ikData[ikGoal];
            ikData.positionWeight = 0;
            ikData.rotationWeight = 0;
            _ikData[ikGoal] = ikData;
        }

        private void InitIKData()
        {
            _leftHandIKData.positionWeight = 0;
            _leftFootIKData.positionWeight = 0;
            _rightHandIKData.positionWeight = 0;
            _rightFootIKData.positionWeight = 0;
            _leftHandIKData.rotationWeight = 0;
            _leftFootIKData.rotationWeight = 0;
            _rightHandIKData.rotationWeight = 0;
            _rightFootIKData.rotationWeight = 0;
        }

        public Subject<Unit> AnimationFinished => _animationFinished;
    }
}
