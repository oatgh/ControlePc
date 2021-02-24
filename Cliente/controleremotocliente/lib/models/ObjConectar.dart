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
  static Future<bool> conectar(int choose) async
  {
    ret();

    if(!isConectado){
      try
      {
        socket = await Socket.connect("10.0.0.123", 2000);

        print("Conectado");
        isConectado = true;
        switch(choose){
          case 0:
            Fluttertoast.showToast(msg: "Conectado");
            break;
        }
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
      switch(choose){
        case 0:
          Fluttertoast.showToast(msg: "Disconectado");
          break;
      }
      print("Disconectado");
      return false;
    }
  }



  static enviaComando(String comando) async
  {
    if(isConectado){
      try
      {
        socket.write(comando);
        Fluttertoast.showToast(
            msg: "Enviado");
        conectar(1);
        conectar(1);
      }catch(err)
      {
        Fluttertoast.showToast(msg: "Ocorreu um erro ${err}");
      }
    }else{
      Fluttertoast.showToast(msg: "Você nao esta conectado");
    }


  }

  connect(String comando) async {

    try
    {
      socket.write(comando);
      conectar(0);
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