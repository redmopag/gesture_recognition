import csv
import numpy as np
import tensorflow as tf
from sklearn.model_selection import train_test_split

RANDOM_SEED = 42

# Настройка путей чтения данных и сохарнения модели
dataset = 'model\points_classifier\keypoint.csv'
model_save_path = 'model/points_classifier/keypoint_classifier.hdf5'
tflite_save_path = 'model/points_classifier/keypoint_classifier.tflite'

# Кол-во классов классификации
NUM_CLASSES = 5

# Чтение датасета
X_dataset = np.loadtxt(dataset, delimiter=',', dtype='float32', usecols=list(range(1, (21 * 2) + 1)))
y_dataset = np.loadtxt(dataset, delimiter=',', dtype='int32', usecols=(0))

# Разбиение датасета на обучающую и тестирующую выборку
X_train, X_test, y_train, y_test = train_test_split(X_dataset, y_dataset, train_size=0.75, random_state=RANDOM_SEED)

# Построение модели
model = tf.keras.models.Sequential([
    tf.keras.layers.Input((21 * 2, )),
    # Первый слой: 64 нейрона
    tf.keras.layers.Dense(64, activation='relu', kernel_regularizer=tf.keras.regularizers.l2(0.01)),
    tf.keras.layers.Dropout(0.3),
    # Второй слой: 32
    tf.keras.layers.Dense(32, activation='relu', kernel_regularizer=tf.keras.regularizers.l2(0.01)),
    tf.keras.layers.Dropout(0.3),
    # Третий слой: 16
    tf.keras.layers.Dense(16, activation='relu', kernel_regularizer=tf.keras.regularizers.l2(0.01)),
    # Четвёртый слой: NUM_CLASSES
    tf.keras.layers.Dense(NUM_CLASSES, activation='softmax')
])
model.summary()
# Обратный вызов контрольной точки модели
cp_callback = tf.keras.callbacks.ModelCheckpoint(
    model_save_path, verbose=1, save_weights_only=False)
# Обратный вызов для ранней остановки
es_callback = tf.keras.callbacks.EarlyStopping(patience=20, verbose=1)
# Компиляция модели
model.compile(
    optimizer='adam',
    loss='sparse_categorical_crossentropy',
    metrics=['accuracy']
)

# Тренировка модели
hisroty = model.fit(
    X_train,
    y_train,
    epochs=100,
    batch_size=128,
    validation_data=(X_test, y_test),
    callbacks=[cp_callback, es_callback]
)

# Оценка модели
val_loss, val_acc = model.evaluate(X_test, y_test, batch_size=128)
print(f"Loss: {val_loss}")
print(f"Accuracy: {val_acc}")

import matplotlib.pyplot as plt

# Извлечение потерь и точности из истории обучения
train_loss = hisroty.history['loss']
val_loss = hisroty.history['val_loss']
train_accuracy = hisroty.history['accuracy']
val_accuracy = hisroty.history['val_accuracy']

# Построение графика потерь
plt.figure(figsize=(12, 5))

# Подграфик для потерь
plt.subplot(1, 2, 1)
plt.plot(train_loss, label='Train Loss')
plt.plot(val_loss, label='Validation Loss')
plt.xlabel('Epochs')
plt.ylabel('Loss')
plt.title('Training and Validation Loss')
plt.legend()

# Подграфик для точности
plt.subplot(1, 2, 2)
plt.plot(train_accuracy, label='Train Accuracy')
plt.plot(val_accuracy, label='Validation Accuracy')
plt.xlabel('Epochs')
plt.ylabel('Accuracy')
plt.title('Training and Validation Accuracy')
plt.legend()

# Отображение графиков
plt.tight_layout()
plt.show()


# Конвертирование модели для Tensorflow-Lite
converter = tf.lite.TFLiteConverter.from_keras_model(model)
converter.optimizations = [tf.lite.Optimize.DEFAULT]
tflite_quantized_model = converter.convert()

open(tflite_save_path, 'wb').write(tflite_quantized_model)