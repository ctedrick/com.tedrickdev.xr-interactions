// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// https://github.com/microsoft/MixedRealityToolkit-Unity/blob/d343a0897f43ba0094d8e5ca032dc3a243cf2712/Assets/HoloToolkit/Utilities/Scripts/Singleton.cs

using UnityEngine;

namespace TedrickDev.HandPoser.Utility
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T Instance{ get; private set; }

        protected virtual void Awake()
        {
            if (Instance != null) {
                Debug.LogErrorFormat("Trying to instantiate a second instance of singleton class {0}", GetType().Name);
            }
            else Instance = (T) this;
        }

        protected virtual void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}