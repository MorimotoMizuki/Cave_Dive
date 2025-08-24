using System.Collections.Generic;
using UnityEngine;

using static Common_Cave_Dive.GrovalConst_CaveDive;
using static Common_Cave_Dive.GrovalNum_CaveDive;
using static Common_Cave_Dive.GrovalStruct_CaveDive;

public class Game_Preference_Cave_Dive : MonoBehaviour
{
    [Header("各スクリプト")]
    [SerializeField] private Game_Manager_Cave_Dive     game_manager;
    [SerializeField] private Click_Manager_Cave_Dive    click_manager;
    [SerializeField] private Music_Manager_Cave_Dive    music_manager;
    [SerializeField] private Screen_Change_Cave_Dive    screen_change;
    [SerializeField] private Image_Manager_Cave_Dive    image_manager;
    [SerializeField] private Csv_Roader_Cave_Dive       csv_roader;

    [Header("初期ステージ番号")]
    public int _Stage_Num = 1;

    [Header("ステージの空気ゲージ制限時間(秒)")]
    public int[] _AirGage_Time;

    [Header("ステージごとのプレイヤーの初期角度")]
    public Dir_ID[] _Player_Angle;

    [Header("ステージごとの生成するキャラクターリスト\n\n座標 x,y :左上(-300, 360), 右下(300, -450)\n\n※オブジェクトのプレハブに設定していないオブジェクトは生成できません")]
    public List<Stage_Data> _Stage_Chara_Data;

    [Header("ゲーム判定画面に遷移する待機時間(秒)")]
    public float _Judge_Screen_Latency = 1.0f;

    [Header("マスク画像の透明度の最大値と最小値")]
    public float _Max_Mask_Alpha = 0.8f;
    public float _Min_Mask_Alpha = 0.2f;

    [Header("プレイヤーの無敵時間(フレーム)")]
    public int _Player__Invincible_Frame = 30;
    [Header("プレイヤーの基本の移動速度")]
    public float _Player_MoveSpeed = 3.0f;
    [Header("プレイヤーの回転速度")]
    public float _Player_RotSpeed = 200.0f;
    [Header("プレイヤーのノックバック速度")]
    public float _Player_KnockBackSpeed = 3.0f;

    [Header("機雷の基本の移動速度")]
    public float _Mine_MoveSpeed = 1.0f;

    [Header("サメの基本の移動速度")]
    public float _Shark_MoveSpeed = 1.0f;

    [Header("水中での摩擦・抵抗の強さ")]
    public float _Water_Drag = 2.0f;

    [Header("下方向の速度(浮力) : 1.0が通常時")]
    public float _Downward_SpeedModifier = 0.7f;

    [Header("空気ゲージが減少する量(秒)")]
    public float _Rock_DecAirGage = 5.0f;
    public float _Shark_DecAirGage = 10.0f;

    //プレイヤーのアニメーション切り替えフレーム数
    [HideInInspector]
    public int _Player_Anim_Cnt = 5;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {
        sGamePreference = this;
        sGameManager  = game_manager;
        sClickManager = click_manager;
        sMusicManager = music_manager;
        sScreenChange = screen_change;
        sImageManager = image_manager;
        sCsvRoader    = csv_roader;

        //60fpsに設定
        Application.targetFrameRate = 60;

        //60fps以上 : 5フレームごと
        if(Application.targetFrameRate >= 60)
            _Player_Anim_Cnt = 5;
        //60fps未満 : 3フレームごと
        else
            _Player_Anim_Cnt = 3;

        gNOW_STAGE_LEVEL = _Stage_Num;
    }
}
