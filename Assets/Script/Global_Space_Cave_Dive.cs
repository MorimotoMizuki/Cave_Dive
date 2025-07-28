using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Common_Cave_Dive
{ 
    /// <summary>
    /// 共通定数
    /// </summary>
    public static class GrovalConst_CaveDive
    {
        /// <summary>
        /// ゲームの状態
        /// </summary>
        public enum GameState
        {
            READY,          //待機
            CREATE_STAGE,   //ステージ生成
            PLAYING,        //ゲームプレイ
            GAMECLEAR,      //ゲームクリア
            GAMEOVER,       //ゲームオーバー
        }

        /// <summary>
        /// 画面のID
        /// </summary>
        public enum Screen_ID
        {
            TITLE,  //タイトル画面
            GAME,   //ゲーム画面
            CLEAR,  //クリア画面
            FADE,   //フェード画面
            NONE,
        }

        /// <summary>
        /// BGMのID
        /// </summary>
        public enum BGM_ID
        {
            TITLE,
            GAME,
        }

        public enum SE_ID
        {
            CLEAR,          //クリア時
            OVER,           //ゲームオーバー時
            TAP,            //タップ時
            PLAYER_MOVE,    //プレイヤー移動時
        }
    }

    /// <summary>
    /// 共通変数
    /// </summary>
    public static class GrovalNum_CaveDive
    {
        //現在の画面ID
        public static GrovalConst_CaveDive.Screen_ID gNOW_SCREEN_ID = GrovalConst_CaveDive.Screen_ID.TITLE;

        //現在のフェーズ状態
        public static GrovalConst_CaveDive.GameState gNOW_GAMESTATE = GrovalConst_CaveDive.GameState.READY;

        //現在のステージレベル
        public static int gNOW_STAGE_LEVEL = 1;

        //各スクリプト
        public static Game_Manager_Cave_Dive    sGameManager;
        public static Click_Manager_Cave_Dive   sClickManager;
        public static Game_Preference_Cave_Dive sGamePreference;
        public static Music_Manager_Cave_Dive   sMusicManager;
        public static Screen_Change_Cave_Dive   sScreenChange;
    }
}
