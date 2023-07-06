#pragma warning disable IDE0051 // Remove unused private members

using System;
using UnityEngine;
using AdvancedSceneManager.Models;

using scene = UnityEngine.SceneManagement.Scene;
using System.Linq;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AdvancedSceneManager.Utility
{

    /// <summary>A utility for storing scene related data. Data can only be saved to disk in editor.</summary>
    public static class SceneDataUtility
    {

        static ASMSettings.SceneData Data
        {
            get
            {
                if (SceneManager.settings.project.sceneData == null)
                    SceneManager.settings.project.sceneData = new ASMSettings.SceneData();
                return SceneManager.settings.project.sceneData;
            }
        }

        #region Update when scene is moved

#if UNITY_EDITOR

        class PostProcessor : AssetPostprocessor
        {

            static void OnPostprocessAllAssets(string[] _1, string[] _2, string[] movedAssets, string[] movedFromPath)
            {

                foreach (var (source, to) in movedAssets.Select((_, i) => (source: movedFromPath[i], to: movedAssets[i])).ToArray())
                {

                    var key = AssetDatabase.AssetPathToGUID(source).ToString();

                    if (Data.ContainsKey(key))
                    {

                        var data = Data[key];
                        _ = Data.Remove(key);
                        _ = Data.Set(AssetDatabase.AssetPathToGUID(to).ToString(), data);
                        SceneManager.settings.project.MarkAsDirty();

                    }

                }

                if (EditorUtility.IsDirty(SceneManager.settings.project))
                    SceneManager.settings.project.Save();

            }

        }

#endif

        #endregion
        #region Get

        /// <summary>Enumerates data using the specified key on all scenes that uses it.</summary>
        public static IEnumerable<(Scene scene, T data)> Enumerate<T>(string key)
        {
            foreach (var scene in Data)
                if (scene.Value.TryGetValue(key, out var json) && SceneManager.assets.scenes.TryFind(scene.Key, out var targetScene) && TryDeserialize<T>(json, out var t))
                    yield return (targetScene, t);
        }

        /// <summary>Gets the value with the specified key, for the specified scene.</summary>
        public static T Get<T>(Scene scene, string key, T defaultValue = default) =>
            Get(scene.path, key, defaultValue);

        /// <summary>Gets the value with the specified key, for the specified scene.</summary>
        public static T Get<T>(string scene, string key, T defaultValue = default) =>
            Get<T>(scene, key, out var value)
            ? value
            : defaultValue;

        /// <summary>Gets the value with the specified key, for the specified scene. This is the direct version, all values are stores as string, which means <see cref="Get{T}(string, string, T)"/> must convert value beforehand, this method doesn't.</summary>
        public static string GetDirect(Scene scene, string key)
        {
            if (Data.ContainsKey(scene.path))
                return Data[scene.path].GetValue(key);
            else
                return null;
        }

        static bool Get<T>(string path, string key, out T value)
        {

            value = default;

            if (!Data.ContainsKey(path))
                return false;
            if (!Data[path].ContainsKey(key))
                return false;

            var json = Data[path][key];
            if (string.IsNullOrWhiteSpace(json))
                return false;

            return
                Type.GetTypeCode(typeof(T)) != TypeCode.Object
                ? TryConvert(json, out value)
                : TryDeserialize(json, out value);

        }

        #endregion
        #region Set

        /// <summary>Sets the value with the specified key, for the specified scene.</summary>
        /// <remarks>Changes will only be persisted in editor.</remarks>
        public static void Set<T>(Scene scene, string key, T value) =>
            Set(scene.path, key, value);

        /// <summary>Sets the value with the specified key, for the specified scene.</summary>
        /// <remarks>Changes will only be persisted in editor.</remarks>
        public static void Set<T>(string scene, string key, T value)
        {
            if (Convert.GetTypeCode(value) != TypeCode.Object)
                SetDirect(scene, key, Convert.ToString(value));
            else
                SetDirect(scene, key, JsonUtility.ToJson(value));
        }

        /// <summary>Sets the value with the specified key, for the specified scene. This is the direct version, all values are stores as string, which means <see cref="Get{T}(string, string, T)"/> must convert value beforehand, this method doesn't.</summary>
        /// <remarks>Changes will only be persisted in editor.</remarks>
        public static void SetDirect(string scene, string key, string value)
        {

            if (string.IsNullOrWhiteSpace(scene))
                return;

            if (!Data.ContainsKey(scene))
                Data.Add(scene, new ASMSettings.CustomData());
            _ = Data[scene].Set(key, value);

            Save();

        }

        /// <summary>Unsets the value with the specified key, for the specified scene.</summary>
        /// <remarks>Changes will only be persisted in editor.</remarks>
        public static void Unset(Scene scene, string key)
        {

            if (!scene)
                return;

            if (Data.ContainsKey(scene.path) && Data[scene.path].Remove(key))
                Save();

        }

        static void Save()
        {
#if UNITY_EDITOR
            SceneManager.settings.project.Save();
#endif
        }

        #endregion
        #region Update

#if UNITY_EDITOR

        [InitializeOnLoadMethod]
        static void Update()
        {

            if (SceneManager.settings.project.sceneData == null)
                SceneManager.settings.project.sceneData = new ASMSettings.SceneData();

            var resources = Resources.LoadAll<TextAsset>("AdvancedSceneManager/SceneData");
            foreach (var resource in resources)
            {
                var data = JsonUtility.FromJson<SceneData>(resource.text);
                if (data != null)
                    _ = SceneManager.settings.project.sceneData.Set(data.path, data.data.ToCustomData());
            }

            var paths = resources.Select(AssetDatabase.GetAssetPath).Where(p => AssetDatabase.LoadAssetAtPath<TextAsset>(p)).ToArray();

            if (paths.Any())
                EditorApplication.delayCall += () =>
                {

#pragma warning disable UNT0031 // Asset operations in LoadAttribute method

                    var failedPaths = new List<string>();

#if UNITY_2019
                    foreach (var path in paths)
                        if (!AssetDatabase.DeleteAsset(path))
                            failedPaths.Add(path);
#else
                    _ = AssetDatabase.DeleteAssets(paths, failedPaths);
#endif
                    if (failedPaths.Any())
                        Debug.LogError("Updating SceneData: Could not delete the following files:\n\n" + string.Join("\n", failedPaths));

#pragma warning restore UNT0031 // Asset operations in LoadAttribute method

                    SceneManager.settings.project.Save();

                };

        }

        [Serializable]
        class SceneData
        {

            [SerializeField] public string path;
            [SerializeField] public SerializableDictionary data = new SerializableDictionary();

            [Serializable]
            public class SerializableDictionary : SerializableDictionary<string, string>
            {

                public ASMSettings.CustomData ToCustomData()
                {
                    
                    var data = new ASMSettings.CustomData();
                    foreach (var key in Keys)
                        data.Add(key, this[key]);

                    return data;

                }

            }

        }

#endif

        #endregion
        #region Json

        static bool TryConvert<T>(object obj, out T value)
        {
            try
            {
                value = (T)Convert.ChangeType(obj, typeof(T));
                return true;
            }
            catch (Exception)
            { }
            value = default;
            return false;
        }

        static bool TryDeserialize<T>(string json, out T value)
        {
            try
            {
                value = JsonUtility.FromJson<T>(json);
                return true;
            }
            catch (Exception)
            { }
            value = default;
            return false;
        }

        #endregion

    }

}
