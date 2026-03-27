import 'package:flutter_test/flutter_test.dart';
import 'package:syncfusion_flutter_calendar/calendar.dart';
import 'package:flutter/material.dart';
import 'package:home_function/models/meeting_data_source.dart';
import 'package:home_function/navBar.dart';


void main() {
  group('Calendar Functionality Tests', () {
    group('Overlapping Events', () {
test('Adding events with the same time ', () {
  // Arrange
  final event1 = Appointment(
    startTime: DateTime(2024, 12, 25, 10, 0),
    endTime: DateTime(2024, 12, 25, 11, 0),
    subject: 'Event 1',
    color: Colors.blue,
  );

  final event2 = Appointment(
    startTime: DateTime(2024, 12, 25, 10, 0),
    endTime: DateTime(2024, 12, 25, 11, 0),
    subject: 'Event 2',
    color: Colors.red,
  );

  final List<Appointment> appointments = [event1];

  // Act
  // Directly add the second event without checking overlap
  appointments.add(event2);

  // Assert
  expect(appointments.length, 1, reason: 'Testing for see if 2 appointements could be added at the same time .');
});
    });

group('Edit, Delete, and Display Events', () {
      testWidgets('Display an event in the calendar', (WidgetTester tester) async {
        // Arrange
        final event = Appointment(
          startTime: DateTime(2024, 12, 25, 14, 0),
          endTime: DateTime(2024, 12, 25, 15, 0),
          subject: 'Original Event',
          notes: 'Original Description',
          color: Colors.green,
        );
        final List<Appointment> appointments = [event];
        final dataSource = MeetingDataSource(appointments);

        // Act
        await tester.pumpWidget(MaterialApp(
          home: Scaffold(body: SfCalendar(
            view: CalendarView.week,
            dataSource: dataSource,
          )),
        ));

        // Wait for the calendar to settle
        await tester.pumpAndSettle();

        // Assert
        // Verify the event is displayed with the correct subject
        expect(
          find.byWidgetPredicate((widget) =>
            widget is Text && widget.data == 'Original Event'),
          findsOneWidget,
        );
      });

      testWidgets('Edit an event in the calendar', (WidgetTester tester) async {
        // Arrange
        final event = Appointment(
          startTime: DateTime(2024, 12, 25, 14, 0),
          endTime: DateTime(2024, 12, 25, 15, 0),
          subject: 'Original Event',
          notes: 'Original Description',
          color: Colors.green,
        );
        final List<Appointment> appointments = [event];
        final dataSource = MeetingDataSource(appointments);

        await tester.pumpWidget(MaterialApp(
          home: Scaffold(body: SfCalendar(
            view: CalendarView.week,
            dataSource: dataSource,
          )),
        ));

        // Wait for the calendar to settle
        await tester.pumpAndSettle();

        // Act
        // Tap on the event to simulate editing it
        await tester.tap(find.text('Original Event'));
        await tester.pumpAndSettle();

        // Change event title and description
        await tester.enterText(find.byType(TextField).at(0), 'Edited Event');
        await tester.enterText(find.byType(TextField).at(1), 'Edited Description');
        await tester.tap(find.text('Save')); // Make sure you have a Save button in your widget
        await tester.pumpAndSettle();

        // Assert
        // Verify the updated title is displayed
        expect(
          find.byWidgetPredicate((widget) =>
            widget is Text && widget.data == 'Edited Event'),
          findsOneWidget,
        );
      });

      testWidgets('Delete an event from the calendar', (WidgetTester tester) async {
        // Arrange
        final event = Appointment(
          startTime: DateTime(2024, 12, 25, 14, 0),
          endTime: DateTime(2024, 12, 25, 15, 0),
          subject: 'Original Event',
          notes: 'Original Description',
          color: Colors.green,
        );
        final List<Appointment> appointments = [event];
        final dataSource = MeetingDataSource(appointments);

        await tester.pumpWidget(MaterialApp(
          home: Scaffold(body: SfCalendar(
            view: CalendarView.week,
            dataSource: dataSource,
          )),
        ));

        // Wait for the calendar to settle
        await tester.pumpAndSettle();

        // Act
        // Tap on the event to open the delete dialog
        await tester.tap(find.text('Original Event'));
        await tester.pumpAndSettle();

        // Trigger the delete action (you should have a delete button in your event dialog)
        await tester.tap(find.text('Delete Event'));
        await tester.pumpAndSettle();

        // Assert
        // Verify the event is no longer displayed
        expect(
          find.byWidgetPredicate((widget) =>
            widget is Text && widget.data == 'Original Event'),
          findsNothing,
        );
      });
    });

    group('Event Duration', () {
      test('Event duration can be exactly 24 hours', () {
        final event = Appointment(
          startTime: DateTime(2024, 12, 25, 0, 0),
          endTime: DateTime(2024, 12, 26, 0, 0),
          subject: 'All-Day Event',
          color: Colors.orange,
        );

        expect(event.endTime.difference(event.startTime).inHours, 24);
      });
    });

    group('Screen Size Compatibility', () {
      testWidgets('Calendar screen adapts to small and large screen sizes',
          (WidgetTester tester) async {
        await tester.pumpWidget(const MaterialApp(
          home: MediaQuery(
            data: MediaQueryData(size: Size(320, 480)), // Small screen
            child: NavBar(),
          ),
        ));

        // Verify small screen rendering
        expect(find.byType(SfCalendar), findsOneWidget);

        await tester.pumpWidget(const MaterialApp(
          home: MediaQuery(
            data: MediaQueryData(size: Size(1920, 1080)), // Large screen
            child: NavBar(),
          ),
        ));

        // Verify large screen rendering
        expect(find.byType(SfCalendar), findsOneWidget);
      });
    });

    group('Event Color', () {
      test('Event color is displayed correctly', () {
        final event = Appointment(
          startTime: DateTime(2024, 12, 25, 10, 0),
          endTime: DateTime(2024, 12, 25, 11, 0),
          subject: 'Colored Event',
          color: Colors.purple,
        );

        expect(event.color, Colors.purple);
      });
    });
  });
}
