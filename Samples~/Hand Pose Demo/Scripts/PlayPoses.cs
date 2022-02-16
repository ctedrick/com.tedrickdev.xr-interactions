using System.Collections;
using System.Collections.Generic;
using TedrickDev.HandPoser.Poser;
using UnityEngine;

namespace TedrickDev.HandPoser.HandPoseDemo
{
    public class PlayPoses : MonoBehaviour
    {
        [SerializeField] private List<PoseData> poses;
        [SerializeField] private float interval = 1f;

        private void Start() => StartCoroutine(PlayRoutine());

        private IEnumerator PlayRoutine()
        {
            while (true) {
                for (var i = 0; i < poses.Count; i++) {
                    PoserManager.Instance.ApplyPose(PoserManager.Instance.LeftPoserHand, poses[i]);
                    PoserManager.Instance.ApplyPose(PoserManager.Instance.RightPoserHand, poses[i]);

                    if (i == poses.Count) i = 0;
                    yield return new WaitForSeconds(interval);
                }
            }
        }
    }
}