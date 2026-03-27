import 'package:flutter/material.dart';
import 'package:home_function/constants/Colors.dart';

class Subjects extends StatefulWidget {
  const Subjects({super.key});

  @override
  State<Subjects> createState() => _Subjects();
}

class _Subjects extends State<Subjects> {
  final List<Color> subjectColors = [
    electricBlue,
    teaGreen,
    cream,
    sunset,
    melon,
  ];

  // Improved subject card widget with better overflow handling
  Widget subject(String subjectTitle, String subjectNotes, int index) {
    return Padding(
      padding: const EdgeInsets.all(8.0),
      child: Container(
        width: 180,
        constraints: const BoxConstraints(minHeight: 160),
        decoration: BoxDecoration(
          color: subjectColors[index % subjectColors.length],
          borderRadius: BorderRadius.circular(20),
          boxShadow: [
            BoxShadow(
              color: Colors.grey.withOpacity(0.3),
              spreadRadius: 2,
              blurRadius: 5,
              offset: const Offset(0, 3),
            ),
          ],
        ),
        child: Material(
          color: Colors.transparent,
          child: InkWell(
            borderRadius: BorderRadius.circular(20),
            onTap: () {},
            child: Padding(
              padding: const EdgeInsets.all(16.0),
              child: Column(
                mainAxisSize: MainAxisSize.min,
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Icon(
                    _getSubjectIcon(subjectTitle),
                    size: 32,
                    color: Colors.white,
                  ),
                  const SizedBox(height: 12),
                  Text(
                    subjectTitle,
                    style: const TextStyle(
                      fontSize: 18,
                      fontWeight: FontWeight.bold,
                      color: Colors.white,
                    ),
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                  ),
                  const SizedBox(height: 8),
                  Flexible(
                    child: Text(
                      subjectNotes,
                      style: const TextStyle(
                        fontSize: 14,
                        color: Colors.white70,
                      ),
                      maxLines: 3,
                      overflow: TextOverflow.ellipsis,
                    ),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }

  IconData _getSubjectIcon(String subject) {
    switch (subject.toLowerCase()) {
      case 'machine learning':
        return Icons.psychology;
      case 'software engineering':
        return Icons.code;
      case 'data science':
        return Icons.analytics;
      case 'data compression':
        return Icons.compress;
      default:
        return Icons.school;
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.grey[100],
      appBar: AppBar(
        backgroundColor: Colors.white,
        elevation: 0,
        title: const Text(
          'My Subjects',
          style: TextStyle(
            fontWeight: FontWeight.bold,
            color: Colors.black,
            fontSize: 24,
          ),
        ),
        
        actions: [
          IconButton(
            icon: const Icon(Icons.search, color: Colors.white),
            onPressed: () {},
          ),
          IconButton(
            icon: const Icon(Icons.more_vert, color: Colors.white),
            onPressed: () {},
          ),
        ],
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Padding(
              padding: EdgeInsets.only(left: 8.0, bottom: 16.0),
              child: Text(
                'Subjects',
                style: TextStyle(
                  fontSize: 20,
                  fontWeight: FontWeight.bold,
                  color: Colors.black,
                ),
              ),
            ),
            Wrap(
              spacing: 8.0,
              runSpacing: 8.0,
              children: [
                subject("Machine Learning", "This subject is about machine learning models", 0),
                subject("Software Engineering", "This subject is about how to make software", 1),
                subject("Data Science", "Advanced data analysis and visualization", 2),
                subject("Data Compression", "Algorithms for data compression", 3),
                subject("AI Ethics", "Ethical considerations in AI development", 4),
              ],
            ),
          ],
        ),
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: () {},
        backgroundColor: electricBlue,
        child: const Icon(Icons.add),
      ),
    );
  }
}