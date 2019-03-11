# -*- coding: utf-8 -*-
"""
Partie relative à la reconnaissance d'émotions à partir d'un flux d'images du 
projet de réalité virtuelle d'ET5 INFO | Polytech Paris-Sud.

Ce module concerne la récupération d'informations sur serveur distant.

-- First published on git/anthwozn

Toute réutilisation du programme ci dessous requiert la mention de ses auteurs
dans l'en-tête du fichier.

@authors: BRINDAMOUR B., AZEMARD T., WOZNICA A., MBOURRA M. and SHI Y.
"""



# Librairies de requetes web pour les services Kairos
import requests, json, cv2

# INFORMATIONS
INFOS    = {"Age": 0,
            "Gender": "",
            "Ethnicity": "",
            "Glass": False,
            "Skinstatus": {'acne': 0, 'dark_circle': 0, 'stain': 0, 'health': 0}
            }



def request_infos(image):
    """
    Effectue une requete vers FaceAPI/Kairos et initialise les données.
    A noter toutefois, ces requêtes sont limitées en nombre, et sont lentes
    à s'exécuter.
    """
    
    # On enregistre l'image dans un buffer
    cv2.imwrite("buffer.png", image)
    data = open("buffer.png", 'rb').read()
    
    # Envoi de la requête au serveur distant
    response = requests.post('http://faceapi.us.to/', data=data)
    
    # Récupération et mise en forme des données
    json_response_ = json.loads(response.text)
    INFOS['Age'] = json_response_[0]['fpp']['attributes']['age']['value']
    INFOS['Gender'] = json_response_[0]['fpp']['attributes']['gender']['value']
    INFOS['Ethnicity'] = json_response_[0]['fpp']['attributes']['ethnicity']['value']
    INFOS['Glass'] = json_response_[0]['fpp']['attributes']['glass']['value']
    INFOS['Skinstatus'] = json_response_[0]['fpp']['attributes']['skinstatus']



def infos_format(predictions, emotions):
    """
    Mise en forme des données
    """
    
    output = []
    output.append("Informations:")
    output.append("Age: " + str(INFOS['Age']))
    output.append("Gender: " + str(INFOS['Gender']))
    output.append("Ethnicity: " + str(INFOS['Ethnicity']))
    output.append("Glasses: " + str(INFOS['Glass']))
    output.append("Skinstatus: ")
    output.append("   |Acne: " + str(INFOS['Skinstatus']['acne']))
    output.append("   |Cernes: " + str(INFOS['Skinstatus']['dark_circle']))
    output.append("   |Stain: " + str(INFOS['Skinstatus']['stain']))
    output.append("   |Health: " + str(INFOS['Skinstatus']['health']))
    pred_list_ = predictions.tolist()
    output.append("EMOTIONS:")
    for i_ in range(0,len(emotions)):
        output.append("   |" + emotions[i_].upper() + " : " +  " {:.1%}".format(pred_list_[0][i_]))
    
    return output


def infos_format_serverside():
    """
    Mise en forme des données dans le cas d'un serveur distant
    """
    
    out = ""
    out += ("Informations:" + "\n")
    out += ("Age: " + str(INFOS['Age'])  + "\n")
    out += ("Gender: " + str(INFOS['Gender']) + "\n")
    out += ("Ethnicity: " + str(INFOS['Ethnicity']) + "\n")
    out += ("Glasses: " + str(INFOS['Glass']) + "\n")
    out += ("Skinstatus: " + "\n")
    out += ("   |Acne: " + str(INFOS['Skinstatus']['acne']) + "\n")
    out += ("   |Cernes: " + str(INFOS['Skinstatus']['dark_circle']) + "\n")
    out += ("   |Stain: " + str(INFOS['Skinstatus']['stain']) + "\n")
    out += ("   |Health: " + str(INFOS['Skinstatus']['health']) + "\n")
    return out