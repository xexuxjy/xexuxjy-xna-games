using UnityEngine;
using System.Collections;

public class TestPopulateSelectionGrid : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
	
	}

    // Update is called once per frame
    void Update()
    {
        if (!havePopulatedGrid)
        {
            selectionGrid = gameObject.GetComponentInChildren<SelectionGrid>();
            if (selectionGrid != null)
            {
                havePopulatedGrid = true;

                for (int i = 0; i < selectionGrid.MaxSize; ++i)
                {
                    CharacterData characterData = ActorGenerator.CreateRandomCharacter(0);
                    selectionGrid.SetSlot(i, characterData);
                }

            }

        }
    }
    bool havePopulatedGrid;
    SelectionGrid selectionGrid;

}
