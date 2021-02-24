import 'package:flutter/material.dart';
import 'package:fluttertoast/fluttertoast.dart';
import 'ObjConectar.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'dart:convert';
class Commands extends StatefulWidget {
  @override
  _CommandsState createState() => _CommandsState();
}

class _CommandsState extends State<Commands> {
  @override
  TextEditingController _controllerTxt = TextEditingController();
  _enviaCmd(String comand, int choose)async{
    final prefs = await SharedPreferences.getInstance();
    Map<String, dynamic> sendMap = Map();

    sendMap["pass"] = prefs.getString("pass");
    switch(choose){

      case 0:
        sendMap["msg"] = "shutdown";
        break;
      case 1:
        sendMap["msg"] = "message";
        break;
      case 2:
        sendMap["msg"] = "person";
        break;
    }
    sendMap["args"] = comand;
    String sendJson = json.encode(sendMap);
    Connect.enviaComando(sendJson);
  }
  Widget build(BuildContext context) {
    return Container(child: Column(children: [

      //Coluna de cima(Prefixados)
      Column(children: [
        ListTile(title: Text("DESLIGAR COM TEMPO"), subtitle:
        Text("Desliga seu computador num tempo determinado pelo usuário"),
        onTap: (){
          return showDialog(context: context, builder: (context){
            return AlertDialog(
              title: Text("Quantos segundos?(Ex: 120)"),
              content: TextField(controller: _controllerTxt,),
              actions: [
                FlatButton(onPressed: (){

                  if(Connect.isConectado){
                    Navigator.pop(context);
                    try{
                      if(_controllerTxt.text.isNotEmpty){
                        String cmd = "-s -t ${_controllerTxt.text}";
                        _enviaCmd(cmd, 0);
                      }
                    }catch(err){
                      Fluttertoast.showToast(msg: "Erro!");
                    }
                  }else{
                    Fluttertoast.showToast(msg: "Você está desconectado!");
                  }
                }, child: Text("ENVIAR")),


                FlatButton(onPressed: (){

                  Navigator.pop(context);

                }, child: Text("CANCELAR"))
              ],
            );
          });
        },
        ),
        ListTile(title: Text("CANCELAR DESLIGAMENTO"),
          subtitle: Text("Cancela o desligamento do computador"),
        onTap: () => _enviaCmd("-a", 0),),
        ListTile(title: Text("MANDAR AVISO"),
          subtitle: Text("Manda um aviso de texto para o servidor"),
          onTap: () {
          TextEditingController controlerTextMsg = TextEditingController();
            showDialog(context: context, builder: (context){
              return AlertDialog(
               title: Text("Qual mensagem você deseja enviar?"),
               content: TextField(controller: controlerTextMsg,),
               actions: [
                 Row(children: [
                   FlatButton(onPressed: (){
                     _enviaCmd(controlerTextMsg.text, 1);
                     Navigator.pop(context);
                   }, child: Text("Enviar")),
                   FlatButton(onPressed: (){
                     Navigator.pop(context);
                   }, child: Text("Cancelar"))
                 ],)
               ],

              );
            });
          })
      ],),


      //Coluna de baixo(Dinamicos)
      Column(children: [

      ],)
    ],),);
  }
}
