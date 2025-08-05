//================================================================
// @file   :game_base.js
// @author :KazuteruSonoyama
// @date   :2020.10.18 新規作成
//================================================================

//================================================================
//
// 変数の宣言
//
//================================================================
//var g_chara;				//キャラ

var g_mapDraw=[];			//マップ描画
var g_view;					//
var g_canvas;				//キャンバス
var g_context;				//コンテキスト
var g_update;				//更新
var g_count;				//フレームカウント
var g_direction;			//移動方向
var g_moveCount;			//移動カウント


var g_keyStatePress=[];		//キー入力取得
var g_keyStateTrig=[];		//キー入力取得
var g_mapY;
var g_mapX;

var g_cursorSize;

var g_mapW;
var g_mapH;

var g_chipW;
var g_chipH;

var g_chipDisplayW;
var g_chipDisplayH;

var g_retKeyDown = false;


// チップセット関連
var g_chipset = [];
/*
var g_chipset;				//
var g_chipset_canvas;				//キャンバス
var g_chipset_context;				//コンテキスト
*/

//var g_select_mapchip_no;

var g_map_loop_sw;

// 表示ステータス
var g_sts_map_x;
var g_sts_map_y;

var g_sts_map_w;
var g_sts_map_h;

var g_sts_cursor_size;

var g_sts_chip_size;

var g_sts_display_size;

var g_sts_chip_no;

var g_textarea_message;

var g_layer = [];
var g_layer_check = [];
var g_select_layer;

var g_bank = [];

var g_chip_status;

var g_chipset_view;

var g_div_chipset_image = [];

var g_div_character_list;
var g_character_list_ul;
var g_character_list = [];
var g_select_chara_idx;

var g_div_event_list;
var g_event_list_ul;
var g_event_list = [];
var g_select_event_idx;

var g_edit_param_dialog;

var g_edit_param_x;
var g_edit_param_y;
var g_edit_param_tag;
var g_edit_param_comment;

var g_whole_map_dialog;
var g_whole_map_dialog_div;
var g_whole_map_canvas;
var g_whole_map_context;

var g_export_dialog;
var g_export_data;

var g_export_csv_dialog;
var g_export_csv_data;

var g_edging_dialog;

var g_drag;
var g_dragShift;

var g_bakClientMouseX;
var g_bakClientMouseY;
var g_clientMouseX;
var g_clientMouseY;

var g_isTouched;
var g_touchX;
var g_touchY;

var g_edging_type_canvas;
var g_edging_type_context;
var g_select_edging_type;

var g_help_dialog;

// 定数
const MAPDATA_MAX = 3;

const FRAME = 30;
const COUNT_MAX = 60;
const CANVAS_W = 720;
const CANVAS_H = 540;

const CHIPSIZE_W = 32;
const CHIPSIZE_H = 32;

const CHIPSET_CANVAS_W = 512;
const CHIPSET_CANVAS_H = 256;

const MAPSIZE_W = 29;
const MAPSIZE_H = 44;

const ARRAY_MAX = 512;

//================================================================
//
// ファイル読み込み
//
//================================================================

//document.write('<script type="text/javascript" src="./js/character.js"></script>');	//キャラクラス
document.write('<script type="text/javascript" src="./js/mapDraw.js"></script>');	//マップ描画クラス

document.write('<script type="text/javascript" src="./js/chipset.js"></script>');	//チップセット関連


//================================================================
//
// クラス
//
//================================================================

// イベント
function Event(id,x,y,idx,tag,comment) {
	this.id = id;
	this.x = x;
	this.y = y;
	this.idx = idx;
	this.tag = tag;
	this.comment = comment;
	this.getCSV = getCSV;
	this.getArrayCode = getArrayCode;
	this.getHtml = getHtml;

	function getCSV(){
		var csv_str = "";

		csv_str+="\""+this.id+"\",";
		csv_str+=this.x+",";
		csv_str+=this.y+",";
		csv_str+=this.idx+",";
		csv_str+="\""+this.tag+"\",";
		csv_str+="\""+this.comment+"\"\n";	
		return csv_str;
	}

	function getArrayCode(){
		var array_str = "";

		array_str+="{\""+this.tag+"\",";
		array_str+=this.x+",";
		array_str+=this.y+"},";
		array_str+=" //"+this.comment+"\n";	
		return array_str;
	}

	function getHtml(){
		var html = '<table style="width:100%"><tr style="width:100%">';
		html +='<td style="width:100px">'+'('+this.x+','+this.y+')'+'</td>';
		html +='<td style="width:100px">'+this.tag+'</td>';
		html +='<td>'+this.comment+'</td>';
		html +='</tr></table>';
		return html;
	}
}

// キャラクター
function Character(id,x,y,idx,tag,comment) {
	this.id = id;
	this.x = x;
	this.y = y;
	this.idx = idx;
	this.tag = tag;
	this.comment = comment;
	this.getCSV = getCSV;
	this.getArrayCode = getArrayCode;
	this.getHtml = getHtml;

	function getCSV(){
		var csv_str = "";

		csv_str+="\""+this.id+"\",";
		csv_str+=this.x+",";
		csv_str+=this.y+",";
		csv_str+=this.idx+",";
		csv_str+="\""+this.tag+"\",";
		csv_str+="\""+this.comment+"\"\n";	
		return csv_str;
	}

	function getArrayCode(){
		var array_str = "";

		array_str+="{\""+this.tag+"\",";
		array_str+=this.x+",";
		array_str+=this.y+"},";
		array_str+=" //"+this.comment+"\n";	
		return array_str;
	}

	function getHtml(){
		var html = '<table style="width:100%"><tr style="width:100%">';
		html +='<td style="width:100px">'+'('+this.x+','+this.y+')'+'</td>';
		html +='<td style="width:100px">'+this.tag+'</td>';
		html +='<td>'+this.comment+'</td>';
		html +='</tr></table>';
		return html;		
	}
}

