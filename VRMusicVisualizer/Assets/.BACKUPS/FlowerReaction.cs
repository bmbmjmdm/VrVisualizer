using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

namespace Assets.Scripts
{
public class FlowerReaction : MonoBehaviour
{
    public GameObject[] flowers;
    public int numFlowers;
    public float percentChange;
    private GameObject[] realObjs;
    private Vector3 beatVectorGrow = new Vector3(0.03f, 0.03f, 0.03f);
    private Vector3 beatVectorNew = new Vector3(0f, 0f, 0f);
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
        realObjs = new GameObject[numFlowers];
        for (int i = 0; i < numFlowers; i++) {
            // exclusive so dont have to do Length-1
            int ran = UnityEngine.Random.Range(0, flowers.Length);
            GameObject prefab = flowers[ran];
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
            for (int i = changeSetLeftBound; i < changeSetRightBound; i++) {
                Vector3 temp = new Vector3(0f, 0f, 0f);
                temp += Vector3.right * UnityEngine.Random.Range(-125.0f, 125.0f);
                temp += Vector3.forward * UnityEngine.Random.Range(-125.0f, 125.0f);
                realObjs[i].transform.position = temp;
                realObjs[i].transform.localScale = beatVectorNew;
            }
            hasBeat = false;
        }
        else {
            if (clock >= 0.02) {
                clock = 0f;
                for (int i = 0; i < numFlowers; i++) {
                    if (realObjs[i].transform.localScale.x < 1) {
                        realObjs[i].transform.localScale += beatVectorGrow;
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
