using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Common_Cave_Dive;
using UnityEditor.Purchasing;

public class Game_Manager_Cave_Dive : MonoBehaviour
{
    [Header("ゲームオーバー時,ゲームクリア時に表示するオブジェクト")]
    [SerializeField] private GameObject _GameOver_obj;
    [SerializeField] private GameObject _GameClear_obj;

    [Header("オブジェクトのプレハブ")]
    [SerializeField] private GameObject[] _Obj_prefab;

    [Header("生成オブジェクトの親オブジェクト")]
    [SerializeField] private Transform _Obj_area;

    [Header("カメラ")]
    public Camera _Camera;

    //タイマー関係
    private float _Limit_time;   //制限時間
    private float _Current_time; //残り時間
    private float _Damage_time;  //障害物で減らす時間

    //財宝の総数
    private int _Treasure_sum = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //ゲーム画面以外の場合は終了
        if (GrovalNum_CaveDive.gNOW_SCREEN_ID != GrovalConst_CaveDive.Screen_ID.GAME)
            return;

        switch (GrovalNum_CaveDive.gNOW_GAMESTATE)
        {
            case GrovalConst_CaveDive.GameState.READY:
                {
                    break;
                }
            case GrovalConst_CaveDive.GameState.CREATE_STAGE:
                {
                    Create_Stage();
                    break;
                }
            case GrovalConst_CaveDive.GameState.PLAYING:
                {
                    AirGage_Timer(); //空気ゲージタイマー
                    break;
                }
            case GrovalConst_CaveDive.GameState.GAMECLEAR:
                {
                    //ゲームクリア用オブジェクトの表示
                    GrovalNum_CaveDive.sImageManager.Change_Active(_GameClear_obj, true);
                    break;
                }
            case GrovalConst_CaveDive.GameState.GAMEOVER:
                {
                    //ゲームオーバー用オブジェクトの表示
                    GrovalNum_CaveDive.sImageManager.Change_Active( _GameOver_obj, true);
                    GrovalNum_CaveDive.gNOW_GAMESTATE = GrovalConst_CaveDive.GameState.READY;
                    break;
                }
        }
    }

    /// <summary>
    /// ステージリセット処理
    /// </summary>
    public void Reset_Stage()
    {
        //ゲームクリア、ゲームオーバー用のオブジェクトの非表示
        GrovalNum_CaveDive.sImageManager.Change_Active(_GameClear_obj, false);
        GrovalNum_CaveDive.sImageManager.Change_Active(_GameOver_obj, false);

        //オブジェクトエリア内の子オブジェクトを全て削除
        //foreach (Transform child in _Obj_area)
        //    Delete_Obj(child.gameObject);
    }

    /// <summary>
    /// ステージ生成処理
    /// </summary>
    private void Create_Stage()
    {
        _Treasure_sum = 1; //仮

        GrovalNum_CaveDive.gNOW_GAMESTATE = GrovalConst_CaveDive.GameState.PLAYING;
    }

    /// <summary>
    /// オブジェクトの削除処理
    /// </summary>
    /// <param name="target_obj">対象のゲームオブジェクト</param>
    public void Delete_Obj(GameObject target_obj)
    {
        Destroy(target_obj);
    }

    /// <summary>
    /// マスク画像のアルファ値の減少処理
    /// </summary>
    public void Dec_Mask_Alpha()
    {
        //アルファ値の幅
        float dec_alpha = GrovalNum_CaveDive.sGamePreference._Max_Mask_Alpha - GrovalNum_CaveDive.sGamePreference._Min_Mask_Alpha;
        //減少するアルファ値を風船の合計数で割って求める
        dec_alpha /= _Treasure_sum;
        //マスク画像のアルファ値を減少させる
        GrovalNum_CaveDive.sImageManager.Decrement_Alpha(GrovalNum_CaveDive.sImageManager._Mask_obj, dec_alpha);
    }

    /// <summary>
    /// 制限時間設定
    /// </summary>
    /// <param name="time">制限時間</param>
    public void Set_Limit_Time(float time)
    {
        _Limit_time   = time;
        _Current_time = time;
        _Damage_time  = time;
        //タイマー初期表示
        GrovalNum_CaveDive.sImageManager._AirGage_Fill.fillAmount     = Mathf.InverseLerp(0, _Limit_time, _Current_time);
        GrovalNum_CaveDive.sImageManager._Damage_Gage_Fill.fillAmount = Mathf.InverseLerp(0, _Limit_time, _Damage_time);
    }

    /// <summary>
    /// 制限時間計測
    /// </summary>
    /// <returns>ゲームオーバーの可否</returns>
    private bool AirGage_Timer()
    {
        //時間計測
        _Current_time -= Time.deltaTime;
        //タイマーが 0 以下になった場合
        if (_Current_time <= 0.0f)
        {
            _Current_time = 0.0f;

            //タイマー(UI)に反映 : 残像が残らないようにするため
            GrovalNum_CaveDive.sImageManager._AirGage_Fill.fillAmount
                        = Mathf.InverseLerp(0, _Limit_time, _Current_time);
            GrovalNum_CaveDive.sImageManager._Damage_Gage_Fill.fillAmount
                        = Mathf.InverseLerp(0, _Limit_time, _Current_time);

            //ゲームオーバー
            GrovalNum_CaveDive.gNOW_GAMESTATE = GrovalConst_CaveDive.GameState.GAMEOVER;
            return true;
        }

        // 追従ゲージを追いつかせる
        _Damage_time = Mathf.MoveTowards(_Damage_time, _Current_time, Time.deltaTime * 5.0f);

        //タイマー(UI)に反映
        GrovalNum_CaveDive.sImageManager._AirGage_Fill.fillAmount 
                    = Mathf.InverseLerp(0, _Limit_time, _Current_time);
        GrovalNum_CaveDive.sImageManager._Damage_Gage_Fill.fillAmount 
                    = Mathf.InverseLerp(0, _Limit_time, _Damage_time);

        return false;
    }

    /// <summary>
    /// 空気ゲージ減少処理
    /// </summary>
    /// <param name="dec_time"></param>
    public void Dec_AirGage_Timer(float dec_time)
    {
        //現在の空気ゲージから減算
        _Current_time -= dec_time;

        //0未満にはしない
        if (_Current_time < 0.0f)
            _Current_time = 0;

        //一気に減らす場合のみ値を揃えてずれを作る
        if(_Damage_time < _Current_time)
            _Damage_time = _Current_time;
    }

}
