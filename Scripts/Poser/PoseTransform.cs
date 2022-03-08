using System;
using UnityEngine;

namespace TedrickDev.InteractionsToolkit.Poser
{
    [Serializable]
    public struct PoseTransform
    {
        public Vector3 LocalPosition;
        public Quaternion LocalRotation;
    }
}