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

    private enum Judge_State
    {
        READY, JUDGE, SCREEN_CHANGE, END,
    }

    private Judge_State _JudgeState = Judge_State.READY;
    private int _Judge_cnt = 0;

    // Start is called before the first frame update
    void Start()
    {
        //全ての画面非表示
        for(int i = 0; i < _Screen_obj.Length; i++)
            _Screen_obj[i].gameObject.SetActive(false);

        //タイトル画面を表示
        _Screen_obj[(int)GrovalConst_CaveDive.Screen_ID.TITLE].gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        //ボタン判定
        Clicked_Button();
        //ゲームクリア判定
        Game_Judge_Screen();
    }

    /// <summary>
    /// ボタン判定
    /// </summary>
    private void Clicked_Button()
    {
        for(GrovalConst_CaveDive.Button_ID i = GrovalConst_CaveDive.Button_ID.START; i <= GrovalConst_CaveDive.Button_ID.TITLE;i++)
        {
            //何らかのボタンをクリックしている場合
            if (GrovalNum_CaveDive.sClickManager._Is_Button[(int)i])
            {
                //表示非表示画面ID用
                GrovalConst_CaveDive.Screen_ID display_id = GrovalConst_CaveDive.Screen_ID.NONE, invisible_id = GrovalConst_CaveDive.Screen_ID.NONE;

                switch (i)
                {
                    case GrovalConst_CaveDive.Button_ID.START:
                        {
                            display_id = GrovalConst_CaveDive.Screen_ID.GAME;
                            invisible_id = GrovalConst_CaveDive.Screen_ID.TITLE;
                            break;
                        }
                    case GrovalConst_CaveDive.Button_ID.NEXT:
                        {
                            display_id = GrovalConst_CaveDive.Screen_ID.GAME;
                            invisible_id = GrovalConst_CaveDive.Screen_ID.CLEAR;
                            break;
                        }
                    case GrovalConst_CaveDive.Button_ID.REPLAY:
                        {
                            display_id = GrovalConst_CaveDive.Screen_ID.GAME;
                            invisible_id = GrovalConst_CaveDive.Screen_ID.GAME;
                            break;
                        }
                    case GrovalConst_CaveDive.Button_ID.TITLE:
                        {
                            display_id = GrovalConst_CaveDive.Screen_ID.TITLE;
                            invisible_id = GrovalConst_CaveDive.Screen_ID.GAME;
                            break;
                        }
                }
                //画面切り替え
                if (display_id != GrovalConst_CaveDive.Screen_ID.NONE && invisible_id != GrovalConst_CaveDive.Screen_ID.NONE)
                    Screen_Change_Start(display_id, invisible_id, true);

                //フラグfalse
                GrovalNum_CaveDive.sClickManager._Is_Button[(int)i] = false;

            }
        }
    }

    /// <summary>
    /// 画面切り替えのコルーチンを呼び出す関数
    /// </summary>
    /// <param name="display_id">表示したい画面を示すID</param>
    /// <param name="invisible_id">非表示にしたい画面を示すID</param>
    /// <param name="is_fade">フェードを行う可否</param>
    public void Screen_Change_Start(GrovalConst_CaveDive.Screen_ID display_id, GrovalConst_CaveDive.Screen_ID invisible_id, bool is_fade)
    {
        //画面IDが未設定の場合は終了
        if (display_id == GrovalConst_CaveDive.Screen_ID.NONE || invisible_id == GrovalConst_CaveDive.Screen_ID.NONE)
            return;

        //コルーチン開始
        StartCoroutine(Screen_Change_Coroutine(display_id, invisible_id, is_fade));
    }

    /// <summary>
    ///  画面切り替えコルーチン
    /// </summary>
    /// <param name="display_id">表示する画面を示すID</param>
    /// <param name="invisible_id">非表示にする画面を示すID</param>
    /// <param name="is_fade">フェードを行う可否</param>
    /// <returns>コルーチン用 IEnumerator</returns>
    private IEnumerator Screen_Change_Coroutine(GrovalConst_CaveDive.Screen_ID display_id, GrovalConst_CaveDive.Screen_ID invisible_id, bool is_fade)
    {
        Color fade_color = _Fade_img.color; //フェードする色

        if (is_fade)
        {
            //フェードする色の設定
            if (display_id == GrovalConst_CaveDive.Screen_ID.CLEAR)
                fade_color = Color.white;
            else
                fade_color = Color.black;

            //フェードイン
            GrovalNum_CaveDive.sImageManager.Change_Active(_Fade_img.gameObject, true);
            yield return StartCoroutine(Fade(0f, 1f, fade_color)); //透明→色
        }

        GrovalNum_CaveDive.sImageManager.Change_Active(_Screen_obj[(int)invisible_id].gameObject, false); //画面非表示            
        GrovalNum_CaveDive.sImageManager.Change_Active(_Screen_obj[(int)display_id].gameObject, true);    //画面表示

        GrovalNum_CaveDive.gNOW_SCREEN_ID = display_id;      //現在の画面情報更新
        Debug.Log(GrovalNum_CaveDive.gNOW_SCREEN_ID);

        //現在表示されている画面
        switch (display_id)
        {
            //タイトル画面
            case GrovalConst_CaveDive.Screen_ID.TITLE:
                {
                    //BGMを再生
                    //GrovalNum_CaveDive.sMusicManager.BGM_Change(GrovalConst_CaveDive.BGM_ID.TITLE);

                    GrovalNum_CaveDive.gNOW_GAMESTATE = GrovalConst_CaveDive.GameState.READY;//待機フェーズ
                    GrovalNum_CaveDive.gNOW_STAGE_LEVEL = 1; //ステージレベルを1にする
                    break;
                }
            //ゲーム画面
            case GrovalConst_CaveDive.Screen_ID.GAME:
                {
                    //BGMを再生
                    //GrovalNum_CaveDive.sMusicManager.BGM_Change(GrovalConst_CaveDive.BGM_ID.GAME);

                    GrovalNum_CaveDive.sGameManager.Reset_Stage();  //ステージリセット                                                                 
                    GrovalNum_CaveDive.gNOW_GAMESTATE = GrovalConst_CaveDive.GameState.CREATE_STAGE; //ステージ生成フェーズへ

                    int index = GrovalNum_CaveDive.gNOW_STAGE_LEVEL - 1; //配列呼び出すインデクス
                    int time = 60; //制限時間

                    //時間が設定されている場合
                    if (index < GrovalNum_CaveDive.sGamePreference._AirGage_Time.Length)
                        time = GrovalNum_CaveDive.sGamePreference._AirGage_Time[index];

                    //タイマーの時間を設定
                    GrovalNum_CaveDive.sGameManager.Set_Limit_Time(time);

                    if (index < GrovalNum_CaveDive.sImageManager._BackGround_img.Length)
                    {
                        //背景画像設定
                        for (int i = 0; i < GrovalNum_CaveDive.sImageManager._BackGround_obj.Length; i++)
                            GrovalNum_CaveDive.sImageManager.Change_Image(GrovalNum_CaveDive.sImageManager._BackGround_obj[i], GrovalNum_CaveDive.sImageManager._BackGround_img[index]);
                    }

                    //マスク画像設定
                    GrovalNum_CaveDive.sImageManager.Change_Image(GrovalNum_CaveDive.sImageManager._Mask_obj, GrovalNum_CaveDive.sImageManager._Mask_img);
                    //マスク画像のアルファ値を最大値に変更
                    GrovalNum_CaveDive.sImageManager.Change_Alpha(GrovalNum_CaveDive.sImageManager._Mask_obj, GrovalNum_CaveDive.sGamePreference._Max_Mask_Alpha);
                    break;
                }
            //クリア画面
            case GrovalConst_CaveDive.Screen_ID.CLEAR:
                {
                    //GrovalNum_CaveDive.sMusicManager.SE_Play_BGM_Stop(GrovalConst_CaveDive.SE_ID.CLEAR); //SE再生 : BGM停止

                    GrovalNum_CaveDive.gNOW_GAMESTATE = GrovalConst_CaveDive.GameState.READY;//待機フェーズ
                    _JudgeState = Judge_State.READY;
                    _Judge_cnt = 0;
                    break;
                }
        }

        if (is_fade)
        {
            // フェードアウト
            yield return StartCoroutine(Fade(1f, 0f, fade_color)); //色→透明
            GrovalNum_CaveDive.sImageManager.Change_Active(_Fade_img.gameObject, false);
        }
    }

    /// <summary>
    /// フェードコルーチン
    /// </summary>
    /// <param name="from">現在の画面の alpha値</param>
    /// <param name="to">最終的なの画面の alpha値</param>
    /// <param name="color">フェードする色</param>
    /// <returns>コルーチン用の IEnumerator</returns>
    private IEnumerator Fade(float from, float to, Color color)
    {
        float timer = 0f; //経過時間の初期化

        //フェード時間中ループ
        while (timer < _Fade_Speed)
        {
            float alpha = Mathf.Lerp(from, to, timer / _Fade_Speed);        //指定時間内でalpha値を補間            
            _Fade_img.color = new Color(color.r, color.g, color.b, alpha);  //補間したalpha値を用いて色を設定(rgbはそのままでalpha値のみ変更)           
            timer += Time.deltaTime;                                        //経過時間を加算            
            yield return null;                                              //1フレーム待機
        }
        _Fade_img.color = new Color(color.r, color.g, color.b, to);         //最終的な色を設定
    }

    /// <summary>
    /// ゲーム判定処理
    /// </summary>
    private void Game_Judge_Screen()
    {
        switch(_JudgeState)
        {
            case Judge_State.READY:
            {
                //クリア判定では無い場合は終了
                if (GrovalNum_CaveDive.gNOW_GAMESTATE != GrovalConst_CaveDive.GameState.GAMECLEAR)
                    break;

                _JudgeState = Judge_State.JUDGE;

                break;
            }
            case Judge_State.JUDGE:
            {
                //カウントが指定の秒数以上の場合
                if (_Judge_cnt >= GrovalNum_CaveDive.sGamePreference._Judge_Screen_Latency * Application.targetFrameRate)
                    _JudgeState = Judge_State.SCREEN_CHANGE;
                else
                    _Judge_cnt++;

                break;
            }
            case Judge_State.SCREEN_CHANGE:
            {
                //表示非表示画面ID用
                GrovalConst_CaveDive.Screen_ID display_id = GrovalConst_CaveDive.Screen_ID.NONE, invisible_id = GrovalConst_CaveDive.Screen_ID.NONE;

                display_id   = GrovalConst_CaveDive.Screen_ID.CLEAR;
                invisible_id = GrovalConst_CaveDive.Screen_ID.GAME;

                //画面切り替え
                if (display_id != GrovalConst_CaveDive.Screen_ID.NONE && invisible_id != GrovalConst_CaveDive.Screen_ID.NONE)
                    Screen_Change_Start(display_id, invisible_id, true);

                _JudgeState = Judge_State.END;
                break;
            }
        }
    }
}
