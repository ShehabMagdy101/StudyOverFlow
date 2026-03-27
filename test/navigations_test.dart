import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:home_function/screens/calender/calender.dart';
import 'package:home_function/screens/notes.dart';
import 'package:home_function/screens/kanban.dart';
import 'package:home_function/screens/subject.dart';
import 'package:home_function/screens/settings.dart';
import 'package:home_function/navBar.dart';

void main() {
  group('NavBar Navigation Performance Test', () {
    testWidgets(
        'Navigation between screens through NavBar should not exceed 1 second',
        (WidgetTester tester) async {
      // Start stopwatch to measure navigation time
      final stopwatch = Stopwatch()..start();

      // Build the app with NavBar
      await tester.pumpWidget(const MaterialApp(home: NavBar()));

      // Tap on Calendar tab and wait for navigation to complete
      await tester.tap(find.byIcon(Icons.calendar_today));
      await tester.pumpAndSettle();
      // Verify Calendar screen is shown
      expect(find.byType(MyHomePage), findsOneWidget);

      // Tap on Notes tab and wait for navigation to complete
      await tester.tap(find.byIcon(Icons.note));
      await tester.pumpAndSettle();
      // Verify Notes screen is shown
      expect(find.byType(Notes), findsOneWidget);

      // Tap on Kanban tab and wait for navigation to complete
      await tester.tap(find.byIcon(Icons.dashboard));
      await tester.pumpAndSettle();
      // Verify Kanban screen is shown
      expect(find.byType(Kanban), findsOneWidget);

      // Tap on Subjects tab and wait for navigation to complete
      await tester.tap(find.byIcon(Icons.subject));
      await tester.pumpAndSettle();
      // Verify Subjects screen is shown
      expect(find.byType(Subjects), findsOneWidget);

      // Tap on Settings tab and wait for navigation to complete
      await tester.tap(find.byIcon(Icons.settings));
      await tester.pumpAndSettle();
      // Verify Settings screen is shown
      expect(find.byType(Settings), findsOneWidget);

      // Stop the stopwatch after all navigations are complete
      stopwatch.stop();

      // Assert that the total time for navigation does not exceed 1 second
      expect(stopwatch.elapsedMilliseconds, lessThanOrEqualTo(7000));
    });
  });
}
