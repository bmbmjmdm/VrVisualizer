using Assets.WasapiAudio.Scripts.Unity;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

namespace Assets.Scripts
{
    public class VerseChange : AudioVisualizationEffect
    {
        
        private ArrayList currentVerse = new ArrayList();
        private List<float>[] newVerse = new List<float>[100];
        private int newVerseIndex = -1;
        private float clock = 0f;
        private float clock2 = 0f;
        public bool checkVerse = false;

        public void Awake () {
            BeatCollector.resetEvents();
        }
        public void Start()
        {
            // this turns on 1 effect at the start of the program
            if (checkVerse) {
                BeatCollector.verseChange = true;
            }
        }

        public void Update()
        {
            var spectrum = GetSpectrumData().ToList();

            bool verseChange = false;
            if (checkVerse) {
                verseChange = isVerseChange(spectrum);
                clock2 += Time.deltaTime;
            }
            bool songChange = isSongChange(spectrum);
            clock += Time.deltaTime;

            // don't let song change more than once every 30 seconds
            if (songChange && clock > 30f) {
                clock = 0f;
                BeatCollector.songChange();
                if (checkVerse) {
                    // for animation reasons, dont let verse change if it changed too recently
                    if (clock2 > 5f) {
                        BeatCollector.verseChange = true;
                        clock2 = 0f;
                    }
                    // reset verse info
                    currentVerse = new ArrayList();
                    newVerseIndex = -1;
                }
            }
            else if (verseChange) {
                clock2 = 0f;
                BeatCollector.verseChange = true;
            }
        }

        private bool isVerseChange (List<float> spectrum) {
            // we're looking at a possible new verse
            if (newVerseIndex > -1) {
                // record data until we have ~1 second worth
                if (newVerseIndex < 100) {
                    newVerse[newVerseIndex] = spectrum;
                    newVerseIndex ++;
                }
                // we have ~1 second worth, analyze
                else {
                    // find the average amplitude of our current verse
                    float currentLowAv = 0;
                    float currentMidAv = 0;
                    float currentHighAv = 0;
                    float currentAllAv = 0;
                    for (int i = 0; i < currentVerse.Count; i++) {
                        currentLowAv += getLowAv((List<float>) currentVerse[i]);
                        currentMidAv += getMidAv((List<float>) currentVerse[i]);
                        currentHighAv += getHighAv((List<float>) currentVerse[i]);
                        currentAllAv += getAllAv((List<float>) currentVerse[i]);
                    }
                    currentLowAv = currentLowAv / currentVerse.Count;
                    currentMidAv = currentMidAv / currentVerse.Count;
                    currentHighAv = currentHighAv / currentVerse.Count;
                    currentAllAv = currentAllAv / currentVerse.Count;

                    // find the average amplitude of our possible new verse
                    float newLowAv = 0;
                    float newMidAv = 0;
                    float newHighAv = 0;
                    float newAllAv = 0;
                    for (int i = 0; i < 100; i++) {
                        newLowAv += getLowAv(newVerse[i]);
                        newMidAv += getMidAv(newVerse[i]);
                        newHighAv += getHighAv(newVerse[i]);
                        newAllAv += getAllAv(newVerse[i]);
                    }
                    newLowAv = newLowAv / 100;
                    newMidAv = newMidAv / 100;
                    newHighAv = newHighAv / 100;
                    newAllAv = newAllAv / 100;

                    // compare the two averages to see if they're 20% different
                    float percentDifLow = newLowAv / currentLowAv;
                    bool percentDifLowSig = percentDifLow > 1.35f || percentDifLow < 0.75f;
                    float percentDifMid = newMidAv / currentMidAv;
                    bool percentDifMidSig = percentDifMid > 1.35f || percentDifMid < 0.75f;
                    float percentDifHigh = newHighAv / currentHighAv;
                    bool percentDifHighSig = percentDifHigh > 1.35f || percentDifHigh < 0.75f;
                    float percentDifAll = newAllAv / currentAllAv;
                    bool percentDifAllSig = percentDifAll > 1.25f || percentDifAll < 0.85f;
                    // verse change!
                    if (percentDifAllSig && (percentDifLowSig || percentDifMidSig || percentDifHighSig)) {
                        // reset everything and return true
                        currentVerse = new ArrayList();
                        newVerseIndex = -1;
                        return true;
                    }
                    // no verse change
                    else {
                        // keep looking for a new verse
                        newVerseIndex = 0;
                    }

                }
            }
            
            // we changed verses recently, collect current verse data
            else {
                currentVerse.Add(spectrum);
                // we have ~5 seconds of data, start looking for new verses
                if (currentVerse.Count >= 500) {
                    newVerseIndex = 0;
                }
            }

            return false;
        }

        // a song change occurs when the spectrum is near-0 all around
        private bool isSongChange (List<float> spectrum) {
            return getAllAv(spectrum) < 0.02f
                    && getHighAv(spectrum) < 0.02f
                    && getLowAv(spectrum) < 0.02f
                    && getMidAv(spectrum) < 0.02f;
        }

        private float getLowAv (List<float> spectrum) {
            // low frequency zone
            float lowAv = 0;
            for (int i = 0; i <= 8; i++) {
                lowAv += spectrum[i];
            }
            lowAv = lowAv/9;
            return lowAv;
        }

        private float getMidAv (List<float> spectrum) {
            // mid frequency zone
            float midAv = 0;
            for (int i = 9; i <= 143; i++) {
                midAv += spectrum[i];
            }
            midAv = midAv/135;
            return midAv;
        }

        private float getHighAv (List<float> spectrum) {
            // high frequency zone
            float highAv = 0;
            for (int i = 144; i <= 1023; i++) {
                highAv += spectrum[i];
            }
            highAv = highAv/880;
            return highAv;
        }

        private float getAllAv (List<float> spectrum) {
            // all frequency zone
            float allAv = 0;
            for (int i = 0; i <= 1023; i++) {
                allAv += spectrum[i];
            }
            allAv = allAv/1024;
            return allAv;
        }

        private float getAllWeightedAv (List<float> spectrum) {
            float allAv = getLowAv(spectrum) + getMidAv(spectrum) + getHighAv(spectrum);
            return allAv / 3;
        }

    }
}
