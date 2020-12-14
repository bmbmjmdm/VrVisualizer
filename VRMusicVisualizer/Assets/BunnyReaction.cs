using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

namespace Assets.Scripts
{
public class BunnyReaction : MonoBehaviour
{
    public GameObject bunny;
    public int numBuns;
    public float percentChange;
    public int bunForce = 1;
    public int bunTorque = 40;
    private GameObject[] realObjs;
    private Animator[] realObjsAnimators;
    private bool hasBeat = false;
    public bool active = true;
    private int sizeRange;
    private int changeSetLeftBound;
    private int changeSetRightBound;
    private float timeSinceChangeBounds = 999f;
    private float changeBoundsEvery = 15f;
    private float fadeOutClock = 0f;
    private int HOP = 1;
    private int IDLE = 0;
    private Vector3[] originalScales = new Vector3[1];
    private bool destroyed = false;
    private float clock = 0f;

    // Start is called before the first frame update
    void Start()
    {
        BeatCollector.registerBeatListener(recieveBeat);
        BeatCollector.registerVerseListener(toggleActive);
        if (active) CreateObjs(false);
        else {
            realObjs = null;
            destroyed = true;
        }
    }

    void CreateObjs(Boolean small) {
        realObjs = new GameObject[numBuns];
        realObjsAnimators = new Animator[numBuns];
        for (int i = 0; i < numBuns; i++) {
            Vector3 t = new Vector3();
            t += Vector3.up * 0.1f;
            t += Vector3.right * UnityEngine.Random.Range(-100.0f, 100.0f);
            t += Vector3.forward * UnityEngine.Random.Range(-100.0f, 100.0f);
            realObjs[i] = (GameObject) Instantiate(bunny, t, Quaternion.identity);
            // if we're starting small, set the size to 0 so we can fade in
            if (small){
                realObjs[i].transform.localScale = new Vector3(0,0,0);
            }
            realObjsAnimators[i] = realObjs[i].GetComponent<Animator>();
        }
        sizeRange = (int) Math.Floor(percentChange * realObjs.Length / 100);
        originalScales[0] = bunny.transform.localScale;
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
                    realObjs = null;
                }
            }
        }
        // we're active
        else {
            // fade in all objects
            // if any of them aren't done fading in, dont proceed
            if (destroyed) {
                // if this is our first iteration after being destroyed, instantiate all objects
                if (realObjs == null) CreateObjs(true);
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
            if (hasBeat && clock >= 0.75f) {
                for (int i = changeSetLeftBound; i < changeSetRightBound; i++) {
                    // 50/50 shot
                    int rand = UnityEngine.Random.Range(0, 2);
                    int rand2 = UnityEngine.Random.Range(0, 2);
                    Rigidbody rigBod = realObjs[i].GetComponent<Rigidbody>();
                    if (rand == 0) {
                        int negate = rand2 == 0 ? -1 : 1;
                        rigBod.AddTorque(Vector3.up * bunTorque * negate, ForceMode.VelocityChange);
                    }
                    else {
                        // wait for jump to finish afterwards
                        clock = 0f;
                        realObjsAnimators[i].SetInteger("AnimIndex", HOP);
                        realObjsAnimators[i].SetTrigger("Next");
                        // jump "forward" based on its rotation
                        StartCoroutine(delayedForce(rigBod, realObjs[i].transform.forward * bunForce));
                    }
                }
                hasBeat = false;
            }
            else {
                // we missed the beat cause we were already jumping
                if (hasBeat) {
                    hasBeat = false;
                }
                clock += Time.deltaTime;
            }
        }
    }
    
    // delay the jump forward by a split second
    IEnumerator delayedForce(Rigidbody rigBod, Vector3 vector) {
        yield return new WaitForSeconds(0.13f);
        rigBod.AddForce(vector, ForceMode.VelocityChange);
    }

    void recieveBeat() {
        hasBeat = true;
    }

    void toggleActive() {
        active = !active;
    }
}
}
