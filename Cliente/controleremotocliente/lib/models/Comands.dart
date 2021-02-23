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
  _enviaCmd(String comand)async{
    final prefs = await SharedPreferences.getInstance();
    Map<String, dynamic> sendMap = Map();

    sendMap["pass"] = prefs.getString("pass");
    sendMap["msg"] = comand;

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
                    try{
                      if(_controllerTxt.text.isNotEmpty){
                        String cmd = "shutdown -s -t ${_controllerTxt.text}";
                        _enviaCmd(cmd);
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
        )
      ],),


      //Coluna de baixo(Dinamicos)
      Column(children: [

      ],)
    ],),);
  }
}
