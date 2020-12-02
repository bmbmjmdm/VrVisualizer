using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

namespace Assets.Scripts
{
public class PlanetReaction : MonoBehaviour
{
    public GameObject[] planets;
    public int numPlanets;
    public float percentChange;
    private GameObject[] realObjs;
    private Vector3 onBeatVector = new Vector3(2f, 2f, 2f);
    private Vector3 offBeatVector = new Vector3(-0.016f, -0.016f, -0.016f);
    private bool hasBeat = false;
    public bool active = true;
    private int sizeRange;
    private int changeSetLeftBound;
    private int changeSetRightBound;
    private float timeSinceChangeBounds = 999f;
    private float changeBoundsEvery = 10f;
    private float clock = 0f;
    private Vector3[] originalScales = new Vector3[1];
    private bool destroyed = false;
    private float fadeOutClock = 0f;

    // Start is called before the first frame update
    void Start()
    {
        BeatCollector.registerBeatListener(recieveBeat);
        BeatCollector.registerVerseListener(toggleActive);
        if (active) CreateObjs(false);
        else {
            realObjs = new GameObject[numPlanets];
            destroyed = true;
        }
    }

    void CreateObjs(Boolean small) {
        realObjs = new GameObject[numPlanets];
        for (int i = 0; i < numPlanets; i++) {
            // exclusive so dont have to do Length-1
            int ran = UnityEngine.Random.Range(0, planets.Length);
            GameObject prefab = planets[ran];
            // create planets all around the player, randomly between -300 and 300 on every axis
            Transform t = new GameObject().transform;
            do {
                t.position = new Vector3(0f, 0f, 0f);
                t.position += Vector3.up * UnityEngine.Random.Range(-300.0f, 300.0f);
                t.position += Vector3.right * UnityEngine.Random.Range(-300.0f, 300.0f);
                t.position += Vector3.forward * UnityEngine.Random.Range(-300.0f, 300.0f);
            }
            // however dont let them spawn too close to the player
            while (Utilities.isNearPlayer(t.position));
            realObjs[i] = (GameObject) Instantiate(prefab, t.position, t.rotation);
            // if we're starting small, set the size to 0 so we can fade in
            if (small){
                realObjs[i].transform.localScale = new Vector3(0,0,0);
            }
        }
        sizeRange = (int) Math.Floor(percentChange * realObjs.Length / 100);
        originalScales[0] = planets[0].transform.localScale;
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
            clock += Time.deltaTime;

            // change which objects we're animating every X seconds
            timeSinceChangeBounds += Time.deltaTime;
            if (timeSinceChangeBounds > changeBoundsEvery) {
                timeSinceChangeBounds = 0f;
                // left bound >= 0, right bound < realObjs.Length
                int rand = UnityEngine.Random.Range(0, realObjs.Length - sizeRange);
                changeSetLeftBound = rand;
                changeSetRightBound = rand + sizeRange;
            }
            if (hasBeat && active) {
                for (int i = changeSetLeftBound; i < changeSetRightBound; i++) {
                    realObjs[i].transform.localScale = onBeatVector;
                }
                hasBeat = false;
            }
            else {
                if (clock >= 0.02) {
                    clock = 0f;
                    for (int i = changeSetLeftBound; i < changeSetRightBound; i++) {
                        // going into negative scale causes it to grow again
                        if (realObjs[i].transform.localScale.x > 0) {
                            realObjs[i].transform.localScale += offBeatVector;
                        }
                    }
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
