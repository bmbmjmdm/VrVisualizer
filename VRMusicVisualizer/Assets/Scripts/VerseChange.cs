using Assets.WasapiAudio.Scripts.Unity;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Assets.Scripts
{
    public class VerseChange : AudioVisualizationEffect
    {
        private float clock = 0.0f;

        public void Start()
        {
        }

        public void Update()
        {
            clock += Time.deltaTime;
            if (clock >= 10) {
                BeatCollector.verseChange = true;
                clock = 0.0f;
            }
        }

    }
}
