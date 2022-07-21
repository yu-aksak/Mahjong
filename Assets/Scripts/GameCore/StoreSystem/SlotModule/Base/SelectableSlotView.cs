namespace Core.StoreModule
{
    public abstract class SelectableSlotView<T> : SlotView<T> where T : SelectableSlotView<T>, new()
    {
        protected override void OnOtherItemSelected(in int index)
        {
            base.OnOtherItemSelected(index);
            ItemSelector.RemoveListener(Type, OnOtherItemSelected);
        }

        public override void OnSelect()
        {
            base.OnSelect();
            ItemSelector.AddListener(Type, OnOtherItemSelected);
        }
    }
}