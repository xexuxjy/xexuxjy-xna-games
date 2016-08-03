using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SelectionGridSlot : MonoBehaviour
{
    public CharacterData Required;
    private CharacterData m_current;
    public SelectionGrid OwnerGrid;

    public void Start()
    {
        Button.onClick.AddListener(() => OnSlotClicked());
    }

    public CharacterData Current
    {
        get { return m_current; }
        set
        {
            m_current = value;
            Unavailable = m_current == null;
            if (m_current != null)
            {
                string path = GladiusGlobals.UIElements + Current.ThumbnailName;
                Sprite s = Resources.Load<Sprite>(path);
                Button.image.sprite = s;
            }
        }
    }

    public void OnSlotClicked()
    {
        if (OwnerGrid != null)
        {
            OwnerGrid.SlotClicked(m_current);
        }
    }


    public int index = 0;
    public Button Button;
    public bool Unavailable;

}
