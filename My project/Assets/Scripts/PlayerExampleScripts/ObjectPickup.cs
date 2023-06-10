using System;
using TMPro;
using UnityEngine;

namespace PlayerExampleScripts
{
    public class ObjectPickup : MonoBehaviour
    {
        private TextMeshProUGUI text;
        public DungeonCreator item;

        private void Start()
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            text.text = item.ItemCount.ToString();
        }
    }
}