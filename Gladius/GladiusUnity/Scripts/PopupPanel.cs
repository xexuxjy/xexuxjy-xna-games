using UnityEngine;
using System.Collections;

public class PopupPanel : BaseGUIPanel
{
    
    // Use this for initialization
    public override void ChildStart()
    {
        //base.Start();
        //m_panelName = "PopUp";
        //dfControl okButton = m_panel.FindPath<dfControl>("Ok");
        //if (okButton != null)
        //{
        //    okButton.Click += OkButton_Click;
        //}

    }

    //private void OkButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    //{
    //    m_townGuiController.HidePopupPanel();
    //}

    // Update is called once per frame
    void Update()
    {

    }

    public void SetPanelText(string text)
    {
        //transform.Find("PanelText").GetComponent<dfRichTextLabel>().Text = text;
    }



}
