// **************************************************************** //
//
//   Copyright (c) RimuruDev. All rights reserved.
//   Contact me: 
//          - Gmail:    rimuru.dev@gmail.com
//          - LinkedIn: https://www.linkedin.com/in/rimuru/
//          - GitHub:   https://github.com/RimuruDev
//
// **************************************************************** //

using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.DeviceSimulation;
#endif

namespace Game
{
    [Serializable]
    public enum DeviceType : byte
    {
        WebPC = 0,
        WebMobile = 1,
    }

    [SelectionBase]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-100)]
    [HelpURL("https://github.com/RimuruDev/Unity-WEBGL-DeviceTypeDetector")]
    public sealed class DeviceTypeDetector : MonoBehaviour
    {
        [field: SerializeField] public DeviceType DeviceType { get; private set; }

        [SerializeField] private bool isTest = true;
#if UNITY_2020_1_OR_NEWER
        [SerializeField] private bool enableDeviceSimulator = true;
#endif
        private void Awake()
        {
#if UNITY_EDITOR
            if (isTest)
                return;
#endif
            
            if (IsMobile() && enableDeviceSimulator)
            {
                Debug.Log("WEBGL -> Mobile");
                DeviceType = DeviceType.WebMobile;
            }
            else
            {
                Debug.Log("WEBGL -> PC");
                DeviceType = DeviceType.WebPC;
            }
        }

#if UNITY_EDITOR
        public static bool IsMobile()
        {
#if UNITY_2020_1_OR_NEWER
            if (DeviceSimulatorExists() && IsDeviceSimulationActive())
                return true;
#endif
            return false;
        }

        private static bool DeviceSimulatorExists()
        {
            var simulatorType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.DeviceSimulation.DeviceSimulator");
            return simulatorType != null;
        }

        private static bool IsDeviceSimulationActive()
        {
            var simulatorType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.DeviceSimulation.DeviceSimulator");
            if (simulatorType != null)
            {
                var simulatorInstance = simulatorType.GetProperty("instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)?.GetValue(null);
                var isDeviceActive = simulatorType.GetProperty("isDeviceActive", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(simulatorInstance);
                return (bool)isDeviceActive;
            }
            return false;
        }
#else
        [System.Runtime.InteropServices.DllImport("__Internal")]
        public static extern bool IsMobile();
#endif
    }
}