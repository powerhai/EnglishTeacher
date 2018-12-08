import 'dart:async';
import 'dart:convert';
import 'dart:io';
import 'package:english_teacher_app/pages/page_register.dart';
import 'package:english_teacher_app/services/baidu_ai_service.dart'; 
import 'package:path_provider/path_provider.dart';
import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:camera/camera.dart';

class PageHome extends StatefulWidget {
  PageHome({Key key, this.title}) : super(key: key);
  final String title;
  @override
  PageHomeState createState() => new PageHomeState();
}

class PageHomeState extends State<PageHome> {
  List<CameraDescription> mCamers;
  CameraController mController;
  BaiduAIService mBaiduAIService = BaiduAIService.getInstance();
  String photo;
  bool isTakedPhoto = false;
  PageHomeState();

  @override
  void initState() {
    super.initState();

    availableCameras().then((a) {
      mCamers = a;
      var camera = mCamers.firstWhere((c) {
        return c.lensDirection == CameraLensDirection.back;
      });
      mController = new CameraController(camera, ResolutionPreset.low);
      mController.initialize().then((_) {
        if (!mounted) {
          return;
        }
        setState(() {});
      });
    });
  }

  @override
  void dispose() {
    mController?.dispose();
    super.dispose();
  }

  String timestamp() => new DateTime.now().millisecondsSinceEpoch.toString();
  bool show = true;
  Future _incrementCounter() async {
    takePhoto().then((base64) async {
      //
      await mBaiduAIService.login();
      await mBaiduAIService.getFaceID(base64);
      //setState(() {});
    });
  }

  void goDemo() {
    Navigator.pushNamed(context, "Test");

  }

  Future goRegister() async {
    var photo = await takePhoto();
    //Navigator.pushNamed(context, "Register");
    Navigator.push(context, new MaterialPageRoute(builder: (c) {
      return new PageRegister(
        photo: photo,
      );
    }));
  }

  String getPhotoBase64(String filePath) {
    isTakedPhoto = true;
    File file = new File(filePath);
    List<int> bytes = file.readAsBytesSync();
    print("图片大小:  " + bytes.length.toString());
    String base64 = base64Encode(bytes);
    return base64;
  }

  Future<String> takePhoto() async {
    try {
      final Directory extDir = await getApplicationDocumentsDirectory();
      final String dirPath = '${extDir.path}/Pictures/flutter_test';
      Directory dc = new Directory(dirPath);
      if (await dc.exists()) {
        dc.delete(recursive: true);
      }

      await new Directory(dirPath).create(recursive: true);
      final String filePath = '$dirPath/${timestamp()}.jpg';
      await mController.takePicture(filePath);
      photo = filePath;
    } catch (e) {}

    return photo;
  }

  String pageName = "china";
  @override
  Widget build(BuildContext context) {
    return Scaffold(
        appBar: AppBar(
          title: Text(widget.title),
        ),
        body: getBody(context));
  }

  Widget getCamera() {
    if (mController?.value != null && mController.value.isInitialized)
      return new CameraPreview(mController);
    else
      return new Container();
  }

  Widget getBody(BuildContext context) {
    var screenSize = MediaQuery.of(context).size;
    return Container(
      child: Stack(
        children: <Widget>[
          getCamera(),
          new Positioned(
            bottom: 60.0,
            width: screenSize.width,
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceAround,
              children: <Widget>[
                new RaisedButton(
                  child: new Text(pageName),
                  onPressed: goDemo,
                ),
                new RaisedButton(
                    child: new Text("Register"), onPressed: goRegister),
                this.show
                    ? new Text("show")
                    : new RaisedButton(
                        child: new Text("hide"), onPressed: () {})
              ],
            ),
          )
        ],
      ),
    );
  }
}
