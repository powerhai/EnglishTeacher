import 'package:english_teacher_app/pages/page_home.dart';
import 'package:english_teacher_app/pages/page_register.dart';
import 'package:english_teacher_app/pages/page_type_words.dart';
import 'package:english_teacher_app/pages/page_typewriting.dart';
import 'package:flutter/material.dart';

void main() => runApp(MyApp());

class MyApp extends StatelessWidget { 
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'English Teacher',
      theme: ThemeData( 
        primarySwatch: Colors.blue,
      ),
      home:  PageTypeWords(), // PageHome( ),
      routes: <String,WidgetBuilder>{
       
         "Register": (BuildContext context) => new PageRegister(),
         "Typewriting": (content)=> new PageTypewriting()
      },
    );
  }
}
 