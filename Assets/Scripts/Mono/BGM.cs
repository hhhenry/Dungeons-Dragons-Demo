using System;
using UnityEngine;
using UnityEngine.UI;

public class BGM : MonoBehaviour
{
    public AudioSource bgm;
    public Button bgm_btn;

    private void Start() {
        if (!PlayerPrefs.HasKey("bgm")) {
            PlayerPrefs.SetInt("bgm",1);
            bgm.Play();
            bgm_btn.image.sprite = Resources.Load<Sprite>("image/bgm_on");
        }
        else {
            int bgm_set = PlayerPrefs.GetInt("bgm");
            if (bgm_set == 1)
            {
                bgm_btn.image.sprite = Resources.Load<Sprite>("image/bgm_on");
                bgm.Play();
                PlayerPrefs.SetInt("bgm",1);
            }
            else
            {
                bgm_btn.image.sprite = Resources.Load<Sprite>("image/bgm_off");
                bgm.Stop();
                PlayerPrefs.SetInt("bgm",0);
            }
        }

        bgm_btn?.onClick.AddListener(OnMusicControl);
    }

    public void OnMusicControl()
    {
        int bgm_set = PlayerPrefs.GetInt("bgm");
        if (bgm_set == 0)
        {
            bgm_btn.image.sprite = Resources.Load<Sprite>("image/bgm_on");
            bgm.Play();
            PlayerPrefs.SetInt("bgm",1);
        }
        else
        {
            bgm_btn.image.sprite = Resources.Load<Sprite>("image/bgm_off");
            bgm.Stop();
            PlayerPrefs.SetInt("bgm",0);
        }
    }
}
