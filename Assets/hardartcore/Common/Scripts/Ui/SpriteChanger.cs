using UnityEngine;
using UnityEngine.UI;

namespace hardartcore.Common.Scripts.Ui {
    public class SpriteChanger : MonoBehaviour {
        public Slider slider;
        public Sprite enabledSprite;
        public Sprite disabledSprite;

        private Image _image;

        public void Awake () {
            _image = GetComponent<Image> ();
            slider.wholeNumbers = true;
        }

        private void OnEnable () {
            slider.onValueChanged.AddListener (delegate { ChangeSprite (); });
        }

        private void OnDisable () {
            slider.onValueChanged.RemoveAllListeners ();
        }

        private void Start () {
            // Init based on Slider's value
            ChangeSprite ();
        }

        public void ChangeSprite () {
            if (slider.value == slider.minValue) {
                _image.sprite = disabledSprite;
            } else {
                _image.sprite = enabledSprite;
            }
        }

    }
}