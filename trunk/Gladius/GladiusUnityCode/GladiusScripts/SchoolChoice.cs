using UnityEngine;
using System.Collections;
using Gladius;
using System.Collections.Generic;
using System;

public class SchoolChoice : MonoBehaviour 
{

    String atlasPath = "GladiusUI/ArenaPlacement/PortraitAtlas";

	// Use this for initialization
	void Start () 
    {
        BackgroundTextureName = "GladiusUI/Menu/ScrollBackground";
        m_backgroundTexture = Resources.Load<Texture2D>(BackgroundTextureName);

        TPackManager.load(atlasPath);

        m_gladiatorSchool = new GladiatorSchool();
        m_gladiatorSchool.Load("CharacterData");
        SetCharacterData();
        RegisterListeners();
        if (GladiusGlobals.ColourTextureHelper == null)
        {
            GladiusGlobals.ColourTextureHelper = new ColourTextureHelper();
        }

        m_largeFont = Resources.Load<Font>("GladiusUI/Arena/TREBUC");

        m_largeTextStyle = new GUIStyle();
        m_largeTextStyle.font = m_largeFont;
        m_largeTextStyle.fontSize = 20;
        m_largeTextStyle.fontStyle = FontStyle.Bold;


        m_smallTextStyle = new GUIStyle();
        m_smallTextStyle.font = m_largeFont;
        m_smallTextStyle.fontSize = 10;

        BackgroundRect = new Rect(0, 0, Screen.width, Screen.height);    



    }


    public void RegisterListeners()
    {
        EventManager.ActionPressed += new EventManager.ActionButtonPressed(EventManager_ActionPressed);

    }

    public void SetCharacterData()
    {
        m_allCharacters.Clear();
        m_selectedCharacters.Clear();
        m_allCharacters.AddRange(m_gladiatorSchool.Gladiators);
        foreach (CharacterData character in m_allCharacters)
        {
            character.Selected = false;
        }
        SetCurrentCharacter(new Point());
    }

    void EventManager_ActionPressed(object sender, ActionButtonPressedArgs e)
    {
        switch (e.ActionButton)
        {

            case (ActionButton.Move1Left):
                {
                    CursorLeft();
                    break;
                }
            case (ActionButton.Move1Right):
                {
                    CursorRight();
                    break;
                }
            case (ActionButton.Move1Up):
                {
                    CursorUp();
                    break;
                }
            case (ActionButton.Move1Down):
                {
                    CursorDown();
                    break;
                }
            case (ActionButton.ActionButton1):
                {
                    SetCharacterSelected(m_currentCharacter);
                    break;
                }
            case (ActionButton.ActionButton2):
                {
                    //CancelMode();
                    break;
                }
            case (ActionButton.ActionButton3):
                {
                    ConfirmSelection();
                    break;
                }

        }
    }

    public void CursorLeft()
    {
        UpdateCursor(new Point(-1, 0));
    }

    public void CursorRight()
    {
        UpdateCursor(new Point(1, 0));
    }

    public void CursorUp()
    {
        UpdateCursor(new Point(0, -1));
    }

    public void CursorDown()
    {
        UpdateCursor(new Point(0, 1));
    }

    private void UpdateCursor(Point delta)
    {
        SetCurrentCharacter(Adjust(m_currentCharacterPoint, delta, CharGridWidth, CharGridHeight));
    }

    public void UnregisterListeners()
    {
        //EventManager.ActionPressed -= new event ActionButtonPressed();
        EventManager.ActionPressed -= new EventManager.ActionButtonPressed(EventManager_ActionPressed);

    }

    public Point Adjust(Point start, Point delta, int width, int height)
    {
        start = GladiusGlobals.Add(start, delta);
        if (start.X < 0)
        {
            start.X += width;
        }
        if (start.X >= width)
        {
            start.X = 0;
        }
        if (start.Y < 0)
        {
            start.Y += height;
        }
        if (start.Y >= height)
        {
            start.Y = 0;
        }
        return start;
    }



    private void SetCurrentCharacter(Point p)
    {
        m_currentCharacterPoint = p;
        int index = (p.Y * CharGridWidth) + p.X;
        index = Mathf.Clamp(index, 0, m_allCharacters.Count - 1);
        m_currentCharacter = m_allCharacters[index];

    }


	// Update is called once per frame
	void Update () 
    {
	
	}

    void OnGUI()
    {
        GUI.DrawTexture(BackgroundRect, m_backgroundTexture);
        DrawCharacterGrid(AllRect, m_allCharacters,true);
        DrawCharacterGrid(SelectedRect, m_selectedCharacters,false);
        DrawCharacterSelection();
    }

