using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.SceneManagement;

namespace Assets.Scripts {

    public class SceneChange : MonoBehaviour 
    {
        public SteamVR_Action_Boolean ChangeScene;
        public SteamVR_Input_Sources handLeft; 
        public SteamVR_Input_Sources handRight;
        public string nextScene;
        public string prevScene;
        private bool changingScene = false;

        void Start() {
            ChangeScene.AddOnStateDownListener(triggerPrevScene, handLeft);
            ChangeScene.AddOnStateDownListener(triggerNextScene, handRight);
        }
        
        public void triggerPrevScene(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
            if (changingScene) return;
            changingScene = true;
            StartCoroutine(LoadYourAsyncScene(nextScene));
        }
        
        public void triggerNextScene(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
            if (changingScene) return;
            changingScene = true;
            StartCoroutine(LoadYourAsyncScene(nextScene));
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