using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

namespace Assets.Scripts
{
public class GalaxyReaction : MonoBehaviour
{
    public GameObject galaxy;
    public int numGalaxies;
    private GameObject[] realObjs;
    private Vector3 onBeatVector = new Vector3(2f, 2f, 2f);
    private Vector3 offBeatVector = new Vector3(-0.016f, -0.016f, -0.016f);
    private int numBeat = 0;
    private bool active = true;
    private float fadeOutClock = 0f;
    float speed = 30f;
    private Vector3[] originalScales = new Vector3[1];
    private bool destroyed = false;
    public int galTorque = 40;

    // Start is called before the first frame update
    void Start()
    {
        BeatCollector.registerBeatListener(recieveBeat);
        BeatCollector.registerVerseListener(toggleActive);
        CreateObjs(false);
    }

    void CreateObjs(Boolean small) {
        realObjs = new GameObject[numGalaxies];
        for (int i = 0; i < numGalaxies; i++) {
            Transform t = new GameObject().transform;
            // create galaxies all around the player, randomly between -300 and 300 on every axis
            do {
                t.position += Vector3.up * UnityEngine.Random.Range(-300.0f, 300.0f);
                t.position += Vector3.right * UnityEngine.Random.Range(-300.0f, 300.0f);
                t.position += Vector3.forward * UnityEngine.Random.Range(-300.0f, 300.0f);
            }
            // however dont let them spawn too close to the player
            while (Utilities.isNearPlayer(t.position));
            realObjs[i] = (GameObject) Instantiate(galaxy, t.position, t.rotation);
            // if we're starting small, set the size to 0 so we can fade in
            if (small){
                realObjs[i].transform.localScale = new Vector3(0,0,0);
            }
        }
        originalScales[0] = galaxy.transform.localScale;
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
            if (numBeat > 0) {
                numBeat = 0;
                //direction = direction == Vector3.up ? Vector3.down : Vector3.up;
                // spin all galaxies in a direction
                for (int i = 0; i < numGalaxies; i++) {
                    Rigidbody rigBod = realObjs[i].GetComponent<Rigidbody>();
                    rigBod.AddTorque(Vector3.up * galTorque, ForceMode.VelocityChange);
                }
            }
        }
    }

    void recieveBeat() {
        numBeat++;
    }

    void toggleActive() {
        active = !active;
    }
}
}
