var undoStack = document.getElementById('stack');
var storage = document.getElementById('storage');
var currentPositionStack = 0;
var lockUndoRedo = 0;

function getElementByName(obj, searchName)
{
	if (obj.name == searchName)
	{
		return obj;
	}
	var index;
	for(index=0; index < obj.childNodes.length; ++index)
	{
		var res = getElementByName(obj.childNodes[index], searchName);
		if (res)
		{
			return res;
		}
	}
}

function stack(action, object, reference)
{
	switch(action)
	{
		case "cut":
		{
			if (currentPositionStack != undoStack.childNodes.length)
			{
				while(undoStack.childNodes.length > currentPositionStack)
					undoStack.removeChild(undoStack.lastChild);
			}
			undoStack.innerHTML += "<li action='cut'><ul>" + object.innerHTML + "</ul></li>";
			++currentPositionStack;
			break;
		}
		case "paste":
		{
			if (currentPositionStack != undoStack.childNodes.length)
			{
				while(undoStack.childNodes.length > currentPositionStack)
					undoStack.removeChild(undoStack.lastChild);
			}
			undoStack.innerHTML += "<li action='paste' objects='" + object + "'/>";
			++currentPositionStack;
			break;
		}
		case "popup":
		{
			if (currentPositionStack != undoStack.childNodes.length)
			{
				while(undoStack.childNodes.length > currentPositionStack)
					undoStack.removeChild(undoStack.lastChild);
			}
			undoStack.innerHTML += "<li action='popup'/>";
			++currentPositionStack;
			break;
		}
		case "insert":
		{
			setDirty();
			var ne = object.nextSibling.nextSibling;
			var pe = object.previousSibling.previousSibling;
			if (!pe) { pe = "first"; } else { pe = pe.id; } 
			if (!ne) { ne = "last"; } else { ne = ne.id; }
			if (currentPositionStack != undoStack.childNodes.length)
			{
				while(undoStack.childNodes.length > currentPositionStack)
					undoStack.removeChild(undoStack.lastChild);
			}
			undoStack.innerHTML += "<li action='insert' object='" + object.id + "' before='" + pe + "' after='" + ne + "'/>";
			++currentPositionStack;
			break;
		}
		case "delete":
		{
			setDirty();
			var ne = object.nextSibling.nextSibling;
			var pe = object.previousSibling.previousSibling;
			if (!pe) { pe = "first"; } else { pe = pe.id; } 
			if (!ne) { ne = "last"; } else { ne = ne.id; }
			if (currentPositionStack != undoStack.childNodes.length)
			{
				while(undoStack.childNodes.length > currentPositionStack)
					undoStack.removeChild(undoStack.lastChild);
			}
			undoStack.innerHTML += "<li action='delete' object='" + object.id + "' before='" + pe + "' after='" + ne + "'/>";
			undoStack.childNodes[undoStack.childNodes.length - 1].source = object.outerHTML;
			++currentPositionStack;
			break;
		}
		case "beforeInsert":
		{
			var text = getElementByName(object, "text");
			storage.source = object.outerHTML;
			storage.initialId = object.id;
			break;
		}
		case "afterInsert":
		{
			setDirty();
			var before = object.previousSibling.previousSibling;
			var after = object.nextSibling.nextSibling.nextSibling.nextSibling;
			if (currentPositionStack != undoStack.childNodes.length)
			{
				while(undoStack.childNodes.length > currentPositionStack)
					undoStack.removeChild(undoStack.lastChild);
			}
			var selectionBefore = searchPreviousSelection(before.id);
			undoStack.innerHTML += "<li action='afterInsert' selection='" + selectionBefore + "' object='" + object.id + "' before='" + before.id + "' after='" + after.id + "' initialId='" + storage.initialId + "'/>";
			undoStack.childNodes[undoStack.childNodes.length - 1].source = storage.source;
			++currentPositionStack;
			break;
		}
		case "update":
		{
			lockUndoRedo = 0;
			if (storage.reference)
			{
				var text = getElementByName(object, storage.reference);
				if (storage.object == object.id && storage.texte != text.innerText)
				{
					if (storage.reference == "variable" || storage.reference == "tableau" || storage.reference == "champ" || storage.reference == "expression" || storage.reference == "size") {
						if (storage.reference == "champ")
						{
							var tabName = getElementByName(object, "tableau").innerText;
							RenameLegende(tabName, storage.texte, tabName, text.innerText);
						}
						else if (storage.reference == "tableau")
						{
							RenameLegende("", storage.texte, "", text.innerText);
							var champName = getElementByName(object, "champ").innerText;
							RenameLegende(storage.texte, champName, text.innerText, champName)
						}
						else
						{
							RenameLegende("", storage.texte, "", text.innerText);
						}
					}
					setDirty();
					if (currentPositionStack != undoStack.childNodes.length)
					{
						while(undoStack.childNodes.length > currentPositionStack)
							undoStack.removeChild(undoStack.lastChild);
					}
					undoStack.innerHTML += "<li action='update' object='" + object.id + "' reference='" + storage.reference + "' oldtext='" + storage.texte + "' newtext='" + text.innerText + "'/>";
					++currentPositionStack;
				}
			}
			break;
		}
		case "store":
		{
			lockUndoRedo = 1;
			if (reference)
			{
				storage.reference = reference.name;
				var text = getElementByName(object, storage.reference);
				storage.object = object.id;
				storage.texte = text.innerText;
			}
			else
			{
				storage.reference = "";
				storage.object = object.id;
				storage.texte = "référence introuvable";
			}
			break;
		}
	}
}

