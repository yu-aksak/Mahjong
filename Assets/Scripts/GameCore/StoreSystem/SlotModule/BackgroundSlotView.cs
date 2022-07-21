using UnityEngine;
using UnityEngine.UI;

namespace Core.StoreModule
{
    public class BackgroundSlotView : SelectableSlotView<BackgroundSlotView>
    {
        protected override ItemType Type => ItemType.Background;
        private Color defaultColor;
        private Image image;
        protected override void OnCreate()
        {
            base.OnCreate();
            image = gameObject.GetComponent<Image>();
            defaultColor = image.color;
        }

        public override void OnSelect()
        {
            base.OnSelect();
            image.color = StoreWindow.SelectedColor;
        }

        protected override void OnOtherItemSelected(in int index)
        {
            base.OnOtherItemSelected(index);

            if (this.index != index)
            {
                image.color = defaultColor;
            }
        }
    }
}
