/*
	Put Your Custome Script From Here
*/
$(document).ready(function(e) {
	$('#close').click(function(e) {
		$('.heading_message').slideUp(400);
	});
	$('#get').click(function(e) {
		//$('#form_wraper').css('display','none');
		$('.form_message').css('display','block');
    });
});
function goTo(href) {
    window.location.href = href;
}