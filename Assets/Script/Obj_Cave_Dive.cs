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

    private Obj_ID _Obj_ID = Obj_ID.NONE;

    #endregion ------------------------------------------------------------------------------------------------------------

    //機雷の初期座標
    private Vector3 _Start_pos;

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
        _Obj_ID = Obj_Identification(gameObject.name);

        //オブジェクトの各情報を取得
        _Img = GetComponent<Image>();
        _Rigid2D = GetComponent<Rigidbody2D>();

        switch(_Obj_ID)
        {
            case Obj_ID.PLAYER:
            {
                //摩擦で徐々に減速
                _Rigid2D.drag = sGamePreference._Water_Drag;
                break;
            }
            case Obj_ID.SPIKE:
            {
                //初期座標設定
                _Start_pos = transform.position;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch(_Obj_ID)
        {
            //プレイヤー
            case Obj_ID.PLAYER:
            {
                Player_Move();

                if(_PlayerState == Player_State.NO_OPERATION)
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
            //財宝
            case Obj_ID.TREASURE:
            {
                break;
            }
            //機雷
            case Obj_ID.SPIKE:
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
        Vector3 dir = tap_pos - transform.position;
        dir = dir.normalized; //正規化

        // 回転を滑らかに合わせる
        float target_angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        float current_angle = transform.eulerAngles.z;
        float angle = Mathf.MoveTowardsAngle(current_angle, target_angle, sGamePreference._Player_RotSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // 浮力で速度補正
        float speed_modifier = 1f;
        if (dir.y < 0) speed_modifier = sGamePreference._Downward_SpeedModifier;

        _Rigid2D.velocity = transform.up * sGamePreference._Player_MoveSpeed * speed_modifier;
    }

    /// <summary>
    /// 機雷の移動処理
    /// </summary>
    private void Spike_Move()
    {
        //Mathf.Sinはサイン波で -1～ 1 の値を返す
        float new_y = Mathf.Sin(Time.time * sGamePreference._Spike_MoveSpeed);
        //返された値を (移動幅)_Spike_Amplitude で拡大する
        new_y *= sGamePreference._Spike_Amplitude;

        //X,Zはそのままで、Y だけ上下に移動させる
        transform.position = new Vector3(_Start_pos.x, _Start_pos.y + new_y, _Start_pos.z);
    }

    /// <summary>
    /// サメの移動処理
    /// </summary>
    private void Shark_Move()
    {

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
            case Obj_ID.SPIKE:
            {
                //ゲームオーバー
                gNOW_GAMESTATE = GameState.GAMEOVER;
                break;
            }
            case Obj_ID.SHARK:
            {
                //空気ゲージを減少
                sGameManager.Dec_AirGage_Timer(10);
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
            case Obj_ID.ROCK:
            {
                //ぶつかった面の法線(プレイヤーを押し返す方向)
                Vector2 normal = collision.contacts[0].normal;

                //速度リセット
                _Rigid2D.velocity = Vector2.zero;

                //法線方向にノックバック
                _Rigid2D.velocity = normal * sGamePreference._Player_KnockBackSpeed;

                if (_PlayerState == Player_State.PLAY)
                {
                    //空気ゲージを減少
                    sGameManager.Dec_AirGage_Timer(5);
                    _PlayerState = Player_State.NO_OPERATION;
                }
                break;
            }
        }
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
}
