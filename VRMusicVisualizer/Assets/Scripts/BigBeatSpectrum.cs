using Assets.WasapiAudio.Scripts.Unity;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts
{
    public class BigBeatSpectrum : AudioVisualizationEffect
    {

        public GameObject Prefab;
        public float AudioScale = 1.0f;
        public float Power = 1.0f;
        // increases the threshold for a frequency to be considered a beat
        public float thresholdMultiplier = 1;
        // the size of the window (in frames) we look at to determine average frequency amplitude when determining if a given frequency is significant (a beat)
        public int thresholdWindowSize = 5;

        private List<SpectrumBucket> buckets = new List<SpectrumBucket>();

        public void Start()
        {
            // order of buckets matters for splitting up bass, mid, treble, total
            buckets.Add(new SpectrumBucket(thresholdWindowSize, thresholdMultiplier, 0, 1));
            buckets.Add(new SpectrumBucket(thresholdWindowSize, thresholdMultiplier, 2, 3));
            buckets.Add(new SpectrumBucket(thresholdWindowSize, thresholdMultiplier, 4, 8));
            buckets.Add(new SpectrumBucket(thresholdWindowSize, thresholdMultiplier, 9, 17));
            buckets.Add(new SpectrumBucket(thresholdWindowSize, thresholdMultiplier, 18, 70));
            buckets.Add(new SpectrumBucket(thresholdWindowSize, thresholdMultiplier, 71, 143));
            buckets.Add(new SpectrumBucket(thresholdWindowSize, thresholdMultiplier, 144, 283));
            buckets.Add(new SpectrumBucket(thresholdWindowSize, thresholdMultiplier, 284, 567));
            buckets.Add(new SpectrumBucket(thresholdWindowSize, thresholdMultiplier, 568, 1023));
            buckets.Add(new SpectrumBucket(thresholdWindowSize, thresholdMultiplier, 0, 1023));
        }

        public void Update()
        {
            bool peakedBass = false;
            bool peakedMid = false;
            bool peakedTreb = false;
            bool peakedTotal = false;
            var spectrum = GetSpectrumData().ToList();
            // Adjust the spectrum as requested
            for (int i = 0; i < spectrum.Count; i++) {
                spectrum[i] = Mathf.Pow(spectrum[i] * AudioScale, Power);
            }
            // Process each frequency bucket
            for (int i = 0; i < buckets.Count; i++) {
                if (buckets[i].Update(spectrum)) {
                    if (i < 3) {
                        peakedBass = true;
                    }
                    else if (i < 6) {
                        peakedMid = true;
                    }
                    else if (i < 9) {
                        peakedTreb = true;
                    }
                    else {
                        peakedTotal = true;
                    }
                }
            }

            if (peakedTotal) BeatCollector.bigBeatTotal = true;
            if (peakedBass)  BeatCollector.bigBeatBass = true;
            if (peakedMid)   BeatCollector.bigBeatMid = true;
            if (peakedTreb)  BeatCollector.bigBeatTreb = true;
        }

        public class SpectrumBucket {
            private float[] spectrumTotals;
            private float multiplier;
            // this is the frequency range used for this bucket
            private int lowerBound;
            private int upperBound;

            public SpectrumBucket (int size, float mult, int lowFreq, int highFreq) {
                spectrumTotals = new float[size];
                for (int i = 0; i < spectrumTotals.Count(); i++) {
                    spectrumTotals[i] = 0;
                }
                lowerBound = lowFreq;
                upperBound = highFreq;
                multiplier = mult;
            }

            public bool Update(List<float> spectrum) {
                // Set spectrum
                updateSpectrum(spectrum);                

                // We have enough samples to detect a peak
                if (isPeak()) {
                    return true;
                }
                return false;
            }

            // keep history of spectrum data
            public void updateSpectrum(List<float> spectrum) {
                for (int i = spectrumTotals.Count() - 1; i > 0; i--) {
                    spectrumTotals[i] = spectrumTotals[i-1];
                }
                float total = 0f;
                for (int i = lowerBound; i <= upperBound; i++) {
                    total += spectrum[i];
                }
                spectrumTotals[0] = total;
            }

            public bool isPeak() {
                bool more = true;
                for (int i = 1; i < spectrumTotals.Count(); i++) {
                    if (spectrumTotals[0] <= spectrumTotals[i]) more = false;
                }
                return more;
            }
        }
    }
}
