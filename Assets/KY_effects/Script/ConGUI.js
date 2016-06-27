#pragma strict
public var mainCamera:Transform;
public var cameraTrs:Transform;
public var rotSpeed:int = 20;
public var effectObj:GameObject[];
public var effectObProj:GameObject[];
private var arrayNo:int = 0;

private var nowEffectObj:GameObject;
private var cameraState:String[] = ["Camera move" ,"Camera stop"];
private var cameraRotCon:int = 1;

private var num:int = 0;
private var numBck:int = 0;
private var initPos:Vector3;

private var haveProFlg:boolean = false;
private var nonProFX:GameObject;

function visibleBt():boolean{
	for(var tmpObj:GameObject in effectObProj){
		if( effectObj[ arrayNo ].name == tmpObj.name){
			nonProFX = tmpObj;
			return true;
		}
	}
	return false;
}

function Start () {
	initPos = mainCamera.localPosition;
	
	haveProFlg = visibleBt();
}

function Update () {
	if( cameraRotCon == 1)cameraTrs.Rotate(0 ,rotSpeed * Time.deltaTime ,0);
	
	if(num > numBck){
		numBck = num;
		mainCamera.localPosition.y += 0.05;
		mainCamera.localPosition.z -= 0.3;
		
	}else if(num < numBck){
		numBck = num;
		mainCamera.localPosition.y -= 0.05;
		mainCamera.localPosition.z += 0.3;
	}else if(num == 0){
		mainCamera.localPosition.y = initPos.y;
		mainCamera.localPosition.z = initPos.z;
	}
	
	if(mainCamera.localPosition.y < initPos.y )mainCamera.localPosition.y = initPos.y;
	if(mainCamera.localPosition.z > initPos.z )mainCamera.localPosition.z = initPos.z;
}

function  OnGUI(){
		
	if (GUI.Button (Rect(20, 0, 30, 30), "←")) {//return
		arrayNo --;
		if(arrayNo < 0)arrayNo = effectObj.Length -1;
		effectOn();
		
		haveProFlg = visibleBt();
	}
	
	if (GUI.Button (Rect(50, 0, 200, 30), effectObj[ arrayNo ].name )) {
		effectOn();
	}
	
	if (GUI.Button (Rect(250, 0, 30, 30), "→")) {//next
		arrayNo ++;
		if(arrayNo >= effectObj.Length)arrayNo = 0;
		effectOn();
		
		haveProFlg = visibleBt();
	}
	
	if( haveProFlg ){
		if (GUI.Button (Rect(50, 30, 300, 70), "+Distorsion (Pro only)" )) {
			if(nowEffectObj != null)Destroy( nowEffectObj );
			nowEffectObj = Instantiate( nonProFX );
		}
	}
	
	
	if (GUI.Button (Rect(300, 0, 200, 30), cameraState[ cameraRotCon ] )) {
		if( cameraRotCon == 1){
			cameraRotCon = 0;
		}else{
			cameraRotCon = 1;
		}
	}
	
	num = GUI.VerticalSlider(Rect(30, 100, 20, 200), num, 0, 20);
	

}

function effectOn(){
	if(nowEffectObj != null)Destroy( nowEffectObj );
	nowEffectObj = Instantiate(effectObj[ arrayNo ] );
}