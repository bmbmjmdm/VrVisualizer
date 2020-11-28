using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 
using UnityEngine.XR;

namespace Assets.Scripts
{
public class LaserReaction : MonoBehaviour
{
    public GameObject[] lasers;
    public GameObject player;
    public int numLasers;
    private bool hasBeat = false;
    public bool active = true;

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
            // this gives you a new "forward" vector based on where the user is looking
            Quaternion playerDirection = InputTracking.GetLocalRotation(XRNode.CenterEye);
            playerDirection = new Quaternion(0, playerDirection.y, playerDirection.z, playerDirection.w);
            playerDirection = playerDirection * player.transform.rotation;
            Vector3 fwd = playerDirection * Vector3.forward;
            for (int i = 0; i < numLasers; i++) {
                // select the direction the lasers's flying in so we can make each group fly together
                int x = 0;
                int y = 0;
                int z = 0;
                while (x == 0 && z == 0 && y == 0) {
                    x = UnityEngine.Random.Range(-1, 2);
                    z = UnityEngine.Random.Range(-1, 2);
                    y = UnityEngine.Random.Range(-1, 2);
                }
                // This places a random point directly in front of the user, up to 30 units away
                float ranDistance = UnityEngine.Random.Range(5.0f, 30.0f);
                // It then finds another point within (+-30,+-30,+-30) of that first point 
                Vector3 pointAlongVisionLine = player.transform.position + fwd * ranDistance;
                pointAlongVisionLine += Vector3.up * UnityEngine.Random.Range(-ranDistance, ranDistance);
                pointAlongVisionLine += Vector3.right * UnityEngine.Random.Range(-ranDistance, ranDistance);
                pointAlongVisionLine += Vector3.forward * UnityEngine.Random.Range(-ranDistance, ranDistance);
                // Spawns a laser at that second point, where it flies in laser_movement.cs
                int ranObj = UnityEngine.Random.Range(0, lasers.Length);
                GameObject prefab = lasers[ranObj];
                GameObject newLaser = Instantiate(prefab, pointAlongVisionLine, Quaternion.identity);
                // fly in a random direction
                newLaser.GetComponent<laser_movement>().setDirection(x, y, z);
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
