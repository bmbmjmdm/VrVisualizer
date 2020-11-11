using Assets.WasapiAudio.Scripts.Unity;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Assets.Scripts
{
    public class ContinuousSpectrum : AudioVisualizationEffect
    {
    private float clock = 0f;
    private float clock2 = 0f;

        public void Start()
        {
        }

        public void Update()
        {
            // TODO we might want to normalize this list for 2 reasons:
            // 1. high spectrums tend to be lower amplitude than low spectrum
            // 2. high spectrum has higher # to divide by, which can make them seem even smaller 
            var spectrum = GetSpectrumData().ToList();
            // low frequency zone
            float lowAv = 0;
            for (int i = 0; i <= 8; i++) {
                lowAv += spectrum[i];
            }
            lowAv = lowAv/9;
            // mid frequency zone
            float midAv = 0;
            for (int i = 9; i <= 143; i++) {
                midAv += spectrum[i];
            }
            midAv = midAv/135;
            // high frequency zone
            float highAv = 0;
            for (int i = 144; i <= 1023; i++) {
                highAv += spectrum[i];
            }
            highAv = highAv/880;
            // all frequency zone
            float allAv = 0;
            for (int i = 0; i <= 1023; i++) {
                allAv += spectrum[i];
            }
            allAv = allAv/1024;

            BeatCollector.setLowAv(lowAv);
            BeatCollector.setMidAv(midAv);
            BeatCollector.setHighAv(highAv);
            BeatCollector.setAllAv(allAv);

            // every 15 seconds, make our average significance functions reset their dividor so that we never get too set in our ways of measuring static (aka will adapt to changing volume/songs)
            clock += Time.deltaTime;
            if (clock > 15f) {
                BeatCollector.resetAverageDifferences();
                clock = 0.0f;
            }

            // TODO replace this timer with the verse/song detection algorithm
            // resets max averages so we dont get too stale (adapt to song/volume change)
            clock2 += Time.deltaTime;
            if (clock2 > 30f) {
                BeatCollector.resetAverageMaxes();
                clock2 = 0.0f;
            }

        }

    }
}
