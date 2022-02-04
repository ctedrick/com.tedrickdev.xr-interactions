using System;
using UnityEngine;

namespace TedrickDev.XRPoser
{
    [Serializable]
    public struct PoseTransform
    {
        public Vector3 LocalPosition;
        public Quaternion LocalRotation;
    }
}