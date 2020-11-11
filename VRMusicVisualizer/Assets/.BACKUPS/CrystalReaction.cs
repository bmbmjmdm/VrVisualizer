using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

namespace Assets.Scripts
{
public class CrystalReaction : MonoBehaviour
{
    public GameObject[] crystals;
    public int numCrystals;
    public float percentChange;
    private GameObject[] realObjs;
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
        realObjs = new GameObject[numCrystals];
        for (int i = 0; i < numCrystals; i++) {
            // exclusive so dont have to do Length-1
            int ran = UnityEngine.Random.Range(0, crystals.Length);
            GameObject prefab = crystals[ran];
            Transform t = new GameObject().transform;
            t.position = new Vector3(0f, 0f, 0f);
            t.position += Vector3.right * UnityEngine.Random.Range(-150.0f, 150.0f);
            t.position += Vector3.forward * UnityEngine.Random.Range(-150.0f, 150.0f);
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
            int ran = UnityEngine.Random.Range(0, crystals.Length);
            GameObject prefab = crystals[ran];
            for (int i = changeSetLeftBound; i < changeSetRightBound; i++) {
                Transform t = realObjs[i].transform;
                Destroy(realObjs[i]);
                realObjs[i] = (GameObject) Instantiate(prefab, t.position, t.rotation);
            }
            hasBeat = false;
        }
    }

    void recieveBeat() {
        hasBeat = true;
    }
}
}
