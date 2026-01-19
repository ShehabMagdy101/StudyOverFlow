import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:home_function/screens/notes.dart';
import 'package:home_function/navBar.dart';
import 'package:syncfusion_flutter_calendar/calendar.dart';

void main() {
  group('Notes Screen Tests', () {
    testWidgets('Add Note Test', (WidgetTester tester) async {
      // Arrange: Pump the Notes widget into the tester
      await tester.pumpWidget(MaterialApp(home: Notes()));

      // Act: Tap on the floating action button to show the dialog
      await tester.tap(find.byIcon(Icons.add));
      await tester.pumpAndSettle(); // Wait for the dialog to appear

      // Act: Enter title and content in the text fields
      await tester.enterText(find.byType(TextField).at(0), 'Test Note Title');
      await tester.enterText(find.byType(TextField).at(1), 'This is the content of the note.');

      // Act: Tap the 'Add' button to submit the note
      await tester.tap(find.text('Add'));
      await tester.pumpAndSettle(); // Wait for the note to be added

      // Assert: Check if the new note title appears in the note list
      expect(find.text('Test Note Title'), findsOneWidget);
      expect(find.text('This is the content of the note.'), findsOneWidget);
    });

    testWidgets('Calendar screen adapts to small and large screen sizes', (WidgetTester tester) async {
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
}
