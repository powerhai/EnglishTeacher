class Word {
  String english;
  String chinese;
  String symbol;
  Word({this.english, this.chinese, this.symbol});
  bool error = false;
  int errorCount = 0;
  Sentence sentence;
  Article article;
}

class Sentence {
  String id;
  String english;
  String chinese;
  String audioLocalPath;
  String audioUrl;
  Sentence({this.id, this.english, this.chinese});
  List<Word> words = new List<Word>();
  Word getWord(String word) {
    return words.firstWhere((t) {
      return t.english == word;
    }, orElse: () {
      return null;
    });
  }
}

class Article {
  String id;
  String title;
  Article({this.id, this.title});
  List<Sentence> sentences = new List<Sentence>();
}

class Practice {
  List<Article> articles = new List<Article>();
}

Practice getPractice() {
  var p = new Practice();
  {
    var a1 = new Article(title: "Our school day");

    {
      var s1 = new Sentence(english: "I have a storybook.", chinese: "我有一本英语书");
      s1.words.add(new Word(english: "have", chinese: "有"));
      s1.words.add(new Word(english: "storybook", chinese: "故事书"));
      a1.sentences.add(s1);
    }

    {
      var s1 = new Sentence(english: "Look at that room.", chinese: "我有一本英语书");
      s1.words.add(new Word(english: "Look", chinese: "有"));
      s1.words.add(new Word(english: "that", chinese: "故事书"));
      a1.sentences.add(s1);
    }

    {
      var s1 = new Sentence(
          english:
              "Base on your comments, I designed two drafts see below, which one is more appropriate? ",
          chinese: "基于你的提醒，我做了两个设计草图。");
      s1.words.add(new Word(english: "on", chinese: "在。。。里"));
      a1.sentences.add(s1);
    }

    p.articles.add(a1);
  }


  {
    var a1 = new Article(title: "My living room");

   
    {
      var s1 = new Sentence(english: "I love you", chinese: "我喜欢你");
      s1.words.add(new Word(english: "love", chinese: "爱")); 
      a1.sentences.add(s1);
    }

    {
      var s1 = new Sentence(
          english:
              "A more experience user will prefer the Connection screen since it is easier and quicker. ",
          chinese: "我这里有一个更好的方案");
      s1.words.add(new Word(english: "will", chinese: "会"));
      s1.words.add(new Word(english: "quicker", chinese: "快速的"));

      a1.sentences.add(s1);
    }

    p.articles.add(a1);
  }


  return p;
}
