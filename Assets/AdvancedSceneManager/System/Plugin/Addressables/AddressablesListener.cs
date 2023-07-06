#if ASM_PLUGIN_ADDRESSABLES && UNITY_EDITOR

using AdvancedSceneManager.Editor.Utility;
using AdvancedSceneManager.Models;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace AdvancedSceneManager.Plugin.Addressables.Editor
{

    [InitializeOnLoad]
    static class AddressablesListener
    {

        public static AddressableAssetSettings settings { get; }

        static AddressablesListener()
        {

            settings = AddressableAssetSettingsDefaultObject.GetSettings(true);

            Refresh();
            ListenToAddressablesChange();

        }

        static void ListenToAddressablesChange()
        {

            if (settings == null)
                return;

            settings.OnModification -= OnModification;
            settings.OnModification += OnModification;

            void OnModification(AddressableAssetSettings s, AddressableAssetSettings.ModificationEvent e, object obj)
            {

                if (obj is Scene scene && scene)
                {

                    if (e == AddressableAssetSettings.ModificationEvent.EntryAdded)
                        scene.IsAddressable(true);
                    else if (e == AddressableAssetSettings.ModificationEvent.EntryRemoved)
                        scene.IsAddressable(false);

                    BuildUtility.UpdateSceneList();

                }

            }

        }

        static void Refresh()
        {

            if (settings == null)
                return;

            foreach (var scene in SceneManager.assets.allScenes)
            {
                var entry = settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(scene.path));
                scene.IsAddressable(entry != null);
            }

            BuildUtility.UpdateSceneList();

        }

    }

}
#endif
