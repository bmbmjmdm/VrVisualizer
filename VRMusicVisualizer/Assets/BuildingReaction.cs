using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

namespace Assets.Scripts
{
public class BuildingReaction : MonoBehaviour
{
    public GameObject[] buildings;
    private int numBuildings;
    private GameObject[] realObjs;
    public bool active = true;
    private float clock = 0f;
    private float fadeOutClock = 0f;
    private Vector3[] originalScales;
    private bool destroyed = false;

    // Start is called before the first frame update
    void Start()
    {
        BeatCollector.registerVerseListener(toggleActive);
        numBuildings = buildings.Length;
        originalScales = new Vector3[numBuildings];
        // for some reason suns act funny when we dont fade them in, so fade them in even if theyre on by default
        //if (active) CreateObjs(false);
        //else {
            realObjs = new GameObject[numBuildings];
            destroyed = true;
        //}
    }

    void CreateObjs(Boolean small) {
        realObjs = new GameObject[numBuildings];
        for (int i = 0; i < numBuildings; i++) {
            // unlike most reactions, we dont choose a random prefab, but rather just make 1 of each
            GameObject prefab = buildings[i];
            Transform t = new GameObject().transform;
            t.position = new Vector3(0f, 0f, 0f);
            do {
                t.position += Vector3.right * UnityEngine.Random.Range(-200.0f, 200.0f);
                t.position += Vector3.forward * UnityEngine.Random.Range(-200.0f, 200.0f);
            }
            while (Utilities.isNearPlayerFar(t.position));
            realObjs[i] = (GameObject) Instantiate(prefab, t.position, t.rotation);
            // if we're starting small, set the size to 0 so we can fade in
            if (small){
                realObjs[i].transform.localScale = new Vector3(0,0,0);
            }
            originalScales[i] = prefab.transform.localScale;
        }
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

            // reduce jitters by changing scale at constant interval
            // dont change a building's scale if it only changed by a tiny tiny bit. this gets rid of jitters 
            clock += Time.deltaTime;
            if (clock >= 0.02 && BeatCollector.midSig) {
                clock = 0f;
                // set each building's size relative to our current spectrum mid average percentage
                for (int i = 0; i < numBuildings; i++) {
                    Vector3 scale = originalScales[i];
                    float yChange = scale.y * BeatCollector.getMidPer();
                    realObjs[i].transform.localScale = new Vector3(scale.x, yChange, scale.z);
                }
            }
        }
    }

    void toggleActive() {
        active = !active;
    }
}
}
