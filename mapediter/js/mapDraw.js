//================================================================
// @file   :mapDraw.js
// @author : KazuteruSonoyama
// @date   :2020.10.20 新規作成
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
	// マップクラス
	// @author :KazuteruSonoyama
	// @date   :2020.10.20 新規作成
	//========================================================
	function MapDraw(){
		//================================================
		// コンストラクタ
		// @author :KazuteruSonoyama
		// @date   :2020.10.20 新規作成
		//================================================
/*
		this.g_image  =new Image();			//描画イメージ
//		this.g_image.src  ="./img/blockchip.png";
		this.g_image.src  ="./img/mapchip.png";
*/
		this.g_mapdata = null;
		this.g_bankno = 0;

		//メソッド
		this.map_draw=map_draw;			//描画
//		this.map_check=map_check;			//チェック

		this.setMapData=setMapData;			//マップセット

		this.whole_map_draw=whole_map_draw;
	}

	//================================================
	// 描画
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2020.10.18 新規作成
	//================================================

	function map_draw(context,map_x,map_y,direction,moveCount){ 
		var loop_v = Math.floor(CANVAS_H/g_chipDisplayH)+4;
		var loop_h = Math.floor(CANVAS_W/g_chipDisplayW)+3;
		if (this.g_mapdata==null) return;
		for (var v = 0;v <loop_v;v++) {

			var v_index = map_y - Math.floor(loop_v/2)+v;
			if (g_map_loop_sw){
				v_index = (v_index+g_mapH)%g_mapH;
			}
			if (v_index < 0 || v_index>=g_mapH) continue;

	   		for (var h = 0;h < loop_h;h++) {
				var h_index = map_x -Math.floor(loop_h/2)+h;
				if (g_map_loop_sw){
					h_index = (h_index+g_mapW)%g_mapW;
				}
				if (h_index < 0 || h_index>=g_mapW) continue;
	
				var index = (h_index + v_index * g_mapW);
				var ofs_x=0,ofs_y=0;

				if (moveCount>0){
					switch(direction)
					{
						case 0:
							// 下
							ofs_y = Math.floor(((4-moveCount)*g_chipDisplayH)/(-4));
							break;

						case 1:
							// 上
							ofs_y = Math.floor(((4-moveCount)*g_chipDisplayH)/(4));
							break;

						case 2:
							// 左
							ofs_x = Math.floor(((4-moveCount)*g_chipDisplayW)/(4));
							break;

						case 3:
							// 右
							ofs_x = Math.floor(((4-moveCount)*g_chipDisplayW)/(-4));
							break;

						case 4:
							// 左下
							ofs_x = Math.floor(((4-moveCount)*g_chipDisplayW)/(4));
							ofs_y = Math.floor(((4-moveCount)*g_chipDisplayH)/(-4));
							break;

						case 5:
							// 右下
							ofs_x = Math.floor(((4-moveCount)*g_chipDisplayW)/(-4));
							ofs_y = Math.floor(((4-moveCount)*g_chipDisplayH)/(-4));
							break;

						case 6:
							// 左上
							ofs_x = Math.floor(((4-moveCount)*g_chipDisplayW)/(4));
							ofs_y = Math.floor(((4-moveCount)*g_chipDisplayH)/(4));
							break;

						case 7:
							// 右上
							ofs_x = Math.floor(((4-moveCount)*g_chipDisplayW)/(-4));
							ofs_y = Math.floor(((4-moveCount)*g_chipDisplayH)/(4));
							break;

					}
				}

				switch(g_chipDisplayH)
				{
					case 16:
						// 16x16
						ofs_x -=34;
						ofs_y -=28;
						break;
				
					case 32:
						// 32x32
						ofs_x -=42;
						ofs_y -=68;
						break;

					case 64:
						// 64x64
						ofs_x -=122;
						ofs_y -=148;
						break;
				}

				var x = h*g_chipDisplayW +ofs_x;
				var y = v*g_chipDisplayH +ofs_y;

				context.globalAlpha = 1.0;

				drawMapImage(context,g_chipset[this.g_bankno].g_image,this.g_mapdata[index],x,y,g_chipDisplayW,g_chipDisplayH);
			}
		}

	}
