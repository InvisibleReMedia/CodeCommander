var currentSelectionId = "selectionArea";
var previousSelectionObject;
var selectionActivated = false;

function changeToPopup(popupNumber)
{
	if (popupNumber)
		currentSelectionId = "selectionArea" + popupNumber.toString();
	else
		currentSelectionId = "selectionArea";
}

function searchPreviousSelection(toolId)
{
	var sa = document.getElementById("selection" + toolId);
	if (sa.previousSibling)
	{
		return sa.previousSibling.id;
	}
	else
	{
		return "";
	}
}

function addTopZoneSelection(toolId)
{
	var sa = document.getElementById(currentSelectionId).firstChild;
	sa.insertAdjacentHTML('afterEnd', CreateGap());
	sa.insertAdjacentHTML('afterEnd', CreateZoneSelection(toolId));
}

function addZoneSelection(toolId)
{
	var sa = document.getElementById(currentSelectionId);
	if (sa)
	{
		sa.insertAdjacentHTML('beforeEnd', CreateGap());
		sa.insertAdjacentHTML('beforeEnd', CreateZoneSelection(toolId));
	}
}

function insertZoneSelection(from, placement, toolId)
{
	var sa = document.getElementById("selection" + from);
	if (sa)
	{
		sa.insertAdjacentHTML(placement, CreateZoneSelection(toolId));
		sa.insertAdjacentHTML(placement, CreateGap());
	}
	else
	{
		addZoneSelection(toolId);
	}
}

function insertGap(from, placement)
{
	var sa = document.getElementById("selection" + from);
	sa.insertAdjacentHTML(placement, CreateGap());
}

function removeZoneSelection(toolId)
{
	var sa = document.getElementById(currentSelectionId);
	var obj = document.getElementById("selection" + toolId);
	var gap = obj.previousSibling;
	sa.removeChild(obj);
	sa.removeChild(gap);
}

function resizeSelection(toolId)
{
	var objSelection = document.getElementById("selection" + toolId);
	var tool = document.getElementById(toolId);
	if (objSelection)
	{
		objSelection.style.height = tool.offsetHeight + "px";
	}
}

function clearZoneSelection()
{
	var sa = document.getElementById(currentSelectionId);
	sa.innerHTML = "";
}

function activeSelectionObject(obj)
{
	if (selectionActivated)
	{
		if (previousSelectionObject)
		{
			if (obj.selected && previousSelectionObject.selected &&
				((obj.previousSibling.previousSibling && obj.previousSibling.previousSibling.id == previousSelectionObject.id) || 
				(obj.nextSibling && obj.nextSibling.nextSibling && obj.nextSibling.nextSibling.id == previousSelectionObject.id)))
			{
				var toolId = previousSelectionObject.id.substring(9);
				var tool = document.getElementById(toolId);
				tool.style.border = "1px solid white";
				previousSelectionObject.style.backgroundColor = "";
				var objects = document.getElementById("selectedObjects");
				objects.removeChild(document.getElementById("object" + toolId));
				previousSelectionObject.selected = false;
			}
		}
		var toolId = obj.id.substring(9);
		var tool = document.getElementById(toolId);
		tool.style.border = "1px outset gray";
		obj.style.backgroundColor = "gray";
		var objects = document.getElementById("selectedObjects");
		objects.insertAdjacentHTML('beforeEnd', "<li id='object" + toolId + "' selection='selection" + toolId + "' tool='" + toolId + "'/>");
		obj.selected = true;
		if (tool.type == "coding" || tool.type == "usetemplate" || tool.type == "parallel")
		{
			var nextObject = tool.nextSibling;
			while(nextObject)
			{
				if (nextObject.type == "paste")
				{
				} else if (nextObject.indent > tool.indent)
				{
					nextObject.style.border = "1px outset gray";
					var selObj = document.getElementById("selection" + nextObject.id);
					selObj.style.backgroundColor = "gray";
					selObj.selected = true;
				}
				else
				{
					break;
				}
				nextObject = nextObject.nextSibling;
			}
		}
		previousSelectionObject = obj;
	}
}