function undo()
{
	if (lockUndoRedo) return;
	if (currentPositionStack > 0)
	{
		--currentPositionStack;
		var elem = undoStack.childNodes[currentPositionStack];
		switch(elem.action)
		{
			case "cut":
			{
				setDirty();
				var previousId;
				var list = "";
				for(index = 0; index < elem.firstChild.childNodes.length; ++index)
				{
					var obj = elem.firstChild.childNodes[index];
					list += obj.toolName + ",";
					if (obj.isNext)
					{
						var prev = document.getElementById(previousId);
						prev.insertAdjacentHTML("afterEnd", obj.source);
						if (obj.toolName.substring(0, 5) == "table")
						{
							insertZoneSelection(prev.previousSibling.id, "afterEnd", prev.nextSibling.id);
						}
						previousId = obj.toolName;
					}
					else
					{
						previousId = obj.toolName;
						if (obj.first)
						{
							var src = document.getElementById(getCurrentContent()).firstChild;
							src.insertAdjacentHTML("afterEnd", obj.source);
							addTopZoneSelection(obj.toolName);
						}
						else
						{
							var prev = document.getElementById(obj.previous);
							prev.nextSibling.insertAdjacentHTML("afterEnd", obj.source);
							if (obj.toolName.substring(0, 5) == "table")
							{
								insertZoneSelection(prev.id, "afterEnd", prev.nextSibling.nextSibling.id);
							}
						}
					}
				}
				elem.objects = list;
				break;
			}
			case "paste":
			{
				setDirty();
				var objects = document.createElement("ul");
				var list = elem.objects;
				var split = list.split(',');
				for(index = 0; index < split.length; ++index)
				{
					if (split[index])
					{
						var item = document.createElement("li");
						item.toolName = split[index];
						var obj = document.getElementById(split[index]);
						item.previous = obj.previousSibling.id;
						item.source = obj.outerHTML;
						if (split[index].substring(0, 5) == "table")
						{
							unlock(split[index]);
							removeNoStack(split[index]);
						}
						objects.appendChild(item);
					}
				}
				elem.innerHTML = objects.outerHTML;
				break;
			}
			case "insert":
			{
				setDirty();
				elem.source = document.getElementById(elem.object).outerHTML;
				document.getElementById(elem.object).parentNode.removeChild(document.getElementById(elem.object).nextSibling);
				document.getElementById(elem.object).parentNode.removeChild(document.getElementById(elem.object));
				removeZoneSelection(elem.object);
				break;
			}
			case "delete":
			{
				setDirty();
				var adj;
				if (elem.before != "first")
				{
					adj = document.getElementById(elem.before).nextSibling;
					adj.insertAdjacentHTML("afterEnd", CreatePaste(0));
					adj.insertAdjacentHTML("afterEnd", elem.source);
					insertZoneSelection(elem.before, "afterEnd", elem.object);
				}
				else
				{
					if (elem.after != "last")
					{
						adj = document.getElementById(elem.after).previousSibling;
						adj.insertAdjacentHTML("beforeBegin", CreatePaste(0));
						adj.insertAdjacentHTML("beforeBegin", elem.source);
						insertZoneSelection(elem.after, "beforeBegin", elem.object);
					}
					else
					{
						adj = document.getElementById("src");
						adj.innerHTML = elem.source + CreatePaste(0);
						addZoneSelection(elem.object);
					}
				}
				break;
			}
			case "afterInsert":
			{
				setDirty();
				var adj = document.getElementById(elem.before);
				adj.insertAdjacentHTML("afterEnd", elem.source);
				elem.oldId = adj.nextSibling.id;
				elem.sourceBefore = document.getElementById(elem.before).outerHTML;
				elem.sourceAfter  = document.getElementById(elem.after).outerHTML;
				elem.sourceObject = document.getElementById(elem.object).outerHTML;
				document.getElementById(elem.object).parentNode.removeChild(document.getElementById(elem.before).nextSibling.nextSibling);
				document.getElementById(elem.object).parentNode.removeChild(document.getElementById(elem.before));
				document.getElementById(elem.object).parentNode.removeChild(document.getElementById(elem.after).previousSibling);
				document.getElementById(elem.object).parentNode.removeChild(document.getElementById(elem.after));
				document.getElementById(elem.object).parentNode.removeChild(document.getElementById(elem.object));
				removeZoneSelection(elem.before);
				removeZoneSelection(elem.object);
				removeZoneSelection(elem.after);
				insertZoneSelection(elem.selectionBefore, "afterEnd", elem.initialId);
				break;
			}
			case "update":
			{
				setDirty();
				var obj = document.getElementById(elem.object);
				var text = getElementByName(obj, elem.reference);
				text.innerText = elem.oldtext;
				if (elem.reference == "variable" || elem.reference == "tableau" || elem.reference == "champ" || elem.reference == "expression" || elem.reference == "size") {
					if (elem.reference == "champ")
					{
						var tabName = getElementByName(obj, "tableau").innerText;
						RenameLegende(tabName, elem.newtext, tabName, elem.oldtext);
					}
					else if (storage.reference == "tableau")
					{
						RenameLegende("", elem.newtext, "", elem.oldtext);
						var champName = getElementByName(obj, "champ").innerText;
						RenameLegende(elem.newtext, champName, elem.oldtext, champName)
					}
					else
					{
						RenameLegende("", elem.newtext, "", elem.oldtext);
					}
				}
				break;
			}
			case "popup":
			{
				++currentPositionStack;
				// il n'y a rien à supprimer dans la popup
				break;
			}
		}
	}
}

