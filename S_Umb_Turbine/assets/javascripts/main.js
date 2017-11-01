$(document).ready(function () {

    $('#booking-create-modal').on('click', function () {
        $('#create-booking-modal').modal('show');
    })

    $('#update-delete-booking').on('click', function () {
        $('#delete-view-booking-modal').modal('show');
    });

    $('.list-group-item-room-filter').on('click', function (event) {
        $('.list-group-item-room-filter').removeClass('active');
        $(event.target).addClass('active');
        $('#overview-conference-room').val($(event.target).data('conference-room-id'));
        $(event.target).closest("form").submit();
    });

    $('#list-group-item-show-bookings-only').on('click', function (event) {
        $('#list-group-item-show-bookings-only').toggleClass('active');
        $('#overview-show-only-current-bookings').val($(event.target).hasClass('active') ? 'on' : '');
        $(event.target).closest("form").submit();
    });

    $('.list-group-item-room-create').on('click', function (event) {
        $('.list-group-item-room-create').removeClass('active');
        $(event.target).addClass('active');
        $('#create-conference-room').val($(event.target).data('conference-room-id'));
    });

    $('.list-group-item-room-update').on('click', function (event) {
        $('.list-group-item-room-update').removeClass('active');
        $(event.target).addClass('active');
        $('#update-conference-room').val($(event.target).data('conference-room-id'));
    });

    $('#logout-button').on('click', function (event) {
        $(event.target).closest("form").submit();
    })
    
    moment.locale('da', {
        week: { dow: 1 } // Monday is the first day of the week
    });

    moment.locale('da');

    $.fullCalendar.locale("da", {
        buttonText: {
            month: "Måned",
            week: "Uge",
            day: "Dag",
            list: "Liste",
            today: "idag"
        },
        allDayText: "Hele dagen",
        eventLimitText: "flere",
        noEventsMessage: "Ingen arrangementer at vise"
    });

    $('#calendar').fullCalendar({
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'month,agendaWeek,listMonth'
        },
        eventAfterAllRender: function (view, element) { $('.fc-time-grid-container').css({ height: '100%' }); },
        weekends: false,
        defaultView: 'agendaWeek',
        timeFormat: 'H:mm',
        slotLabelFormat: 'H:mm',
        slotDuration: '00:15:00',
        minTime: '08:00:00',
        maxTime: '18:00:00',
        allDaySlot: false,
        allDayText: '',
        locale: 'da',
        viewRender: function () {
            var postData = [];
            console.log(postData);
            var showOnlyCurrentUsersBookings = $('#overview-show-only-current-bookings').val();
            var conferenceRoomId = $('#overview-conference-room').val();
            var memberId = $('#overview-member-id').val();
            var formURL = '/api/BookingHandler.ashx?action=findBookingsByConferenceRoomOrMember&overview-conference-room=' + conferenceRoomId + '&overview-show-only-current-bookings=' + showOnlyCurrentUsersBookings + '&overview-member-id=' + memberId;
            $.ajax({
                url: formURL,
                type: "POST",
                data: postData,
                success: function (data, textStatus, jqXHR) {
                    console.log(data);
                    $('#calendar').fullCalendar('removeEvents');
                    $('#calendar').fullCalendar('addEventSource', JSON.parse(data));
                    $('#calendar').fullCalendar('rerenderEvents');
                },
                error: function (jqXHR, textStatus, errorThrown) {

                }
            });
        },
        eventClick: function (calEvent, jsEvent, view) {
            
            console.log(calEvent);
            if ($.inArray("booked-by-other", calEvent.className)) {
                $('#update-title').val(calEvent.title);
                $('#update-conference-room').val(calEvent.conferenceRoomId);
                $('#update-booked-by').val(calEvent.bookedByName);
                $('#update-start-date').val(calEvent.start.format('DD-MM-YYYY HH:mm'));
                $('#update-end-date').val(calEvent.end.format('DD-MM-YYYY HH:mm'));
                $('#update-booking-id').val(calEvent.id);
                $('.list-group-item-room-update').each(function (i, e) {
                    item = $(e)
                    if (item.data('conference-room-id') == calEvent.conferenceRoomId) {
                        item.addClass('active');
                    } else {
                        item.removeClass('active');
                    }
                });
                $('#update-view-booking-modal').modal('show');

                $('#delete-booking-id').val(calEvent.id);
            } else {
                $('#detail-title').val(calEvent.title);
                $('#detail-conference-room').val(calEvent.conferenceRoomName);
                $('#detail-booked-by').val(calEvent.bookedByName);
                $('#detail-start-date').val(calEvent.start.format('DD-MM-YYYY HH:mm'));
                $('#detail-end-date').val(calEvent.end.format('DD-MM-YYYY HH:mm'));

                $('#detailed-view-booking-modal').modal('show');
            }
        }
    });

    //Disable scrolling on the calendar.
    $('.fc-time-grid-container').css({ height: '100%' });

    $('.datetimepicker').datetimepicker({
        format: 'DD-MM-YYYY HH:mm',
        stepping: 15,
        ignoreReadonly: true,
        daysOfWeekDisabled: [0, 6],
        enabledHours: [8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18],
        locale: 'da'
    });

    $('.datetimepicker-readonly').datetimepicker({
        format: 'DD-MM-YYYY HH:mm',
        stepping: 15
    });

    $('#update-datetimepicker').datetimepicker();
    $('#update-datetimepicker').on('dp.change', function (event) {
        console.log('changing...');
    });

    var dateTimePickerIds = ['create','update']

    for (var i in dateTimePickerIds) {
        $('#' + dateTimePickerIds[i] + '-start-datetimepicker').on('dp.change', function (event) {
            var startTime = event.date;
            var $endTimeObject = $('#' + dateTimePickerIds[i] + '-end-datetimepicker');
            var endTimeDateTimePicker = $('#' + dateTimePickerIds[i] + '-end-datetimepicker').data('DateTimePicker');
            if (endTimeDateTimePicker.date().isBefore(startTime)) {
                $endTimeObject.data('DateTimePicker').date(moment(startTime.format()).add(15, 'minutes'));
            }
        });

        $('#' + dateTimePickerIds[i] + '-start-datetimepicker,' + '#' + dateTimePickerIds[i] + '-end-datetimepicker').on('dp.show', function (event) {
            $("a[data-action='togglePicker']").trigger('click');
        })
    }
    
    $('#overview-conference-room').on('change', function () {
        $("#overview-calendar-form").submit();
    });

    $('#overview-show-only-current-bookings').on('change', function () {
        $("#overview-calendar-form").submit();
    });
    

    $('#overview-calendar-form').on('submit', function (event) {
        event.preventDefault();
        event.stopPropagation();

        var postData = $(this).serializeArray();
        var formURL = $(this).attr("action");
        $.ajax({
            url: formURL,
            type: "POST",
            data: postData,
            success: function (data, textStatus, jqXHR) {
                console.log(data);
                $('#calendar').fullCalendar('removeEvents');
                $('#calendar').fullCalendar('addEventSource', JSON.parse(data));
                $('#calendar').fullCalendar('rerenderEvents');
            },
            error: function (jqXHR, textStatus, errorThrown) {

            }
        });
    });

    $('#create-booking-form').on('submit', function (event) {
        event.preventDefault();
        event.stopPropagation();

        var postData = $(this).serializeArray();
        var formURL = $(this).attr("action");

        $.ajax({
            url: "/api/BookingHandler.ashx?action=validateTime",
            type: "POST",
            data: postData,
            success: function (data, textStatus, jqXHR) {
                var parsedData = JSON.parse(data);
                if (parsedData.message == 'OPEN') {
                    $.ajax({
                        url: formURL,
                        type: "POST",
                        data: postData,
                        success: function (data, textStatus, jqXHR) {
                            var parsedData2 = JSON.parse(data);
                            $('#calendar').fullCalendar('renderEvent', parsedData2.booking);
                            $('#create-booking-modal').modal('hide');
                        },
                        error: function (jqXHR, textStatus, errorThrown) {

                        }
                    });
                } else {
                    console.log('Slot is reserved. Book at another time.');
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                
            }
        });
    });

    $('#update-booking-form').on('submit', function (event) {
        event.preventDefault();
        event.stopPropagation();

        var postData = $(this).serializeArray();
        var formURL = $(this).attr("action");
        $.ajax({
            url: "/api/BookingHandler.ashx?action=validateTime",
            type: "POST",
            data: postData,
            success: function (data, textStatus, jqXHR) {
                var parsedData = JSON.parse(data);
                if (parsedData.message == 'OPEN'){
                    $.ajax({
                        url: formURL,
                        type: "POST",
                        data: postData,
                        success: function (data, textStatus, jqXHR) {
                            var parsedData2 = JSON.parse(data);
                            console.log(parsedData2);
                            $('#calendar').fullCalendar('removeEvents', [parsedData2.booking.id]);
                            $('#calendar').fullCalendar('renderEvent', parsedData2.booking);
                            $('#update-view-booking-modal').modal('hide');
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            console.log("Error...");
                        }
                    });
                } else {
                    console.log('Slot is reserved. Book at another time.');
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                
            }
        });
        
    });

    $('#delete-booking-form').on('submit', function (event) {
        event.preventDefault();
        event.stopPropagation();

        var postData = $(this).serializeArray();
        var formURL = $(this).attr("action");
        $.ajax({
            url: formURL,
            type: "POST",
            data: postData,
            success: function (data, textStatus, jqXHR) {
                var parsedData = JSON.parse(data);
                $('#calendar').fullCalendar('removeEvents', [parsedData.message]);
                $('#update-view-booking-modal').modal('hide');
                $('#delete-view-booking-modal').modal('hide');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                
            }
        });
    });

    $('#change-password-form').on('submit', function (event) {
        event.preventDefault();
        event.stopPropagation();
        
        var postData = $(this).serializeArray();
        var formURL = $(this).attr("action");
        $.ajax({
            url: formURL,
            type: "POST",
            data: postData,
            success: function (data, textStatus, jqXHR) {
                console.log(data);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                
            }
        });

    })

});