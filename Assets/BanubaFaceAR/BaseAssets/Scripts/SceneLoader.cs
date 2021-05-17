using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace BNB
{
    public class SceneLoader : MonoBehaviour
    {
        private bool loadScene = false;

        [SerializeField]
        private string scene;
        [SerializeField]
        private Text loadingText;

        // Updates once per frame
        void Update()
        {
            if (loadScene == false) {
                loadScene = true;
                loadingText.text = "Loading...";
                StartCoroutine(LoadScene());
            }

            if (loadScene == true) {
                loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));
            }
        }

        IEnumerator LoadScene()
        {
#if (UNITY_ANDROID || UNITY_WEBGL) && !UNITY_EDITOR
            yield return CopyResourcesToPersistent();
#endif

            AsyncOperation async = SceneManager.LoadSceneAsync(scene);
            while (!async.isDone) {
                yield return null;
            }
        }

#if (UNITY_ANDROID || UNITY_WEBGL) && !UNITY_EDITOR
        IEnumerator CopyResourcesToPersistent()
        {
            // copy resources from assets on Android
            string src = Application.streamingAssetsPath + "/BanubaFaceAR/";
            string dst = Application.persistentDataPath + "/BanubaFaceAR/";

            var resource = Resources.Load<TextAsset>("BNBResourceList");
            var files = ResourcesJSON.CreateFromJSON(resource.text).resources;
            foreach (var file in files) {
                if (file.Length == 0)
                    continue;

                var filename = file.Replace("\r", "");

                var from = src + filename;
                var to = dst + filename;

                var dir = Path.GetDirectoryName(to);
                if (!Directory.Exists(dir)) {
                    Directory.CreateDirectory(dir);
                }

                if (!File.Exists(to)) {
                    Debug.Log("copying from " + from + " to " + to);
                    WWW www = new WWW(from);
                    yield return www;
                    File.WriteAllBytes(to, www.bytes);
                }
            }
        }
#endif
    }

}
