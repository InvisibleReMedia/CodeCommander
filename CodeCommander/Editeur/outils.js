var lock = new Array();
var lockLegende = 0;
var lockMenuTools = 0;
var arrColors;
var arrSelectionColors;
var arrContours;
var arrTime;
var arrOpacity;
var arrSize;
var arrImgSize;
var lockTemplateSearch = 0;
var lockSyntaxSearch = 0;
var lockMOPSearch = 0;
var lockPaste = 0;
var currentSelectionTemplateSearch;
var currentSelectionMOPSearch;
var currentSelectionSyntaxSearch;
var currentSelectionPaste;
arrColors = new Array('#D3ECFA', '#F3EDFC', '#F3F1FE', '#F2F2FF', '#F1F1FF');
arrSelectionColors = new Array('#FFFFFF', '#F1F1FF', '#E1E1FF', '#D1D1FF', '#C1C1FF');
arrContours = new Array('#FFFFFF', '#FFFCFC', '#FFF8F8', '#FFF2F2', '#FFF1F1');
arrTime = new Array(200, 100, 100, 100, 50, 10);
arrOpacity = new Array(10, 30, 40, 80, 100);
arrPositionLegende = new Array(-100, -80, -70, -65, -40, -30, 20, 30, 43);
arrSize = new Array(1, 1, 2, 2, 3, 4, 4, 5, 6, 6, 7, 7, 7, 8, 8, 8, 9, 9, 9, 11, 11, 12, 12, 15, 16, 18, 19);
arrImgSize = new Array(2, 2, 4, 4, 6, 6, 8, 8, 10, 10, 11, 11, 12, 12, 14, 14, 16, 16, 18, 18, 20, 20, 22, 22, 24, 24, 26);
var onSelection = false;
var isDirty = false;
var currentWindowId = "windowTop";
var currentContentId = "src";
var whereToPaste = "";

function increaseFontSize() {
	var f = parseInt(document.styleSheets[0].rules[1].style.fontSize);
	if (f < 30) {
		++f;
		document.styleSheets[0].rules[1].style.fontSize = f.toString() + "pt";
		document.styleSheets[0].rules[1].style.lineHeight = (f+2).toString() + "pt";
		document.styleSheets[0].rules[2].style.width = arrImgSize[f - 4].toString() + "pt";
		document.styleSheets[0].rules[2].style.height = arrImgSize[f - 4].toString() + "pt";
		document.styleSheets[0].rules[3].style.height = (f + 2).toString() + "pt";
		document.styleSheets[0].rules[17].style.fontSize = arrSize[f - 4].toString() + "pt";
		document.styleSheets[0].rules[17].style.height = (arrSize[f - 4] + 8).toString() + "pt";
		document.styleSheets[0].rules[18].style.fontSize = arrSize[f - 4].toString() + "pt";
		document.styleSheets[0].rules[18].style.height = (arrSize[f - 4] + 5).toString() + "pt";
	}
}

function decreaseFontSize() {
	var f = parseInt(document.styleSheets[0].rules[1].style.fontSize);
	if (f > 4) {
		--f;
		document.styleSheets[0].rules[1].style.fontSize = f.toString() + "pt";
		document.styleSheets[0].rules[1].style.lineHeight = (f + 2).toString() + "pt";
		document.styleSheets[0].rules[2].style.width = arrImgSize[f - 4].toString() + "pt";
		document.styleSheets[0].rules[2].style.height = arrImgSize[f - 4].toString() + "pt";
		document.styleSheets[0].rules[3].style.height = (f + 2).toString() + "pt";
		document.styleSheets[0].rules[17].style.fontSize = arrSize[f - 4].toString() + "pt";
		document.styleSheets[0].rules[17].style.height = (arrSize[f - 4] + 8).toString() + "pt";
		document.styleSheets[0].rules[18].style.fontSize = arrSize[f - 4].toString() + "pt";
		document.styleSheets[0].rules[18].style.height = (arrSize[f - 4] + 5).toString() + "pt";
	}
}

function pageResize()
{
	var obj = document.getElementById("containerSrc");
	if (obj.offsetWidth < screen.availWidth)
	{
		document.body.width = screen.availWidth;
	}
	else
	{
		document.body.width = obj.offsetWidth;
	}
	if (obj.offsetHeight < screen.availHeight)
	{
		document.body.height = screen.availHeight;
	}
	else
	{
		document.body.height = obj.offsetHeight;
	}
}

function selectionStart()
{
//	onSelection = true;
}

function selectionEnd()
{
//	onSelection = false;
}

function selectionMove()
{
//	if (onSelection)
//	{
//		document.selection.clear();
//	}
}

function continueOpenLegende(count) {
	if (lockLegende == 1)
	{
		if (count < 8)
		{
			var obj = document.getElementById("legende");
			obj.style.top = (document.body.scrollTop + arrPositionLegende[count]) + "px";
			setTimeout('continueOpenLegende(' + (count+1) + ')', 10);
		} else {
			var obj = document.getElementById("legende");
			obj.style.top = document.body.scrollTop;
			// indication que la fenêtre legende est ouverte
			lockLegende = 2;
		}
	}
}

function continueCloseLegende(count) {
	if (lockLegende == 3)
	{
		if (count > 0)
		{
			var obj = document.getElementById("legende");
			obj.style.top = (document.body.scrollTop + arrPositionLegende[count]).toString() + "px";
			setTimeout('continueCloseLegende(' + (count-1) + ')', 10);
		} else {
			var obj = document.getElementById("legende");
			obj.style.display = "none";
			lockLegende = 0;
		}
	}
}

function closeLegende(forSave)
{
	// si la fenêtre legende est ouverte
	if (lockLegende > 0)
	{
		if (forSave)
		{
			GetLegendeIHM();
		}
		lockLegende = 3;
		continueCloseLegende(7);
	}
}

function openLegende(value, forcedType, context)
{
	var objName;
	var contextName;
	if (context)
	{
		contextName = document.getElementById(context).innerText;
		objName = document.getElementById(value).innerText;
	}
	else
	{
		contextName = "";
		objName = document.getElementById(value).innerText;
	}
	SetLegendeIHM(contextName, objName, forcedType);
	centerLegende();
	var obj = document.getElementById("legende");
	obj.style.top = "-100px";
	obj.style.display = "block";
	lockLegende = 1;
	continueOpenLegende(0);
}

function centerLegende()
{
	var obj = document.getElementById("legende");
	obj.style.left = (document.body.offsetWidth - 300) / 2;
}

function getOffsetLeft(obj)
{
	if (obj)
		return getOffsetLeft(obj.offsetParent) + obj.offsetLeft;
	else
		return 0;
}

function getOffsetRight(obj)
{
	if (obj)
		return getOffsetRight(obj.offsetParent) + obj.offsetLeft + obj.offsetWidth;
	else
		return 0;
}

function getOffsetTop(obj)
{
	if (obj)
		return getOffsetTop(obj.offsetParent) + obj.offsetTop;
	else
		return 0;
}

function noScrollLegende()
{
	var obj = document.getElementById("legende");
	obj.style.top = document.body.scrollTop;
	obj = document.getElementById("pasteZone");
	obj.style.top = getOffsetTop(document.body) + document.body.scrollTop;
	obj = document.getElementById("syntaxSearch");
	obj.style.top = getOffsetTop(document.body) + document.body.scrollTop;
}

function openPaste()
{
	if (!lockPaste)
	{
		lockPaste = 1;
		var obj = document.getElementById("pasteZone");
		obj.style.left = getOffsetRight(document.body) - 500;
		obj.style.top = getOffsetTop(document.body) + document.body.scrollTop;
		obj.style.display = "block";
	}
	callback.action = "store";
	callback.repositoryId = "divPasteAndStore";
	callback.click();
	initDragAndDrop("paste", "pasteAndStore");
}

function closePaste()
{
	if (lockPaste)
	{
		var obj = document.getElementById("pasteZone");
		obj.style.display = "none";
		lockPaste = 0;
	}
}

function openSyntaxSearch()
{
	if (!lockSyntaxSearch)
	{
		lockSyntaxSearch = 1;
		var obj = document.getElementById("syntaxSearch");
		obj.style.left = getOffsetRight(document.body) - 320;
		obj.style.top = getOffsetTop(document.body) + document.body.scrollTop;
		obj.style.display = "block";
		callback.action = "search";
		var text = document.getElementById("areaSyntaxSearch");
		callback.searchString = text.value;
		callback.repositoryId = "divSyntaxSearch";
		callback.repositoryType = "syntax";
		callback.click();
		initDragAndDrop("syntax", "syntax");
	}
}

function closeSyntaxSearch()
{
	if (lockSyntaxSearch)
	{
		var obj = document.getElementById("syntaxSearch");
		obj.style.display = "none";
		lockSyntaxSearch = 0;
	}
}

function selectSyntax(data)
{
	callback.action = "runSyntax";
	callback.syntaxName = data;
	callback.click();
}

function onChangeSelectionSyntaxSearch(obj)
{
	// on efface la selection en cours
	if (currentSelectionSyntaxSearch > 0 && currentSelectionSyntaxSearch != obj.index)
	{
		var old = document.getElementById("tableSyntaxSearch");
		old.rows[currentSelectionSyntaxSearch].className = "syntaxRow";
	}
	// on change la selection en cours
	currentSelectionSyntaxSearch = obj.index;
}

function onChangeTextForSyntaxSearch()
{
	callback.action = "search";
	var text = document.getElementById("areaSyntaxSearch");
	callback.searchString = text.value;
	callback.repositoryId = "divSyntaxSearch";
	callback.repositoryType = "syntax";
	callback.click();
	initDragAndDrop("syntax", "syntax");
}

function openTemplateSearch(toolId, id)
{
	if (!lockTemplateSearch)
	{
		lockTemplateSearch = 1;
		currentSelectionTemplateSearch = 0;
	}
	var obj = document.getElementById("templateSearch");
	var child = document.getElementById(id);
	// on réinitialise si c'est un autre objet que le précédent
	if (obj.refId != child.id)
	{
		obj.refId = child.id;
		currentSelectionTemplateSearch = 0;
	}
	obj.style.left = getOffsetLeft(child);
	if (child.offsetWidth > 300)
	{
		obj.style.width = child.offsetWidth;
	}
	else
	{
		obj.style.width = 300;
	}
	obj.style.top = getOffsetTop(child) + child.offsetHeight;
	obj.style.display = "block";
	// si je n'ai pas appuyé sur les touches haut ou bas
	if (window.event.keyCode != 40 && window.event.keyCode != 38)
	{
		// on réinitialise le compteur de lignes
		currentSelectionTemplateSearch = 0;
		callback.action = "search";
		callback.searchString = child.innerText;
		callback.repositoryId = "templateSearch";
		callback.repositoryType = "template";
		callback.click();
	}
}

function closeTemplateSearch()
{
	if (lockTemplateSearch)
	{
		var obj = document.getElementById("templateSearch");
		obj.style.display = "none";
		lockTemplateSearch = 0;
	}
}

function openMOPSearch(toolId, id)
{
	if (!lockMOPSearch)
	{
		lockMOPSearch = 1;
		currentSelectionMOPSearch = 0;
	}
	var obj = document.getElementById("MOPSearch");
	var child = document.getElementById(id);
	// on réinitialise si c'est un autre objet que le précédent
	if (obj.refId != child.id)
	{
		obj.refId = child.id;
		currentSelectionMOPSearch = 0;
	}
	obj.style.left = getOffsetLeft(child);
	if (child.offsetWidth > 300)
	{
		obj.style.width = child.offsetWidth;
	}
	else
	{
		obj.style.width = 300;
	}
	obj.style.top = getOffsetTop(child) + child.offsetHeight;
	obj.style.display = "block";
	// si je n'ai pas appuyé sur les touches haut ou bas
	if (window.event.keyCode != 40 && window.event.keyCode != 38)
	{
		// on réinitialise le compteur de lignes
		currentSelectionMOPSearch = 0;
		callback.action = "search";
		callback.searchString = child.innerText;
		callback.repositoryId = "MOPSearch";
		callback.repositoryType = "mop";
		callback.click();
	}
}

function closeMOPSearch()
{
	if (lockMOPSearch)
	{
		var obj = document.getElementById("MOPSearch");
		obj.style.display = "none";
		lockMOPSearch = 0;
	}
}

function selectMOP(data)
{
	var obj = document.getElementById("MOPSearch");
	var child = document.getElementById(obj.refId);
	child.innerText = data;
	closeMOPSearch();
}

function selectTemplate(data)
{
	var obj = document.getElementById("templateSearch");
	var child = document.getElementById(obj.refId);
	child.innerText = data;
	closeTemplateSearch();
}

function onChangeSelectionTemplateSearch(obj)
{
	// on efface la selection en cours
	if (currentSelectionTemplateSearch > 0)
	{
		var obj = document.getElementById("tableTemplateSearch");
		obj.rows[currentSelectionTemplateSearch].className = "templateRow";
	}
	// on change la selection en cours
	currentSelectionTemplateSearch = obj.index;
}

function onChangeTextForTemplateSearch(toolId, obj)
{
	openTemplateSearch(toolId, obj.id);
}

function onChangeSelectForTemplateSearch()
{
	if (lockTemplateSearch)
	{
		// fleche du bas
		if (window.event.keyCode == 40)
		{
			var obj = document.getElementById("tableTemplateSearch");
			// obj.row.length contient une dernière ligne vide
			if (currentSelectionTemplateSearch < (obj.rows.length - 2))
			{
				if (currentSelectionTemplateSearch > 0)
				{
					obj.rows[currentSelectionTemplateSearch].className = "templateRow";
				}
				currentSelectionTemplateSearch++;
				obj.rows[currentSelectionTemplateSearch].className = "templateRowOver";
				obj.rows[currentSelectionTemplateSearch].scrollIntoView(true);
			}
		}
		// fleche du haut
		else if (window.event.keyCode == 38)
		{
			if (currentSelectionTemplateSearch > 1)
			{
				var obj = document.getElementById("tableTemplateSearch");
				obj.rows[currentSelectionTemplateSearch].className = "templateRow";
				currentSelectionTemplateSearch--;
				obj.rows[currentSelectionTemplateSearch].className = "templateRowOver";
				obj.rows[currentSelectionTemplateSearch].scrollIntoView(false);
			}
		}
		// touche entrée
		else if (window.event.keyCode == 13)
		{
			if (currentSelectionTemplateSearch > 0)
			{
				window.event.returnValue = false;
				var obj = document.getElementById("tableTemplateSearch");
				selectTemplate(obj.rows[currentSelectionTemplateSearch].data);
			}
		}
	}
}

