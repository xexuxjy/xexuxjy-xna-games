using UnityEngine;
using System.Collections;
using System;

namespace Gladius
{
    public class ShopItemGUI : MonoBehaviour, IDFVirtualScrollingTile
    {
        private dfPanel m_panel;
        private dfLabel m_labelName;
        private dfLabel m_labelCost;
        private dfRichTextLabel m_labelStats;

        private int m_index;

        // Use this for initialization
        void Awake()
        {
            m_panel = GetComponent<dfPanel>();
            //var slot = control.GetComponentInChildren<SpellSlot>();
            m_labelName = m_panel.Find<dfLabel>("ItemName");
            m_labelCost = m_panel.Find<dfLabel>("ItemCost");
            m_labelStats = m_panel.Find<dfRichTextLabel>("ItemStats");


            if (m_labelName == null) throw new Exception("Not found: lblName");
            if (m_labelCost == null) throw new Exception("Not found: lblCosts");
            if (m_labelStats == null) throw new Exception("Not found: lblStats");
        }

        // Update is called once per frame
        void Update()
        {

        }

        bool _selected;
        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
                m_labelName.Color = Selected?Color.blue:Color.white;
            }
        }

        public ShopItem ShopItem
        { get; set; }

        public void Refresh()
        {

            var control = gameObject.GetComponent<dfControl>();
            var container = control.Parent as dfScrollPanel;

            if (container != null)
            {
                control.Width = container.Width -container.ScrollPadding.horizontal;
            }

            m_labelName.Text = ShopItem.Name;
            m_labelCost.Text = "" + ShopItem.Cost;
            if (ShopItem.Item != null)
            {
                m_labelStats.Text = ShopItem.Item.Description;
            }

            // Resize this control to match the size of the contents
            var statsHeight = m_labelStats.RelativePosition.y + m_labelStats.Size.y;
            var costHeight = m_labelCost.RelativePosition.y + m_labelCost.Size.y;
            control.Height = Mathf.Max(statsHeight, costHeight) + 5;

        }



        #region IDFVirtualScrollingTile Members

        public int VirtualScrollItemIndex
        {
            get
            {
                return m_index;
            }
            set
            {
                m_index = value;
            }
        }

        public void OnScrollPanelItemVirtualize(object backingListItem)
        {
            ShopItem = backingListItem as ShopItem;
            Refresh();
        }

        public dfPanel GetDfPanel()
        {
            return m_panel;
        }

        #endregion
    }
}