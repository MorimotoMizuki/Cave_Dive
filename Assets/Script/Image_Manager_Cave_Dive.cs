using UnityEngine;
using UnityEngine.UI;

public class Image_Manager_Cave_Dive : MonoBehaviour
{
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
