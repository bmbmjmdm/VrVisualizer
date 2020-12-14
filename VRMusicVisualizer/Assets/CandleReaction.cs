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
    // NEVER START DESTROYED AS FALSE HERE or else spawnCandles will be called with null objects
    private bool destroyed = true;

    // Start is called before the first frame update
    void Start()
    {
        BeatCollector.registerVerseListener(toggleActive);
    }

    // Update is called once per frame
    void Update()
    {
        // if we're not active, destroy all objects
        if (!active) {
            if (!destroyed) {
                spawnCandles(-1f);
                destroyed = true;
                DestroyObjs();
            }
        }
        // we're active
        else {
            if (destroyed) {
                CreateObjs();
            }
            destroyed = false;
            clock += Time.deltaTime;
            
            // dont change num candles if it only changed by a tiny tiny bit. this gets rid of jitters 
            if (clock >= 0.02 && BeatCollector.highSig) {
                clock = 0f;
                spawnCandles(BeatCollector.getHighPer());
            }
        }
    }

    void CreateObjs () {
        realObjs = new GameObject[numCandles];
        // goes through entire array and fill it with random prefabs/inactive objects
        for (int i = 0; i < numCandles; i++) {
            int candleIndex = UnityEngine.Random.Range(0, candles.Length);
            realObjs[i] = (GameObject) Instantiate(candles[candleIndex], new Vector3(), Quaternion.identity);
            realObjs[i].SetActive(false);
        }
    }

    void DestroyObjs () {
        // goes through entire array and destroy the objects
        for (int i = 0; i < numCandles; i++) {
            GameObject.Destroy(realObjs[i]);
        }
        realObjs = new GameObject[0];
    }

    // spawns percent of numCandles below the user. if this percent is lower than the previous one, it unspawns candles
    void spawnCandles(float percent) {
        // goes through entire array
        for (int i = 0; i < numCandles; i++) {
            bool shouldHide = (i/(float) numCandles) > percent;
            // inactivate all excess candles
            if (shouldHide) {
                if (realObjs[i].activeInHierarchy) {
                    realObjs[i].SetActive(false);
                }
            }
            // activate all necessary candles at random locations
            else {
                if (!realObjs[i].activeInHierarchy) {
                    Vector3 t = new Vector3();
                    t += Vector3.up * UnityEngine.Random.Range(-100.0f, -10.0f);
                    t += Vector3.right * UnityEngine.Random.Range(-100.0f, 100.0f);
                    t += Vector3.forward * UnityEngine.Random.Range(-100.0f, 100.0f);
                    realObjs[i].transform.position = t;
                    realObjs[i].SetActive(true);
                }
            }
        }
    }

    void toggleActive() {
        active = !active;
    }
}
}
