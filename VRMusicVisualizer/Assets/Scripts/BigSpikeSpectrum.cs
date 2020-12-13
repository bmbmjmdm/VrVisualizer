using Assets.WasapiAudio.Scripts.Unity;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts
{
    public class BigSpikeSpectrum : AudioVisualizationEffect
    {

        public GameObject Prefab;
        // increases the threshold for a frequency to be considered a beat
        public float thresholdMultiplier = 1;
        // the size of the window (in frames) we look at to determine average frequency amplitude when determining if a given frequency is significant (a beat)
        public int thresholdWindowSize = 5;
        private float[][] spectrums = new float[1024][];

        public void Start()
        {
            for (int i = 0; i < 1024; i++) {
                spectrums[i] = new float[thresholdWindowSize];
                for (int r = 0; r < thresholdWindowSize; r++) {
                    spectrums[i][r] = 0;
                }
            }
        }

        public void Update()
        {
            bool peaked = false;
            var curSpec = GetSpectrumData();
            // Process each frequency bucket
            for (int i = 0; i < 1024; i++) {
                if(updateSpectrum(i, curSpec)) peaked = true;
            }

            if (peaked) {
                BeatCollector.spikeBeat = true;
            }
        }

        public bool updateSpectrum(int index, float[] curSpec) {
            for (int i = thresholdWindowSize - 1; i > 0; i--) {
                spectrums[index][i] = spectrums[index][i-1];
            }
            spectrums[index][0] = curSpec[index];
            bool more = true;
            bool peak = false;
            for (int i = 1; i < thresholdWindowSize; i++) {
                if (spectrums[index][0] < spectrums[index][i]) more = false;
                if (spectrums[index][0] > spectrums[index][i]*10) peak = true;
            }
            return more && peak;
        }
    }
}
