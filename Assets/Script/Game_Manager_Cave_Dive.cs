using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Common_Cave_Dive.GrovalConst_CaveDive;
using static Common_Cave_Dive.GrovalNum_CaveDive;
using static Common_Cave_Dive.GrovalStruct_CaveDive;
using static UnityEditor.PlayerSettings;

public class Game_Manager_Cave_Dive : MonoBehaviour
{
    [Header("ゲームオーバー時,ゲームクリア時に表示するオブジェクト")]
    [SerializeField] private GameObject _GameOver_obj;
    [SerializeField] private GameObject _GameClear_obj;

    [Header("オブジェクトのプレハブ")]
    [SerializeField] private GameObject[] _Obj_prefab;

    [Header("岩オブジェクトのプレハブ")]
    [SerializeField] private GameObject[] _Rock_prefab;

    [Header("生成オブジェクトの親オブジェクト")]
    [SerializeField] private Transform _Obj_area;
    [Header("岩オブジェクトの親オブジェクト")]
    [SerializeField] private Transform _Rock_area;

    [Header("ゴールオブジェクト")]
    [SerializeField] private GameObject _Goal_obj;

    [Header("出口を塞ぐ障害物オブジェクト")]
    [SerializeField] private GameObject _Obstacle_obj;

    [Header("カメラ")]
    public Camera _Camera;

    //プレイヤーの情報
    private Obj_Cave_Dive _Player_Data;

    //ゴールの処理のフェーズ用
    [HideInInspector] public GoalMoveState _GoalMoveState;

    //岩マップデータ
    private List<List<int>> _Rock_StageMap = new List<List<int>>();
    //岩のサイズ
    private float _RockBlock_Size = 17.6f;

    //財宝オブジェクトのリスト
    private List<GameObject> _Treasure_List = new List<GameObject>();

    //タイマー関係
    private float _Limit_time;   //制限時間
    private float _Current_time; //残り時間
    private float _Damage_time;  //障害物で減らす時間

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //ゲーム画面以外の場合は終了
        if (gNOW_SCREEN_ID != Screen_ID.GAME)
            return;