/*
	function map_check(map_x,map_y,direction)
	{

		var retdir = direction;
		var base_x = Math.floor(map_x/2);
		var base_y = Math.floor(map_y/2);
		var x = base_x;
		var y = base_y;
		switch(direction)
		{
			case 0:
				// 下
				y=(base_y+1)%g_mapH;
				if (map_y%2==0) y=base_y;
				if (this.g_mapdata[x+y*g_mapW]==0){
					retdir = -1;
				}
				if (map_x%2==0) x=(base_x-1+g_mapW)%g_mapW;
				if (this.g_mapdata[x+y*g_mapW]==0){
					retdir = -1;
				}
				break;

			case 1:
				// 上
				y=(base_y-1+g_mapH)%g_mapH;
				if (this.g_mapdata[x+y*g_mapW]==0){
					retdir = -1;
				}
				if (map_x%2==0) x=(base_x-1+g_mapW)%g_mapW;
				if (this.g_mapdata[x+y*g_mapW]==0){
					retdir = -1;
				}
				break;

			case 2:
				// 左
				x=(base_x-1+g_mapW)%g_mapW;
				if (this.g_mapdata[x+y*g_mapW]==0){
					retdir = -1;
				}
				if (map_y%2==0) y=(base_y-1+g_mapH)%g_mapH;
				if (this.g_mapdata[x+y*g_mapW]==0){
					retdir = -1;
				}
				break;

			case 3:
				// 右
				x=(base_x+1)%g_mapW;
				if (map_x%2==0) x=base_x;
				if (this.g_mapdata[x+y*g_mapW]==0){
					retdir = -1;
				}
				if (map_y%2==0) y=(base_y-1+g_mapH)%g_mapH;
				if (this.g_mapdata[x+y*g_mapW]==0){
					retdir = -1;
				}
				break;

			case 4:
				// 左下
				y=(base_y+1)%g_mapH;
				if (map_y%2==0) y=base_y;
				x=(base_x-1+g_mapW)%g_mapW;
				if (this.g_mapdata[x+y*g_mapW]==0){					
					retdir = -1;
				}
				x=base_x;
				if (this.g_mapdata[x+y*g_mapW]==0){					
					retdir = -1;
				}
				x=(base_x-1+g_mapW)%g_mapW;
				y=base_y;
				if (this.g_mapdata[x+y*g_mapW]==0){
					retdir = -1;
				}
				break;

			case 5:
				// 右下
				y=(base_y+1)%g_mapH;
				if (map_y%2==0) y=base_y;
				x=(base_x+1)%g_mapW;
				if (map_x%2==0) x=base_x;
				if (this.g_mapdata[x+y*g_mapW]==0){					
					retdir = -1;
				}
				x=base_x;
				if (this.g_mapdata[x+y*g_mapW]==0){					
					retdir = -1;
				}
				x=(base_x+1)%g_mapW;
				if (map_x%2==0) x=base_x;
				y=base_y;
				if (this.g_mapdata[x+y*g_mapW]==0){
					retdir = -1;
				}
				break;

			case 6:
				// 左上
				y=(base_y-1+g_mapH)%g_mapH;
				x=(base_x-1+g_mapW)%g_mapW;
				if (this.g_mapdata[x+y*g_mapW]==0){					
					retdir = -1;
				}
				x=base_x;
				if (this.g_mapdata[x+y*g_mapW]==0){					
					retdir = -1;
				}
				x=(base_x-1+g_mapW)%g_mapW;
				y=base_y;
				if (this.g_mapdata[x+y*g_mapW]==0){
					retdir = -1;
				}
				break;

			case 7:
				// 右上
				y=(base_y-1+g_mapH)%g_mapH;
				x=(base_x+1)%g_mapW;
				if (map_x%2==0) x=base_x;
				if (this.g_mapdata[x+y*g_mapW]==0){					
					retdir = -1;
				}
				x=base_x;
				if (this.g_mapdata[x+y*g_mapW]==0){					
					retdir = -1;
				}
				x=(base_x+1)%g_mapW;
				if (map_x%2==0) x=base_x;
				y=base_y;
				if (this.g_mapdata[x+y*g_mapW]==0){
					retdir = -1;
				}
				break;
		}
		return retdir;
	}
*/

	//================================================
	// 全体描画
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.18 新規作成
	//================================================

	function whole_map_draw(context,size){ 
		if (this.g_mapdata==null) return;
		for (var v = 0;v < g_mapH;v++) {
	   		for (var h = 0;h < g_mapW;h++) {
				var index = v * g_mapW + h;
				var x=h*4;
				var y=v*4;
				drawMapImage(context,g_chipset[this.g_bankno].g_image,this.g_mapdata[index],x,y,size,size);
			}
		}

	}

	//================================================
	// マップチップ描画
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.18 新規作成
	//================================================

	function drawMapImage(context,image,chip_idx,x,y,chip_w,chip_h)
	{
		if (image.src==null) return;
		var column_max = Math.floor(image.width / g_chipW);
		var tex_x = (chip_idx % column_max) * g_chipW;
		var tex_y = Math.floor(chip_idx / column_max) * g_chipH;
	
		context.drawImage(image,tex_x,tex_y,g_chipW,g_chipH,x,y,chip_w,chip_h);

	}

	//================================================
	// マップデータの書き換え
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.1.08 新規作成
	//================================================
	function setMapData(x,y,chip_idx)
	{
		var index = (y * g_mapW) + x;
		this.g_mapdata[index] = chip_idx;
    }
