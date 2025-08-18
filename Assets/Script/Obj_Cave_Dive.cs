using UnityEngine;
using UnityEngine.UI;

using static Common_Cave_Dive.GrovalConst_CaveDive;
using static Common_Cave_Dive.GrovalNum_CaveDive;

public class Obj_Cave_Dive : MonoBehaviour
{
    #region 共通 ----------------------------------------------------------------------------------------------------------

    private Image _Img;             //画像情報
    private Rigidbody2D _Rigid2D;   //Rigidbody2D情報
    private Collider2D _Collider2D; //Collider2D情報
    private RectTransform _Rect;    //RectTransform情報

    [Header("オブジェクトID")]
    [SerializeField] private Obj_ID _Obj_ID = Obj_ID.NONE;

    //方向転換用
    private Vector2 _Dir;

    #endregion ------------------------------------------------------------------------------------------------------------

    #region プレイヤー ----------------------------------------------------------------------------------------------------

    //プレイヤーの状態
    private Player_State _PlayerState = Player_State.PLAY;
    //プレイヤーの無敵時間用
    private int _Invincible_cnt = 0;
    //プレイヤーの矢印オブジェクト
    private Transform _PlayerArrow;
    //向いている角度
    Dir_ID _PlayerDir;
    //アニメーション用
    private int _Anim_index = 0;
    private int _Anim_cnt = 0;
    //フロントフラグ : 方向転換する角度が大きい場合true
    private bool _isFront = false;

    #endregion ------------------------------------------------------------------------------------------------------------

    //機雷オブジェクトの機雷の状態管理用
    private Mine_State _Obj_MineState = Mine_State.READY;
    //ゴール矢印の状態管理用
    [HideInInspector]
    public GoalMoveState _Obj_GoalState = GoalMoveState.INVISIBLE;

    //機雷とサメの移動幅
    [HideInInspector]
    public float _MoveRange;
    //初期の方向フラグ
    [HideInInspector]
    public bool _IsStart_Up_Right= true;
    //初期の方向用
    private float _PhaseOffset = 0.0f;

    //初期座標
    private Vector3 _Start_pos;
    //移動幅保存用
    private float _Keep_sin = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        //オブジェクトのIDを設定
        if(_Obj_ID == Obj_ID.NONE)
            _Obj_ID = Obj_Identification(gameObject.name);

        //オブジェクトの各情報を取得
        _Img = GetComponent<Image>();
        _Rigid2D = GetComponent<Rigidbody2D>();
        _Rect = GetComponent<RectTransform>();

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
                Player_Action();
                break;
            //機雷
            case Obj_ID.MINE:
                Mine_Action();
                break;
            //サメ
            case Obj_ID.SHARK:
                Shark_Move();
                break;
            //ゴールの矢印
            case Obj_ID.GOAL_ARROW:
                GoalArrow_Action();
                break;
            //ヒットマーク
            case Obj_ID.HIT_MARK:
                _Anim_cnt++;
                //無敵フレーム数以上になった場合は削除
                if (_Anim_cnt >= sGamePreference._Player__Invincible_Frame)
                    sGameManager.Delete_Obj(gameObject);
                break;
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
                    sImageManager.Change_Image(_Img, sImageManager._Player_Right_img[0]);
                    //摩擦で徐々に減速
                    _Rigid2D.drag = sGamePreference._Water_Drag;

                    //プレイヤーの子オブジェクトのRectTransform情報を取得
                    _Rect = transform.GetChild(0).GetComponent<RectTransform>();
                    _Img  = transform.GetChild(0).GetComponent<Image>();
                    //プレイヤーの子オブジェクトの情報を取得
                    _PlayerArrow = transform.GetChild(1);

