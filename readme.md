# Ejemplo de conexión al api de PayPhone
Proyecto para demostrar el funcionamiento de la integracion al api de PayPhone empleando el 
# Como ejecutar el proyecto
Clonar este proyecto o descargarse como zip, compilar para que se descargue los paquetes que se emplean
## Levantando el proyecto
oprimir F5 

# Como configurar
Antes de ejecutar el proyecto se debe poner las configuraciones de su aplicación

El ejemplo le pide el Client Id y el key secret además del RUC de la empresa con la que su aplicación esta vinculada.
Debe abrir la pagina https://appdeveloper.payphonetodoesposible.com e ingresar sus credenciales de developer.
Estando dentro de la página en los detalles de la aplicacion puede encontrar el Id Cliente y la Clave Secreta, debe configurarla en la clase Configuration.cs en la siguiente sección:

``
public const string ClientId = "<client id of your application, you can get this in developer page>";
public const string KeySecret = "<key secret of your application, you can get this in developer page>";
``
El ruc de la empresa con la que esta vinculada su aplicación lo puede encontrar en el menú "Empresas" y debe configurarlo en la siguiente sección

``
public const string Ruc = "<Company unique code ralated with your application>";
``
Si no tiene como generar un token válido no se preocupe que la aplicación de ejemplo con las credenciales ingresadas es capaz de obtener uno. De tener un token válido lo debe configurar en la siguiente sección:

``
return _token ?? "<default token, you can get this in developer page>";
``

# Como emplear el ejemplo
La consola muestra las opciones que uste puede realizar.
Para crear un pago con el número de teléfono debe oprimir la opcion 1, si desea crearlo mediante la cédula debe oprimir la opción 3
El ejemplo mostrara el listado de países que puede seleccionar y le pedirá que ingrese el código de región del país que se lista al lado de cada uno. Un vez ingresado oprima "Enter"
Luego se solicita el número de teléfono o de cédula dependiendo que opción escogió, una vez ingresado oprima "Enter"

## Obtener el estado de la transacción
Si el telefono o la cedula fueron correctos la transacción es generada y enviada al cliente para que este realice el pago. El ejemplo muestra un listado de nuevas opciones.
Las opciones 1 y 2 permiten obtener el estado de la transacción actual.
Si la transacción ha sido aprobada o cancelada el sistema mostrará los detalles de la misma
