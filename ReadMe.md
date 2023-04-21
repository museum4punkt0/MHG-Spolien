# Auf der Spur der verschwundenen Stadt. Eine digitale Reise durch Raum und Zeit 

 Für Entwicklungszwecke, sowie allgemeine Verbesserung der Lesbarkeit, wird die Anwendung vorwiegend mit dem Arbeitstitel „MHG-Spolien“ statt dem vollen Namen „Auf der Spur der verschwundenen Stadt. Eine digitale Reise durch Raum und Zeit“ benannt. 

## Kurzbeschreibung 

Wie lassen sich das Museum und der städtische Raum verbinden? Wie können historische Baufragmente, sogenannte Spolien, Stadtgeschichte erzählen? Das Museum für Hamburgische Geschichte entwickelt eine App mit Augmented- Reality-Technologie, die Stadt und Museum zusammenführt. Mit der Methode des Site Specific Storytellings bietet das Tool Inhalte an spezifischen Orten im Museum und im Stadtraum an. Die am Museumsgebäude verbauten Spolien lassen sich digital mit ihren ursprünglichen Herkunftsgebäuden kontextualisieren und im Stadtraum verorten. 

 Diese Mobile App ist entstanden im Verbundprojekt museum4punkt0 – Digitale Strategien für das Museum der Zukunft, Teilprojekt [Auf der Spur der verschwundenen Stadt. Eine digitale Reise durch Raum und Zeit](https://www.museum4punkt0.de/teilprojekt/auf-der-spur-der-verschwundenen-stadt-eine-digitale-reise-durch-raum-und-zeit/). Das Projekt museum4punkt0 wird gefördert durch die Beauftragte der Bundesregierung für Kultur und Medien aufgrund eines Beschlusses des Deutschen Bundestages. [Weitere Informationen](www.museum4punkt0.de). 

 ![NeuStartKultur](https://github.com/museum4punkt0/media_storage/blob/e87f37973c3d91e2762d74d51bed81de5026e06e/BKM_Neustart_Kultur_Wortmarke_pos_RGB_RZ_web.jpg)  ![Beauftragte der Bundesregierung für Kultur und Medien](https://github.com/museum4punkt0/media_storage/blob/2c46af6cb625a2560f39b01ecb8c4c360733811c/BKM_Fz_2017_Web_de.gif) 

 ## Installation 

Die MHG-Spolien-App basiert auf der Unity-Engine, für die Bearbeitung des Projekts wird der Unity Editor benötigt. 
Um die Größe des Repositorys so gering wie möglich zu halten, ist das enthaltene Projekt stark reduziert. Einige Tool sowie Assets müssen separat als Package geladen werden.

 Für die Bearbeitung des Projektes empfehlen wir folgende Unity-Version: 

- [Unity 2021.3.6](https://unity.com/releases/editor/whats-new/2021.3.6) mit Buildsupport Android / iOS 

 

Das Projekt sollte sich ebenfalls mit neueren Unity-Versionen öffnen lassen, jedoch kann eine korrekte Funktionalität nicht sichergestellt werden. 

 

Beim ersten Öffnen des Projektes werden Fehler auftreten, da externe Tools sowie große Daten aus dem Projekt entfernt wurden um den Museum-4.0 Richtlinien / Upload-Beschränkungen zu entsprechen. Um diese Fehler zu beheben, müssen fehlenden Pakete installiert werden: 

- [Odin Inspector](https://odininspector.com/)  [(Installation über Custom Package)](https://docs.unity3d.com/Manual/AssetPackagesImport.html) 

- [PagedRect](https://assetstore.unity.com/packages/tools/gui/pagedrect-paging-galleries-and-menus-for-unity-ui-54552) (Installation über den Unity Asset Store / Package Manager) 

- [MHG Asset Pack](https://gitlab.jn.de/jn-public/mhg_spolien-additionaldata) [(Installation über Custom Package)](https://docs.unity3d.com/Manual/AssetPackagesImport.html) 

 

Nach einem Neustart des Unity-Editors ist das Projekt vollständig und bereit für die Nutzung. 

 

## Nutzung 

Die Spolienapp kann in dem Unity-Editor getestet, oder als App für Android / iOS gebuildet werden. 

 

### Unity Editor 

Um die Anwendung über den Unity-Editor zu starten muss die Bootstraper-Szene (Assets/01_Scenes/MHG_Bootstrapper.unity) geladen und ausgeführt werden. 

Bitte beachten: Die MHG-Spolienapp ist als Mobile-App im Portrait-Mode entwickelt und konzipiert. Im Unity Editor werden deswegen nicht alle Funktionen ausführbar sein. Es empfiehlt sich, die Zielauflösung des Unity-Editor-Game-Windows auf eine Portrait-Auflösung einzustellen. 

 

### Android 

Für einen Build auf Android sind die Build-Einstellungen bereits gesetzt. Über File > Build Settings (Ctrl+Shift+B) kann nun ein Build als App Package (APK) in ein Zielordner, oder auf ein verbundenes, kompatibles Android-Gerät gestartet werden.

 Soll die App direkt über "Build and Run" oder "Patch and Run" auf ein Android-Gerät deployed werden, muss auf diesem Gerät 

- mindestens Android 7.0 installiert sein 

- der Developer-Modus aktiv sein 

- USB-Debugging aktiviert sein. 

APKs können auf jedem Android-Gerät installiert, welches 

- mindestens auf Softwarelevel Android 7.0 ist 

- externe Quellen zulässt. 

 [Mehr Informationen](https://docs.unity3d.com/Manual/android-BuildProcess.html) 
 
### iOS 
Für ein Build auf iOS muss über die Build-Settings das Build-Target auf iOS gewechselt werden. Für das Deployment auf iOS-Geräten wird zwingend ein Computer mit MacOS und XCode benötigt. 

[Mehr Informationen](https://docs.unity3d.com/Manual/iphone-BuildProcess.html) 
## Credits 

![jangled-rgb](https://user-images.githubusercontent.com/101568996/233662078-c7a12e1d-1f50-401b-abd2-4d597ecb7b84.jpg)

 Konzeption und Produktion: jangled nerves GmbH im Auftrag des Museum für Hamburgische Geschichte 

 Projektleitung: Lucas.Froeschle@jn.de

## Lizenz 

Dieses Projekt ist unter der MIT Lizenz lizensiert. Mehr Informationen unter [Licence.md]() 

 
