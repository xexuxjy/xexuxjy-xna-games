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
            Shop shop = ShopManager.LoadOriginalShopFile("");
            GladiusGlobals.CurrentShop = shop;
            GameObject panel = GameObject.Find("ItemListPanel");

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