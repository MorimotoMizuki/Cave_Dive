using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using static Common_Cave_Dive.GrovalConst_CaveDive;
using static Common_Cave_Dive.GrovalNum_CaveDive;
using static Common_Cave_Dive.GrovalStruct_CaveDive;

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
        _Screen_obj[(int)Screen_ID.TITLE].gameObject.SetActive(true);
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
        for(Button_ID i = Button_ID.START; i <= Button_ID.TITLE;i++)
        {
            //何らかのボタンをクリックしている場合
            if (sClickManager._Is_Button[(int)i])
            {
                //表示非表示画面ID用
                Screen_ID display_id = Screen_ID.NONE, invisible_id = Screen_ID.NONE;

                switch (i)
                {
                    case Button_ID.START:
                        {
                            display_id = Screen_ID.GAME;
                            invisible_id = Screen_ID.TITLE;
                            break;
                        }
                    case Button_ID.NEXT:
                        {
                            display_id = Screen_ID.GAME;
                            invisible_id = Screen_ID.CLEAR;
                            break;
                        }
                    case Button_ID.REPLAY:
                        {
                            display_id = Screen_ID.GAME;
                            invisible_id = Screen_ID.GAME;
                            break;
                        }
                    case Button_ID.TITLE:
                        {
                            display_id = Screen_ID.TITLE;
                            invisible_id = Screen_ID.GAME;
                            break;
                        }
                }
                //画面切り替え
                if (display_id != Screen_ID.NONE && invisible_id != Screen_ID.NONE)
                    Screen_Change_Start(display_id, invisible_id, true);

                //フラグfalse
                sClickManager._Is_Button[(int)i] = false;

            }
        }
    }

    /// <summary>
    /// 画面切り替えのコルーチンを呼び出す関数
    /// </summary>
    /// <param name="display_id">表示したい画面を示すID</param>
    /// <param name="invisible_id">非表示にしたい画面を示すID</param>
    /// <param name="is_fade">フェードを行う可否</param>
    public void Screen_Change_Start(Screen_ID display_id, Screen_ID invisible_id, bool is_fade)
    {
        //画面IDが未設定の場合は終了
        if (display_id == Screen_ID.NONE || invisible_id == Screen_ID.NONE)
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
    private IEnumerator Screen_Change_Coroutine(Screen_ID display_id, Screen_ID invisible_id, bool is_fade)
    {
        Color fade_color = _Fade_img.color; //フェードする色

        if (is_fade)
        {
            //フェードする色の設定
            if (display_id == Screen_ID.CLEAR)
                fade_color = Color.white;
            else
                fade_color = Color.black;

            //フェードイン
            sImageManager.Change_Active(_Fade_img.gameObject, true);
            yield return StartCoroutine(Fade(0f, 1f, fade_color)); //透明→色
        }

        sImageManager.Change_Active(_Screen_obj[(int)invisible_id].gameObject, false); //画面非表示            
        sImageManager.Change_Active(_Screen_obj[(int)display_id].gameObject, true);    //画面表示

        gNOW_SCREEN_ID = display_id;      //現在の画面情報更新

        //現在表示されている画面
        switch (display_id)
        {
            //タイトル画面
            case Screen_ID.TITLE:
                {
                    //BGMを再生
                    //GrovalNum_CaveDive.sMusicManager.BGM_Change(GrovalConst_CaveDive.BGM_ID.TITLE);

                    gNOW_GAMESTATE = GameState.READY;//待機フェーズ
                    gNOW_STAGE_LEVEL = 1; //ステージレベルを1にする
                    break;
                }
            //ゲーム画面
            case Screen_ID.GAME:
                {
                    //BGMを再生
                    //GrovalNum_CaveDive.sMusicManager.BGM_Change(GrovalConst_CaveDive.BGM_ID.GAME);

                    sGameManager.Reset_Stage();  //ステージリセット                                                                 
                    gNOW_GAMESTATE = GameState.CREATE_STAGE; //ステージ生成フェーズへ

                    int index = gNOW_STAGE_LEVEL - 1; //配列呼び出すインデクス
                    int time = 60; //制限時間

                    //時間が設定されている場合
                    if (index < sGamePreference._AirGage_Time.Length)
                        time = sGamePreference._AirGage_Time[index];

                    //タイマーの時間を設定
                    sGameManager.Set_Limit_Time(time);

                    if (index < sImageManager._BackGround_img.Length)
                    {
                        //背景画像設定
                        for (int i = 0; i < sImageManager._BackGround_obj.Length; i++)
                            sImageManager.Change_Image(sImageManager._BackGround_obj[i], sImageManager._BackGround_img[index]);
                    }

                    //マスク画像設定
                    sImageManager.Change_Image(sImageManager._Mask_obj, sImageManager._Mask_img);
                    //マスク画像のアルファ値を最大値に変更
                    sImageManager.Change_Alpha(sImageManager._Mask_obj, sGamePreference._Max_Mask_Alpha);
                    break;
                }
            //クリア画面
            case Screen_ID.CLEAR:
                {
                    //GrovalNum_CaveDive.sMusicManager.SE_Play_BGM_Stop(GrovalConst_CaveDive.SE_ID.CLEAR); //SE再生 : BGM停止

                    gNOW_GAMESTATE = GameState.READY;//待機フェーズ
                    _JudgeState = Judge_State.READY;
                    _Judge_cnt = 0;
                    break;
                }
        }

        if (is_fade)
        {
            // フェードアウト
            yield return StartCoroutine(Fade(1f, 0f, fade_color)); //色→透明
            sImageManager.Change_Active(_Fade_img.gameObject, false);
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
                if (gNOW_GAMESTATE != GameState.GAMECLEAR)
                    break;

                _JudgeState = Judge_State.JUDGE;

                break;
            }
            case Judge_State.JUDGE:
            {
                //カウントが指定の秒数以上の場合
                if (_Judge_cnt >= sGamePreference._Judge_Screen_Latency * Application.targetFrameRate)
                    _JudgeState = Judge_State.SCREEN_CHANGE;
                else
                    _Judge_cnt++;

                break;
            }
            case Judge_State.SCREEN_CHANGE:
            {
                //表示非表示画面ID用
                Screen_ID display_id = Screen_ID.NONE, invisible_id = Screen_ID.NONE;

                display_id   = Screen_ID.CLEAR;
                invisible_id = Screen_ID.GAME;

                //画面切り替え
                if (display_id != Screen_ID.NONE && invisible_id != Screen_ID.NONE)
                    Screen_Change_Start(display_id, invisible_id, true);

                _JudgeState = Judge_State.END;
                break;
            }
        }
    }
}
