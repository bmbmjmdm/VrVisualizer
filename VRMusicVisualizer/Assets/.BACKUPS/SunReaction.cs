using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

namespace Assets.Scripts
{
public class SunReaction : MonoBehaviour
{
    public GameObject sun;
    private GameObject realObj;
    private Vector3 beatVector = new Vector3(0.15f, 0.15f, 0.15f);
    private bool minusVector = false;
    private bool hasBeat = false;
    private float clock = 0f;

    // Start is called before the first frame update
    void Start()
    {
        BeatCollector.registerListener(recieveBeat);
        Transform t = new GameObject().transform;
        t.position += Vector3.up * 200;
        t.position += Vector3.right * UnityEngine.Random.Range(200.0f, 200.0f);
        t.position += Vector3.forward * UnityEngine.Random.Range(200.0f, 200.0f);
        realObj = (GameObject) Instantiate(sun, t.position, t.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        clock += Time.deltaTime;
        if (hasBeat) {
            minusVector = !minusVector;
            hasBeat = false;
        }
        if (clock >= 0.02) {
            clock = 0f;
            // TODO this can cause runaway growth
            if (minusVector) {
                if (realObj.transform.localScale.x > 15) {
                    realObj.transform.localScale -= beatVector;
                }
            }
            else {
                if (realObj.transform.localScale.x < 200) {
                    realObj.transform.localScale += beatVector;
                }
            }
        }
    }

    void recieveBeat() {
        hasBeat = true;
    }
}
}
