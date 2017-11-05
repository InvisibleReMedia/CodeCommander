
function refreshGroups()
{
	var obj = document.getElementById("groups");
	var locales = new ActiveXObject("o2Mate.LocaleGroup");
	var groups = locales.Groups;
	var tab = groups.split(" ");
	obj.innerHTML = "";
	for(index = 0; index < tab.length; ++index)
	{
		var option = new Option();
		option.value = tab[index];
		option.text = tab[index];
		obj.options.add(option);
	}
	reload(obj);
}

function reload(obj)
{
	var currentGroup = obj.options[obj.selectedIndex].value;
	var locales = new ActiveXObject("o2Mate.LocaleGroup");
	var group = locales.Get(currentGroup);
	var n = group.Names;
	var disp = document.getElementById("displayed");
	var selectedLanguage = disp.options[disp.selectedIndex].value;
	var dict = new ActiveXObject("o2Mate.Dictionnaire");
	var arr = new ActiveXObject("o2Mate.Array");
	for(index = 0; index < n.Count; ++index)
	{
		if (selectedLanguage)
		{
			try
			{
				var value = group.Get(n.Item(index).Name, selectedLanguage);
				var fields = new ActiveXObject("o2Mate.Fields");
				fields.AddString("name", n.Item(index).Name);
				fields.AddString("language", selectedLanguage);
				fields.AddString("value", value);
				arr.Add(fields);
			} catch(err) { }
		}
		else
		{
			var list = group.GetValues(n.Item(index).Name);
			for(langIndex = 0; langIndex < list.Count; ++langIndex)
			{
				var fields = new ActiveXObject("o2Mate.Fields");
				var le = list.Item(langIndex);
				fields.AddString("name", le.Name);
				fields.AddString("language", le.Language);
				fields.AddString("value", le.Value);
				arr.Add(fields);
			}
		}
	}
	dict.AddArray("tab", arr);
	document.getElementById("list").innerHTML = CreateLocaleTab(dict);
}

function changeValue(area, lang, name)
{
	var obj = document.getElementById("groups");
	var currentGroup = obj.options[obj.selectedIndex].value;
	var locales = new ActiveXObject("o2Mate.LocaleGroup");
	var group = locales.Get(currentGroup);
	if (group.ExistsOne(document.getElementById(name).innerText, document.getElementById(lang).innerText))
	{
		try
		{
			group.Modify(document.getElementById(name).innerText, document.getElementById(lang).innerText, document.getElementById(area).innerText);
		}
		catch(err)
		{
			alert(err.description);
		}
	}
}

function changeName(area, lang, name)
{
	var obj = document.getElementById("groups");
	var currentGroup = obj.options[obj.selectedIndex].value;
	var locales = new ActiveXObject("o2Mate.LocaleGroup");
	var group = locales.Get(currentGroup);
	var nameValue = document.getElementById(name).innerText;
	var oldNameValue = document.getElementById(name).oldName;
	if (nameValue)
	{
		if (oldNameValue)
		{
			if (oldNameValue != nameValue)
			{
				try
				{
					group.RenameOne(oldNameValue, nameValue, document.getElementById(lang).innerText);
				}
				catch(err)
				{
					alert(err.description);
				}
			}
		}
		else
		{
			if (!group.ExistsOne(nameValue, document.getElementById(lang).innerText))
			{
				try
				{
					group.Add(nameValue, document.getElementById(lang).innerText, document.getElementById(area).innerText);
				}
				catch(err)
				{
					alert(err.description);
				}
			}
		}
		document.getElementById(name).oldName = nameValue;
	}
}



function addRow()
{
	var tbl = document.getElementById("tabData");
	var current = document.getElementById("current");
	var currentLanguage = current.options[current.selectedIndex].value;
	var row = tbl.insertRow(tbl.rows.length - 1);
	row.height = "34px";
	var pos = tbl.rows.length - 1;
	var cell = row.insertCell(0);
	cell.width = "11px";
	cell.align = "center";
	cell.innerHTML = "<img align='center' style='cursor:pointer' valign='middle' src='sup.jpg' height='11px' width='11px' onclick='javascript:deleteRow(" + pos + ");'/>";
	cell = row.insertCell(1);
	cell.width = "100px";
	cell.align = "left";
	cell.innerHTML = "<div valign='top' style='background-color:white;color:green;font-weight:bold;height:34px;width:100%' contenteditable='true' id='nameValue" + pos + "' onfocusout='javascript:changeName(\"areaValue" + pos + "\",\"languageValue" + pos + "\",\"nameValue" + pos + "\");' oldName=''></div>";
	cell = row.insertCell(2);
	cell.width = "50px";
	cell.align = "left";
	cell.innerHTML = "<span valign='top' style='font-weight:bold;height:100%' id='languageValue" + pos + "'>" + currentLanguage + "</span>";
	cell = row.insertCell(3);
	cell.align = "left";
	cell.innerHTML = "<div valign='top' style='background-color:white;color:green;font-weight:bold;height:34px;width:100%' contenteditable='true' id='areaValue" + pos + "' onfocusout='javascript:changeValue(\"areaValue" + pos + "\",\"languageValue" + pos + "\",\"nameValue" + pos + "\");'></div>";
	document.getElementById("nameValue" + pos).focus();
}

function deleteRow(number)
{
	var obj = document.getElementById("groups");
	var currentGroup = obj.options[obj.selectedIndex].value;
	var locales = new ActiveXObject("o2Mate.LocaleGroup");
	var group = locales.Get(currentGroup);
	var name = document.getElementById("nameValue" + number.toString()).innerText;
	var lang = document.getElementById("languageValue" + number.toString()).innerText;
	group.DeleteOne(name, lang);
	var tbl = document.getElementById("tabData");
	for(var index = 0; index < tbl.rows.length - 1; ++index)
	{
		if (tbl.rows[index].cells.item(1).firstChild.id.substring(9) == number)
		{
			tbl.deleteRow(index);
			break;
		}
	}
}

function keyTab()
{
	if (window.event.srcElement.tagName == "BODY" && window.event.keyCode == 9)
	{
		addRow();
	}
}
