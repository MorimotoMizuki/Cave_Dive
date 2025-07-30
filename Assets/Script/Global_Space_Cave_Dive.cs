using System.Collections.Generic;
using UnityEngine;

using static Common_Cave_Dive.GrovalConst_CaveDive;
using static Common_Cave_Dive.GrovalNum_CaveDive;
using static Common_Cave_Dive.GrovalStruct_CaveDive;

namespace Common_Cave_Dive
{
    /// <summary>
    /// 共通定数
    /// </summary>
    public static class GrovalConst_CaveDive
    {
        //オブジェクト識別辞書
        public static readonly Dictionary<string, Obj_ID> Name_to_Obj_ID
        = new Dictionary<string, Obj_ID>
        {
            { "PLAYER"      , Obj_ID.PLAYER },
            { "TREASURE"    , Obj_ID.TREASURE },
            { "SPIKE"       , Obj_ID.SPIKE },
            { "SHARK"       , Obj_ID.SHARK },
            { "ROCK"       , Obj_ID.ROCK },
        };

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

        public enum GoalMoveState
        {
            INVISIBLE,      //非表示
            DISPLAY,        //表示
            PLAYER_IN,      //プレイヤーが入った
            PLAYER_UP,      //プレイヤーが上昇
            END,
        }

        public enum Obj_ID
        {
            NONE,
            PLAYER,     //プレイヤー
            TREASURE,   //財宝
            SPIKE,      //機雷
            SHARK,      //サメ
            ROCK,
        }

        public enum Rock_ID
        {
            NONE,
            SQUARE,                 //四角
            RIGHTUP_TRIANGLE,       //右上三角
            LEFTUP_TRIANGLE,        //左上三角
            RIGHTDOWN_TRIANGLE,     //右下三角
            LEFTDOWN_TRIANGLE,      //左下三角
        }

        /// <summary>
        /// 画面のID
        /// </summary>
        public enum Screen_ID
        {
            TITLE,  //タイトル画面
            GAME,   //ゲーム画面
            CLEAR,  //クリア画面
            NONE,
        }

        public enum Button_ID
        {
            START,
            NEXT,
            REPLAY,
            TITLE,
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

    public static class GrovalStruct_CaveDive
    {
        [System.Serializable]
        public struct Character_Data
        {
            //オブジェクトID
            public Obj_ID Obj_ID;

            //座標
            public Vector2 pos;
        }

        [System.Serializable]
        public struct Stage_Data
        {
            //ステージ番号
            public int StageNum;

            //キャラクターのデータ
            public Character_Data[] Chara_Data;
        }
    }

    /// <summary>
    /// 共通変数
    /// </summary>
    public static class GrovalNum_CaveDive
    {
        //現在の画面ID
        public static Screen_ID gNOW_SCREEN_ID = Screen_ID.TITLE;

        //現在のフェーズ状態
        public static GameState gNOW_GAMESTATE = GameState.READY;

        //現在のステージレベル
        public static int gNOW_STAGE_LEVEL = 1;

        //各スクリプト
        public static Game_Manager_Cave_Dive    sGameManager;
        public static Click_Manager_Cave_Dive   sClickManager;
        public static Game_Preference_Cave_Dive sGamePreference;
        public static Music_Manager_Cave_Dive   sMusicManager;
        public static Screen_Change_Cave_Dive   sScreenChange;
        public static Image_Manager_Cave_Dive   sImageManager;
        public static Csv_Roader_Cave_Dive      sCsvRoader;
    }
}
