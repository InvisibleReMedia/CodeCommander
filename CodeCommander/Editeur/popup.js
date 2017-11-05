var countPopup = 0;

function getCountPopup()
{
	return countPopup;
}

function openPopup(objName)
{
	stack("popup");
	closeLegende(false);
	++countPopup;
	var container = document.getElementById('containerSrc');
	container.insertAdjacentHTML("beforeEnd", CreatePopup(objName));
	//document.body.insertAdjacentHTML("beforeEnd", CreatePopup());
	//document.body.insertAdjacentHTML("beforeEnd", "<div style='z-index: 99; display: none; position: absolute; left: 0; top: 0; width: 100%; height: 100%' id='popup" + countPopup + "'><table border='0' cellpadding='0' cellspacing='0' style='width: 100%; height: 100%'><tr><td align='center'><div id='window" + countPopup + "' style='position:relative; left:0; right:0; width: 100%; height: 100%; border: 1px solid #000000; background: #FFFFFF'><input type='button' value='Save' onclick='javascript:SaveTemplate();'/></div></td></tr></table></div>");
	document.getElementById("popup" + countPopup).style.zIndex = 98 + countPopup;
	document.getElementById(currentWindowId).style.display = "none";
	document.getElementById("popup" + countPopup).style.display = "block";
	currentWindowId = "popup" + countPopup;
	currentContentId = "windowContent" + countPopup;
	changeToPopup(countPopup);
	// set legendes
	pushLegendes(getElementByName(document.getElementById(objName), "myLegendes").id);
	var xmlString = getElementByName(document.getElementById(objName), "xml").innerHTML;
	var callback = document.getElementById("callback");
	callback.template = objName;
	callback.action = "open";
	callback.data = xmlString;
	callback.click();
}

function ClosePopup()
{
	clearUndoUntilPopup();
	closeLegende(false);
	popLegendes();
	document.getElementById("popup" + countPopup).style.display = "none";
	var pop = document.getElementById("popup" + countPopup);
	var container = document.getElementById("containerSrc");
	container.removeChild(pop);
	--countPopup;
	changeToPopup(countPopup);
	if (countPopup > 0)
	{
		currentWindowId = "popup" + countPopup;
		currentContentId = "windowContent" + countPopup;
		document.getElementById(currentWindowId).style.display = "block";
	}
	else
	{
		currentWindowId = "windowTop";
		currentContentId = "src";
		document.getElementById(currentWindowId).style.display = "block";
	}
}

function SaveTemplate(name)
{
	clearUndoUntilPopup();
	closeLegende(false);
	callback.action = "close";
	callback.window = "windowContent" + countPopup;
	callback.template = name; 
	callback.click();
	popLegendes();
	document.getElementById("popup" + countPopup).style.display = "none";
	var pop = document.getElementById("popup" + countPopup);
	var container = document.getElementById("containerSrc");
	container.removeChild(pop);
	--countPopup;
	changeToPopup(countPopup);
	if (countPopup > 0)
	{
		MakeHiddenClose();
		currentWindowId = "popup" + countPopup;
		currentContentId = "windowContent" + countPopup;
		document.getElementById(currentWindowId).style.display = "block";
	}
	else
	{
		currentWindowId = "windowTop";
		currentContentId = "src";
		document.getElementById(currentWindowId).style.display = "block";
	}
}

function endWithPaste()
{
	var win = document.getElementById(currentContentId);
	win.insertAdjacentHTML("beforeEnd", CreatePaste(0));
}

function suppressEndPaste()
{
	var win = document.getElementById(currentContentId);
	win.removeChild(win.lastChild);
}

function CloseDocument()
{
	if (countPopup > 0)
	{
		closeLegende(false);
		popLegendes();
		document.getElementById("popup" + countPopup).style.display = "none";
		var pop = document.getElementById("popup" + countPopup);
		var container = document.getElementById("containerSrc");
		container.removeChild(pop);
		countPopup = 0;
		changeToPopup(countPopup);
		currentWindowId = "windowTop";
		currentContentId = "src";
		document.getElementById(currentWindowId).style.display = "block";
	}
}

function MakeHiddenClose()
{
	var obj = document.getElementById("closePopup" + countPopup);
	obj.style.display = "none";
}

function isHiddenClose()
{
	if (countPopup > 0)
	{
		var obj = document.getElementById("closePopup" + countPopup);
		if (obj.style.display == "none")
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	else
	{
		return false;
	}
}
