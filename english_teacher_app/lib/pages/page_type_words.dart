import 'package:english_teacher_app/controls/english_rich_text.dart';
import 'package:english_teacher_app/models/text.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter/widgets.dart';

class PageTypeWords extends StatefulWidget {
  @override
  State<StatefulWidget> createState() => _PageTypeWordsState();
}

class _PageTypeWordsState extends State<PageTypeWords> {
  WordOffsetsController wordOffsetsController = new WordOffsetsController();
  ScrollController scrollController = new ScrollController();
  FocusNode focusNode = new FocusNode();
  Practice practice;
  List<Word> words = new List<Word>();
  Word currentWord;
  int currentWordIndex = 0;
  double fontSize = 22.0;
  double cacheFontSize;
  final maxFontSize = 70.0;
  final minFontSize = 12.0;
  String inputText = "";

  RegExp regWords = new RegExp("[a-zA-Z\-']+");
  void loadData() {
    this.setState(() {
      practice = getPractice();
      practice.articles.forEach((a) {
        a.sentences.forEach((s) {
          s.words.forEach((w) {
            w.sentence = s;
            w.article = a;
          });
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
    print("build");
    FocusScope.of(context).requestFocus(focusNode);
    return Scaffold(
        appBar: AppBar(
          actions: renderToolbar(),
        ),
        body: Column(
          children: <Widget>[
            RawKeyboardListener(
              onKey: onKey,
              child: Expanded(
                child: GestureDetector(
                  child: SingleChildScrollView(
                    padding: EdgeInsets.all(10.0),
                    controller: scrollController,
                    child: EnglishRichText([renderCurrentArticle()],
                        controller: wordOffsetsController),
                  ),
                  onScaleStart: (a) {
                    cacheFontSize = fontSize;
                  },
                  onScaleUpdate: (ScaleUpdateDetails s) {
                    var z = cacheFontSize * s.scale;
                    if (z > maxFontSize || z < minFontSize) return;
                    this.setState(() {
                      this.fontSize = z;
                    });
                  },
                ),
              ),
              focusNode: focusNode,
            )
          ],
        ));
  }

  void onKey(RawKeyEvent event) {
    if (event is RawKeyUpEvent) return;
    var c = event.data as RawKeyEventDataAndroid;

    if (c.codePoint >= 32 && c.codePoint <= 126) {
      var char = String.fromCharCode(c.codePoint);
      this.setState(() {
        this.inputText += char;
      });
    } else {
      if (c.keyCode == 67) {
        if (this.inputText.length <= 0) return;
        this.setState(() {
          this.inputText =
              this.inputText.substring(0, this.inputText.length - 1);
        });
      } else if (c.keyCode == 66) {
        this.setState(() {
          this.inputText = "";
          moveToNextWord();
        });
      }
    }

    print(
        "keycode: ${c.keyCode} flags: ${c.flags} codePoint: ${c.codePoint} metaState: ${c.metaState} scanCode: ${c.scanCode}  ");
  }

  @override
  void didUpdateWidget(PageTypeWords oldWidget) {
    super.didUpdateWidget(oldWidget);
    print("didUpdateWidget");
  }

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    print("didChangeDependencies");
  }

  void moveToNextWord() {
    this.setState(() {
      this.currentWordIndex += 1;
      if (currentWordIndex >= words.length) {
        currentWordIndex = 0;
      }
      this.currentWord = words.elementAt(currentWordIndex);
    });
  }

  WordTextSpan renderCurrentArticle() {
    var art = currentWord.article;
    return renderArticle(art);
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
           moveToNextWord();
        },
      ),
    ];
  }

  WordTextSpan renderArticleTitle(String title) {
    return WordTextSpan(
      text: "${title}\r\n\r\n",
      style: TextStyle(
        fontSize: fontSize + 2,
        color: Colors.blue,
      ),
    );
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
    WordTextSpan senSpan = new WordTextSpan(
        children: [],
        style: TextStyle(
            fontSize: fontSize,
            fontWeight: sentence == currentWord.sentence
                ? FontWeight.bold
                : FontWeight.normal));
    if (sentence.words == null || sentence.words.length <= 0) {
      senSpan.children.add(new TextSpan(text: sentence.english));
    } else {
      var matches = regWords.allMatches(sentence.english);

      int curIndex = 0;
      for (var i = 0; i < matches.length; i++) {
        var m = matches.elementAt(i);
        var wordStr = m.group(0);
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
      senSpan.children.add(renderSentenceNormalPart(s3 + "\r\n"));

      matches.forEach((m) {});
    }

    //添加句子的中文解释
    if (sentence == currentWord.sentence) {
      senSpan.children.add(new WordTextSpan(
          text: sentence.chinese + "\r\n",
          style: TextStyle(
              color: Colors.cyan[600],
              fontSize: fontSize - 2.0,
              fontWeight: FontWeight.normal)));
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
      Color textColor = Colors.orange;
      if(inputText.length > 0 && inputText.length<= word.english.length){
        if(inputText == word.english)
        {
          textColor = Colors.green;
        }else  if(inputText == word.english.substring(0, inputText.length)){
            textColor = Colors.blue  ;
        }
      }
      sp.children.add(new WordTextSpan(
          text: "    ${inputText}    ",
          style: TextStyle(
              decoration: TextDecoration.underline, color: textColor)));
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
