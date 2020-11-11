using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 
using UnityEngine.XR;

namespace Assets.Scripts
{
public class AsteroidReaction : MonoBehaviour
{
    public GameObject[] asteroids;
    public GameObject player;
    public int numAsteroids;
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
            // this gives you a new "forward" vector based on where the user is looking
            Quaternion playerDirection = InputTracking.GetLocalRotation(XRNode.CenterEye);
            playerDirection = new Quaternion(0, playerDirection.y, playerDirection.z, playerDirection.w);
            playerDirection = playerDirection * player.transform.rotation;
            Vector3 fwd = playerDirection * Vector3.forward;
            for (int i = 0; i < numAsteroids; i++) {
                // select the direction the asteroid's flying in so we can make each group fly together
                int x = 0;
                int y = 0;
                int z = 0;
                while (x == 0 && z == 0 && y == 0) {
                    x = UnityEngine.Random.Range(-1, 2);
                    z = UnityEngine.Random.Range(-1, 2);
                    y = UnityEngine.Random.Range(-1, 2);
                }
                // select the relative location of the group so they spawn together
                float ranDistance = UnityEngine.Random.Range(1.0f, 40.0f);
                // spawn a group of 3-7
                for (int ii = 0; ii < UnityEngine.Random.Range(3, 7); ii++) {
                    // exclusive so dont have to do Length-1
                    int ranObj = UnityEngine.Random.Range(0, asteroids.Length);
                    GameObject prefab = asteroids[ranObj];
                    // This places a random point directly in front of the user, up to 20 units away
                    // It then finds another point within (+-2,+-2,+-2) of that first point 
                    // It spawns an asteroid at that second point, where it flies in asteroid_movement.cs
                    Vector3 pointAlongVisionLine = player.transform.position + fwd * ranDistance;
                    pointAlongVisionLine += Vector3.up * UnityEngine.Random.Range(-ranDistance/2, ranDistance/2);
                    pointAlongVisionLine += Vector3.right * UnityEngine.Random.Range(-ranDistance/2, ranDistance/2);
                    pointAlongVisionLine += Vector3.forward * UnityEngine.Random.Range(-ranDistance/2, ranDistance/2);
                    GameObject newMeteor = Instantiate(prefab, pointAlongVisionLine, Quaternion.identity);
                    // fly in a random direction
                    newMeteor.GetComponent<asteroid_movement>().setDirection(x, y, z);
                }
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
