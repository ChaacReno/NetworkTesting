using System;
using System.Linq;
using System.Net.Http;
using AdvancedSceneManager.Callbacks;
using AdvancedSceneManager.Setup.Editor;
using UnityEditor;
using UnityEngine;

namespace AdvancedSceneManager.Editor.Utility
{

    internal static class PatchUtility
    {

        public static bool hasUpdate => Version.TryParse(version, out var patch) && Version.TryParse(ASM.GetVersion(), out var current) && patch > current;
        public static string version { get; private set; }
        public static string patchNotes { get; private set; }

        const string url = "https://gist.githubusercontent.com/Zumwani/195afd3053cf1cb951013e30908903c0/raw/";

        internal static void Initialize()
        {

            if (Application.isPlaying)
                return;

            CheckForUpdate();

            EditorApplication.playModeStateChanged -= EditorApplication_playModeStateChanged;
            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;

        }

        static void EditorApplication_playModeStateChanged(PlayModeStateChange e)
        {
            if (e == PlayModeStateChange.EnteredEditMode)
                CheckForUpdate();
        }

        static async void CheckForUpdate()
        {

            if (!CanCheck())
                return;

#if ASM_DEV
            Debug.Log("Debug: Checking for update...");
#endif

            try
            {

                using (var client = new HttpClient())
                {

                    client.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue() { NoCache = true };

                    var result = await client.GetStringAsync(url);

                    var lines = result.Split('\n');
                    var version = Version.Parse(lines.First());
                    var patchNotes = string.Join("\n", lines.Skip(1));

                    PatchUtility.version = version.ToString();
                    PatchUtility.patchNotes = patchNotes;

                }

            }
            catch (Exception)
            {
                //Not very critical, if it fails, this would probably be due to rate limit or no internet access
            }

#if ASM_DEV
            Debug.Log("Debug: " + (hasUpdate ? $"Update found ({version})..." : "No patch available..."));
#endif

        }

        static SerializableDateTime lastPatchCheck
        {
            get => SceneManager.settings.local.lastPatchCheck;
            set
            {
                SceneManager.settings.local.lastPatchCheck = value;
                SceneManager.settings.local.Save();
            }
        }

        static bool CanCheck()
        {

            if (lastPatchCheck == null || lastPatchCheck.value == DateTime.MinValue)
            {
                lastPatchCheck = DateTime.Now;
                return true;
            }

            if ((DateTime.Now - lastPatchCheck).TotalHours >= 1)
            {
                lastPatchCheck = DateTime.Now;
                return true;
            }

            return false;

        }

    }

}
