import 'dart:core';

import 'package:flutter/material.dart';
import 'package:home_function/constants/calendar_settings.dart';
import 'package:home_function/models/meeting_data_source.dart';
import 'package:home_function/screens/home/widgets/add_appointment_dialog.dart';
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
      color: Colors.green,
    ),
    Appointment(
      startTime: DateTime.now().add(Duration(hours: 2)),
      endTime: DateTime.now().add(Duration(hours: 4)),
      subject: 'Test Event 2',
      notes:
          "There are many variations of passages of Lorem Ipsum available, but the majority have suffered alteration in some form, by injected humour, or randomised words which don't look even slightly believable. If you are going There are many variations of passages of Lorem Ipsum available, but the majority have suffered alteration in some form, by injected humour, or randomised words which don't look even slightly believable. If you are going to use a passage of Lorem Ipsum, you need to be sure there isn't anything embarrassing hidden in the middle of text. All the Lorem Ipsum generators on the Internet tend to repeat predefined chunks as necessary, making this the first true generator on the Internet. It uses a dictionary of over 200 Latin words, combined with a handful of model sentence structures, to generate Lorem Ipsum which looks reasonable. The generated Lorem Ipsum is therefore always free from repetition, injected humour, or non-characteristic words etc.to use a passage of Lorem Ipsum, you need to be sure there isn't anything embarrassing hidden in the middle of text. All the Lorem Ipsum generators on the Internet tend to repeat predefined chunks as necessary, making this the first true generator on the Internet. It uses a dictionary of over 200 Latin words, combined with a handful of model sentence structures, to generate Lorem Ipsum which looks reasonable. The generated Lorem Ipsum is therefore always free from repetition, injected humour, or non-characteristic words etc.",
      color: const Color.fromARGB(255, 86, 76, 175),
    ),
  ];

  void _addAppointment(Appointment appointment) {
    setState(() {
      _appointments.add(appointment);
    });
  }

  void deleteEvent(Appointment appointment) {
    _appointments.remove(appointment);
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
                  maxLines: 5, // Allows multi-line editing
                  minLines: 3, // Minimum height for the TextField
                  textInputAction: TextInputAction.newline,
                  keyboardType: TextInputType.multiline,
                ),
                // const SizedBox(height: 10),
                // TextButton(
                //   onPressed: () async {
                //     TimeOfDay? pickedTime = await showTimePicker(
                //       context: context,
                //       initialTime: selectedTime,
                //       builder: (BuildContext context, Widget? child) {
                //         return MediaQuery(
                //           data: MediaQuery.of(context)
                //               .copyWith(alwaysUse24HourFormat: true),
                //           child: child!,
                //         );
                //       },
                //       errorInvalidText: "Invalid Time",
                //     );
                //     if (pickedTime != null) {
                //       selectedTime = pickedTime; // Update selected time
                //     }
                //   },
                //   child: Text(
                //     'Select Start Time: ${selectedTime.format(context)}',
                //     style: const TextStyle(fontSize: 16),
                //   ),
                // ),
                // const SizedBox(height: 10),
                // TextField(
                //   controller: durationController,
                //   decoration: const InputDecoration(
                //     labelText: 'Duration (Minutes)',
                //   ),
                //   keyboardType: TextInputType.number,
                // ),
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
          title: Text("Calendar"),
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
