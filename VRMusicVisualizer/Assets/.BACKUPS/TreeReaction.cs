using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

namespace Assets.Scripts
{
public class TreeReaction : MonoBehaviour
{
    public GameObject[] trees;
    public int numTrees;
    public float percentChange;
    public int treeForce = 2;
    public int treeTorque = 90;
    private GameObject[] realObjs;
    private bool hasBeat = false;
    private int sizeRange;
    private int changeSetLeftBound;
    private int changeSetRightBound;
    private float timeSinceChangeBounds = 999f;
    private float changeBoundsEvery = 7f;
    private float clock = 0f;

    // Start is called before the first frame update
    void Start()
    {
        BeatCollector.registerListener(recieveBeat);
        realObjs = new GameObject[numTrees];
        for (int i = 0; i < numTrees; i++) {
            // exclusive so dont have to do Length-1
            int ran = UnityEngine.Random.Range(0, trees.Length);
            GameObject prefab = trees[ran];
            Transform t = new GameObject().transform;
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
            // WARNING this can pop trees up as much as it wants, so this depends on the delay from the beat collector
            for (int i = changeSetLeftBound; i < changeSetRightBound; i++) {
                //if (realObjs[i].transform.position.y <= 4) {
                    Rigidbody rigBod = realObjs[i].GetComponent<Rigidbody>();
                    rigBod.AddForce(Vector3.up * treeForce, ForceMode.VelocityChange);
                    rigBod.AddTorque(Vector3.up * treeTorque, ForceMode.VelocityChange);
                //}
            }
            hasBeat = false;
        }
    }

    void recieveBeat() {
        hasBeat = true;
    }
}
}
