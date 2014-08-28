using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gladius
{
    public class ItemPanelInit : MonoBehaviour
    {

        public GameObject itemSlotPrefab;
        dfControl lastControl;
        private ShopItem lastShopItem;


        dfRichTextLabel detailsPanelLabel;
        dfControl imagePanel;

        GameObject modelImageSprite;



        // Use this for initialization
        void Awake()
        {
            GameObject panel = GameObject.Find("DetailsRTLabel");
            detailsPanelLabel = panel.GetComponent<dfRichTextLabel>();

            modelImageSprite = GameObject.Find("ModelImageSprite");

        }


        public void Populate()
        {
            List<string> shopItemNames = GladiusGlobals.CurrentShop.GetItemList();
            List<ShopItem> shopItems = new List<ShopItem>();

            dfScrollPanel container = gameObject.GetComponent<dfScrollPanel>();

            dfPanel itemPanel = itemSlotPrefab.GetComponent<dfPanel>();

            if (container != null)
            {
                //container.
                foreach (string itemName in shopItemNames)
                {
                    shopItems.Add(new ShopItem(itemName));
                }

                container.Virtualize<ShopItem>(shopItems, itemPanel);


                //GameObject shopItemGameObject = (GameObject)Instantiate(itemSlotPrefab);
                //shopItemGameObject.name = itemName;
                //ShopItemGUI shopItemGUI = shopItemGameObject.GetComponent<ShopItemGUI>();
                //shopItemGUI.ItemName = itemName;

                //dfControl panel = shopItemGameObject.GetComponent<dfControl>();
                //container.AddControl(panel);
                //shopItemGUI.Refresh();
            }

        }


        // Update is called once per frame
        void Update()
        {

            // test
            if(Input.GetMouseButton(0))
            {
                dfControl controlUnderMouse = dfInputManager.ControlUnderMouse;
                if(controlUnderMouse != null)
                {
                    GameObject go = controlUnderMouse.gameObject;
                    ShopItemGUI shopItemGUI = go.GetComponent<ShopItemGUI>();
                    if (shopItemGUI != null)
                    {
                        ShopItem currentShopItem = shopItemGUI.ShopItem;
                        ItemSelectionChanged(lastShopItem, currentShopItem);
                    }
                    lastControl = controlUnderMouse;
                    
                }
            }
        }

        public void ItemSelectionChanged(ShopItem previous, ShopItem current)
        {
            if (previous != null)
            {
                previous.Selected = false;
            }


            if (current != null)
            {
                current.Selected = true;
                detailsPanelLabel.Text = current.Item.Name;
                detailsPanelLabel.Text = string.Format("<h2 color=\"yellow\">{0}</h1><p>PWR: {1} CON:{2} INI: {3}</p><p><i>{4}</i></p>", current.Name, 10, 10, 10, current.Item.Description);

                GameObject goPrefab = (GameObject)(Resources.Load(GladiusGlobals.ModelsRoot + current.Item.ShortMeshName));
                if (goPrefab != null)
                {
                    modelImageSprite.GetComponent<ModelWindowHolder>().AttachedModelPrefabToWindow(goPrefab);
                }
                else
                {
                    Debug.LogWarning("Can't find : " + current.Item.ShortMeshName);
                }

                previous = current;
            }
        }


        public void PurchaseItem(ShopItemGUI itemGui)
        {
            GladiusGlobals.GladiatorSchool.CurrentCharacter.ReplaceItem(itemGui.ShopItem.Item.Name);
        }

    }
}