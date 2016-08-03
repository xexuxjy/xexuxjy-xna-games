using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    //public Texture2D texture;
    //LoadingScreen instance;

    void Awake()
    {
        //if (instance)
        //{
        //    Destroy(gameObject);
        //    return;
        //}
        //instance = this;
        SetVisible(false);
        
      //  texture = Resources.Load<Texture2D>("GladiusUI/TitleScreen");
        //GetComponent<GUITexture>().texture = texture;
        //transform.position = new Vector3(0.5f, 0.5f, 0.0f);
        DontDestroyOnLoad(this);
    }

    private void SetVisible(bool val)
    {
        //if (val)
        //{
        //    gameObject.GetComponent<dfTextureSprite>().RelativePosition = new Vector3();
        //    gameObject.GetComponent<dfTextureSprite>().BringToFront();
        //}
        //gameObject.GetComponent<dfTextureSprite>().IsVisible = val;

    }


    public void Load(int index)
    {
        //if (NoInstance()) return;
        SetVisible(true);
        Application.LoadLevel(index);
        SetVisible(false);
    }

    public void Load(string name)
    {
        //if (NoInstance()) return;
        SetVisible(true);
        Application.LoadLevel(name);
        SetVisible(false);
    }

    //bool NoInstance()
    //{
    //    if (!instance)
    //        Debug.LogError("Loading Screen is not existing in scene.");
    //    return instance;
    //}
}