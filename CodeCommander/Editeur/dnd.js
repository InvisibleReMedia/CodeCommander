
function initDragAndDrop(type, name)
{
		$('.' + type + 'Draggable').each(function(index) {
			
			$(this).draggable = true;
			$(this).on({
			
				mousedown: function(e) {
					callback.action = "initDnD";
					callback.repositoryType = type;
					callback.target = name + $(this).attr('index');
					callback.click();
				},
				
				selectstart: function(e) {
					e.preventDefault();
					return false;
				}
			});
		});
}