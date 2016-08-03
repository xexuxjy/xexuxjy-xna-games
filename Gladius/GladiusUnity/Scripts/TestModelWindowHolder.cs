using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestModelWindowHolder : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {

    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!builtData)
        {
            builtData = true;
            List<CharacterData> characterList = new List<CharacterData>();
            characterList.Add(ActorGenerator.CreateRandomCharacter(ActorClass.BARBARIANF, 1));
            characterList.Add(ActorGenerator.CreateRandomCharacter(ActorClass.BANDITA, 1));
            characterList.Add(ActorGenerator.CreateRandomCharacter(ActorClass.OGRE, 1));

            GetComponentInChildren<CharacterPanel>().CharacterDataList.SetData(characterList);


        }
    }

    bool builtData;

}
