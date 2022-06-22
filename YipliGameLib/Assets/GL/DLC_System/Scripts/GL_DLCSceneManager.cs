using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using UnityEngine.SceneManagement;

namespace Yipli.GameLib.DlcSystem
{
    public class GL_DLCSceneManager : MonoBehaviour
    {
        // private variables
        private string sceneDownloadUrl = "file:///Users/yipli-m1/FW/UP/yipli_game_lib/YipliGameLib/Assets/GL/AB/Resources/Android/scene_ygl";
        private string sceneInBundle = "";

        // assets variables
        AssetBundle sceneBundle;

        // Unity operations
        private void Start()
        {
            StartCoroutine(DownloadAllScenes());
        }

        // Dlc operations
        private IEnumerator DownloadAllScenes()
        {
            UnityWebRequest scenesDLCrequest = UnityWebRequestAssetBundle.GetAssetBundle(sceneDownloadUrl);

            yield return scenesDLCrequest.SendWebRequest();
            if (scenesDLCrequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Scene Download failed : {scenesDLCrequest.error}");
                yield break;
            }

            sceneBundle = DownloadHandlerAssetBundle.GetContent(scenesDLCrequest);
            string[] scenePaths = sceneBundle.GetAllScenePaths();

            foreach (string path in scenePaths)
            {
                sceneInBundle = Path.GetFileNameWithoutExtension(path);
            }

            if (sceneInBundle.Length <= 2) yield break;

            SceneManager.LoadScene(sceneInBundle);
        }
    }
}