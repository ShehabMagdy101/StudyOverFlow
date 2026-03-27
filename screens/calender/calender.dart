import 'dart:core';

import 'package:flutter/material.dart';
import 'package:home_function/constants/Colors.dart';
import 'package:home_function/constants/calendar_settings.dart';
import 'package:home_function/models/meeting_data_source.dart';
import 'package:home_function/screens/calender/widgets/add_appointment_dialog.dart';
import 'package:intl/intl.dart';
import 'package:syncfusion_flutter_calendar/calendar.dart';

class MyHomePage extends StatefulWidget {
  const MyHomePage({super.key});

  @override
  State<MyHomePage> createState() => _MyHomePageState();
}

class _MyHomePageState extends State<MyHomePage> {
  final List<Appointment> _appointments = [
    Appointment(
      startTime: DateTime.now(),
      endTime: DateTime.now().add(Duration(hours: 2)),
      subject: 'Test Event',
      notes:
          "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
      color: electricBlue,
    ),
    Appointment(
      startTime: DateTime.now().add(Duration(hours: 2)),
      endTime: DateTime.now().add(Duration(hours: 4)),
      subject: 'Test Event 2',
      notes:
          "There are many variations of passages of Lorem Ipsum available, but the majority have suffered alteration in some form, by injected humour, or randomised words which don't look even slightly believable. If you are going There are many variations of passages of Lorem Ipsum available, but the majority have suffered alteration in some form, by injected humour, or randomised words which don't look even slightly believable. If you are going to use a passage of Lorem Ipsum, you need to be sure there isn't anything embarrassing hidden in the middle of text. All the Lorem Ipsum generators on the Internet tend to repeat predefined chunks as necessary, making this the first true generator on the Internet. It uses a dictionary of over 200 Latin words, combined with a handful of model sentence structures, to generate Lorem Ipsum which looks reasonable. The generated Lorem Ipsum is therefore always free from repetition, injected humour, or non-characteristic words etc.to use a passage of Lorem Ipsum, you need to be sure there isn't anything embarrassing hidden in the middle of text. All the Lorem Ipsum generators on the Internet tend to repeat predefined chunks as necessary, making this the first true generator on the Internet. It uses a dictionary of over 200 Latin words, combined with a handful of model sentence structures, to generate Lorem Ipsum which looks reasonable. The generated Lorem Ipsum is therefore always free from repetition, injected humour, or non-characteristic words etc.",
      color: teaGreen,
      // recurrenceRule:
      //     'FREQ=WEEKLY;BYDAY=${getDayOfWeek(DateTime.now().weekday)};COUNT=4',
      //     recurrenceId: 2
    ),
  ];

  void _addAppointment(Appointment appointment) {
    setState(() {
      _appointments.add(appointment);
    });
  }

