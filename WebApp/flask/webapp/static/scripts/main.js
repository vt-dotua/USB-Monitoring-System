$(document).ready(function(){
    $('.spoiler-search-body').css({'display':'none'});
    $('.add-user').css({'display':'none'});   

    $('.spoiler-zagolovok-search').click(function(){
        $(this).next('.spoiler-search-body').slideToggle(500)});
    
    $('.spoiler-zagolovok-manage-add').click(function(){
        $(this).next('.add-user').slideToggle(500)});
    
    $('.spoiler-zagolovok-manage-delete').click(function(){
            $(this).next('.remove-user').slideToggle(500)});

    var ua = navigator.userAgent;    
    if (ua.search(/Chrome/) > 0){
        var v = $(".wDate");
        v.removeClass("wDate")
        v.addClass("wDate-chrome");
        v = $(".wTime");
        v.removeClass("wTime")
        v.addClass("wTime-chrome");
        v = $(".wPidVid");
        v.removeClass("wPidVid")
        v.addClass("wPidVid-chrome");
        v = $(".wSN");
        v.removeClass("wSN")
        v.addClass("wSN-chrome");
    };

    if (ua.search(/Firefox/) > 0){
        var v = $(".wDate");
        v.removeClass("wDate")
        v.addClass("wDate-Firefox");
        v = $(".wTime");
        v.removeClass("wTime")
        v.addClass("wTime-Firefox");
        v = $(".wPidVid");
        v.removeClass("wPidVid")
        v.addClass("wPidVid-Firefox");
        v = $(".wSN");
        v.removeClass("wSN")
        v.addClass("wSN-Firefox");
    };

    if (ua.search(/Opera/) > 0){
        alert('Opera')
    };
    
    $('#formSearch').on('submit', function(event){
    
        var DateFrom, DateTo, TimeFrom, TimeTo;
        DateFrom = $('#dFrom').val();
        DateTo   = $('#dTo').val();
        TimeFrom = $('#tFrom').val();
        TimeTo   = $('#tTo').val();
        var DateIsNull, TimeIsNull = false;
        var DateErro, TimeErro = false;

        if(DateFrom ==="" || DateTo ==="")
            DateIsNull = true;

        if(TimeFrom ==="" || TimeTo ==="")
            TimeIsNull = true;

        if(!DateIsNull){
            DateFrom = Date.parse(DateFrom);
            DateTo   = Date.parse(DateTo);
            if(DateFrom > DateTo)
                DateErro = true;
        }

        if(!TimeIsNull){
            TimeFrom = moment(TimeFrom, 'HH:mm');
            TimeTo  = moment(TimeTo, 'HH:mm');
            if(TimeFrom > TimeTo)
                TimeErro = true;
        }

        if(DateErro){
            $("#formSearch").append("<div class='erroDateInpute'>Дата з повина бути меншою дати до!</li>");
            return false;
        }

        $form.find('.error').remove();
        
    });

    $('.admin').click(function(){
        var countСhecked = 0;
        var amount = $(".admin").length;
        $('.admin').each(function(i,elem) {
        
            if ($(elem).is(':checked')){
                countСhecked++;
            } 
        });
        if((countСhecked - amount) === 0){
            $(this).prop('checked', false);
            alert("Всіх адміністраторів видалити не можна!")
        }

    });
    
});