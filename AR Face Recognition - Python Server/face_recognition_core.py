# -*- coding: utf-8 -*-
"""
Partie relative à la reconnaissance d'émotions à partir d'un flux d'images du 
projet de réalité virtuelle d'ET5 INFO | Polytech Paris-Sud.

Ce module effectue l'apprentissage statistiques à partir de datasets d'images 
obtenus via le module FaceAnalysis.py.

Ce module présente également la particularité d'avoir une partie destinée
exclusivement aux appels distants, et une partie dédiée à l'application
de réalité augmentée sur Webcam.

-- First published on git/anthwozn

Toute réutilisation du programme ci dessous requiert la mention de ses auteurs
dans l'en-tête du fichier.

@authors: BRINDAMOUR B., AZEMARD T., WOZNICA A., MBOURRA M. and SHI Y.
"""


# Librairies de traitement d'images
import cv2, glob

# Librairie de Machine Learning
from sklearn.svm import SVC
from sklearn.utils import shuffle

# Bibliothèques scientifiques
import numpy as np

# Librairies personalisées
import kairosrequest as kr
import FaceAnalysis as fa



# PARAMETRES D'AFFICHAGE DU TEXTE
font                   = cv2.FONT_HERSHEY_PLAIN 
fontScale              = 1
fontColor              = (0, 0, 255)
lineType               = 1

# Nombre de cycles de l'apprentissage statistique
epochs = 1

def load_set_(emotion):
    """
    Charge en mémoire les images relatives à une émotion.
    Ex: 'anger' chargera les images du dossier relaif à 'anger'.
    """
    files_ = glob.glob("dataset\\%s\\*" %emotion)
    train_, test_ = files_[:int(len(files_)*0.8)], files_[-int(len(files_)*0.2):]
    return train_, test_


        

def init():
    """
    Crée les datasets de test et d'entrainement.
    """
    
    X_train = []
    Y_train = []
    X_test  = []
    Y_test  = []
    
    clahe_ = cv2.createCLAHE(clipLimit=2.0, tileGridSize=(8,8))
    
    for emotion in fa.EMOTIONS:
        print("INFO: Processing", emotion)
        train_, test_ = load_set_(emotion)
        # Ajoute les données aux sets de test et d'entraînement, génère également les labels
        for item_ in train_:
            image_ = clahe_.apply(cv2.cvtColor(cv2.imread(item_), cv2.COLOR_BGR2GRAY))
            fa.landmarks_from_image_(image_)
            
            if fa.landmarks_set_['landmarks'] == "error":
                print("WARNING: no face detected !")
            else:
                X_train.append(fa.landmarks_set_['landmarks'])
                Y_train.append(fa.EMOTIONS.index(emotion))
                
        for item_ in test_:
            image_ = clahe_.apply(cv2.cvtColor(cv2.imread(item_), cv2.COLOR_BGR2GRAY))
            fa.landmarks_from_image_(image_)
            if fa.landmarks_set_['landmarks'] == "error":
                print("WARNING: no face detected !")
            else:
                X_test.append(fa.landmarks_set_['landmarks'])
                Y_test.append(fa.EMOTIONS.index(emotion))
                
    return X_train, Y_train, X_test, Y_test
   
    

def predict_format(predictions):
    """
    Met en forme les données
    """
    pred_list_ = predictions.tolist()
    output = "EMOTIONS:\n"
    for i_ in range(0,len(fa.EMOTIONS)):
        output += "\t | " + fa.EMOTIONS[i_].upper() + " : " +  " {:.1%}".format(pred_list_[0][i_]) + "\n"
        
    return output
  





# ============================================================
#           C O M M O N
# ============================================================
# Cette partie du code effectue l'apprentissage automatique à l'aide d'une SVM
    
clf_ = SVC(kernel='linear', probability=True, tol=1e-3) # Classifieur pour la reconnaissance

# Apprentissage
precision = []
X_train, Y_train, X_test, Y_test = init()