    public void DrawCharacterSelection()
    {
        GUI.DrawTexture(CharacterRect, GladiusGlobals.ColourTextureHelper.GetV4Texture(new Vector4(0.3f, 0.3f, 0.3f, 1f)));
        if(m_currentCharacter != null)
        {
            GUI.Label(CharacterRect, m_currentCharacter.GetInfoString());
        }
    }

    public void DrawCharacterGrid(Rect toDraw,List<CharacterData> characters,bool drawSelection)
    {
        Rect adjustedRect = toDraw;

        Rectangle dims = DestThumbnailDims;
        for (int i = 0; i < CharGridWidth; i++)
        {
            for (int j = 0; j < CharGridHeight; ++j)
            {
                Point p = new Point(i, j);

                Rect destRect = new Rect(adjustedRect.x + (p.X * dims.Width), adjustedRect.y + (p.Y * dims.Height), DestThumbnailDims.Width, DestThumbnailDims.Height);

                if (drawSelection)
                {
                    Color borderColour = Color.white;
                    if (p == m_currentCharacterPoint)
                    {
                        borderColour = Color.black;
                    }
                    GUI.DrawTexture(destRect, GladiusGlobals.ColourTextureHelper.GetColourTexture(borderColour));
                }
                destRect = GladiusGlobals.InsetRectangle(destRect, 2);

                int index = (j * CharGridWidth) + i;
                CharacterData currentCharacter = null;
                if (index < characters.Count)
                {
                    currentCharacter = characters[index];
                }


                String regionName = currentCharacter != null?currentCharacter.ThumbNailName:"UnavailableSlot.png";
                TPackManager.draw(destRect, atlasPath, regionName);

                if(currentCharacter != null)
                {
                    String levelString = "" + currentCharacter.Level;
                    if (drawSelection && currentCharacter.Selected)
                    {
                        GUI.DrawTexture(destRect, GladiusGlobals.ColourTextureHelper.GetV4Texture(new Vector4(1,1,1,0.3f)));

                    }

                    Vector2 textDims = m_largeTextStyle.CalcSize(new GUIContent(levelString));

                    Vector2 textPos = new Vector2(destRect.x + 40, destRect.y + textDims.y + LevelTextAdjust);

                    m_largeTextStyle.normal.textColor = Color.red;
                    GUI.Label(new Rect(textPos.x, textPos.y, textDims.x, textDims.y), levelString, m_largeTextStyle);
                }
            }
        }
    }




    public void SetCharacterSelected(CharacterData character)
    {
        if (!character.Selected)
        {
            character.Selected = true;
            System.Diagnostics.Debug.Assert(!m_selectedCharacters.Contains(character));
            m_selectedCharacters.Add(character);
        }
        else
        {
            character.Selected = false;
            m_selectedCharacters.Remove(character);
            System.Diagnostics.Debug.Assert(!m_selectedCharacters.Contains(character));
        }
    }

    public void ConfirmSelection()
    {
        GladiusGlobals.ChosenCharacterList.Clear();
        GladiusGlobals.ChosenCharacterList.AddRange(m_selectedCharacters);
        Application.LoadLevel("GladiatorPlacement");

    }


    public String BackgroundTextureName;
    private Texture2D m_backgroundTexture;

    public int m_currentIndex;

    public CharacterData m_currentCharacter;


    //public Texture2D m_portraitAtlasTexture;
    //public TextureAtlas m_portraitAtlas;


    private GladiatorSchool m_gladiatorSchool;
    public List<CharacterData> m_allCharacters = new List<CharacterData>();
    public List<CharacterData> m_selectedCharacters = new List<CharacterData>();

    public Rect AllRect = new Rect(0, 0, 300, 250);
    public Rect SelectedRect = new Rect(0, 200, 300, 250);
    public Rect CharacterRect = new Rect(350, 0, 200, 390);
    public Rect BackgroundRect = new Rect(0, 0, 0,0);    

    private Point m_currentCharacterPoint = new Point();

    public int CharGridWidth = 5;
    public int CharGridHeight = 3;

    public int LevelTextAdjust = 14;

    Font m_smallFont;
    Font m_largeFont;

    GUIStyle m_largeTextStyle;
    GUIStyle m_smallTextStyle;

    GUIText m_largeText;
    GUIText m_smallText;


    //public Rectangle SourceThumbnailDims = new Rectangle(0, 0, 128, 128);
    public Rectangle DestThumbnailDims = new Rectangle(0, 0, 64, 64);

}

