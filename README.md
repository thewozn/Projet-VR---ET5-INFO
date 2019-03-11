# AUGMENTED REALITY FACE RECOGNITION

Implémentation d'une application d'affective computing en réalité augmentée usant d'un Web Service Python recevant des images et renvoyant la position du visage ainsi que les émotions associées.
Le projet est disponible en trois applications, à savoir en Local (Webcam), Vuforia (Android App) et HoloLens (UWP). Les deux dernières nécessitent l'usage du Web Service. Son déploiement est expliqué ci-dessous.

![image1](https://github.com/thewozn/Projet-VR---ET5-INFO/blob/master/Images/Capture d'écran 2019-03-11 23.27.34.png)

## Débuter
Afin de bénéficier pleinement de l'application, différentes dépendances sont à satisfaire pour le projet. Ces dernières sont explicitées avec la commande associée dans la section des Pré-Requis. La marche à suivre pour le lancement des applications est décrite dans les parties ultérieures spécifiques à chaque applications.

**NOTE**: Ces applications ont été développées indépendamment les unes des autres.


### Pré-requis:
#### Général
**D'autres versions peuvent être supportées mais le fonctionnement n'est pas garanti.**
* Python 3.5+ w/ scientific libraries (Anaconda est recommandé)
* OpenCV 2.4.3 `conda install -c menpo opencv=3.4.3`
* Dlib 19.4 `conda install -c conda-forge dlib=19.4`

#### Web Service
* Flask 0.12 `pip install Flask==0.12.2`

#### HoloLens
- Unity 2017.4.23f1

#### Vuforia:
- Android-sdk
- Unity (XXXXXX VERSION)


### Local
Le projet sur PC en local effectue la reconnaissance faciale en temps réel sur le PC via la WebCam.

#### Procédure
* Lancer le script face_recognition_core.py dans un éditeur Spyder ou en mode console. 

Suivant le nombre d'epochs, l'entraînement peut prendre plus ou moins longtemps. Par défaut, sa valeur est fixée à 1 pour des besoins de tests. Dans le cas pratique, il est recommandé de la fixer à 5 afin d'obtenir un score moyen davantage centré autour de 87%.

### Démarrage du Web Service
Afin de permettre la reconnaissance d'émotions sur support distant, il est nécessaire de démarrer le Web Service **server_back.py** ou **server_back_update.py** suivant le support.

Le guide de démarrage du Web Service sur réseau local est le suivant:
* **1)** Activer le partage de connexion dans Paramètres> Réseau et internet> Point d'accès sans fil mobile > Partager ma connexion Internet avec d'autres appareils. Cela aura pour effet de démarrer un réseau local sur le point d'accès Wifi accessible à tout appareil connecté au même réseau.
![image2](https://github.com/thewozn/Projet-VR---ET5-INFO/blob/master/Images/pacs.PNG)

* **2)** Vérifier en ligne de commande que le réseau a bien été créé. Cela est aisément vérifiable en entrant la commande ipconfig en ligne de commande.
![image3](https://github.com/thewozn/Projet-VR---ET5-INFO/blob/master/Images/ipconfig.png)

* **3)** Saisir dans server_back ou server_back_updated l'adresse de déploiement du serveur.

Si ces manipulations ne sont pas disponibles, il est également possible de se connecter à un serveur factice à l'adresse http://anthwozn.pythonanywhere.com. Ce dernier n'effectue cependant aucune opération et n'est utilisé qu'à des fins de test.

###Démarrage du projet sur HoloLens
**Cette partie requiert de lancer le Web Service pour profiter de ses fonctionnalités. **
Pour lancer le Web Service, se référer à la section éponyme. Le fichier **server_back_update.py** est utilisé.

**NOTE:** La suite des instructions exige un appareil HoloLens appairé avec le PC cible pour permettre le déploiement.

Le projet HoloLens est fourni sous forme de dossier Unity, et n'est donc pas exécutable en l'état. Pour build le projet, les manipulations suivantes sont requises:
* Aller dans Files > Build Settings et s'assurer que la configuration est la suivante:
![image4](https://github.com/thewozn/Projet-VR---ET5-INFO/blob/master/Images/config_hololens.PNG)

* Build le projet dans le dossier App (le créer si nécessaire)
* Dans App, ouvrir la solution générée Visual Studio (**2017** recommandé)
* La configuration de compilation est la suivante: **Debug > x86 > Distant Computer**. Au premier déploiement, l'IP de la machine distante sera demandée. **Saisir l'IP de l'HoloLens**.
- Le projet ainsi déployé sera automatiquement lancé sur l'HoloLens à l'issue du déploiement.

Lors du clic, une photo est prise et envoyée au Web Service ainsi qu'à des API distantes. Les réponses sont interprétées et l'écran de l'HoloLens affiche le résultat de l'interprétation en réalité augmentée à l'aide de prefabs. Chaque nouveau clic regénère la scène vue.

Il est théoriquement (uniquement théoriquement) possible d'effectuer une reconnaissance en temps réel. Auquel cas, il suffirait de décommenter les lignes du code dans la fonction start() et commenter la partie relative au Server1. Cependant, l'implémentation souffre de quelques failles ne permettant pas l'envoi et la récéption en continu.


## Auteurs

* **Anthony WOZNICA**

* **Benjamin BRINDAMOUR**
* **Max MBOURRA**

* **Thomas AZEMARD**

* **Yao SHI**
