import 'dart:async';
import 'dart:convert' ;
import 'dart:io';
import 'package:http/http.dart' as Http;

class BaiduAIService {
  final String ClientID = "QUvZVAgyj6VpZtrypGFvW800";
  final String ClientSecret = "jrvTbtO1CEtQs3ji0xdCVxa6mYVxMPY8";
  final String Host = "aip.baidubce.com";

  String mAccessToken;
  bool mIsLoged = false;
  BaiduAIService();
  static BaiduAIService myInstance;
  static getInstance() {
    if (BaiduAIService.myInstance == null) {
      BaiduAIService.myInstance = new BaiduAIService();
    }
    return BaiduAIService.myInstance;
  }

  Future<Null> getFaceID(String imageBase64) async {
    var url = 'https://${Host}/rest/2.0/face/v3/search?access_token=${this.mAccessToken}';
    var group = "Users";
    var type = "BASE64"; 
    var img =   Uri.encodeQueryComponent(imageBase64); 
    var body = 'image=${img}&image_type=${type}&group_id_list=${group}';
    var response = await Http.post(url, body: body);
    var rv  = new ApiSearchResult.fromJsonString(response.body);
    return rv;
  }

  Future<ApiLoginResult> login() async {
    if (mIsLoged) {
      return new ApiLoginResult();
    }

    var method = "client_credentials";
    var url =   'https://${Host}/oauth/2.0/token?grant_type=${method}&client_id=${ClientID}&client_secret=${ClientSecret}';
    var response = await Http.get(url);
    var rv = new ApiLoginResult.fromJsonString(response.body);
    if (rv.error_msg == null && rv.access_token != null) {
      this.mAccessToken = rv.access_token;
      this.mIsLoged = true;
    }
    return rv;
  }
}

class ApiResult {
  String error_msg;
  String error_code;
}

class ApiLoginResult extends ApiResult {
  String access_token;
  ApiLoginResult();
  ApiLoginResult.fromJsonString(String data) {
    var j = json.decode(data);
    access_token = j["access_token"];
    error_msg = j["error_msg"];
    error_code = j["error_code"];
  }
}

class ApiSearchResult extends ApiResult {
  FaceResult result;
  ApiSearchResult.fromJsonString(String data){
    var j = json.decode(data);
    error_msg = j["error_msg"];
    error_code = j["error_code"]; 
    
  }
}

class FaceData {
  String group_id;
  String user_id;
  String user_info;
  double score;
}

class FaceResult {
  String face_token;
  List<FaceData> user_list;
}

class FaceAnalyzer {
  bool Find;
  String FindUser;
  double Score;
  String Group;
  constructor(FaceResult faces) {
    if (faces == null ||
        faces.user_list == null ||
        faces.user_list.length <= 0) {
      this.Find = false;
    } else {
      var face = faces.user_list[0];
      if (face.score < 84) {
        this.Find = false;
      } else {
        this.Find = true;
        this.FindUser = face.user_id;
        this.Score = face.score;
        this.Group = face.group_id;
      }
    }
  }
}
