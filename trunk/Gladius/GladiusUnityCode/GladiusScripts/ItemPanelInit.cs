using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gladius
{
    public class ItemPanelInit : MonoBehaviour
    {

        public GameObject itemSlotPrefab;
        dfControl lastControl;

        dfRichTextLabel detailsPanelLabel;
        dfControl imagePanel;


        // Use this for initialization
        void Start()
        {
            GameObject panel = GameObject.Find("DetailsRTLabel");
            detailsPanelLabel = panel.GetComponent<dfRichTextLabel>();

        }

        public void Populate()
        {
            List<string> shopItems = GladiusGlobals.CurrentShop.GetItemList();
            dfScrollPanel container = gameObject.GetComponent<dfScrollPanel>();
            //dfControl container = gameObject.GetComponent<dfListbox>();

            if (container != null)
            {
                foreach (string itemName in shopItems)
                {
                    GameObject shopItemGameObject = (GameObject)Instantiate(itemSlotPrefab);
                    shopItemGameObject.name = itemName;
                    ShopItemGUI shopItemGUI = shopItemGameObject.GetComponent<ShopItemGUI>();
                    shopItemGUI.ItemName = itemName;

                    dfControl panel = shopItemGameObject.GetComponent<dfControl>();
                    container.AddControl(panel);
                    shopItemGUI.Refresh();
                }
            }

        }


        // Update is called once per frame
        void Update()
        {
            
            dfControl controlUnderMouse = dfInputManager.ControlUnderMouse;
            if (controlUnderMouse != null && controlUnderMouse != lastControl)
            {
                ShopItemGUI lastGuiItem = null;

                GameObject go = controlUnderMouse.gameObject;
                ShopItemGUI currentGuiItem = go.GetComponent<ShopItemGUI>();
                
                if (lastControl != null)
                {
                    lastGuiItem = lastControl.gameObject.GetComponent<ShopItemGUI>();
                }

                if (lastGuiItem != null)
                {
                    lastGuiItem.Selected = false;
                }
                if (currentGuiItem != null)
                {
                    currentGuiItem.Selected = true;
                }

                ItemSelectionChanged(lastGuiItem, currentGuiItem);

                lastControl = controlUnderMouse;
            }
        }

        public void ItemSelectionChanged(ShopItemGUI previous, ShopItemGUI current)
        {
            if (previous != null && current != null)
            {
                detailsPanelLabel.Text = current.ItemName;
            }
        }

    }
}