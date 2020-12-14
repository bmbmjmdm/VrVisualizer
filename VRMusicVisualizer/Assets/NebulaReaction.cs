using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

namespace Assets.Scripts
{
public class NebulaReaction : MonoBehaviour
{
    public GameObject[] nebuli;
    public int numNebuli;
    public float percentChange;
    private GameObject[] realObjs;
    private bool hasBeat = false;
    public bool active = true;
    private int sizeRange;
    private int changeSetLeftBound;
    private int changeSetRightBound;
    private float timeSinceChangeBounds = 999f;
    private float changeBoundsEvery = 10f;
    private float fadeOutClock = 0f;
    private Vector3[] originalScales = new Vector3[1];
    private bool destroyed = false;

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
        realObjs = new GameObject[numNebuli];
        InitializePool();
        for (int i = 0; i < numNebuli; i++) {
            // exclusive so dont have to do Length-1
            int ran = UnityEngine.Random.Range(0, nebuli.Length);
            GameObject prefab = nebuli[ran];
            Vector3 t = new Vector3();
            Quaternion q = new Quaternion(
                Quaternion.identity.x,
                Quaternion.identity.y,
                Quaternion.identity.z,
                Quaternion.identity.w
            );
            q.eulerAngles = new Vector3(
                UnityEngine.Random.Range(0.0f, 360.0f),
                UnityEngine.Random.Range(0.0f, 360.0f),
                UnityEngine.Random.Range(0.0f, 360.0f)
            );
            // create nebuli all around the player, randomly between -300 and 300 on every axis
            do {
                t += Vector3.up * UnityEngine.Random.Range(-300.0f, 300.0f);
                t += Vector3.right * UnityEngine.Random.Range(-300.0f, 300.0f);
                t += Vector3.forward * UnityEngine.Random.Range(-300.0f, 300.0f);
            }
            // however dont let them spawn too close to the player
            while (Utilities.isNearPlayer(t));
            realObjs[i] = (GameObject) Instantiate(prefab, t, q);
            // if we're starting small, set the size to 0 so we can fade in
            if (small){
                realObjs[i].transform.localScale = new Vector3(0,0,0);
            }
            AddToPool(realObjs[i], ran);
        }
        sizeRange = (int) Math.Floor(percentChange * realObjs.Length / 100); 
        originalScales[0] = nebuli[0].transform.localScale;
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
                    DestroyPool();
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
            if (hasBeat) {
                int ran = UnityEngine.Random.Range(0, nebuli.Length);
                // go through change set and activate crystals of the new color for all of them
                for (int i = changeSetLeftBound; i < changeSetRightBound; i++) {
                    Vector3 position = realObjs[i].transform.position;
                    Quaternion rotation = realObjs[i].transform.rotation;
                    realObjs[i].SetActive(false);
                    GameObject newNeb = GetNext(ran);
                    realObjs[i] = newNeb;
                    realObjs[i].transform.position = position;
                    realObjs[i].transform.rotation = rotation;
                }
                hasBeat = false;
            }
        }
    }

    

    void recieveBeat() {
        hasBeat = true;
    }

    void toggleActive() {
        active = !active;
    }

    


// ================================================================ object pool logic =======================================

    // this simply resets all the lists for our pool
    void InitializePool() {
        // look through all current instances of the given color
        objectPool = new List<GameObject>[nebuli.Length];
        for (int color = 0; color < nebuli.Length; color++) {
            objectPool[color] = new List<GameObject>();
        }
    }

    void AddToPool(GameObject obj, int color) {
        List<GameObject> colorList = objectPool[color];
        colorList.Add(obj);
    }

    // go through all objects in our object pool and destroy any that were missed by fadeOutObject
    void DestroyPool() {
        // look through all colors lists
        for (int color = 0; color < nebuli.Length; color++) {
            List<GameObject> colorList = objectPool[color];
            // look at all objs in our current list
            for (int i = 0; i < colorList.Count; i++) {
                GameObject obj = colorList[i];
                // if the current stage isn't destroyed, destroy it
                if (obj != null) {
                    GameObject.Destroy(obj);
                }
            }
        }
        objectPool = new List<GameObject>[0];
    }

    // An array of Lists of gameobjects. 
    // The initial array index corresponds to the crystal color
    // Then the List index corresponds to the gameobject instance
    private List<GameObject>[] objectPool;

    // it's assumed that the previous stage has been deactivated and the nextStage has been verified <=2
    GameObject GetNext(int color) {
        List<GameObject> colorList = objectPool[color];
        // look through all current instances of the given color
        for (int i = 0; i < colorList.Count; i++) {
            if (!colorList[i].activeInHierarchy) {
                // looks like we found an inactive one! lets make it active and return it so the caller can move its position
                colorList[i].SetActive(true);
                return colorList[i];
            }
        }
        // uh oh, we got through all our colors and couldn't find a usable stage instance. let's add a new one
        Vector3 t = new Vector3();
        GameObject newObj = (GameObject) Instantiate(nebuli[color], t, Quaternion.identity);
        colorList.Add(newObj);
        newObj.SetActive(true);
        return newObj;
    }
}
}
