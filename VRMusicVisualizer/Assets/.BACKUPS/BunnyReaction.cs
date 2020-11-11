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
    private int sizeRange;
    private int changeSetLeftBound;
    private int changeSetRightBound;
    private float timeSinceChangeBounds = 999f;
    private float changeBoundsEvery = 15f;
    private float clock = 0f;
    private int HOP = 1;
    private int IDLE = 0;

    // Start is called before the first frame update
    void Start()
    {
        BeatCollector.registerListener(recieveBeat);
        realObjs = new GameObject[numBuns];
        realObjsAnimators = new Animator[numBuns];
        for (int i = 0; i < numBuns; i++) {
            Transform t = new GameObject().transform;
            t.position += Vector3.up * 0.1f;
            t.position += Vector3.right * UnityEngine.Random.Range(-100.0f, 100.0f);
            t.position += Vector3.forward * UnityEngine.Random.Range(-100.0f, 100.0f);
            realObjs[i] = (GameObject) Instantiate(bunny, t.position, t.rotation);
            realObjsAnimators[i] = realObjs[i].GetComponent<Animator>();
        }
        sizeRange = (int) Math.Floor(percentChange * realObjs.Length / 100);
    }

    // Update is called once per frame
    void Update()
    {
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
        if (hasBeat) {
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
                    realObjsAnimators[i].SetInteger("AnimIndex", HOP);
                    realObjsAnimators[i].SetTrigger("Next");
                    // jump "forward" based on its rotation
                    rigBod.AddForce(realObjs[i].transform.forward * bunForce, ForceMode.VelocityChange);
                }
            }
            hasBeat = false;
        }
    }

    void recieveBeat() {
        hasBeat = true;
    }
}
}
