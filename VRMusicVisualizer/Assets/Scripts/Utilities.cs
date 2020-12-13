using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

// NOTE ALL REACTIONS THAT FADE IN / OUT NEED TO HAVE NUM OBJECTS >= 1. 
// DO NOT ADD A REACTION SCRIPT TO THE SCENE IF YOU HAVE THE NUM OBJECTS = 0

namespace Assets.Scripts{
     public static class Utilities {

        public static Boolean fadeOutObjects(GameObject[] realObjs, Vector3[] originalScaleArray, ref float fadeOutClock, float deltaTime) {
            // keep track of if we need to keep calling this fun
            Boolean anyFadeOut = false;
            Vector3 originalScale;

            fadeOutClock += deltaTime;
            // every twentieth of a second
            if (fadeOutClock > 0.05f) {
                fadeOutClock = 0.0f;
                // go through all objects
                for (int i = 0; i < realObjs.Length; i++) {
                    // use either the default original scale or the one corresponding to our array object
                    if (originalScaleArray.Length == 1) originalScale = originalScaleArray[0];
                    else originalScale = originalScaleArray[i];
                    // check if its already been destroyed
                    if (realObjs[i] == null) continue;
                    // and decrease their scale by 10% of the original scale
                    if (realObjs[i].transform.localScale.x > originalScale.x) {
                        realObjs[i].transform.localScale = originalScale;
                        anyFadeOut = true;
                    }
                    else if (realObjs[i].transform.localScale.x > originalScale.x * 0.1f) {
                        realObjs[i].transform.localScale -= originalScale * 0.1f;
                        anyFadeOut = true;
                    }
                    else if (realObjs[i].transform.localScale.x > 0) {
                        realObjs[i].transform.localScale = originalScale * 0;
                        anyFadeOut = true;
                    }
                    else {
                        // if the object is done shrinking, DESTROY IT
                        GameObject.Destroy(realObjs[i]);
                    }
                }
            }
            else {
                // since we haven't reached the time req yet, we assume we're still fading out.
                // once we read 0.05 seconds and nothing needs resizing, we're done
                anyFadeOut = true;
            }

            return anyFadeOut;
        }

        public static Boolean fadeInObjects(GameObject[] realObjs, Vector3[] originalScaleArray, ref float fadeOutClock, float deltaTime) {
            // keep track if any needed fading in
            // return false if we're done
            Boolean anyFadeIn = false;
            Vector3 originalScale;

            fadeOutClock += deltaTime;
            // every twentieth of a second
            if (fadeOutClock > 0.05f) {
                fadeOutClock = 0.0f;
                // go through all objects
                for (int i = 0; i < realObjs.Length; i++) {
                    // use either the default original scale or the one corresponding to our array object
                    if (originalScaleArray.Length == 1) originalScale = originalScaleArray[0];
                    else originalScale = originalScaleArray[i];
                    // and increase their scale by 10% of the original scale
                    if (realObjs[i].transform.localScale.x < originalScale.x) {
                        realObjs[i].transform.localScale += originalScale * 0.1f;
                        anyFadeIn = true;
                    }
                    // this should never happen, but if they grow beyond the original scale, set them to it
                    if (realObjs[i].transform.localScale.x > originalScale.x) {
                        realObjs[i].transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
                    }
                }
            }
            else {
                // since we haven't reached the time req yet, we assume we're still fading in.
                // once we read 0.05 seconds and nothing needs resizing, we're done
                anyFadeIn = true;
            }
            return anyFadeIn;
        }

        // within 15 of player on all axis is too close
        public static Boolean isNearPlayer (Vector3 position) {
            if (position.x > -15.0f && position.x < 15.0f) {
                if (position.y > -15.0f && position.y < 15.0f) {
                    if (position.z > -15.0f && position.z < 15.0f) {
                        return true;
                    }
                }
            }
            return false;
        }

        // within 100 of player on 2d axis is too close
        public static Boolean isNearPlayerFar (Vector3 position) {
            if (position.x > -100.0f && position.x < 100.0f) {
                if (position.z > -100.0f && position.z < 100.0f) {
                    return true;
                }
            }
            return false;
        }

     }
}