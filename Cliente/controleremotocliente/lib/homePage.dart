import 'package:controleremotocliente/models/ObjConectar.dart';
import 'package:shared_preferences/shared_preferences.dart' as Prefs;
import 'package:controleremotocliente/models/Comands.dart';
import 'package:controleremotocliente/models/Settings.dart';
import 'package:flutter/material.dart';
import 'package:fluttertoast/fluttertoast.dart';


class HomePage extends StatefulWidget {
  @override
  _HomePageState createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  int _index = 0;
  List _pages = [
    Commands(),
    Settings()
  ];
  bool isOn = false;


  void verifica() async
  {
    TextEditingController controllerIP = TextEditingController();
    TextEditingController controllerPort = TextEditingController();
    TextEditingController controllerSenha = TextEditingController();

    var prefs = await Prefs.SharedPreferences.getInstance();

      if(!prefs.containsKey("HOST") || !prefs.containsKey("PORT") ||
      !prefs.containsKey("PASS")){
        showDialog(context: context, builder: (context)
        {
          return AlertDialog(
            title: Text("Configurar"),
            content: Column(children: [
              TextField(controller: controllerIP ,
                decoration: InputDecoration(labelText: "IP"),),
              TextField(controller: controllerPort ,
                decoration: InputDecoration(labelText: "Porta"),),
              TextField(controller: controllerSenha ,
                decoration: InputDecoration(labelText: "Senha"),),
            ],),
          actions: [
            Row(children: [
              FlatButton(onPressed: (){
                if(controllerIP.text.isNotEmpty
                || controllerPort.text.isNotEmpty
                || controllerSenha.text.isNotEmpty){
                  prefs.setString("HOST", controllerIP.text);
                  prefs.setInt("PORT", int.parse(controllerPort.text));
                  prefs.setString("PASS", controllerSenha.text);
                  Navigator.pop(context);
                  Fluttertoast.showToast(msg: "Salvo");
                }else{
                  Fluttertoast.showToast(msg: "Algo ficou em branco");
                }
              }, child: Text("SALVAR")),
              FlatButton(onPressed: (){
                Navigator.pop(context);
                Fluttertoast.showToast(msg:
                "Você cancelou as configuracoes primarias, "
                    "você pode usar a aba configs para continuar por lá");
              }, child: Text("CANCELAR"))
            ],)
          ],);
        });
      }
  }
  @override
  Widget build(BuildContext context) {
    verifica();
    return Scaffold(
      appBar: AppBar(
        title: Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
          Text("Controle Remoto Cliente"),
          IconButton(icon: isOn == false ? Icon(Icons.play_arrow) :
              Icon(Icons.stop), onPressed: () async{
           bool conectado = await Connect.conectar(0);
            setState(() {
              isOn = conectado;
            });
            print(isOn);
          })
        ],),
      ),
      body: _pages[_index],
      bottomNavigationBar: BottomNavigationBar(
        currentIndex: _index,
        onTap: (index){
          setState(() {
            _index = index;
          });
        },
        items: [
          BottomNavigationBarItem(icon: Icon(Icons.home),
              label: "HOME"),
          BottomNavigationBarItem(icon: Icon(Icons.settings),
              label: "CONFIGURAR")
        ],
      )
    );
  }
}
