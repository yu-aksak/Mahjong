using System;
using UnityEngine;

namespace Core.CurrencyModule
{
    [Serializable]
    public class Currency
    {
        public event Action<int> ValueChanged;
    
        [SerializeField] private CurrencyType type;
        [SerializeField] private int value;

        public CurrencyType Type => type;

        public int Value
        {
            get
            {
                return value;
            }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    ValueChanged?.Invoke(value);
                }
            }
        }
    
        public Currency(CurrencyType type, int value = 0)
        {
            this.type = type;
            this.value = value;
        }
    }
}
