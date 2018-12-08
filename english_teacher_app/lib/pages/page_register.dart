import 'dart:io';

import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:haiser_flutter_lib/twoLevelPicker/models.dart';
import 'package:haiser_flutter_lib/twoLevelPicker/twoLevelPicker.dart'; 
import 'package:haiser_flutter_lib/date_picker/dateformfield.dart';

import 'package:intl/intl.dart';

class PageRegister extends StatefulWidget {
  PageRegister({Key key, this.photo = "", this.title = "Register"})
      : super(key: key);
  final String title;
  final String photo;
  @override
  PageRegisterState createState() => new PageRegisterState();
}

class PageRegisterState extends State<PageRegister> {
  final List<String> allActivities = <String>['男', '女'];
  final FixedExtentScrollController scrollController =
      new FixedExtentScrollController(initialItem: 2);
  String sex;
  final formKey = new GlobalKey<FormState>();

  DateTime dateTime;
  String name;
  @override
  void initState() {
    super.initState();
  }

  void didchangename(String s) {
    //showDialog(child: new Text(s), context: this.context);
  }

  Widget wgUserName() {
    return new TextFormField(
        onFieldSubmitted: didchangename,
        onSaved: (c) {
          // Scaffold.of(this.context).showSnackBar(new SnackBar( content: new Text("on save username"),));
        },
        validator: (s) {
          return s == "ok" ? null : "验证失败";
        },
        decoration: const InputDecoration(
            border: const UnderlineInputBorder(),
            filled: true,
            fillColor: Colors.white,
            hintText: '请输入您的姓名',
            labelText: '姓名',
            contentPadding: const EdgeInsets.all(5.0)));
  }

  Widget wgLevel() {
    return new TextFormField(
        decoration: const InputDecoration(
            border: const UnderlineInputBorder(),
            filled: true,
            fillColor: Colors.white,
            hintText: '你的英语有几级?',
            labelText: '级别',
            contentPadding: const EdgeInsets.all(5.0)));
  }

  Widget wgSex() {
    return new InputDecorator(
      isEmpty: this.sex == null,
      decoration: const InputDecoration(
          border: const UnderlineInputBorder(),
          labelText: '性别',
          fillColor: Colors.white,
          filled: true,
          contentPadding: const EdgeInsets.all(5.0)),
      child: new DropdownButton<String>(
          elevation: 24,
          isDense: true,
          value: this.sex,
          onChanged: (String newValue) {
            this.setState(() {
              this.sex = newValue;
            });
          },
          items: allActivities.map((String value) {
            return new DropdownMenuItem<String>(
                value: value, child: new Text(value));
          }).toList()),
    );
  }

  Widget wgPhoto() {
    return new Container(
        padding: new EdgeInsets.all(30.0),
        child: new CircleAvatar(
            backgroundImage: new FileImage(new File(widget.photo))));
  }

  Widget wgGrade() {
    return new InkWell(
      child: new InputDecorator(
        decoration: new InputDecoration(
            labelText: "出生日期",
            contentPadding: new EdgeInsets.all(5.0),
            fillColor: Colors.white),
        child: new Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          mainAxisSize: MainAxisSize.min,
          children: <Widget>[
            new Row(
              children: <Widget>[
                new Text("1978"),
                new Icon(Icons.arrow_drop_down,
                    color: Theme.of(context).brightness == Brightness.light
                        ? Colors.grey.shade700
                        : Colors.white70),
                new Text("年")
              ],
            )
          ],
        ),
      ),
    );
  }

  Widget wgAge() {
    var format = new DateFormat("yyyy-MM-dd");
    return new DatePickerFormField(
      decoration: new InputDecoration(labelText: "生日"),
      format: format,
      startDate: new DateTime(1978, 10, 1),
      endDate: new DateTime.now(),
      onSaved: (b) {
        this.setState(() {
          this.dateTime = b;
        });
      },
      validator: (b) {
        if (b == null) return null;
        return b.day == 1 ? null : "必须是1日";
      },
    );
  }

  Widget wgSex3() {
    var list = new List<LevelA<int, String>>();
    var itema1 = new LevelA(1, "小学");
    itema1.list.add(new LevelB(11, "一年级"));
    itema1.list.add(new LevelB(12, "二年级"));
    itema1.list.add(new LevelB(13, "三年级"));
    var itema2 = new LevelA(2, "中学");
    itema2.list.add(new LevelB(21, "七年级"));
    itema2.list.add(new LevelB(22, "八年级"));
    itema2.list.add(new LevelB(23, "九年级"));
    list.add(itema1);
    list.add(itema2);

    return new TwoLevelFormField(
      list: list,
      decoration: new InputDecoration(labelText: "年级"),
      validator: (c) {
        if (c != null) return c.data;
      },
      onSaved: (c) {
        if (c != null) print(c.data);
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    var box = const SizedBox(height: 24.0);
    return new Scaffold(
        backgroundColor: Colors.grey[200],
        appBar: new AppBar(
          title: new Text(widget.title),
        ),
        body: new DropdownButtonHideUnderline(
            child: Form(
          autovalidate: true,
          key: formKey,
          onChanged: () {
            //showAboutDialog(context: context, children: [new Text("dat3a")]);
          },
          child: new Column(children: <Widget>[
            wgAge(),
            wgSex3(),

            new RaisedButton(
              child: new Text("Submit"),
              onPressed: () {
                this.formKey.currentState.save();
              },
            )
            // CircleAvatar( backgroundImage: new FileImage(new File(widget.photoPath)))
          ]),
        )));
  }
}
