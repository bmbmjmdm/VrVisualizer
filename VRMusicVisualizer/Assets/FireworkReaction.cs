using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

namespace Assets.Scripts
{
public class FireworkReaction : MonoBehaviour
{
    public GameObject[] fireworks;
    public int numFireworks;
    private bool hasBeat = false;
    private bool active = true;

    // Start is called before the first frame update
    void Start()
    {
        BeatCollector.registerBeatListener(recieveBeat);
        BeatCollector.registerVerseListener(toggleActive);
    }

    // Update is called once per frame
    void Update()
    {
        if (hasBeat && active) {
            for (int i = 0; i < numFireworks; i++) {
                int ranObj = UnityEngine.Random.Range(0, fireworks.Length);
                GameObject prefab = fireworks[ranObj];
                Vector3 position = new Vector3(0,0,0);
                position += Vector3.up * UnityEngine.Random.Range(25, 125);
                position += Vector3.right * UnityEngine.Random.Range(-200, 200);
                position += Vector3.forward * UnityEngine.Random.Range(-200, 200);
                Instantiate(prefab, position, Quaternion.identity);
            }
            hasBeat = false;
        }
    }

    void recieveBeat() {
        hasBeat = true;
    }

    void toggleActive() {
        active = !active;
    }
}
}