function onChangeTextForMOPSearch(toolId, obj)
{
	openMOPSearch(toolId, obj.id);
}

function onChangeSelectForMOPSearch()
{
	if (lockMOPSearch)
	{
		// fleche du bas
		if (window.event.keyCode == 40)
		{
			var obj = document.getElementById("tableMOPSearch");
			// obj.row.length contient une dernière ligne vide
			if (currentSelectionMOPSearch < (obj.rows.length - 2))
			{
				if (currentSelectionMOPSearch > 0)
				{
					obj.rows[currentSelectionMOPSearch].className = "templateRow";
				}
				currentSelectionMOPSearch++;
				obj.rows[currentSelectionMOPSearch].className = "templateRowOver";
				obj.rows[currentSelectionMOPSearch].scrollIntoView(true);
			}
		}
		// fleche du haut
		else if (window.event.keyCode == 38)
		{
			if (currentSelectionMOPSearch > 1)
			{
				var obj = document.getElementById("tableMOPSearch");
				obj.rows[currentSelectionMOPSearch].className = "templateRow";
				currentSelectionMOPSearch--;
				obj.rows[currentSelectionMOPSearch].className = "templateRowOver";
				obj.rows[currentSelectionMOPSearch].scrollIntoView(false);
			}
		}
		// touche entrée
		else if (window.event.keyCode == 13)
		{
			if (currentSelectionMOPSearch > 0)
			{
				window.event.returnValue = false;
				var obj = document.getElementById("tableMOPSearch");
				selectMOP(obj.rows[currentSelectionMOPSearch].data);
			}
		}
	}
}

function continueStarEnter(name, count) {
    if (lock[name] == 1) {
        if (count < 6) {
            var obj;
            obj = document.getElementById(name);
            if (count % 2) {
                obj.style.border = '1px solid ' + arrContours[4];
            } else {
                obj.style.border = '1px solid red';
            }
            setTimeout('continueStarEnter(\'' + name + '\', ' + (count + 1) + ')', arrTime[count]);
        } else {
            lock[name] = 0;
            var obj;
            obj = document.getElementById(name);
            obj.style.border = '1px solid red';
        }
    }
}

function continueImageEnter(name, count) {
    if (lock[name] == 1) {
        if (count < 6) {
            var obj;
            obj = document.getElementById(name);
            if (obj)
            {
				if (count % 2) {
					obj.style.border = '1px solid ' + arrContours[4];
				} else {
					obj.style.border = '1px solid red';
				}
	            setTimeout('continueImageEnter(\'' + name + '\', ' + (count + 1) + ')', arrTime[count]);
	        }
        } else {
            lock[name] = 0;
        }
    }
}

function continueImageEnterTools(name, next, direction, count) {
    if (lock[name] == 1) {
        if (count < 6) {
            var obj;
            obj = document.getElementById(name);
            if (count % 2) {
                obj.style.border = '1px solid ' + arrContours[4];
            } else {
                obj.style.border = '1px solid red';
            }
            setTimeout('continueImageEnterTools(\'' + name + '\', \'' + next + '\', \'' + direction + '\', ' + (count + 1) + ')', arrTime[count]);
        } else {
            lock[name] = 0;
            openMenuTools(name, next, direction);
        }
    }
}

function continueToolEnter(name, type, count) {
    if (lock[name] == 1) {
        var obj;
        obj = document.getElementById(name);
        if (!onSelection)
        {
			if (count < arrColors.length) {
				obj.style.backgroundColor = arrColors[count];
				if (type == 'variable') {
					var div = document.getElementById('divcontent' + name);
					div.style.border = '1px outset ' + arrContours[count];
				} else if (type == 'champ') {
					var div = document.getElementById('divexpr' + name);
					div.style.border = '1px outset ' + arrContours[count];
				} else if (type == 'condition') {
					var div = document.getElementById('divexpr' + name);
					div.style.border = '1px outset ' + arrContours[count];
					var div = document.getElementById('divtrue' + name);
					div.style.border = '1px outset ' + arrContours[count];
					var div = document.getElementById('divfalse' + name);
					div.style.border = '1px outset ' + arrContours[count];
				} else if (type == 'chaine') {
					var div = document.getElementById('divvar' + name);
					div.style.border = '1px outset ' + arrContours[count];
					var div = document.getElementById('divstring' + name);
					div.style.border = '1px outset ' + arrContours[count];
				} else if (type == 'affectation') {
					var div = document.getElementById('divvar' + name);
					div.style.border = '1px outset ' + arrContours[count];
					var div = document.getElementById('divexpr' + name);
					div.style.border = '1px outset ' + arrContours[count];
				} else if (type == 'template') {
					var div = document.getElementById('divpath' + name);
					div.style.border = '1px outset ' + arrContours[count];
					var div = document.getElementById('divparams' + name);
					div.style.border = '1px outset ' + arrContours[count];
				} else if (type == 'mop') {
					var div = document.getElementById('divlanguage' + name);
					div.style.border = '1px outset ' + arrContours[count];
					var div = document.getElementById('divname' + name);
					div.style.border = '1px outset ' + arrContours[count];
					var div = document.getElementById('divparams' + name);
					div.style.border = '1px outset ' + arrContours[count];
				} else if (type == 'syntax') {
					var div = document.getElementById('divname' + name);
					div.style.border = '1px outset ' + arrContours[count];
				} else if (type == 'createWriter') {
					var div = document.getElementById('divname' + name);
					div.style.border = '1px outset ' + arrContours[count];
					var div = document.getElementById('divfile' + name);
					div.style.border = '1px outset ' + arrContours[count];
				}
				setTimeout('continueToolEnter(\'' + name + '\', \'' + type + '\', ' + (count + 1) + ')', 100);
			} else {
				lock[name] = 0;
			}
		}
    }
}

function continueToolLeave(name, type, count) {
    if (lock[name] == 2) {
        var obj;
        obj = document.getElementById(name);
        if (!onSelection)
        {
			if (count >= 0) {
				if (obj)
				{
					obj.style.backgroundColor = arrColors[count];
					if (type == 'variable') {
						var div = document.getElementById('divcontent' + name);
						div.style.border = '1px solid white';
					} else if (type == 'champ') {
						var div = document.getElementById('divexpr' + name);
						div.style.border = '1px solid white';
					} else if (type == 'condition') {
						var div = document.getElementById('divexpr' + name);
						div.style.border = '1px solid white';
						var div = document.getElementById('divtrue' + name);
						div.style.border = '1px solid white';
						var div = document.getElementById('divfalse' + name);
						div.style.border = '1px solid white';
					} else if (type == 'chaine') {
						var div = document.getElementById('divvar' + name);
						div.style.border = '1px solid white';
						var div = document.getElementById('divstring' + name);
						div.style.border = '1px solid white';
					} else if (type == 'affectation') {
						var div = document.getElementById('divvar' + name);
						div.style.border = '1px solid white';
						var div = document.getElementById('divexpr' + name);
						div.style.border = '1px solid white';
					} else if (type == 'template') {
						var div = document.getElementById('divpath' + name);
						div.style.border = '1px solid white';
						var div = document.getElementById('divparams' + name);
						div.style.border = '1px solid white';
					} else if (type == 'mop') {
						var div = document.getElementById('divlanguage' + name);
						div.style.border = '1px solid white';
						var div = document.getElementById('divname' + name);
						div.style.border = '1px solid white';
						var div = document.getElementById('divparams' + name);
						div.style.border = '1px solid white';
					} else if (type == 'syntax') {
						var div = document.getElementById('divname' + name);
						div.style.border = '1px solid white';
					} else if (type == 'createWriter') {
						var div = document.getElementById('divname' + name);
						div.style.border = '1px solid white';
						var div = document.getElementById('divfile' + name);
						div.style.border = '1px solid white';
					}
					setTimeout('continueToolLeave(\'' + name + '\', \'' + type + '\', ' + (count - 1) + ')', 100);
				}
			} else {
				lock[name] = 0;
				var img = document.getElementById('imgfld' + obj.id);
				img.src = "flv.jpg";
				img.style.border = '1px solid white';
				img = document.getElementById('imgflg' + obj.id);
				img.src = "flv.jpg";
				img.style.border = '1px solid white';
				img = document.getElementById('imgsup' + obj.id);
				img.src = "flv.jpg";
				img.style.border = '1px solid white';
			}
		}
    }
}

function continueOpenMenuTools(name, count) {
    if (lockMenuTools) {
        var obj;
        obj = document.getElementById(name);
        if (count < arrOpacity.length) {
			obj.style.filter = 'alpha(opacity=' + arrOpacity[count] + ')';
            setTimeout('continueOpenMenuTools(\'' + name + '\', ' + (count + 1) + ')', 100);
        }
    }
}

function toolEnter(obj, type) {
	if (onSelection)
	{
		obj.style.borderColor = '#C1C1FF';
		obj.selected = true;
	}
	else
	{
		lock[obj.id] = 1;
		var img = document.getElementById('imgfld' + obj.id);
		img.src = "fld.jpg";
		img.style.border = '1px solid ' + arrContours[4];
		img = document.getElementById('imgflg' + obj.id);
		img.src = "flg.jpg";
		img.style.border = '1px solid ' + arrContours[4];
		img = document.getElementById('imgsup' + obj.id);
		img.src = "sup.jpg";
		img.style.border = '1px solid ' + arrContours[4];
		continueToolEnter(obj.id, type, 0);
	}
}

function toolLeave(obj, type) {
	if (!onSelection)
	{
	    lock[obj.id] = 2;
		continueToolLeave(obj.id, type, arrColors.length - 1);
	}
}

function starEnter(obj) {
    lock[obj.id] = 1;
    continueStarEnter(obj.id, 0);
}

function starLeave(obj) {
    lock[obj.id] = 2;
    obj.style.border = '1px solid white';
}

function imageEnter(obj) {
    lock[obj.id] = 1;
    continueImageEnter(obj.id, 0);
}

function imageLeave(obj) {
    lock[obj.id] = 2;
    obj.style.border = '1px solid white';
}

function imageEnterTools(obj, name, direction) {
    lock[obj.id] = 1;
    continueImageEnterTools(obj.id, name, direction, 0);
}

function imageLeaveTools(obj) {
    lock[obj.id] = 2;
    obj.style.border = '1px solid white';
}


function keyDown()
{
	if (window.event.keyCode == 9)
	{
		var range = document.selection.createRange();
		range.text = String.fromCharCode(172);
	    window.event.returnValue = false;
	}
}

function keyPressed() {
    if (window.event.ctrlKey) {
    } else if (window.event.keyCode == 13) {
        if (window.event.shiftKey) {
            window.event.returnValue = true;
        } else {
			var range = document.selection.createRange();
            document.execCommand("InsertParagraph", true, "");
            var childs = window.event.srcElement.childNodes;
            for (i = childs.length - 1; i >= 0; --i) {
                if (childs[i].tagName == "P") {
                    var child = document.createElement("span");
                    child.innerHTML = String.fromCharCode(182) + "<br/>";
                    window.event.srcElement.insertBefore(child, childs[i]);
                    window.event.srcElement.removeChild(childs[i+1]);
                }
            }
            window.event.returnValue = false;
        }
    } else if (window.event.keyCode == 32) {
        window.event.keyCode = 183;
        window.event.returnValue = true;
    }
}

function toolTextContextMenu(obj, name) {
	if (!lockMenuTools) {
		lockMenuTools = 1;
		var cButton = document.getElementById("cutButton");
		cButton.style.display = "block";
		var x = window.event.clientX;
		var y = window.event.clientY;
		var menu = document.getElementById('menuTools');
		menu.direction = "inner";
		menu.clickRef = name;
		menu.textRef = obj.id;
		menu.indent = parseInt(document.getElementById(name).indent);
		menu.style.display = 'block';
		var left,top;
		left = document.body.scrollLeft + x;
		top  = document.body.scrollTop + y;
		if (left < 0)
			left = 0;
		if (top < 0)
			top = 0;
		menu.style.left = left;
		menu.style.top  = top;
		continueOpenMenuTools(menu.id, 0);
	}
	window.event.returnValue = false;
}

function openMenuTools(name, next, direction) {
	if (!lockMenuTools) {
		lockMenuTools = 1;
		var cButton = document.getElementById("cutButton");
		cButton.style.display = "none";
		var obj = document.getElementById('menuTools');
		var img = document.getElementById(name);
		obj.direction = direction;
		var nextObj = document.getElementById(next);
		var indent = parseInt(nextObj.indent);
		if (nextObj.type == "coding" || nextObj.type == "parallel")
		{
			obj.indent = indent + 1;
		}
		else
		{
			obj.indent = indent;
		}
		// dans le cas d'un usetemplate, on va chercher l'objet plus en bas
		// qui n'est pas indenté
		if (direction == "after" && nextObj.type == "usetemplate")
		{
			var myIndent = indent;
			while(nextObj.nextSibling && (nextObj.nextSibling.type == "paste" || parseInt(nextObj.nextSibling.indent) > myIndent))
			{
				nextObj = nextObj.nextSibling;
			}
			next = nextObj.previousSibling.id;
		}
		obj.clickRef = next;
		obj.style.display = 'block';
		var left,top;
		left = document.body.scrollLeft + img.getBoundingClientRect().left - (obj.offsetWidth  / 2);
		top  = document.body.scrollTop  + img.getBoundingClientRect().top  - (obj.offsetHeight / 2);
		if (left < 0)
			left = 0;
		if (top < 0)
			top = 0;
		obj.style.left = left;
		obj.style.top  = top;
		continueOpenMenuTools(obj.id, 0);
	}
}

