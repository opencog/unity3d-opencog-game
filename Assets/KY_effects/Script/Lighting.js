#pragma strict
public var lighting:float = 1;
public var lightPower:Light;
public var flashFlg:boolean = false;
public var flashTimer:float = 0.3;

private var lightKeepFlg:boolean = false;
public var revOnTime:float = 0;
public var keepOnTime:float = 0;
public var keepTime:float = 0;

public var flashingFlg:boolean = false;
public var minLight:float = 0;
public var maxLight:float = 1;
private var lightOffFlg:boolean = false;
public var flashingOff:float = 0;
public var flashingOffPower:float = 0;
public var flashingOffIntensity:float = 1;

function Start () {
	lightPower = this.GetComponent.<Light>();
	
	flash();
	setRev();
	keepOn();
	setFlashingOff();
}

function Update () {
	
	if( flashingFlg ){
		if( lightOffFlg ){
			lightPower.intensity -= lighting * Time.deltaTime;
			if( lightPower.intensity <= minLight)lightOffFlg = false;
		}else{
			lightPower.intensity += lighting * Time.deltaTime;
			if( lightPower.intensity > maxLight )lightOffFlg = true;
		}
	}else	if( lightPower.intensity > 0 && lightPower.enabled && !lightKeepFlg){
		lightPower.intensity -= lighting * Time.deltaTime;
	}
	
	if( lightKeepFlg && keepTime > 0){
		keepTime -= Time.deltaTime;
		if( keepTime <= 0 )lightKeepFlg = false;
	}
}

function flash(){
	if( flashFlg ){
		lightPower.enabled = false;
		yield WaitForSeconds( flashTimer );
		lightPower.enabled = true;
	}
}

function setRev(){
	if( revOnTime > 0){
		yield WaitForSeconds( revOnTime );
		lighting *= -1; 
	}
}

function keepOn(){
	if(  keepOnTime > 0){
		yield WaitForSeconds( keepOnTime );
		lightKeepFlg = true;
	}
}

function setFlashingOff(){
	if(  flashingOff > 0){
		yield WaitForSeconds( flashingOff );
		flashingFlg = false;
		if( flashingOffPower > 0 ){
			lightPower.intensity = flashingOffIntensity;
			lighting = flashingOffPower;
		}
	}
}