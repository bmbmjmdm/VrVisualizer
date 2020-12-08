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
        public SteamVR_Action_Boolean CycleAddEffect;
        public SteamVR_Action_Boolean CycleDeleteEffect;
        public SteamVR_Action_Boolean ConfirmAddEffect;
        public SteamVR_Action_Boolean ConfirmDeleteEffect;
        public SteamVR_Input_Sources handLeft; 
        public SteamVR_Input_Sources handRight;
        public string nextScene;
        public string prevScene;
        private bool changingScene = false;
        private bool isAddingEffect = false;
        private bool isDeletingEffect = false;
        private float clock = 1.5f;

        void OnDisable() {
            ChangeScene.RemoveOnStateDownListener(triggerPrevScene, handLeft);
            ChangeScene.RemoveOnStateDownListener(triggerNextScene, handRight);
            MakeBeat.RemoveOnStateDownListener(triggerBeat, handRight);
            MakeBeat.RemoveOnStateDownListener(triggerBeat, handLeft);
            CycleAddEffect.RemoveOnStateDownListener(cycleEffect, handRight);
            CycleDeleteEffect.RemoveOnStateDownListener(cycleDeleteEffect, handLeft);
            ConfirmAddEffect.RemoveOnStateDownListener(confirmEffect, handRight);
            ConfirmDeleteEffect.RemoveOnStateDownListener(confirmDeleteEffect, handLeft);
        }

        void Start() {
            ChangeScene.AddOnStateDownListener(triggerPrevScene, handLeft);
            ChangeScene.AddOnStateDownListener(triggerNextScene, handRight);
            MakeBeat.AddOnStateDownListener(triggerBeat, handRight);
            MakeBeat.AddOnStateDownListener(triggerBeat, handLeft);
            CycleAddEffect.AddOnStateDownListener(cycleEffect, handRight);
            CycleDeleteEffect.AddOnStateDownListener(cycleDeleteEffect, handLeft);
            ConfirmAddEffect.AddOnStateDownListener(confirmEffect, handRight);
            ConfirmDeleteEffect.AddOnStateDownListener(confirmDeleteEffect, handLeft);
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
        
        public void cycleEffect(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
            if (SceneManager.GetActiveScene().name != "CustomScene") return;
            if (clock > 0f) return;
            clock = 1f;
            
            if (this.isDeletingEffect) {
                this.isDeletingEffect = false;
                BeatCollector.abandonDeletingEffect();
                return;
            }
            BeatCollector.cycleEffect(!this.isAddingEffect);
            this.isAddingEffect = true;
        }    
        
        public void confirmEffect(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
            if (SceneManager.GetActiveScene().name != "CustomScene") return;
            
            if (this.isAddingEffect) {
                this.isAddingEffect = false;
                BeatCollector.confirmEffect();
            }
        }    
        
        public void cycleDeleteEffect(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
            if (SceneManager.GetActiveScene().name != "CustomScene") return;
            if (clock > 0f) return;
            clock = 1f;
            
            if (this.isAddingEffect) {
                this.isAddingEffect = false;
                BeatCollector.abandonAddingEffect();
                return;
            }
            BeatCollector.cycleDeletingEffect(!this.isDeletingEffect);
            this.isDeletingEffect = true;
        }    
        
        public void confirmDeleteEffect(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
            if (SceneManager.GetActiveScene().name != "CustomScene") return;
            
            if (this.isDeletingEffect) {
                this.isDeletingEffect = false;
                BeatCollector.confirmDeleteEffect();
            }
        }  

        IEnumerator LoadYourAsyncScene(string sceneName)
        {   
            if (this.isAddingEffect) {
                this.isAddingEffect = false;
                BeatCollector.abandonAddingEffect();
            }
            if (this.isDeletingEffect) {
                this.isDeletingEffect = false;
                BeatCollector.abandonDeletingEffect();
            }
            // The Application loads the Scene in the background as the current Scene runs.
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
        
        void Update() {
            if (clock > 0f) {
                clock -= Time.deltaTime;
            }
        }
    }

}