﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

namespace Assets.Scripts
{
public class CrystalReaction : MonoBehaviour
{
    public GameObject[] crystals;
    public int numCrystals;
    public float percentChange;
    private GameObject[] realObjs;
    private bool hasBeat = false;
    public bool active = true;
    private int sizeRange;
    private int changeSetLeftBound;
    private int changeSetRightBound;
    private float timeSinceChangeBounds = 999f;
    private float changeBoundsEvery = 10f;
    private float fadeOutClock = 0f;
    private Vector3[] originalScales = new Vector3[1];
    private bool destroyed = false;

    // Start is called before the first frame update
    void Start()
    {
        BeatCollector.registerBeatListener(recieveBeat);
        BeatCollector.registerVerseListener(toggleActive);
        CreateObjs(false);
    }

    void CreateObjs(Boolean small) {
        realObjs = new GameObject[numCrystals];
        for (int i = 0; i < numCrystals; i++) {
            // exclusive so dont have to do Length-1
            int ran = UnityEngine.Random.Range(0, crystals.Length);
            GameObject prefab = crystals[ran];
            Transform t = new GameObject().transform;
            t.position = new Vector3(0f, 0f, 0f);
            t.position += Vector3.right * UnityEngine.Random.Range(-150.0f, 150.0f);
            t.position += Vector3.forward * UnityEngine.Random.Range(-150.0f, 150.0f);
            realObjs[i] = (GameObject) Instantiate(prefab, t.position, t.rotation);
            // if we're starting small, set the size to 0 so we can fade in
            if (small){
                realObjs[i].transform.localScale = new Vector3(0,0,0);
            }
        }
        sizeRange = (int) Math.Floor(percentChange * realObjs.Length / 100); 
        originalScales[0] = crystals[0].transform.localScale;
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
            if (hasBeat) {
                // on each beat, change the color of crystals in our current range by destroying them and replacing with a random other colored one
                int ran = UnityEngine.Random.Range(0, crystals.Length);
                GameObject prefab = crystals[ran];
                for (int i = changeSetLeftBound; i < changeSetRightBound; i++) {
                    Transform t = realObjs[i].transform;
                    Destroy(realObjs[i]);
                    realObjs[i] = (GameObject) Instantiate(prefab, t.position, t.rotation);
                }
                hasBeat = false;
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
