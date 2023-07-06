using System;
using System.Collections;
using AdvancedSceneManager.Core;
using AdvancedSceneManager.Utility;
using UnityEngine;

namespace AdvancedSceneManager.Callbacks
{

    /// <summary>
    /// <para>A class that contains callbacks for loading screens.</para>
    /// <para>One instance must exist in a scene that specified as a loading screen.</para>
    /// </summary>
    public abstract class LoadingScreen : MonoBehaviour
    {

#pragma warning disable CS0414
        [SerializeField, HideInInspector] private bool isLoadingScreen = true;
#pragma warning restore CS0414

        /// <summary>Occurs when loading screen is destroyed.</summary>
        public Action<LoadingScreen> onDestroy;
        protected virtual void OnDestroy() =>
            onDestroy?.Invoke(this);

        /// <summary>
        /// <para>The canvas that this loading screen uses.</para>
        /// <para>This will automatically register canvas with <see cref="CanvasSortOrderUtility"/>, to automatically manage canvas sort order.</para>
        /// <para>You probably want to set this through the inspector.</para>
        /// </summary>
        [Tooltip("The canvas to automatically manage sort order for, optional.")]
        public Canvas canvas;

        /// <summary>The current scene operation that this loading screen is associated with. May be null for the first few frames, before loading has actually begun.</summary>
        public SceneOperation operation { get; internal set; }

        /// <summary>
        /// <para>Called when the associated <see cref="SceneOperation"/> is about to start.</para>
        /// <para>Use this callback to show your loading screen, the scene manager will wait until its done.</para>
        /// </summary>
        public abstract IEnumerator OnOpen(SceneOperation operation);

        /// <summary>
        /// <para>Called when the associated <see cref="SceneOperation"/> has ended.</para>
        /// <para>Use this callback to hide your loading screen.</para>
        /// </summary>
        public abstract IEnumerator OnClose(SceneOperation operation);

        /// <summary>Called when the associated <see cref="SceneOperation"/> is moving to a different phase.</summary>
        public virtual void OnScenePhaseChanged(SceneOperation operation, Phase previousPhase, Phase nextPhase)
        { }

        /// <summary>
        /// <para>Called when the associated <see cref="SceneOperation"/> is cancelled.</para>
        /// <para>Note that <see cref="OnClose(SceneOperation)"/> is not called.</para>
        /// </summary>
        public virtual void OnCancel(SceneOperation operation)
        { }

    }

}