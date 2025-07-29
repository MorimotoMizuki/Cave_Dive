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

    [Header("プレイヤーの回転速度")]
    public float _Player_RotSpeed = 200.0f;

    [Header("プレイヤーの基本の移動速度")]
    public float _Player_MoveSpeed = 3.0f;

    [Header("水中での摩擦・抵抗の強さ")]
    public float _Water_Drag = 2.0f;

    [Header("下方向の速度(浮力) : 1.0が通常時")]
    public float _Downward_SpeedModifier = 0.7f;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {
        GrovalNum_CaveDive.sGamePreference = this;
        GrovalNum_CaveDive.sGameManager = game_manager;
        GrovalNum_CaveDive.sClickManager = click_manager;
        GrovalNum_CaveDive.sMusicManager = music_manager;
        GrovalNum_CaveDive.sScreenChange = screen_change;
        GrovalNum_CaveDive.sImageManager = image_manager;

        //60fpsに設定
        Application.targetFrameRate = 60;
    }
}
