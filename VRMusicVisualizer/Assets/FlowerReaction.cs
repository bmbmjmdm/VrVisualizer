﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

namespace Assets.Scripts
{
public class FlowerReaction : MonoBehaviour
{
    public GameObject[] flowers;
    public int numFlowers;
    public float percentChange;
    private GameObject[] realObjs;
    private Vector3 beatVectorGrow = new Vector3(0.03f, 0.03f, 0.03f);
    private Vector3 beatVectorNew = new Vector3(0f, 0f, 0f);
    private bool hasBeat = false;
    public bool active = true;
    private int sizeRange;
    private int changeSetLeftBound;
    private int changeSetRightBound;
    private float timeSinceChangeBounds = 999f;
    private float changeBoundsEvery = 10f;
    private float clock = 0f;
    private float fadeOutClock = 0f;
    private Vector3[] originalScales = new Vector3[1];
    private bool destroyed = false;

    // Start is called before the first frame update
    void Start()
    {
        //BeatCollector.registerBeatListener(recieveBeat);
        BeatCollector.registerVerseListener(toggleActive);
        // for some reason suns act funny when we dont fade them in, so we fade them in even if theyre on by default. do this for flowers just incase im missing something
        //if (active) CreateObjs(false);
        //else {
            realObjs = new GameObject[numFlowers];
            destroyed = true;
        //}
    }

    void CreateObjs(Boolean small) {
        realObjs = new GameObject[numFlowers];
        for (int i = 0; i < numFlowers; i++) {
            // exclusive so dont have to do Length-1
            int ran = UnityEngine.Random.Range(0, flowers.Length);
            GameObject prefab = flowers[ran];
            Vector3 t = new Vector3();
            t = new Vector3(0f, 0f, 0f);
            t += Vector3.right * UnityEngine.Random.Range(-150.0f, 150.0f);
            t += Vector3.forward * UnityEngine.Random.Range(-150.0f, 150.0f);
            realObjs[i] = (GameObject) Instantiate(prefab, t, Quaternion.identity);
            // if we're starting small, set the size to 0 so we can fade in
            if (small){
                realObjs[i].transform.localScale = new Vector3(0,0,0);
            }
        }
        sizeRange = (int) Math.Floor(percentChange * realObjs.Length / 100); 
        originalScales[0] = flowers[0].transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        // if we're not active, destroy all objects
        if (!active) {
            if (!destroyed) {
                // we're still destroying
                Boolean isDestroyed = !Utilities.fadeOutObjects(realObjs, originalScales, ref fadeOutClock, Time.deltaTime);
                if (isDestroyed) {
                    // we're done destroying
                    destroyed = true;
                }
            }
        }
        // we're active
        else {
            // fade in all objects
            // if any of them aren't done fading in, dont proceed
            if (destroyed) {
                // if this is our first iteration after being destroyed, instantiate all objects
                if (realObjs[0] == null) CreateObjs(true);
                // we're still growing
                Boolean isFull = !Utilities.fadeInObjects(realObjs, originalScales, ref fadeOutClock, Time.deltaTime);
                if (isFull) {
                    // we're done growing
                    destroyed = false;
                }
                return;
            }

            // change which objects we're animating every X seconds
            timeSinceChangeBounds += Time.deltaTime;
            if (timeSinceChangeBounds > changeBoundsEvery) {
                timeSinceChangeBounds = 0f;
                // left bound >= 0, right bound < realObjs.Length
                int rand = UnityEngine.Random.Range(0, realObjs.Length - sizeRange);
                changeSetLeftBound = rand;
                changeSetRightBound = rand + sizeRange;
            }

            /* This is commented out because it looks wierd having the flowers jump around
            if (hasBeat) {
                for (int i = changeSetLeftBound; i < changeSetRightBound; i++) {
                    Vector3 temp = new Vector3(0f, 0f, 0f);
                    temp += Vector3.right * UnityEngine.Random.Range(-125.0f, 125.0f);
                    temp += Vector3.forward * UnityEngine.Random.Range(-125.0f, 125.0f);
                    realObjs[i].transform.position = temp;
                }
                hasBeat = false;
            }*/

            // reduce jitters by changing scale at constant interval
            // dont change a flower's scale if it only changed by a tiny tiny bit. this gets rid of jitters 
            clock += Time.deltaTime;
            if (clock >= 0.02 && BeatCollector.midSig) {
                clock = 0f;
                // set each flower's size relative to our current spectrum mid-end average percentage
                for (int i = changeSetLeftBound; i < changeSetRightBound; i++) {
                    float scale = 3.0f * BeatCollector.getMidPer();
                    realObjs[i].transform.localScale = new Vector3(scale, scale, scale);
                }
            }
        }
    }

    void recieveBeat() {
        hasBeat = true;
    }

    void toggleActive() {
        active = !active;
    }
}
}
