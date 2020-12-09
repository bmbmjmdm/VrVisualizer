using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

namespace Assets.Scripts {

    public class StartScene : MonoBehaviour 
    {

        void Start() {
            string scene = "SpaceScene";
            if (File.Exists(Application.persistentDataPath + "/savedEnvironment.vrmv")) {
                scene = "CustomScene";
            }
            StartCoroutine(LoadYourAsyncScene(scene));
        }

        IEnumerator LoadYourAsyncScene(string sceneName)
        {
            // The Application loads the Scene in the background as the current Scene runs.
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }

        void Update() {}
    }

}