#if ASM_PLUGIN_ADDRESSABLES

using AdvancedSceneManager.Models;
using AdvancedSceneManager.Utility;

namespace AdvancedSceneManager.Plugin.Addressables
{

    /// <summary>Provides extension methods for <see cref="Scene"/>.</summary>
    public static class SceneExtensions
    {

        const string key = "addressables";

        /// <summary>Gets whatever this scene should be opened by addressables.</summary>
        public static bool IsAddressable(this Scene scene) =>
            scene.path == SceneDataUtility.Get<string>(scene, key);

        /// <summary>Gets addressable address for this scene.</summary>
        internal static string GetAddress(this Scene scene) =>
            SceneDataUtility.Get<string>(scene, key);

#if UNITY_EDITOR

        /// <summary>Sets whatever this scene should be opened by addressables.</summary>
        /// <remarks>Only available in editor.</remarks>
        public static void IsAddressable(this Scene scene, bool isEnabled)
        {

            if (scene.IsAddressable() == isEnabled)
                return;

            if (isEnabled)
                SceneDataUtility.Set(scene, key, scene.path);
            else
                SceneDataUtility.Unset(scene, key);

        }
#endif

    }

}
#endif
