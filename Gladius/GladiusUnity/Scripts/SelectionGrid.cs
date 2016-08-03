using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class SelectionGrid : MonoBehaviour
{
    public int GridWidth = 5;
    public int GridHeight = 2;

    public GameObject SelectionGridSlotPrefab;
    public Transform GroupPanel;
    public SelectionGridSlot[] Slots;

    public Image EmptySlotImage;
    public Image UnavailableSlotImage;
    public Image AvailableSlotImage;
    public Image RequiredSlotImage;

    public int NumAvailable;

    private CharacterDataList CurrentCharacterList;

    public int MaxSize
    { get { return GridWidth * GridHeight; } }

    public void Start()
    {
        SelectionGridSlot[] gridSlots = gameObject.GetComponentsInChildren<SelectionGridSlot>();
        Debug.Assert(gridSlots != null && gridSlots.Length == MaxSize);
        for (int i = 0; i < gridSlots.Length; ++i)
        {
            gridSlots[i].OwnerGrid = this;
        }

        Slots = gridSlots;
    }

    public void SlotClicked(CharacterData characterData)
    {
        if (CurrentCharacterList != null && characterData != null)
        {
            CurrentCharacterList.CurrentCharacter = characterData;
        }
    }


    public void SetStartDefault(CharacterDataList characterList)
    {
        CurrentCharacterList = characterList;

        int count = 0;
        int maxIndex = Math.Min(Slots.Length, characterList.NumCharacters);
        NumAvailable = maxIndex;

        for (int i = 0; i < maxIndex; ++i)
        {
            SetSlot(i, characterList.CharacterAtPosition(i));
            count++;
        }
        for (int i = count; i < Slots.Length; ++i)
        {
            SetSlot(i, null);
        }
    }

    public void SetChooseDefault(List<CharacterData> characterList)
    {
        int count = 0;
        int maxIndex = Math.Min(Slots.Length, characterList.Count);
        NumAvailable = maxIndex;

        for (int i = 0; i < maxIndex; ++i)
        {
            SelectionGridSlot gridSlot = Slots[i];
            gridSlot.Required = characterList[count];
            count++;
        }
    }

    public void Cleanup()
    {

    }

    public void SetSlot(int index, CharacterData characterData)
    {
        int x = index % GridWidth;
        int y = index / GridWidth;
        SetSlot(x, y, characterData);
    }

    public void SetSlot(int x, int y, CharacterData characterData)
    {
        DebugUtils.Assert(x >= 0 && x < GridWidth && y >= 0 && y < GridHeight);
        int index = (y * GridWidth) + x;
        SetSlot(Slots[index], characterData);
    }

    public void SetSlot(SelectionGridSlot gridSlot, CharacterData characterData)
    {
        gridSlot.Current = characterData;
    }

    public int NextAvailableSlot()
    {
        int result = -1;
        for (int i = 0; i < Slots.Length; ++i)
        {
            if(IsSlotEmpty(i))
            {
                result = i;
                break;
            }
        }

        return result;
    }

    public bool IsSlotEmpty(int index)
    {
        return Slots[index].Current == null;
    }

    public int AvailableSlots
    { get { return NumAvailable; } }

    public int FilledSlots
    {
        get
        {
            int count = 0;
            for (int i = 0; i < AvailableSlots; ++i)
            {
                if (!IsSlotEmpty(i))
                {
                    count++;
                }
            }
            return count;
        }
    }

    public int EmptySlots
    {
        get
        {
            return AvailableSlots - FilledSlots;
        }
    }

    public void FillParty(List<CharacterData> party)
    {
        party.Clear();
        for (int i = 0; i < Slots.Length; ++i)
        {
            if (Slots[i].Current != null)
            {
                party.Add(Slots[i].Current);
            }
        }
    }

    public void OnDisable()
    {
        Debug.LogWarning("SelectionGrid Disabled");
    }

    public void OnEnable()
    {
        Debug.LogWarning("SelectionGrid Enabled");
    }


}




