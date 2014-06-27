using UnityEngine;
using System.Collections;
using System;

namespace Gladius
{
    public class ShopItemGUI : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

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
                var control = gameObject.GetComponent<dfControl>();
                var lblName = control.Find<dfLabel>("ItemName");
                lblName.Color = Selected?Color.blue:Color.white;
            }
        }

        public string ItemName
        { get; set; }

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

            //var slot = control.GetComponentInChildren<SpellSlot>();
            var lblName = control.Find<dfLabel>("ItemName");
            var lblCost = control.Find<dfLabel>("ItemCost");
            var lblStats = control.Find<dfRichTextLabel>("ItemStats");


            if (lblCost == null) throw new Exception("Not found: lblCosts");
            if (lblName == null) throw new Exception("Not found: lblName");
            if (lblStats == null) throw new Exception("Not found: lblStats");

            ShopItem = GladiusGlobals.CurrentShop.GetItem(this.ItemName);
            lblName.Text = ShopItem.Name;
            lblCost.Text = "" + ShopItem.Cost;
            if (ShopItem.Item != null)
            {
                lblStats.Text = ShopItem.Item.Description;
            }

            //if (assignedSpell == null)
            //{
            //    slot.Spell = "";
            //    lblCosts.Text = "";
            //    lblName.Text = "";
            //    lblDescription.Text = "";
            //    return;
            //}
            //else
            //{
            //    slot.Spell = this.spellName;
            //    lblName.Text = assignedSpell.Name;
            //    lblCosts.Text = string.Format("{0}/{1}/{2}", assignedSpell.Cost, assignedSpell.Recharge, assignedSpell.Delay);
            //    lblDescription.Text = assignedSpell.Description;
            //}

            // Resize this control to match the size of the contents
            var statsHeight = lblStats.RelativePosition.y + lblStats.Size.y;
            var costHeight = lblCost.RelativePosition.y + lblCost.Size.y;
            control.Height = Mathf.Max(statsHeight, costHeight) + 5;

        }


    }
}