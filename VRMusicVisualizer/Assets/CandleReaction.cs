using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

namespace Assets.Scripts
{
public class CandleReaction : MonoBehaviour
{
    public GameObject[] candles;
    public int numCandles;
    private GameObject[] realObjs;
    public bool active = true;
    private float clock = 0f;
    private bool destroyed = true;

    // Start is called before the first frame update
    void Start()
    {
        BeatCollector.registerVerseListener(toggleActive);
        realObjs = new GameObject[numCandles];
    }

    // Update is called once per frame
    void Update()
    {
        // if we're not active, destroy all objects
        if (!active) {
            if (!destroyed) {
                spawnCandles(-1f);
                destroyed = true;
            }
        }
        // we're active
        else {
            destroyed = false;
            clock += Time.deltaTime;
            
            // dont change num candles if it only changed by a tiny tiny bit. this gets rid of jitters 
            if (clock >= 0.02 && BeatCollector.highSig) {
                clock = 0f;
                spawnCandles(BeatCollector.getHighPer());
            }
        }
    }

    // spawns percent of numCandles below the user. if this percent is lower than the previous one, it unspawns candles
    void spawnCandles(float percent) {
        // goes through entire array
        for (int i = 0; i < numCandles; i++) {
            bool shouldHide = (i/(float) numCandles) > percent;
            // destroy all excess candles
            if (shouldHide) {
                if (realObjs[i] != null) {
                    GameObject.Destroy(realObjs[i]);
                }
            }
            // spawn all necessary candles 
            else {
                if (realObjs[i] == null) {
                    int candleIndex = UnityEngine.Random.Range(0, candles.Length);
                    Transform t = new GameObject().transform;
                    t.position += Vector3.up * UnityEngine.Random.Range(-100.0f, -10.0f);
                    t.position += Vector3.right * UnityEngine.Random.Range(-50.0f, 50.0f);
                    t.position += Vector3.forward * UnityEngine.Random.Range(-50.0f, 50.0f);
                    realObjs[i] = (GameObject) Instantiate(candles[candleIndex], t.position, t.rotation);
                }
            }
        }
    }

    void toggleActive() {
        active = !active;
    }
}
}
