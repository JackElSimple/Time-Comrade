# Time-Comrade 🎮

> **Language / Idioma / Język:**   [English](#english) | [Español](#español) | [Polski](#polish)

---

<a id="español"></a>
# 🇪🇸 Organización del Proyecto

## 🛠️ GitHub
* **Carpetas vacías:** Para subir carpetas vacías a un sistema de gestión de versiones basado en Git, se puede crear un fichero vacío `.gitKeep` en dicha carpeta.
* **Archivos .meta:** También hay que asegurarse de que se suben los ficheros `.meta` al repositorio, ya que contienen información importante sobre ficheros y directorios del juego, como por ejemplo, la configuración de importación de las texturas.

### 1. Convenciones y Estructura
* **Documentación:** Es fundamental seguir las convenciones de nombres y estructuras de directorio documentadas aquí para que todo el equipo trabaje en sintonía.
* **Nomenclatura:** **No usar espacios** en los nombres de ficheros y carpetas. Usar siempre **CamelCase** (ejemplo: `MiNuevoScript.cs` o `PersonajePrincipal`).
* **Ubicación:** No guardar ficheros fuera de la carpeta de `Assets`.

### 2. Commits
* **Mensaje:** En el mensaje de commit, explicar brevemente el objetivo de los cambios realizados. Usamos el formato "tipo: verbo imperativo + descripción".
* **Tipos comunes:**
    * `feat`: Añade una nueva característica.
    * `fix`: Arregla un bug.
    * `docs`: Cambios en la documentación.
    * `refactor`: Refactorización del código (no cambia el comportamiento).
* **Formato:** Se deberá agregar la descripción de commit, empezando siempre por un verbo en imperativo, por ejemplo: *añade, crea, remueve, cambia, soluciona* y la descripción que generemos.

### 3. Ramas (Branches)
* **Concepto:** Una rama se crea como una copia perfecta de otra, pero a partir de ese momento cada una puede seguir caminos distintos.
* **Main:** Inicialmente sólo existe la rama `main` (o master).
* **Flujo:** Se recomienda añadir una nueva funcionalidad creando una rama, trabajando en dicha rama y, una vez que está lista, fundir la rama con `main`.
* **Integración:** Una vez que hemos acabado de hacer los cambios en la rama y estamos seguros de que funcionan, tenemos que llevar dichos cambios a `main`, para que formen parte del juego.
* **Sincronización:** Puede ser que, mientras que hemos estado trabajando en la rama, `main` haya cambiado. En ese entonces hay que actualizar nuestra rama con los cambios de `main` (merge a nuestra rama), antes de volcarle los cambios (merge a `main`).
* **Pull Request:** El proceso para volcar los cambios a la rama `main` se gestiona mediante una pull request / merge request (PR ó MR).
* **Colaboración:** Permite documentar y comentar los cambios que se van a realizar, y el resto de miembros del equipo puede hacer comentarios.

## 🎬 Escena
* **Conflictos:** Cuando varias personas trabajan a la vez sobre la misma escena, el sistema de control de versiones puede tener problemas a la hora de integrar los cambios.
* **División:** Es mejor dividir los niveles en varias escenas más pequeñas para reducir el riesgo de conflictos.
* **Carga Aditiva:** En tiempo de ejecución, el proyecto puede cargar escenas de forma aditiva:
    * `SceneManager.LoadScene(int sceneBuildIndex, LoadSceneMode.Additive)`
* **Carga Asíncrona:** También se puede lanzar la carga en segundo plano:
    * `SceneManager.LoadSceneAsync(int sceneBuildIndex, LoadSceneMode.Additive)`

## 📦 Objetos
* **Prefabs:** Recuerda usar prefabs siempre que sea posible.
* **Organización:** Organiza la escena para evitar que la jerarquía crezca demasiado usando GameObjects vacíos.
<img width="181" height="203" alt="image" src="https://github.com/user-attachments/assets/ae97d2f9-ca74-4cd5-8595-c7e773301e51" />

* **Instanciación:** Al crear objetos en tiempo de ejecución, aparecen en la raíz del grafo de escena. Si se crean muchos objetos, la escena se vuelve inmanejable.
* **Padres:** Para manejarlo correctamente, al instanciar un objeto, se le puede indicar un padre (asegúrate que su Transform sea la identidad). Así se puede desplegar o replegar el padre para ver los objetos.
<img width="1058" height="294" alt="image" src="https://github.com/user-attachments/assets/332a2897-29fb-413d-90a2-12900c3691d0" />

## 📁 Carpetas
* **Ficheros .meta:** Cada fichero y carpeta tiene un fichero de texto con el mismo nombre y extensión `.meta`. Son necesarios; mantenlos junto a su fichero.
* **Estructura necesaria:** Sólo son necesarias las carpetas `Assets`, `Packages` y `ProjectSettings`.
* **Ubicación:** No guardar ficheros fuera de la carpeta de `Assets`.
* **Mover archivos:** Al mover ficheros entre carpetas, hacerlo siempre desde Unity, para que se mueva también el fichero `.meta`.
* **Escenas de prueba:** Separarlas en una carpeta aparte y, dentro, incluso se pueden separar en carpetas por autor.
* **Terceros:** Mantener los ficheros propios separados de los de terceros (ej: carpeta `ThirdParty`).

## 💡 Otros consejos
* **Always playable:** Que el proyecto siempre sea jugable.
* **Frecuencia:** Subir código a menudo, commits pequeños y continuados.
* **Pruebas:** Prueba el juego tras la actualización y verifica que funciona.
* **Atención:** Cuidado al subir sólo parte de los cambios, o no subir ficheros nuevos.

---

<a id="english"></a>
# 🇬🇧 Project Organization

## 🛠️ GitHub
* **Empty Folders:** To upload empty folders to a Git-based version control system, you can create an empty `.gitKeep` file in that folder.
* **.meta Files:** Ensure `.meta` files are uploaded to the repository, as they contain important information about game files and directories, such as texture import settings.

### 1. Conventions & Structure
* **Documentation:** Everyone must follow the established naming conventions and directory structures.
* **Naming:** **Do not use spaces** in file or folder names. Always use **CamelCase** (e.g., `MyNewScript.cs` or `MainCharacter`).
* **Location:** Never store files outside the `Assets` folder.

### 2. Commits
* **Message:** In the commit message, briefly explain the goal of the changes. We use the format "type: imperative verb + description".
* **Common Types:**
    * `feat`: Adds a new feature.
    * `fix`: Fixes a bug.
    * `docs`: Changes in documentation.
    * `refactor`: Code refactoring (does not change behavior).
* **Format:** The commit description should always start with an imperative verb (e.g., *add, create, remove, change, fix*).

### 3. Branches
* **Concept:** A branch is created as a perfect copy of another, but from that point on, each can follow different paths.
* **Main:** Initially, only the `main` (or master) branch exists.
* **Workflow:** It is recommended to add new functionality by creating a branch, working on it, and merging it with `main` once it's ready.
* **Integration:** Once changes are finished and verified, they must be merged into `main` to become part of the game.
* **Syncing:** If `main` changes while working on a branch, you must update your branch with those changes (merge from `main`) before merging your changes back into `main`.
* **Pull Request:** The process of merging changes into `main` is managed through a pull request / merge request (PR or MR).
* **Collaboration:** PRs allow documenting and discussing the changes, where other team members can provide feedback.

## 🎬 Scene
* **Conflicts:** When multiple people work on the same scene simultaneously, version control may have issues merging changes.
* **Splitting:** It is better to divide levels into smaller scenes to reduce the risk of conflicts.
* **Additive Loading:** At runtime, the project can load scenes additively:
    * `SceneManager.LoadScene(int sceneBuildIndex, LoadSceneMode.Additive)`
* **Async Loading:** Background loading is also possible:
    * `SceneManager.LoadSceneAsync(int sceneBuildIndex, LoadSceneMode.Additive)`

## 📦 Objects
* **Prefabs:** Remember to use prefabs whenever possible.
* **Organization:** Use empty GameObjects to organize the scene and prevent the hierarchy from becoming too large.
<img width="181" height="203" alt="image" src="https://github.com/user-attachments/assets/ae97d2f9-ca74-4cd5-8595-c7e773301e51" />

* **Instantiation:** Objects created at runtime appear at the scene root. If many are created, the scene becomes unmanageable.
* **Parenting:** To handle this correctly, assign a parent when instantiating (ensure its Transform is set to identity). This allows collapsing the parent to manage the view.
<img width="1058" height="294" alt="image" src="https://github.com/user-attachments/assets/332a2897-29fb-413d-90a2-12900c3691d0" />

## 📁 Folders
* **.meta Files:** Every file and folder has a `.meta` text file. They are necessary; keep them with their corresponding file.
* **Required Folders:** Only `Assets`, `Packages`, and `ProjectSettings` are necessary.
* **Location:** Do not save files outside the `Assets` folder.
* **Moving Files:** Always move files within Unity so the `.meta` file moves along with them.
* **Test Scenes:** Separate test scenes into a dedicated folder, which can be further organized by author.
* **Third Party:** Keep custom files separate from third-party assets (e.g., a `ThirdParty` folder).

## 💡 Other Tips
* **Always playable:** The project should always be in a playable state.
* **Frequency:** Upload code often with small, continuous commits.
* **Testing:** Playtest after updating to verify everything works.
* **Caution:** Be careful not to upload only partial changes or forget to include new files.

---

<a id="polish"></a>
# 🇵🇱 Organizacja Projektu

## 🛠️ GitHub
* **Puste foldery:** Aby przesłać puste foldery do systemu kontroli wersji opartego na Git, można utworzyć w danym folderze pusty plik `.gitKeep`.
* **Pliki .meta:** Upewnij się, że pliki `.meta` są przesyłane do repozytorium, ponieważ zawierają ważne informacje o plikach i katalogach gry, takie jak ustawienia importu tekstur.

### 1. Konwencje i struktura
* **Dokumentacja:** Wszyscy muszą przestrzegać ustalonych konwencji nazewnictwa oraz struktury katalogów.
* **Nazewnictwo:** **Nie używaj spacji** w nazwach plików ani folderów. Zawsze stosuj **CamelCase** (np. `MyNewScript.cs` lub `MainCharacter`).
* **Lokalizacja:** Nigdy nie przechowuj plików poza folderem `Assets`.

### 2. Commity
* **Wiadomość:** W wiadomości commita krótko wyjaśnij cel zmian. Używamy formatu „typ: czasownik w trybie rozkazującym + opis”.
* **Najczęstsze typy:**
    * `feat`: Dodaje nową funkcjonalność.
    * `fix`: Naprawia błąd.
    * `docs`: Zmiany w dokumentacji.
    * `refactor`: Refaktoryzacja kodu (nie zmienia zachowania).
* **Format:** Opis commita powinien zawsze zaczynać się od czasownika w trybie rozkazującym (np. *dodaj, utwórz, usuń, zmień, napraw*).

### 3. Gałęzie (Branches)
* **Koncepcja:** Gałąź jest tworzona jako dokładna kopia innej, ale od tego momentu każda może rozwijać się w innym kierunku.
* **Main:** Początkowo istnieje tylko gałąź `main` (lub `master`).
* **Workflow:** Zaleca się dodawanie nowej funkcjonalności poprzez utworzenie gałęzi, pracę na niej, a następnie połączenie jej z `main`, gdy będzie gotowa.
* **Integracja:** Po zakończeniu i zweryfikowaniu zmian należy scalić je z `main`, aby stały się częścią gry.
* **Synchronizacja:** Jeśli `main` ulegnie zmianie podczas pracy na gałęzi, należy zaktualizować swoją gałąź (scalić zmiany z `main`) przed ponownym scaleniem jej z `main`.
* **Pull Request:** Proces scalania zmian do `main` odbywa się za pomocą pull request / merge request (PR lub MR).
* **Współpraca:** PR umożliwia dokumentowanie i omawianie zmian, gdzie inni członkowie zespołu mogą przekazać opinię.

## 🎬 Sceny
* **Konflikty:** Gdy kilka osób pracuje jednocześnie nad tą samą sceną, system kontroli wersji może mieć problemy ze scalaniem zmian.
* **Podział:** Lepiej dzielić poziomy na mniejsze sceny, aby zmniejszyć ryzyko konfliktów.
* **Ładowanie addytywne:** W czasie działania projektu można ładować sceny addytywnie:
    * `SceneManager.LoadScene(int sceneBuildIndex, LoadSceneMode.Additive)`
* **Ładowanie asynchroniczne:** Możliwe jest także ładowanie w tle:
    * `SceneManager.LoadSceneAsync(int sceneBuildIndex, LoadSceneMode.Additive)`

## 📦 Obiekty
* **Prefabrykaty (Prefabs):** Pamiętaj, aby używać prefabów zawsze, gdy to możliwe.
* **Organizacja:** Używaj pustych obiektów GameObject do organizowania sceny i zapobiegania nadmiernemu rozrostowi hierarchii.

* **Instancjonowanie:** Obiekty tworzone w czasie działania pojawiają się w głównym katalogu sceny (root). Jeśli powstaje ich wiele, scena staje się nieczytelna.
* **Parenting:** Aby poprawnie tym zarządzać, przypisz obiekt nadrzędny (parent) podczas instancjonowania (upewnij się, że jego Transform ma ustawione wartości identity). Pozwala to zwijać obiekt nadrzędny i łatwiej zarządzać widokiem.

## 📁 Foldery
* **Pliki .meta:** Każdy plik i folder ma tekstowy plik `.meta`. Są one niezbędne — zachowuj je razem z odpowiadającymi im plikami.
* **Wymagane foldery:** Wymagane są tylko `Assets`, `Packages` oraz `ProjectSettings`.
* **Lokalizacja:** Nie zapisuj plików poza folderem `Assets`.
* **Przenoszenie plików:** Zawsze przenoś pliki wewnątrz Unity, aby plik `.meta` został przeniesiony razem z nimi.
* **Sceny testowe:** Oddziel sceny testowe do dedykowanego folderu, który można dodatkowo organizować według autora.
* **Zasoby zewnętrzne:** Przechowuj własne pliki oddzielnie od zasobów firm trzecich (np. folder `ThirdParty`).

## 💡 Inne wskazówki
* **Zawsze grywalne:** Projekt powinien zawsze znajdować się w stanie umożliwiającym uruchomienie gry.
* **Częstotliwość:** Wysyłaj kod często, wykonując małe i regularne commity.
* **Testowanie:** Po aktualizacji przetestuj grę, aby upewnić się, że wszystko działa poprawnie.
* **Ostrożność:** Uważaj, aby nie przesłać jedynie częściowych zmian ani nie zapomnieć dodać nowych plików.