                    //プレイヤーの初期角度
                    int index = gNOW_STAGE_LEVEL - 1;
                    //インデクスが無い場合は下向きにする
                    if (index >= sGamePreference._Player_Angle.Length)
                        transform.eulerAngles = new Vector3(0, 0, 180);
                    else
                    {
                        float angle = 0.0f;
                        switch (sGamePreference._Player_Angle[index])
                        {
                            case Dir_ID.RIGHT:
                                angle = 270.0f;
                                break;
                            case Dir_ID.LEFT:
                                angle = 90.0f;
                                break;
                            case Dir_ID.UP:
                                angle = 0.0f;
                                break;
                            case Dir_ID.DOWN:
                                angle = 180.0f;
                                break;
                        }
                        transform.eulerAngles = new Vector3(0, 0, angle);
                    }
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
                    //初期方向設定
                    if (_IsStart_Up_Right)
                        _PhaseOffset = 0.0f;
                    else
                        _PhaseOffset = Mathf.PI;
                    break;
                }
            case Obj_ID.SHARK:
                {
                    //画像設定
                    sImageManager.Change_Image(_Img, sImageManager._Shark_img[(int)Dir_ID.RIGHT]);
                    //初期方向設定
                    if (_IsStart_Up_Right)
                        _PhaseOffset = 0.0f;
                    else
                        _PhaseOffset = Mathf.PI;
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

    #region プレイヤー関係 ------------------------------------------------------------------------------------------------

    /// <summary>
    /// プレイヤーの処理
    /// </summary>
    private void Player_Action()
    {
        if(_PlayerState == Player_State.TIME_OVER)
        {
            //死亡時アニメーション
            Normal_Animation(_Img, sImageManager._Player_Dead_img, sGamePreference._Player_Anim_Cnt);
            return;
        }

        //プレイヤーの角度変更
        Player_Angle_Change();
        //プレイヤーのアニメーション変更処理
        Player_Animation(_Img);

        switch (sGameManager._GoalMoveState)
        {
            case GoalMoveState.INVISIBLE:
            case GoalMoveState.DISPLAY_READY:
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
                    {
                        sGameManager._GoalMoveState = GoalMoveState.END;
                    }

                    break;
                }
            case GoalMoveState.END:
                {
                    gNOW_GAMESTATE = GameState.GAMECLEAR;
                    sGameManager.Delete_Obj(gameObject);
                    break;
                }
        }
    }

    /// <summary>
    /// タイムオーバーした時のプレイヤーの設定
    /// </summary>
    public void TimeOver_Player_Setting()
    {
        _Anim_cnt = 0;
        _Anim_index = 0;

        _PlayerState = Player_State.TIME_OVER;
        //プレイヤーの矢印非表示
        sImageManager.Change_Active(_PlayerArrow.gameObject, false);
    }

    /// <summary>
    /// プレイヤーの移動処理
    /// </summary>
    private void Player_Move()
    {
        //プレイ中以外は終了
        if (gNOW_GAMESTATE != GameState.PLAYING ||
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
        //プレイヤーのZ座標に合わせる
        tap_pos.z = transform.position.z;

        //タッチ座標からプレイヤーまでの方向を計算
        _Dir = tap_pos - transform.position;
        _Dir = _Dir.normalized; //正規化

        //プレイヤーとタッチ座標の距離を計算
        float distance = Vector3.Distance(tap_pos, transform.position);

        //距離が0.1以下の場合は処理終了
        if (distance < 0.1f)
        {
            sClickManager.Player_Near();
            return;
        }

        //目的の角度と現在の角度
        float target_angle = Mathf.Atan2(_Dir.y, _Dir.x) * Mathf.Rad2Deg - 90f;
        float current_angle = transform.eulerAngles.z;

        // 角度差を取得
        float angle_diff = Mathf.DeltaAngle(current_angle, target_angle);

        // 角度差が閾値以下なら滑らかに補間
        if (Mathf.Abs(angle_diff) < 90f)
        {
            float angle = Mathf.MoveTowardsAngle(current_angle, target_angle, sGamePreference._Player_RotSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        else
        {
            // 角度差が大きいなら即ターン
            transform.rotation = Quaternion.Euler(0f, 0f, target_angle);
            _isFront = true;
        }

        // 浮力で速度補正
        float speed_modifier = 1f;
        if (_Dir.y < 0) speed_modifier = sGamePreference._Downward_SpeedModifier;

        _Rigid2D.velocity = transform.up * sGamePreference._Player_MoveSpeed * speed_modifier;
    }

    /// <summary>
    /// プレイヤーアニメーション画像切り替え
    /// </summary>
    /// <param name="target_img">Imageの情報</param>
    /// <param name="change_img">ループさせる画像配列</param>
    private void Player_Animation(Image target_img)
    {
        if (!_isFront && _Anim_cnt < sGamePreference._Player_Anim_Cnt)
        {
            _Anim_cnt++;
            return;
        }

        //角度調整用
        float add_angle = 0.0f;

        if (_isFront)
        {
            sImageManager.Change_Image(_Img, sImageManager._Player_Front_img);  //画像変更
            //角度調整
            switch (_PlayerDir)
            {
                case Dir_ID.UP:
                case Dir_ID.DOWN:
                    add_angle = 0.0f;
                    break;
                case Dir_ID.RIGHT:
                    add_angle = 90.0f;
                    break;
                case Dir_ID.LEFT:
                    add_angle = 270.0f;
                    break;
            }
            _Rect.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + add_angle);

            _isFront = false;
            _Anim_cnt = 0;
            return;
        }

        //一定間隔未満は終了
        if (_Anim_cnt < sGamePreference._Player_Anim_Cnt)
            return;

        switch (_PlayerDir)
        {
            case Dir_ID.UP:
                sImageManager.Change_Image(target_img, sImageManager._Player_Up_img[PLAYER_ANIM_LOOP[_Anim_index]]);    //画像変更
                add_angle = 0.0f;
                break;
            case Dir_ID.DOWN:
                sImageManager.Change_Image(target_img, sImageManager._Player_Down_img[PLAYER_ANIM_LOOP[_Anim_index]]);  //画像変更
                add_angle = 180.0f;
                break;
            case Dir_ID.RIGHT:
                sImageManager.Change_Image(target_img, sImageManager._Player_Right_img[PLAYER_ANIM_LOOP[_Anim_index]]); //画像変更
                add_angle = 90.0f;
                break;
            case Dir_ID.LEFT:
                sImageManager.Change_Image(target_img, sImageManager._Player_Left_img[PLAYER_ANIM_LOOP[_Anim_index]]);  //画像変更
                add_angle = -90.0f;
                break;
        }

        //角度調整
        _Rect.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + add_angle);

        //インデクス設定
        if (_Anim_index < 3)
            _Anim_index++;
        else
            _Anim_index = 0;

        _Anim_cnt = 0;
    }

    /// <summary>
    /// プレイヤーの角度ID変更処理
    /// </summary>
    private void Player_Angle_Change()
    {
        //プレイヤーの角度を取得 : 0～360度
        float angle = transform.eulerAngles.z;

        //上
        if ((angle >= 345f && angle < 360f) || (angle >= 0f && angle < 15f))
        {
            _PlayerDir = Dir_ID.UP;
        }
        //左
        else if (angle >= 15f && angle < 165f)
        {
            _PlayerDir = Dir_ID.LEFT;
        }
        //下
        else if (angle >= 165f && angle < 195f)
        {
            _PlayerDir = Dir_ID.DOWN;
        }
        //右
        else if (angle >= 195f && angle < 345f)
        {
            _PlayerDir = Dir_ID.RIGHT;
        }
    }

    #endregion ------------------------------------------------------------------------------------------------------------

    #region プレイヤー以外のオブジェクト関係 ------------------------------------------------------------------------------

    /// <summary>
    /// 機雷の処理
    /// </summary>
    private void Mine_Action()
    {
        //移動処理
        Mine_Move();
        switch (_Obj_MineState)
        {
            case Mine_State.EXPLOSION:
                {
                    //爆発アニメーション
                    if (Short_Animation(_Img, sImageManager._Mine_Anim_img, 10))
                        _Obj_MineState = Mine_State.DELETE;
                    break;
                }
            case Mine_State.DELETE:
                {
                    //ゲームオーバー
                    gNOW_GAMESTATE = GameState.GAMEOVER;
                    //削除
                    sGameManager.Delete_Obj(gameObject);
                    break;
                }
        }
    }

    /// <summary>
    /// 機雷の移動処理
    /// </summary>
    private void Mine_Move()
    {
        if (_Obj_MineState == Mine_State.READY)
        {
            //Mathf.Sinはサイン波で -1～ 1 の値を返す
            _Dir.y = Mathf.Sin(Time.time * sGamePreference._Mine_MoveSpeed + _PhaseOffset);
            //返された値を (移動幅)_MoveRange で拡大する
            _Dir.y *= _MoveRange;
        }

        //X,Zはそのままで、Y だけ上下に移動させる
        transform.position = new Vector3(_Start_pos.x, _Start_pos.y + _Dir.y, _Start_pos.z);
    }

    /// <summary>
    /// サメの移動処理
    /// </summary>
    private void Shark_Move()
    {
        //Mathf.Sinはサイン波で -1～ 1 の値を返す
        float current_sin = Mathf.Sin(Time.time * sGamePreference._Shark_MoveSpeed + _PhaseOffset);
        //返された値を (移動幅)_MoveRange で拡大する
        float new_x = current_sin * _MoveRange;

        //Y,Zはそのままで、X だけ上下に移動させる
        transform.position = new Vector3(_Start_pos.x + new_x, _Start_pos.y, _Start_pos.z);

        //勾配(差分)
        float diff = current_sin - _Keep_sin;

        if (diff > 0)
        {
            // 右に動いている
            if (_Dir.x != 1.0f)
            {
                _Dir.x = 1.0f;
                sImageManager.Change_Image(_Img, sImageManager._Shark_img[(int)Dir_ID.RIGHT]);
            }
        }
        else if (diff < 0)
        {
            // 左に動いている
            if (_Dir.x != -1.0f)
            {
                _Dir.x = -1.0f;
                sImageManager.Change_Image(_Img, sImageManager._Shark_img[(int)Dir_ID.LEFT]);
            }
        }
        _Keep_sin = current_sin;
    }

    /// <summary>
    /// ゴールの矢印の処理
    /// </summary>
    private void GoalArrow_Action()
    {
        if (_Obj_GoalState == GoalMoveState.DISPLAY)
        {
            //不透明にする
            sImageManager.Change_Alpha(_Img, 1.0f);

            if(sGameManager._GoalMoveState == GoalMoveState.DISPLAY)
                sGameManager._GoalMoveState = GoalMoveState.DISPLAY_READY;

            _Obj_GoalState = GoalMoveState.DISPLAY_READY;
        }
    }

    #endregion ------------------------------------------------------------------------------------------------------------

    #region 当たり判定 ----------------------------------------------------------------------------------------------------

    /// <summary>
    /// Trigger コライダーでの衝突判定「すり抜け判定」
    /// </summary>
    /// <param name="collision">衝突した相手の Collider2D</param>
    void OnTriggerEnter2D(Collider2D collision)
    {
        //プレイヤー以外の場合は終了
        if (_Obj_ID != Obj_ID.PLAYER)
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
                    sMusicManager.SE_Play(SE_ID.TREASURE_GET); //SE再生
                    break;
                }
            case Obj_ID.GOAL:
                {
                    if (sGameManager._GoalMoveState != GoalMoveState.DISPLAY_READY)
                        break;

                    sMusicManager.SE_Play_BGM_Stop(SE_ID.GOAL_IN); //SE再生 : BGM停止
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
                    //プレイヤーがゴールインした場合は終了
                    if (sGameManager._GoalMoveState == GoalMoveState.PLAYER_IN)
                        break;

                    //ノックバック処理
                    KnockBack(collision, sGamePreference._Player_KnockBackSpeed);

                    if (_PlayerState == Player_State.PLAY && gNOW_GAMESTATE == GameState.PLAYING)
                    {
                        //ヒットマーク生成
                        sGameManager.Create_HitMark(collision.gameObject.transform.position);

                        //空気ゲージを減少
                        sGameManager.Dec_AirGage_Timer(sGamePreference._Rock_DecAirGage);
                        sMusicManager.SE_Play(SE_ID.ROCK_HIT); //SE再生
                        _PlayerState = Player_State.NO_OPERATION; //操作不可にする
                    }
                    break;
                }
            //機雷
            case Obj_ID.MINE:
                {
                    if (gNOW_GAMESTATE != GameState.PLAYING)
                        break;

                    //機雷爆発フェーズ
                    collision.transform.GetComponent<Obj_Cave_Dive>()._Obj_MineState = Mine_State.EXPLOSION;
                    sMusicManager.SE_Play_BGM_Stop(SE_ID.EXPLOSION); //SE再生
                    //プレイヤー削除
                    sGameManager.Delete_Obj(gameObject);
                    break;
                }
            //サメ
            case Obj_ID.SHARK:
                {
                    //ノックバック処理
                    KnockBack(collision, sGamePreference._Player_KnockBackSpeed * 3.0f);

                    if (_PlayerState == Player_State.PLAY && gNOW_GAMESTATE == GameState.PLAYING)
                    {
                        //空気ゲージを減少
                        sGameManager.Dec_AirGage_Timer(sGamePreference._Shark_DecAirGage);
                        sMusicManager.SE_Play(SE_ID.SHARK_HIT); //SE再生
                        _PlayerState = Player_State.NO_OPERATION; //操作不可にする
                    }
                    break;
                }
            //ゴールを塞ぐ障害物
            case Obj_ID.OBSTACLE:
                {
                    //ノックバック処理
                    KnockBack(collision, sGamePreference._Player_KnockBackSpeed);

                    if (_PlayerState == Player_State.PLAY && gNOW_GAMESTATE == GameState.PLAYING)
                        _PlayerState = Player_State.NO_OPERATION; //操作不可にする

                    break;
                }
        }
    }

    #endregion ------------------------------------------------------------------------------------------------------------

    #region 汎用系 --------------------------------------------------------------------------------------------------------

    /// <summary>
    /// ノックバック処理
    /// </summary>
    /// <param name="collision">衝突情報をまとめた Collision2D 型</param>
    /// <param name="knockback_speed">ノックバックの速度</param>
    private void KnockBack(Collision2D collision, float knockback_speed)
    {
        //ぶつかった面の法線(プレイヤーを押し返す方向)
        Vector2 normal = collision.contacts[0].normal;

        //速度リセット
        _Rigid2D.velocity = Vector2.zero;

        //法線方向にノックバック
        _Rigid2D.velocity = normal * knockback_speed;
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

    /// <summary>
    /// 常時アニメーション処理
    /// </summary>
    /// <param name="target_img">対象の画像オブジェクト</param>
    /// <param name="change_img">アニメーションするSprite配列</param>
    private void Normal_Animation(Image target_img, Sprite[] change_img, int anim_change_frame)
    {
        _Anim_cnt++;
        if (_Anim_cnt > anim_change_frame)
        {
            //画像変更
            sImageManager.Change_Image(target_img, change_img[_Anim_index]);

            //インデクス設定
            if (_Anim_index < change_img.Length - 1)
                _Anim_index++;
            else
                _Anim_index = 0;
            _Anim_cnt = 0;
        }
    }

    /// <summary>
    /// 一回だけ再生されるアニメーション処理
    /// </summary>
    /// <param name="target_img"></param>
    /// <param name="change_img"></param>
    private bool Short_Animation(Image target_img, Sprite[] change_img, int anim_change_frame)
    {
        _Anim_cnt++;
        if (_Anim_cnt > anim_change_frame)
        {
            //画像変更
            sImageManager.Change_Image(target_img, change_img[_Anim_index]);

            //インデクス設定
            if (_Anim_index < change_img.Length - 1)
                _Anim_index++;
            else
            {
                return true;
            }
            _Anim_cnt = 0;
        }
        return false;
    }


    #endregion ------------------------------------------------------------------------------------------------------------

}
