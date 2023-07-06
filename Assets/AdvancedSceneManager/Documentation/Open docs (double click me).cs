#if UNITY_EDITOR

using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace AdvancedSceneManager
{

    //1. Extracts zip file with documentation webpage
    //2. Opens offline-index.html
    public static class OpenDocumentation
    {

        [OnOpenAsset]
        public static bool OnOpen(int instanceID, int _)
        {

            //Check if user opened this file
            var openedPath = AssetDatabase.GetAssetPath(EditorUtility.InstanceIDToObject(instanceID));
            if (!openedPath.EndsWith("/AdvancedSceneManager/Documentation/Open docs (double click me).cs"))
                return false;

            var folder = Directory.GetParent(openedPath).FullName;

            //Unzip docs (we always do this since docs may have been updated since last time it was extracted)
            //Open offline-index.html
            if (UnzipDocs(folder))
                Open(folder);

            return true;

        }


        static bool UnzipDocs(string folder)
        {
            if (Directory.Exists(folder + "/docs/_site"))
                return true;

            var zipFile = folder + "/docs.zip";
            if (!File.Exists(zipFile))
            {

                //No zip file, and no extracted files, let's notify user
                Debug.LogError("Could not find zip file.");
                return false;

            }

            //Extract zip file
            ZipFile.ExtractToDirectory(zipFile, folder + "/docs");
            AssetDatabase.Refresh();

            return true;

        }

        static void Open(string folder)
        {

            var relpath = "/docs/_site/offline-index.html";
            var file = folder + relpath;

            //Open offline-index.html
            if (File.Exists(file))
                Application.OpenURL(file);
            else
                Debug.LogError($"Could not find '{relpath}'.");

        }

    }

}
#endif
