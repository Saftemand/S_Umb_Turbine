$(document).ready(function () {

    $('#booking-create-modal').on('click', function () {
        $('#create-start-date-row .datetimepicker').data('DateTimePicker').date(moment());
        $('#create-end-date-row .datetimepicker').data('DateTimePicker').date(moment().add(15,'minutes'));
        $('#create-booking-modal').modal('show');
    })

    $('#update-delete-booking').on('click', function () {
        $('#delete-view-booking-modal').modal('show');
    });

    $('.list-group-item-room-filter').on('click', function (event) {
        $('.list-group-item-room-filter').removeClass('active');
        $(event.target).addClass('active');
        $('#overview-conference-room').val($(event.target).data('conference-room-id'));
        selectCreateConferenceRoom($(event.target).data('conference-room-id'));
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

    $('#create-start-date, #create-end-date, #update-start-date, #update-end-date').on('click', function (event) {
        console.log(event);
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
                $('#update-start-date-row .datetimepicker').data("DateTimePicker").date(calEvent.start.format('DD-MM-YYYY HH:mm'));
                $('#update-end-date').val(calEvent.end.format('DD-MM-YYYY HH:mm'));
                $('#update-end-date-row .datetimepicker').data("DateTimePicker").date(calEvent.end.format('DD-MM-YYYY HH:mm'));
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
        if (!validateCreateForm(postData, 'create')) {
            return;
        }
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
                            if (parsedData2.status == "SUCCESS") {
                                $('#create-success-alert').fadeIn().delay(5000).fadeOut();
                                $('#calendar').fullCalendar('renderEvent', parsedData2.booking);
                                $('#create-booking-modal').modal('hide');
                            } else if (parsedData2.status == "ERROR") {
                                $('#error-dates-alert').fadeIn().delay(5000).fadeOut();
                            } else if (parsedData2.status == "EXCEPTION"){
                                $('#exception-alert').fadeIn().delay(5000).fadeOut();
                            }
                        },
                        error: function (jqXHR, textStatus, errorThrown) {

                        }
                    });
                } else {
                    $('#error-collision-alert').fadeIn().delay(5000).fadeOut();
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

        if (!validateCreateForm(postData, 'update')) {
            return;
        }

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
                            if (parsedData2.status == "SUCCESS") {
                                $('#update-success-alert').fadeIn().delay(5000).fadeOut();
                                $('#calendar').fullCalendar('removeEvents', [parsedData2.booking.id]);
                                $('#calendar').fullCalendar('renderEvent', parsedData2.booking);
                                $('#update-view-booking-modal').modal('hide');
                            } else if (parsedData2.status == "ERROR"){
                                $('#error-dates-alert').fadeIn().delay(5000).fadeOut();
                            } else if (parsedData2.status == "EXCEPTION") {
                                $('#exception-alert').fadeIn().delay(5000).fadeOut();
                            }
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            console.log("Error...");
                        }
                    });
                } else {
                    $('#error-collision-alert').fadeIn().delay(5000).fadeOut();
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
                if (parsedData.status == "SUCCESS"){
                    $('#delete-success-alert').fadeIn().delay(5000).fadeOut();
                    $('#calendar').fullCalendar('removeEvents', [parsedData.message]);
                    $('#update-view-booking-modal').modal('hide');
                    $('#delete-view-booking-modal').modal('hide');
                } else if (parsedData.status == "EXCEPTION"){
                    $('#exception-alert').fadeIn().delay(5000).fadeOut();
                }
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
                var parsedData = JSON.parse(data);
                if (parsedData.status == "SUCCESS") {
                    $('#Password').val("");
                    $('#PasswordRepeat').val("");
                    $('#success-alert').fadeIn().delay(5000).fadeOut();
                } else if (parsedData.status == "ERROR"){
                    if (parsedData.message == "NOT LOGGED IN"){
                        $('#error-not-logged-in-alert').fadeIn().delay(5000).fadeOut();
                    } else if (parsedData.message == "NOT IDENTICAL OR EMPTY"){
                        $('#error-unidentical-alert').fadeIn().delay(5000).fadeOut();
                    } else if (parsedData.message == "NOT ENOUGH CHARACTERS") {
                        $('#error-invalid-alert').fadeIn().delay(5000).fadeOut();
                    }
                } else if (parsedData.status == "EXCEPTION") {
                    $('#exception-alert').fadeIn().delay(5000).fadeOut();
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {

            }
        });

    })

    var validateCreateForm = function (serializedForm, context) {
        var validated = true;
        var formObject = {}

        for (var i in serializedForm) {
            formObject[serializedForm[i].name] = serializedForm[i].value;
        }

        if (formObject[context + '-conference-room'] == '') {
            validated = false;
            $('#' + context + '-conference-room-row').addClass('has-danger');
            $('#' + context + '-conference-room-feedback').show();
        } else {
            $('#' + context + '-conference-room-row').removeClass('has-danger');
            $('#' + context + '-conference-room-feedback').hide();
        }

        var startDate = formObject[context + '-start-date'];
        var endDate = formObject[context + '-end-date'];

        //check if startDate comes before endDate
        if (!moment(startDate, 'DD-MM-YYYY HH:mm').isBefore(moment(endDate, 'DD-MM-YYYY HH:mm'))) {
            validated = false;
            $('#' + context + '-end-date-row').addClass('has-danger');
            $('#' + context + '-end-date-feedback').show();
        } else {
            $('#' + context + '-end-date-row').removeClass('has-danger');
            $('#' + context + '-end-date-feedback').hide();
        }


        return validated;
    };

    var dateTimePickerIds = ['create', 'update']

    dateTimePickerIds.forEach(function (item, index) {
        var startDateTimePicker = $('#' + item + '-start-date-row .datetimepicker');
        startDateTimePicker.on('dp.change', function (event) {
            var startTime = event.date;
            var $endTimeObject = $('#' + item + '-end-date-row .datetimepicker');
            var endTimeDateTimePicker = $endTimeObject.data('DateTimePicker');
            if (endTimeDateTimePicker.date().isBefore(startTime)) {
                $endTimeObject.data('DateTimePicker').date(moment(startTime.format()).add(15, 'minutes'));
            }
        });

        var endDateTimePicker = $('#' + item + '-end-date-row .datetimepicker');
        endDateTimePicker.on('dp.change', function (event) {
            var endTime = event.date;
            var $startTimeObject = $('#' + item + '-start-date-row .datetimepicker');
            var startTimeDateTimePicker = $startTimeObject.data('DateTimePicker');
            if (startTimeDateTimePicker.date().isAfter(endTime)) {
                $startTimeObject.data('DateTimePicker').date(moment(endTime.format()).subtract(15, 'minutes'));
            }
        });

        $('#' + item + '-start-date-row .datetimepicker, ' + '#' + item + '-end-date-row .datetimepicker').on('dp.show', function (event) {
            $("a[data-action='togglePicker']").trigger('click');
        })
    })

    var selectCreateConferenceRoom = function (roomId) {
        var rooms = $('.list-group-item-room-create');
        rooms.removeClass('active');
        var room = rooms.filter("[data-conference-room-id=" + roomId + "]");
        room.addClass('active');
        
        $('#create-conference-room').val(roomId);
    }

    selectCreateConferenceRoom($($('.list-group-item-room-create')[0]).data('conference-room-id'));
    
});