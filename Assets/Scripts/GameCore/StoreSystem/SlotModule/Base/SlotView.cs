using Core.Extensions.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.StoreModule
{
    public abstract class SlotView
    {
        protected const string StatesPath = "States";
        protected const string LockStatePath = "Lock";
        protected const string BuyStatePath = "NotBuy";
        protected const string PreviewImagePath = "LabelImage/Image";
        protected const string PriceTextPath = "Price";
        protected const string NameTextPath = "LabelText";
    
        public abstract void OnClick();
    }

    public abstract class SlotView<T> : SlotView where T : SlotView<T>, new()
    {
        protected GameObject gameObject;
        private GameObject states;
        private GameObject lockState;
        private GameObject buyState;
        protected abstract ItemType Type { get; }
        protected int index;

        public static TSlot Create<TSlot>(TSlot prefab, Transform parent, Item item, Sprite preview, out SlotView<T> slotView) where TSlot : Component
        {
            slotView = new T();
            var slot = Object.Instantiate(prefab, parent);
        
            slot.Get<Image>(PreviewImagePath).sprite = preview;
            slot.Get<TextMeshProUGUI>(NameTextPath).text = item.name;
            slotView.index = item.Index;
            slotView.states = slot.GetGameObject(StatesPath);
            slotView.lockState = slotView.states.GetGameObject(LockStatePath);
            slotView.buyState = slotView.states.GetGameObject(BuyStatePath);
            slotView.buyState.Get<TextMeshProUGUI>(PriceTextPath).text = item.currency.Value.ToString();
            slotView.gameObject = slot.gameObject;
            slotView.OnCreate();
        
            return slot;
        }

        public void Lock()
        {
            buyState.SetActive(false);
            lockState.SetActive(true);
        }
    
        public void Unlock()
        {
            buyState.SetActive(true);
            lockState.SetActive(false);
        }
    
        public void OnBuy()
        {
            states.SetActive(false);
        }

        protected virtual void OnCreate()
        {
        
        }
    
        protected virtual void OnOtherItemSelected(in int index)
        {
        
        }
    
        public virtual void OnSelect()
        {
        
        }

        public override void OnClick()
        {
            ItemSelector.Select(Type, index);
            OnSelect();
        }
    }
}