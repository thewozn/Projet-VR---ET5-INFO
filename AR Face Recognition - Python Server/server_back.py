# -*- coding: utf-8 -*-
"""
Partie relative à l'interface serveur du 
projet de réalité virtuelle d'ET5 INFO | Polytech Paris-Sud.

Une fois lancé, ce module fait office de Web Service pouvant
être appelé par différentes applications.

-- First published on git/anthwozn

Toute réutilisation du programme ci dessous requiert la mention de ses auteurs
dans l'en-tête du fichier.

@authors: BRINDAMOUR B., AZEMARD T., WOZNICA A., MBOURRA M. and SHI Y.
"""


from flask import Flask, request
import scipy.misc
import time
import json
import face_recognition_core


time.sleep(3)
app = Flask(__name__)

# INFORMATIONS A RENVOYER
faceinfo = {
        'emotion' : "",
        'x': 0,
        'y': 0,
        'w': 0,
        'h': 0
        
    }


HOST = "192.168.43.89"
PORT = 18001

# "PAGE PRINCIPALE" DU WEB SERVICE
@app.route('/')
def index():
    return '''<html>
    <body>
    
        <form action = "http://''' + HOST + ':' + PORT + '''/emotions_rec" method = "POST" 
            enctype = "multipart/form-data">
            <input type = "file" name = "file" />
            <input type = "submit"/>
        </form>
    </body>
    </html>'''


# Fonction appelée par le client
@app.route('/emotions_rec', methods=["POST"])
def emotions_rec():
    print("Request received !")
    print (request.headers)
    image = scipy.misc.imread(request.files['file'])
    preds, coords = face_recognition_core.predict(image)
    print(preds)
    print(coords)

    faceinfo["emotion"] = preds
    if(coords != []):
        faceinfo["x"] = int(coords[0])
        faceinfo["y"] = int(coords[1])
        faceinfo["w"] = int(coords[2])
        faceinfo["h"] = int(coords[3])
        
    return json.dumps(faceinfo)


if __name__ == '__main__':
    ## Remplacer l'adresse IP /!\
    app.run(host= HOST,port= PORT)
