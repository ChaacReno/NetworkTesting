#pragma warning disable CS0414

using Object = UnityEngine.Object;
using Scene = AdvancedSceneManager.Models.Scene;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using AdvancedSceneManager.Models;
using UnityEngine.Serialization;
using System.Collections.Generic;

#if UNITY_EDITOR
using AdvancedSceneManager.Editor.Utility;
using UnityEditor;
#endif

namespace AdvancedSceneManager.Core
{

    class AssetRef : ScriptableObject
    {

        [SerializeField] private string m_path;
        [SerializeField, FormerlySerializedAs("profiles")] private Profile[] m_profiles = Array.Empty<Profile>();
        [SerializeField, FormerlySerializedAs("scenes")] private Scene[] m_scenes = Array.Empty<Scene>();
        [SerializeField, FormerlySerializedAs("collections")] private SceneCollection[] m_collections = Array.Empty<SceneCollection>();

        [SerializeField] private ASMSettings m_settings;
        [SerializeField] private CollectionManager m_collectionManager;
        [SerializeField] private StandaloneManager m_standaloneManager;
        [SerializeField] private Utility.ASM m_sceneHelper;

        public IEnumerable<Profile> profiles => m_profiles;
        public IEnumerable<SceneCollection> collections => m_collections;
        public IEnumerable<Scene> scenes => m_scenes;

        public ASMSettings settings => GetSingleton(ref m_settings);
        public CollectionManager collectionManager => GetSingleton(ref m_collectionManager);
        public StandaloneManager standaloneManager => GetSingleton(ref m_standaloneManager);
        public Utility.ASM sceneHelper => GetSingleton(ref m_sceneHelper);

        static AssetRef m_instance;
        public static AssetRef instance => GetSingleton(ref m_instance);

        public IEnumerable<Object> allAssets =>
            profiles.OfType<Object>().Concat(scenes).Concat(collections).Concat(new Object[] { settings, collectionManager, standaloneManager, sceneHelper, instance }).Where(o => o);

        void OnValidate() =>
            m_path = path;

        #region Add / Remove

#if UNITY_EDITOR

        public void Add<T>(params T[] obj) where T : Object, IASMObject
        {
            if (obj is Profile[] profiles)
                Add(ref m_profiles, profiles);
            else if (obj is SceneCollection[] collections)
                Add(ref m_collections, collections);
            else if (obj is Scene[] scenes)
                Add(ref m_scenes, scenes);
        }

        public void Remove<T>(params T[] obj) where T : Object, IASMObject
        {
            if (obj is Profile[] profiles)
                Remove(ref m_profiles, profiles);
            else if (obj is SceneCollection[] collections)
                Remove(ref m_collections, collections);
            else if (obj is Scene[] scenes)
                Remove(ref m_scenes, scenes);
        }

        void Add<T>(ref T[] list, params T[] item) where T : Object =>
            Do<T>(ref list, (l) => l.Concat(item));

        void Remove<T>(ref T[] list, params T[] item) where T : Object =>
            Do(ref list, (l) => l.Except(item));

        void Do<T>(ref T[] list, Func<IEnumerable<T>, IEnumerable<T>> action) where T : Object
        {

            var savedList = list?.ToArray();

            IEnumerable<T> newList = (list ?? Array.Empty<T>());
            newList = action.Invoke(newList);
            newList = newList.Distinct().Where(o => o).OrderBy(o => AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(o)));

            if (!newList.SequenceEqual(savedList))
            {
                list = newList.ToArray();
                EditorUtility.SetDirty(this);
            }

        }

        public void Cleanup()
        {
            Do(ref m_profiles, (l) => l);
            Do(ref m_collections, (l) => l);
            Do(ref m_scenes, (l) => l);
        }

        public void Clear()
        {
            m_profiles = null;
            m_collections = null;
            m_scenes = null;
            Cleanup();
        }

#endif

        #endregion
        #region Singletons

