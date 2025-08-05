using System.Collections.Generic;
using UnityEngine;

using static Common_Cave_Dive.GrovalConst_CaveDive;

namespace Common_Cave_Dive
{
    /// <summary>
    /// 共通定数
    /// </summary>
    public static class GrovalConst_CaveDive
    {
        public static int[] PLAYER_ANIM_LOOP = new int[4]{ 0, 1, 0, 2 };

        //オブジェクト識別辞書
        public static readonly Dictionary<string, Obj_ID> Name_to_Obj_ID
        = new Dictionary<string, Obj_ID>
        {
            { "PLAYER"      , Obj_ID.PLAYER },
            { "TREASURE"    , Obj_ID.TREASURE },
            { "MINE"       , Obj_ID.MINE },
            { "SHARK"       , Obj_ID.SHARK },
            { "ROCK"        , Obj_ID.ROCK },
            { "GOAL"        , Obj_ID.GOAL },
            { "OBSTACLE"    , Obj_ID.OBSTACLE },
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
            DISPLAY_READY,  //表示して待機
            PLAYER_IN,      //プレイヤーが入った
            END,
        }

        public enum Obj_ID
        {
            NONE,
            PLAYER,     //プレイヤー
            TREASURE,   //財宝
            MINE,      //機雷
            SHARK,      //サメ
            GOAL_ARROW, //ゴール矢印
            GOAL,       //ゴール
            ROCK,       //岩
            OBSTACLE,   //障害物
        }

        public enum Rock_ID
        {
            NONE,
            SQUARE,                 //四角
            LEFTUP_TRIANGLE,        //左上三角
            RIGHTUP_TRIANGLE,       //右上三角
            LEFTDOWN_TRIANGLE,      //左下三角
            RIGHTDOWN_TRIANGLE,     //右下三角
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
            TIMEOVER,       //タイムオーバー時
            TAP,            //タップ時
            ROCK_HIT,       //岩に衝突時
            EXPLOSION,      //機雷爆発時
            SHARK_HIT,      //サメ衝突時
            COUNTDOWN,      //カウントダウン
            TREASURE_GET,   //財宝獲得時
            GOAL_IN,        //ゴールに入った時
        }

        public enum Dir_ID
        {
            RIGHT,
            LEFT,
            UP,
            DOWN,
        }

        /// <summary>
        /// プレイヤーの状態
        /// </summary>
        public enum Player_State
        {
            PLAY,           //通常
            NO_OPERATION,   //操作不可
            TIME_OVER,      //制限時間を超えた
        }

        /// <summary>
        /// 機雷の状態
        /// </summary>
        public enum Mine_State
        {
            READY,      //待機
            EXPLOSION,  //爆発
            DELETE,     //削除
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

            //オブジェクトIDが機雷とサメの場合のみ表示
            [Tooltip("機雷とサメの移動幅")]
            public float move_range;
            [Tooltip("機雷とサメの初期の移動方向\n機雷なら上移動、サメなら右移動")]
            public bool is_start_up_right;
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
        public static int gNOW_STAGE_LEVEL = 5;

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
