using UnityEngine;
using UnityEditor;

using static Common_Cave_Dive.GrovalConst_CaveDive;
using static Common_Cave_Dive.GrovalNum_CaveDive;

public class Click_Manager_Cave_Dive : MonoBehaviour
{
    //ボタンクリック可否フラグ
    [HideInInspector] 
    public bool[] _Is_Button;

    //クリックフラグ
    [HideInInspector] public bool _Is_Touch_or_Click_up;     //クリックまたはタッチが終了した瞬間

    //クリック中または画面に触れている状態
    [HideInInspector] 
    public bool _Is_Touch_or_Click = false;

    //プレイヤーがタッチ座標に近づいたフラグ
    private bool _Is_Player_Near = false;

    // Start is called before the first frame update
    void Start()
    {
        _Is_Button = new bool[4];
    }

    // Update is called once per frame
    void Update()
    {
        switch(gNOW_SCREEN_ID)
        {
            case Screen_ID.GAME:
                {
                    //ゲームプレイ中以外は終了
                    if (gNOW_GAMESTATE != GameState.PLAYING)
                        break;

                    if(_Is_Player_Near)
                    {
                        //クリックまたはタッチが終了した瞬間（離した）を検出する
                        _Is_Touch_or_Click_up = Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);

                        //手を離していない場合は終了
                        if (!_Is_Touch_or_Click_up)
                            return;
                        else
                            _Is_Player_Near = false;
                    }

                    //クリック中または画面に触れている状態を検出する（押している間ずっと true）
                    _Is_Touch_or_Click = Input.GetMouseButton(0) || (Input.touchCount > 0);

                    break;
                }
        }


        //エスケープキー入力
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();  //ゲーム終了
        }
    }

    /// <summary>
    /// マウス or タッチの入力位置を返す
    /// </summary>
    /// <returns>マウス or タッチの入力位置</returns>
    public Vector2 GetInputPosition()
    {
        if (Input.touchCount > 0)
            return Input.GetTouch(0).position;
        else
            return Input.mousePosition;
    }

    /// <summary>
    /// プレイヤーがタッチ座標が一定距離になった場合の処理
    /// </summary>
    public void Player_Near()
    {
        _Is_Player_Near = true;
        _Is_Touch_or_Click = false;
    }

    /// <summary>
    /// ゲーム終了関数
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        // エディタ上ではプレイモードを停止
        EditorApplication.isPlaying = false;
#else
            // ビルド後はアプリを終了
            Application.Quit();
#endif
    }

    //スタートボタン : ゲーム画面
    public void Button_Clicked_Start()
    {
        _Is_Button[(int)Button_ID.START] = true;     //ボタンフラグtrue
        sMusicManager.SE_Play(SE_ID.TAP); //SE再生
    }
    //ネクストボタン : クリア画面
    public void Button_Clicked_Next()
    {
        _Is_Button[(int)Button_ID.NEXT] = true;     //ボタンフラグtrue
        sMusicManager.SE_Play(SE_ID.TAP); //SE再生
    }
    //リプレイボタン : オーバー画面
    public void Button_Clicked_Replay()
    {
        _Is_Button[(int)Button_ID.REPLAY] = true;     //ボタンフラグtrue
        sMusicManager.SE_Play(SE_ID.TAP); //SE再生
    }
    //タイトルボタン : オーバー画面
    public void Button_Clicked_Title()
    {
        _Is_Button[(int)Button_ID.TITLE] = true;     //ボタンフラグtrue
        sMusicManager.SE_Play(SE_ID.TAP); //SE再生
    }
}
