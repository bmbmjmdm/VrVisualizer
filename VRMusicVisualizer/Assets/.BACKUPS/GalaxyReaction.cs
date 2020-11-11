using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

namespace Assets.Scripts
{
public class GalaxyReaction : MonoBehaviour
{
    public GameObject galaxy;
    private GameObject realObj;
    private Vector3 onBeatVector = new Vector3(2f, 2f, 2f);
    private Vector3 offBeatVector = new Vector3(-0.016f, -0.016f, -0.016f);
    private int numBeat = 0;
    float speed = 30f;
    Vector3 direction = Vector3.up;

    // Start is called before the first frame update
    void Start()
    {
        BeatCollector.registerListener(recieveBeat);
        // exclusive so dont have to do Length-1
        Transform t = new GameObject().transform;
        t.position += Vector3.up * 200;
        t.position += Vector3.right * UnityEngine.Random.Range(-200.0f, -200.0f);
        t.position += Vector3.forward * UnityEngine.Random.Range(-200.0f, -200.0f);
        realObj = (GameObject) Instantiate(galaxy, t.position, t.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        if (numBeat > 0) {
            numBeat = 0;
            direction = direction == Vector3.up ? Vector3.down : Vector3.up;
        }
        else {
            realObj.transform.Rotate(direction * speed * Time.deltaTime);
        }
    }

    void recieveBeat() {
        numBeat++;
    }
}
}
