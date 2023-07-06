using AdvancedSceneManager.Utility;
using UnityEngine;
using AdvancedSceneManager.Core.Actions;
using AdvancedSceneManager.Core;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AdvancedSceneManager.Setup
{

    /// <summary>Entry point of ASM.</summary>
    internal static class ASM
    {

#if UNITY_EDITOR
        /// <summary>Gets if asm is set up, and intro process has been completed.</summary>
        /// <remarks>Only available in editor.</remarks>
        public static bool isSetup => AssetDatabase.FindAssets("t:Profile").Length > 0;
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        internal static void InitializeSubsubsystem()
        {

            AssetRef.Initialize();

            SceneManager.Initialize();
            UtilitySceneManager.Initialize();

            InGameToolbarUtility.Initialize();
            PauseScreenUtility.Initialize();

        }

        //Called by SceneManager.Editor assembly when in editor
#if !UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
#endif
        internal static void OnLoad()
        {
            if (Application.isPlaying)
            {
                QuitAction.Reset();
                Runtime.Initialize();
            }
        }

    }

}
