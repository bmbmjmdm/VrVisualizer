using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

namespace Assets.Scripts
{
public class NebulaReaction : MonoBehaviour
{
    public GameObject[] nebuli;
    public int numNebuli;
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
        if (active) CreateObjs(false);
        else {
            realObjs = new GameObject[numNebuli];
            destroyed = true;
        }
    }

    void CreateObjs(Boolean small) {
        realObjs = new GameObject[numNebuli];
        for (int i = 0; i < numNebuli; i++) {
            // exclusive so dont have to do Length-1
            int ran = UnityEngine.Random.Range(0, nebuli.Length);
            GameObject prefab = nebuli[ran];
            Transform t = new GameObject().transform;
            // create nebuli all around the player, randomly between -300 and 300 on every axis
            do {
                t.position += Vector3.up * UnityEngine.Random.Range(-300.0f, 300.0f);
                t.position += Vector3.right * UnityEngine.Random.Range(-300.0f, 300.0f);
                t.position += Vector3.forward * UnityEngine.Random.Range(-300.0f, 300.0f);
                t.eulerAngles = new Vector3(
                    UnityEngine.Random.Range(0.0f, 360.0f),
                    UnityEngine.Random.Range(0.0f, 360.0f),
                    UnityEngine.Random.Range(0.0f, 360.0f)
                );
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
        originalScales[0] = nebuli[0].transform.localScale;
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
                // on each beat, change the color of nebuli in our current range by swapping them with a random other one in our range (swapping positions+rotations)
                // in order to do this, we swap i's position with a random entry from [i, changeSetRightBound]. Repeating this until the end gives us a random new position for each object except the last one in the most efficient way possible
                for (int i = changeSetLeftBound; i < changeSetRightBound; i++) {
                    int ran = UnityEngine.Random.Range(i + 1, changeSetRightBound);
                    // edge case of the last object in our list, which we default to swapping with the first
                    if (ran >= changeSetRightBound) ran = changeSetLeftBound;
                    Vector3 iPosition = realObjs[i].transform.position;
                    Quaternion iRotation = realObjs[i].transform.rotation;
                    Vector3 randomVector = realObjs[ran].transform.position;
                    Quaternion randomRotation = realObjs[ran].transform.rotation;
                    realObjs[i].transform.position = randomVector;
                    realObjs[i].transform.rotation = randomRotation;
                    realObjs[ran].transform.position = iPosition;
                    realObjs[ran].transform.rotation = iRotation;
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