function clickSelectionObject(obj)
{
	if (!selectionActivated)
	{
		var objects = document.getElementById("selectedObjects");
		for(index = 0; index < objects.childNodes.length; ++index)
		{
			toolId = objects.childNodes[index].tool;
			var tool = document.getElementById(toolId);
			selObj = document.getElementById("selection" + toolId);
			selObj.style.backgroundColor = "";
			selObj.selected = false;
			if (tool.type == "coding" || tool.type == "usetemplate" || tool.type == "parallel")
			{
				var nextObject = tool.nextSibling;
				while(nextObject)
				{
					if (nextObject.type == "paste")
					{
					} else if (nextObject.indent > tool.indent)
					{
						var selObj = document.getElementById("selection" + nextObject.id);
						selObj.style.backgroundColor = "";
						selObj.selected = false;
					}
					else
					{
						break;
					}
					nextObject = nextObject.nextSibling;
				}
			}
		}
		while(objects.firstChild)
			objects.removeChild(objects.firstChild);
		var toolId = obj.id.substring(9);
		var tool = document.getElementById(toolId);
		tool.style.border = "1px outset gray";
		obj.style.backgroundColor = "gray";
		objects.insertAdjacentHTML('beforeEnd', "<li id='object" + toolId + "' selection='selection" + toolId + "' tool='" + toolId + "'/>");
		obj.selected = true;
		if (tool.type == "coding" || tool.type == "usetemplate" || tool.type == "parallel")
		{
			var nextObject = tool.nextSibling;
			while(nextObject)
			{
				if (nextObject.type == "paste")
				{
				} else if (nextObject.indent > tool.indent)
				{
					nextObject.style.border = "1px outset gray";
					var selObj = document.getElementById("selection" + nextObject.id);
					selObj.style.backgroundColor = "gray";
					selObj.selected = true;
				}
				else
				{
					break;
				}
				nextObject = nextObject.nextSibling;
			}
		}
		previousSelectionObject = obj;
		selectionActivated = true;
	}
	else
	{
		var toolId = obj.id.substring(9);
		var tool = document.getElementById(toolId);
		tool.style.border = "1px solid white";
		tool.style.borderLeft = "1px solid black";
		obj.style.backgroundColor = "";
		obj.selected = false;
		previousSelectionObject = "";
		selectionActivated = false;
		var objects = document.getElementById("selectedObjects");
		var clip = document.getElementById("copyToClipboard");
		clip.ready = "YES";
		clip.objects = "selectedObjects";
		for(index = 0; index < objects.childNodes.length; ++index)
		{
			toolId = objects.childNodes[index].tool;
			tool = document.getElementById(toolId);
			tool.style.border = "1px solid white";
			tool.style.borderLeft = "1px solid black";
			selObj = document.getElementById("selection" + toolId);
			selObj.style.backgroundColor = "blue";
			if (tool.type == "coding" || tool.type == "usetemplate" || tool.type == "parallel")
			{
				var nextObject = tool.nextSibling;
				while(nextObject)
				{
					if (nextObject.type == "paste")
					{
					} else if (nextObject.indent > tool.indent)
					{
						nextObject.style.border = "1px solid white";
						nextObject.style.borderLeft = "1px solid black";
						var selObj = document.getElementById("selection" + nextObject.id);
						selObj.style.backgroundColor = "blue";
						selObj.selected = true;
					}
					else
					{
						break;
					}
					nextObject = nextObject.nextSibling;
				}
			}
		}
	}
}

