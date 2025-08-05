//================================================================
// @file   :chara.js
// @author : KazuteruSonoyama
// @date   :2020.10.18 新規作成
//================================================================



//================================================================
//
// 変数の宣言
//
//================================================================
const CHARA_W = 64;
const CHARA_H = 96;
const SCREEN_W = 720;
const SCREEN_H = 540;
const PATTERN = [0, 1, 0, 2];

//================================================================
//
// メイン処理
//
//================================================================


	//========================================================
	// キャラクラス
	// @author :KazuteruSonoyama
	// @date   :2020.10.18 新規作成
	//========================================================
	function Character(){
		//================================================
		// コンストラクタ
		// @author :KazuteruSonoyama
		// @date   :2020.10.18 新規作成
		//================================================
		this.g_image  =new Image();			//描画イメージ
		var padding = "";
		this.g_image.src  ="./img/testchr_tex.png";

		//メソッド
		this.chara_draw=chara_draw;			//描画

	}

	//================================================
	// 描画
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2020.10.18 新規作成
	//================================================

	function chara_draw(context,direction,frame){

//		var sx = ((direction*3)%6 +  Math.floor(frame/5)%4)*CHARA_W;
		var sx = ((direction*3)%6 +  PATTERN[Math.floor(frame/5)%4])*CHARA_W;
//		var sy = Math.floor((direction*3)/6)*CHARA_H;  
		var sy = Math.floor(direction/2)*CHARA_H;  

		context.drawImage(
							this.g_image,
							sx,
							sy,
							CHARA_W,
							CHARA_H,
							(SCREEN_W-CHARA_W)/2,
							(SCREEN_H-CHARA_H)/2,
							CHARA_W,
							CHARA_H);

	}
