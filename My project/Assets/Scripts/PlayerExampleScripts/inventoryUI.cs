using System;
using UnityEngine;
using TMPro;

namespace PlayerExampleScripts
{
    public class inventoryUI : MonoBehaviour
    {
        private TextMeshProUGUI itemText;

        private void Start()
        {
            itemText = GetComponent<TextMeshProUGUI>();
        }

        public void UpdateItemNum(inventory inventory)
        {
            itemText.text = inventory.numberOfItems.ToString();
        }
    }
}