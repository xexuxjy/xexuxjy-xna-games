using UnityEngine;
using System.Collections;
using System.Collections.Generic;

    public class TestPopulateShop : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            Shop shop = ShopManager.Load("");

            GameObject shopNameLabel = GameObject.Find("ShopNameLabel");
            dfLabel label = shopNameLabel.GetComponent<dfLabel>();
            label.Text = shop.Name;

            GameObject panel = GameObject.Find("InventoryPanel");

            if (panel != null)
            {
                ItemPanelInit panelInit = panel.GetComponent<ItemPanelInit>();
                if (panelInit != null)
                {
                    panelInit.Populate(shop);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
