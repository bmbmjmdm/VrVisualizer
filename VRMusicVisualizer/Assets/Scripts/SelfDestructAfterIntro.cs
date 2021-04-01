using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
public class SelfDestructAfterIntro : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        if (IntroProgress.completedIntro) Destroy(this.gameObject);
    }
}
}