        switch (gNOW_GAMESTATE)
        {
            case GameState.READY:
                {
                    break;
                }
            case GameState.CREATE_STAGE:
                {
                    Create_Stage();
                    break;
                }
            case GameState.PLAYING:
                {
                    AirGage_Timer(); //空気ゲージタイマー
                    Goal_Open_Judge(); //ゴール開放判定
                    break;
                }
            case GameState.GAMECLEAR:
                {
                    //ゲームクリア用オブジェクトの表示
                    sImageManager.Change_Active(_GameClear_obj, true);
                    break;
                }
            case GameState.GAMEOVER:
                {
                    //ゲームオーバー用オブジェクトの表示
                    sImageManager.Change_Active( _GameOver_obj, true);
                    gNOW_GAMESTATE = GameState.READY;
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
        sImageManager.Change_Active(_GameClear_obj, false);
        sImageManager.Change_Active(_GameOver_obj, false);

        //ゴールを非表示設定
        _GoalMoveState = GoalMoveState.INVISIBLE;
        //ゴールオブジェクト非表示
        sImageManager.Change_Active(_Goal_obj, false);
        //障害物オブジェクト表示
        sImageManager.Change_Active(_Obstacle_obj, true);

        _Treasure_List.Clear();
        _Treasure_List = new List<GameObject>();

        //オブジェクトエリア内の子オブジェクトを全て削除
        foreach (Transform child in _Obj_area)
            Delete_Obj(child.gameObject);
        foreach (Transform child in _Rock_area)
            Delete_Obj(child.gameObject);
    }

    /// <summary>
    /// ステージ生成処理
    /// </summary>
    private void Create_Stage()
    {
        string index = $"stage{gNOW_STAGE_LEVEL}";
        //マップデータにステージデータがあるかチェック
        if (sCsvRoader._RockMapData.ContainsKey(index))
            _Rock_StageMap = sCsvRoader._RockMapData[index];
        else
        {
            Debug.LogError("岩ステージマップデータがありません"); 
            return;
        }

        //岩オブジェクトマップ生成
        Create_RockMap(_Rock_StageMap);

        for(int i = 0; i < sGamePreference._Stage_Chara_Data.Count; i++)
        {
            //キャラデータのステージ番号とステージレベルが一致した場合
            if (sGamePreference._Stage_Chara_Data[i].StageNum == gNOW_STAGE_LEVEL)
            {
                //キャラマップ生成
                Create_CharaMap(sGamePreference._Stage_Chara_Data[i].Chara_Data);
                break;
            }
        }

        //カウントダウンスタート
        StartCoroutine(Countdown());

        gNOW_GAMESTATE = GameState.READY;
    }

    /// <summary>
    /// 岩のステージマップ生成
    /// </summary>
    /// <param name="map_data"></param>
    private void Create_RockMap(List<List<int>> map_data)
    {
        //マップの初期座標 : 左上端
        Vector2 pos = new Vector2(0 + _RockBlock_Size / 2, 0 - _RockBlock_Size / 2);

        for (int y = 0; y < map_data.Count; y++)
        {
            for(int x = 0; x < map_data[y].Count; x++)
            {
                int index = map_data[y][x];
                //空白ではない場合
                if(index != (int)Rock_ID.NONE)
                {
                    //オブジェクト生成
                    GameObject obj = Instantiate(_Rock_prefab[index - 1], _Rock_area);
                    //座標設定
                    obj.GetComponent<RectTransform>().anchoredPosition = pos;
                    //名前設定
                    obj.name = $"{Obj_ID.ROCK}";
                }
                pos.x += _RockBlock_Size;
            }
            pos.x = 0 + _RockBlock_Size / 2;
            pos.y -= _RockBlock_Size;
        }
    }

    /// <summary>
    /// キャラクターの生成処理
    /// </summary>
    /// <param name="chara_data"></param>
    private void Create_CharaMap(Character_Data[] chara_data)
    {
        for(int i = 0;  i < chara_data.Length; i++)
        {
            int index = (int)chara_data[i].Obj_ID;

            //オブジェクトプレハブに設定されているオブジェクトだけ生成
            if (index <= _Obj_prefab.Length && index > 0) 
            {
                //オブジェクト生成
                GameObject obj = Instantiate(_Obj_prefab[index - 1], _Obj_area);
                //座標設定
                obj.GetComponent<RectTransform>().anchoredPosition = chara_data[i].pos;
                //名前設定
                obj.name = $"{chara_data[i].Obj_ID}";

                switch (chara_data[i].Obj_ID)
                {
                    case Obj_ID.PLAYER:
                        {
                            //プレイヤーのデータを追加
                            _Player_Data = obj.GetComponent<Obj_Cave_Dive>();
                            break;
                        }
                    case Obj_ID.TREASURE:
                    {
                        //財宝のデータ追加
                        _Treasure_List.Add(obj);
                        break;
                    }
                    case Obj_ID.MINE:
                    case Obj_ID.SHARK:
                        {
                            Obj_Cave_Dive obj_data = obj.GetComponent<Obj_Cave_Dive>();
                            //機雷とサメに移動幅を設定
                            obj_data._MoveRange = chara_data[i].move_range;
                            obj_data._IsStart_Up_Right = chara_data[i].is_start_up_right;
                            break;
                        }
                }
            }
        }
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
    /// ゴール開放判定
    /// </summary>
    public void Goal_Open_Judge()
    {
        //ゴールが非表示フェーズ以外の場合は終了
        if (_GoalMoveState != GoalMoveState.INVISIBLE)
            return;

        for(int i = 0; i < _Treasure_List.Count; i++)
        {
            //財宝オブジェクトがある場合は終了
            if (_Treasure_List[i] != null)
                return;
        }

        //ゴールを表示フェーズにする
        _GoalMoveState = GoalMoveState.DISPLAY;

        //ゴール表示
        sImageManager.Change_Active(_Goal_obj, true);
        //障害物オブジェクト非表示
        sImageManager.Change_Active(_Obstacle_obj, false);
    }

    /// <summary>
    /// マスク画像のアルファ値の減少処理
    /// </summary>
    public void Dec_Mask_Alpha()
    {
        //アルファ値の幅
        float dec_alpha = sGamePreference._Max_Mask_Alpha - sGamePreference._Min_Mask_Alpha;
        //減少するアルファ値を風船の合計数で割って求める
        dec_alpha /= _Treasure_List.Count;
        //マスク画像のアルファ値を減少させる
        sImageManager.Decrement_Alpha(sImageManager._Mask_obj, dec_alpha);
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
        sImageManager._AirGage_Fill.fillAmount     = Mathf.InverseLerp(0, _Limit_time, _Current_time);
        sImageManager._Damage_Gage_Fill.fillAmount = Mathf.InverseLerp(0, _Limit_time, _Damage_time);
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
            sImageManager._AirGage_Fill.fillAmount
                        = Mathf.InverseLerp(0, _Limit_time, _Current_time);
            sImageManager._Damage_Gage_Fill.fillAmount
                        = Mathf.InverseLerp(0, _Limit_time, _Current_time);

            //ゲームオーバー
            gNOW_GAMESTATE = GameState.GAMEOVER;
            //タイムオーバーした場合のプレイヤーの設定
            _Player_Data.TimeOver_Player_Setting();
            return true;
        }

        // 追従ゲージを追いつかせる
        _Damage_time = Mathf.MoveTowards(_Damage_time, _Current_time, Time.deltaTime * 5.0f);

        //タイマー(UI)に反映
        sImageManager._AirGage_Fill.fillAmount 
                    = Mathf.InverseLerp(0, _Limit_time, _Current_time);
        sImageManager._Damage_Gage_Fill.fillAmount 
                    = Mathf.InverseLerp(0, _Limit_time, _Damage_time);

        return false;
    }

    /// <summary>
    /// カウントダウン
    /// </summary>
    /// <returns></returns>
    IEnumerator Countdown()
    {
        int cnt = 3;
        //カウントダウンオブジェクト表示
        sImageManager.Change_Active(sImageManager._Countdown_obj.gameObject, true);
        //カウントダウン画像に変更
        sImageManager.Change_Image(sImageManager._Countdown_obj, sImageManager._Countdown_img[cnt - 1]);
        //SE再生
        //sMusicManager.SE_Play_BGM_Stop(SE_ID.COUNTDOWN);

        while (cnt > 0)
        {
            //カウントダウン画像変更
            sImageManager.Change_Image(sImageManager._Countdown_obj, sImageManager._Countdown_img[cnt - 1]);
            yield return StartCoroutine(WaitForFrames(60)); // 60フレーム待機
            cnt--;
        }
        //スタート画像に変更
        sImageManager.Change_Image(sImageManager._Countdown_obj, sImageManager._Start_img);
        yield return StartCoroutine(WaitForFrames(60));     // 60フレーム待機

        //カウントダウンオブジェクト非表示
        sImageManager.Change_Active(sImageManager._Countdown_obj.gameObject, false);

        //ゲームプレイ可
        gNOW_GAMESTATE = GameState.PLAYING;
    }

    /// <summary>
    /// 指定したフレーム数待機
    /// </summary>
    /// <param name="frame_cnt">フレーム数</param>
    /// <returns></returns>
    IEnumerator WaitForFrames(int frame_cnt)
    {
        for (int i = 0; i < frame_cnt; i++)
        {
            yield return null; // 1フレーム待機
        }
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