  void deleteEvent(Appointment appointment) {
    // If the event is part of a recurrence series
    if (appointment.recurrenceRule != null) {
      showDialog(
        context: context,
        builder: (BuildContext context) {
          return AlertDialog(
            title: Text('Delete Recurring Event'),
            content: Text(
                'Do you want to delete the entire series or just this occurrence?'),
            actions: [
              TextButton(
                onPressed: () {
                  // Delete the entire series
                  setState(() {
                    if (_appointments.isNotEmpty) {
                      _appointments.removeWhere((item) =>
                          item.recurrenceId != null &&
                          item.recurrenceId == appointment.recurrenceId);
                    }
                  });
                  Navigator.of(context).pop();
                  ScaffoldMessenger.of(context).showSnackBar(
                    SnackBar(
                        content: Text('Entire series deleted successfully.')),
                  );
                },
                child: Text('Entire Series'),
              ),
              TextButton(
                onPressed: () {
                  // Delete only this occurrence
                  setState(() {
                    _appointments.remove(appointment);
                  });
                  Navigator.of(context).pop();
                  ScaffoldMessenger.of(context).showSnackBar(
                    SnackBar(
                      content: Text('This occurrence deleted successfully.'),
                      backgroundColor: Colors.green,
                    ),
                  );
                },
                child: Text('This Occurrence'),
              ),
              TextButton(
                onPressed: () {
                  Navigator.of(context).pop(); // Close dialog
                },
                child: Text('Cancel'),
              ),
            ],
          );
        },
      );
    } else {
      // For non-recurring events, delete directly
      setState(() {
        if (_appointments.isNotEmpty) {
          _appointments.remove(appointment);
        }
      });
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Event deleted successfully.'),
          backgroundColor: Colors.green,
        ),
      );
    }
  }

  void showEditEventDialog(Appointment appointment) {
    final titleController = TextEditingController(text: appointment.subject);
    final durationController = TextEditingController(
        text: appointment.endTime
            .difference(appointment.startTime)
            .inMinutes
            .toString());
    final descriptionController =
        TextEditingController(text: appointment.notes);

    TimeOfDay selectedTime = TimeOfDay.fromDateTime(appointment.startTime);
    Color _selectedColor = appointment.color;

    showDialog(
      context: context,
      builder: (BuildContext context) {
        return AlertDialog(
          title: const Text('Edit Event'),
          content: SingleChildScrollView(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                TextField(
                  controller: titleController,
                  decoration: const InputDecoration(
                    labelText: 'Event Title',
                  ),
                ),
                const SizedBox(height: 10),
                TextField(
                  controller: descriptionController,
                  decoration: const InputDecoration(
                    labelText: 'Event Notes',
                  ),
                  maxLines: CalendarSettings.defaultSettings['maxLine'] as int,
                  minLines: CalendarSettings.defaultSettings['minLine'] as int,
                  textInputAction: TextInputAction.newline,
                  keyboardType: TextInputType.multiline,
                ),
                StatefulBuilder(
                  builder: (BuildContext context, StateSetter setState) {
                    return DropdownButton<Color>(
                      value: _selectedColor,
                      items: const [
                        DropdownMenuItem(
                          value: electricBlue,
                          child: Row(
                            children: [
                              Icon(Icons.circle, color: electricBlue),
                              SizedBox(width: 8),
                              Text('Electric Blue'),
                            ],
                          ),
                        ),
                        DropdownMenuItem(
                          value: teaGreen,
                          child: Row(
                            children: [
                              Icon(Icons.circle, color: teaGreen),
                              SizedBox(width: 8),
                              Text('Tea Green'),
                            ],
                          ),
                        ),
                        DropdownMenuItem(
                          value: cream,
                          child: Row(
                            children: [
                              Icon(Icons.circle, color: cream),
                              SizedBox(width: 8),
                              Text('Cream'),
                            ],
                          ),
                        ),
                        DropdownMenuItem(
                          value: sunset,
                          child: Row(
                            children: [
                              Icon(Icons.circle, color: sunset),
                              SizedBox(width: 8),
                              Text('Sunset'),
                            ],
                          ),
                        ),
                        DropdownMenuItem(
                          value: melon,
                          child: Row(
                            children: [
                              Icon(Icons.circle, color: melon),
                              SizedBox(width: 8),
                              Text('Melon'),
                            ],
                          ),
                        ),
                      ],
                      onChanged: (value) {
                        setState(() {
                          _selectedColor = value!;
                        });
                      },
                    );
                  },
                ),
              ],
            ),
          ),
          actions: [
            TextButton(
              onPressed: () {
                appointment.subject = titleController.text;
                appointment.startTime = DateTime(
                  appointment.startTime.year,
                  appointment.startTime.month,
                  appointment.startTime.day,
                  selectedTime.hour,
                  selectedTime.minute,
                );
                appointment.endTime = appointment.startTime.add(
                  Duration(minutes: int.parse(durationController.text)),
                );
                appointment.notes = descriptionController.text;
                appointment.color = _selectedColor;
                Navigator.of(context).pop(); // Close dialog
                setState(() {}); // Refresh UI
              },
              child: const Text('Save'),
            ),
            TextButton(
              onPressed: () {
                Navigator.of(context).pop();
              },
              child: const Text('Cancel'),
            ),
          ],
        );
      },
    );
  }

  void calendarTapped(CalendarTapDetails details) {
    if (details.targetElement == CalendarElement.appointment ||
        details.targetElement == CalendarElement.agenda) {
      final Appointment appointmentDetails = details.appointments![0];
      final String subject = appointmentDetails.subject;
      final String dateText = DateFormat('MMMM dd, yyyy')
          .format(appointmentDetails.startTime)
          .toString();
      final String startTimeText =
          DateFormat('hh:mm a').format(appointmentDetails.startTime).toString();
      final String endTimeText =
          DateFormat('hh:mm a').format(appointmentDetails.endTime).toString();
      final String timeDetails = '$startTimeText - $endTimeText';
      final String? description =
          appointmentDetails.notes ?? "No description available";

      showDialog(
        context: context,
        builder: (BuildContext context) {
          return AlertDialog(
            title: Text(subject),
            content: SingleChildScrollView(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                mainAxisSize: MainAxisSize.min,
                children: [
                  Text(
                    dateText,
                    style: TextStyle(
                      fontWeight: FontWeight.w400,
                      fontSize: 20,
                    ),
                  ),
                  const SizedBox(height: 8),
                  Text(
                    timeDetails,
                    style: TextStyle(
                      fontWeight: FontWeight.w400,
                      fontSize: 15,
                    ),
                  ),
                  const SizedBox(height: 8),
                  Text(
                    description!,
                    style: TextStyle(
                      fontWeight: FontWeight.w400,
                      fontSize: 15,
                    ),
                  ),
                ],
              ),
            ),
            actions: [
              TextButton(
                onPressed: () {
                  Navigator.of(context).pop(); // Close the current dialog
                  showEditEventDialog(appointmentDetails);
                },
                child: Text('Edit Event'),
              ),
              TextButton(
                onPressed: () {
                  // To delete event
                  deleteEvent(appointmentDetails);
                  Navigator.of(context).pop();
                  // Refresh the UI to reflect the deletion
                  setState(() {});
                },
                child: const Text(
                  "Delete Event",
                  style: TextStyle(color: Colors.red),
                ),
              ),
              TextButton(
                onPressed: () {
                  Navigator.of(context).pop();
                },
                child: const Text("Close"),
              ),
            ],
          );
        },
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        appBar: AppBar(
          backgroundColor: Colors.white,
          title: Text("My Calendar", style: TextStyle(
            fontWeight: FontWeight.bold,
            color: Colors.black,
            fontSize: 24,
          ),),
          actions: [
            IconButton(
              icon: Icon(Icons.add),
              onPressed: () async {
                final newAppointment = await showDialog<Appointment>(
                  context: context,
                  builder: (context) => AddAppointmentDialog(),
                );
                if (newAppointment != null) {
                  _addAppointment(newAppointment);
                }
              },
            ),
          ],
        ),
        body: SfCalendar(
          view: CalendarView.week,
          dataSource: MeetingDataSource(_appointments),
          onTap: calendarTapped,
          firstDayOfWeek:
              CalendarSettings.defaultSettings['firstDayOfWeek'] as int,
          headerHeight:
              CalendarSettings.defaultSettings['headerHeight'] as double,
          todayHighlightColor:
              CalendarSettings.defaultSettings['todayHighlightColor'] as Color,
          viewNavigationMode: CalendarSettings
              .defaultSettings['viewNavigationMode'] as ViewNavigationMode,
          showCurrentTimeIndicator: CalendarSettings
              .defaultSettings['showCurrentTimeIndicator'] as bool,
          headerStyle: CalendarSettings.defaultSettings['headerStyle']
              as CalendarHeaderStyle,
          selectionDecoration: CalendarSettings
              .defaultSettings['selectionDecoration'] as BoxDecoration,
          appointmentTextStyle: CalendarSettings
              .defaultSettings['appointmentTextStyle'] as TextStyle,
          timeSlotViewSettings: CalendarSettings
              .defaultSettings['timeSlotViewSettings'] as TimeSlotViewSettings,
        ));
  }
}