# On effectue 5 ré-itérations (on mélange les sets puis on les re-parse).
# Cela à pour effet de renforcer la prédiction de la SVM, mais présente 
# des risques accrus d'overfitting puisque l'on refait circuler à peu
# de choses près les mêmes données.
for epoch in range(0, epochs):
    print("epoch = ", epoch) 
    print("Training....")  
    
    # Fusion des sets, re-split et re-fit du classifieur
    X_set_ = X_train + X_test
    Y_set_ = Y_train + Y_test
    X_set_, Y_set_ = shuffle(X_set_, Y_set_)
    X_train, X_test = X_set_[:int(len(X_set_)*0.8)], X_set_[-int(len(X_set_)*0.2):]
    Y_train, Y_test = Y_set_[:int(len(Y_set_)*0.8)], Y_set_[-int(len(Y_set_)*0.2):]
    
    clf_.fit(np.array(X_train), Y_train)
    pred_ = np.array(X_test)
    prec_iter_ = clf_.score(pred_, Y_test)
    p = clf_.predict_proba([X_test[0]])
    print("Success rate: ", prec_iter_)
    precision.append(prec_iter_)
    
print("Average success rate:", np.mean(precision))
    


# ============================================================
#           S E R V E R   O N L Y
# ============================================================  
#
#   La fonction predict doit être appelée par le serveur
if __name__ == 'face_recognition_core':
    print("SERVER SCRIPT")

    def predict(frame_):

        image_, coords_ = fa.detect_faces(frame_)
        
        if(coords_ != []):
            fa.landmarks_from_image_(image_)
            
            if fa.landmarks_set_['landmarks'] == "error":
                print("WARNING: Couldn't place landmarks !")
            else:
                pred = clf_.predict_proba([fa.landmarks_set_['landmarks']])
                return(predict_format(pred), coords_)
        else:
            return("", [])



    
# ================================================================
# ================================================================
#               S T A N D A L O N E   P A R T
# ================================================================
# ================================================================
#   the part below is only used for testing purposes. It is NOT
#   executed while calling the import statement, through it is
#   still called when script is executed by itself.
if __name__ == '__main__':
    
    print("STANDALONE SCRIPT")
    
    # ============================================================
    #           W E B C A M   O N L Y
    # ============================================================
    capture_  = cv2.VideoCapture(0)
    clahe_ = cv2.createCLAHE(clipLimit=2.0, tileGridSize=(8,8))
    frame_no_ = 0
    displayedtext_ = []

    input("INPUT: Press Enter to start capture")
    print("INFO: Starting capture")
    
    
    while 1:
        ret_, frame_ = capture_.read()
        
        gray_ = cv2.cvtColor(frame_, cv2.COLOR_BGR2GRAY)
        clahe = cv2.createCLAHE(clipLimit=2.0, tileGridSize=(8,8))
        clahe_image = clahe.apply(gray_)
        
        # Update toutes les frames (ajustable)
        if(frame_no_ % 1 == 0): 
            image_, coords_ = fa.detect_faces(frame_)

            # Si un visage a été détecté
            if(len(image_) > 0):
                cv2.rectangle(frame_, (coords_[0], coords_[1]),
                      (coords_[0] + coords_[2], 
                       coords_[1] + coords_[3]), 
                       (0,0,255), thickness=2)
                 
    
                for j in range(0, len(displayedtext_)):
                    cv2.putText(frame_, displayedtext_[j], 
                        (coords_[0] + coords_[2] + 10, coords_[1] + 19* j + 10), 
                        font, 
                        fontScale,
                        fontColor,
                        lineType)
                
                if(frame_no_ % 10 == 0):                 
                    fa.landmarks_from_image_(image_)
                    
                    if fa.landmarks_set_['landmarks'] == "error":
                        print("WARNING: Couldn't place landmarks !")
                    else:
                        pred = clf_.predict_proba([fa.landmarks_set_['landmarks']])
                        displayedtext_ = kr.infos_format(pred, fa.EMOTIONS)
                        print(predict_format(pred))
            
                        # On limite les appels aux serveurs distants !
                        if(frame_no_ % 300 == 0 or displayedtext_ == [] or kr.INFOS['Age'] == 0):
                            kr.request_infos(image_)
                            displayedtext_ = kr.infos_format(pred, fa.EMOTIONS)
                 

    
        if cv2.waitKey(1) & 0xFF == ord('q'): #Exit program when the user presses 'q'
            capture_.release()
            break
        
        cv2.imshow("image", frame_)
        frame_no_ += 1
        
    cv2.destroyAllWindows()
