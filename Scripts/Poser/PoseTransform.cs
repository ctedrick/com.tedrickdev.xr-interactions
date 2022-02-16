using System;
using UnityEngine;

namespace TedrickDev.HandPoser.Poser
{
    [Serializable]
    public struct PoseTransform
    {
        public Vector3 LocalPosition;
        public Quaternion LocalRotation;
    }
}