using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 
using UnityEngine.XR;

namespace Assets.Scripts
{
    public class BirdReaction : MonoBehaviour
    {
        public GameObject[] birds;
        public GameObject player;
        public GameObject featherEffect;
        public int numBirds;
        private bool hasBeat = false;

        // Start is called before the first frame update
        void Start()
        {
            BeatCollector.registerListener(recieveBeat);
        }

        // Update is called once per frame
        void Update()
        {
            // this gives you a new "forward" vector based on where the user is looking
            Quaternion playerDirection = InputTracking.GetLocalRotation(XRNode.CenterEye);
            playerDirection = new Quaternion(0, playerDirection.y, playerDirection.z, playerDirection.w);
            playerDirection = playerDirection * player.transform.rotation;
            Vector3 fwd = playerDirection * Vector3.forward;
            if (hasBeat) {
                for (int i = 0; i < numBirds; i++) {
                    // exclusive so dont have to do Length-1
                    int ranObj = UnityEngine.Random.Range(0, birds.Length);
                    GameObject prefab = birds[ranObj];
                    // This places a random point directly in front of the user, up to 20 units away
                    // It then finds another point within (+-2,+-2,+-2) of that first point 
                    // It spawns a bird and its feathers at that second point, where it flies in lb.Bird.cs
                    float ranDistance = UnityEngine.Random.Range(1.0f, 20.0f);
                    Vector3 pointAlongVisionLine = player.transform.position + fwd * ranDistance;
                    pointAlongVisionLine += Vector3.up * UnityEngine.Random.Range(-ranDistance/2, ranDistance/2);
                    pointAlongVisionLine += Vector3.right * UnityEngine.Random.Range(-ranDistance/2, ranDistance/2);
                    pointAlongVisionLine += Vector3.forward * UnityEngine.Random.Range(-ranDistance/2, ranDistance/2);
                    Instantiate(prefab, pointAlongVisionLine, Quaternion.identity);
                    Instantiate(featherEffect, pointAlongVisionLine, Quaternion.identity);
                }
                hasBeat = false;
            }
        }

        void recieveBeat() {
            hasBeat = true;
        }

        void setTransformX(Transform t, float n){
            t.position = new Vector3(n, t.position.y, t.position.z);
        }

        void setTransformZ(Transform t, float n){
            t.position = new Vector3(t.position.x, t.position.y, n);
        }
    }
}
