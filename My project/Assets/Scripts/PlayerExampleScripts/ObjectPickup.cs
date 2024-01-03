﻿using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayerExampleScripts
{
    public class ObjectPickup : MonoBehaviour
    {
        private List<GameObject> itemsList;
        [SerializeField] 
        private TextMeshProUGUI itemText;

        public int items;
        private bool collectedItems;
        private Canvas _canvas;

        private void Start()
        {
            items = 0;
            _canvas = FindObjectOfType<Canvas>();

            if (_canvas != null)
                itemText = _canvas.GetComponentInChildren<TextMeshProUGUI>();
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
            if (other.CompareTag("endItem"))
            {
                Destroy(other.gameObject);
                items++;
                itemText.text = "Items Collected: " + items;
            }

            if (items >= 5)
            {
                LoadNextScene();
            }
        }

        void LoadNextScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}