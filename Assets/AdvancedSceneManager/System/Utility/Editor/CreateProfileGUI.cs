#if UNITY_EDITOR

using System;
using System.Linq;
using AdvancedSceneManager.Editor.Utility;
using UnityEngine;
using UnityEngine.UIElements;

namespace AdvancedSceneManager.Editor.Window
{

    internal class CreateProfileGUI
    {

        public CreateProfileGUI(Button doneButton, string profileName, BlacklistUtility.BlacklistModule blacklist = null, bool hideBlacklist = false)
        {
            this.doneButton = doneButton;
            this.blacklist = blacklist ?? new BlacklistUtility.BlacklistModule();
            this.profileName = profileName;
            this.hideBlacklist = hideBlacklist;
            element = new VisualElement();
            Setup();
        }

        readonly Button doneButton;
        public BlacklistUtility.BlacklistModule blacklist { get; }
        public string profileName { get; private set; }
        public bool hideBlacklist { get; }

        public VisualElement element { get; }
        public StyleSheet style { get; private set; }

        TextField profileNameField;
        IMGUIContainer blacklistContainer;
        VisualElement blacklistContainerParent;
        Label errorMessage;

        void Setup()
        {

            var template = Resources.Load<VisualTreeAsset>("AdvancedSceneManager/CreateProfileGUI/CreateProfileGUI");
            template.CloneTree(element);

            var uss = Resources.LoadAll("AdvancedSceneManager/Tabs/Welcome/Tab").OfType<StyleSheet>().Where(s => !s.name.Contains("inline")).FirstOrDefault();
            element.styleSheets.Add(uss);
            style = uss;

            profileNameField = element.Q<TextField>("profileName");
            blacklistContainer = element.Q<IMGUIContainer>("blacklistContainer");
            blacklistContainerParent = element.Q<VisualElement>("blacklistContainerParent");
            errorMessage = element.Q<Label>("errorMessage");

            ValidateName(profileName);
            profileNameField.SetValueWithoutNotify(profileName);
            profileNameField.RegisterCallback<ChangeEvent<string>>(e =>
            {
                ValidateName(e.newValue);
                profileName = e.newValue;
            });

            blacklistContainer.onGUIHandler = () =>
            {
                var (_, _, height) = BlacklistUtility.DrawGUI(blacklist);
                blacklistContainerParent.style.minHeight = height;
            };

            if (hideBlacklist)
                element.Q("blacklistPanel").RemoveFromHierarchy();

        }

        void ValidateName(string name)
        {

            var (isError, message) = IsValid(name);
            errorMessage.text = message;
            errorMessage.visible = isError;

            doneButton.SetEnabled(!isError);

        }

        static (bool isError, string message) IsValid(string name)
        {

            var checks = new Func<string, (bool isError, string message)>[] { IsEmpty, CheckDuplicates };

            foreach (var check in checks)
            {
                var result = check.Invoke(name);
                if (result.isError)
                    return result;
            }

            return (isError: false, "");

        }

        static (bool isError, string message) IsEmpty(string value) =>
            (isError: string.IsNullOrEmpty(value), "");

        static (bool isError, string message) CheckDuplicates(string value) =>
            (isError: SceneManager.assets.profiles.Where(p => p).Any(p => p.name.ToLower() == value?.ToLower()), "The name is already in use.");

    }

}
#endif