//================================================================
//
// メイン処理
//
//================================================================

	// ドロップダウンメニューの設定
	$(function() {
    	$("ul.menu li").hover(
    		function() {
				$(".menuSub:not(:animated)", this).slideDown();
      		},
			function() {
				$(".menuSub", this).slideUp();
			}
		);
	});

	//画面読み込み後に初期化呼び出し
	window.addEventListener('load',initialize,false);

	//========================================================
	// 初期化
	// @author :KazuteruSonoyama
	// @date   :2020.10.18 新規作成
	//========================================================
	function initialize(){

//		chipset_initialize(); // チップセットの初期化
		for (var i=0;i<3;i++){
			g_chipset[i] = new ChipSet(i);
		}

		// changeイベントを設定
		document.getElementById( "mapchip_file" ).addEventListener( "change", load_mapchip); 
		document.getElementById( "mapdata_file" ).addEventListener( "change", load_mapdata); 
		document.getElementById( "csv_file" ).addEventListener( "change", load_csv); 
	
//		g_chara=new Character();	//　インスタンス生成
		for(var i=0;i<MAPDATA_MAX;i++){
			g_mapDraw[i]=new MapDraw();	//　インスタンス生成
		}
		g_view = document.getElementById('view');
		g_canvas=document.getElementById('canvas_view');	//キャンバスのセット
		if(!g_canvas||!g_canvas.getContext){
			return false;
		}
		g_canvas.width=CANVAS_W;					//キャンバスの横幅
		g_canvas.height=CANVAS_H;					//キャンバスの縦幅
		g_context=g_canvas.getContext('2d');			//コンテキストセット
		var framerate = Math.floor(1000/FRAME);
		g_update=setInterval(update,framerate);			//フレームのセット（1秒に30回描画）
		g_count = 0; //フレームカウント
		g_direction = 0; //移動方向
		g_moveCount = 0;

		g_mapX = 0;
		g_mapY = 0;

		g_mapW = MAPSIZE_W;
		g_mapH = MAPSIZE_H;

		g_chipW = CHIPSIZE_W;
		g_chipH = CHIPSIZE_H;

		g_chipDisplayW = CHIPSIZE_W;
		g_chipDisplayH = CHIPSIZE_H;

		g_cursorSize = 1;


		g_textarea_message = document.getElementById("message");
		g_textarea_message.value = "";
/*
		if (g_textarea_datacode!=null){
 			let binary = atob(g_textarea_datacode.value);
  			let len = binary.length;
  			let bytes = new Uint8Array(len);
  			for (let i = 0; i < len; i++)        {
    			bytes[i] = binary.charCodeAt(i);
  			}
  			g_mapDraw.g_mapdata = bytes;
		}
*/

		for (var i=0;i<MAPDATA_MAX;i++){
			g_mapDraw[i].g_mapdata = resize_map_data(0,0,null);
		}
		// ステータス編集エリア

		g_sts_map_x = document.getElementById('map_x');
		g_sts_map_x.addEventListener('keydown', enterKeyPress);
		g_sts_map_x.value = g_mapX;
		g_sts_map_y = document.getElementById('map_y');
		g_sts_map_y.addEventListener('keydown', enterKeyPress);
		g_sts_map_y.value = g_mapY;

		g_sts_map_w = document.getElementById('map_w');
		g_sts_map_w.addEventListener('keydown', enterKeyPress);
		g_sts_map_w.value = g_mapW;
		g_sts_map_h = document.getElementById('map_h');
		g_sts_map_h.addEventListener('keydown', enterKeyPress);
		g_sts_map_h.value = g_mapH;

		g_sts_cursor_size = document.getElementById('cursor_size');
		g_sts_cursor_size.addEventListener( "change", change_cursor_size);
		g_sts_cursor_size.value = "1";

		g_sts_chip_size = document.getElementById('chip_size');
		g_sts_chip_size.addEventListener( "change", change_chip_size);
		g_sts_chip_size.value = "32";

		g_sts_display_size = document.getElementById('display_size');
		g_sts_display_size.addEventListener( "change", change_chip_display_size);
		g_sts_display_size.value = "64";

		g_map_loop_checkbox = document.getElementById('map_loop_checkbox');
		g_map_loop_sw = false;
		g_map_loop_checkbox.checked = g_map_loop_sw;

		// レイヤーリストのチェック時のクリックが親DIVに伝搬しないようにする
		g_layer[0] = document.getElementById("layer_event");
		g_layer_check[0] = document.getElementById("layer_event_check");
		g_layer[1] = document.getElementById("layer_char");
		g_layer_check[1] = document.getElementById("layer_char_check");
		g_layer[2] = document.getElementById("layer2");
		g_layer_check[2] = document.getElementById("layer2_check");
		g_layer[3] = document.getElementById("layer1");
		g_layer_check[3] = document.getElementById("layer1_check");
		g_layer[4] = document.getElementById("layer0");
		g_layer_check[4] = document.getElementById("layer0_check");
	
		for (var i=0;i<5;i++){
			g_layer[i].style.color = "rgba(0,0,0,255)";
			g_layer[i].style.backgroundColor = "rgba(0,0,0,0)";
			g_layer_check[i].addEventListener('click', function(e){e.stopPropagation();});
			g_layer_check[i].checked = true;
		}
		g_select_layer = 4;
		g_layer[g_select_layer].style.color = "rgba(255,255,255,255)";
		g_layer[g_select_layer].style.backgroundColor = "rgba(64,64,64,255)";

		g_chipset_view = document.getElementById("chipset_view");
		for (var i=0;i<3;i++){
			g_div_chipset_image[i] = document.getElementById("chipset_image"+i);
		}

		g_sts_chip_no = document.getElementById('chip_no');
		g_sts_chip_no.value = g_chipset[g_mapDraw[4-g_select_layer].g_bankno].g_select_mapchip_no;

		for (var i=0;i<3;i++){
			g_bank[i] = document.getElementById("bank"+i);
			g_bank[i].style.color = "rgba(0,0,0,255)";
			g_bank[i].style.backgroundColor = "rgba(0,0,0,0)";
		}
		g_bank[g_mapDraw[4-g_select_layer].g_bankno].style.color = "rgba(255,255,255,255)";
		g_bank[g_mapDraw[4-g_select_layer].g_bankno].style.backgroundColor = "rgba(64,64,64,255)";

		g_chip_status = document.getElementById("chip_status");

		g_div_character_list = document.getElementById("character_list");
		g_character_list_ul = document.getElementById("character_list_ul");

		g_div_event_list = document.getElementById("event_list");
		g_event_list_ul = document.getElementById("event_list_ul");

		g_select_chara_idx = -1;
		g_select_event_idx = -1;

		g_edit_param_dialog = document.getElementById("paramEditDialog");

		g_whole_map_dialog = document.getElementById("wholeMapDialog");
		g_whole_map_dialog_div = document.getElementById("whole_map_dialog");
		g_whole_map_canvas = document.getElementById("whole_map_canvas");
		g_whole_map_context=g_whole_map_canvas.getContext('2d');

		g_export_dialog = document.getElementById("exportDialog");
		g_export_data = document.getElementById("export_data");

		g_export_csv_dialog = document.getElementById("exportCSVDialog");
		g_export_csv_data = document.getElementById("export_csv_data");

		g_edging_dialog = document.getElementById("edgingDialog");
		g_edging_type_canvas = document.getElementById("edging_type_canvas");
		g_edging_type_canvas.width = 96*5;
		g_edging_type_canvas.height = 96*6;
		g_edging_type_context = g_edging_type_canvas.getContext('2d');
		g_select_edging_type = 0;
		g_edging_dialog.addEventListener("close", (e) => {
			if (g_edging_dialog.returnValue === "ok"){
				// 縁取り実行
				execEdging();
				g_textarea_message.value +="縁取りを実行しました\n";
			}
		});


		g_edit_item_id = document.getElementById("edit_item_id");
		g_edit_param_x = document.getElementById("edit_param_x");
		g_edit_param_y = document.getElementById("edit_param_y");
		g_edit_param_tag = document.getElementById("edit_param_tag");
		g_edit_param_comment = document.getElementById("edit_param_comment");
		
		g_edit_param_dialog.addEventListener("close", (e) => {
			if (g_edit_param_dialog.returnValue === "ok"){
				var edit_item_id = g_edit_item_id.value;
				var edit_item = document.getElementById(edit_item_id);
				if (edit_item!=null){
					var target_array_ul = edit_item.parentNode;
					var target_array = g_event_list;
					if (target_array_ul == g_character_list_ul){
						target_array = g_character_list;
					}
					var item_idx = find_idx_with_id(target_array,edit_item_id);
					if (item_idx!=-1){
						target_array[item_idx].x = chg2Num4ce(g_edit_param_x.value,target_array[item_idx].x) % g_mapW;
						target_array[item_idx].y = chg2Num4ce(g_edit_param_y.value,target_array[item_idx].y) % g_mapH;
						target_array[item_idx].tag = chg2alpha4ce(g_edit_param_tag.value);
						target_array[item_idx].comment = g_edit_param_comment.value;
						var html = target_array[item_idx].getHtml();
						edit_item.innerHTML = html;
						if (target_array_ul == g_character_list_ul){
							g_textarea_message.value +="キャラクター「"+target_array[item_idx].tag+"」を編集しました\n";
						}else{
							g_textarea_message.value +="イベント「"+target_array[item_idx].tag+"」を編集しました\n";
						}
					}
				}
			}
		});

		g_drag = false;
		g_dragShift = false;
		g_isTouched = false;

		g_help_dialog = document.getElementById("helpDialog");

/*
		if (document.cookie == ''){
			// cookieが存在しない
		}else{


			// cookieが存在する
			let ret = getCookie("MAP_W");
			if (ret){
				g_mapW = ret;
				g_sts_map_w.value = g_mapW;
			}
			ret = getCookie("MAP_H");
			if (ret){
				g_mapH = ret;
				g_sts_map_h = g_mapH;
			}
			ret = getCookie("CHIP_SIZE");
			if (ret){
				g_chipW = ret;
				g_chipH = ret;
				g_sts_chip_size.value = g_chipW;
			}
			ret = getCookie("DISPLAY_SIZE");
			if (ret){
				g_chipDisplayW = ret;
				g_chipDisplayH  = ret;
				g_sts_display_size.value = g_chipDisplayW;
			}
		}
*/

	}
	//================================================
	// 更新
	// @author :KazuteruSonoyama
	// @date   :2020.10.18 新規作成
	//================================================
	function update(){

		if (g_select_layer>=2){
			// チップセットの更新
			g_chipset[g_mapDraw[4-g_select_layer].g_bankno].chipset_update(); // チップセットの更新
		}
		g_map_loop_sw = g_map_loop_checkbox.checked;

		var active =  document.activeElement;

	 	if( !(
			 g_sts_map_x === active || 
			 g_sts_map_y === active || 
			 g_sts_map_w === active || 
			 g_sts_map_h === active || 
			 g_sts_chip_no === active ||
			 g_edit_param_x === active ||
			 g_edit_param_y === active ||
			 g_edit_param_tag === active ||
			 g_edit_param_comment === active			 
			)
		 ){
			// 編集状態になっていない
			get_key_state();
			if (g_isTouched) calc_direction(g_touchX,g_touchY);
			g_retKeyDown = false;
    	}else{
			// 編集状態になっている
			g_retKeyDown = true;
    	}

		if (!g_whole_map_dialog.open) {
			//メイン描画
			game_draw();
		}else{
 			whole_map_dialog_draw();
		}

		if (g_edging_dialog.open){
			// 縁取りダイアログ描画
			draw_edging_type();
		}

		g_count++;
		g_count%=COUNT_MAX;

	}
	
	//================================================
	// キー入力取得
	// @author :KazuteruSonoyama
	// @date   :2020.10.18 新規作成
	//================================================
	function get_key_state(){

		// 塗りつぶし
		if(g_keyStateTrig["f"] || g_keyStateTrig["F"] ){
			if (g_select_layer>=2){
				fill_mapdata(g_keyStatePress["Shift"]);
				g_textarea_message.value +="塗りつぶしを実行しました\n";
			}
			g_keyStateTrig["f"] = false;
			g_keyStateTrig["F"] = false;
		}

		// クリア
		if(g_keyStateTrig["c"] || g_keyStateTrig["C"] ){
			if (g_select_layer>=2){
				if (window.confirm("選択しているチップで全体を塗りつぶしますか？")) {
					var bankno = g_mapDraw[4-g_select_layer].g_bankno;
					for (var i=0;i<g_mapW*g_mapH;i++){
						g_mapDraw[4-g_select_layer].g_mapdata[i] = g_chipset[bankno].g_select_mapchip_no;
					}
					g_textarea_message.value +="塗りつぶしを実行しました\n";
				}
			}
			g_keyStateTrig["c"] = false;
			g_keyStateTrig["C"] = false;
		}

		// 置き換え
		if(g_keyStateTrig["r"] || g_keyStateTrig["R"] ){
			if (g_select_layer>=2){
				if (window.confirm("選択しているチップで置き換えますか？")) {
					var bankno = g_mapDraw[4-g_select_layer].g_bankno;
					var mapidx = (g_mapY * g_mapW) + g_mapX;
					var chipno = g_mapDraw[4-g_select_layer].g_mapdata[mapidx];
					for (var i=0;i<g_mapW*g_mapH;i++){
						if (g_mapDraw[4-g_select_layer].g_mapdata[i]===chipno) g_mapDraw[4-g_select_layer].g_mapdata[i] = g_chipset[bankno].g_select_mapchip_no;
					}
					g_textarea_message.value +="置き換えを実行しました\n";
				}
			}
			g_keyStateTrig["r"] = false;
			g_keyStateTrig["R"] = false;
		}

		// カーソルサイズ
		if(g_select_layer>=2 && (g_keyStateTrig["."] || g_keyStateTrig[">"]) ){
			if (g_cursorSize<5) g_cursorSize = g_cursorSize+2;
			g_sts_cursor_size.value = g_cursorSize;
			g_keyStateTrig["."] = false;
			g_keyStateTrig[">"] = false;
		}
		if(g_select_layer>=2 && (g_keyStateTrig[","] || g_keyStateTrig["<"]) ){
			if (g_cursorSize>1) g_cursorSize = g_cursorSize-2;
			g_sts_cursor_size.value = g_cursorSize;
			g_keyStateTrig[","] = false;
			g_keyStateTrig["<"] = false;
		}

		// マップデータセット
		if(g_keyStatePress[" "]){
			if (g_select_layer>=2){
				var ofs_coef = Math.floor(g_cursorSize/2);
				for (var j=0;j<g_cursorSize;j++){
					for (var i=0;i<g_cursorSize;i++){
						var x = (g_mapX - ofs_coef + i + g_mapW) % g_mapW;
						var y = (g_mapY - ofs_coef + j + g_mapH) % g_mapH;
						g_mapDraw[4-g_select_layer].setMapData(x,y,g_chipset[g_mapDraw[4-g_select_layer].g_bankno].g_select_mapchip_no);
					}
				}
			}
		}
		if(g_keyStateTrig[" "]){
			if (g_select_layer==0){
				// イベントリスト追加
				var exist_idx = search_same_position_idx(g_event_list,g_mapX,g_mapY);
				if (exist_idx==-1){
					var length = g_event_list.length;
					if (length<ARRAY_MAX){
						var idx = get_available_idx(g_event_list);
						if (idx!=-1){
							var id = generate_id("event",idx);
							var event =  new Character(id,g_mapX,g_mapY,idx,'event'+idx,'');
							g_event_list.push(event);
							var html = event.getHtml();
							var li = create_list_item(id,html);
							g_event_list_ul.appendChild(li);
						} 
					}
				}else{
					// 削除
					g_event_list.splice(exist_idx, 1);
					const li = g_event_list_ul.children[exist_idx];
					g_event_list_ul.removeChild (li);
				}
			}else if (g_select_layer==1){
				// キャラクターリスト追加
				var exist_idx = search_same_position_idx(g_character_list,g_mapX,g_mapY);
				if (exist_idx==-1){
					var length = g_character_list.length;
					if (length<ARRAY_MAX){
						var idx = get_available_idx(g_character_list);
						if (idx!=-1){
							var id = generate_id("chara",idx);
							var character =  new Character(id,g_mapX,g_mapY,idx,'chara'+idx,'');
							g_character_list.push(character);
							var html = character.getHtml();
							var li = create_list_item(id,html);
							g_character_list_ul.appendChild(li);
						} 
					}
				}else{
					// 削除
					g_character_list.splice(exist_idx, 1);
					const li = g_character_list_ul.children[exist_idx];
					g_character_list_ul.removeChild (li);
				}
			}
			g_keyStateTrig[" "] = false;
		}

		if (g_moveCount>0){
			g_moveCount--;
			if (g_moveCount==0){
				switch(g_direction)
				{
					case 0:
						// 下
						g_mapY+=1;			
						break;

					case 1:
						// 上
						g_mapY-=1;			
						break;

					case 2:
						// 左
						g_mapX-=1;			
						break;

					case 3:
						// 右
						g_mapX+=1;			
						break;

					case 4:
						// 左下
						g_mapX-=1;			
						g_mapY+=1;			
						break;

					case 5:
						// 右下
						g_mapX+=1;			
						g_mapY+=1;			
						break;

					case 6:
						// 左上
						g_mapX-=1;			
						g_mapY-=1;			
						break;

					case 7:
						// 右上
						g_mapX+=1;			
						g_mapY-=1;			
						break;

				}
				if (g_mapX<0) g_mapX=(g_mapX+g_mapW)%g_mapW;	
				if (g_mapY<0) g_mapY=(g_mapY+g_mapH)%g_mapH;	
				if (g_mapX>=g_mapW) g_mapX=g_mapX%g_mapW;
				if (g_mapY>=g_mapH) g_mapY=g_mapY%g_mapH;

				if (g_sts_map_x!=null) g_sts_map_x.value = g_mapX;
				if (g_sts_map_y!=null) g_sts_map_y.value = g_mapY;
		
			}			
		}
		if (g_moveCount==0){
			//左
			if(g_keyStatePress["ArrowLeft"]){
				g_moveCount=4;
				//上
				if(g_keyStatePress["ArrowUp"]){
					g_direction = 6;
				}
				//下
				else if(g_keyStatePress["ArrowDown"]){
					g_direction = 4;			
				}
				else{
					g_direction = 2;			
				}
			}
			//右
			else if(g_keyStatePress["ArrowRight"]){
				g_moveCount=4;
				//上
				if(g_keyStatePress["ArrowUp"]){
					g_direction = 7;			
				}
				//下
				else if(g_keyStatePress["ArrowDown"]){
					g_direction = 5;			
				}
				else{
					g_direction = 3;			
				}
			}
			//上
			else if(g_keyStatePress["ArrowUp"]){
					g_moveCount=4;
				g_direction = 1;			
			}
			//下
			else if(g_keyStatePress["ArrowDown"]){
				g_moveCount=4;
				g_direction = 0;			
			}

			// カーソルスピードアップ
			if (g_moveCount>0 && g_keyStatePress["Shift"]){
				g_moveCount /= 2;
			}

		}
	}
	
	//================================================
	// メイン描画
	// @author :KazuteruSonoyama
	// @date   :2020.10.18 新規作成
	//================================================
	function game_draw(){
		g_context.beginPath();

		for (var j=0;j<Math.floor(g_canvas.height/16)+1;j++){
			for (var i=0;i<Math.floor(g_canvas.width/16)+1;i++){
			//	if ((j*Math.floor(g_canvas.height/16)+(j%2)*((Math.floor(g_canvas.width/16)+1)%2)+i)%2==0){
				if ((j+i)%2==0){
						g_context.fillStyle="rgb(128,128,128)";
				}else{
					g_context.fillStyle="rgb(64,64,64)";
				}
				g_context.fillRect(i*16,j*16,16,16);
					
			}	
		}

		g_context.save();

		for (var i=0;i<MAPDATA_MAX;i++){
			if (g_layer_check[4-i].checked){
				g_mapDraw[i].map_draw(g_context,g_mapX,g_mapY,g_direction,g_moveCount);
			}
		}

		// イベント、キャラクターの描画
		event_character_draw();

		drawArrow();

		// マップカーソル
		var cursor_y = Math.floor((CANVAS_H - g_chipDisplayH) / 2);
		var cursor_x = Math.floor((CANVAS_W - g_chipDisplayW) / 2);
		drawMapCursor(cursor_x,cursor_y,g_chipDisplayW,g_chipDisplayH,(g_select_layer>=2 ? g_cursorSize:1));

		g_context.restore();



	}

	//================================================
	// イベント・キャラクター描画
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.15 新規作成
	//================================================
	function event_character_draw(){ 
		
		var loop_v = Math.floor(CANVAS_H/g_chipDisplayH)+4;
		var loop_h = Math.floor(CANVAS_W/g_chipDisplayW)+3;
		for (var v = 0;v <loop_v;v++) {

			var v_index = g_mapY - Math.floor(loop_v/2)+v;
			if (g_map_loop_sw){
				v_index = (v_index+g_mapH)%g_mapH;
			}
			if (v_index < 0 || v_index>=g_mapH) continue;

	   		for (var h = 0;h < loop_h;h++) {
				var h_index = g_mapX -Math.floor(loop_h/2)+h;
				if (g_map_loop_sw){
					h_index = (h_index+g_mapW)%g_mapW;
				}
				if (h_index < 0 || h_index>=g_mapW) continue;
	
				var index = (h_index + v_index * g_mapW);
				var ofs_x=0,ofs_y=0;

				if (g_moveCount>0){
					switch(g_direction)
					{
						case 0:
							// 下
							ofs_y = Math.floor(((4-g_moveCount)*g_chipDisplayH)/(-4));
							break;

						case 1:
							// 上
							ofs_y = Math.floor(((4-g_moveCount)*g_chipDisplayH)/(4));
							break;

						case 2:
							// 左
							ofs_x = Math.floor(((4-g_moveCount)*g_chipDisplayW)/(4));
							break;

						case 3:
							// 右
							ofs_x = Math.floor(((4-g_moveCount)*g_chipDisplayW)/(-4));
							break;

						case 4:
							// 左下
							ofs_x = Math.floor(((4-g_moveCount)*g_chipDisplayW)/(4));
							ofs_y = Math.floor(((4-g_moveCount)*g_chipDisplayH)/(-4));
							break;

						case 5:
							// 右下
							ofs_x = Math.floor(((4-g_moveCount)*g_chipDisplayW)/(-4));
							ofs_y = Math.floor(((4-g_moveCount)*g_chipDisplayH)/(-4));
							break;

						case 6:
							// 左上
							ofs_x = Math.floor(((4-g_moveCount)*g_chipDisplayW)/(4));
							ofs_y = Math.floor(((4-g_moveCount)*g_chipDisplayH)/(4));
							break;

						case 7:
							// 右上
							ofs_x = Math.floor(((4-g_moveCount)*g_chipDisplayW)/(-4));
							ofs_y = Math.floor(((4-g_moveCount)*g_chipDisplayH)/(4));
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

				if (g_layer_check[1].checked){
					var length = g_character_list.length;
					for (var i=0;i<length;i++){
						if (g_character_list[i].x==h_index && g_character_list[i].y==v_index){
							drawObjectImage(x,y,g_chipDisplayW,g_chipDisplayH,g_character_list[i].tag,'#fff');
						}
					}
				}

				if (g_layer_check[0].checked){
					var length = g_event_list.length;
					for (var i=0;i<length;i++){
						if (g_event_list[i].x==h_index && g_event_list[i].y==v_index){
							drawObjectImage(x,y,g_chipDisplayW,g_chipDisplayH,g_event_list[i].tag,'#f80');
						}
					}
				}


				g_context.fillStyle = '#ccc'; //
				g_context.lineWidth = 4; // 線の幅は4px
				var blink_count = g_count % 30;
//				g_context.globalAlpha =  (( blink_count > 15 ? 30 - blink_count : blink_count ) / 15)  * 0.5+0.5;		
				g_context.globalAlpha =  1;		
		
				if (h_index == 0){
					g_context.fillRect(x-4,y-4,4,g_chipDisplayH+8);
				}else if (h_index==g_mapW-1){
					g_context.fillRect(x+g_chipDisplayW,y-4,4,g_chipDisplayH+8);
				}
				if (v_index == 0){
					g_context.fillRect(x,y-4,g_chipDisplayW,4);
				}else if (v_index==g_mapH-1){
					g_context.fillRect(x,y+g_chipDisplayH,g_chipDisplayW,4);
				}
				g_context.globalAlpha =  1.0;


			}
		}
	}

	//================================================
	// 全体マップ描画
	// @author :KazuteruSonoyama
	// @date   :2020.10.18 新規作成
	//================================================
	function whole_map_dialog_draw(){
		g_whole_map_context.beginPath();

		for (var j=0;j<Math.floor(g_whole_map_canvas.height/4)+1;j++){
			for (var i=0;i<Math.floor(g_whole_map_canvas.width/4)+1;i++){
			//	if ((j*Math.floor(g_whole_map_canvas.height/4)+(j%2)*((Math.floor(g_whole_map_canvas.width/4)+1)%2)+i)%2==0){
				if ((j+i)%2==0){
						g_whole_map_context.fillStyle="rgb(128,128,128)";
				}else{
					g_whole_map_context.fillStyle="rgb(64,64,64)";
				}
				g_whole_map_context.fillRect(i*4,j*4,4,4);
					
			}	
		}

		g_whole_map_context.save();

		for (var i=0;i<MAPDATA_MAX;i++){
			if (g_layer_check[4-i].checked){
				g_mapDraw[i].whole_map_draw(g_whole_map_context,4);
			}
		}

		drawWholeMapFrame();

		if (g_drag) drawWholeMapCursor(g_clientMouseX,g_clientMouseY);
	
		g_whole_map_context.restore();

	}

	//================================================
	// キー入力取得
	// @author :KazuteruSonoyama
	// @date   :2020.10.18 新規作成
	//================================================
	document.onkeydown=function(e){
		g_keyStateTrig[e.key] = true;
		g_keyStatePress[e.key]=true;
		return g_retKeyDown;
	};
	document.onkeyup=function(e){
		g_keyStateTrig[e.key]=false;
		g_keyStatePress[e.key]=false;
		return g_retKeyDown;
	};

	//================================================
	// マップサイズの変更
	// @author :KazuteruSonoyama
	// @date   :2025.01.06 新規作成
	//================================================
	function resize_map_data(oldW,oldH,oldData)
	{
		var mapbytes_len = g_mapW * g_mapH;
		var bytes = new Uint8Array(mapbytes_len);
		for (var i=0;i<mapbytes_len;i++){
			bytes[i] = 0;
		}

		if (oldData!=null){
			for (var y=0;y < oldH && y < g_mapH;y++){
				for (var x=0;x < oldW && x < g_mapW; x++){
					bytes[y*g_mapW + x] = oldData[y*oldW + x];
				}
			}
		}
		return bytes;
	}

	//================================================
	// チップセット選択ファイルダイアログを表示
	// @author :KazuteruSonoyama
	// @date   :2025.01.06 新規作成
	//================================================
	function open_chipset_filedialog(){
		var filebutton = document.getElementById('mapchip_file');
		if (filebutton!=null) filebutton.click();
	}

	//================================================
	// マップデータ選択ファイルダイアログを表示
	// @author :KazuteruSonoyama
	// @date   :2025.01.06 新規作成
	//================================================
	function open_mapdata_load_dialog(){
		if (g_select_layer>=2){
			var filebutton = document.getElementById('mapdata_file');
			if (filebutton!=null) filebutton.click();
		}else{
			var filebutton = document.getElementById('csv_file');
			if (filebutton!=null) filebutton.click();
		}
	}

	//================================================
	// マップデータ保存ダイアログを表示
	// @author :KazuteruSonoyama
	// @date   :2025.01.06 新規作成
	//================================================
	function open_mapdata_save_dialog(){
		if (g_select_layer==0){
			var filename = prompt("保存ファイル名", "eventdata.csv");
			var length = g_event_list.length;
			var str="";
			for (var i=0;i<length;i++){
				str += g_event_list[i].getCSV();
			}
			var array = new TextEncoder().encode(str);
			write_binary(filename,array);
			g_textarea_message.value +="レイヤー"+(4-g_select_layer)+"のデータを\'"+filename+"\'として保存しました\n";
		}else
		if (g_select_layer==1){
			var filename = prompt("保存ファイル名", "characterdata.csv");
			var length = g_character_list.length;
			var str="";
			for (var i=0;i<length;i++){
				str += g_character_list[i].getCSV();
			}
			var array = new TextEncoder().encode(str);
			write_binary(filename,array);
			g_textarea_message.value +="レイヤー"+(4-g_select_layer)+"のデータを\'"+filename+"\'として保存しました\n";
		}
		else{
			var filename = prompt("保存ファイル名", "mapdata.bin");
			write_binary(filename,g_mapDraw[4-g_select_layer].g_mapdata);
			g_textarea_message.value +="レイヤー"+(4-g_select_layer)+"のデータを\'"+filename+"\'として保存しました\n";
		}
	}

	//================================================
	// マップチップの読み込み
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.06 新規作成
	//================================================
	function load_mapchip(){
		// フォームで選択された全ファイルを取得

		var file_list = this.files;

		// 選択された画像を表示
//		for( var i=0,l=file_list.length; l>i; i++ ) {
			// Blob URLの作成
			var bankno = 0;
			if (g_select_layer>=2) bankno = g_mapDraw[4-g_select_layer].g_bankno;
			var blobUrl = window.URL.createObjectURL( file_list[0] ) ;
			g_chipset[bankno].g_image.src  = blobUrl;
//		}

		g_textarea_message.value +="bank"+bankno+"へマップチップ\'"+file_list[0].name+"\'を読み込みました\n";
	}

	//================================================
	// マップデータの読み込み
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.17 新規作成
	//================================================
	function load_mapdata(){

		if (g_select_layer>=2){
			// フォームで選択された全ファイルを取得
			var file_list = this.files;
			read_binary(file_list[0],4-g_select_layer);
			g_textarea_message.value +="マップデータ\'"+file_list[0].name+"\'をレイヤー"+(4-g_select_layer)+"へ読み込みました\n";
		}
	
	}

	//================================================
	// イベント、キャrクターデータの読み込み
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.25 新規作成
	//================================================
	function load_csv(){
		// フォームで選択された全ファイルを取得
		const reader = new FileReader();
		// NOTE: fileを文字列として読み込む
		reader.readAsText(this.files[0]);
		reader.onload = function(event) {
			result = event.target.result;
			if (g_select_layer==0){
				parseCSV(result,g_event_list);
				g_textarea_message.value +="イベントデータ\'"+file_list[0].name+"\'をレイヤー"+(4-g_select_layer)+"へ読み込みました\n";
			}else{
				parseCSV(result,g_character_list);
				g_textarea_message.value +="キャラクターデータ\'"+file_list[0].name+"\'をレイヤー"+(4-g_select_layer)+"へ読み込みました\n";
			}

		}
	}

	//========================================================
	// マウスダウン
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2024.12.02 新規作成
	//========================================================
	document.onmousedown=function(e){

		var rect = e.target.getBoundingClientRect();
		var clientMouseX = e.clientX - rect.left;
		var clientMouseY = e.clientY - rect.top;

		if (e.target.id === "canvas_view"){
			g_touchX = clientMouseX;
			g_touchY = clientMouseY;
			g_isTouched = true;
		}
		if (e.target.id.indexOf("canvas_chipset") === 0){
			// チップセット

		}
		if (g_whole_map_dialog.open && e.target.id === "whole_map_canvas"){
			// 全体マップ
			g_drag = true;
			if (!g_keyStatePress["Shift"]){
				g_dragShift = false;
				g_clientMouseX = Math.floor(clientMouseX/4);
				g_clientMouseY = Math.floor(clientMouseY/4);
			}else{
				g_dragShift = true;
				g_mapX = Math.floor(clientMouseX/4);
				g_mapY = Math.floor(clientMouseY/4);
			}

		}


	}

	//========================================================
	// マウスムーブ
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2024.12.02 新規作成
	//========================================================
	document.onmousemove=function(e){

		var rect = e.target.getBoundingClientRect();
		var clientMouseX = e.clientX - rect.left;
		var clientMouseY = e.clientY - rect.top;

		if (g_isTouched){
			if (e.target.id === "canvas_view"){
				g_touchX = clientMouseX;
				g_touchY = clientMouseY;
			}else{
				g_isTouched = false;
			}
		}
		if (e.target.id.indexOf("canvas_chipset") === 0){
			// チップセット

		}
		if (g_whole_map_dialog.open && e.target.id === "whole_map_canvas" && g_drag){

			if (!g_dragShift){
				// 全体マップ
				g_bakClientMouseX = g_clientMouseX;
				g_bakClientMouseY = g_clientMouseY;
				g_clientMouseX = Math.floor(clientMouseX/4);
				g_clientMouseY = Math.floor(clientMouseY/4);
				setMapLine(g_bakClientMouseX,g_bakClientMouseY,g_clientMouseX,g_clientMouseY);
			}else if (g_keyStatePress["Shift"]){
				g_mapX = Math.floor(clientMouseX/4);
				g_mapY = Math.floor(clientMouseY/4);
			}

		}

	}

	//========================================================
	// マウスアップ
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2024.12.02 新規作成
	//========================================================
	document.onmouseup=function(e){

		var rect = e.target.getBoundingClientRect();
		var clientMouseX = e.clientX - rect.left;
		var clientMouseY = e.clientY - rect.top;

		if (e.target.id.indexOf("canvas_chipset") === 0){
			// チップセット
			var bankno = g_mapDraw[4-g_select_layer].g_bankno;
			var column = Math.floor(clientMouseX / g_chipW);
			var row = Math.floor(clientMouseY / g_chipH);
			var column_max = Math.floor(g_chipset[bankno].g_image.width / g_chipW);
			g_chipset[bankno].g_select_mapchip_no = ((row * column_max) + column) % 0x100; // とりあえず0~255
			g_sts_chip_no.value = g_chipset[bankno].g_select_mapchip_no;

		}
		if (g_whole_map_dialog.open && e.target.id === "whole_map_canvas" && g_drag){
			// 全体マップ
			if (!g_dragShift){
				g_bakClientMouseX = g_clientMouseX;
				g_bakClientMouseY = g_clientMouseY;
				g_clientMouseX = Math.floor(clientMouseX/4);
				g_clientMouseY = Math.floor(clientMouseY/4);
				setMapLine(g_bakClientMouseX,g_bakClientMouseY,g_clientMouseX,g_clientMouseY);
			}else{	
				g_mapX = Math.floor(clientMouseX/4);
				g_mapY = Math.floor(clientMouseY/4);
			}

		}

		if (g_edging_dialog.open && e.target.id === "edging_type_canvas" ){
			var no = Math.floor(clientMouseY/96)*5 + Math.floor(clientMouseX/96);
			if (no<30) g_select_edging_type = no;
		}

		g_isTouched = false;
		g_dragShift = false;
		g_drag = false;

	}

	//========================================================
	// 方向計算
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.22 新規作成
	//========================================================
	function calc_direction(touchX,touchY){

		if (g_moveCount>0) return true; 

		var pad_center_x = CANVAS_W / 2;
		var pad_center_y = CANVAS_H / 2;
		
		var diffX = touchX - pad_center_x;
		var diffY = touchY - pad_center_y; 
		var distance = Math.sqrt(Math.pow(diffX, 2) + Math.pow(diffY, 2));

		if ((Math.abs(diffX) >= CANVAS_W/4 ||  Math.abs(diffY) >= CANVAS_H/4) && Math.abs(diffX) < CANVAS_W/2 && Math.abs(diffY) < CANVAS_H/2 ){
			var deg =  (180 - (Math.atan2(diffX,diffY) * 180 / Math.PI)) % 360;
			g_moveCount=4;
			if ((deg >= 330 && deg < 360) || (deg >= 0 && deg<30)){
				// 上
				g_direction = 1;
			}
			else if ( deg >= 30 && deg < 60 ){
				//　右上
				g_direction = 7;
			}
			else if ( deg >= 60 && deg < 120 ){
				// 右
				g_direction = 3;
			}
			else if ( deg >= 120 && deg < 150 ){
				// 右下
				g_direction = 5;
			}
			else if ( deg >= 150 && deg < 210 ){
				// 下
				g_direction = 0;
			}
			else if ( deg >= 210 && deg < 240 ){
				// 左下
				g_direction = 4;
			}
			else if ( deg >= 240 && deg < 300 ){
				// 左
				g_direction = 2;
			}
			else if ( deg >= 300 && deg < 330 ){
				// 左上
				g_direction = 6;
			}
			return true;			
		}
		return false;			
	}	

	//========================================================
	// テキストボックス、ENTERキー押下
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2024.12.02 新規作成
	//========================================================
	function enterKeyPress(e)
	{
		if(e.key === "Enter"){
			switch (e.target.id)
			{
				case "map_x":
				{
					var new_x = chg2Num4ce(g_sts_map_x.value) % g_mapW;
					if (new_x<0) new_x = 0;
					g_mapX = new_x;
					g_sts_map_x.value = new_x;
					g_moveCount = 0;
				}
				break;

				case "map_y":
				{
					var new_y = chg2Num4ce(g_sts_map_y.value) % g_mapH;
					if (new_y<0) new_y = 0;
					g_mapY = new_y;
					g_sts_map_y.value = new_y;
					g_moveCount = 0;
				}
				break;

				case "map_w":
				{
					var new_w = chg2Num4ce(g_sts_map_w.value);
					if (new_w<8) new_w = 8;
					if (new_w>4096) new_w = 4096;
					
					var oldW = g_mapW;
					g_mapW = new_w;
					g_sts_map_w.value = new_w;
					for (var i=0;i<MAPDATA_MAX;i++){
						g_mapDraw[i].g_mapdata = resize_map_data(oldW,g_mapH,g_mapDraw[i].g_mapdata);
					}
					g_mapX %= new_w;
					g_sts_map_x.value = g_mapX;
					
					g_moveCount = 0;

//					document.cookie = "MAP_W="+g_mapW+";max-age=2592000";
				}
				break;

				case "map_h":
				{
					var new_h = chg2Num4ce(g_sts_map_h.value);
					if (new_h<8) new_h = 8;
					if (new_h>4096) new_h = 4096;

					var oldH = g_mapH;
					g_mapH = new_h;
					g_sts_map_h.value = new_h;
					for (var i=0;i<3;i++){
						g_mapDraw[i].g_mapdata = resize_map_data(g_mapW,oldH,g_mapDraw[i].g_mapdata);
					}

					g_mapY %= new_h;
					g_sts_map_y.value = g_mapY;
					
					g_moveCount = 0;

//					document.cookie = "MAP_H="+g_mapH+";max-age=2592000";
				}
				break;
			}
		}
	}

	//========================================================
	// カーソルサイズの変更
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2024.12.02 新規作成
	//========================================================
	function change_cursor_size(e)
	{
		g_cursorSize = chg2Num4ce(e.target.value);
	}

	//========================================================
	// チップサイズの変更
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2024.12.02 新規作成
	//========================================================
	function change_chip_size(e)
	{
		g_chipW = chg2Num4ce(e.target.value);
		g_chipH = chg2Num4ce(e.target.value);		
		g_chipDisplayW = g_chipW;
		g_chipDisplayH = g_chipH;
		g_sts_display_size.value = g_chipDisplayW;		

//		document.cookie = "CHIP_SIZE="+g_chipW+";max-age=2592000";

	}

	//========================================================
	// チップ表示サイズの変更
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.26 新規作成
	//========================================================
	function change_chip_display_size(e)
	{
		g_chipDisplayW = chg2Num4ce(e.target.value);
		g_chipDisplayH = chg2Num4ce(e.target.value);		

//		document.cookie = "DISPLAY_SIZE="+g_chipDisplayW+";max-age=2592000";
	}

	//========================================================
	// レイヤーの変更
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2024.12.02 新規作成
	//========================================================
	function select_layer(idx)
	{
		g_layer[g_select_layer].style.color = "rgba(0,0,0,255)";
		g_layer[g_select_layer].style.backgroundColor = "rgba(0,0,0,0)";

		g_select_layer = idx;
		g_layer[g_select_layer].style.color = "rgba(255,255,255,255)";
		g_layer[g_select_layer].style.backgroundColor = "rgba(64,64,64,255)";

		if (g_select_layer==0){
			// イベントレイヤー
			g_chipset_view.style.backgroundColor = "white";
			for(var i=0;i<3;i++)  g_div_chipset_image[i].style.display="none";
			g_div_character_list.style.display="none";
			g_div_event_list.style.display="block";

			g_chip_status.style.display = 'none';
		}else if (g_select_layer==1){
			// キャラクターレイヤー
			g_chipset_view.style.backgroundColor = "white";
			for(var i=0;i<3;i++)  g_div_chipset_image[i].style.display="none";
			g_div_character_list.style.display="block";
			g_div_event_list.style.display="none";

			g_chip_status.style.display = 'none';
		}else{
			var bankno = g_mapDraw[4-g_select_layer].g_bankno;
			g_chipset_view.style.backgroundColor = "black";
			for(var i=0;i<3;i++)  g_div_chipset_image[i].style.display="none";
			g_div_chipset_image[bankno].style.display="block";
			g_div_character_list.style.display="none";
			g_div_event_list.style.display="none";

			g_sts_chip_no.value = g_chipset[g_mapDraw[4-g_select_layer].g_bankno].g_select_mapchip_no;

			g_chip_status.style.display = 'block';
		}

	}

	//========================================================
	// バンクの変更
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.19 新規作成
	//========================================================
	function select_bank(idx)
	{
		if (g_select_layer>=2){
			var bankno = g_mapDraw[4-g_select_layer].g_bankno;
			g_bank[bankno].style.color = "rgba(0,0,0,255)";
			g_bank[bankno].style.backgroundColor = "rgba(0,0,0,0)";

			bankno = g_mapDraw[4-g_select_layer].g_bankno = idx;
			g_bank[bankno].style.color = "rgba(255,255,255,255)";
			g_bank[bankno].style.backgroundColor = "rgba(64,64,64,255)";
			
			for (var i=0;i<3;i++) g_div_chipset_image[i].style.display="none";
			g_div_chipset_image[bankno].style.display="block";

			g_sts_chip_no.value = g_chipset[g_mapDraw[4-g_select_layer].g_bankno].g_select_mapchip_no;
		}

	}

	//========================================================
	// 指定されたチップでのマップの塗りつぶし
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.18 新規作成
	//========================================================
	function fill_mapdata(isPressShift)
	{
		var bankno = g_mapDraw[4-g_select_layer].g_bankno;
		var fillChipIdx = g_chipset[bankno].g_select_mapchip_no;

		var mapLayerIdx = 4-g_select_layer;
		var mapdata_len = g_mapW * g_mapH;
		var fillData = new Int16Array(mapdata_len);
		for (var i=0;i<mapdata_len;i++){
			fillData[i] = -1;
		}
		var dataIdx = calc_map_index(g_mapX,g_mapY);
		if (isPressShift || g_mapDraw[mapLayerIdx].g_mapdata[dataIdx]!=fillChipIdx) fillData[dataIdx] = 0;
		var startChipIdx = g_mapDraw[mapLayerIdx].g_mapdata[dataIdx];

		var fill_count = 0;
		var set_count;
		do{
			set_count = 0;
			for (var y=0;y<g_mapH;y++){
				for (var x=0;x<g_mapW;x++){
					var idx = calc_map_index(x,y);
					if (fillData[idx] == -1 && 
						(
							(!isPressShift && g_mapDraw[mapLayerIdx].g_mapdata[idx] != fillChipIdx) ||
							(isPressShift && g_mapDraw[mapLayerIdx].g_mapdata[idx] == startChipIdx)
						)
					 ){
						var idx_up = calc_map_index(x,y-1);
						if ( idx_up!=-1 && fillData[idx_up]==fill_count){
							fillData[idx] = fill_count+1;
							set_count++;
						}
						var idx_down = calc_map_index(x,y+1);
						if ( idx_down!=-1 && fillData[idx_down]==fill_count){
							fillData[idx] = fill_count+1;
							set_count++;
						}
						var idx_left = calc_map_index(x-1,y);
						if ( idx_left!=-1 && fillData[idx_left]==fill_count){
							fillData[idx] = fill_count+1;
							set_count++;
						}
						var idx_right = calc_map_index(x+1,y);
						if ( idx_right!=-1 && fillData[idx_right]==fill_count){
							fillData[idx] = fill_count+1;
							set_count++;
						}
					}
				}
			}
			fill_count++;
		}while(set_count>0);
		for (var y=0;y<g_mapH;y++){
			for (var x=0;x<g_mapW;x++){
				var idx = calc_map_index(x,y);
				if (fillData[idx] != -1){
					g_mapDraw[mapLayerIdx].g_mapdata[idx] = fillChipIdx;
				}
			}
		}
	}

	function calc_map_index(x,y)
	{
		if (x<0 || y<0 || x>=g_mapW || y>=g_mapH) return -1;
		return (y*g_mapW+x);
	}

	//========================================================
	// バイナリファイルの読み込み
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2024.12.02 新規作成
	//========================================================
	function read_binary(file,idx)
	{
		var reader = new FileReader();
		reader.onload = function(e) {
			// ここでreader.resultプロパティを処理する
			g_mapDraw[idx].g_mapdata = new Uint8Array(reader.result);
		}
		reader.onerror = function(e) {
			console.error('reading failed');
		};
		reader.readAsArrayBuffer(file);
	}

	//========================================================
	// バイナリファイルの保存
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2024.12.02 新規作成
	//========================================================
	function write_binary(filename,target_array)
	{

		if (filename!='' && filename!=null){

			var buffer = new ArrayBuffer(target_array.length);
			var dv = new DataView(buffer);
			for( var i = 0 ; i < target_array.length ; i++ )
				dv.setUint8(i, target_array[i]);
			
			//適当な見えないエレメントを作成                                                                                                                                       
			var a = document.createElement("a");
			document.body.appendChild(a);
			a.style = "display: none";
			console.log(a);

			//ArrayBufferをBlobに変換                                                                                                                                                
			var blob = new Blob([buffer], {type: "octet/stream"}),
			url = window.URL.createObjectURL(blob);
			console.log(url);

			//データを保存する                                                                                                                                                     
			a.href = url;
			a.target = '_blank';
			a.download = filename;
			a.click();
			window.URL.revokeObjectURL(url);
		}
	}

	//========================================================
	// リストアイテムの生成
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2024.12.02 新規作成
	//========================================================
	function create_list_item(id,html)
	{

		var li = document.createElement('li');
		li.innerHTML = html;
		li.draggable = true;
		li.id = id;
	
		// クリック
		li.onclick  = function () {

			var select_item = select_list_item(this);
			if (select_item!=null){
				if(g_keyStatePress["Shift"]){
					g_mapX = select_item.x;
					g_mapY = select_item.y;
				}
			}

		}

		// ダブルクリック
		li.ondblclick  = function () {
			var select_item = select_list_item(this,true);
			if (select_item!=null){
				g_edit_item_id.value = select_item.id;
				g_edit_param_x.value = select_item.x;
				g_edit_param_y.value = select_item.y;
				g_edit_param_tag.value = select_item.tag;
				g_edit_param_comment.value = select_item.comment;
				g_edit_param_dialog.showModal();
			}
	
		}

		li.ondragstart = function (e) {
			e.dataTransfer.setData('text/plain', e.target.id);
		};
			
		li.ondragover = function (e) {
			e.preventDefault();
			this.style.borderTop = '3px solid';
			this.style.backgroundColor = "rgba(160,160,160,128)";
			this.style.color = "white";
		};
		
		li.ondragleave = function () {
			this.style.borderTop = "";
			this.style.backgroundColor = "rgba(0,0,0,0)";
			this.style.color = "black";
		};
		
		li.ondrop = function (e) {
			e.preventDefault();
			let id = e.dataTransfer.getData('text');
			let element_drag = document.getElementById(id);
			this.parentNode.insertBefore(element_drag, this);
			this.style.borderTop = '';
			this.style.backgroundColor = "rgba(0,0,0,0)";
			this.style.color = "black";

			move_list_items(element_drag,this);
		};
		return li;

	}


	//========================================================
	// 有効なインデックスの取得
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.14S 新規作成
	//========================================================
	function get_available_idx(tgt_array)
	{
		var length = tgt_array.length;
		var j=0;
		for (;j<ARRAY_MAX;j++){
			var i=0;
			for (;i<length;i++){
				if (tgt_array[i].idx==j) break; 
			}
			if (i==length) break;
		}
		if (j<ARRAY_MAX) return j;
		return -1;
	}

	//========================================================
	// 指定位置に要素が設定されていればインデックスを返す
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.14 新規作成
	//========================================================
	function search_same_position_idx(tgt_array,x,y)
	{
		var length = tgt_array.length;
		for (var i=0;i<length;i++){
			if (tgt_array[i].x==x && tgt_array[i].y==y) return i; 
		}
		return -1;
	}

	//========================================================
	// リストの要素の選択
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.16 新規作成
	//========================================================
	function select_list_item(tgt_item,forced = false)
	{
		var select_idx = g_select_event_idx;
		var tgt_array = g_event_list;
		if (tgt_item.parentNode.parentNode.id=='character_list'){
			select_idx = g_select_chara_idx;
			tgt_array = g_character_list;
		}
			
		var idx = find_idx_with_id(tgt_array,tgt_item.id);
		if (idx!=-1){
			if (!forced && select_idx==idx){
				select_idx=-1;
				tgt_item.style.backgroundColor = "rgba(0,0,0,0)";
			}else{
				if (select_idx!=-1){
					var current_li = document.getElementById(tgt_array[select_idx].id);
					current_li.style.backgroundColor = "rgba(0,0,0,0)";
				}
				select_idx=idx;
				tgt_item.style.backgroundColor = "rgba(128,128,128,255)";
			}
		}

		if (tgt_array===g_event_list){
			g_select_event_idx = select_idx;
		}else{
			g_select_chara_idx = select_idx;
		}
		if (select_idx==-1) return null;
		return tgt_array[select_idx];
	}

	//========================================================
	// リストの要素のスワップ
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.16 新規作成
	//========================================================
	function move_list_items(tgt_item1,tgt_item2)
	{

		var tgt_array = g_event_list;
		if (tgt_item1.parentNode.parentNode.id=='character_list'){
			tgt_array = g_character_list;
		}

		var idx1 = find_idx_with_id(tgt_array,tgt_item1.id);
		if (idx1==-1) return;

		// 移動対象のオブジェクトを退避してから配列から取り除く
		var moveObj = tgt_array[idx1];
		tgt_array.splice(idx1, 1);

		var idx2 = find_idx_with_id(tgt_array,tgt_item2.id);
		if (idx2==-1) idx2 = idx1;
		if (idx2==tgt_array.length) idx2=tgt_array.length-1;

		tgt_array.splice(idx2,0,moveObj);

		if (tgt_array===g_event_list){
			g_select_event_idx = -1;
		}else{
			g_select_chara_idx = -1;
		}

	}

	//========================================================
	// 指定idの要素のインデックスを返す
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.16 新規作成
	//========================================================
	function find_idx_with_id(tgt_array,id)
	{
		var length = tgt_array.length;
		for (var i=0;i<length;i++){
			if (tgt_array[i].id==id) return i; 
		}
		return -1;
	}

	//========================================================
	// idの生成
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.14 新規作成
	//========================================================
	function generate_id(prefix,idx)
	{
		var date = new Date();
		var id = prefix+date.getFullYear().toString().slice(-2);
		id += ("0"+(date.getMonth() + 1)).slice(-2);
		id += ("0"+date.getDate()).slice(-2);
		id += ('0' + date.getHours()).slice(-2);
		id += ('0' + date.getMinutes()).slice(-2);
		id += ('0' + date.getSeconds()).slice(-2);
		id += ('00' + idx).slice(-3);
		return id;
	}

	//================================================
	// マップカーソルの描画
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.1.08 新規作成
	//================================================
	function drawMapCursor(x,y,w,h,size){
		g_context.strokeStyle = '#f00'; //
    	g_context.lineWidth = 4; // 線の幅は4px
		var blink_count = g_count % 30;
		g_context.globalAlpha =  (( blink_count > 15 ? 30 - blink_count : blink_count ) / 15)  * 0.5;

		var ofs_coef = Math.floor(size/2); 
		var ofs_x =  ofs_coef * w * (-1);
		var ofs_y =  ofs_coef * h * (-1);

		g_context.strokeRect(x+ofs_x, y+ofs_y, (w*size)-4, (h*size)-4);

		g_context.globalAlpha =  1.0;

    }

	//================================================
	// 全体マップへカーソルを描画
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.21 新規作成
	//================================================
	function drawWholeMapCursor(x,y){
	
		g_whole_map_context.fillStyle = '#fff'; //
		var blink_count = g_count % 30;
		g_whole_map_context.globalAlpha =  (( blink_count > 15 ? 30 - blink_count : blink_count ) / 15)  * 0.5;
		var size = 4+(( blink_count > 15 ? 30 - blink_count : blink_count ) / 15)  * 8;
		var ofs = Math.floor(size/2);
		g_whole_map_context.fillRect(x*4-ofs, y*4-ofs,size,size);

		g_whole_map_context.globalAlpha =  1.0;

    }

	//================================================
	// 全体マップへ枠を描く
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.20 新規作成
	//================================================
	function drawWholeMapFrame()
	{
		g_whole_map_context.strokeStyle = '#f00'; //
    	g_whole_map_context.lineWidth = 4; // 線の幅は4px
		g_whole_map_context.globalAlpha =  1.0;

		var width = Math.floor( CANVAS_W * 4 / g_chipDisplayW );
		var height = Math.floor( CANVAS_H * 4 / g_chipDisplayH );
		var ofs_x = Math.floor( width/2 ) * (-1);
		var ofs_y = Math.floor( height/2 ) * (-1);
		g_whole_map_context.strokeRect(g_mapX*4+ofs_x, g_mapY*4+ofs_y, width-4, height-4);

	}

	//================================================
	// オブジェクトの描画
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.1.08 新規作成
	//================================================
	function drawObjectImage(x,y,w,h,tag,col){
		g_context.strokeStyle = col; //
    	g_context.fillStyle = '#000'; //

		g_context.globalAlpha = 0.3;
		g_context.fillRect(x, y, w, h);


		g_context.lineWidth = 4; // 線の幅は4px
		g_context.globalAlpha =  1.0;
		g_context.strokeRect(x+2, y+2, w-4, h-4);

		var fontsize = w/4-2;
		var ofs_x = 0 - (Math.floor((tag.length+1)/2)*(fontsize/2));
		var ofs_y = fontsize/2;

		g_context.font = fontsize+"px serif";

		g_context.fillText(tag, x+(w/2)+ofs_x+1, y+(h/2)+ofs_y+1);
    	g_context.fillStyle = col; //
		g_context.fillText(tag, x+(w/2)+ofs_x, y+(h/2)+ofs_y);

    }

	//================================================
	// 数値にする
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.17 新規作成
	//================================================
	function chg2Num4ce(value,defnum=0)
	{
		return (isNaN(parseInt(value)) ? defnum:parseInt(value));
	}

	//================================================
	// アルファベットにする
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.17 新規作成
	//================================================
	function chg2alpha4ce(value)
	{
		var result_array = value.match(/[a-zA-Z0-9_]+/g);
		var result="";
		for (var i=0;i<result_array.length;i++){
			result+=result_array[i];
		}
		return result;
	}

	//================================================
	// 全体マップを開く
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.20 新規作成
	//================================================
	function open_whole_map()
	{
		g_whole_map_dialog.showModal();
		g_whole_map_dialog.style.width = (g_mapW*4)+"px";
		g_whole_map_dialog.style.height = (g_mapH*4)+"px";
		g_whole_map_canvas.width=g_mapW*4;					//キャンバスの横幅
		g_whole_map_canvas.height=g_mapH*4;					//キャンバスの縦幅

	}

	//================================================
	// エキスポートダイアログを開く
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.20 新規作成
	//================================================
	function open_export_dialog()
	{
		g_export_dialog.showModal();
		if (g_select_layer==0){

			// イベント配列出力
			let code="";
			let length = g_event_list.length;
			for (let i=0;i<length;i++){
				code+=g_event_list[i].getArrayCode();
			}
			g_export_data.value = code;

		}else if (g_select_layer==1){
			// キャラクター配列出力
			let code="";
			let length = g_character_list.length;
			for (let i=0;i<length;i++){
				code+=g_character_list[i].getArrayCode();
			}
			g_export_data.value = code;

		}else{
			var data_no = 4-g_select_layer;
			const decodeBinaryString = uint8Array => uint8Array.reduce(
				(binaryString, uint8) => binaryString + String.fromCharCode(uint8),
				'',
			);

			const binaryStringA = decodeBinaryString(g_mapDraw[data_no].g_mapdata);
//			const base64 = btoa(binaryStringA);
			g_export_data.value = btoa(binaryStringA);
		}

	}

	//================================================
	// エキスポートCSVダイアログを開く
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.07.31 新規作成
	//================================================
	function open_export_csv_dialog()
	{
		g_export_csv_dialog.showModal();
		if (g_select_layer==0){

			// イベント配列出力
/*
			let code="";
			let length = g_event_list.length;
			for (let i=0;i<length;i++){
				code+=g_event_list[i].getArrayCode();
			}
			g_export_data.value = code;
*/
		}else if (g_select_layer==1){
			// キャラクター配列出力
/*
			let code="";
			let length = g_character_list.length;
			for (let i=0;i<length;i++){
				code+=g_character_list[i].getArrayCode();
			}
			g_export_data.value = code;
*/
		}else{
			var data_no = 4-g_select_layer;
			const decodeBinaryString = uint8Array => uint8Array.reduce(
				(binaryString, uint8) => binaryString + String.fromCharCode(uint8),
				'',
			);

			var data ="";
			
			for (let my =0;my<g_mapH;my++){
				for (let mx =0;mx<g_mapW;mx++){
					let idx = (my*g_mapW)+mx;
					if (mx>0) data+=",";
					data+=g_mapDraw[data_no].g_mapdata[idx];
					if (mx==g_mapW-1) data+="\n";
				}
			}
			g_export_csv_data.value = data;
			
		}

	}

	//================================================
	// 縁取りダイアログを開く
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.22 新規作成
	//================================================
	function open_edging_dialog()
	{
		if (g_select_layer>=2){
			g_select_edging_type = 0;
			g_edging_dialog.showModal();
		}else{
			g_textarea_message.value +="＜！＞縁取り処理はマップレイヤーにのみ有効です\n";
		}
	}

	//================================================
	// 縁取りタイプの表示
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.20 新規作成
	//================================================
	function draw_edging_type()
	{
		g_edging_type_context.beginPath();
		g_edging_type_context.fillStyle="rgba(0,0,0,0)";
		g_edging_type_context.fillRect(0,0,g_edging_type_canvas.width,g_edging_type_canvas.height);
     	g_edging_type_context.lineWidth = 1; // 線の幅は1px
		g_edging_type_context.globalAlpha =  1.0;

		for (var i=0;i<30;i++){
			var x1 = (i%5)*96;
			var y1 = Math.floor(i/5)*96;
			var x2 = x1+96;
			var y2 = y1+96;

			g_edging_type_context.fillStyle = '#000'; //
			g_edging_type_context.fillRect( x1, y1, 96, 96);
			if (i==g_select_edging_type){
				g_edging_type_context.fillStyle = '#f00'; //
				var blink_count = g_count % 30;
				g_edging_type_context.globalAlpha =  (( blink_count > 15 ? 30 - blink_count : blink_count ) / 15)  * 0.5;
				g_edging_type_context.fillRect( x1, y1, 96, 96);
				g_edging_type_context.globalAlpha =  1.0;
			}

			g_edging_type_context.strokeStyle = '#fff'; //
			g_edging_type_context.strokeRect( x1, y1, 96, 96);
			g_edging_type_context.strokeStyle = '#000'; //
			if (i==g_select_edging_type) g_edging_type_context.fillStyle = '#ccc'; //
			else g_edging_type_context.fillStyle = '#fff'; //
			var type = i+1;
			if (i>=15){
				type = 0x10*(i-15+1);
			}

			if (type & 0x01){
				// 上
				g_edging_type_context.beginPath();
				g_edging_type_context.moveTo(x1, y1);
				g_edging_type_context.lineTo(x1+16, y1+16);
				g_edging_type_context.lineTo(x2-16, y1+16);
				g_edging_type_context.lineTo(x2, y1);
				g_edging_type_context.fill();			
				g_edging_type_context.closePath();
				g_edging_type_context.stroke();			
			}
			if (type & 0x02){
				// 下
				g_edging_type_context.beginPath();
				g_edging_type_context.moveTo(x1, y2);
				g_edging_type_context.lineTo(x1+16, y2-16);
				g_edging_type_context.lineTo(x2-16, y2-16);
				g_edging_type_context.lineTo(x2, y2);
				g_edging_type_context.fill();			
				g_edging_type_context.closePath();
				g_edging_type_context.stroke();			
			}
			if (type & 0x04){
				// 左
				g_edging_type_context.beginPath();
				g_edging_type_context.moveTo(x1, y1);
				g_edging_type_context.lineTo(x1+16, y1+16);
				g_edging_type_context.lineTo(x1+16, y2-16);
				g_edging_type_context.lineTo(x1, y2);
				g_edging_type_context.fill();			
				g_edging_type_context.closePath();
				g_edging_type_context.stroke();							
			}
			if (type & 0x08){
				// 右
				g_edging_type_context.beginPath();
				g_edging_type_context.moveTo(x2, y1);
				g_edging_type_context.lineTo(x2-16, y1+16);
				g_edging_type_context.lineTo(x2-16, y2-16);
				g_edging_type_context.lineTo(x2, y2);
				g_edging_type_context.fill();			
				g_edging_type_context.closePath();
				g_edging_type_context.stroke();											
			}
			if (type & 0x10){
				// 左上
				g_edging_type_context.beginPath();
				g_edging_type_context.moveTo(x1, y1);
				g_edging_type_context.lineTo(x1+32, y1);
				g_edging_type_context.lineTo(x1, y1+32);
				g_edging_type_context.lineTo(x1, y1);
				g_edging_type_context.fill();			
				g_edging_type_context.closePath();
				g_edging_type_context.stroke();
			}											
			if (type & 0x20){
				// 右上
				g_edging_type_context.beginPath();
				g_edging_type_context.moveTo(x2, y1);
				g_edging_type_context.lineTo(x2-32, y1);
				g_edging_type_context.lineTo(x2, y1+32);
				g_edging_type_context.lineTo(x2, y1);
				g_edging_type_context.fill();			
				g_edging_type_context.closePath();
				g_edging_type_context.stroke();
			}
			if (type & 0x40){
				// 右下
				g_edging_type_context.beginPath();
				g_edging_type_context.moveTo(x2, y2);
				g_edging_type_context.lineTo(x2-32, y2);
				g_edging_type_context.lineTo(x2, y2-32);
				g_edging_type_context.lineTo(x2, y2);
				g_edging_type_context.fill();			
				g_edging_type_context.closePath();
				g_edging_type_context.stroke();												
			}
			if (type & 0x80){
				// 左下
				g_edging_type_context.beginPath();
				g_edging_type_context.moveTo(x1, y2);
				g_edging_type_context.lineTo(x1+32, y2);
				g_edging_type_context.lineTo(x1, y2-32);
				g_edging_type_context.lineTo(x1, y2);
				g_edging_type_context.fill();			
				g_edging_type_context.closePath();
				g_edging_type_context.stroke();												
			}

		}

		this.g_context.save();
		g_edging_type_context.restore();


	}

	//================================================
	// マップにラインを引く
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.20 新規作成
	//================================================
	function setMapLine(x0,y0,x1,y1)
	{

		var mapLayerIdx = 4-g_select_layer;
		var bankno = g_mapDraw[mapLayerIdx].g_bankno;

		// ２点間の距離
		var dx,dy;
		dx = ( x1 > x0 ) ? x1 - x0 : x0 - x1;
		dy = ( y1 > y0 ) ? y1 - y0 : y0 - y1;

		var sx,sy;
		sx = ( x1 > x0 ) ? 1 : -1;
		sy = ( y1 > y0 ) ? 1 : -1;		

		var mapDataIdx;

 		//  傾きが1より小さい場合
		if ( dx > dy ) {
		    var E = -dx;
   			for ( var i = 0 ; i <= dx ; i++ ) {
				mapDataIdx = (y0 * g_mapW) + x0;		
				g_mapDraw[mapLayerIdx].g_mapdata[mapDataIdx] = g_chipset[bankno].g_select_mapchip_no;
				x0 += sx;
				E += 2 *dy;
	
				if ( E >= 0 ) {
					y0 += sy;
					E -= 2 * dx;
				}
			}
		// 傾きが1以上の場合
		} else {
		    var E = -dy;
			for ( var i = 0 ; i <= dy ; i++ ) {
				mapDataIdx = (y0 * g_mapW) + x0;		
				g_mapDraw[mapLayerIdx].g_mapdata[mapDataIdx] = g_chipset[bankno].g_select_mapchip_no;
     			y0 += sy;
      			E += 2 * dx;
		    	
				if ( E >= 0 ) {
       				x0 += sx;
        			E -= 2 * dy;
				}
			}
		}

	}

	//================================================
	// 縁取り実行
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.24 新規作成
	//================================================
	function execEdging()
	{
		var layer_idx = 4-g_select_layer;
		var curidx = (g_mapY*g_mapW)+g_mapX;
		var check_chip = g_mapDraw[layer_idx].g_mapdata[curidx];
		var bankno = g_mapDraw[layer_idx].g_bankno;


		for (var y=0;y<g_mapH;y++){
			for (var x=0;x<g_mapW;x++){
				var chkmap = 0;
				var chkidx = 0
				var mapidx = (y*g_mapW)+x;
				if (g_mapDraw[layer_idx].g_mapdata[mapidx]===check_chip) continue;

				// 上
				chkidx = ((y-1)*g_mapW)+x;
				if (g_map_loop_sw && y===0) chkidx+=g_mapH*g_mapW;
				if (g_map_loop_sw || y>0){
					if (g_mapDraw[layer_idx].g_mapdata[chkidx]===check_chip){
						chkmap |= 0x01;
					}
				}

				// 下
				chkidx = ((y+1)*g_mapW)+x;
				if (g_map_loop_sw && y===g_mapH-1) chkidx-=g_mapH*g_mapW;
				if (g_map_loop_sw || y<g_mapH-1){
					if (g_mapDraw[layer_idx].g_mapdata[chkidx]===check_chip){
						chkmap |= 0x02;
					}
				}

				// 左
				chkidx = (y*g_mapW)+x-1;
				if (g_map_loop_sw && x===0) chkidx+=g_mapW;
				if (g_map_loop_sw || x>0){
					if (g_mapDraw[layer_idx].g_mapdata[chkidx]===check_chip){
						chkmap |= 0x04;
					}
				}

				// 右
				chkidx = (y*g_mapW)+x+1;
				if (g_map_loop_sw && x===g_mapW-1) chkidx-=g_mapW;
				if (g_map_loop_sw || x<g_mapW-1){
					if (g_mapDraw[layer_idx].g_mapdata[chkidx]===check_chip){
						chkmap |= 0x08;
					}
				}

				if (chkmap===0){

					// 左上
					chkidx = ((y-1)*g_mapW)+x-1;
					if (g_map_loop_sw && y===0) chkidx+=g_mapH*g_mapW;
					if (g_map_loop_sw && x===0) chkidx+=g_mapW;
					if (g_map_loop_sw || (y>0 && x>0)){
						if (g_mapDraw[layer_idx].g_mapdata[chkidx]===check_chip){
							chkmap |= 0x10;
						}
					}
	
					// 右上
					chkidx = ((y-1)*g_mapW)+x+1;
					if (g_map_loop_sw && y===0) chkidx+=g_mapH*g_mapW;
					if (g_map_loop_sw && x===g_mapW-1) chkidx-=g_mapW;
					if (g_map_loop_sw || (y>0 && x<g_mapW-1)){
						if (g_mapDraw[layer_idx].g_mapdata[chkidx]===check_chip){
							chkmap |= 0x20;
						}
					}

					// 右下
					chkidx = ((y+1)*g_mapW)+x+1;
					if (g_map_loop_sw && y===g_mapH-1) chkidx-=g_mapH*g_mapW;
					if (g_map_loop_sw && x===g_mapW-1) chkidx-=g_mapW;
					if (g_map_loop_sw || (y<g_mapH-1 && x<g_mapW-1)){
						if (g_mapDraw[layer_idx].g_mapdata[chkidx]===check_chip){
							chkmap |= 0x40;
						}
					}

					// 左下
					chkidx = ((y+1)*g_mapW)+x-1;
					if (g_map_loop_sw && y===g_mapH-1) chkidx-=g_mapH*g_mapW;
					if (g_map_loop_sw && x===0) chkidx+=g_mapW;
					if (g_map_loop_sw || (y<g_mapH-1 && x>0)){
						if (g_mapDraw[layer_idx].g_mapdata[chkidx]===check_chip){
							chkmap |= 0x80;
						}
					}

				}
				if ((g_select_edging_type<15 && chkmap===g_select_edging_type+1) || (g_select_edging_type>=15 && (chkmap>>4)===g_select_edging_type-15+1) ){
					g_mapDraw[layer_idx].g_mapdata[mapidx] = g_chipset[bankno].g_select_mapchip_no;
				}
			}
		}

	}

	//================================================
	// 矢印描画
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.24 新規作成
	//================================================
	function drawArrow()
	{
		if (g_moveCount==0) return;
		g_context.fillStyle="#fff";
		var center_x = Math.floor(g_canvas.width/2);
		var center_y = Math.floor(g_canvas.height/2);
		var px1,px2,px3,px4,px5,px7;
		var py1,py2,py3,py4,py5,py7;

		if (g_direction==1){
			px1 = center_x;
			py1 = center_y-160;
			px2 = center_x-30;
			py2 = center_y-120;
			px3 = center_x+30;
			py3 = center_y-120;

			px4 = center_x+15;
			py4 = center_y-120;
			px5 = center_x-15;
			py5 = center_y-120;
			px6 = center_x+15;
			py6 = center_y-100;
			px7 = center_x-15;
			py7 = center_y-100;
		}
		else if (g_direction==0){
			px1 = center_x;
			py1 = center_y+160;
			px2 = center_x-30;
			py2 = center_y+120;
			px3 = center_x+30;
			py3 = center_y+120;

			px4 = center_x+15;
			py4 = center_y+120;
			px5 = center_x-15;
			py5 = center_y+120;
			px6 = center_x+15;
			py6 = center_y+100;
			px7 = center_x-15;
			py7 = center_y+100;
		}
		else if (g_direction==2){
			px1 = center_x-160;
			py1 = center_y;
			px2 = center_x-120;
			py2 = center_y-30;
			px3 = center_x-120;
			py3 = center_y+30;

			px4 = center_x-120;
			py4 = center_y+15;
			px5 = center_x-120;
			py5 = center_y-15;
			px6 = center_x-100;
			py6 = center_y+15;
			px7 = center_x-100;
			py7 = center_y-15;
		}
		else if (g_direction==3){
			px1 = center_x+160;
			py1 = center_y;
			px2 = center_x+120;
			py2 = center_y-30;
			px3 = center_x+120;
			py3 = center_y+30;

			px4 = center_x+120;
			py4 = center_y+15;
			px5 = center_x+120;
			py5 = center_y-15;
			px6 = center_x+100;
			py6 = center_y+15;
			px7 = center_x+100;
			py7 = center_y-15;
		}
		else if (g_direction==4){
			// 左下
			px1 = center_x-126;
			py1 = center_y+126;
			px2 = center_x-71;
			py2 = center_y+118;
			px3 = center_x-118;
			py3 = center_y+71;

			px4 = center_x-84;
			py4 = center_y+106;
			px5 = center_x-107;
			py5 = center_y+83;
			px6 = center_x-68;
			py6 = center_y+91;
			px7 = center_x-91;
			py7 = center_y+68;
		}
		else if (g_direction==5){
			// 右下
			px1 = center_x+126;
			py1 = center_y+126;
			px2 = center_x+71;
			py2 = center_y+118;
			px3 = center_x+118;
			py3 = center_y+71;

			px4 = center_x+84;
			py4 = center_y+106;
			px5 = center_x+107;
			py5 = center_y+83;
			px6 = center_x+68;
			py6 = center_y+91;
			px7 = center_x+91;
			py7 = center_y+68;
		}
		else if (g_direction==6){
			// 左上
			px1 = center_x-126;
			py1 = center_y-126;
			px2 = center_x-71;
			py2 = center_y-118;
			px3 = center_x-118;
			py3 = center_y-71;

			px4 = center_x-84;
			py4 = center_y-106;
			px5 = center_x-107;
			py5 = center_y-83;
			px6 = center_x-68;
			py6 = center_y-91;
			px7 = center_x-91;
			py7 = center_y-68;
		}
		else if (g_direction==7){
			// 右上
			px1 = center_x+126;
			py1 = center_y-126;
			px2 = center_x+71;
			py2 = center_y-118;
			px3 = center_x+118;
			py3 = center_y-71;

			px4 = center_x+84;
			py4 = center_y-106;
			px5 = center_x+107;
			py5 = center_y-83;
			px6 = center_x+68;
			py6 = center_y-91;
			px7 = center_x+91;
			py7 = center_y-68;
		}

		g_context.beginPath();
		g_context.moveTo(px1, py1);
		g_context.lineTo(px2, py2);
		g_context.lineTo(px3, py3);
		g_context.lineTo(px1, py1);
		g_context.fill();			
		g_context.beginPath();
		g_context.moveTo(px4, py4);
		g_context.lineTo(px5, py5);
		g_context.lineTo(px7, py7);
		g_context.lineTo(px6, py6);
		g_context.lineTo(px4, py4);
		g_context.fill();			

	}

	//================================================
	// csv解析
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.24 新規作成
	//================================================
	function parseCSV(data) {
		const lines = data.split('\n');
		if (g_select_layer==0){
			g_event_list = [];
			g_event_list_ul.innerHTML = "";
		}else{
			g_character_list = [];
			g_character_list_ul.innerHTML = "";
		}

		// データ行を設定
		for (let i = 0; i < lines.length; i++) {
			var cells = lines[i].split(',');

			var id = cells[0].replaceAll("\"","");
			var map_x = chg2Num4ce(cells[1]);
			var map_y = chg2Num4ce(cells[2]);
			var idx = chg2Num4ce(cells[3]);
			var tag = cells[4].replaceAll("\"","");
			var comment = cells[5].replaceAll("\"","");

			if (g_select_layer==0){
				var object =  new Event(id,map_x,map_y,idx,tag,comment);
				g_event_list.push(object);
				var html = object.getHtml();
				var li = create_list_item(id,html);
				g_event_list_ul.appendChild(li);
			}else{
				var object =  new Character(id,map_x,map_y,idx,tag,comment);			
				g_character_list.push(object);
				var html = object.getHtml();
				var li = create_list_item(id,html);
				g_character_list_ul.appendChild(li);
			}
		}
	}

	//================================================
	// about
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.24 新規作成
	//================================================
	function showHelp() {
		g_help_dialog.showModal();
	}

	//================================================
	// about
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.24 新規作成
	//================================================
	function showAbout() {
		alert("2D Map Editer Ver.1.00");
	}
	 
/*
	//================================================
	// クッキーの取得
	// @author :Buzz.ltd KazuteruSonoyama
	// @date   :2025.01.26 新規作成
	//================================================
	function getCookie(name) {
		let cookieArr = document.cookie.split(";");
		for (let i = 0; i < cookieArr.length; i++) {
			let cookiePair = cookieArr[i].split("=");
			if (name === cookiePair[0].trim()) {
				return decodeURIComponent(cookiePair[1]);
			}
		}
		return null;
	}
*/
