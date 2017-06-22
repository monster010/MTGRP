﻿API.onServerEventTrigger.connect((eventName, args) => {
	switch (eventName) {
	case "fuel_updatevalue":
		if(myBrowser !== null)
			myBrowser.call("setFuel", args[0]);
		break;
	}
});

var myBrowser = null;

function getSafeResolution () {
	var offsetX = 0;
	var screen = API.getScreenResolutionMantainRatio();
	var screenX = screen.Width;
	var screenY = screen.Height;
	if (screenX / screenY > 1.7777) {
		// aspect ratio is larger than 16:9
		var idealBox = Math.ceil(screenY * 1.7777);
		// ex: 2850 - 1920 == 660 / 2 == 330
		offsetX = (screenX - idealBox) / 2;
		// and gotta set the ideal box to make it work
		screenX = idealBox;
	}

	return { offsetX, screenX, screenY }
}

function scaleCoordsToReal (point) {
	var ratioScreen = API.getScreenResolutionMantainRatio();
	var realScreen = API.getScreenResolution();

	var widthDivisor = realScreen.Width / ratioScreen.Width;
	var heightDivisor = realScreen.Height / ratioScreen.Height;

	return { X: point.X * widthDivisor, Y: point.Y * heightDivisor }
}

API.onPlayerEnterVehicle.connect((vehicle) => {
	if (API.getPlayerVehicleSeat(API.getLocalPlayer()) !== -1) return;

	var res = API.getScreenResolutionMantainRatio();
	var width = 470;
	var height = 225;
	myBrowser = API.createCefBrowser(width, height);
	API.waitUntilCefBrowserInit(myBrowser);
	var pos = scaleCoordsToReal({X: 310,Y: res.Height - height - 5 });
	API.setCefBrowserPosition(myBrowser, pos.X, pos.Y);
	API.loadPageCefBrowser(myBrowser, "speed_fuel_system/SpeedoFuel.html");
	API.waitUntilCefBrowserLoaded(myBrowser);
});

function loaded() {
	var vehicle = API.getPlayerVehicle(API.getLocalPlayer());
	var speed = API.getVehicleMaxSpeed(API.getEntityModel(vehicle));
	var intSpeed = Math.round(speed * 4.3); //m/s to km/h  | I know this is not a real correct rate but the game for some reason isnt accurate so I increased the rate to make sure speed never goes above max.
	if(myBrowser !== null) myBrowser.call("setupSpeed", intSpeed);
	API.triggerServerEvent("fuel_getvehiclefuel");
}

API.onPlayerExitVehicle.connect((vehicle) => {
	if(myBrowser === null) return;

	API.destroyCefBrowser(myBrowser);
	myBrowser = null;
});

var posUpdateTick = Date.now();

function getDirectionName(direction) {
	var angle = Math.round(direction.Z);
	if (angle >= -23 && angle < 23)
		return "N";
	else if (angle >= 23 && angle < 67)
		return "NW";
	else if (angle >= 67 && angle < 112)
		return "W";
	else if (angle >= 112 && angle < 156)
		return "SW";
	else if ((angle >= 156 && angle < 180) || (angle < -156 && angle >= -180))
		return "S";
	else if (angle < -23 && angle >= -67)
		return "NE";
	else if (angle < -67 && angle >= -112)
		return "E";
	else if (angle < -112 && angle >= -156)
		return "SE";
	else
		return "NO";
}

var lastZone = "";
var lastStreet = "";
var lastDirection = "";

var screenRes = null;

API.onUpdate.connect(() => {

	//ZoneStreet name.
	if (Date.now() >= posUpdateTick) {
		posUpdateTick = Date.now() + 1000;
		var pos = API.getEntityPosition(API.getLocalPlayer());
		lastStreet = API.getStreetName(pos);
		lastZone = API.getZoneName(pos);

		if(myBrowser !== null)
			myBrowser.call("setZoneStreet", lastStreet, lastZone);
	}

	//Direction
	var rot = API.getEntityRotation(API.getLocalPlayer());
	lastDirection = getDirectionName(rot);

	if (myBrowser !== null) {
		var vehicule = API.getPlayerVehicle(API.getLocalPlayer());
		var velocity = API.getEntityVelocity(vehicule);
		var speed = Math.sqrt(
			velocity.X * velocity.X +
			velocity.Y * velocity.Y +
			velocity.Z * velocity.Z
		);
		speed = Math.floor(speed * 3.6);
		myBrowser.call("setSpeed", speed);

		//Set dir.
		myBrowser.call("setDirection", lastDirection);

	} else {
		if (screenRes === null)
			screenRes = API.getScreenResolutionMantainRatio();

		if (lastDirection !== "")
			API.drawText(lastDirection, 310, screenRes.Height - 155, 1, 225, 225, 225, 255, 4, 0, false, true, 0);

		if(lastStreet !== "")
			API.drawText(lastStreet, 365, screenRes.Height - 150, 0.5, 225, 225, 225, 255, 4, 0, false, true, 0);

		if(lastZone !== "")
			API.drawText(lastZone, 365, screenRes.Height - 125, 0.5, 225, 225, 225, 255, 4, 0, false, true, 0);
	}
});