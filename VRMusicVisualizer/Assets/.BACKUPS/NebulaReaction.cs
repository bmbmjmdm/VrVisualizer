using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

namespace Assets.Scripts
{
public class NebulaReaction : MonoBehaviour
{
    public GameObject[] nebuli;
    private GameObject realObj;
    private int numBeat = 0;

    // Start is called before the first frame update
    void Start()
    {
        BeatCollector.registerListener(recieveBeat);
        // exclusive so dont have to do Length-1
        int ran = UnityEngine.Random.Range(0, nebuli.Length);
        GameObject prefab = nebuli[ran];
        Transform t = new GameObject().transform;
        realObj = (GameObject) Instantiate(prefab, t.position, t.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        if (numBeat > 10) {
            numBeat = 0;
            Destroy(realObj.gameObject);
            // exclusive so dont have to do Length-1
            int ran = UnityEngine.Random.Range(0, nebuli.Length);
            GameObject prefab = nebuli[ran];
            Transform t = new GameObject().transform;
            realObj = (GameObject) Instantiate(prefab, t.position, t.rotation);
        }
    }

    void recieveBeat() {
        numBeat++;
    }
}
}
