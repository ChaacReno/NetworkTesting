using System.IO;
using AdvancedSceneManager.Callbacks;
using AdvancedSceneManager.Core;
using AdvancedSceneManager.Editor;
using AdvancedSceneManager.Editor.Utility;
using AdvancedSceneManager.Models;
using AdvancedSceneManager.Utility;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace AdvancedSceneManager.Setup.Editor
{

    /// <summary>Entry point of ASM.</summary>
    internal class ASM
    {

        /// <summary>Gets the current version of ASM.</summary>
        public static string GetVersion() => GetResource("AdvancedSceneManager/Editor/Setup/version");

        /// <summary>Gets patch notes for asset store release.</summary>
        public static string GetFullPatchNotes() => GetResource("AdvancedSceneManager/Editor/Setup/patchNotes-full");

        /// <summary>Gets patch notes for patch.</summary>
        public static string GetPartialPatchNotes() => GetResource("AdvancedSceneManager/Editor/Setup/patchNotes-partial");

        static string GetResource(string path)
        {
            var resource = Resources.Load<TextAsset>(path);
            return resource ? resource.text : string.Empty;
        }

        [InitializeOnLoadMethod]
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void OnLoad()
        {

            SceneManager.settings.local.Reload();
            AssetRef.Initialize();
            SetProfile();

            Setup.ASM.InitializeSubsubsystem();
            InitalizeShared();
            Setup.ASM.OnLoad();

            if (Application.isBatchMode)
                InitializeBatchMode();
            else
                InitializeEditor();

        }

        static void InitalizeShared()
        {

            SceneManager.settings.project.Initialize();
            ScriptingDefineUtility.Set("ADVANCED_SCENE_MANAGER");

            //Editor coroutine package is a soft dependency for CoroutineUtility (a direct dependency of ASM, which is embedded)
            //Needed for editor functionality
            if (!File.ReadAllText("Packages/manifest.json").Contains("com.unity.editorcoroutines"))
                _ = Client.Add("com.unity.editorcoroutines@1.0");

        }

        static void SetProfile()
        {

            Profile profile;

            if (Application.isBatchMode) profile = Profile.buildProfile;
            else if (Profile.forceProfile) profile = Profile.forceProfile;
            else if (Profile.defaultProfile) profile = Profile.defaultProfile;
            else
                profile = SceneManager.assets.profiles.Find(SceneManager.settings.local.activeProfile);

            Profile.SetProfile(profile, updateBuildSettings: false);

        }

        static void InitializeBatchMode()
        {
            BuildUtility.DoPreBuild();
        }

        static void InitializeEditor()
        {

            DefaultSceneUtility.Initialize();

            EditorManager.Initialize();

            DrawCollectionOnScenesInHierarchy.Initialize();
            PatchUtility.Initialize();
            PluginUtility.Initialize();
            CallbackUtility.Initialize();
            HierarchyGUIUtility.Initialize();
            PersistentUtility.Initialize();

            DynamicCollectionUtility.Initialize();
            BuildUtility.Initialize();

            AssetRefreshUtility.Initialize();
            SceneManagerWindow.Initialize();
            FixWeirdUnityBug();

        }

        [MenuItem("Tools/Advanced Scene Manager/FixWeirdUnityBug")]
        static void FixWeirdUnityBug()
        {

            //Unity keeps flagging
            //Assets/AdvancedSceneManager/System/Resources/AdvancedSceneManager/asm-icon.png
            //with HideFlags.DontSave for some reason, 
            //ASM does not have a singlular reference to even HideFlags, so this must be on unitys side?
            //This causes build errors, so we can't just ignore it either

            //Reimport of does not work
            //Rename does not work
            //Moving icon to desktop, making sure unity detects and removes meta file, then dragging it back and reassigning icons to script files, does not work

            var icon = Resources.Load<Object>("AdvancedSceneManager/asm-icon");
            if (icon && icon.hideFlags != HideFlags.None)
            {
                icon.hideFlags = HideFlags.None;
                EditorUtility.SetDirty(icon);
#if UNITY_2019
                AssetDatabase.SaveAssets();
#else
                AssetDatabase.SaveAssetIfDirty(icon);
#endif
            }

        }

    }

}
