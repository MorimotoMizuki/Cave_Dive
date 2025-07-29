using UnityEngine;
using UnityEngine.UI;

using Common_Cave_Dive;

public class Obj_Cave_Dive : MonoBehaviour
{
    #region 共通 ----------------------------------------------------------------------------------------------------------

    private Image _Img;             //画像情報
    private Rigidbody2D _Rigid2D;   //Rigidbody2D情報
    private Collider2D _Collider2D; //Collider2D情報

    private GrovalConst_CaveDive.Obj_ID _Obj_ID = GrovalConst_CaveDive.Obj_ID.NONE;

    #endregion ------------------------------------------------------------------------------------------------------------

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
            case GrovalConst_CaveDive.Obj_ID.PLAYER:
                //摩擦で徐々に減速
                _Rigid2D.drag = GrovalNum_CaveDive.sGamePreference._Water_Drag;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch(_Obj_ID)
        {
            //プレイヤー
            case GrovalConst_CaveDive.Obj_ID.PLAYER:
                {
                    Player_Move();
                    break;
                }
            //財宝
            case GrovalConst_CaveDive.Obj_ID.TREASURE:
                {
                    break;
                }
            //機雷
            case GrovalConst_CaveDive.Obj_ID.SPIKE:
                {
                    break;
                }
            //サメ
            case GrovalConst_CaveDive.Obj_ID.SHARK:
                {
                    break;
                }
            //岩 : 後々増えるかも
            case GrovalConst_CaveDive.Obj_ID.ROCK:
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
        //タッチしていない場合は終了
        if (!GrovalNum_CaveDive.sClickManager._Is_Touch_or_Click)
            return;

        //タッチしている座標の取得
        Vector3 tap_pos = GrovalNum_CaveDive.sClickManager.GetInputPosition();
        tap_pos = GrovalNum_CaveDive.sGameManager._Camera.ScreenToWorldPoint(tap_pos);
        tap_pos.z = 0.0f;

        //タッチ座標からプレイヤーまでの方向を計算
        Vector3 dir = tap_pos - transform.position;
        dir = dir.normalized; //正規化

        // 回転を滑らかに合わせる
        float target_angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        float current_angle = transform.eulerAngles.z;
        float angle = Mathf.MoveTowardsAngle(current_angle, target_angle, GrovalNum_CaveDive.sGamePreference._Player_RotSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // 浮力で速度補正
        float speed_modifier = 1f;
        if (dir.y < 0) speed_modifier = GrovalNum_CaveDive.sGamePreference._Downward_SpeedModifier;

        _Rigid2D.velocity = transform.up * GrovalNum_CaveDive.sGamePreference._Player_MoveSpeed * speed_modifier;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //プレイヤー以外の場合は終了
        if(_Obj_ID != GrovalConst_CaveDive.Obj_ID.PLAYER) 
            return;

        //衝突したオブジェクトIDを取得
        GrovalConst_CaveDive.Obj_ID collision_obj_id = Obj_Identification(collision.name);

        switch (collision_obj_id)
        {
            case GrovalConst_CaveDive.Obj_ID.TREASURE:
                {
                    //マスク画像のアルファ値を減少させる
                    GrovalNum_CaveDive.sGameManager.Dec_Mask_Alpha();
                    //財宝の削除
                    GrovalNum_CaveDive.sGameManager.Delete_Obj(collision.gameObject);
                    break;
                }
            case GrovalConst_CaveDive.Obj_ID.SPIKE:
                {
                    break;
                }
            case GrovalConst_CaveDive.Obj_ID.SHARK:
                {
                    //空気ゲージを減少
                    GrovalNum_CaveDive.sGameManager.Dec_AirGage_Timer(10);
                    break;
                }
            case GrovalConst_CaveDive.Obj_ID.ROCK:
                {
                    //空気ゲージを減少
                    GrovalNum_CaveDive.sGameManager.Dec_AirGage_Timer(5);
                    break;
                }
        }
    }

    /// <summary>
    /// オブジェクトの識別
    /// </summary>
    /// <returns>オブジェクトID</returns>
    private GrovalConst_CaveDive.Obj_ID Obj_Identification(string obj_name)
    {
        //Name_to_Obj_ID: 文字列（部分一致用）とオブジェクトIDのマッピング辞書
        foreach (var pair in GrovalConst_CaveDive.Name_to_Obj_ID)
        {
            //ゲームオブジェクトの名前に、辞書のキー（判別用文字列）が含まれているか確認
            if (obj_name.Contains(pair.Key))
            {
                //一致した場合、対応するオブジェクトIDを返す
                return pair.Value;
            }
        }

        //どのキーとも一致しない場合、NONEを返す（未識別の意味）
        return GrovalConst_CaveDive.Obj_ID.NONE;
    }

}
