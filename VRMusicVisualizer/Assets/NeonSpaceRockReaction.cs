using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

namespace Assets.Scripts
{
public class NeonSpaceRockReaction : MonoBehaviour
{
    // we have 12 sets of prefabs with 3 prefabs in each set
    public GameObject[,] rocks;
    public GameObject[] rockSet1;
    public GameObject[] rockSet2;
    public GameObject[] rockSet3;
    public GameObject[] rockSet4;
    public GameObject[] rockSet5;
    public GameObject[] rockSet6;
    public GameObject[] rockSet7;
    public GameObject[] rockSet8;
    public GameObject[] rockSet9;
    public GameObject[] rockSet10;
    public GameObject[] rockSet11;
    public GameObject[] rockSet12;
    public int numRocks;
    public float percentChange;
    private GameObject[] realObjs;
    // stored info for all our rocks. [index][0] is set index, [index][1] is previous prefab index
    public int[,] partOfSet;
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
        // ugly code to transfer Unity-defined arrays :/
        rocks = new GameObject[12, 3];
        for (int i = 0; i < 3; i++) {
            rocks[0, i] = rockSet1[i];
            rocks[1, i] = rockSet2[i];
            rocks[2, i] = rockSet3[i];
            rocks[3, i] = rockSet4[i];
            rocks[4, i] = rockSet5[i];
            rocks[5, i] = rockSet6[i];
            rocks[6, i] = rockSet7[i];
            rocks[7, i] = rockSet8[i];
            rocks[8, i] = rockSet9[i];
            rocks[9, i] = rockSet10[i];
            rocks[10, i] = rockSet11[i];
            rocks[11, i] = rockSet12[i];
        }
        if (active) CreateObjs(false);
        else {
            realObjs = new GameObject[numRocks];
            partOfSet = new int[numRocks, 2];
            destroyed = true;
        }
    }

    void CreateObjs(Boolean small) {
        realObjs = new GameObject[numRocks];
        partOfSet = new int[numRocks, 2];
        for (int i = 0; i < numRocks; i++) {
            // determine which prefab set we'll be using for this rock
            int set = UnityEngine.Random.Range(0, 12);
            int ran = UnityEngine.Random.Range(0, 3);
            GameObject prefab = rocks[set, ran];
            // store this data so we reuse the same set but dont reuse the same index
            partOfSet[i, 0] = set;
            partOfSet[i, 1] = ran;
            Transform t = new GameObject().transform;
            // create rocks all around the player, randomly between -150 and 150 on every axis
            do {
                t.position += Vector3.up * UnityEngine.Random.Range(15.0f, 250.0f);
                t.position += Vector3.right * UnityEngine.Random.Range(-250.0f, 250.0f);
                t.position += Vector3.forward * UnityEngine.Random.Range(-250.0f, 250.0f);
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
        originalScales[0] = new Vector3(1, 1, 1);
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
                // on each beat, change the prefab of rocks in our current range by destroying them and replacing with a random other one in their set
                for (int i = changeSetLeftBound; i < changeSetRightBound; i++) {
                    // keep the transform
                    Transform t = realObjs[i].transform;
                    // look up their set and previous prefab index
                    int set = partOfSet[i, 0];
                    int prefabIndex = partOfSet[i, 1];
                    // select a new prefab index that's sequential from the last one
                    prefabIndex ++;
                    if (prefabIndex > 2) prefabIndex = 0;
                    // grab the new prefab from the set
                    GameObject prefab = rocks[set, prefabIndex];
                    // destroy old and instantiate new
                    Destroy(realObjs[i]);
                    realObjs[i] = (GameObject) Instantiate(prefab, t.position, t.rotation);
                    // store new prefab index
                    partOfSet[i, 1] = prefabIndex;
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