function endSelectionObject()
{
	var clip = document.getElementById("copyToClipboard");
	clip.ready = "YES";
	clip.objects = "selectedObjects";
	var objects = document.getElementById("selectedObjects");
	for(index = 0; index < objects.childNodes.length; ++index)
	{
		toolId = objects.childNodes[index].tool;
		tool = document.getElementById(toolId);
		tool.style.border = "1px solid white";
		tool.style.borderLeft = "1px solid black";
		selObj = document.getElementById("selection" + toolId);
		selObj.style.backgroundColor = "blue";
		if (tool.type == "coding" || tool.type == "usetemplate" || tool.type == "parallel")
		{
			var nextObject = tool.nextSibling;
			while(nextObject)
			{
				if (nextObject.type == "paste")
				{
				} else if (nextObject.indent > tool.indent)
				{
					nextObject.style.border = "1px outset white";
					nextObject.style.borderLeft = "1px solid black";
					var selObj = document.getElementById("selection" + nextObject.id);
					selObj.style.backgroundColor = "blue";
					selObj.selected = true;
				}
				else
				{
					break;
				}
				nextObject = nextObject.nextSibling;
			}
		}
	}
	selectionActivated = false;
}

function endCopy()
{
	var objects = document.getElementById("selectedObjects");
	for(index = 0; index < objects.childNodes.length; ++index)
	{
		toolId = objects.childNodes[index].tool;
		var tool = document.getElementById(toolId);
		selObj = document.getElementById("selection" + toolId);
		selObj.style.backgroundColor = "";
		selObj.selected = false;
		if (tool.type == "coding" || tool.type == "usetemplate" || tool.type == "parallel")
		{
			var nextObject = tool.nextSibling;
			while(nextObject)
			{
				if (nextObject.type == "paste")
				{
				} else if (nextObject.indent > tool.indent)
				{
					var selObj = document.getElementById("selection" + nextObject.id);
					selObj.style.backgroundColor = "";
					selObj.selected = false;
				}
				else
				{
					break;
				}
				nextObject = nextObject.nextSibling;
			}
		}
	}
	while(objects.firstChild)
		objects.removeChild(objects.firstChild);
}

function endCut()
{
	var list = document.createElement("ul");
	setDirty();
	var objects = document.getElementById("selectedObjects");
	for(index = 0; index < objects.childNodes.length; ++index)
	{
		toolId = objects.childNodes[index].tool;
		var item = document.createElement("li");
		item.toolName = toolId;
		var tool = document.getElementById(toolId);
		if (tool.previousSibling.previousSibling)
		{
			item.previous = tool.previousSibling.previousSibling.id;
		}
		else
		{
			item.first = "true";
		}
		item.source = tool.outerHTML;
		list.appendChild(item);
		item = document.createElement("li");
		item.isNext = "true";
		item.toolName = tool.nextSibling.id;
		item.source = tool.nextSibling.outerHTML;
		list.appendChild(item);
		selObj = document.getElementById("selection" + toolId);
		selObj.style.backgroundColor = "";
		selObj.selected = false;
		if (tool.type == "coding" || tool.type == "usetemplate" || tool.type == "parallel")
		{
			var nextObject = tool.nextSibling.nextSibling;
			while(nextObject)
			{
				if (nextObject.type == "paste")
				{
					var temp = nextObject.nextSibling;
					item = document.createElement("li");
					item.isNext = "true";
					item.toolName = nextObject.id;
					item.source = nextObject.outerHTML;
					list.appendChild(item);
					nextObject = temp;
				}
				else if (nextObject.indent > tool.indent)
				{
					var selObj = document.getElementById("selection" + nextObject.id);
					selObj.style.backgroundColor = "";
					selObj.selected = false;
					var temp = nextObject.nextSibling;
					item = document.createElement("li");
					item.isNext = "true";
					item.toolName = nextObject.id;
					item.source = nextObject.outerHTML;
					list.appendChild(item);
					unlock(nextObject.id);
					removeNoStack(nextObject.id);
					nextObject = temp;
				}
				else
				{
					break;
				}
			}
		}
		unlock(toolId);
		removeNoStack(toolId);
	}
	while(objects.firstChild)
		objects.removeChild(objects.firstChild);
	stack("cut", list);
}

function endPaste()
{
	setDirty();
	// enregistrement des objets à coller
	var list = GetNames();
	stack("paste", list);
}