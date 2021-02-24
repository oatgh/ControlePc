import 'dart:io';
import 'dart:convert';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:fluttertoast/fluttertoast.dart';
class Connect{
  Connect();

  static String hostIp;
  static int port;
  static void ret() async{
    final prefs = await SharedPreferences.getInstance();
    hostIp = prefs.getString("hostIp");
    port = prefs.getInt("port");
  }

  static Socket socket;
  static bool isConectado = false;
  static Future<bool> conectar() async
  {
    ret();

    if(!isConectado){
      try
      {
        socket = await Socket.connect("10.0.0.123", 2000);
        Fluttertoast.showToast(msg: "Conectado");
        print("Conectado");
        isConectado = true;
        return true;
      }catch(e){
        Fluttertoast.showToast(msg: "Erro ao conectar ${e.toString()}");
        print("Erro ao conectar ${e.toString()}");
        isConectado = false;
        return false;
      }
    }else{
      socket.close();
      isConectado = false;
      Fluttertoast.showToast(msg: "Disconectado");
      print("Disconectado");
      return false;
    }
  }



  static enviaComando(String comando) async
  {
    try
    {
      socket.write(comando);
      Fluttertoast.showToast(
          msg: "Comando ${json.decode(comando)["msg"]}\nEnviado");
      conectar();
    }catch(err)
    {
      Fluttertoast.showToast(msg: "Ocorreu um erro ${err}");
    }

  }

  connect(String comando) async {

    try
    {
      socket.write(comando);
      conectar();
    }catch(err)
    {
      Fluttertoast.showToast(msg: "Ocorreu um erro ${err.toString()}");
    }
    Fluttertoast.showToast(
        msg: "Comando ${json.decode(comando)["msg"]}\nEnviado");

    ret();
    // try{
    //  Socket.connect(hostIp, port).then((socket){
    //    socket.write(comando);
    //    socket.listen((data) {
    //      String retorno = new String.fromCharCodes(data);
    //      print(retorno);
    //      if (retorno == "0"){
    //
    //      }else{
    //        Fluttertoast.showToast(msg: "${json.decode(comando)["msg"]}\nMal sucedido");
    //      }
    //    });
    //  },);
    // }catch(e){
    //   Fluttertoast.showToast(msg: "Ocorreu um erro: ${e}");
    // }
  }
}