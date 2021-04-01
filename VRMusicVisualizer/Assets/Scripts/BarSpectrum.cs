using Assets.WasapiAudio.Scripts.Unity;
using UnityEngine;

namespace Assets.Scripts
{
    public class BarSpectrum : AudioVisualizationEffect
    {
        private GameObject[] _spectrumBars;
        private Vector3[] _originalPositions;
        private Vector3 _originalScale;
        private float width;

        public GameObject Prefab;
        public float AudioScale;
        public float Power;
        public bool active = true;

        public void Start()
        {
            BeatCollector.registerVerseListener(toggleActive);

            _spectrumBars = new GameObject[SpectrumSize];
            _originalPositions = new Vector3[SpectrumSize];
            _originalScale = Prefab.transform.localScale;

            width = Prefab.transform.localScale.x;
            Prefab.SetActive(false);

             if (active) {
                createObjs();
             }
        }

        void createObjs() {
            Prefab.SetActive(true);

            for (var i = 0; i < SpectrumSize; i++)
            {
                var spectrumBar = GameObject.Instantiate(Prefab);
                spectrumBar.transform.parent = transform;
                spectrumBar.transform.localPosition = new Vector3(width * i, 0.0f, 0.0f);
                _spectrumBars[i] = spectrumBar;
                _originalPositions[i] = spectrumBar.transform.localPosition;
            }

            Prefab.SetActive(false);
        }

        void destroyObjs() {
            for (var i = 0; i < SpectrumSize; i++)
            {
                GameObject.Destroy(_spectrumBars[i]);
            }
            _spectrumBars = new GameObject[SpectrumSize];
            _originalPositions = new Vector3[SpectrumSize];
        }

        public void Update()
        {
            if (!active) return;
            var spectrumData = GetSpectrumData();

            for (var i = 0; i < SpectrumSize; i++)
            {
                var audioScale = Mathf.Pow(spectrumData[i] * AudioScale, Power);
                var newScale = new Vector3(_originalScale.x, _originalScale.y + audioScale, _originalScale.z);
                var halfScale = newScale / 2.0f;
                _spectrumBars[i].transform.localPosition = new Vector3(_originalPositions[i].x + halfScale.x, _originalPositions[i].y + halfScale.y, _originalPositions[i].z + halfScale.z);
                _spectrumBars[i].transform.localScale = newScale;
            }
        }

        void toggleActive() {
            active = !active;
            if (active) {
                createObjs();
            }
            else {
                destroyObjs();
            }
        }
    }
}
