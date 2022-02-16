using UnityEngine;

namespace TedrickDev.HandPoser.Poser.HandPoseDemo
{
    public class PoseOnTriggerEnter : MonoBehaviour
    {
        [SerializeField] private PoseData pose;

        private void Awake()
        {
            if (!pose) {
                Debug.LogError($"{nameof(pose)} is missing. {gameObject}");
                enabled = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var hand = other.GetComponentInChildren<PoserHand>();
            if (hand) PoserManager.Instance.ApplyPose(hand, pose);
        }

        private void OnTriggerExit(Collider other)
        {
            var hand = other.GetComponentInChildren<PoserHand>();
            if (hand) PoserManager.Instance.ApplyDefaultPose(hand);
        }
    }
}