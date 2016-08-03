using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Progress : MonoBehaviour
{
    private Image ForegroundImage;

    public int MaxValue;
    public int Value;
    private int LastValue=-1;


    public void Update()
    {
        Value = Mathf.Clamp(Value, 0, MaxValue);
        if (ForegroundImage != null && Value != LastValue)
        {
            LastValue = Value;
            ForegroundImage.fillAmount = (float)((float)Value / (float)MaxValue);
        }
    }

    public void SetSprite(Sprite sprite)
    {
        if (ForegroundImage != null)
        {
            ForegroundImage.sprite = sprite;
        }
    }

    //void Update()
    //{
    //    if (ForegroundImage != null && MaxValue != 0)
    //    {
    //        ForegroundImage.fillAmount = (float)m_value / (float)MaxValue;
    //    }
    //}

    void Start()
    {
        ForegroundImage = transform.Find("Foreground").GetComponent<Image>();
        Value = 0;
    }

    //Testing: this function will be called when Test Button is clicked
    public void UpdateProgress()
    {
        Hashtable param = new Hashtable();
        param.Add("from", 0.0f);
        param.Add("to", 100);
        param.Add("time", 5.0f);
        param.Add("onupdate", "TweenedSomeValue");
        param.Add("onComplete", "OnFullProgress");
        param.Add("onCompleteTarget", gameObject);
        //iTween.ValueTo(gameObject, param);
    }

    public void TweenedSomeValue(int val)
    {
        Value = val;
    }

    public void OnFullProgress()
    {
        Value = 0;
    }
}