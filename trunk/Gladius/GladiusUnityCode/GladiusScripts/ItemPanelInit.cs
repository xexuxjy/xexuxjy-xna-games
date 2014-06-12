using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gladius
{
    public class ItemPanelInit : MonoBehaviour
    {

        public GameObject itemSlotPrefab;


        // Use this for initialization
        void Start()
        {

        }

        public void Populate()
        {
            List<string> shopItems = GladiusGlobals.CurrentShop.GetItemList();
            dfScrollPanel scrollPanel = gameObject.GetComponent<dfScrollPanel>();

            foreach (string itemName in shopItems)
            {
                GameObject shopItemGameObject = (GameObject)Instantiate(itemSlotPrefab);
                shopItemGameObject.name = itemName;
                ShopItemGUI shopItemGUI = shopItemGameObject.GetComponent<ShopItemGUI>();
                shopItemGUI.ItemName = itemName;

                dfControl panel = shopItemGameObject.GetComponent<dfControl>();
                scrollPanel.AddControl(panel);
                shopItemGUI.Refresh();
            }

        }


        // Update is called once per frame
        void Update()
        {

        }
    }
}