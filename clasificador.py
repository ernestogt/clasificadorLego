import numpy as np
import tensorflow as tf
from tensorflow.keras import layers
import matplotlib.pyplot as plt
from PIL import Image
import numpy as np
from tensorflow.keras import regularizers
from tensorflow.keras.callbacks import ModelCheckpoint, EarlyStopping, ReduceLROnPlateau
from tensorflow.keras import regularizers
from tensorflow.keras.preprocessing import image
import pathlib

import splitfolders

input_folder="Images"
splitfolders.ratio(input_folder, output="Data2", seed=1337, ratio=(.8, .1, .1))

width_shape = 224
height_shape = 224
num_classes = 10
epochs = 50
batch_size = 32 

image_size = 224
IMG_SHAPE = (image_size, image_size, 3)

base1_model = tf.keras.applications.MobileNetV2(input_shape=IMG_SHAPE, include_top=False, weights='imagenet')

for layer in base1_model.layers[:-26]:
    layer.trainable = False

filepath = "/content/drive/My Drive/model7.h5"
checkpoint = ModelCheckpoint(filepath, verbose=2, save_best_only=True)
reduce_lr = ReduceLROnPlateau(monitor='val_accuracy', factor=0.5, patience=2, verbose=2, mode='max', min_lr=0.00015)
train_datagen = tf.keras.preprocessing.image.ImageDataGenerator(
    rescale=1/255.0,
    rotation_range=20,
    width_shift_range=0.2,
    height_shift_range=0.2,
    shear_range=0.2,
    zoom_range=0.2,
    horizontal_flip=True,
    fill_mode='nearest'
)

valid_datagen = tf.keras.preprocessing.image.ImageDataGenerator(
    rescale=1/255.0
)

train_data = train_datagen.flow_from_directory(
    directory='/content/drive/MyDrive/Data/train/',
    target_size=(224, 224),
    class_mode='categorical',
    batch_size=64,
    seed=42
)

valid_data = valid_datagen.flow_from_directory(
    directory='/content/drive/MyDrive/Data/val/',
    target_size=(224, 224),
    class_mode='categorical',
    batch_size=64,
    seed=42
)

model_7 = tf.keras.Sequential([
    base1_model,
    tf.keras.layers.Conv2D(filters=32, kernel_size=(3, 3), activation='relu'),    
    tf.keras.layers.GlobalAveragePooling2D(),
    tf.keras.layers.BatchNormalization(),
    #tf.keras.layers.GlobalAveragePooling2D(),        
    tf.keras.layers.Dense(128, activation='relu',kernel_regularizer=regularizers.l1_l2(l1=1e-5, l2=1e-4),
    bias_regularizer=regularizers.l2(1e-4),
    activity_regularizer=regularizers.l2(1e-5)),
    tf.keras.layers.Dropout(rate=0.4),
    tf.keras.layers.Dense(128, activation='relu'),
    tf.keras.layers.Dropout(rate=0.4),
    tf.keras.layers.Dense(64),
    tf.keras.layers.Dropout(rate=0.3),
    tf.keras.layers.Dense(64),
    tf.keras.layers.Dropout(rate=0.3),
    tf.keras.layers.Dense(32),
    tf.keras.layers.Dropout(rate=0.3),
    tf.keras.layers.Dense(14, activation='softmax'),
])



model_7.compile(
    loss=tf.keras.losses.categorical_crossentropy,
    optimizer=tf.keras.optimizers.Adam(learning_rate=0.00015),
    metrics=['accuracy']
)

early_stopping=tf.keras.callbacks.EarlyStopping(monitor='val_loss',patience=3)


history_7 = model_7.fit(
    train_data,
    validation_data=valid_data,
    epochs=30,
    callbacks=[reduce_lr, early_stopping, checkpoint]    
)

def preparar_imagen(file):
    
    img = image.load_img(file, target_size=(224, 224))
    img_array = image.img_to_array(img)
    img_array_expanded_dims = np.expand_dims(img_array, axis=0)
    return tf.keras.applications.mobilenet.preprocess_input(img_array_expanded_dims)

def predecirDirectorio(path:str, numClass:int, model:tf.keras.models):
    num_total, num_correct = 0, 0
    for dir in pathlib.Path(path).iterdir():
        num_total += 1
        try:
            img = preparar_imagen(str(dir))
            pred = model.predict(img)
            pred = pred.argmax()
            if pred == numClass:
                num_correct += 1
        except Exception as e:
            print(dir)
            print(e)
        continue
    percentage=num_correct / num_total
    print("model porcentaje de aciertos de clase"+' {}: '.format(numClass)+ '{}'.format(percentage))

def predict(model:tf.keras.models):
      num=0
      for dir in pathlib.Path('/content/drive/MyDrive/Data2/test').iterdir():
            (dir,num,model)
            num+=1