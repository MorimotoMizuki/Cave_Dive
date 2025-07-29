using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Common_Cave_Dive;

public class Game_Preference_Cave_Dive : MonoBehaviour
{
    [Header("各スクリプト")]
    [SerializeField] private Game_Manager_Cave_Dive     game_manager;
    [SerializeField] private Click_Manager_Cave_Dive    click_manager;
    [SerializeField] private Music_Manager_Cave_Dive    music_manager;
    [SerializeField] private Screen_Change_Cave_Dive    screen_change;
    [SerializeField] private Image_Manager_Cave_Dive    image_manager;

    [Header("ステージの空気ゲージ制限時間(秒)")]
    public int[] _AirGage_Time;

    [Header("ゲーム判定画面に遷移する待機時間(秒)")]
    public float _Judge_Screen_Latency = 1.0f;

    [Header("マスク画像の透明度の最大値と最小値")]
    public float _Max_Mask_Alpha = 0.8f;
    public float _Min_Mask_Alpha = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        GrovalNum_CaveDive.sGamePreference  = this;
        GrovalNum_CaveDive.sGameManager     = game_manager;
        GrovalNum_CaveDive.sClickManager    = click_manager;
        GrovalNum_CaveDive.sMusicManager    = music_manager;
        GrovalNum_CaveDive.sScreenChange    = screen_change;
        GrovalNum_CaveDive.sImageManager    = image_manager;

        //60fpsに設定
        Application.targetFrameRate = 60;
    }
}
