using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Placement : MonoBehaviour
{
    String atlasPath = "GladiusUI/ArenaPlacement/PortraitAtlas";

    // Use this for initialization
    void Start()
    {
        BackgroundTextureName = "GladiusUI/Menu/ScrollBackground";
        m_backgroundTexture = Resources.Load<Texture2D>(BackgroundTextureName);
        m_arenaTexture = Resources.Load<Texture2D>("GladiusUI/ArenaPlacement/MoveGridHelper");

        TPackManager.load(atlasPath);


        m_gladiatorSchool = new GladiatorSchool();
        m_gladiatorSchool.Load("CharacterData");

        //m_arena = ArenaLoader.BuildArena("GladiusData/ArenaData/Arena1");
        m_availablePositions.AddRange(m_arena.PlayerPointList);

        SetCharacterData();
        RegisterListeners();
        BackgroundRect = new Rect(0, 0, Screen.width, Screen.height);    
    }


    public void RegisterListeners()
    {
        EventManager.ActionPressed += new EventManager.ActionButtonPressed(EventManager_ActionPressed);

    }

    public void DrawArenaScreen(Rect rect)
    {
        GUI.DrawTexture(rect, m_arenaTexture);
        foreach (Point p in m_availablePositions)
        {
            Rect t = new Rect(p.X * ArenaGridSquare.width,p.Y * ArenaGridSquare.height,ArenaGridSquare.width,ArenaGridSquare.height);
            t.x += rect.x;
            t.y += rect.y;
            GUI.DrawTexture(t,GladiusGlobals.ColourTextureHelper.GetV4Texture(new Vector4(1,1,1,1)));
        }

        foreach (Point p in m_filledPositions)
        {
            Rect t = new Rect(p.X * ArenaGridSquare.width,p.Y * ArenaGridSquare.height,ArenaGridSquare.width,ArenaGridSquare.height);
            t.x += rect.x;
            t.y += rect.y;
            GUI.DrawTexture(t, GladiusGlobals.ColourTextureHelper.GetV4Texture(new Vector4(.3f, .3f, .3f, 1)));
        }

        foreach (Point p in m_opponentPositions)
        {
            Rect t = new Rect(p.X * ArenaGridSquare.width, p.Y * ArenaGridSquare.height, ArenaGridSquare.width, ArenaGridSquare.height);
            t.x += rect.x;
            t.y += rect.y;
            GUI.DrawTexture(t, GladiusGlobals.ColourTextureHelper.GetV4Texture(new Vector4(1,0,0, 1)));
        }

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
                    Confirm();
                    break;
                }
            case (ActionButton.ActionButton2):
                {
                    CancelMode();
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
        if (CurrentMode == PlacementMode.Characters)
        {
            SetCurrentCharacter(Adjust(m_currentCharacterPoint, delta, CharGridWidth, CharGridHeight));

        }
        else
        {
            m_currentArenaPoint = GladiusGlobals.Add(m_currentArenaPoint, delta);
            // find next freepoint to arenapoint..
            int max = int.MaxValue;
            int nearestIndex = -1;
            for(int i=0;i<m_playerPointList.Count;++i)
            {
                int dist = GladiusGlobals.PointDist2(m_playerPointList[i],m_currentArenaPoint);
                if (dist < max)
                {
                    max = dist;
                    nearestIndex = i;
                }
            }
            m_currentIndex = nearestIndex;
        }
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


    // Update is called once per frame
    void Update()
    {

    }

    public void OnGUI()
    {
        GUI.DrawTexture(BackgroundRect, m_backgroundTexture);
        DrawCharacterGrid(PlayerRect,m_players,true);
        DrawCharacterGrid(OpponentsRect, m_opponents,false);
        DrawArenaScreen(ArenaRect);

        if (GUI.Button(ConfirmButtonRect, ConfirmButton))
        {
            DoConfirm();
        }

        if (GUI.Button(CancelButtonRect, CancelButton))
        {
            DoCancel();
        }

    }

    public void DoConfirm()
    {
        GladiusGlobals.CharacterPlacementMap.Clear();
        foreach (Point p in m_characterMap.Keys)
        {
            GladiusGlobals.CharacterPlacementMap[p] = m_characterMap[p];
        }
        // Move on to arena....
    }

    public void DoCancel()
    {



    }


    public void SetupPoints()
    {


    }


    void SelectNextPoint()
    {
        if (m_playerPointList.Count > 0)
        {
            m_currentIndex++;
            m_currentIndex = m_currentIndex % m_playerPointList.Count;
        }

    }

    void SelectPrevPoint()
    {
        if (m_playerPointList.Count > 0)
        {
            m_currentIndex--;
            if(m_currentIndex <0 )
            {
                m_currentIndex+=m_playerPointList.Count;
            }
        }

    }

    public void FillPosition(Point p, CharacterData character)
    {
        System.Diagnostics.Debug.Assert(m_availablePositions.Contains(p) && character.StartPosition.HasValue == false);
        if (m_availablePositions.Contains(p) && character.StartPosition.HasValue == false)
        {
            m_characterMap[p] = character;
            m_availablePositions.Remove(p);
            m_filledPositions.Add(p);
            character.StartPosition = p;
        }
    }

    public void EmptyPosition(Point p)
    {
        System.Diagnostics.Debug.Assert(m_filledPositions.Contains(p));
        if (m_filledPositions.Contains(p))
        {
            CharacterData character;
            if (m_characterMap.TryGetValue(p, out character))
            {
                character.StartPosition = null;
            }
            m_filledPositions.Remove(p);
            m_availablePositions.Add(p);
        }
    }

    public Dictionary<Point, CharacterData> PlacementMap
    {
        get
        {
            return m_characterMap;
        }
    }

    public void CancelMode()
    {
        if(CurrentMode == PlacementMode.Arena)
        {
            CurrentMode = PlacementMode.Characters;
        }
    }

    public void Confirm()
    {
        if (CurrentMode == PlacementMode.Characters)
        {
            CurrentMode = PlacementMode.Characters;

        }
        else
        {
            if(!m_filledPositions.Contains(m_currentArenaPoint))
            {
                FillPosition(m_currentArenaPoint, m_currentCharacter);
            }
        }
    }

    public void DrawCharacterGrid(Rect toDraw, List<CharacterData> characters, bool drawSelection)
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

                String regionName = currentCharacter != null ? currentCharacter.ThumbNailName : "UnavailableSlot.png";
                TPackManager.draw(destRect, atlasPath, regionName);

                if (currentCharacter != null)
                {
                    String levelString = "" + currentCharacter.Level;
                    if (drawSelection && currentCharacter.Selected)
                    {
                        GUI.DrawTexture(destRect, GladiusGlobals.ColourTextureHelper.GetV4Texture(new Vector4(1, 1, 1, 0.3f)));

                    }

                    Vector2 textDims = m_largeTextStyle.CalcSize(new GUIContent(levelString));

                    Vector2 textPos = new Vector2(destRect.x + 40, destRect.y + textDims.y + LevelTextAdjust);

                    m_largeTextStyle.normal.textColor = Color.red;
                    GUI.Label(new Rect(textPos.x, textPos.y, textDims.x, textDims.y), levelString, m_largeTextStyle);
                }
            }
        }
    }

    public void SetCharacterData()
    {
        m_players.Clear();
        m_players.AddRange(m_gladiatorSchool.Gladiators.Values);
        SetCurrentCharacter(new Point());
    }

    private void SetCurrentCharacter(Point p)
    {
        m_currentCharacterPoint = p;
        int index = (p.Y * CharGridWidth) + p.X;
        index = Mathf.Clamp(index, 0, m_players.Count - 1);
        m_currentCharacter = m_players[index];

    }


    public void SetCharacterSelected(CharacterData character)
    {
        if (!character.Selected)
        {
            character.Selected = true;
            //System.Diagnostics.Debug.Assert(!m_selectedCharacters.Contains(character));
            //m_selectedCharacters.Add(character);
        }
        else
        {
            character.Selected = false;
            //m_selectedCharacters.Remove(character);
            //System.Diagnostics.Debug.Assert(!m_selectedCharacters.Contains(character));
        }
    }

    public void ConfirmSelection()
    {
        //GladiusGlobals.ChosenCharacterList.Clear();
        //GladiusGlobals.ChosenCharacterList.AddRange(m_selectedCharacters);
        //Application.LoadLevel("GladiatorPlacement");

    }

    public bool AllPlaced
    {
        get
        {
            return m_filledPositions.Count == m_players.Count;
        }
    }


    public String BackgroundTextureName;
    private Texture2D m_backgroundTexture;
    private Texture2D m_arenaTexture;



    public int m_currentIndex;
    public List<Point> m_playerPointList = new List<Point>();
    public List<Point> m_opponentPointList = new List<Point>();

    public Dictionary<Point, CharacterData> m_characterMap = new Dictionary<Point, CharacterData>();
    public List<CharacterData> m_players = new List<CharacterData>();
    public List<CharacterData> m_opponents = new List<CharacterData>();


    public List<Point> m_availablePositions = new List<Point>();
    public List<Point> m_filledPositions = new List<Point>();
    public List<Point> m_opponentPositions = new List<Point>();

    public PlacementMode CurrentMode = PlacementMode.Characters;
    public CharacterData m_currentCharacter;


    public Rect BackgroundRect = new Rect(0, 0, 0, 0);
    public Rect PlayerRect = new Rect(0, 0, 600, 200);
    public Rect OpponentsRect = new Rect(0, 200, 600, 200);
    public Rect ArenaRect = new Rect(350, 0, 400, 400);

    public Rect ConfirmButtonRect = new Rect(600,400, 40, 10);
    public Rect CancelButtonRect = new Rect(650, 400, 40, 10);

    public GUIContent ConfirmButton = new GUIContent("Confirm");
    public GUIContent CancelButton = new GUIContent("Cancel");

    public Rectangle DestThumbnailDims = new Rectangle(0, 0, 64, 64);

    private Point m_currentArenaPoint = new Point();
    private Point m_currentCharacterPoint = new Point();

    public int CharGridWidth = 5;
    public int CharGridHeight = 3;
    public List<CharacterData> m_allCharacters;

    public Rect ArenaGridSquare = new Rect(0, 0, 10, 10);
    private Arena m_arena;

    public int LevelTextAdjust = 14;

    Font m_smallFont;
    Font m_largeFont;

    GUIStyle m_largeTextStyle;
    GUIStyle m_smallTextStyle;

    GUIText m_largeText;
    GUIText m_smallText;


    private GladiatorSchool m_gladiatorSchool;


    public enum PlacementMode
    {
        Characters,
        Arena
    }


}
