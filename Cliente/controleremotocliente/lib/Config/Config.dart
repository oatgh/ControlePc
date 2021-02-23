import 'package:shared_preferences/shared_preferences.dart' as Prefs;
import 'package:flutter/material.dart';
class Config{

  Config();

  Future<bool> verificaIP() async{

    var prefs = await Prefs.SharedPreferences.getInstance();

    if(!prefs.containsKey("hostIP"))
    {
      return true;
    }

  }
  Future<bool> verificaPorta() async{

    var prefs = await Prefs.SharedPreferences.getInstance();

    if(!prefs.containsKey("port"))
    {
      return false;
    }
    else{
      return true;
    }

  }

  Future<bool> verificaSenha() async{

    var prefs = await Prefs.SharedPreferences.getInstance();

    if(!prefs.containsKey("senha"))
    {
      return false;
    }else{
      return true;
    }

  }
}