using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayerExampleScripts
{
    public class objects : MonoBehaviour
    {
        public List<GameObject> items;

        private int itemsCollected;

        private void Start()
        {
            itemsCollected = 0;
        }
        
        public void PickedUpAllItems()
        {
            itemsCollected++;
            if (itemsCollected >= items.Count)
                GameOver();
        }

        private void GameOver()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}