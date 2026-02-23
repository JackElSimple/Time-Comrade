# Time-Comrade


# Organización del proyecto (ES)

	GitHub
• Para subir carpetas vacías a un sistema de gestión de versiones basado en Git, se puede crear un fichero vacío .gitKeep en dicha carpeta 

• También hay que asegurarse de que se suben los ficheros .meta al repositorio, ya que contienen información importante sobre ficheros y directorios del juego, como por ejemplo, la configuración de importación de las texturas

	Commits
• En el mensaje de commit, explicar brevemente el objetivo de los cambios realizados

• Tipos más comunes de commits:
	feat: Añade una nueva característica 
	fix: Arregla un bug
	docs: Cambios en la documentación
	refactor: Refactorización del código (no afecta a la funcionalidad)
	
• Se deberá agregar la descripción de commit, empezando siempre por un verbo en imperativo, por ejemplo:
añade, crea, remueve, cambia, soluciona y la descripción que generemos.

	Ramas
• Una rama se crea como una copia perfecta de otra, pero a partir de ese momento cada una puede seguir caminos distintos

• Inicialmente sólo existe la rama main (o master)

• Se recomienda añadir una nueva funcionalidad creando una rama, trabajando en dicha rama y, una vez que está lista, fundir la rama con main

• Una vez que hemos acabado de hacer los cambios en la rama y 
estamos seguros de que funcionan, tenemos que llevar dichos cambios a main, para que formen parte del juego

• Puede ser que, mientras que hemos estado trabajando en la rama, 
main haya cambiado. En ese entonces hay que actualizar nuestra rama con los cambios de main (merge a nuestra rama), antes de volcarle los cambios (merge a main)

• El proceso para volcar los cambios a la rama main se gestiona mediante una pull request / merge request.

• La pull/merge request (PR ó MR) permite documentar y comentar los cambios que se van a realizar

• El resto de miembros del equipo puede hacer comentarios sobre dichos cambios

	Escena
• Cuando varias personas trabajan a la vez sobre la misma escena, el sistema de control de versiones puede tener problemas a la hora de integrar los cambios

• Es mejor dividir los niveles en varias escenas más pequeñas para reducir el riesgo de conflictos

• En tiempo de ejecución, el proyecto puede cargar escenas de forma aditiva:
	-SceneManager.LoadScene(int sceneBuildIndex, LoadSceneMode.Additive)	
• También se puede lanzar la carga en segundo plano:
	-SceneManager.LoadSceneAsync(int sceneBuildIndex, LoadSceneMode.Additive)

	Objetos
• Recuerda usar prefabs siempre que sea posible

• Organiza la escena para evitar que la jerarquía crezca demasiado

• Usa gameobjects vacíos para crear dicha organización

• Al crear objetos en tiempo de ejecución, aparecen en la raíz del grafo de escena. Si se crean muchos objetos, puede pasar que la escena se vuelva inmanejable.
	-Para manejarlo correctamente, al instanciar un objeto, se le puede indicar un padre(asegúrate que su Transform sea la identidad). Así se puede desplegar o replegar el padre para ver los objetos.
	
	Carpetas
• Cada fichero y carpeta en el proyecto tiene un fichero de texto con el mismo nombre y extensión .meta

• Los ficheros .meta son necesarios. Mantenlos junto a su fichero

• Sólo son necesarias las carpetas Assets, Packages y ProjectSettings.

• No guardar ficheros fuera de la carpeta de Assets

• El resto de carpetas se reconstruyen a partir de estas y no deberían almacenarse en el sistema de control de versiones.

• A la hora de mover ficheros entre carpetas, hacerlo siempre desde Unity, para que se mueva también el fichero .meta

•Separar las escenas de prueba en una carpeta separada
	-Y dentro, incluso se pueden separar en carpetas por autor
	
• Mantener los ficheros propios separados de los de terceros
	-Crear, por ejemplo, una carpeta ThirdParty para librerías o paquetes de assets externos
	
	Otros consejos
• Always playable: que el proyecto siempre sea jugable.

• Subir código a menudo, commits pequeños y continuados

• Prueba el juego tras la actualización y verifica que funciona

• Cuidado al subir sólo parte de los cambios, o no subir ficheros nuevos
