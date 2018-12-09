import 'package:english_teacher_app/controls/english_rich_text.dart';
import 'package:english_teacher_app/models/text.dart';
import 'package:flutter/material.dart';
import 'package:flutter/widgets.dart';

class PageTypeWords extends StatefulWidget {
  @override
  State<StatefulWidget> createState() => _PageTypeWordsState();
}

class _PageTypeWordsState extends State<PageTypeWords> {
  WordOffsetsController wordOffsetsController = new WordOffsetsController();
  ScrollController scrollController = new ScrollController();
  Practice practice;
  List<Word> words = new List<Word>();
  Word currentWord;
  int currentWordIndex = 0;
  double fontSize = 22.0;
  double cacheFontSize;
  RegExp regWords = new RegExp("[a-zA-Z\-']+");
  void loadData() {
    this.setState(() {
      practice = getPractice();
      practice.articles.forEach((a) {
        a.sentences.forEach((s) {
          words.addAll(s.words);
        });
      });
      currentWord = words.first;
    });
  }

  @override
  void initState() {
    super.initState();
    loadData();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        appBar: AppBar(
          actions: renderToolbar(),
        ),
        body: GestureDetector(
          child:  SingleChildScrollView(
            padding: EdgeInsets.all(10.0),
            controller: scrollController,
            child: EnglishRichText([renderArticle(practice.articles[0])],
                controller: wordOffsetsController),
          ),
          onScaleStart: (a) {
            cacheFontSize = fontSize;
          },
          onScaleUpdate: (ScaleUpdateDetails s) {
            var z = cacheFontSize * s.scale;
            
            this.setState(() {
              this.fontSize = z;
              print( "==============" + z.toString());
            });
          },
        ));
  }

  List<Widget> renderToolbar() {
    return <Widget>[
      IconButton(
        icon: Icon(Icons.add_circle_outline),
        onPressed: () {
          this.setState(() {
            fontSize = 2.0 + fontSize;
          });
        },
      ),
      IconButton(
        icon: Icon(Icons.remove_circle_outline),
        onPressed: () {
          this.setState(() {
            var c = fontSize - 2.0;
            if (c < 12.0) c = 12.0;
            fontSize = c;
          });
        },
      ),
      IconButton(
        icon: Icon(Icons.add_circle_outline),
        onPressed: () {
          var c = wordOffsetsController.getOffset("have");
          print(c);
          scrollController.jumpTo(c);
        },
      ),
      IconButton(
        icon: Icon(Icons.next_week),
        onPressed: () {
          this.setState(() {
            this.currentWord = words.elementAt(currentWordIndex);
            currentWordIndex = currentWordIndex + 1;
            if (currentWordIndex >= words.length) {
              currentWordIndex = 0;
            }
          });
        },
      ),
    ];
  }

  WordTextSpan renderArticleTitle(String title) {
    return WordTextSpan(
        text: "${title}\r\n\r\n",
        style: TextStyle(fontSize: fontSize + 2, color: Colors.blue));
  }

  WordTextSpan renderArticle(Article article) {
    WordTextSpan artSpan =
        WordTextSpan(children: [renderArticleTitle(article.title)]);
    article.sentences.forEach((s) {
      artSpan.children.add(renderSentence(s));
    });
    return artSpan;
  }

  WordTextSpan renderSentence(Sentence sentence) {
    WordTextSpan senSpan =
        new WordTextSpan(children: [], style: TextStyle(fontSize: fontSize));
    if (sentence.words == null || sentence.words.length <= 0) {
      senSpan.children.add(new TextSpan(text: sentence.english));
    } else {
      var matches = regWords.allMatches(sentence.english);

      int curIndex = 0;
      for (var i = 0; i < matches.length; i++) {
        var m = matches.elementAt(i);
        var wordStr = m.group(0);
        print("word 1  = ${wordStr} ${m.start} ${m.end} ");
        var word = sentence.getWord(wordStr);

        if (word == null) continue;

        //---------添加单词前的句子成分
        var s = sentence.english.substring(curIndex, m.start);
        if (s != null && s.length > 0) {
          senSpan.children.add(renderSentenceNormalPart(s));
        }

        //---------添加单词
        senSpan.children.add(renderWord(word));
        curIndex = m.start + wordStr.length;
      }

      //---------添加句末尾
      var s3 = sentence.english.substring(curIndex);
      senSpan.children.add(renderSentenceNormalPart(s3 + " "));

      matches.forEach((m) {});
    }
    return senSpan;
  }

  WordTextSpan renderSentenceNormalPart(String part) {
    return WordTextSpan(text: part);
  }

  WordTextSpan renderWord(Word word) {
    WordTextSpan sp = WordTextSpan(id: word.english, children: []);
    if (word != currentWord) {
      sp.children.add(new WordTextSpan(text: word.english, style: TextStyle()));
    } else {
      sp.children.add(new WordTextSpan(
          text: "          ",
          style: TextStyle(
              decoration: TextDecoration.underline, color: Colors.orange)));
      sp.children.add(
          new WordTextSpan(text: "(", style: TextStyle(color: Colors.grey)));
      sp.children.add(new WordTextSpan(
          text: word.chinese, style: TextStyle(color: Colors.green)));
      sp.children.add(
          new WordTextSpan(text: ")", style: TextStyle(color: Colors.grey)));
    }

    return sp;
  }
}
