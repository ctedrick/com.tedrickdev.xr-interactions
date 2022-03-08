using TedrickDev.InteractionsToolkit.Utility;
using UnityEngine;

namespace TedrickDev.InteractionsToolkit.Poser
{
    public class PoserManager : Singleton<PoserManager>
    {
        [Header("Pose Asset")]
        public PoseData DefaultPose;

        [Header("Scene Hands")]
        public PoserHand LeftPoserHand;
        public PoserHand RightPoserHand;
        
        [Header("Asset Hands")]
        public PoserHand LeftPrefab;
        public PoserHand RightPrefab;

        protected override void Awake()
        {
            base.Awake();
            
            if (!LeftPoserHand) {
                Debug.LogWarning($"{nameof(LeftPoserHand)} is null, check reference on {gameObject}");
                enabled = false;
            }
            
            if (!RightPoserHand) {
                Debug.LogWarning($"{nameof(RightPoserHand)} is null, check reference on {gameObject}");
                enabled = false;
            }
        }
        
        public void ApplyPose(Handedness hand, PoseData pose)
        {
            if (hand == Handedness.Left) LeftPoserHand.SetPose(pose.LeftJoints);
            else RightPoserHand.SetPose(pose.RightJoints);
        }
        
        public void ApplyPose(PoserHand hand, PoseData pose)
        {
            if (hand.Type == Handedness.Left) LeftPoserHand.SetPose(pose.LeftJoints);
            else RightPoserHand.SetPose(pose.RightJoints);
        }
        
        public void ApplyDefaultPose(Handedness hand)
        {
            if (hand == Handedness.Left) LeftPoserHand.SetPose(DefaultPose.LeftJoints);
            else RightPoserHand.SetPose(DefaultPose.RightJoints);
        }
        
        public void ApplyDefaultPose(PoserHand hand)
        {
            if (hand.Type == Handedness.Left) LeftPoserHand.SetPose(DefaultPose.LeftJoints);
            else RightPoserHand.SetPose(DefaultPose.RightJoints);
        }
    }
}