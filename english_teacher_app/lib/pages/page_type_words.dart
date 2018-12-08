

import 'package:english_teacher_app/controls/english_rich_text.dart';
import 'package:flutter/material.dart';
import 'package:flutter/widgets.dart';

class PageTypeWords extends StatefulWidget{
  @override
  State<StatefulWidget> createState()  => _PageTypeWordsState();

}

class _PageTypeWordsState extends State<PageTypeWords> {

   WordOffsetsController wordOffsetsController = new WordOffsetsController();
  ScrollController scrollController = new ScrollController();


  @override
  Widget build(BuildContext context) {
    return Scaffold(
        appBar: AppBar(
          actions: <Widget>[
            IconButton(
              icon: Icon(Icons.satellite),
              onPressed: () {
                var c = wordOffsetsController.getOffset("Hello");
                print(c);
                scrollController.jumpTo(c);
              },
            ),
             IconButton(
              icon: Icon(Icons.satellite),
              onPressed: () {
                var c = wordOffsetsController.getOffset("World");
                print(c);
                scrollController.jumpTo(c);
              },
            ),
          ],
        ),
        body: SingleChildScrollView( controller: scrollController,
          child: EnglishRichText([
             WordTextSpan(text: "444 历史总是惊人的相似\r\n" , id: "World"),
             WordTextSpan(children: [
              WordTextSpan(
                  text:
                      "1111 last Ideally we could compute the min/max intr\r\ninsic width/height with a "),
              WordTextSpan(
                  text:
                      "2222 lihai  non-destructive operation. However, currently, computing these values a ")
            ], style: TextStyle(color: Colors.yellow, fontSize: 50.3)),
             WordTextSpan(children: [
              WordTextSpan(
                  text:
                      "1111 last Ideally we could compute the min/max intr\r\ninsic width/height with a "),
              WordTextSpan(
                  text:
                      "2222 lihai  non-destructive operation. However, currently, computing these values a ")
            ], style: TextStyle(color: Colors.yellow, fontSize: 50.3)),
            WordTextSpan(children: [
              WordTextSpan(
                  text:
                      "1111 last Ideally we could compute the min/max intr\r\ninsic width/height with a "),
              WordTextSpan(
                  text:
                      "2222 lihai  non-destructive operation. However, currently, computing these values a ")
            ], style: TextStyle(color: Colors.yellow, fontSize: 50.3)),
            WordTextSpan(
                text: "333 my \r\n counter",
                style: TextStyle(color: Colors.green, fontSize: 50.3)),
            //ImageSpan(AssetImage("assets/res/wwc.jpg"),
            //     imageWidth: 120.0, imageHeight: 80.0),
            WordTextSpan(text: "444 历史总是惊人的相似\r\n"),
            WordTextSpan(children: [
              WordTextSpan(
                  id: "Hello",
                  text:
                      "555 last Ideally we could compute the min/max intrinsic width/height with a ",
                  style: TextStyle(color: Colors.blue, fontSize: 50.3)),
              WordTextSpan(
                  text:
                      "666 lihai  non-destructive operation. However, currently, computing these values a ")
            ]),
          ], controller: wordOffsetsController),
        ));
  }

}