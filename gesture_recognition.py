from flask import Flask, request, jsonify
import cv2
import mediapipe as mp
import numpy as np
import base64
import copy
import csv
import itertools
from model.points_classifier.keypoint_classifier import KeyPointClassifier

# Инициализация Flask
app = Flask(__name__)

# Инициализация MediaPipe
hands = mp.solutions.hands.Hands(
    static_image_mode=False,
    max_num_hands=1,
    min_tracking_confidence=0.5,
    min_detection_confidence=0.5
)
mp_draw = mp.solutions.drawing_utils

# Инициализация классификатора жестов рук
hand_sign_classifier = KeyPointClassifier()

# Чтение названий классов классификации
with open('model/points_classifier/classifier_labels.csv', encoding='utf-8-sig') as f:
    classifier_labels = [row[0] for row in csv.reader(f)]

# Функция обработки кадра и классификации
def process_frame(frame):
    frame = cv2.flip(frame, 1)
    result = hands.process(frame)

    if result.multi_hand_landmarks:
        for hand_landmarks, handedness in zip(result.multi_hand_landmarks, result.multi_handedness):
            # Подсчёт координат руки относительно фрейма
            landmark_list = calculate_landmark_list(frame, hand_landmarks)
            pre_processed_landmark_list = pre_process_landmark(landmark_list)

            # Классификация жеста
            hand_sign_id = hand_sign_classifier(pre_processed_landmark_list)
            gesture_name = classifier_labels[hand_sign_id]
            return gesture_name
    return "No gesture detected"

def process_logging_dataset(frame, number):
    frame = cv2.flip(frame, 1)
    result = hands.process(frame)

    if result.multi_hand_landmarks:
        for hand_landmarks, handedness in zip(result.multi_hand_landmarks, result.multi_handedness):
            # Подсчёт координат руки относительно фрейма
            landmark_list = calculate_landmark_list(frame, hand_landmarks)
            pre_processed_landmark_list = pre_process_landmark(landmark_list)

            write_csv(number, pre_processed_landmark_list)

            return "saved"
        
    return "not_saved"


def write_csv(number, landmark_list):
    if 0 <= number <= 9: # Режим записи датасета и задание индекса для жеста (number)
        csv_path = 'model\points_classifier\keypoint.csv'
        with open(csv_path, 'a', newline="") as f:
            writer = csv.writer(f)
            writer.writerow([number, *landmark_list])
    
    return

# Функции для обработки ключевых точек
def calculate_landmark_list(frame, landmarks):
    frame_width, frame_height = frame.shape[1], frame.shape[0]
    landmark_point = []

    for _, landmark in enumerate(landmarks.landmark):
        landmark_x = min(int(landmark.x * frame_width), frame_width - 1)
        landmark_y = min(int(landmark.y * frame_height), frame_height - 1)
        landmark_point.append([landmark_x, landmark_y])

    return landmark_point

def pre_process_landmark(landmark_list):
    temp_landmark_list = copy.deepcopy(landmark_list)
    x, y = temp_landmark_list[0][0], temp_landmark_list[0][1]

    for i, landmark_point in enumerate(temp_landmark_list):
        temp_landmark_list[i][0] -= x
        temp_landmark_list[i][1] -= y

    temp_landmark_list = list(itertools.chain.from_iterable(temp_landmark_list))
    max_value = max(map(abs, temp_landmark_list))
    temp_landmark_list = [n / max_value for n in temp_landmark_list]

    return temp_landmark_list

# Маршрут для обработки REST-запросов
@app.route('/classify_frame', methods=['POST'])
def classify_frame():
    data = request.get_json()
    if 'frame' not in data:
        return jsonify({'error': 'No frame provided'}), 400

    # Декодирование изображения из base64
    frame_data = base64.b64decode(data['frame'])
    np_frame = np.frombuffer(frame_data, np.uint8)
    frame = cv2.imdecode(np_frame, cv2.IMREAD_COLOR)

    # Классификация кадра
    gesture_name = process_frame(frame)
    return jsonify({'gesture': gesture_name})

@app.route('/log_dataset', methods=['POST'])
def log_dataset():
    data = request.get_json()
    if 'frame' not in data or 'number' not in data:
        return jsonify({'error': 'Frame or number not provided'}), 400

    # Декодирование изображения из base64
    frame_data = base64.b64decode(data['frame'])
    np_frame = np.frombuffer(frame_data, np.uint8)
    frame = cv2.imdecode(np_frame, cv2.IMREAD_COLOR)

    # Получение номера для записи
    try:
        number = int(data['number'])
    except ValueError:
        return jsonify({'error': 'Invalid number provided'}), 400

    # Запись кадра в CSV
    status = process_logging_dataset(frame, number)
    return jsonify({'status': status})

# Запуск сервера
if __name__ == '__main__':
    print('Host is running at port: 5000')
    app.run(host='0.0.0.0', port=5000)