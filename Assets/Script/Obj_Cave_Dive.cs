using Common_Cave_Dive;
using UnityEngine;
using UnityEngine.UI;

using static Common_Cave_Dive.GrovalConst_CaveDive;
using static Common_Cave_Dive.GrovalNum_CaveDive;
using static Common_Cave_Dive.GrovalStruct_CaveDive;

public class Obj_Cave_Dive : MonoBehaviour
{
    #region 共通 ----------------------------------------------------------------------------------------------------------

    private Image _Img;             //画像情報
    private Rigidbody2D _Rigid2D;   //Rigidbody2D情報
    private Collider2D _Collider2D; //Collider2D情報

    [Header("オブジェクトID")]
    [SerializeField] private Obj_ID _Obj_ID = Obj_ID.NONE;

    #endregion ------------------------------------------------------------------------------------------------------------

    Transform _PlayerArrow;

    //初期座標
    private Vector3 _Start_pos;
    //移動幅保存用
    private float _Keep_sin = 0.0f;
    private float _Keep_diff = 0.0f;
    //方向転換用
    private Vector2 _Dir;

    //初回起動用
    private bool _Is_once = true;

    private enum Player_State
    {
        PLAY,           //通常
        NO_OPERATION,   //操作不可
    }

    private Player_State _PlayerState = Player_State.PLAY;
    //プレイヤーの無敵時間用
    private int _Invincible_cnt = 0;

    // Start is called before the first frame update
    void Start()
    {
        //オブジェクトのIDを設定
        if(_Obj_ID == Obj_ID.NONE)
            _Obj_ID = Obj_Identification(gameObject.name);

        //オブジェクトの各情報を取得
        _Img = GetComponent<Image>();
        _Rigid2D = GetComponent<Rigidbody2D>();

        //初期座標設定
        _Start_pos = transform.position;

        //各オブジェクトの初期設定
        Obj_Initial_Setting();
    }

    // Update is called once per frame
    void Update()
    {
        switch(_Obj_ID)
        {
            //プレイヤー
            case Obj_ID.PLAYER:
            {
                //プレイヤーのアニメーション処理
                Player_Animation();

                switch (sGameManager._GoalMoveState)
                {
                    case GoalMoveState.INVISIBLE:
                    case GoalMoveState.DISPLAY:
                    {
                        //プレイヤーの移動処理
                        Player_Move();

                        //プレイヤーの矢印の表示設定をタッチの可否で切り替える
                        sImageManager.Change_Active(_PlayerArrow.gameObject, sClickManager._Is_Touch_or_Click);

                        if (_PlayerState == Player_State.NO_OPERATION)
                        {
                            _Invincible_cnt++;
                            //プレイヤーの無敵時間以上になった場合
                            if (_Invincible_cnt >= sGamePreference._Player__Invincible_Frame)
                            {
                                _Invincible_cnt = 0;
                                _PlayerState = Player_State.PLAY;
                            }
                        }
                        break;
                    }
                    case GoalMoveState.PLAYER_IN:
                    {
                        _Rigid2D.velocity = transform.up * sGamePreference._Player_MoveSpeed;

                        //画面外処理
                        if (Out_Screen(gameObject))
                            sGameManager._GoalMoveState = GoalMoveState.END;

                            break;
                    }
                    case GoalMoveState.END:
                    {
                        gNOW_GAMESTATE = GameState.GAMECLEAR;
                        sGameManager.Delete_Obj(gameObject);
                        break;
                    }
                }
                break;
            }
            //財宝
            case Obj_ID.TREASURE:
            {
                break;
            }
            //機雷
            case Obj_ID.MINE:
            {
                Spike_Move();
                break;
            }
            //サメ
            case Obj_ID.SHARK:
            {
                Shark_Move();
                break;
            }
            //岩
            case Obj_ID.ROCK:
            {
                break;
            }
            case Obj_ID.GOAL_ARROW:
            {
                if(sGameManager._GoalMoveState == GoalMoveState.DISPLAY && _Is_once)
                {
                    //不透明にする
                    sImageManager.Change_Alpha(_Img, 1.0f);
                    _Is_once = false;
                }
                break;
            }
        }
    }

