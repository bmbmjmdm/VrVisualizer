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
    private int sizeRange;
    private int changeSetLeftBound;
    private int changeSetRightBound;
    private float timeSinceChangeBounds = 999f;
    private float changeBoundsEvery = 10f;
    private float clock = 0f;

    // Start is called before the first frame update
    void Start()
    {
        BeatCollector.registerListener(recieveBeat);
        realObjs = new GameObject[numPlanets];
        for (int i = 0; i < numPlanets; i++) {
            // exclusive so dont have to do Length-1
            int ran = UnityEngine.Random.Range(0, planets.Length);
            GameObject prefab = planets[ran];
            Transform t = new GameObject().transform;
            t.position += Vector3.up * UnityEngine.Random.Range(10.0f, 200.0f);
            t.position += Vector3.right * UnityEngine.Random.Range(-200.0f, 200.0f);
            t.position += Vector3.forward * UnityEngine.Random.Range(-200.0f, 200.0f);
            realObjs[i] = (GameObject) Instantiate(prefab, t.position, t.rotation);
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

    void recieveBeat() {
        hasBeat = true;
    }
}
}
