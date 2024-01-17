using System;
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
        private Collider itemCollider;
        private Renderer itemRenderer;

        private void Start()
        {
            items = 0;
            _canvas = FindObjectOfType<Canvas>();
            itemCollider = GetComponent<Collider>();
            itemRenderer = GetComponent<Renderer>();

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
            Debug.Log("OntriggerCalled");
            if (other.CompareTag("endItem"))
            {
                Collect();
            }
        }

        private void Collect()
        {
            Debug.Log("collected called");
            //Destroy(other.gameObject);
            itemCollider.enabled = false;
            itemRenderer.enabled = false;
            items++;
            itemText.text = "Items Collected: " + items;
            
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