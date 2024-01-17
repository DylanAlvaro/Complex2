using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayerExampleScripts
{
    public class CollectablePickup : MonoBehaviour
    {
        private List<GameObject> itemsList;
        [SerializeField] 
        private TextMeshProUGUI collectableText;

        public int collectables;
        private Canvas _canvas;
     

        private void Start()
        {
            collectables = 0;
            _canvas = FindObjectOfType<Canvas>();
           

            if (_canvas != null)
                collectableText = _canvas.GetComponentInChildren<TextMeshProUGUI>();
        }

        /// <summary>
        /// if item is called enditem
        /// item int is increased
        /// item is destroyed
        /// value is increased in UI
        /// if more than 5 are collected than game over.
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("collectable"))
            {
                Destroy(other.gameObject);
                collectables++;
                collectableText.text = "Collectables Found: " + collectables;
            }
            
            if (collectables >= 5)
            {
                // User can create functionality for what happens after collecting all collectables
            }
        }
    }
}