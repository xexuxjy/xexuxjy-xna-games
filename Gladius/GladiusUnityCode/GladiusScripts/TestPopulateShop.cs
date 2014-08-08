using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gladius
{
    public class TestPopulateShop : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            GladiusGlobals.LocalisationData = new LocalisationData();
            GladiusGlobals.LocalisationData.Load("");
            
            GladiusGlobals.ItemManager = new ItemManager();
            GladiusGlobals.ItemManager.Load("");


            Shop shop = ShopManager.Load("");
            GladiusGlobals.CurrentShop = shop;

            GameObject shopNameLabel = GameObject.Find("ShopNameLabel");
            dfLabel label = shopNameLabel.GetComponent<dfLabel>();
            label.Text = shop.Name;

            GameObject panel = GameObject.Find("InventoryPanel");

            //panel.Vi

            //GameObject panel = GameObject.Find("Listbox");

            if (panel != null)
            {
                ItemPanelInit panelInit = panel.GetComponent<ItemPanelInit>();
                if (panelInit != null)
                {
                    panelInit.Populate();
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}