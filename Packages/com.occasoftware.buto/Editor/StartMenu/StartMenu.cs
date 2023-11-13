using UnityEngine;
using UnityEditor;

namespace OccaSoftware.Buto.Editor
{
    public class StartMenu : EditorWindow
    {
        // Source for UUID: https://shortunique.id/
        private static string modalIdKey = "Buto";
        private static string modalIdShortUUID = "NaSOmj";
        private static string key = $"{modalIdKey}?id={modalIdShortUUID}";
        private static string shownState = "Shown";

        private static bool listenToEditorUpdates;
        private static StartMenu startMenu;

        [MenuItem("Tools/OccaSoftware/Buto/Start Menu")]
        public static void SetupMenu()
        {
            startMenu = CreateWindow();
            CenterWindowInEditor(startMenu);
        }

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            RegisterModal();
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("Thank you for downloading Buto!");
            EditorGUILayout.LabelField("If you have any questions or need any support, I am here to help.");
            EditorGUILayout.LabelField("In the meantime, here are some helpful links:");
            if (EditorGUILayout.LinkButton("Documentation"))
            {
                Application.OpenURL("https://docs.occasoftware.com/buto/");
            }
            if (EditorGUILayout.LinkButton("Support on Discord"))
            {
                Application.OpenURL("https://www.occasoftware.com/discord");
            }
            if (EditorGUILayout.LinkButton("Join my Newsletter"))
            {
                Application.OpenURL("https://www.occasoftware.com/newsletter");
            }
            if (EditorGUILayout.LinkButton("Get the OccaSoftware Bundle"))
            {
                Application.OpenURL("https://occasoftware.com/occasoftware-bundle");
            }
        }

        #region Setup
        private static StartMenu CreateWindow()
        {
            StartMenu startMenu = (StartMenu)GetWindow(typeof(StartMenu));
            startMenu.position = new Rect(0, 0, 400, 200);
            return startMenu;
        }

        private static void CenterWindowInEditor(EditorWindow startMenu)
        {
            Rect mainWindow = EditorGUIUtility.GetMainWindowPosition();
            Rect currentWindowPosition = startMenu.position;
            float centerX = (mainWindow.width - currentWindowPosition.width) * 0.5f;
            float centerY = (mainWindow.height - currentWindowPosition.height) * 0.5f;
            currentWindowPosition.x = mainWindow.x + centerX;
            currentWindowPosition.y = mainWindow.y + centerY;
            startMenu.position = currentWindowPosition;
        }
        #endregion

        #region Modal Handler
        private static void RegisterModal()
        {
            if (!listenToEditorUpdates && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                listenToEditorUpdates = true;
                EditorApplication.update += PopModal;
            }
        }

        private static void PopModal()
        {
            EditorApplication.update -= PopModal;
            string storedModalIdValue = EditorPrefs.GetString(key, "");
            if (string.IsNullOrEmpty(storedModalIdValue) || storedModalIdValue != shownState)
            {
                EditorPrefs.SetString(key, shownState);
                SetupMenu();
            }
        }
        #endregion
    }
}
