import 'package:flutter/material.dart';
import 'package:home_function/screens/kanban.dart';
import 'package:home_function/screens/notes.dart';
import 'package:home_function/screens/settings.dart';
import 'package:home_function/screens/subject.dart';
import 'package:persistent_bottom_nav_bar/persistent_bottom_nav_bar.dart';
import 'package:home_function/screens/calender/calender.dart';


class NavBar extends StatelessWidget {
  const NavBar({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    // PersistentTabController to manage the active tab
    final PersistentTabController _controller =
        PersistentTabController(initialIndex: 0);


    // List of navigation items
    final List<PersistentBottomNavBarItem> _navBarsItems = [
      PersistentBottomNavBarItem(
        icon: Icon(Icons.calendar_today),
        title: ("Calendar"),
        activeColorPrimary: Colors.blue,
        inactiveColorPrimary: Colors.grey,
      ),
      PersistentBottomNavBarItem(
        icon: Icon(Icons.note),
        title: ("Notes"),
        activeColorPrimary: Colors.blue,
        inactiveColorPrimary: Colors.grey,
      ),
      PersistentBottomNavBarItem(
        icon: Icon(Icons.dashboard),
        title: ("Kanban"),
        activeColorPrimary: Colors.blue,
        inactiveColorPrimary: Colors.grey,
      ),
      PersistentBottomNavBarItem(
        icon: Icon(Icons.subject),
        title: ("Subjects"),
        activeColorPrimary: Colors.blue,
        inactiveColorPrimary: Colors.grey,
      ),
      PersistentBottomNavBarItem(
        icon: Icon(Icons.settings),
        title: ("Settings"),
        activeColorPrimary: Colors.blue,
        inactiveColorPrimary: Colors.grey,
      ),
    ];

List<Widget> _buildScreens() {
    return [
       MyHomePage(),
      Notes(),
      Kanban(),
      Subjects(),
      Settings(),
    ];
  }
    // Build the PersistentTabView
     return PersistentTabView(
      context,
      controller: _controller,
      screens: _buildScreens(),
      items: _navBarsItems,
      confineToSafeArea: true,
      backgroundColor: Colors.white,
      handleAndroidBackButtonPress: true,
      resizeToAvoidBottomInset: true,
      stateManagement: true,
      navBarStyle: NavBarStyle.style3, // Choose your desired style
    );
  }
}
