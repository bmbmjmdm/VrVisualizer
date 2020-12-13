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
        private float clock = 1f;

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

            // introduction logic
            if (IntroProgress.needsFinish) finishTutorial();
            if (!IntroProgress.completedIntro && !IntroProgress.needsLT && !IntroProgress.needsLT_2) return;
            if (!IntroProgress.completedIntro) {
                if (IntroProgress.needsLT) {
                    IntroProgress.needsLT = false;
                    IntroProgress.needsLT_2 = true;
                }
                else {
                    IntroProgress.needsLT_2 = false;
                    IntroProgress.needsButton1 = true;
                }
            }

            changingScene = true;
            StartCoroutine(LoadYourAsyncScene(prevScene));
        }
        
        public void triggerNextScene(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
            if (changingScene) return;
            
            // introduction logic
            if (IntroProgress.needsFinish) finishTutorial();
            if (!IntroProgress.completedIntro && !IntroProgress.needsRT) return;
            if (!IntroProgress.completedIntro) {
                IntroProgress.needsRT = false;
                IntroProgress.needsLT = true;
            }

            changingScene = true;
            StartCoroutine(LoadYourAsyncScene(nextScene));
        }   
        
        public void triggerBeat(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
            // introduction logic
            if (IntroProgress.needsFinish) finishTutorial();
            if (!IntroProgress.completedIntro && !IntroProgress.needsGrip) return;
            if (!IntroProgress.completedIntro) {
                IntroProgress.needsGrip = false;
                IntroProgress.needsRT = true;
            }

            BeatCollector.maxOutBeat();
        }  
        
        public void cycleEffect(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
            if (SceneManager.GetActiveScene().name != "CustomScene") return;

            // introduction logic
            if (IntroProgress.needsFinish) finishTutorial();
            if (!IntroProgress.completedIntro && !IntroProgress.needsButton1 && !IntroProgress.needsButton1_2 && !IntroProgress.needsButton2 && !IntroProgress.needsButton2_2) return;
            if (!IntroProgress.completedIntro && (IntroProgress.needsButton1 || IntroProgress.needsButton1_2)) {
                if (IntroProgress.needsButton1) {
                    IntroProgress.needsButton1 = false;
                    IntroProgress.needsButton2 = true;
                }
                else {
                    IntroProgress.needsButton1_2 = false;
                    IntroProgress.needsButton2_2 = true;
                }
            }

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

            // introduction logic
            if (IntroProgress.needsFinish) finishTutorial();
            if (!IntroProgress.completedIntro && !IntroProgress.needsButton2 && !IntroProgress.needsButton2_2) return;
            if (!IntroProgress.completedIntro) {
                if (IntroProgress.needsButton2) {
                    IntroProgress.needsButton2 = false;
                    IntroProgress.needsButton1_2 = true;
                }
                else {
                    IntroProgress.needsButton2_2 = false;
                    IntroProgress.needsButton3 = true;
                }
            }
            
            if (this.isAddingEffect) {
                this.isAddingEffect = false;
                BeatCollector.confirmEffect();
            }
        }    
        
        public void cycleDeleteEffect(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
            if (SceneManager.GetActiveScene().name != "CustomScene") return;

            // introduction logic
            if (IntroProgress.needsFinish) finishTutorial();
            if (!IntroProgress.completedIntro && !IntroProgress.needsButton3 && !IntroProgress.needsButton4) return;
            if (!IntroProgress.completedIntro && IntroProgress.needsButton3) {
                IntroProgress.needsButton3 = false;
                IntroProgress.needsButton4 = true;
            }

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

            // introduction logic
            if (IntroProgress.needsFinish) finishTutorial();
            if (!IntroProgress.completedIntro && !IntroProgress.needsButton4) return;
            if (!IntroProgress.completedIntro) {
                IntroProgress.needsButton4 = false;
                IntroProgress.needsFinish = true;
            }

            
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

        void finishTutorial () {
            IntroProgress.needsFinish = false;
            IntroProgress.completedIntro = true;
            BeatCollector.needSave = true;
        }
    }

}