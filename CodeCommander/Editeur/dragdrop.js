var dragged, mousex, mousey, coordinates = [], sequenceLine = [];
$.event.props.push('dataTransfer');

var continueDragging = function(e) {

    // Change the location of the draggable object
    dragged.css({
        "left": e.pageX - (dragged.offset().left + dragged.width() / 2),
        "top": e.pageY - (dragged.offset().top + dragged.height() / 2)
    });
    
    // Check if we hit any boxes
    var drop;
    for (var i in coordinates) {
		if ((mousex >= coordinates[i].left && mousex <= coordinates[i].right) && (mousey >= coordinates[i].top && mousey <= coordinates[i].bottom)) {
			// Yes, the mouse is on a droppable area
			// Lets change the background color
			coordinates[i].dom.addClass("somethingover");
			coordinates[i].dom.trigger("mydrag", [ jQuery.Event('dataTransfer') ]);
        } else {
            // Nope, we did not hit any objects yet
            coordinates[i].dom.removeClass("somethingover");
            dragged.show();
        }
    }
    
    // create sequence of a line
    var adding;
    for(var point in sequenceLine) {
		if (sequenceLine[point].left < mousex || sequenceLine[point].top > mousey &&
			Math.abs(sequenceLine[point].left - mousex) > 10 && Math.abs(sequenceLine[point].top - mousey) > 10) {
			adding = true;
		}
    }
    if (adding && sequenceLine.length <= 20) {
		sequenceLine.push({
			left:mousex,
			top:mousey
		});
    }

    // Keep the last positions of the mouse coord.s
    mousex = e.pageX;
    mousey = e.pageY;
}

function backToOldPlace(index) {
	if (index >= 5) {
		dragged.css({
			"left": sequenceLine[index].left - (dragged.offset().left + dragged.width() / 2),
			"top": sequenceLine[index].top - (dragged.offset().top + dragged.height() / 2)
		});
		setTimeout('backToOldPlace(' + (index-1).toString() + ')', 100);
	} else {
		// replace to initial
		dragged.css({
			"left":"auto",
			"top":"auto",
			"position":"relative"
		});
		// Reset variables
		mousex = 0;
		mousey = 0;
		dragged = null;
		coordinates = [];
		sequenceLine = [];
	}
}

var endDragging = function(e) {
    // Remove document event listeners
    $(document).unbind("mousemove", continueDragging);
    $(document).unbind("mouseup", endDragging);

    // Check if we hit any boxes
    var droptarget;
    for (var i in coordinates) {
		droptarget = coordinates[i].dom;
		droptarget.removeClass("somethingover");
    }
    if (!droptarget)
    {
		backToOldPlace(20);
    }
}


var startDragging = function(e) {

    // Find coordinates of the droppable bounding boxes
    $(".droppable").each(function() {
		$(this).on("mydrag", function(e, d) {
			alert(typeof(d));
			d.setData("text/plain", "test");
		});
        var lefttop = $(this).offset();
        // and save them in a container for later access
        coordinates.push({
            dom: $(this),
            left: lefttop.left,
            top: lefttop.top,
            right: $(this).width(),
            bottom: $(this).height()
        });
    });

    // When the mouse down event is received
    if (e.type == "mousedown") {
        dragged = $(this);
        	    
        // Change the position of the draggable
        var offset = dragged.offset();
        dragged.css({
            "left": e.pageX - (offset.left + dragged.width() / 2),
            "top": e.pageY - (offset.top + dragged.height() / 2),
            "position": "absolute"
        });
        sequenceLine.push({
			"left": e.pageX - (offset.left + dragged.width() / 2),
			"top": e.pageY - (offset.top + dragged.height() / 2)
        });
        // Bind the events for dragging and stopping
        $(document).bind("mousemove", continueDragging);
        $(document).bind("mouseup", endDragging);
    }
}

var obj;
function initDragAndDrop()
{
	// Start the dragging
	$(".draggable").bind("mousedown", startDragging);
	obj = jQuery.Event('dataTransfer');
}