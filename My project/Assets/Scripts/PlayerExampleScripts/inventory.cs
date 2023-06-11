using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class inventory : MonoBehaviour
{ 
   public int numberOfItems { get; private set; }

   public UnityEvent<inventory> onItemCollected;

   public void ItemsPickedUp()
   {
      numberOfItems++;
      onItemCollected.Invoke(this);
   }
}
