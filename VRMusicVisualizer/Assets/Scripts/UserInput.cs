using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.SceneManagement;

namespace Assets.Scripts {

    public class UserInput : MonoBehaviour 
    {
        public SteamVR_Action_Boolean ChangeScene;
        public SteamVR_Action_Boolean MakeBeat;
        public SteamVR_Input_Sources handLeft; 
        public SteamVR_Input_Sources handRight;
        public string nextScene;
        public string prevScene;
        private bool changingScene = false;

        void Start() {
            ChangeScene.AddOnStateDownListener(triggerPrevScene, handLeft);
            ChangeScene.AddOnStateDownListener(triggerNextScene, handRight);
            MakeBeat.AddOnStateDownListener(triggerBeat, handRight);
            MakeBeat.AddOnStateDownListener(triggerBeat, handLeft);
        }
        
        public void triggerPrevScene(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
            if (changingScene) return;
            changingScene = true;
            StartCoroutine(LoadYourAsyncScene(prevScene));
        }
        
        public void triggerNextScene(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
            if (changingScene) return;
            changingScene = true;
            StartCoroutine(LoadYourAsyncScene(nextScene));
        }   
        
        public void triggerBeat(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
            BeatCollector.maxOutBeat();
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