        static string defaultPath = "Assets/Settings/AdvancedSceneManager/Resources/AdvancedSceneManager";
        public static string path
        {
            get
            {

#if UNITY_EDITOR
                if (!instance || string.IsNullOrEmpty(AssetDatabase.GetAssetPath(instance)))
                    return defaultPath;
#endif

                if (!instance)
                    return defaultPath;
                else
                {

#if UNITY_EDITOR
                    var path = Directory.GetParent(AssetDatabase.GetAssetPath(instance)).FullName;
                    path = "Assets" + path.Remove(0, Application.dataPath.Length).Replace('\\', '/');
                    return path;
#else
                    return instance.m_path;
#endif

                }

            }
        }

#if UNITY_EDITOR

        /// <summary>Move all ASM assets to a new folder.</summary>
        /// <remarks>Only available in editor.</remarks>
        public static void Move(string path)
        {

            if (AssetDatabase.IsValidFolder(path))
                if (!EditorUtility.DisplayDialog("Moving assets...", "The specified folder is not empty, do you wish to clear it before moving?", ok: "Cancel", cancel: "Yes"))
                    _ = AssetDatabase.DeleteAsset(path);
                else
                    return;

            var originalPath = AssetRef.path;

            var key = new object();
            AssetDatabaseUtility.DisallowAutoRefresh(key);

            EditorFolderUtility.EnsureFolderExists(path);

            Move(originalPath + "/Scenes", path + "/Scenes");
            Move(originalPath + "/SceneCollections", path + "/SceneCollections");
            Move(originalPath + "/Collections", path + "/Collections");
            Move(originalPath + "/Profiles", path + "/Profiles");
            Move(originalPath + "/SceneData", path + "/SceneData");
            Move(originalPath + "/AdvancedSceneManager.unity", path + "/AdvancedSceneManager.unity");

            MoveSingleton(instance.settings);
            MoveSingleton(instance.collectionManager);
            MoveSingleton(instance.standaloneManager);
            MoveSingleton(instance.sceneHelper);
            MoveSingleton(instance);

            AssetDatabaseUtility.AllowAutoRefresh(key);

            void MoveSingleton<T>(T instance) where T : ScriptableObject =>
                Move(AssetDatabase.GetAssetPath(instance), path + "/" + GetName<T>() + ".asset");

            void Move(string from, string to)
            {
                if (AssetDatabase.IsValidFolder(from) || AssetDatabase.LoadAssetAtPath<Object>(from))
                {
                    var str = AssetDatabase.MoveAsset(from, to);
                    if (!string.IsNullOrEmpty(str))
                        throw new Exception(str);
                }
            }

        }

#endif

        internal static void Initialize()
        {
            _ = instance.settings;
            _ = instance.collectionManager;
            _ = instance.standaloneManager;
            _ = instance.sceneHelper;
        }

        static string GetName<T>()
        {
            if (typeof(T) == typeof(Utility.ASM))
                return "SceneHelper";
            else
                return typeof(T).Name;
        }

        static T GetSingleton<T>(ref T field) where T : ScriptableObject
        {

            if (field)
                return field;

#if UNITY_EDITOR
            _ = UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget("AdvancedSceneManager/" + GetName<T>());
#endif

            if (Resources.Load<T>("AdvancedSceneManager/" + GetName<T>()) is T t && t)
                return field = t;

#if UNITY_EDITOR

            if (Application.isBatchMode)
                Debug.Log("#UCB ASM Singleton '" + typeof(T).Name + "' not found, creating...");

            field = CreateInstance<T>();
            EditorFolderUtility.EnsureFolderExists(path);
            AssetDatabase.CreateAsset(field, path + "/" + GetName<T>() + ".asset");

            return field;

#else
                throw new InvalidOperationException($"Cannot create singleton of '{typeof(T).Name}' outside of editor!");
#endif

        }

        #endregion

    }

}