    /// <summary>
    /// 各オブジェクトの初期設定
    /// </summary>
    private void Obj_Initial_Setting()
    {
        switch (_Obj_ID)
        {
            case Obj_ID.PLAYER:
                {
                    //画像設定
                    sImageManager.Change_Image(_Img, sImageManager._Player_img[0]);
                    //摩擦で徐々に減速
                    _Rigid2D.drag = sGamePreference._Water_Drag;

                    //プレイヤーの子オブジェクトの情報を取得
                    _PlayerArrow = transform.GetChild(0);
                    break;
                }
            case Obj_ID.TREASURE:
                {
                    //画像設定
                    sImageManager.Change_Image(_Img, sImageManager._Treasure_img);
                    break;
                }
            case Obj_ID.MINE:
                {
                    //画像設定
                    sImageManager.Change_Image(_Img, sImageManager._Mine_img);
                    break;
                }
            case Obj_ID.SHARK:
                {
                    //画像設定
                    sImageManager.Change_Image(_Img, sImageManager._Shark_img[(int)Dir_ID.RIGHT]);
                    _Dir.x = 1.0f;
                    break;
                }
            case Obj_ID.GOAL_ARROW:
                {
                    //画像設定
                    sImageManager.Change_Image(_Img, sImageManager._GoalArrow_img);
                    //透明にする
                    sImageManager.Change_Alpha(_Img, 0.0f);
                    break;
                }
        }
    }

