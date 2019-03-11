# -*- coding: utf-8 -*-
"""
Partie relative à la reconnaissance d'émotions à partir d'un flux d'images du 
projet de réalité virtuelle d'ET5 INFO | Polytech Paris-Sud.

Ce module concerne l'analyse de visage (detection, mapping).

-- First published on git/anthwozn

Toute réutilisation du programme ci dessous requiert la mention de ses auteurs
dans l'en-tête du fichier.

@authors: BRINDAMOUR B., AZEMARD T., WOZNICA A., MBOURRA M. and SHI Y.
"""

import cv2, dlib
import numpy as np
import math


# EMOTIONS RECONNUES
EMOTIONS = ["neutral", "anger", "disgust", "happy", "surprise"]


# FACE FEATURES
faceclasses_  = [cv2.CascadeClassifier("haarcascade_frontalface_default.xml"),
                 cv2.CascadeClassifier("haarcascade_frontalface_alt2.xml"),
                 cv2.CascadeClassifier("haarcascade_frontalface_alt.xml"),
                 cv2.CascadeClassifier("haarcascade_frontalface_alt_tree.xml")
                ]


# PREDICTEURS
detector_ = dlib.get_frontal_face_detector()
predictor_ = dlib.shape_predictor("shape_predictor_68_face_landmarks.dat") # Face feature landmarks


landmarks_set_ = {}



def landmarks_from_image_(image):
    """
    Retourne les landmarks des visages à partir d'une imagen (visage mappé)
    """
    landmarks_set_['landmarks'] = "error"
    
    # Detecte le visage
    detections = detector_(image, 1)
    
    # Parcourt tous les visages detectés
    for k,d in enumerate(detections):
        shape_ = predictor_(image, d) # Dessine les landmarks sur le visage
        landmarks_ = []               # Liste où on stock les landmarks
        X_, Y_ = [], []               # Stockage des coordonnées des points dans les deux listes
        
        for i in range(1,68): # Les 68 landmarks sont stockés dans ces deux listes
            X_.append(float(shape_.part(i).x))
            Y_.append(float(shape_.part(i).y))
            
        # Normalisation
        X_Norm, Y_Norm = [(x - np.mean(X_)) for x in X_], [(y - np.mean(Y_)) for y in Y_]
        
        for x, y, w, z in zip(X_Norm, Y_Norm, X_, Y_):
            landmarks_.append(w)    
            landmarks_.append(z)
            
            mean_   = np.asarray((np.mean(X_) ,np.mean(Y_)))
            coords_ = np.asarray((z,w))
            
            # Calcul de la distance entre les coordonnées et le barycentre
            dist = np.linalg.norm(coords_ - mean_)
            landmarks_.append(dist)
            landmarks_.append((math.atan2(y, x)*360)/(2*math.pi))
        
        # Solution TEMPORAIRE !! On ne stock qu'un seul visage ici !!!
        landmarks_set_['landmarks'] = landmarks_
        
        
        
        
        
def detect_faces(image):
    """
    Détecte les visages dans la scène.
    """
    grayscale_img_ = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    
    # Détection du visage
    facefeatures_ = ""
    for faceclass_ in faceclasses_:
        face_= faceclass_.detectMultiScale(grayscale_img_, scaleFactor=1.1, minNeighbors=10, minSize=(5, 5), flags=cv2.CASCADE_SCALE_IMAGE)
        if(len(face_) == 1):
            facefeatures_ = face_    
        
    # Rognage de l'image selon le visage
    # x, y, w, h sont les coordonnées du visage telles que:
    # (x, y) sont les coordonnées spatiales
    # (w, h) la largeur et la hauteur du visage (width, height)
    for (x, y, w, h) in facefeatures_: 
        print("INFO: Face found on the image")
        # On rogne l'image
        grayscale_img_ = grayscale_img_[y:y+h, x:x+w]
        try:
            out = cv2.resize(grayscale_img_, (350, 350)) # Redimension à l'échelle des données d'entraînement/de test
            return out, [x, y, w, h]
        except:
           pass
    return [], []