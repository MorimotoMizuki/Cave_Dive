using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using Common_Cave_Dive;

public class Screen_Change_Cave_Dive : MonoBehaviour
{
    [Header("各画面のゲームオブジェクト")]
    [SerializeField] private GameObject[] _Screen_obj;

    [Header("フェードさせる画像")]
    [SerializeField] private Image _Fade_img;

    [Header("画面のフェード時間")]
    [SerializeField] private float _Fade_Speed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