    /// <summary>
    /// プレイヤーの移動処理
    /// </summary>
    private void Player_Move()
    {
        //プレイ中以外は終了
        if(gNOW_GAMESTATE != GameState.PLAYING || 
           _PlayerState == Player_State.NO_OPERATION)
        {
            sClickManager._Is_Touch_or_Click = false;
            return;
        }

        //タッチしていない場合は終了
        if (!sClickManager._Is_Touch_or_Click)
            return;

        //タッチしている座標の取得
        Vector3 tap_pos = sClickManager.GetInputPosition();
        tap_pos = sGameManager._Camera.ScreenToWorldPoint(tap_pos);
        tap_pos.z = 0.0f;

        //タッチ座標からプレイヤーまでの方向を計算
        _Dir = tap_pos - transform.position;
        _Dir = _Dir.normalized; //正規化

        // 回転を滑らかに合わせる
        float target_angle = Mathf.Atan2(_Dir.y, _Dir.x) * Mathf.Rad2Deg - 90f;
        float current_angle = transform.eulerAngles.z;
        float angle = Mathf.MoveTowardsAngle(current_angle, target_angle, sGamePreference._Player_RotSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // 浮力で速度補正
        float speed_modifier = 1f;
        if (_Dir.y < 0) speed_modifier = sGamePreference._Downward_SpeedModifier;

        _Rigid2D.velocity = transform.up * sGamePreference._Player_MoveSpeed * speed_modifier;
    }

    /// <summary>
    /// プレイヤーのアニメーション処理
    /// </summary>
    private void Player_Animation()
    {
        //プレイヤーの角度を取得 : 0～360度
        float angle = transform.eulerAngles.z;

        // 0が上　反時計回りに値が増える



        Debug.Log(angle);
    }

    /// <summary>
    /// 機雷の移動処理
    /// </summary>
    private void Spike_Move()
    {
        //Mathf.Sinはサイン波で -1～ 1 の値を返す
        float new_y = Mathf.Sin(Time.time * sGamePreference._Mine_MoveSpeed);
        //返された値を (移動幅)_Mine_Amplitude で拡大する
        new_y *= sGamePreference._Mine_Amplitude;

        //X,Zはそのままで、Y だけ上下に移動させる
        transform.position = new Vector3(_Start_pos.x, _Start_pos.y + new_y, _Start_pos.z);
    }

    /// <summary>
    /// サメの移動処理
    /// </summary>
    private void Shark_Move()
    {
        //Mathf.Sinはサイン波で -1～ 1 の値を返す
        float current_sin = Mathf.Sin(Time.time * sGamePreference._Shark_MoveSpeed);
        //返された値を (移動幅)_Shark_Amplitude で拡大する
        float new_x = current_sin * sGamePreference._Shark_Amplitude;

        //Y,Zはそのままで、X だけ上下に移動させる
        transform.position = new Vector3(_Start_pos.x + new_x, _Start_pos.y, _Start_pos.z);

        //勾配(差分)
        float diff = current_sin - _Keep_sin;

        // 勾配の符号が変わったら極値
        if (_Keep_diff != 0 && Mathf.Sign(_Keep_diff) != Mathf.Sign(diff))
        {
            //方向を反転
            _Dir.x *= -1.0f;
            if (_Dir.x == 1.0f)
                sImageManager.Change_Image(_Img, sImageManager._Shark_img[(int)Dir_ID.RIGHT]);
            else
                sImageManager.Change_Image(_Img, sImageManager._Shark_img[(int)Dir_ID.LEFT]);
        }

        _Keep_diff = diff;
        _Keep_sin = current_sin;
    }

    /// <summary>
    /// Trigger コライダーでの衝突判定「すり抜け判定」
    /// </summary>
    /// <param name="collision">衝突した相手の Collider2D</param>
    void OnTriggerEnter2D(Collider2D collision)
    {
        //プレイヤー以外の場合は終了
        if(_Obj_ID != Obj_ID.PLAYER) 
            return;

        //衝突したオブジェクトIDを取得
        Obj_ID collision_obj_id = Obj_Identification(collision.name);

        switch (collision_obj_id)
        {
            case Obj_ID.TREASURE:
            {
                //マスク画像のアルファ値を減少させる
                sGameManager.Dec_Mask_Alpha();
                //財宝の削除
                sGameManager.Delete_Obj(collision.gameObject);
                break;
            }
            case Obj_ID.SHARK:
            {
                //空気ゲージを減少
                sGameManager.Dec_AirGage_Timer(10);
                break;
            }
            case Obj_ID.GOAL:
                {
                    sGameManager._GoalMoveState = GoalMoveState.PLAYER_IN;
                    break;
                }
        }
    }

    /// <summary>
    /// 衝突判定「物理衝突判定」
    /// </summary>
    /// <param name="collision">衝突情報をまとめた Collision2D 型</param>
    void OnCollisionEnter2D(Collision2D collision)
    {
        //プレイヤー以外の場合は終了
        if (_Obj_ID != Obj_ID.PLAYER)
            return;

        //衝突したオブジェクトIDを取得
        Obj_ID collision_obj_id = Obj_Identification(collision.gameObject.name);

        switch (collision_obj_id)
        {
            //岩
            case Obj_ID.ROCK:
            {
                //ノックバック処理
                KnockBack(collision);

                if (_PlayerState == Player_State.PLAY)
                {
                    //空気ゲージを減少
                    sGameManager.Dec_AirGage_Timer(5);
                    _PlayerState = Player_State.NO_OPERATION; //操作不可にする
                }
                break;
            }
            //機雷
            case Obj_ID.MINE:
            {
                //ゲームオーバー
                gNOW_GAMESTATE = GameState.GAMEOVER;
                break;
            }
            //サメ
            case Obj_ID.SHARK:
            {
                //空気ゲージを減少
                sGameManager.Dec_AirGage_Timer(10);
                break;
            }
            //ゴールを塞ぐ障害物
            case Obj_ID.OBSTACLE:
            {
                //ノックバック処理
                KnockBack(collision);
                
                if (_PlayerState == Player_State.PLAY)
                    _PlayerState = Player_State.NO_OPERATION; //操作不可にする

                break;
            }
        }
    }

    /// <summary>
    /// ノックバック処理
    /// </summary>
    /// <param name="collision">衝突情報をまとめた Collision2D 型</param>
    private void KnockBack(Collision2D collision)
    {
        //ぶつかった面の法線(プレイヤーを押し返す方向)
        Vector2 normal = collision.contacts[0].normal;

        //速度リセット
        _Rigid2D.velocity = Vector2.zero;

        //法線方向にノックバック
        _Rigid2D.velocity = normal * sGamePreference._Player_KnockBackSpeed;
    }

    /// <summary>
    /// オブジェクトの識別
    /// </summary>
    /// <returns>オブジェクトID</returns>
    private Obj_ID Obj_Identification(string obj_name)
    {
        //Name_to_Obj_ID: 文字列（部分一致用）とオブジェクトIDのマッピング辞書
        foreach (var pair in Name_to_Obj_ID)
        {
            //ゲームオブジェクトの名前に、辞書のキー（判別用文字列）が含まれているか確認
            if (obj_name.Contains(pair.Key))
            {
                //一致した場合、対応するオブジェクトIDを返す
                return pair.Value;
            }
        }
        //どのキーとも一致しない場合、NONEを返す（未識別の意味）
        return Obj_ID.NONE;
    }

    /// <summary>
    /// オブジェクトの画面外判定
    /// </summary>
    /// <param name="target_obj">ゲームオブジェクト</param>
    /// <returns></returns>
    private bool Out_Screen(GameObject target_obj)
    {
        //targetUI（RectTransform）のワールド座標をカメラのビューポート座標に変換
        //ビューポート座標は (0,0) が画面左下、(1,1) が画面右上を示す
        Vector3 viewport_pos = sGameManager._Camera.WorldToViewportPoint(target_obj.transform.position);

        //ビューポート座標が 0～1 の範囲外であれば、画面外にあると判定
        if (viewport_pos.x < 0 || viewport_pos.x > 1 || viewport_pos.y < 0 || viewport_pos.y > 1)
            return true;    //画面外
        else
            return false;   //画面内
    }

}
