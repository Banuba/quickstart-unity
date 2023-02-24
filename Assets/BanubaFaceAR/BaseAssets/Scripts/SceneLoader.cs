using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BNB
{
    public class SceneLoader : MonoBehaviour
    {
        private const int _sceneToLoadIndex = 1;

        [SerializeField]
        private string _scene;
        [SerializeField]
        private Text _loadingText;
        private bool _loadScene;

        private void Update()
        {
            if (_loadScene == false) {
                _loadScene = true;
                _loadingText.text = "Loading...";
                StartCoroutine(LoadScene());
            }

            if (_loadScene) {
                _loadingText.color = new Color(_loadingText.color.r, _loadingText.color.g, _loadingText.color.b, Mathf.PingPong(Time.time, 1));
            }
        }

        private IEnumerator LoadScene()
        {
#if (UNITY_ANDROID || UNITY_WEBGL) && !UNITY_EDITOR
            yield return CopyResourcesToPersistent();
#endif

            AsyncOperation async = SceneManager.LoadSceneAsync(_sceneToLoadIndex);
            while (!async.isDone) {
                yield return null;
            }
        }

#if (UNITY_ANDROID || UNITY_WEBGL) && !UNITY_EDITOR
        private IEnumerator CopyResourcesToPersistent()
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
                    UnityWebRequest www = UnityWebRequest.Get(from);
                    yield return www.SendWebRequest();
                    File.WriteAllBytes(to, www.downloadHandler.data);
                }
            }
        }
#endif
    }

}
