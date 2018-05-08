# Ejemplo de conexión al api de PayPhone
Proyecto para demostrar el funcionamiento de la integracion al api de PayPhone desde ionic 2 y angular
# Como ejecutar el proyecto
Clonar este proyecto o descargarse como zip
## Se debe instalar Ionic y Córdova
```
npm install -g ionic cordova
```
## Levantando el proyecto
Ejecutar el terminal de windows y ejecutar el siguiente comando 
```
npm install
```
Luego se debe ejecutar el siguiente comando
```
ionic serve --lab
```
# Ejecutando el proyecto en un dispositivo android
Inicialmente se deben instalar el jdk de java y el sdk de android, añadir la variable de entorno de android sdk.

*[Instal Android Studio](https://developer.android.com/studio/index.html)
*[Android SDK Tool](https://developer.android.com/studio/intro/update.html)

luego que se tiene correctamente configurado el ambiente para android se debe ejecutar el siguiente comando con el telefono conectado a la computadora mediante un cable usb

```
ionic cordova run android --device
```
