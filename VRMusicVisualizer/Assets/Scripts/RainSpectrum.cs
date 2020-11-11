using Assets.WasapiAudio.Scripts.Unity;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Assets.Scripts
{
    public class RainSpectrum : AudioVisualizationEffect
    {

        public GameObject Prefab;

        public float AudioScale = 1.0f;
        public float Power = 1.0f;
        // increases the threshold for a frequency to be considered a beat
        public float thresholdMultiplier = 1;
        // the size of the window (in frames) we look at to determine average frequency amplitude when determining if a given frequency is significant (a beat)
        public int thresholdWindowSize = 30;

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

            if (peakedTotal) BeatCollector.rainBeatTotal = true;
            if (peakedBass)  BeatCollector.rainBeatBass = true;
            if (peakedMid)   BeatCollector.rainBeatMid = true;
            if (peakedTreb)  BeatCollector.rainBeatTreb = true;
        }

        public class SpectrumBucket {
            private List<float> curSpectrum = new List<float>();
            private List<float> prevSpectrum = new List<float>();
            private List<SpectralFluxInfo> spectralFluxSamples = new List<SpectralFluxInfo>();
            private int indexToProcess = 1;
            private int thresholdWindowSize;
            private float multiplier;
            // this is the frequency range used for this bucket
            private int lowerBound;
            private int upperBound;

            public SpectrumBucket (int size, float mult, int lowFreq, int highFreq) {
                thresholdWindowSize = size;
                lowerBound = lowFreq;
                upperBound = highFreq;
                multiplier = mult;
            }

            public bool Update(List<float> spectrum) {
                // Set spectrum
                setCurSpectrum(spectrum);
                // Get current spectral flux from spectrum
                SpectralFluxInfo curInfo = new SpectralFluxInfo();
                curInfo.spectralFlux = calculateRectifiedSpectralFlux();
                spectralFluxSamples.Add(curInfo);

                // We have enough samples to detect a peak
                if (spectralFluxSamples.Count >= thresholdWindowSize) {
                    // Get Flux threshold of time window surrounding index to process
                    spectralFluxSamples[indexToProcess].threshold = getFluxThreshold(indexToProcess);

                    // Only keep amp amount above threshold to allow peak filtering
                    spectralFluxSamples[indexToProcess].prunedSpectralFlux = getPrunedSpectralFlux(indexToProcess);

                    // Now that we are processed at n, n-1 has neighbors (n-2, n) to determine peak
                    int indexToDetectPeak = indexToProcess - 1;
                    indexToProcess++;

                    bool curPeak = isPeak(indexToDetectPeak);

                    if (curPeak) {
                        spectralFluxSamples[indexToDetectPeak].isPeak = true;
                        return true;
                    }
                }
                else {
                    //Debug.Log(string.Format("Not ready yet.  At spectral flux sample size of {0} growing to {1}", spectralFluxSamples.Count, thresholdWindowSize));
                }
                return false;
            }

            // keep history of spectrum data
            public void setCurSpectrum(List<float> spectrum) {
                prevSpectrum = new List<float>(curSpectrum);
                curSpectrum = new List<float>(spectrum);
            }

            // this sums the portion of the spectrum defined for this bucket to find the positive change of the current spectrum to the previous one
            float calculateRectifiedSpectralFlux() {
                float sum = 0f;
                if (prevSpectrum.Count() > 0) {
                    for (int i = lowerBound; i <= upperBound; i++) {
                        sum += Mathf.Max (0f, curSpectrum [i] - prevSpectrum [i]);
                    }
                }
                return sum;
            }

            // This tells us the threshold value that a calculateRectifiedSpectralFlux needs to surpass before it's considered significant (aka a beat)
            float getFluxThreshold(int spectralFluxIndex) {    
                // How many samples in the past and future we include in our average
                int windowStartIndex = (int) Mathf.Max (0, spectralFluxIndex - thresholdWindowSize / 2);
                int windowEndIndex = (int) Mathf.Min (spectralFluxSamples.Count - 1, spectralFluxIndex + thresholdWindowSize / 2);

                // Add up our spectral flux over the window
                float sum = 0f;
                for (int i = windowStartIndex; i < windowEndIndex; i++) {
                    sum += spectralFluxSamples[i].spectralFlux;
                }

                // Return the average multiplied by our sensitivity multiplier
                float avg = sum / (windowEndIndex - windowStartIndex);
                return avg * multiplier;
            }

            // this nullifies any spectral flux that is below our threshold
            float getPrunedSpectralFlux(int spectralFluxIndex) {
                return Mathf.Max (0f, spectralFluxSamples[spectralFluxIndex].spectralFlux - spectralFluxSamples[spectralFluxIndex].threshold);
            }
            
            // determines if a spectral flux is significant by comparing it to its immediate neighbors
            bool isPeak(int spectralFluxIndex) {
                if (spectralFluxSamples[spectralFluxIndex].prunedSpectralFlux > spectralFluxSamples[spectralFluxIndex + 1].prunedSpectralFlux &&
                    spectralFluxSamples[spectralFluxIndex].prunedSpectralFlux > spectralFluxSamples[spectralFluxIndex - 1].prunedSpectralFlux) {
                    return true;
                } else {
                    return false;
                }
            }


            public class SpectralFluxInfo {
                public float spectralFlux;
                public float threshold;
                public float prunedSpectralFlux;
                public bool isPeak;
            }
        }
    }
}