function redo()
{
	if (lockUndoRedo) return;
	if (currentPositionStack < undoStack.childNodes.length)
	{
		var elem = undoStack.childNodes[currentPositionStack++];
		switch(elem.action)
		{
			case "cut":
			{
				setDirty();
				var list = elem.objects;
				var split = list.split(',');
				for(index = 0; index < split.length; ++index)
				{
					if (split[index] && split[index].substring(0, 5) == "table")
					{
						unlock(split[index]);
						removeNoStack(split[index]);
					}
				}
				break;
			}
			case "paste":
			{
				setDirty();
				for(index = 0; index < elem.firstChild.childNodes.length - 1; ++index)
				{
					var obj = elem.firstChild.childNodes[index];
					if (obj.first)
					{
						var src = document.getElementById(getCurrentContent()).firstChild;
						src.insertAdjacentHTML("afterEnd", obj.source);
						addTopZoneSelection(obj.toolName);
					}
					else
					{
						var prev = document.getElementById(obj.previous);
						if (obj.toolName.substring(0, 5) == "table")
						{
							prev.insertAdjacentHTML("afterEnd", obj.source);
							insertZoneSelection(prev.id, "afterEnd", prev.nextSibling.id);
						}
						else
						{
							prev.insertAdjacentHTML("afterEnd", obj.source);
						}
					}
				}
				break;
			}
			case "afterInsert":
			{
				setDirty();
				removeZoneSelection(elem.initialId);
				var adj = document.getElementById(elem.oldId);
				adj.insertAdjacentHTML("afterEnd", elem.sourceAfter);
				insertZoneSelection(elem.selectionBefore, "afterEnd", elem.before);
				adj.insertAdjacentHTML("afterEnd", CreatePaste(0));
				adj.insertAdjacentHTML("afterEnd", elem.sourceObject);
				insertZoneSelection(elem.selectionBefore, "afterEnd", elem.object);
				adj.insertAdjacentHTML("afterEnd", CreatePaste(0));
				adj.insertAdjacentHTML("afterEnd", elem.sourceBefore);
				insertZoneSelection(elem.selectionBefore, "afterEnd", elem.after);
				document.getElementById(elem.oldId).parentNode.removeChild(document.getElementById(elem.oldId));
				break;
			}
			case "insert":
			{
				setDirty();
				var adj;
				if (elem.before != "first")
				{
					adj = document.getElementById(elem.before).nextSibling;
					adj.insertAdjacentHTML("afterEnd", CreatePaste(0));
					adj.insertAdjacentHTML("afterEnd", elem.source);
					insertZoneSelection(elem.before, "afterEnd", elem.object);
				}
				else
				{
					if (elem.after != "last")
					{
						adj = document.getElementById(elem.after).previousSibling;
						adj.insertAdjacentHTML("beforeBegin", CreatePaste(0));
						adj.insertAdjacentHTML("beforeBegin", elem.source);
						insertZoneSelection(elem.after, "beforeBegin", elem.object);
					}
					else
					{
						alert("last and first");
						adj = document.getElementById("src");
						adj.innerHTML = elem.source + CreatePaste(0);
						addZoneSelection(elem.object);
					}
				}
				break;
			}
			case "delete":
			{
				setDirty();
				document.getElementById(elem.object).parentNode.removeChild(document.getElementById(elem.object).nextSibling);
				document.getElementById(elem.object).parentNode.removeChild(document.getElementById(elem.object));
				removeZoneSelection(elem.object);
				break;
			}
			case "update":
			{
				setDirty();
				var obj = document.getElementById(elem.object);
				var text = getElementByName(obj, elem.reference);
				text.innerText = elem.newtext;
				if (elem.reference == "variable" || elem.reference == "tableau" || elem.reference == "champ" || elem.reference == "expression" || elem.reference == "size") {
					if (elem.reference == "champ")
					{
						var tabName = getElementByName(obj, "tableau").innerText;
						RenameLegende(tabName, elem.oldtext, tabName, elem.newtext);
					}
					else if (storage.reference == "tableau")
					{
						RenameLegende("", elem.oldtext, "", elem.newtext);
						var champName = getElementByName(obj, "champ").innerText;
						RenameLegende(elem.oldtext, champName, elem.newtext, champName)
					}
					else
					{
						RenameLegende("", elem.oldtext, "", elem.newtext);
					}
				}
				break;
			}
			case "popup":
			{
				// il n'y a remettre dans la popup
				--currentPositionStack;
				break;
			}
		}
	}
}

function clearUndo()
{
	currentPositionStack = 0;
	if (currentPositionStack != undoStack.childNodes.length)
	{
		while(undoStack.childNodes.length > currentPositionStack)
			undoStack.removeChild(undoStack.lastChild);
	}	
}

function hasUndoInPopup()
{
	if (currentPositionStack == 0)
	{
		return false;
	}
	else if (undoStack.childNodes[currentPositionStack - 1].action == "popup")
	{
		return false;
	}
	else
	{
		return true;
	}
}

function clearUndoUntilPopup()
{
	while(currentPositionStack >= 0)
	{
		if (undoStack.childNodes[--currentPositionStack].action == "popup")
		{
			while(undoStack.childNodes.length > currentPositionStack)
				undoStack.removeChild(undoStack.lastChild);
			break;
		}
	}
}

function getOldName()
{
	return storage.texte;
}