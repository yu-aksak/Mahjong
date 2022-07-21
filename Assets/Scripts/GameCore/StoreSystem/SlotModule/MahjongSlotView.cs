namespace Core.StoreModule
{
    public class MahjongSlotView : SlotView<MahjongSlotView>
    {
        protected override ItemType Type => ItemType.Mahjong;

        public override void OnClick()
        {
            base.OnClick();
            LevelWindow.Show();
        }
    }
}
