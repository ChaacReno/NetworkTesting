using System;
using System.IO;
using System.Linq;
using AdvancedSceneManager.Editor.Utility;
using AdvancedSceneManager.Models;
using AdvancedSceneManager.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ASM = AdvancedSceneManager.Setup.ASM;
using Object = UnityEngine.Object;

namespace AdvancedSceneManager.Editor.Window
{

    public static class WelcomeTab
    {

        static VisualElement welcomeTab;
        static VisualElement linksTab;
        static VisualElement profileTab;

        static Button backButton;
        static Button continueButton;
        static Button doneButton;

        static VisualElement[] tabs;
        static VisualElement tab;

        static ScrollView scroll;

        public static void OnEnable(VisualElement element)
        {

            element.GetRoot().Q("unity-content-container").style.height = new StyleLength(new Length(100, LengthUnit.Percent));

            scroll = element.Q<ScrollView>("scroll");

            SetupNavButtons(element);
            SetupTabs(element);
            EditorApplication.delayCall += () =>
                SetSize(new Vector2(600, 320));

        }

        public static void OnGUI()
        {
            if (ASM.isSetup)
            {
                SceneManagerWindow.RestoreTab();
                SceneManagerWindow.Reload();
            }
        }

        #region Nav buttons

        static void SetupNavButtons(VisualElement element)
        {

            backButton = element.Q<Button>("button-back");
            continueButton = element.Q<Button>("button-continue");
            doneButton = element.Q<Button>("button-done");

            backButton.clicked += () =>
            {
                var index = Array.IndexOf(tabs, tab);
                SetTab(tabs[index - 1]);
            };

            continueButton.clicked += () =>
            {
                var index = Array.IndexOf(tabs, tab);
                SetTab(tabs[index + 1]);
            };

            doneButton.clicked -= OnDone_Click;
            doneButton.clicked += OnDone_Click;

            void OnDone_Click() =>
                OnDone((profileGUI.profileName, profileGUI.blacklist));

        }

        static void UpdateNavButtons()
        {
            backButton.EnableInClassList("visible", tab.ClassListContains("back-button"));
            continueButton.EnableInClassList("visible", tab.ClassListContains("continue-button"));
            doneButton.EnableInClassList("visible", tab.ClassListContains("done-button"));
        }

        #endregion
        #region Tabs

        static void SetupTabs(VisualElement element)
        {

            welcomeTab = element.Q("tab-welcome");
            linksTab = element.Q("tab-links");
            profileTab = element.Q("tab-profile");

            tabs = new VisualElement[]
            {
                welcomeTab,
                linksTab,
                profileTab
            };

            SetupWelcomeTab(element);
            SetupLinksTab(element);
            SetupProfileTab();

            SetTab(welcomeTab);

        }

        static void SetTab(VisualElement tab)
        {

            foreach (var t in tabs)
                t.RemoveFromClassList("active");
            tab.AddToClassList("active");

            WelcomeTab.tab = tab;
            UpdateNavButtons();

            //Fix for scroll not working
            var element = scroll.Q("unity-content-container");
            var h = element.style.height;
            element.style.height = new StyleLength(h.value.value - 1);

            EditorApplication.delayCall += () => element.style.height = h;

            if (tab.name == "tab-welcome")
                SetSize(new Vector2(600, 320));
            else
                SetSize(new Vector2(600, 520));

        }

        static void SetSize(Vector2 size)
        {
            if (SceneManagerWindow.window.tab == SceneManagerWindow.Tab.Welcome)
                SceneManagerWindow.window.minSize = SceneManagerWindow.window.maxSize = size;
        }

        #endregion
        #region Welcome tab

        static void SetupWelcomeTab(VisualElement element)
        {

            var texture = Resources.Load<Sprite>("AdvancedSceneManager/image");
            if (texture)
                element.Q("asm-banner").style.backgroundImage = texture.texture;

        }

        #endregion
        #region Links tab

        static void SetupLinksTab(VisualElement element)
        {

            linksTab.Query<Button>(className: "link").ForEach(link =>
            {

                var url = GetUrl(link.text);
                link.tooltip = url;

                link.clicked += () =>
                {
                    if (url.StartsWith("Assets/"))
                    {
                        if (AssetDatabase.LoadAssetAtPath<Object>(url) is Object o && o)
                            EditorGUIUtility.PingObject(o);
                    }
                    else
                        Application.OpenURL(url);
                };

            });

            string GetUrl(string text)
            {
                if (text.Contains(": "))
                    return text.Remove(0, text.IndexOf(": ") + 2);
                else
                    return text;
            }

        }

        #endregion
        #region Profile tab

        static CreateProfileGUI profileGUI;

        static void SetupProfileTab()
        {
            if (profileGUI == null)
                profileGUI = new CreateProfileGUI(doneButton, "dev");
            profileTab.Clear();
            profileTab.Add(profileGUI.element);
        }

        #endregion
        #region When done

        static void OnDone((string profileName, BlacklistUtility.BlacklistModule blacklist) profile)
        {

            if (!string.IsNullOrEmpty(profile.profileName))
            {

                //Make sure there are no duplicates
                var name = profile.profileName;
                name = ObjectNames.GetUniqueName(ProfileNames(), name);

                var p = AssetUtility.CreateProfile(name, promptBlacklist: false);
                p.m_blacklist = profile.blacklist;
                p.Save();

                Profile.SetProfile(p);

            }

            AssetRefreshUtility.Refresh();
            SceneManagerWindow.Open();

            string[] ProfileNames() =>
                AssetDatabase.FindAssets("t:Profile").Select(AssetDatabase.GUIDToAssetPath).Select(path => Path.GetFileNameWithoutExtension(path)).ToArray();

        }

        #endregion

    }

}
