using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Common_Cave_Dive;

public class Click_Manager_Cave_Dive : MonoBehaviour
{
    //ボタンクリック可否フラグ
    [HideInInspector] public bool[] _Is_Button;

    // Start is called before the first frame update
    void Start()
    {
        _Is_Button = new bool[4];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //スタートボタン : ゲーム画面
    public void Button_Clicked_Start()
    {
        _Is_Button[(int)GrovalConst_CaveDive.Button_ID.START] = true;     //ボタンフラグtrue
        //GrovalNum_CaveDive.sMusicManager.SE_Play(GrovalConst_CaveDive.SE_ID.TAP); //SE再生
    }
    //ネクストボタン : クリア画面
    public void Button_Clicked_Next()
    {
        _Is_Button[(int)GrovalConst_CaveDive.Button_ID.NEXT] = true;     //ボタンフラグtrue
        //GrovalNum_CaveDive.sMusicManager.SE_Play(GrovalConst_CaveDive.SE_ID.TAP); //SE再生
    }
    //リプレイボタン : オーバー画面
    public void Button_Clicked_Replay()
    {
        _Is_Button[(int)GrovalConst_CaveDive.Button_ID.REPLAY] = true;     //ボタンフラグtrue
        //GrovalNum_CaveDive.sMusicManager.SE_Play(GrovalConst_CaveDive.SE_ID.TAP); //SE再生
    }
    //タイトルボタン : オーバー画面
    public void Button_Clicked_Title()
    {
        _Is_Button[(int)GrovalConst_CaveDive.Button_ID.TITLE] = true;     //ボタンフラグtrue
        //GrovalNum_CaveDive.sMusicManager.SE_Play(GrovalConst_CaveDive.SE_ID.TAP); //SE再生
    }

}
