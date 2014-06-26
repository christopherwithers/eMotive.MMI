var UserActions = (function () {
    var fetchEmailLog = function (_username, _forename, _surname) {
        Ajax.DoQuery(window.Routes.URL("FetchEmailSentLog"),
            function (data) {
                var html;
                $.each(data.results, function (key, value) {
                    html += "<tr><td>" + value.EmailKey + "</td><td>" + moment(value.DateSent).format("dddd, MMMM Do YYYY, h:mm:ss a") + "</td></tr>";
                });
                $("#EmailLogMessage").text("Email log for " + _forename + " " + _surname + " (" + _username + ").");
                $("#EmailLogTable > tbody").empty();
                $("#EmailLogTable > tbody:last").append(html);
                $("#EmailLog").modal('toggle');
            },
            { username: _username });
    };
    var viewInterviewSessions = function (_username, _forename, _surname) {
        Ajax.DoQuery(window.Routes.URL("FetchApplicantSignups"),
            function (data) {
                if (data.success == "False") {
                    Ajax.DisplayError(data.message, "Error");
                } else {
                    if (data.results.HasSignedUp == false) {
                        $("#SessionlogInformation").text(_forename + " " + _surname + " (" + _username + ") has not signed up to any interview dates.");
                    } else {
                        $("#SessionlogInformation").text(moment(data.results.SignUpDate).format("dddd, MMMM Do YYYY") + " at " + data.results.SignUpDetails);
                    }
                }
            }, { username: _username });

        $("#SessionLog").modal('toggle');
    };

    var resendAccountCreationEmail = function (_username) {
        Ajax.DoQuery(window.Routes.URL("ResendAccountCreationEmail"),
            function (data) {
                if (data.success == "False") {
                    Ajax.DisplayError("The account creation email was not sent", "Error");
                }
            }, { username: _username });
    };

    var sendAttendanceCertificate = function (username) {
        /*  Ajax.DoQuery(window.Routes.URL("SessionAttendanceCertificate"),
        function (data) {
            if (data.success == "False") {
                Ajax.DisplayError(data.message, "Error");
            } else {

            }
        }, { username: username });*/
        $('#TrainingCertificateError').hide();
        Ajax.DoQuery(window.Routes.URL("FetchSCETrainingInformation"),
            function (data) {
                if (data.success) {
                    var list = $("#TrainingDatelists");
                    $.each(data.results, function (i, item) {
                        list.append($("<option />").val(item.ID).text(item.Date));
                    });
                    $("#CertificateForUsername").val(username);
                    $("#TrainingCertificateModal").modal({ show: true });



                } else {
                    $('#TrainingCertificateError').html(data.message);
                }
            }, { username: username });
    };

    var deleteRecord = function (username) {
        $("#DeleteID").val(username);
        $("#DeleteMessage").html("Are you sure you wish to delete the account of '" + username + "'?");
        $('#DeleteError').hide();
        $("#DeleteModal").modal({ show: true });
    };

    var fetchApplicantData = function (_username) {

        Ajax.DoQuery(window.Routes.URL("FetchApplicantData"),
            function (data) {
                if (data.success == "False") {
                    Ajax.DisplayError(data.message, "Error");
                } else {

                }
            }, { username: _username });
    };

    var fetchNotes = function (_username) {
        $('#NotesError').hide();
        $('#NotesText').html("");
        $("#NotesUsernameTitle").html("Notes for " + _username);
        $("#NotesUsername").val(_username);

        Ajax.DoQuery(window.Routes.URL("FetchUserNotes"), function (data) { $('#NotesText').html(data.results); }, { username: _username });
        $("#NotesModal").modal({ show: true });
    };

    return {
        FetchEmailLog: fetchEmailLog,
        FetchSessionLog: viewInterviewSessions,
        ResendAccountCreationEmail: resendAccountCreationEmail,
        DeleteRecord: deleteRecord,
        FetchApplicantData: fetchApplicantData,
        FetchNotes: fetchNotes,
        SendAttendanceCertificate: sendAttendanceCertificate
    };
})();