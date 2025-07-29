using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Common_Cave_Dive;
using UnityEditor.Purchasing;

public class Game_Manager_Cave_Dive : MonoBehaviour
{
    [Header("ゲームオーバー時,ゲームクリア時に表示するオブジェクト")]
    [SerializeField] private GameObject _GameOver_obj;
    [SerializeField] private GameObject _GameClear_obj;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //ゲーム画面以外の場合は終了
        if (GrovalNum_CaveDive.gNOW_SCREEN_ID != GrovalConst_CaveDive.Screen_ID.GAME)
            return;

        switch (GrovalNum_CaveDive.gNOW_GAMESTATE)
        {
            case GrovalConst_CaveDive.GameState.READY:
                {
                    break;
                }
            case GrovalConst_CaveDive.GameState.CREATE_STAGE:
                {
                    break;
                }
            case GrovalConst_CaveDive.GameState.PLAYING:
                {
                    break;
                }
            case GrovalConst_CaveDive.GameState.GAMECLEAR:
                {
                    //ゲームクリア用オブジェクトの表示
                    GrovalNum_CaveDive.sImageManager.Change_Active(_GameClear_obj, true);
                    break;
                }
            case GrovalConst_CaveDive.GameState.GAMEOVER:
                {
                    //ゲームオーバー用オブジェクトの表示
                    GrovalNum_CaveDive.sImageManager.Change_Active( _GameOver_obj, true);
                    GrovalNum_CaveDive.gNOW_GAMESTATE = GrovalConst_CaveDive.GameState.READY;
                    break;
                }
        }
    }

    /// <summary>
    /// ステージリセット処理
    /// </summary>
    public void Reset_Stage()
    {
        //ゲームクリア、ゲームオーバー用のオブジェクトの非表示
        GrovalNum_CaveDive.sImageManager.Change_Active(_GameClear_obj, false);
        GrovalNum_CaveDive.sImageManager.Change_Active(_GameOver_obj, false);
    }
}
