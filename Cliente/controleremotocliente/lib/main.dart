import 'package:controleremotocliente/Config/Config.dart';
import 'package:controleremotocliente/homePage.dart';
import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart' as prefs;
void main() {
  runApp(MaterialApp(home: HomePage(),
    debugShowCheckedModeBanner: false,));
}