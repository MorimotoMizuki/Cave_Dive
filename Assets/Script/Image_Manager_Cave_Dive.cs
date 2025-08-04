using UnityEngine;
using UnityEngine.UI;

public class Image_Manager_Cave_Dive : MonoBehaviour
{
    [Header("ステージの背景の画像, 画像オブジェクト")]
    public Sprite[] _BackGround_img;
    public Image[]  _BackGround_obj;

    [Header("マスク画像, 画像オブジェクト")]
    public Sprite _Mask_img;
    public Image  _Mask_obj;

    [Header("空気ゲージ、ダメージゲージ画像オブジェクト")]
    public Image _AirGage_Fill;
    public Image _Damage_Gage_Fill;

    [Header("サメの画像 (左右の順番でお願いします)")]
    public Sprite[] _Shark_img;
    [Header("機雷の画像")]
    public Sprite _Mine_img;
    [Header("財宝の画像")]
    public Sprite _Treasure_img;

    [Header("ゴールの矢印画像")]
    public Sprite _GoalArrow_img;

    [Header("プレイヤーの画像")]
    public Sprite[] _Player_Right_img;
    public Sprite[] _Player_Left_img;
    public Sprite[] _Player_Up_img;
    public Sprite[] _Player_Down_img;
    public Sprite _Player_Front_img;

    /// <summary>
    /// オブジェクトの表示切り替え
    /// </summary>
    /// <param name="target_obj">表示を切り替えるオブジェクト</param>
    /// <param name="is_active">true : 表示, false : 非表示</param>
    public void Change_Active(GameObject target_obj, bool is_active)
    {
        target_obj.SetActive(is_active);
    }

    /// <summary>
    /// 画像変更
    /// </summary>
    /// <param name="change_img_obj">変更元オブジェクト</param>
    /// <param name="target_img">変更先の画像</param>
    public void Change_Image(Image change_img_obj, Sprite target_img)
    {
        //変更元のオブジェクトが無い場合は終了
        if (change_img_obj == null) return;

        //画像変更
        change_img_obj.sprite = target_img;
    }

    /// <summary>
    /// 画像のアルファ値変更
    /// </summary>
    /// <param name="change_img_obj">変更元オブジェクト</param>
    /// <param name="alpha">アルファ値</param>
    public void Change_Alpha(Image change_img_obj, float alpha)
    {
        //変更元のオブジェクトが無い場合は終了
        if (change_img_obj == null) return;

        //アルファ値を変更
        Color color = change_img_obj.color;
        color.a = alpha;
        change_img_obj.color = color;
    }

    /// <summary>
    /// 画像のアルファ値を減少させる
    /// </summary>
    /// <param name="change_img_obj">変更元オブジェクト</param>
    /// <param name="dec_alpha">減少させるアルファ値</param>
    public void Decrement_Alpha(Image change_img_obj, float dec_alpha)
    {
        //変更元のオブジェクトが無い場合は終了
        if (change_img_obj == null) return;

        //アルファ値を変更
        Color color = change_img_obj.color;
        color.a = Mathf.Clamp01(color.a - dec_alpha); //0未満にならないようにする
        change_img_obj.color = color;
    }

}
