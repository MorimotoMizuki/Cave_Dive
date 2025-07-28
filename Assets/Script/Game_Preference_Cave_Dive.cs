using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Common_Cave_Dive;

public class Game_Preference_Cave_Dive : MonoBehaviour
{
    [Header("各スクリプト")]
    [SerializeField] private Game_Manager_Cave_Dive     game_manager;
    [SerializeField] private Click_Manager_Cave_Dive    click_manager;
    [SerializeField] private Music_Manager_Cave_Dive    music_manager;
    [SerializeField] private Screen_Change_Cave_Dive    screen_change;

    // Start is called before the first frame update
    void Start()
    {
        GrovalNum_CaveDive.sGamePreference  = this;
        GrovalNum_CaveDive.sGameManager     = game_manager;
        GrovalNum_CaveDive.sClickManager    = click_manager;
        GrovalNum_CaveDive.sMusicManager    = music_manager;
        GrovalNum_CaveDive.sScreenChange    = screen_change;

        //60fpsに設定
        Application.targetFrameRate = 60;
    }
}
