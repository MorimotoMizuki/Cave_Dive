using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Common_Cave_Dive
{ 
    /// <summary>
    /// ���ʒ萔
    /// </summary>
    public static class GrovalConst_CaveDive
    {
        /// <summary>
        /// �Q�[���̏��
        /// </summary>
        public enum GameState
        {
            READY,          //�ҋ@
            CREATE_STAGE,   //�X�e�[�W����
            PLAYING,        //�Q�[���v���C
            GAMECLEAR,      //�Q�[���N���A
            GAMEOVER,       //�Q�[���I�[�o�[
        }

        /// <summary>
        /// ��ʂ�ID
        /// </summary>
        public enum Screen_ID
        {
            TITLE,  //�^�C�g�����
            GAME,   //�Q�[�����
            CLEAR,  //�N���A���
            FADE,   //�t�F�[�h���
            NONE,
        }

        /// <summary>
        /// BGM��ID
        /// </summary>
        public enum BGM_ID
        {
            TITLE,
            GAME,
        }

        public enum SE_ID
        {
            CLEAR,          //�N���A��
            OVER,           //�Q�[���I�[�o�[��
            TAP,            //�^�b�v��
            PLAYER_MOVE,    //�v���C���[�ړ���
        }
    }

    /// <summary>
    /// ���ʕϐ�
    /// </summary>
    public static class GrovalNum_CaveDive
    {
        //���݂̉��ID
        public static GrovalConst_CaveDive.Screen_ID gNOW_SCREEN_ID = GrovalConst_CaveDive.Screen_ID.TITLE;

        //���݂̃t�F�[�Y���
        public static GrovalConst_CaveDive.GameState gNOW_GAMESTATE = GrovalConst_CaveDive.GameState.READY;

        //���݂̃X�e�[�W���x��
        public static int gNOW_STAGE_LEVEL = 1;

        //�e�X�N���v�g
        public static Game_Manager_Cave_Dive    sGameManager;
        public static Click_Manager_Cave_Dive   sClickManager;
        public static Game_Preference_Cave_Dive sGamePreference;
        public static Music_Manager_Cave_Dive   sMusicManager;
        public static Screen_Change_Cave_Dive   sScreenChange;
    }
}