function closeMenuTools() {
	var obj = document.getElementById('menuTools');
	obj.style.filter = 'alpha(opacity=0)';
	obj.style.display = 'none';
	lockMenuTools = 0;
}

function dumpHtml(name)
{
	var obj = document.getElementById(name);
	alert(obj.innerHTML);
}

function dump(bin)
{
	var output = "";
	for(index = 0; index < bin.length; ++index)
	{
		output += bin.charCodeAt(index).toString(2) + ".";
	}
	return output;
}

function insertText(text) {
	var menuTools = document.getElementById("menuTools");
	var refObj = document.getElementById(menuTools.clickRef);
	var textObj = document.getElementById(menuTools.textRef);
	var indentation = menuTools.indent;
	switch(menuTools.direction) {
		case 'inner':
			var range = document.selection.createRange();
			var sr = range.duplicate();
			sr.moveToElementText(textObj);
			sr.setEndPoint('EndToEnd', range);
			var selectionStart = sr.text.length - range.text.length;
			var selText = document.selection.createRange().text;
			if (selectionStart != -1 && selectionEnd != -1)
			{
				stack("beforeInsert", refObj);
				var before = textObj.innerText.substring(0, selectionStart);
				var after = textObj.innerText.substring(selectionStart + selText.length);
				refObj.insertAdjacentHTML('beforeBegin', CreateText(before, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.insertAdjacentHTML('beforeBegin', CreateText(selText, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateText(after, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.previousSibling.previousSibling.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
				toolLeave(refObj.id, "texte");
				stack("afterInsert", refObj.previousSibling.previousSibling);
				removeNoStack(refObj.id);
			}
			break;
		case 'before':
		    refObj.insertAdjacentHTML('beforeBegin', CreateText(text, indentation, false));
		    refObj.insertAdjacentHTML('beforeBegin', CreatePaste(indentation));
		    stack("insert", refObj.previousSibling.previousSibling);
		    insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
		    break;
		case 'after':
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreatePaste(indentation));
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateText(text, indentation, false));
		    stack("insert", refObj.nextSibling.nextSibling);
		    insertZoneSelection(refObj.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
	}
	menuTools.style.filter = 'alpha(opacity=0)';
	menuTools.style.display = 'none';
	lockMenuTools = 0;
}

function displayText(text, indent, readonly)
{
	if (!whereToPaste)
	{
		var refObj = document.getElementById(currentContentId);
		refObj.insertAdjacentHTML('beforeEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('beforeEnd', CreateText(text, indent, readonly));
		addZoneSelection(refObj.lastChild.id);
	}
	else
	{
		var refObj = document.getElementById(whereToPaste);
		refObj.insertAdjacentHTML('afterEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('afterEnd', CreateText(text, indent, readonly));
		if (refObj.previousSibling)
		{
			insertZoneSelection(refObj.previousSibling.id, 'afterEnd', refObj.nextSibling.id);
		}
		else
		{
			insertZoneSelection(refObj.nextSibling.nextSibling.nextSibling.id, 'beforeBegin', refObj.nextSibling.id);
		}
		whereToPaste = refObj.nextSibling.nextSibling.id;
	}
}

function insertIndent() {
	var menuTools = document.getElementById("menuTools");
	var refObj = document.getElementById(menuTools.clickRef);
	var textObj = document.getElementById(menuTools.textRef);
	var indentation = menuTools.indent;
	switch(menuTools.direction) {
		case 'inner':
			var range = document.selection.createRange();
			var sr = range.duplicate();
			sr.moveToElementText(textObj);
			sr.setEndPoint('EndToEnd', range);
			var selectionStart = sr.text.length - range.text.length;
			var selText = document.selection.createRange().text;
			if (selectionStart != -1 && selectionEnd != -1)
			{
				stack("beforeInsert", refObj);
				var before = textObj.innerText.substring(0, selectionStart);
				var after = textObj.innerText.substring(selectionStart + selText.length);
				refObj.insertAdjacentHTML('beforeBegin', CreateText(before, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.insertAdjacentHTML('beforeBegin', CreateIndent(indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateText(after, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.previousSibling.previousSibling.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
				toolLeave(refObj.id, "variable");
				stack("afterInsert", refObj.previousSibling.previousSibling);
				removeNoStack(refObj.id);
			}
			break;
		case 'before':
		    refObj.insertAdjacentHTML('beforeBegin', CreateIndent(indentation, false));
		    refObj.insertAdjacentHTML('beforeBegin', CreatePaste(indentation));
		    stack("insert", refObj.previousSibling.previousSibling);
		    insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
		    break;
		case 'after':
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreatePaste(indentation));
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateIndent(indentation, false));
		    stack("insert", refObj.nextSibling.nextSibling);
		    insertZoneSelection(refObj.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
	}
	menuTools.style.filter = 'alpha(opacity=0)';
	menuTools.style.display = 'none';
	lockMenuTools = 0;
}

function displayIndent(indent, readonly)
{
	if (!whereToPaste)
	{
		var refObj = document.getElementById(currentContentId);
		refObj.insertAdjacentHTML('beforeEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('beforeEnd', CreateIndent(indent, readonly));
		addZoneSelection(refObj.lastChild.id);
	}
	else
	{
		var refObj = document.getElementById(whereToPaste);
		refObj.insertAdjacentHTML('afterEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('afterEnd', CreateIndent(indent, readonly));
		if (refObj.previousSibling)
		{
			insertZoneSelection(refObj.previousSibling.id, 'afterEnd', refObj.nextSibling.id);
		}
		else
		{
			insertZoneSelection(refObj.nextSibling.nextSibling.nextSibling.id, 'beforeBegin', refObj.nextSibling.id);
		}
		whereToPaste = refObj.nextSibling.nextSibling.id;
	}
}

function insertUnindent() {
	var menuTools = document.getElementById("menuTools");
	var refObj = document.getElementById(menuTools.clickRef);
	var textObj = document.getElementById(menuTools.textRef);
	var indentation = menuTools.indent;
	switch(menuTools.direction) {
		case 'inner':
			var range = document.selection.createRange();
			var sr = range.duplicate();
			sr.moveToElementText(textObj);
			sr.setEndPoint('EndToEnd', range);
			var selectionStart = sr.text.length - range.text.length;
			var selText = document.selection.createRange().text;
			if (selectionStart != -1 && selectionEnd != -1)
			{
				stack("beforeInsert", refObj);
				var before = textObj.innerText.substring(0, selectionStart);
				var after = textObj.innerText.substring(selectionStart + selText.length);
				refObj.insertAdjacentHTML('beforeBegin', CreateText(before, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.insertAdjacentHTML('beforeBegin', CreateUnindent(indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateText(after, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.previousSibling.previousSibling.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
				toolLeave(refObj.id, "variable");
				stack("afterInsert", refObj.previousSibling.previousSibling);
				removeNoStack(refObj.id);
			}
			break;
		case 'before':
		    refObj.insertAdjacentHTML('beforeBegin', CreateUnindent(indentation, false));
		    refObj.insertAdjacentHTML('beforeBegin', CreatePaste(indentation));
		    stack("insert", refObj.previousSibling.previousSibling);
		    insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
		    break;
		case 'after':
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreatePaste(indentation));
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateUnindent(indentation, false));
		    stack("insert", refObj.nextSibling.nextSibling);
		    insertZoneSelection(refObj.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
	}
	menuTools.style.filter = 'alpha(opacity=0)';
	menuTools.style.display = 'none';
	lockMenuTools = 0;
}

function displayUnindent(indent, readonly)
{
	if (!whereToPaste)
	{
		var refObj = document.getElementById(currentContentId);
		refObj.insertAdjacentHTML('beforeEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('beforeEnd', CreateUnindent(indent, readonly));
		addZoneSelection(refObj.lastChild.id);
	}
	else
	{
		var refObj = document.getElementById(whereToPaste);
		refObj.insertAdjacentHTML('afterEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('afterEnd', CreateUnindent(indent, readonly));
		if (refObj.previousSibling)
		{
			insertZoneSelection(refObj.previousSibling.id, 'afterEnd', refObj.nextSibling.id);
		}
		else
		{
			insertZoneSelection(refObj.nextSibling.nextSibling.nextSibling.id, 'beforeBegin', refObj.nextSibling.id);
		}
		whereToPaste = refObj.nextSibling.nextSibling.id;
	}
}

function insertVariable() {
	var menuTools = document.getElementById("menuTools");
	var refObj = document.getElementById(menuTools.clickRef);
	var textObj = document.getElementById(menuTools.textRef);
	var indentation = menuTools.indent;
	switch(menuTools.direction) {
		case 'inner':
			var range = document.selection.createRange();
			var sr = range.duplicate();
			sr.moveToElementText(textObj);
			sr.setEndPoint('EndToEnd', range);
			var selectionStart = sr.text.length - range.text.length;
			var selText = document.selection.createRange().text;
			if (selectionStart != -1 && selectionEnd != -1)
			{
				stack("beforeInsert", refObj);
				var before = textObj.innerText.substring(0, selectionStart);
				var after = textObj.innerText.substring(selectionStart + selText.length);
				refObj.insertAdjacentHTML('beforeBegin', CreateText(before, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.insertAdjacentHTML('beforeBegin', CreateVariable(selText, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateText(after, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.previousSibling.previousSibling.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
				toolLeave(refObj.id, "variable");
				stack("afterInsert", refObj.previousSibling.previousSibling);
				removeNoStack(refObj.id);
			}
			break;
		case 'before':
		    refObj.insertAdjacentHTML('beforeBegin', CreateVariable("nom", indentation, false));
		    refObj.insertAdjacentHTML('beforeBegin', CreatePaste(indentation));
		    stack("insert", refObj.previousSibling.previousSibling);
		    insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
		    break;
		case 'after':
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreatePaste(indentation));
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateVariable("nom", indentation, false));
		    stack("insert", refObj.nextSibling.nextSibling);
		    insertZoneSelection(refObj.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
	}
	menuTools.style.filter = 'alpha(opacity=0)';
	menuTools.style.display = 'none';
	lockMenuTools = 0;
}

function displayVariable(name, indent, readonly)
{
	if (!whereToPaste)
	{
		var refObj = document.getElementById(currentContentId);
		refObj.insertAdjacentHTML('beforeEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('beforeEnd', CreateVariable(name, indent, readonly));
		addZoneSelection(refObj.lastChild.id);
	}
	else
	{
		var refObj = document.getElementById(whereToPaste);
		refObj.insertAdjacentHTML('afterEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('afterEnd', CreateVariable(name, indent, readonly));
		if (refObj.previousSibling)
		{
			insertZoneSelection(refObj.previousSibling.id, 'afterEnd', refObj.nextSibling.id);
		}
		else
		{
			insertZoneSelection(refObj.nextSibling.nextSibling.nextSibling.id, 'beforeBegin', refObj.nextSibling.id);
		}
		whereToPaste = refObj.nextSibling.nextSibling.id;
	}
}

function insertAffectation() {
	var menuTools = document.getElementById("menuTools");
	var refObj = document.getElementById(menuTools.clickRef);
	var textObj = document.getElementById(menuTools.textRef);
	var indentation = menuTools.indent;
	switch(menuTools.direction) {
		case 'inner':
			var range = document.selection.createRange();
			var sr = range.duplicate();
			sr.moveToElementText(textObj);
			sr.setEndPoint('EndToEnd', range);
			var selectionStart = sr.text.length - range.text.length;
			var selText = document.selection.createRange().text;
			if (selectionStart != -1 && selectionEnd != -1)
			{
				stack("beforeInsert", refObj);
				var before = textObj.innerText.substring(0, selectionStart);
				var after = textObj.innerText.substring(selectionStart + selText.length);
				refObj.insertAdjacentHTML('beforeBegin', CreateText(before, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.insertAdjacentHTML('beforeBegin', CreateAffectation(selText, "0", indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateText(after, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.previousSibling.previousSibling.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
				toolLeave(refObj.id, "affectation");
				stack("afterInsert", refObj.previousSibling.previousSibling);
				removeNoStack(refObj.id);
			}
			break;

		case 'before':
		    refObj.insertAdjacentHTML('beforeBegin', CreateAffectation("test", "0", indentation, false));
		    refObj.insertAdjacentHTML('beforeBegin', CreatePaste(indentation));
		    stack("insert", refObj.previousSibling.previousSibling);
		    insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
		    break;
		case 'after':
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreatePaste(indentation));
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateAffectation("test", "0", indentation, false));
		    stack("insert", refObj.nextSibling.nextSibling);
		    insertZoneSelection(refObj.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
	}
	menuTools.style.filter = 'alpha(opacity=0)';
	menuTools.style.display = 'none';
	lockMenuTools = 0;
}

function displayAffectation(varName, expression, indent, readonly)
{
	if (!whereToPaste)
	{
		var refObj = document.getElementById(currentContentId);
		refObj.insertAdjacentHTML('beforeEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('beforeEnd', CreateAffectation(varName, expression, indent, readonly));
		addZoneSelection(refObj.lastChild.id);
	}
	else
	{
		var refObj = document.getElementById(whereToPaste);
		refObj.insertAdjacentHTML('afterEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('afterEnd', CreateAffectation(varName, expression, indent, readonly));
		if (refObj.previousSibling)
		{
			insertZoneSelection(refObj.previousSibling.id, 'afterEnd', refObj.nextSibling.id);
		}
		else
		{
			insertZoneSelection(refObj.nextSibling.nextSibling.nextSibling.id, 'beforeBegin', refObj.nextSibling.id);
		}
		whereToPaste = refObj.nextSibling.nextSibling.id;
	}
}

function insertAffectationChaine() {
	var menuTools = document.getElementById("menuTools");
	var refObj = document.getElementById(menuTools.clickRef);
	var textObj = document.getElementById(menuTools.textRef);
	var indentation = menuTools.indent;
	switch(menuTools.direction) {
		case 'inner':
			var range = document.selection.createRange();
			var sr = range.duplicate();
			sr.moveToElementText(textObj);
			sr.setEndPoint('EndToEnd', range);
			var selectionStart = sr.text.length - range.text.length;
			var selText = document.selection.createRange().text;
			if (selectionStart != -1 && selectionEnd != -1)
			{
				stack("beforeInsert", refObj);
				var before = textObj.innerText.substring(0, selectionStart);
				var after = textObj.innerText.substring(selectionStart + selText.length);
				refObj.insertAdjacentHTML('beforeBegin', CreateText(before, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.insertAdjacentHTML('beforeBegin', CreateAffectationChaine(selText, "variable", indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateText(after, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.previousSibling.previousSibling.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
				toolLeave(refObj.id, "affectation");
				stack("afterInsert", refObj.previousSibling.previousSibling);
				removeNoStack(refObj.id);
			}
			break;
		case 'before':
		    refObj.insertAdjacentHTML('beforeBegin', CreateAffectationChaine("test", "variable", indentation, false));
		    refObj.insertAdjacentHTML('beforeBegin', CreatePaste(indentation));
		    stack("insert", refObj.previousSibling.previousSibling);
		    insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
		    break;
		case 'after':
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreatePaste(indentation));
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateAffectationChaine("test", "variable", indentation, false));
		    stack("insert", refObj.nextSibling.nextSibling);
		    insertZoneSelection(refObj.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
	}
	menuTools.style.filter = 'alpha(opacity=0)';
	menuTools.style.display = 'none';
	lockMenuTools = 0;
}

function displayAffectationChaine(varName, stringName, indent, readonly)
{
	if (!whereToPaste)
	{
		var refObj = document.getElementById(currentContentId);
		refObj.insertAdjacentHTML('beforeEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('beforeEnd', CreateAffectationChaine(varName, stringName, indent, readonly));
		addZoneSelection(refObj.lastChild.id);
	}
	else
	{
		var refObj = document.getElementById(whereToPaste);
		refObj.insertAdjacentHTML('afterEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('afterEnd', CreateAffectationChaine(varName, stringName, indent, readonly));
		if (refObj.previousSibling)
		{
			insertZoneSelection(refObj.previousSibling.id, 'afterEnd', refObj.nextSibling.id);
		}
		else
		{
			insertZoneSelection(refObj.nextSibling.nextSibling.nextSibling.id, 'beforeBegin', refObj.nextSibling.id);
		}
		whereToPaste = refObj.nextSibling.nextSibling.id;
	}
}

function insertAffectationChamp() {
	var menuTools = document.getElementById("menuTools");
	var refObj = document.getElementById(menuTools.clickRef);
	var textObj = document.getElementById(menuTools.textRef);
	var indentation = menuTools.indent;
	switch(menuTools.direction) {
		case 'inner':
			var range = document.selection.createRange();
			var sr = range.duplicate();
			sr.moveToElementText(textObj);
			sr.setEndPoint('EndToEnd', range);
			var selectionStart = sr.text.length - range.text.length;
			var selText = document.selection.createRange().text;
			if (selectionStart != -1 && selectionEnd != -1)
			{
				stack("beforeInsert", refObj);
				var before = textObj.innerText.substring(0, selectionStart);
				var after = textObj.innerText.substring(selectionStart + selText.length);
				refObj.insertAdjacentHTML('beforeBegin', CreateText(before, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.insertAdjacentHTML('beforeBegin', CreateAffectationChamp(selText, "tableau", "1", "champ", indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateText(after, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.previousSibling.previousSibling.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
				toolLeave(refObj.id, "affectation");
				stack("afterInsert", refObj.previousSibling.previousSibling);
				removeNoStack(refObj.id);
			}
			break;
		case 'before':
		    refObj.insertAdjacentHTML('beforeBegin', CreateAffectationChamp("test", "tableau", "1", "champ", indentation, false));
		    refObj.insertAdjacentHTML('beforeBegin', CreatePaste(indentation));
		    stack("insert", refObj.previousSibling.previousSibling);
		    insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
		    break;
		case 'after':
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreatePaste(indentation));
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateAffectationChamp("test", "tableau", "1", "champ", indentation, false));
		    stack("insert", refObj.nextSibling.nextSibling);
		    insertZoneSelection(refObj.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
	}
	menuTools.style.filter = 'alpha(opacity=0)';
	menuTools.style.display = 'none';
	lockMenuTools = 0;
}

function displayAffectationChamp(varName, tabName, expression, champ, indent, readonly)
{
	if (!whereToPaste)
	{
		var refObj = document.getElementById(currentContentId);
		refObj.insertAdjacentHTML('beforeEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('beforeEnd', CreateAffectationChamp(varName, tabName, expression, champ, indent, readonly));
		addZoneSelection(refObj.lastChild.id);
	}
	else
	{
		var refObj = document.getElementById(whereToPaste);
		refObj.insertAdjacentHTML('afterEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('afterEnd', CreateAffectationChamp(varName, tabName, expression, champ, indent, readonly));
		if (refObj.previousSibling)
		{
			insertZoneSelection(refObj.previousSibling.id, 'afterEnd', refObj.nextSibling.id);
		}
		else
		{
			insertZoneSelection(refObj.nextSibling.nextSibling.nextSibling.id, 'beforeBegin', refObj.nextSibling.id);
		}
		whereToPaste = refObj.nextSibling.nextSibling.id;
	}
}

function insertCall() {
	var menuTools = document.getElementById("menuTools");
	var refObj = document.getElementById(menuTools.clickRef);
	var textObj = document.getElementById(menuTools.textRef);
	var indentation = menuTools.indent;
	switch(menuTools.direction) {
		case 'inner':
			var range = document.selection.createRange();
			var sr = range.duplicate();
			sr.moveToElementText(textObj);
			sr.setEndPoint('EndToEnd', range);
			var selectionStart = sr.text.length - range.text.length;
			var selText = document.selection.createRange().text;
			if (selectionStart != -1 && selectionEnd != -1)
			{
				stack("beforeInsert", refObj);
				var before = textObj.innerText.substring(0, selectionStart);
				var after = textObj.innerText.substring(selectionStart + selText.length);
				refObj.insertAdjacentHTML('beforeBegin', CreateText(before, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.insertAdjacentHTML('beforeBegin', CreateCall(selText, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateText(after, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.previousSibling.previousSibling.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
				toolLeave(refObj.id, "variable");
				stack("afterInsert", refObj.previousSibling.previousSibling);
				removeNoStack(refObj.id);
			}
			break;
		case 'before':
		    refObj.insertAdjacentHTML('beforeBegin', CreateCall("nom", indentation, false));
		    refObj.insertAdjacentHTML('beforeBegin', CreatePaste(indentation));
		    stack("insert", refObj.previousSibling.previousSibling);
		    insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
		    break;
		case 'after':
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreatePaste(indentation));
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateCall("nom", indentation, false));
		    stack("insert", refObj.nextSibling.nextSibling);
		    insertZoneSelection(refObj.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
	}
	menuTools.style.filter = 'alpha(opacity=0)';
	menuTools.style.display = 'none';
	lockMenuTools = 0;
}

function displayCall(name, indent, readonly)
{
	if (!whereToPaste)
	{
		var refObj = document.getElementById(currentContentId);
		refObj.insertAdjacentHTML('beforeEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('beforeEnd', CreateCall(name, indent, readonly));
		addZoneSelection(refObj.lastChild.id);
	}
	else
	{
		var refObj = document.getElementById(whereToPaste);
		refObj.insertAdjacentHTML('afterEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('afterEnd', CreateCall(name, indent, readonly));
		if (refObj.previousSibling)
		{
			insertZoneSelection(refObj.previousSibling.id, 'afterEnd', refObj.nextSibling.id);
		}
		else
		{
			insertZoneSelection(refObj.nextSibling.nextSibling.nextSibling.id, 'beforeBegin', refObj.nextSibling.id);
		}
		whereToPaste = refObj.nextSibling.nextSibling.id;
	}
}

function insertInjector() {
	var menuTools = document.getElementById("menuTools");
	var refObj = document.getElementById(menuTools.clickRef);
	var textObj = document.getElementById(menuTools.textRef);
	var indentation = menuTools.indent;
	switch(menuTools.direction) {
		case 'inner':
			var range = document.selection.createRange();
			var sr = range.duplicate();
			sr.moveToElementText(textObj);
			sr.setEndPoint('EndToEnd', range);
			var selectionStart = sr.text.length - range.text.length;
			var selText = document.selection.createRange().text;
			if (selectionStart != -1 && selectionEnd != -1)
			{
				stack("beforeInsert", refObj);
				var before = textObj.innerText.substring(0, selectionStart);
				var after = textObj.innerText.substring(selectionStart + selText.length);
				refObj.insertAdjacentHTML('beforeBegin', CreateText(before, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.insertAdjacentHTML('beforeBegin', CreateInjector(selText, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateText(after, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.previousSibling.previousSibling.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
				toolLeave(refObj.id, "variable");
				stack("afterInsert", refObj.previousSibling.previousSibling);
				removeNoStack(refObj.id);
			}
			break;
		case 'before':
		    refObj.insertAdjacentHTML('beforeBegin', CreateInjector("nom", indentation, false));
		    refObj.insertAdjacentHTML('beforeBegin', CreatePaste(indentation));
		    stack("insert", refObj.previousSibling.previousSibling);
		    insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
		    break;
		case 'after':
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreatePaste(indentation));
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateInjector("nom", indentation, false));
		    stack("insert", refObj.nextSibling.nextSibling);
		    insertZoneSelection(refObj.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
	}
	menuTools.style.filter = 'alpha(opacity=0)';
	menuTools.style.display = 'none';
	lockMenuTools = 0;
}

function displayInjector(name, indent, readonly)
{
	if (!whereToPaste)
	{
		var refObj = document.getElementById(currentContentId);
		refObj.insertAdjacentHTML('beforeEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('beforeEnd', CreateInjector(name, indent, readonly));
		addZoneSelection(refObj.lastChild.id);
	}
	else
	{
		var refObj = document.getElementById(whereToPaste);
		refObj.insertAdjacentHTML('afterEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('afterEnd', CreateInjector(name, indent, readonly));
		if (refObj.previousSibling)
		{
			insertZoneSelection(refObj.previousSibling.id, 'afterEnd', refObj.nextSibling.id);
		}
		else
		{
			insertZoneSelection(refObj.nextSibling.nextSibling.nextSibling.id, 'beforeBegin', refObj.nextSibling.id);
		}
		whereToPaste = refObj.nextSibling.nextSibling.id;
	}
}

function insertCallSkeleton() {
	var menuTools = document.getElementById("menuTools");
	var refObj = document.getElementById(menuTools.clickRef);
	var textObj = document.getElementById(menuTools.textRef);
	var indentation = menuTools.indent;
	switch(menuTools.direction) {
		case 'inner':
			var range = document.selection.createRange();
			var sr = range.duplicate();
			sr.moveToElementText(textObj);
			sr.setEndPoint('EndToEnd', range);
			var selectionStart = sr.text.length - range.text.length;
			var selText = document.selection.createRange().text;
			if (selectionStart != -1 && selectionEnd != -1)
			{
				stack("beforeInsert", refObj);
				var before = textObj.innerText.substring(0, selectionStart);
				var after = textObj.innerText.substring(selectionStart + selText.length);
				refObj.insertAdjacentHTML('beforeBegin', CreateText(before, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.insertAdjacentHTML('beforeBegin', CreateCallSkeleton(selText, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateText(after, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.previousSibling.previousSibling.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
				toolLeave(refObj.id, "variable");
				stack("afterInsert", refObj.previousSibling.previousSibling);
				removeNoStack(refObj.id);
			}
			break;
		case 'before':
		    refObj.insertAdjacentHTML('beforeBegin', CreateCallSkeleton("/chemin/path", indentation, false));
		    refObj.insertAdjacentHTML('beforeBegin', CreatePaste(indentation));
		    stack("insert", refObj.previousSibling.previousSibling);
		    insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
		    break;
		case 'after':
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreatePaste(indentation));
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateCallSkeleton("/chemin/path", indentation, false));
		    stack("insert", refObj.nextSibling.nextSibling);
		    insertZoneSelection(refObj.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
	}
	menuTools.style.filter = 'alpha(opacity=0)';
	menuTools.style.display = 'none';
	lockMenuTools = 0;
}

function displayCallSkeleton(name, indent, readonly)
{
	if (!whereToPaste)
	{
		var refObj = document.getElementById(currentContentId);
		refObj.insertAdjacentHTML('beforeEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('beforeEnd', CreateCallSkeleton(name, indent, readonly));
		addZoneSelection(refObj.lastChild.id);
	}
	else
	{
		var refObj = document.getElementById(whereToPaste);
		refObj.insertAdjacentHTML('afterEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('afterEnd', CreateCallSkeleton(name, indent, readonly));
		if (refObj.previousSibling)
		{
			insertZoneSelection(refObj.previousSibling.id, 'afterEnd', refObj.nextSibling.id);
		}
		else
		{
			insertZoneSelection(refObj.nextSibling.nextSibling.nextSibling.id, 'beforeBegin', refObj.nextSibling.id);
		}
		whereToPaste = refObj.nextSibling.nextSibling.id;
	}
}

function insertChamp() {
	var menuTools = document.getElementById("menuTools");
	var refObj = document.getElementById(menuTools.clickRef);
	var textObj = document.getElementById(menuTools.textRef);
	var indentation = menuTools.indent;
	switch(menuTools.direction) {
		case 'inner':
			var range = document.selection.createRange();
			var sr = range.duplicate();
			sr.moveToElementText(textObj);
			sr.setEndPoint('EndToEnd', range);
			var selectionStart = sr.text.length - range.text.length;
			var selText = document.selection.createRange().text;
			if (selectionStart != -1 && selectionEnd != -1)
			{
				stack("beforeInsert", refObj);
				var before = textObj.innerText.substring(0, selectionStart);
				var after = textObj.innerText.substring(selectionStart + selText.length);
				refObj.insertAdjacentHTML('beforeBegin', CreateText(before, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.insertAdjacentHTML('beforeBegin', CreateChamp(selText, "1", "champ", indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateText(after, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.previousSibling.previousSibling.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
				toolLeave(refObj.id, "champ");
				stack("afterInsert", refObj.previousSibling.previousSibling);
				removeNoStack(refObj.id);
			}
			break;
		case 'before':
		    refObj.insertAdjacentHTML('beforeBegin', CreateChamp("tableau", "1", "champ", indentation, false));
		    refObj.insertAdjacentHTML('beforeBegin', CreatePaste(indentation));
		    stack("insert", refObj.previousSibling.previousSibling);
		    insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
		    break;
		case 'after':
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreatePaste(indentation));
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateChamp("tableau", "1", "champ", indentation, false));
		    stack("insert", refObj.nextSibling.nextSibling);
		    insertZoneSelection(refObj.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
	}
	menuTools.style.filter = 'alpha(opacity=0)';
	menuTools.style.display = 'none';
	lockMenuTools = 0;
}

function displayChamp(tabName, expression, champ, indent, readonly)
{
	if (!whereToPaste)
	{
		var refObj = document.getElementById(currentContentId);
		refObj.insertAdjacentHTML('beforeEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('beforeEnd', CreateChamp(tabName, expression, champ, indent, readonly));
		addZoneSelection(refObj.lastChild.id);
	}
	else
	{
		var refObj = document.getElementById(whereToPaste);
		refObj.insertAdjacentHTML('afterEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('afterEnd', CreateChamp(tabName, expression, champ, indent, readonly));
		if (refObj.previousSibling)
		{
			insertZoneSelection(refObj.previousSibling.id, 'afterEnd', refObj.nextSibling.id);
		}
		else
		{
			insertZoneSelection(refObj.nextSibling.nextSibling.nextSibling.id, 'beforeBegin', refObj.nextSibling.id);
		}
		whereToPaste = refObj.nextSibling.nextSibling.id;
	}
}

function firedCoding(toolId, direction)
{
	var menuTools = document.getElementById("menuTools");
	menuTools.direction = direction;
	menuTools.clickRef = toolId;
	var tool = document.getElementById(toolId);
	// hacking +1 sur indentation car on indente automatiquement
	// un coding que l'on crée
	menuTools.indent = parseInt(tool.indent) + 1;
	insertCoding();
}

function insertCoding() {
	var menuTools = document.getElementById("menuTools");
	var refObj = document.getElementById(menuTools.clickRef);
	var textObj = document.getElementById(menuTools.textRef);
	// si l'objet cliqué est un coding alors on décremente
	// sinon un autre objet que coding s'incrémente
	if (refObj.type == "coding")
	{
		var indentation = menuTools.indent - 1;
	}
	else
	{
		var indentation = menuTools.indent + 1;
	}
	switch(menuTools.direction) {
		case 'inner':
			var range = document.selection.createRange();
			var sr = range.duplicate();
			sr.moveToElementText(textObj);
			sr.setEndPoint('EndToEnd', range);
			var selectionStart = sr.text.length - range.text.length;
			var selText = document.selection.createRange().text;
			if (selectionStart != -1 && selectionEnd != -1)
			{
				stack("beforeInsert", refObj);
				var before = textObj.innerText.substring(0, selectionStart);
				var after = textObj.innerText.substring(selectionStart + selText.length);
				refObj.insertAdjacentHTML('beforeBegin', CreateText(before, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.insertAdjacentHTML('beforeBegin', CreateCoding(selText, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateText(after, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.previousSibling.previousSibling.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
				toolLeave(refObj.id, "variable");
				stack("afterInsert", refObj.previousSibling.previousSibling);
				removeNoStack(refObj.id);
			}
			break;
		case 'before':
		    refObj.insertAdjacentHTML('beforeBegin', CreateCoding("nom", indentation, false));
		    refObj.insertAdjacentHTML('beforeBegin', CreatePaste(indentation));
		    stack("insert", refObj.previousSibling.previousSibling);
		    insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
		    break;
		case 'after':
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreatePaste(indentation));
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateCoding("nom", indentation, false));
		    stack("insert", refObj.nextSibling.nextSibling);
		    insertZoneSelection(refObj.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
	}
	menuTools.style.filter = 'alpha(opacity=0)';
	menuTools.style.display = 'none';
	lockMenuTools = 0;
}

function displayCoding(name, indent, readonly)
{
	if (!whereToPaste)
	{
		var refObj = document.getElementById(currentContentId);
		refObj.insertAdjacentHTML('beforeEnd', CreatePaste(parseInt(indent) + 1));
		refObj.insertAdjacentHTML('beforeEnd', CreateCoding(name, indent, readonly));
		addZoneSelection(refObj.lastChild.id);
	}
	else
	{
		var refObj = document.getElementById(whereToPaste);
			refObj.insertAdjacentHTML('afterEnd', CreatePaste(parseInt(indent) + 1));
		refObj.insertAdjacentHTML('afterEnd', CreateCoding(name, indent, readonly));
		if (refObj.previousSibling)
		{
			insertZoneSelection(refObj.previousSibling.id, 'afterEnd', refObj.nextSibling.id);
		}
		else
		{
			insertZoneSelection(refObj.nextSibling.nextSibling.nextSibling.id, 'beforeBegin', refObj.nextSibling.id);
		}
		whereToPaste = refObj.nextSibling.nextSibling.id;
	}
}

function insertCondition() {
	var menuTools = document.getElementById("menuTools");
	var refObj = document.getElementById(menuTools.clickRef);
	var textObj = document.getElementById(menuTools.textRef);
	var indentation = menuTools.indent;
	switch(menuTools.direction) {
		case 'inner':
			var range = document.selection.createRange();
			var sr = range.duplicate();
			sr.moveToElementText(textObj);
			sr.setEndPoint('EndToEnd', range);
			var selectionStart = sr.text.length - range.text.length;
			var selText = document.selection.createRange().text;
			if (selectionStart != -1 && selectionEnd != -1)
			{
				stack("beforeInsert", refObj);
				var before = textObj.innerText.substring(0, selectionStart);
				var after = textObj.innerText.substring(selectionStart + selText.length);
				refObj.insertAdjacentHTML('beforeBegin', CreateText(before, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.insertAdjacentHTML('beforeBegin', CreateCondition(selText, "start", "end", indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateText(after, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.previousSibling.previousSibling.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
				toolLeave(refObj.id, "condition");
				stack("afterInsert", refObj.previousSibling.previousSibling);
				removeNoStack(refObj.id);
			}
			break;
		case 'before':
		    refObj.insertAdjacentHTML('beforeBegin', CreateCondition("true", "start", "end", indentation, false));
		    refObj.insertAdjacentHTML('beforeBegin', CreatePaste(indentation));
		    stack("insert", refObj.previousSibling.previousSibling);
		    insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
		    break;
		case 'after':
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreatePaste(indentation));
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateCondition("true", "start", "end", indentation, false));
		    stack("insert", refObj.nextSibling.nextSibling);
		    insertZoneSelection(refObj.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
	}
	menuTools.style.filter = 'alpha(opacity=0)';
	menuTools.style.display = 'none';
	lockMenuTools = 0;
}

function displayCondition(expression, iftrue, iffalse, indent, readonly)
{
	if (!whereToPaste)
	{
		var refObj = document.getElementById(currentContentId);
		refObj.insertAdjacentHTML('beforeEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('beforeEnd', CreateCondition(expression, iftrue, iffalse, indent, readonly));
		addZoneSelection(refObj.lastChild.id);
	}
	else
	{
		var refObj = document.getElementById(whereToPaste);
		refObj.insertAdjacentHTML('afterEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('afterEnd', CreateCondition(expression, iftrue, iffalse, indent, readonly));
		if (refObj.previousSibling)
		{
			insertZoneSelection(refObj.previousSibling.id, 'afterEnd', refObj.nextSibling.id);
		}
		else
		{
			insertZoneSelection(refObj.nextSibling.nextSibling.nextSibling.id, 'beforeBegin', refObj.nextSibling.id);
		}
		whereToPaste = refObj.nextSibling.nextSibling.id;
	}
}

function insertEndProcess() {
	var menuTools = document.getElementById("menuTools");
	var refObj = document.getElementById(menuTools.clickRef);
	var textObj = document.getElementById(menuTools.textRef);
	var indentation = menuTools.indent;
	switch(menuTools.direction) {
		case 'inner':
			var range = document.selection.createRange();
			var sr = range.duplicate();
			sr.moveToElementText(textObj);
			sr.setEndPoint('EndToEnd', range);
			var selectionStart = sr.text.length - range.text.length;
			var selText = document.selection.createRange().text;
			if (selectionStart != -1 && selectionEnd != -1)
			{
				stack("beforeInsert", refObj);
				var before = textObj.innerText.substring(0, selectionStart);
				var after = textObj.innerText.substring(selectionStart + selText.length);
				refObj.insertAdjacentHTML('beforeBegin', CreateText(before, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.insertAdjacentHTML('beforeBegin', CreateEndProcess(selText, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateText(after, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.previousSibling.previousSibling.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
				toolLeave(refObj.id, "variable");
				stack("afterInsert", refObj.previousSibling.previousSibling);
				removeNoStack(refObj.id);
			}
			break;
		case 'before':
		    refObj.insertAdjacentHTML('beforeBegin', CreateEndProcess("nom", indentation, false));
		    refObj.insertAdjacentHTML('beforeBegin', CreatePaste(indentation));
		    stack("insert", refObj.previousSibling.previousSibling);
		    insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
		    break;
		case 'after':
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreatePaste(indentation));
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateEndProcess("nom", indentation, false));
		    stack("insert", refObj.nextSibling.nextSibling);
		    insertZoneSelection(refObj.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
	}
	menuTools.style.filter = 'alpha(opacity=0)';
	menuTools.style.display = 'none';
	lockMenuTools = 0;
}

function displayEndProcess(name, indent, readonly)
{
	if (!whereToPaste)
	{
		var refObj = document.getElementById(currentContentId);
		refObj.insertAdjacentHTML('beforeEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('beforeEnd', CreateEndProcess(name, indent, readonly));
		addZoneSelection(refObj.lastChild.id);
	}
	else
	{
		var refObj = document.getElementById(whereToPaste);
		refObj.insertAdjacentHTML('afterEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('afterEnd', CreateEndProcess(name, indent, readonly));
		if (refObj.previousSibling)
		{
			insertZoneSelection(refObj.previousSibling.id, 'afterEnd', refObj.nextSibling.id);
		}
		else
		{
			insertZoneSelection(refObj.nextSibling.nextSibling.nextSibling.id, 'beforeBegin', refObj.nextSibling.id);
		}
		whereToPaste = refObj.nextSibling.nextSibling.id;
	}
}

function insertEndSkeleton() {
	var menuTools = document.getElementById("menuTools");
	var refObj = document.getElementById(menuTools.clickRef);
	var textObj = document.getElementById(menuTools.textRef);
	var indentation = menuTools.indent;
	switch(menuTools.direction) {
		case 'inner':
			var range = document.selection.createRange();
			var sr = range.duplicate();
			sr.moveToElementText(textObj);
			sr.setEndPoint('EndToEnd', range);
			var selectionStart = sr.text.length - range.text.length;
			var selText = document.selection.createRange().text;
			if (selectionStart != -1 && selectionEnd != -1)
			{
				stack("beforeInsert", refObj);
				var before = textObj.innerText.substring(0, selectionStart);
				var after = textObj.innerText.substring(selectionStart + selText.length);
				refObj.insertAdjacentHTML('beforeBegin', CreateText(before, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.insertAdjacentHTML('beforeBegin', CreateEndSkeleton(selText, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateText(after, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.previousSibling.previousSibling.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
				toolLeave(refObj.id, "variable");
				stack("afterInsert", refObj.previousSibling.previousSibling);
				removeNoStack(refObj.id);
			}
			break;
		case 'before':
		    refObj.insertAdjacentHTML('beforeBegin', CreateEndSkeleton("/chemin/nom", indentation, false));
		    refObj.insertAdjacentHTML('beforeBegin', CreatePaste(indentation));
		    stack("insert", refObj.previousSibling.previousSibling);
		    insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
		    break;
		case 'after':
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreatePaste(indentation));
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateEndSkeleton("/chemin/nom", indentation, false));
		    stack("insert", refObj.nextSibling.nextSibling);
		    insertZoneSelection(refObj.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
	}
	menuTools.style.filter = 'alpha(opacity=0)';
	menuTools.style.display = 'none';
	lockMenuTools = 0;
}

function displayEndSkeleton(name, indent, readonly)
{
	if (!whereToPaste)
	{
		var refObj = document.getElementById(currentContentId);
		refObj.insertAdjacentHTML('beforeEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('beforeEnd', CreateEndSkeleton(name, indent, readonly));
		addZoneSelection(refObj.lastChild.id);
	}
	else
	{
		var refObj = document.getElementById(whereToPaste);
		refObj.insertAdjacentHTML('afterEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('afterEnd', CreateEndSkeleton(name, indent, readonly));
		if (refObj.previousSibling)
		{
			insertZoneSelection(refObj.previousSibling.id, 'afterEnd', refObj.nextSibling.id);
		}
		else
		{
			insertZoneSelection(refObj.nextSibling.nextSibling.nextSibling.id, 'beforeBegin', refObj.nextSibling.id);
		}
		whereToPaste = refObj.nextSibling.nextSibling.id;
	}
}

function insertBeginProcess() {
	var menuTools = document.getElementById("menuTools");
	var refObj = document.getElementById(menuTools.clickRef);
	var textObj = document.getElementById(menuTools.textRef);
	var indentation = menuTools.indent;
	switch(menuTools.direction) {
		case 'inner':
			var range = document.selection.createRange();
			var sr = range.duplicate();
			sr.moveToElementText(textObj);
			sr.setEndPoint('EndToEnd', range);
			var selectionStart = sr.text.length - range.text.length;
			var selText = document.selection.createRange().text;
			if (selectionStart != -1 && selectionEnd != -1)
			{
				stack("beforeInsert", refObj);
				var before = textObj.innerText.substring(0, selectionStart);
				var after = textObj.innerText.substring(selectionStart + selText.length);
				refObj.insertAdjacentHTML('beforeBegin', CreateText(before, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.insertAdjacentHTML('beforeBegin', CreateBeginProcess(selText, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateText(after, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.previousSibling.previousSibling.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
				toolLeave(refObj.id, "variable");
				stack("afterInsert", refObj.previousSibling.previousSibling);
				removeNoStack(refObj.id);
			}
			break;
		case 'before':
		    refObj.insertAdjacentHTML('beforeBegin', CreateBeginProcess("nom", indentation, false));
		    refObj.insertAdjacentHTML('beforeBegin', CreatePaste(indentation));
		    stack("insert", refObj.previousSibling.previousSibling);
		    insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
		    break;
		case 'after':
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreatePaste(indentation));
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateBeginProcess("nom", indentation, false));
		    stack("insert", refObj.nextSibling.nextSibling);
		    insertZoneSelection(refObj.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
	}
	menuTools.style.filter = 'alpha(opacity=0)';
	menuTools.style.display = 'none';
	lockMenuTools = 0;
}

function displayBeginProcess(name, indent, readonly)
{
	if (!whereToPaste)
	{
		var refObj = document.getElementById(currentContentId);
		refObj.insertAdjacentHTML('beforeEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('beforeEnd', CreateBeginProcess(name, indent, readonly));
		addZoneSelection(refObj.lastChild.id);
	}
	else
	{
		var refObj = document.getElementById(whereToPaste);
		refObj.insertAdjacentHTML('afterEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('afterEnd', CreateBeginProcess(name, indent, readonly));
		if (refObj.previousSibling)
		{
			insertZoneSelection(refObj.previousSibling.id, 'afterEnd', refObj.nextSibling.id);
		}
		else
		{
			insertZoneSelection(refObj.nextSibling.nextSibling.nextSibling.id, 'beforeBegin', refObj.nextSibling.id);
		}
		whereToPaste = refObj.nextSibling.nextSibling.id;
	}
}

function insertBeginSkeleton() {
	var menuTools = document.getElementById("menuTools");
	var refObj = document.getElementById(menuTools.clickRef);
	var textObj = document.getElementById(menuTools.textRef);
	var indentation = menuTools.indent;
	switch(menuTools.direction) {
		case 'inner':
			var range = document.selection.createRange();
			var sr = range.duplicate();
			sr.moveToElementText(textObj);
			sr.setEndPoint('EndToEnd', range);
			var selectionStart = sr.text.length - range.text.length;
			var selText = document.selection.createRange().text;
			if (selectionStart != -1 && selectionEnd != -1)
			{
				stack("beforeInsert", refObj);
				var before = textObj.innerText.substring(0, selectionStart);
				var after = textObj.innerText.substring(selectionStart + selText.length);
				refObj.insertAdjacentHTML('beforeBegin', CreateText(before, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.insertAdjacentHTML('beforeBegin', CreateBeginSkeleton(selText, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateText(after, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.previousSibling.previousSibling.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
				toolLeave(refObj.id, "variable");
				stack("afterInsert", refObj.previousSibling.previousSibling);
				removeNoStack(refObj.id);
			}
			break;
		case 'before':
		    refObj.insertAdjacentHTML('beforeBegin', CreateBeginSkeleton("/chemin/nom", indentation, false));
		    refObj.insertAdjacentHTML('beforeBegin', CreatePaste(indentation));
		    stack("insert", refObj.previousSibling.previousSibling);
		    insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
		    break;
		case 'after':
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreatePaste(indentation));
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateBeginSkeleton("/chemin/nom", indentation, false));
		    stack("insert", refObj.nextSibling.nextSibling);
		    insertZoneSelection(refObj.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
	}
	menuTools.style.filter = 'alpha(opacity=0)';
	menuTools.style.display = 'none';
	lockMenuTools = 0;
}

function displayBeginSkeleton(name, indent, readonly)
{
	if (!whereToPaste)
	{
		var refObj = document.getElementById(currentContentId);
		refObj.insertAdjacentHTML('beforeEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('beforeEnd', CreateBeginSkeleton(name, indent, readonly));
		addZoneSelection(refObj.lastChild.id);
	}
	else
	{
		var refObj = document.getElementById(whereToPaste);
		refObj.insertAdjacentHTML('afterEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('afterEnd', CreateBeginSkeleton(name, indent, readonly));
		if (refObj.previousSibling)
		{
			insertZoneSelection(refObj.previousSibling.id, 'afterEnd', refObj.nextSibling.id);
		}
		else
		{
			insertZoneSelection(refObj.nextSibling.nextSibling.nextSibling.id, 'beforeBegin', refObj.nextSibling.id);
		}
		whereToPaste = refObj.nextSibling.nextSibling.id;
	}
}

function insertHandler() {
	var menuTools = document.getElementById("menuTools");
	var refObj = document.getElementById(menuTools.clickRef);
	var textObj = document.getElementById(menuTools.textRef);
	var indentation = menuTools.indent;
	switch(menuTools.direction) {
		case 'inner':
			var range = document.selection.createRange();
			var sr = range.duplicate();
			sr.moveToElementText(textObj);
			sr.setEndPoint('EndToEnd', range);
			var selectionStart = sr.text.length - range.text.length;
			var selText = document.selection.createRange().text;
			if (selectionStart != -1 && selectionEnd != -1)
			{
				stack("beforeInsert", refObj);
				var before = textObj.innerText.substring(0, selectionStart);
				var after = textObj.innerText.substring(selectionStart + selText.length);
				refObj.insertAdjacentHTML('beforeBegin', CreateText(before, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.insertAdjacentHTML('beforeBegin', CreateHandler(selText, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateText(after, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.previousSibling.previousSibling.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
				toolLeave(refObj.id, "variable");
				stack("afterInsert", refObj.previousSibling.previousSibling);
				removeNoStack(refObj.id);
			}
			break;
		case 'before':
		    refObj.insertAdjacentHTML('beforeBegin', CreateHandler("nom", indentation, false));
		    refObj.insertAdjacentHTML('beforeBegin', CreatePaste(indentation));
		    stack("insert", refObj.previousSibling.previousSibling);
		    insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
		    break;
		case 'after':
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreatePaste(indentation));
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateHandler("nom", indentation, false));
		    stack("insert", refObj.nextSibling.nextSibling);
		    insertZoneSelection(refObj.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
	}
	menuTools.style.filter = 'alpha(opacity=0)';
	menuTools.style.display = 'none';
	lockMenuTools = 0;
}

function displayHandler(name, indent, readonly)
{
	if (!whereToPaste)
	{
		var refObj = document.getElementById(currentContentId);
		refObj.insertAdjacentHTML('beforeEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('beforeEnd', CreateHandler(name, indent, readonly));
		addZoneSelection(refObj.lastChild.id);
	}
	else
	{
		var refObj = document.getElementById(whereToPaste);
		refObj.insertAdjacentHTML('afterEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('afterEnd', CreateHandler(name, indent, readonly));
		if (refObj.previousSibling)
		{
			insertZoneSelection(refObj.previousSibling.id, 'afterEnd', refObj.nextSibling.id);
		}
		else
		{
			insertZoneSelection(refObj.nextSibling.nextSibling.nextSibling.id, 'beforeBegin', refObj.nextSibling.id);
		}
		whereToPaste = refObj.nextSibling.nextSibling.id;
	}
}

function insertLabel() {
	var menuTools = document.getElementById("menuTools");
	var refObj = document.getElementById(menuTools.clickRef);
	var textObj = document.getElementById(menuTools.textRef);
	var indentation = menuTools.indent;
	switch(menuTools.direction) {
		case 'inner':
			var range = document.selection.createRange();
			var sr = range.duplicate();
			sr.moveToElementText(textObj);
			sr.setEndPoint('EndToEnd', range);
			var selectionStart = sr.text.length - range.text.length;
			var selText = document.selection.createRange().text;
			if (selectionStart != -1 && selectionEnd != -1)
			{
				stack("beforeInsert", refObj);
				var before = textObj.innerText.substring(0, selectionStart);
				var after = textObj.innerText.substring(selectionStart + selText.length);
				refObj.insertAdjacentHTML('beforeBegin', CreateText(before, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.insertAdjacentHTML('beforeBegin', CreateLabel(selText, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateText(after, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.previousSibling.previousSibling.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
				toolLeave(refObj.id, "variable");
				stack("afterInsert", refObj.previousSibling.previousSibling);
				removeNoStack(refObj.id);
			}
			break;
		case 'before':
		    refObj.insertAdjacentHTML('beforeBegin', CreateLabel("label", indentation, false));
		    refObj.insertAdjacentHTML('beforeBegin', CreatePaste(indentation));
		    stack("insert", refObj.previousSibling.previousSibling);
		    insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
		    break;
		case 'after':
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreatePaste(indentation));
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateLabel("label", indentation, false));
		    stack("insert", refObj.nextSibling.nextSibling);
		    insertZoneSelection(refObj.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
	}
	menuTools.style.filter = 'alpha(opacity=0)';
	menuTools.style.display = 'none';
	lockMenuTools = 0;
}

function displayLabel(name, indent, readonly)
{
	if (!whereToPaste)
	{
		var refObj = document.getElementById(currentContentId);
		refObj.insertAdjacentHTML('beforeEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('beforeEnd', CreateLabel(name, indent, readonly));
		addZoneSelection(refObj.lastChild.id);
	}
	else
	{
		var refObj = document.getElementById(whereToPaste);
		refObj.insertAdjacentHTML('afterEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('afterEnd', CreateLabel(name, indent, readonly));
		if (refObj.previousSibling)
		{
			insertZoneSelection(refObj.previousSibling.id, 'afterEnd', refObj.nextSibling.id);
		}
		else
		{
			insertZoneSelection(refObj.nextSibling.nextSibling.nextSibling.id, 'beforeBegin', refObj.nextSibling.id);
		}
		whereToPaste = refObj.nextSibling.nextSibling.id;
	}
}

function insertSize() {
	var menuTools = document.getElementById("menuTools");
	var refObj = document.getElementById(menuTools.clickRef);
	var textObj = document.getElementById(menuTools.textRef);
	var indentation = menuTools.indent;
	switch(menuTools.direction) {
		case 'inner':
			var range = document.selection.createRange();
			var sr = range.duplicate();
			sr.moveToElementText(textObj);
			sr.setEndPoint('EndToEnd', range);
			var selectionStart = sr.text.length - range.text.length;
			var selText = document.selection.createRange().text;
			if (selectionStart != -1 && selectionEnd != -1)
			{
				stack("beforeInsert", refObj);
				var before = textObj.innerText.substring(0, selectionStart);
				var after = textObj.innerText.substring(selectionStart + selText.length);
				refObj.insertAdjacentHTML('beforeBegin', CreateText(before, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.insertAdjacentHTML('beforeBegin', CreateSize(selText, "tableau", indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateText(after, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.previousSibling.previousSibling.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
				toolLeave(refObj.id, "affectation");
				stack("afterInsert", refObj.previousSibling.previousSibling);
				removeNoStack(refObj.id);
			}
			break;
		case 'before':
		    refObj.insertAdjacentHTML('beforeBegin', CreateSize("test", "tableau", indentation, false));
		    refObj.insertAdjacentHTML('beforeBegin', CreatePaste(indentation));
		    stack("insert", refObj.previousSibling.previousSibling);
		    insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
		    break;
		case 'after':
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreatePaste(indentation));
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateSize("test", "tableau", indentation, false));
		    stack("insert", refObj.nextSibling.nextSibling);
		    insertZoneSelection(refObj.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
	}
	menuTools.style.filter = 'alpha(opacity=0)';
	menuTools.style.display = 'none';
	lockMenuTools = 0;
}

function displaySize(varName, tabName, indent, readonly)
{
	if (!whereToPaste)
	{
		var refObj = document.getElementById(currentContentId);
		refObj.insertAdjacentHTML('beforeEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('beforeEnd', CreateSize(varName, tabName, indent, readonly));
		addZoneSelection(refObj.lastChild.id);
	}
	else
	{
		var refObj = document.getElementById(whereToPaste);
		refObj.insertAdjacentHTML('afterEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('afterEnd', CreateSize(varName, tabName, indent, readonly));
		if (refObj.previousSibling)
		{
			insertZoneSelection(refObj.previousSibling.id, 'afterEnd', refObj.nextSibling.id);
		}
		else
		{
			insertZoneSelection(refObj.nextSibling.nextSibling.nextSibling.id, 'beforeBegin', refObj.nextSibling.id);
		}
		whereToPaste = refObj.nextSibling.nextSibling.id;
	}
}

function insertTemplate() {
	var menuTools = document.getElementById("menuTools");
	var refObj = document.getElementById(menuTools.clickRef);
	var textObj = document.getElementById(menuTools.textRef);
	var indentation = menuTools.indent;
	switch(menuTools.direction) {
		case 'inner':
			var range = document.selection.createRange();
			var sr = range.duplicate();
			sr.moveToElementText(textObj);
			sr.setEndPoint('EndToEnd', range);
			var selectionStart = sr.text.length - range.text.length;
			var selText = document.selection.createRange().text;
			if (selectionStart != -1 && selectionEnd != -1)
			{
				stack("beforeInsert", refObj);
				var before = textObj.innerText.substring(0, selectionStart);
				var after = textObj.innerText.substring(selectionStart + selText.length);
				refObj.insertAdjacentHTML('beforeBegin', CreateText(before, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.insertAdjacentHTML('beforeBegin', CreateTemplate(selText, "$param1,$param2,...", "<beginprocess>nom</beginprocess><endprocess>nom</endprocess><call>nom</call>", new ActiveXObject("o2Mate.LegendeDict"), indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateText(after, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.previousSibling.previousSibling.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
				toolLeave(refObj.id, "template");
				stack("afterInsert", refObj.previousSibling.previousSibling);
				removeNoStack(refObj.id);
			}
			break;
		case 'before':
		    refObj.insertAdjacentHTML('beforeBegin', CreateTemplate("/Programs/Templates/myTemplate", "$param1,$param2,...", "<beginprocess>nom</beginprocess><endprocess>nom</endprocess><call>nom</call>", new ActiveXObject("o2Mate.LegendeDict"), indentation, false));
		    refObj.insertAdjacentHTML('beforeBegin', CreatePaste(indentation));
		    stack("insert", refObj.previousSibling.previousSibling);
		    insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
		    break;
		case 'after':
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreatePaste(indentation));
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateTemplate("/Programs/Templates/myTemplate", "$param1,$param2,...", "<beginprocess>nom</beginprocess><endprocess>nom</endprocess><call>nom</call>", new ActiveXObject("o2Mate.LegendeDict"), indentation, false));
		    stack("insert", refObj.nextSibling.nextSibling);
		    insertZoneSelection(refObj.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
	}
	menuTools.style.filter = 'alpha(opacity=0)';
	menuTools.style.display = 'none';
	lockMenuTools = 0;
}

function displayTemplate(path, params, xmlCode, myLegendes, indent, readonly)
{
	if (!whereToPaste)
	{
		var refObj = document.getElementById(currentContentId);
		refObj.insertAdjacentHTML('beforeEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('beforeEnd', CreateTemplate(path, params, xmlCode, myLegendes, indent, readonly));
		addZoneSelection(refObj.lastChild.id);
	}
	else
	{
		var refObj = document.getElementById(whereToPaste);
		refObj.insertAdjacentHTML('afterEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('afterEnd', CreateTemplate(path, params, xmlCode, myLegendes, indent, readonly));
		if (refObj.previousSibling)
		{
			insertZoneSelection(refObj.previousSibling.id, 'afterEnd', refObj.nextSibling.id);
		}
		else
		{
			insertZoneSelection(refObj.nextSibling.nextSibling.nextSibling.id, 'beforeBegin', refObj.nextSibling.id);
		}
		whereToPaste = refObj.nextSibling.nextSibling.id;
	}
}

function insertCreateMOP() {
	var menuTools = document.getElementById("menuTools");
	var refObj = document.getElementById(menuTools.clickRef);
	var textObj = document.getElementById(menuTools.textRef);
	var indentation = menuTools.indent;
	switch(menuTools.direction) {
		case 'inner':
			var range = document.selection.createRange();
			var sr = range.duplicate();
			sr.moveToElementText(textObj);
			sr.setEndPoint('EndToEnd', range);
			var selectionStart = sr.text.length - range.text.length;
			var selText = document.selection.createRange().text;
			if (selectionStart != -1 && selectionEnd != -1)
			{
				stack("beforeInsert", refObj);
				var before = textObj.innerText.substring(0, selectionStart);
				var after = textObj.innerText.substring(selectionStart + selText.length);
				refObj.insertAdjacentHTML('beforeBegin', CreateText(before, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.insertAdjacentHTML('beforeBegin', CreateMOP("C", selText, "$param1,$param2,...", "<texte>Write anything</texte>", new ActiveXObject("o2Mate.LegendeDict"), indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateText(after, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.previousSibling.previousSibling.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
				toolLeave(refObj.id, "template");
				stack("afterInsert", refObj.previousSibling.previousSibling);
				removeNoStack(refObj.id);
			}
			break;
		case 'before':
		    refObj.insertAdjacentHTML('beforeBegin', CreateMOP("C", "mop", "$param1,$param2,...", "<texte>Write anything</texte>", new ActiveXObject("o2Mate.LegendeDict"), indentation, false));
		    refObj.insertAdjacentHTML('beforeBegin', CreatePaste(indentation));
		    stack("insert", refObj.previousSibling.previousSibling);
		    insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
		    break;
		case 'after':
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreatePaste(indentation));
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateMOP("C", "mop", "$param1,$param2,...", "<texte>Write anything</texte>", new ActiveXObject("o2Mate.LegendeDict"), indentation, false));
		    stack("insert", refObj.nextSibling.nextSibling);
		    insertZoneSelection(refObj.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
	}
	menuTools.style.filter = 'alpha(opacity=0)';
	menuTools.style.display = 'none';
	lockMenuTools = 0;
}

function displayCreateMOP(language, name, params, xmlCode, myLegendes, indent, readonly)
{
	if (!whereToPaste)
	{
		var refObj = document.getElementById(currentContentId);
		refObj.insertAdjacentHTML('beforeEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('beforeEnd', CreateMOP(language, name, params, xmlCode, myLegendes, indent, readonly));
		addZoneSelection(refObj.lastChild.id);
	}
	else
	{
		var refObj = document.getElementById(whereToPaste);
		refObj.insertAdjacentHTML('afterEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('afterEnd', CreateMOP(language, name, params, xmlCode, myLegendes, indent, readonly));
		if (refObj.previousSibling)
		{
			insertZoneSelection(refObj.previousSibling.id, 'afterEnd', refObj.nextSibling.id);
		}
		else
		{
			insertZoneSelection(refObj.nextSibling.nextSibling.nextSibling.id, 'beforeBegin', refObj.nextSibling.id);
		}
		whereToPaste = refObj.nextSibling.nextSibling.id;
	}
}

function insertUseTemplate() {
	var menuTools = document.getElementById("menuTools");
	var refObj = document.getElementById(menuTools.clickRef);
	var textObj = document.getElementById(menuTools.textRef);
	var indentation = menuTools.indent;
	switch(menuTools.direction) {
		case 'inner':
			var range = document.selection.createRange();
			var sr = range.duplicate();
			sr.moveToElementText(textObj);
			sr.setEndPoint('EndToEnd', range);
			var selectionStart = sr.text.length - range.text.length;
			var selText = document.selection.createRange().text;
			if (selectionStart != -1 && selectionEnd != -1)
			{
				stack("beforeInsert", refObj);
				var before = textObj.innerText.substring(0, selectionStart);
				var after = textObj.innerText.substring(selectionStart + selText.length);
				refObj.insertAdjacentHTML('beforeBegin', CreateText(before, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.insertAdjacentHTML('beforeBegin', CreateUseTemplate(selText, "@var1=valeur de la variable 1 @var2=val2", indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateText(after, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.previousSibling.previousSibling.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
				toolLeave(refObj.id, "template");
				stack("afterInsert", refObj.previousSibling.previousSibling);
				removeNoStack(refObj.id);
			}
			break;
		case 'before':
		    refObj.insertAdjacentHTML('beforeBegin', CreateUseTemplate("/Programs/Templates/myTemplate", "@var1=valeur de la variable 1 @var2=val2", indentation, false));
		    refObj.insertAdjacentHTML('beforeBegin', CreatePaste(indentation));
		    stack("insert", refObj.previousSibling.previousSibling);
		    insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
		    break;
		case 'after':
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreatePaste(indentation));
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateUseTemplate("/Programs/Templates/myTemplate", "@var1=valeur de la variable 1 @var2=val2", indentation, false));
		    stack("insert", refObj.nextSibling.nextSibling);
		    insertZoneSelection(refObj.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
	}
	menuTools.style.filter = 'alpha(opacity=0)';
	menuTools.style.display = 'none';
	lockMenuTools = 0;
}

function displayUseTemplate(path, params, indent, readonly)
{
	if (!whereToPaste)
	{
		var refObj = document.getElementById(currentContentId);
		refObj.insertAdjacentHTML('beforeEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('beforeEnd', CreateUseTemplate(path, params, indent, readonly));
		addZoneSelection(refObj.lastChild.id);
	}
	else
	{
		var refObj = document.getElementById(whereToPaste);
		refObj.insertAdjacentHTML('afterEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('afterEnd', CreateUseTemplate(path, params, indent, readonly));
		if (refObj.previousSibling)
		{
			insertZoneSelection(refObj.previousSibling.id, 'afterEnd', refObj.nextSibling.id);
		}
		else
		{
			insertZoneSelection(refObj.nextSibling.nextSibling.nextSibling.id, 'beforeBegin', refObj.nextSibling.id);
		}
		whereToPaste = refObj.nextSibling.nextSibling.id;
	}
}

function insertUseMOP() {
	var menuTools = document.getElementById("menuTools");
	var refObj = document.getElementById(menuTools.clickRef);
	var textObj = document.getElementById(menuTools.textRef);
	var indentation = menuTools.indent;
	switch(menuTools.direction) {
		case 'inner':
			var range = document.selection.createRange();
			var sr = range.duplicate();
			sr.moveToElementText(textObj);
			sr.setEndPoint('EndToEnd', range);
			var selectionStart = sr.text.length - range.text.length;
			var selText = document.selection.createRange().text;
			if (selectionStart != -1 && selectionEnd != -1)
			{
				stack("beforeInsert", refObj);
				var before = textObj.innerText.substring(0, selectionStart);
				var after = textObj.innerText.substring(selectionStart + selText.length);
				refObj.insertAdjacentHTML('beforeBegin', CreateText(before, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.insertAdjacentHTML('beforeBegin', CreateUseMOP("C", selText, "value1, value2, ...", indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateText(after, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.previousSibling.previousSibling.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
				toolLeave(refObj.id, "template");
				stack("afterInsert", refObj.previousSibling.previousSibling);
				removeNoStack(refObj.id);
			}
			break;
		case 'before':
		    refObj.insertAdjacentHTML('beforeBegin', CreateUseMOP("C", "command", "value1, value2, ...", indentation, false));
		    refObj.insertAdjacentHTML('beforeBegin', CreatePaste(indentation));
		    stack("insert", refObj.previousSibling.previousSibling);
		    insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
		    break;
		case 'after':
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreatePaste(indentation));
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateUseMOP("C", "command", "value1, value2, ...", indentation, false));
		    stack("insert", refObj.nextSibling.nextSibling);
		    insertZoneSelection(refObj.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
	}
	menuTools.style.filter = 'alpha(opacity=0)';
	menuTools.style.display = 'none';
	lockMenuTools = 0;
}

function displayUseMOP(language, command, expr, indent, readonly)
{
	if (!whereToPaste)
	{
		var refObj = document.getElementById(currentContentId);
		refObj.insertAdjacentHTML('beforeEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('beforeEnd', CreateUseMOP(language, command, expr, indent, readonly));
		addZoneSelection(refObj.lastChild.id);
	}
	else
	{
		var refObj = document.getElementById(whereToPaste);
		refObj.insertAdjacentHTML('afterEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('afterEnd', CreateUseMOP(language, name, expr, indent, readonly));
		if (refObj.previousSibling)
		{
			insertZoneSelection(refObj.previousSibling.id, 'afterEnd', refObj.nextSibling.id);
		}
		else
		{
			insertZoneSelection(refObj.nextSibling.nextSibling.nextSibling.id, 'beforeBegin', refObj.nextSibling.id);
		}
		whereToPaste = refObj.nextSibling.nextSibling.id;
	}
}

function insertWriter() {
	var menuTools = document.getElementById("menuTools");
	var refObj = document.getElementById(menuTools.clickRef);
	var textObj = document.getElementById(menuTools.textRef);
	var indentation = menuTools.indent;
	switch(menuTools.direction) {
		case 'inner':
			var range = document.selection.createRange();
			var sr = range.duplicate();
			sr.moveToElementText(textObj);
			sr.setEndPoint('EndToEnd', range);
			var selectionStart = sr.text.length - range.text.length;
			var selText = document.selection.createRange().text;
			if (selectionStart != -1 && selectionEnd != -1)
			{
				stack("beforeInsert", refObj);
				var before = textObj.innerText.substring(0, selectionStart);
				var after = textObj.innerText.substring(selectionStart + selText.length);
				refObj.insertAdjacentHTML('beforeBegin', CreateText(before, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.insertAdjacentHTML('beforeBegin', CreateWriter(selText, "", indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateText(after, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.previousSibling.previousSibling.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
				toolLeave(refObj.id, "template");
				stack("afterInsert", refObj.previousSibling.previousSibling);
				removeNoStack(refObj.id);
			}
			break;
		case 'before':
		    refObj.insertAdjacentHTML('beforeBegin', CreateWriter("writer", "", indentation, false));
		    refObj.insertAdjacentHTML('beforeBegin', CreatePaste(indentation));
		    stack("insert", refObj.previousSibling.previousSibling);
		    insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
		    break;
		case 'after':
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreatePaste(indentation));
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateWriter("writer", "", indentation, false));
		    stack("insert", refObj.nextSibling.nextSibling);
		    insertZoneSelection(refObj.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
	}
	menuTools.style.filter = 'alpha(opacity=0)';
	menuTools.style.display = 'none';
	lockMenuTools = 0;
}

function displayWriter(writer, file, indent, readonly)
{
	if (!whereToPaste)
	{
		var refObj = document.getElementById(currentContentId);
		refObj.insertAdjacentHTML('beforeEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('beforeEnd', CreateWriter(writer, file, indent, readonly));
		addZoneSelection(refObj.lastChild.id);
	}
	else
	{
		var refObj = document.getElementById(whereToPaste);
		refObj.insertAdjacentHTML('afterEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('afterEnd', CreateWriter(writer, file, indent, readonly));
		if (refObj.previousSibling)
		{
			insertZoneSelection(refObj.previousSibling.id, 'afterEnd', refObj.nextSibling.id);
		}
		else
		{
			insertZoneSelection(refObj.nextSibling.nextSibling.nextSibling.id, 'beforeBegin', refObj.nextSibling.id);
		}
		whereToPaste = refObj.nextSibling.nextSibling.id;
	}
}

function insertDefaultWriter() {
	var menuTools = document.getElementById("menuTools");
	var refObj = document.getElementById(menuTools.clickRef);
	var textObj = document.getElementById(menuTools.textRef);
	var indentation = menuTools.indent;
	switch(menuTools.direction) {
		case 'inner':
			var range = document.selection.createRange();
			var sr = range.duplicate();
			sr.moveToElementText(textObj);
			sr.setEndPoint('EndToEnd', range);
			var selectionStart = sr.text.length - range.text.length;
			var selText = document.selection.createRange().text;
			if (selectionStart != -1 && selectionEnd != -1)
			{
				stack("beforeInsert", refObj);
				var before = textObj.innerText.substring(0, selectionStart);
				var after = textObj.innerText.substring(selectionStart + selText.length);
				refObj.insertAdjacentHTML('beforeBegin', CreateText(before, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.insertAdjacentHTML('beforeBegin', CreateUseWriter(selText, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateText(after, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.previousSibling.previousSibling.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
				toolLeave(refObj.id, "template");
				stack("afterInsert", refObj.previousSibling.previousSibling);
				removeNoStack(refObj.id);
			}
			break;
		case 'before':
		    refObj.insertAdjacentHTML('beforeBegin', CreateUseWriter("writer", indentation, false));
		    refObj.insertAdjacentHTML('beforeBegin', CreatePaste(indentation));
		    stack("insert", refObj.previousSibling.previousSibling);
		    insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
		    break;
		case 'after':
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreatePaste(indentation));
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateUseWriter("writer", indentation, false));
		    stack("insert", refObj.nextSibling.nextSibling);
		    insertZoneSelection(refObj.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
	}
	menuTools.style.filter = 'alpha(opacity=0)';
	menuTools.style.display = 'none';
	lockMenuTools = 0;
}

function displayDefaultWriter(writer, indent, readonly)
{
	if (!whereToPaste)
	{
		var refObj = document.getElementById(currentContentId);
		refObj.insertAdjacentHTML('beforeEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('beforeEnd', CreateUseWriter(writer, indent, readonly));
		addZoneSelection(refObj.lastChild.id);
	}
	else
	{
		var refObj = document.getElementById(whereToPaste);
		refObj.insertAdjacentHTML('afterEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('afterEnd', CreateUseWriter(writer, indent, readonly));
		if (refObj.previousSibling)
		{
			insertZoneSelection(refObj.previousSibling.id, 'afterEnd', refObj.nextSibling.id);
		}
		else
		{
			insertZoneSelection(refObj.nextSibling.nextSibling.nextSibling.id, 'beforeBegin', refObj.nextSibling.id);
		}
		whereToPaste = refObj.nextSibling.nextSibling.id;
	}
}

function insertParallel() {
	var menuTools = document.getElementById("menuTools");
	var refObj = document.getElementById(menuTools.clickRef);
	var textObj = document.getElementById(menuTools.textRef);
	var indentation = menuTools.indent;
	switch(menuTools.direction) {
		case 'inner':
			var range = document.selection.createRange();
			var sr = range.duplicate();
			sr.moveToElementText(textObj);
			sr.setEndPoint('EndToEnd', range);
			var selectionStart = sr.text.length - range.text.length;
			var selText = document.selection.createRange().text;
			if (selectionStart != -1 && selectionEnd != -1)
			{
				stack("beforeInsert", refObj);
				var before = textObj.innerText.substring(0, selectionStart);
				var after = textObj.innerText.substring(selectionStart + selText.length);
				refObj.insertAdjacentHTML('beforeBegin', CreateText(before, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.insertAdjacentHTML('beforeBegin', CreateParallel(indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateText(after, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.previousSibling.previousSibling.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
				toolLeave(refObj.id, "texte");
				stack("afterInsert", refObj.previousSibling.previousSibling);
				removeNoStack(refObj.id);
			}
			break;
		case 'before':
		    refObj.insertAdjacentHTML('beforeBegin', CreateParallel(indentation, false));
		    refObj.insertAdjacentHTML('beforeBegin', CreatePaste(indentation));
		    stack("insert", refObj.previousSibling.previousSibling);
		    insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
		    break;
		case 'after':
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreatePaste(indentation));
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateParallel(indentation, false));
		    stack("insert", refObj.nextSibling.nextSibling);
		    insertZoneSelection(refObj.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
	}
	menuTools.style.filter = 'alpha(opacity=0)';
	menuTools.style.display = 'none';
	lockMenuTools = 0;
}

function displayParallel(indent, readonly)
{
	if (!whereToPaste)
	{
		var refObj = document.getElementById(currentContentId);
		refObj.insertAdjacentHTML('beforeEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('beforeEnd', CreateParallel(indent, readonly));
		addZoneSelection(refObj.lastChild.id);
	}
	else
	{
		var refObj = document.getElementById(whereToPaste);
		refObj.insertAdjacentHTML('afterEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('afterEnd', CreateParallel(indent, readonly));
		if (refObj.previousSibling)
		{
			insertZoneSelection(refObj.previousSibling.id, 'afterEnd', refObj.nextSibling.id);
		}
		else
		{
			insertZoneSelection(refObj.nextSibling.nextSibling.nextSibling.id, 'beforeBegin', refObj.nextSibling.id);
		}
		whereToPaste = refObj.nextSibling.nextSibling.id;
	}
}

function insertSyntax(name) {
	var menuTools = document.getElementById("menuTools");
	var refObj = document.getElementById(menuTools.clickRef);
	var textObj = document.getElementById(menuTools.textRef);
	var indentation = menuTools.indent;
	switch(menuTools.direction) {
		case 'inner':
			var range = document.selection.createRange();
			var sr = range.duplicate();
			sr.moveToElementText(textObj);
			sr.setEndPoint('EndToEnd', range);
			var selectionStart = sr.text.length - range.text.length;
			var selText = document.selection.createRange().text;
			if (selectionStart != -1 && selectionEnd != -1)
			{
				stack("beforeInsert", refObj);
				var before = textObj.innerText.substring(0, selectionStart);
				var after = textObj.innerText.substring(selectionStart + selText.length);
				refObj.insertAdjacentHTML('beforeBegin', CreateText(before, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.insertAdjacentHTML('beforeBegin', CreateSyntax(selText, "<texte>Nouvelle syntaxe</texte>", new ActiveXObject("o2Mate.LegendeDict"), indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
				refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateText(after, indentation, false) + CreatePaste(indentation));
				insertZoneSelection(refObj.previousSibling.previousSibling.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
				toolLeave(refObj.id, "texte");
				stack("afterInsert", refObj.previousSibling.previousSibling);
				removeNoStack(refObj.id);
			}
			break;
		case 'before':
		    refObj.insertAdjacentHTML('beforeBegin', CreateSyntax("nom", "<texte>Nouvelle syntaxe</texte>", new ActiveXObject("o2Mate.LegendeDict"), indentation, false));
		    refObj.insertAdjacentHTML('beforeBegin', CreatePaste(indentation));
		    stack("insert", refObj.previousSibling.previousSibling);
		    insertZoneSelection(refObj.id, 'beforeBegin', refObj.previousSibling.previousSibling.id);
		    break;
		case 'after':
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreatePaste(indentation));
		    refObj.nextSibling.insertAdjacentHTML('afterEnd', CreateSyntax("nom", "<texte>Nouvelle syntaxe</texte>", new ActiveXObject("o2Mate.LegendeDict"), indentation, false));
		    stack("insert", refObj.nextSibling.nextSibling);
		    insertZoneSelection(refObj.id, 'afterEnd', refObj.nextSibling.nextSibling.id);
	}
	menuTools.style.filter = 'alpha(opacity=0)';
	menuTools.style.display = 'none';
	lockMenuTools = 0;
}

function displaySyntax(name, xmlCode, myLegendes, indent, readonly)
{
	if (!whereToPaste)
	{
		var refObj = document.getElementById(currentContentId);
		refObj.insertAdjacentHTML('beforeEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('beforeEnd', CreateSyntax(name, xmlCode, myLegendes, indent, readonly));
		addZoneSelection(refObj.lastChild.id);
	}
	else
	{
		var refObj = document.getElementById(whereToPaste);
		refObj.insertAdjacentHTML('afterEnd', CreatePaste(indent));
		refObj.insertAdjacentHTML('afterEnd', CreateSyntax(name, xmlCode, myLegendes, indent, readonly));
		if (refObj.previousSibling)
		{
			insertZoneSelection(refObj.previousSibling.id, 'afterEnd', refObj.nextSibling.id);
		}
		else
		{
			insertZoneSelection(refObj.nextSibling.nextSibling.nextSibling.id, 'beforeBegin', refObj.nextSibling.id);
		}
		whereToPaste = refObj.nextSibling.nextSibling.id;
	}
}


function newDocument(text)
{
	var refObj = document.getElementById("src");
	refObj.innerHTML = CreatePaste(0) + CreateText(text, 0, false) + CreatePaste(0);
	clearZoneSelection();
	addZoneSelection(refObj.childNodes[1].id);
	// création des legendes
	LoadMasterLegendes(new ActiveXObject("o2Mate.LegendeDict"));
}

function writePasteAtEnd()
{
    var refObj = document.getElementById(getCurrentContent());
	refObj.insertAdjacentHTML('beforeEnd', CreatePaste(0));
}

function deploy(id) {
	var obj = document.getElementById(id);
	if (obj.style.display == 'block')
	{
		obj.style.display = 'none';
	}
	else
	{
		obj.style.display = 'block';
	}
}

function removeNoStack(name)
{
	setTimeout('if (lock[\'' + name + '\'] == 0) { removeZoneSelection(\'' + name + '\');document.getElementById(currentContentId).removeChild(document.getElementById(\'' + name + '\').previousSibling);document.getElementById(currentContentId).removeChild(document.getElementById(\'' + name + '\')); lock[\'' + name + '\'] = 3;} else { removeNoStack(\'' + name + '\'); }', 10);
}

function remove(name)
{
	setTimeout('if (lock[\'' + name + '\'] == 0) { stack(\'delete\', document.getElementById(\'' + name + '\'));removeZoneSelection(\'' + name + '\');document.getElementById(currentContentId).removeChild(document.getElementById(\'' + name + '\').previousSibling);document.getElementById(currentContentId).removeChild(document.getElementById(\'' + name + '\')); lock[\'' + name + '\'] = 3;} else { remove(\'' + name + '\'); }', 10);
}

function unlock(name)
{
	lock[name] = 0;
}

function pasteCodeCommander(id, indent)
{
	var callback = document.getElementById("callback");
	setWhereToPaste(id);
	callback.action = "paste";
	callback.indent = indent;
	callback.click();
	unsetWhereToPaste();
}

function pasteFromObject(id)
{
	var obj = document.getElementById(id);
	pasteCodeCommander(obj.nextSibling.id, obj.indent);
}

String.prototype.lpad = function(padString, length) {
	var str = this;
    while (str.length < length)
        str = padString + str;
    return str;
}

function setDirty()
{
	if (!isDirty) {
		callback.action = "Dirty";
		callback.click();
	}
	isDirty = true;
	var now = new Date();
	document.getElementById('modifiedDate').innerText = now.getDate().toString().lpad("0",2) + "/" + (now.getMonth()+1).toString().lpad("0",2) + "/" + now.getYear() + " " + now.getHours().toString().lpad("0",2) + ":" + now.getMinutes().toString().lpad("0",2) + ":" + now.getSeconds().toString().lpad("0",2);
}

function clearDirty()
{
	isDirty = false;
}

function getDirty()
{
	return isDirty;
}

function openHeader(title, dateCreation, dateModified, revision)
{
	document.getElementById('header').className = "active"
	document.getElementById('documentName').value = title;
	document.getElementById('creationDate').innerText = dateCreation;
	document.getElementById('modifiedDate').innerText = dateModified;
	document.getElementById('revision').value = revision;
}

function errorFileNameExists(reason)
{
	document.getElementById('errorFile').className = "active-inline";
	document.getElementById('reasonErrorFile').innerText = reason;
}

function noErrorFileName()
{
	document.getElementById('errorFile').className = "inactive";	
	document.getElementById('reasonErrorFile').innerText = "";
}

function getWhereToPaste()
{
	return whereToPaste;
}

function setWhereToPaste(id)
{
	whereToPaste = id;
}

function unsetWhereToPaste()
{
	whereToPaste = "";
}

function getCurrentContent()
{
	return currentContentId;
}

function writeSyntax(text)
{
	var obj = document.getElementById(currentContentId);
	if (obj)
	{
		setWhereToPaste(obj.lastChild.id);
		displayText(text, obj.lastChild.previousSibling.indent, false);
		unsetWhereToPaste();
		setDirty();
	}
}

function cutAndStore()
{
	var menuTools = document.getElementById("menuTools");
	var textObj = document.getElementById(menuTools.textRef);
	var range = document.selection.createRange();
	var sr = range.duplicate();
	sr.moveToElementText(textObj);
	sr.setEndPoint('EndToEnd', range);
	var selectionStart = sr.text.length - range.text.length;
	var selText = document.selection.createRange().text;
	if (selectionStart != -1 && selectionEnd != -1)
	{
		var before = textObj.innerText.substring(0, selectionStart);
		var after = textObj.innerText.substring(selectionStart + selText.length);
		callback.action = "cutAndStore";
		callback.text = selText;
		callback.click();
		sr.setEndPoint('StartToEnd', range);
		range.text = "";
		setDirty();
		if (lockPaste)
		{
			callback.action = "store";
			callback.repositoryId = "divPasteAndStore";
			callback.click();
			initDragAndDrop("paste", "pasteAndStore");
		}
	}
	closeMenuTools();
}