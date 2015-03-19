using UnityEngine;
using System.Collections;
using System;

public class SelectionGrid : MonoBehaviour
{
    public int GridWidth = 5;
    public int GridHeight = 2;

    public dfPanel GroupPanel;
    public dfSprite SlotPrefab;
    public dfButton[] Slots;
    public string EmptySlot = "blocker.tga";

    public Rectangle SlotDims = new Rectangle(0, 0, 40, 60);

    public void Init(dfControl owner,String name,MouseEventHandler onClick)
    {
        GroupPanel = owner.AddControl<dfPanel>();
        GroupPanel.name = name;
        GroupPanel.RelativePosition = new Vector2();
        Slots = new dfButton[GridWidth * GridHeight];
        //for (int i = 0; i < Slots.Length; ++i)
        //{
        //    Slots[i] = GroupPanel.AddControl<dfSprite>();
        //    Slots[i].Position = new Vector3(
        //    Slots[i].Click += SelectionGrid_Click;
        //}

        Vector2 ownerSize = owner.Size;
        float slotWidth = ownerSize.x / GridWidth;
        float slotHeight = ownerSize.y / GridHeight;


        GroupPanel.Size = new Vector2(GridWidth * SlotDims.Width, GridHeight * SlotDims.Height);
        for (int i = 0; i < GridWidth; ++i)
        {
            for (int j = 0; j < GridHeight; j++)
            {
                int index = (j * GridWidth) + i;
                Slots[index] = GroupPanel.AddControl<dfButton>();
                Slots[index].RelativePosition = new Vector3(i * slotWidth, j * slotHeight, 0);
                Slots[index].Width = slotWidth;
                Slots[index].Height = slotHeight;
                Slots[index].Click += onClick;

                SetSlot(i, j, EmptySlot);
            }
        }
    }


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Cleanup()
    {

    }


    public void SetSlot(int x, int y, string texture)
    {
        DebugUtils.Assert(x >= 0 && x < GridWidth && y >= 0 && y < GridHeight);
        int index = (y * GridWidth) + x;

        Slots[index].BackgroundSprite= texture;
    }



}



