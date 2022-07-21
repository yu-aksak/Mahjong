using UnityEngine;

namespace hardartcore.Scripts.Utils {

    public class SimpleItemChanger : MonoBehaviour {

        public GameObject[] items;

        private int _selectedItemIndex = 0;

        private void Start () {
            items[_selectedItemIndex].SetActive (true);
        }

        public void ChangeItem (int direction) {
            var index = direction + _selectedItemIndex;
            if (index < 0) {
                index = items.Length - 1;
            } else if (index > items.Length - 1) {
                index = 0;
            }
            items[_selectedItemIndex].SetActive (false);
            items[index].SetActive (true);
            _selectedItemIndex = index;
        }

    }
}