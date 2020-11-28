using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

namespace Assets.Scripts
{
public class SunReaction : MonoBehaviour
{
    public GameObject sun;
    public int numSuns;
    private GameObject[] realObjs;
    private Vector3 beatVector = new Vector3(0.15f, 0.15f, 0.15f);
    private bool minusVector = false;
    private bool hasBeat = false;
    public bool active = true;
    private float clock = 0f;
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
        realObjs = new GameObject[numSuns];
        for (int i = 0; i < numSuns; i++) {
            Transform t = new GameObject().transform;
            // create galaxies all around the player, randomly between -300 and 300 on every axis
            do {
                t.position += Vector3.up * UnityEngine.Random.Range(-300.0f, 300.0f);
                t.position += Vector3.right * UnityEngine.Random.Range(-300.0f, 300.0f);
                t.position += Vector3.forward * UnityEngine.Random.Range(-300.0f, 300.0f);
            }
            // however dont let them spawn too close to the player
            while (Utilities.isNearPlayer(t.position));
            realObjs[i] = (GameObject) Instantiate(sun, t.position, t.rotation);
            // if we're starting small, set the size to 0 so we can fade in
            if (small){
                realObjs[i].transform.localScale = new Vector3(0,0,0);
            }
        }
        originalScales[0] = sun.transform.localScale;
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
            /* It would be cool to also have a reaction that does the inverse of the sun/flowers (big by default, gets smaller with amplitude)

            if (hasBeat) {
                minusVector = !minusVector;
                hasBeat = false;
            }
            */
            // reduce jitters by changing scale at constant interval
            // dont change a sun's scale if it only changed by a tiny tiny bit. this gets rid of jitters 
            if (clock >= 0.02 && BeatCollector.lowSig) {
                clock = 0f;
                // set each sun's size relative to our current spectrum low-end average percentage
                for (int i = 0; i < numSuns; i++) {
                    float scale = 50.0f * BeatCollector.getLowPer();
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
