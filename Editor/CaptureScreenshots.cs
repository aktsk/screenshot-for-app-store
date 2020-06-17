using System.Collections;
using System.IO;
using System.Reflection;
using Kyusyukeigo.Helper;
using UnityEditor;
using UnityEngine;
using UnityExtensions;

namespace CaptureScreenshotsForAppStore
{
    /// <summary>
    /// The Unity editor extension to capture screenshots for App Store 
    /// </summary>
    public class CaptureScreenshotsForAppStore : Editor
    {
        class GameViewSize : GameViewSizeHelper.GameViewSize
        {
            internal GameViewSize(int width, int height, string baseText)
            {
                type = GameViewSizeHelper.GameViewSizeType.FixedResolution;
                this.width = width;
                this.height = height;
                this.baseText = baseText;
            }
        }

        static readonly GameViewSize[] _customSizes = {
            new GameViewSize(1242, 2688, "6.5"),
            new GameViewSize(1242, 2208, "5.5"),
            new GameViewSize(2048, 2732, "12.9"),
        };

        static IEnumerator CaptureScreenshot(int number)
        {
            string directoryName = "screenshots";

            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            var assembly = typeof(EditorWindow).Assembly;
            var groupType = currentSizeGroupType(assembly);
            var type = assembly.GetType("UnityEditor.GameView");
            var gameview = EditorWindow.GetWindow(type);

            foreach (var customSize in _customSizes)
            {
                if (!GameViewSizeHelper.Contains(groupType, customSize))
                {
                    GameViewSizeHelper.AddCustomSize(groupType, customSize);
                }

                GameViewSizeHelper.ChangeGameViewSize(groupType, customSize);
                var filename = $"{directoryName}/{customSize.baseText}_{number}.png";
                EditorApplication.Step();
                EditorApplication.Step();
                ScreenCapture.CaptureScreenshot(filename);
                gameview.Repaint();
                Debug.Log($">> CaptureScreenshotsForAppStore : save to {filename}");
                yield return null;
            }
        }

        static GameViewSizeGroupType currentSizeGroupType(Assembly assembly) {
            Type gameView = assembly.GetType("UnityEditor.GameView");
            PropertyInfo currentSizeGroupType = gameView.GetProperty("currentSizeGroupType", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
            return (GameViewSizeGroupType)currentSizeGroupType.GetValue(EditorWindow.GetWindow(gameView), null);
        }

        #region MenuItem methods
        [MenuItem("CaptureScreenshotsForAppStore/CaptureScreenshot1")]
        static void CaptureScreenshot1()
            => EditorCoroutine.Start(CaptureScreenshot(1));

        [MenuItem("CaptureScreenshotsForAppStore/CaptureScreenshot2")]
        static void CaptureScreenshot2()
            => EditorCoroutine.Start(CaptureScreenshot(2));

        [MenuItem("CaptureScreenshotsForAppStore/CaptureScreenshot3")]
        static void CaptureScreenshot3()
            => EditorCoroutine.Start(CaptureScreenshot(3));
        #endregion
    }
}
