//================================================================
// @file   :chipset.js
// @author :KazuteruSonoyama
// @date   :2024.04.17 新規作成
//================================================================




//================================================================
//
// ファイル読み込み
//
//================================================================




//================================================================
//
// 変数の宣言
//
//================================================================

//================================================================
//
// メイン処理
//
//================================================================

	//========================================================
	// チップセットクラス
	// @author :KazuteruSonoyama
	// @date   :2025.01.19 新規作成
	//========================================================
	function ChipSet(idx){
		//================================================
		// コンストラクタ
		// @author :KazuteruSonoyama
		// @date   :2025.01.19 新規作成
		//================================================
		this.g_index = idx;
		this.g_image  = new Image();
		this.g_canvas = document.getElementById('canvas_chipset'+idx);	//キャンバスのセット
		if(!this.g_canvas||!this.g_canvas.getContext){
			return false;
		}
		this.g_canvas.width=CHIPSET_CANVAS_W;					//キャンバスの横幅
		this.g_canvas.height=CHIPSET_CANVAS_H;					//キャンバスの縦幅
		this.g_context = this.g_canvas.getContext('2d');				//コンテキスト
		this.g_select_mapchip_no = 0;

		this.chipset_update = chipset_update;
		this.chipset_draw = chipset_draw;
		this.draw_chip_cursor = draw_chip_cursor;
		

	}
	
	//================================================
	// 更新
	// @author :KazuteruSonoyama
	// @date   :2024.04.17 新規作成
	//================================================
	function chipset_update(){

		// game_base.jsのupdateから呼ばれる、
		//メイン描画
		this.chipset_draw();

	}

	//================================================
	// メイン描画
	// @author :KazuteruSonoyama
	// @date   :2024.04.17 新規作成
	//================================================
	function chipset_draw(){
		this.g_context.beginPath();
		this.g_context.fillStyle="rgb(0,0,0)";
		this.g_context.fillRect(0,0,this.g_canvas.width,this.g_canvas.height);

		this.g_context.save();

		if (this.g_image.src!=null){
			if (this.g_canvas.width != this.g_image.width){
				this.g_canvas.width = this.g_image.width;
			}
			if (this.g_canvas.height != this.g_image.height){
				this.g_canvas.height = this.g_image.height;
			}

			this.g_context.drawImage(
				this.g_image,
				0,
				0,
				this.g_image.width,
				this.g_image.height,
				0,
				0,
				this.g_image.width,
				this.g_image.height
			);

			// カーソル
			var column_max = Math.floor(this.g_image.width / g_chipW);
			if (column_max>0){
				var sel_row = Math.floor(this.g_select_mapchip_no / column_max);
				var sel_column = this.g_select_mapchip_no % column_max;
				this.draw_chip_cursor(sel_column*g_chipW,sel_row*g_chipH,g_chipW,g_chipH);
			}
		}

		this.g_context.restore();

	}

	//================================================
	// チップカーソルの描画
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2024.12.02 新規作成
	//================================================
	function draw_chip_cursor(x,y,w,h){
		this.g_context.strokeStyle = '#f00'; //
    	this.g_context.fillStyle = '#fff'; //
    	this.g_context.lineWidth = 1; // 線の幅は1px
		var blink_count = g_count % 30;
		this.g_context.globalAlpha =  (( blink_count > 15 ? 30 - blink_count : blink_count ) / 15)  * 0.5;

		this.g_context.fillRect(x, y, w, h);
		this.g_context.strokeRect(x, y, w, h);

		this.g_context.globalAlpha =  1.0;

    }
