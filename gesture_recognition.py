import cv2
import mediapipe as mp

import csv
import copy
import itertools
import autopy

import os # для запуска приложения
import pyautogui # для снимка экрана
from datetime import datetime # для указания даты и времени при сохранении файлов
import time

from model.points_classifier.keypoint_classifier import KeyPointClassifier

# Глобальные переменные для хранения времени последнего выполнения действий
last_screenshot_time = 0
last_photo_time = 0
action_interval = 5  # Минимальный интервал между действиями в секундах

def main():
    # Инициализация
    hands = mp.solutions.hands.Hands( # Ключевые точки рук
        static_image_mode = False,
        max_num_hands = 1,
        min_tracking_confidence = 0.5,
        min_detection_confidence = 0.5
    )
    mp_draw = mp.solutions.drawing_utils # Для прорисовки на видео

    # Камера
    cap = cv2.VideoCapture(0) # Захват видео
    cap.set(cv2.CAP_PROP_FRAME_WIDTH, 640)   # Ширина
    cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)  # Высота

    # Классификатор жестов рук
    hand_sign_classifier = None

    width, height = autopy.screen.size() # Размер экрана

    # Чтение названий классов классификации
    with open('model/points_classifier/classifier_labels.csv', encoding='utf-8-sig') as f:
        classifier_labels = csv.reader(f)
        classifier_labels = [ # Пробегаемся по каждой строке файла и считываем первый элемент
            row[0] for row in classifier_labels
        ]

    # Режим работы программы (0 - распознавание, 1 - запись датасета)
    mode = int(input("Enter mode (0 - classification, 1 - logging dataset): "))
    if mode == 0:
        hand_sign_classifier = KeyPointClassifier()

    while cap.isOpened(): # Выполняем пока работает захват камеры
        key = cv2.waitKey(1)
        if key == 27: # Esc
            break;
        number = key - 48

        success, frame = cap.read()
        if not success:
            break;
        
        frame = cv2.flip(frame,1)
        debug_frame = copy.deepcopy(frame)

        result = hands.process(frame)

        if result.multi_hand_landmarks:
            for hand_landmarks, handedness in zip(result.multi_hand_landmarks,
                                                  result.multi_handedness):
                # Подсчёт координат руки относительно фрейма
                landmark_list = calculate_landmark_list(debug_frame, hand_landmarks)

                # Преобразование в относительные координаты (не зависящие от фрейма,а от кисти - точки 0)
                pre_processed_landmark_list = pre_process_landmark(landmark_list)

                # Если включен режим записи датасета, то заполняем датасет
                if mode == 1:
                    write_csv(number, pre_processed_landmark_list)
                    # Вывод информации
                    debug_frame = draw_logging_dataset_info(debug_frame, number)
                elif mode == 0: # Иначе классификация жестов рук
                    hand_sign_id = hand_sign_classifier(pre_processed_landmark_list)

                    # Выполнение действия в зависимости от жеста
                    action_handler(hand_sign_id, debug_frame)

                    # Вывод информации
                    debug_frame = draw_classification_info(debug_frame, handedness, classifier_labels[hand_sign_id])
            
            # Отображение точек руки
            mp_draw.draw_landmarks(debug_frame, result.multi_hand_landmarks[0], mp.solutions.hands.HAND_CONNECTIONS)

        cv2.imshow('Gesture Recognition', debug_frame)

    cap.release()
    cv2.destroyAllWindows()

# Функция получения координат точек рук (x,y) относительно фрейма
def calculate_landmark_list(frame, landmarks):
    frame_width, frame_height = frame.shape[1], frame.shape[0]

    landmark_point = []

    for _, landmark in enumerate(landmarks.landmark):
        landmark_x = min(int(landmark.x * frame_width), frame_width - 1)
        landmark_y = min(int(landmark.y * frame_height), frame_height - 1)

        landmark_point.append([landmark_x, landmark_y])

    return landmark_point

# Функция получения относительных координат (относительно запастья - точки 0)
def pre_process_landmark(landmark_list):
    temp_landmark_list = copy.deepcopy(landmark_list)

    x, y = 0, 0
    for i, landmark_point in enumerate(temp_landmark_list):
        if i == 0:
            x, y = landmark_point[0], landmark_point[1] # Получение x и y точки 0

        # Вычисление остальных точек отсносительно запастья
        temp_landmark_list[i][0] = temp_landmark_list[i][0] - x
        temp_landmark_list[i][1] = temp_landmark_list[i][1] - y

    # Преобразование в одномерный список
    temp_landmark_list = list(
        itertools.chain.from_iterable(temp_landmark_list)
    )

    # Максимальное число координат для нормализации
    max_value = max(list(map(abs, temp_landmark_list)))

    # Функция нормализации
    def normalize(n):
        return n / max_value
    
    # Получаем списоок нормализированных значений
    temp_landmark_list = list(map(normalize, temp_landmark_list))

    return temp_landmark_list

def select_mode(key, mode):
    number = -1
    if 48 <= key <= 57:  # 0 ~ 9
        number = key - 48
    if key == 110:  # n
        mode = 0
    if key == 107:  # k
        mode = 1
    if key == 104:  # h
        mode = 2
    return number, mode

def write_csv(number, landmark_list):
    if 0 <= number <= 9: # Режим записи датасета и задание индекса для жеста (number)
        csv_path = 'model\points_classifier\keypoint.csv'
        with open(csv_path, 'a', newline="") as f:
            writer = csv.writer(f)
            writer.writerow([number, *landmark_list])
    
    return

def draw_logging_dataset_info(frame, number):
    cv2.putText(frame, "MODE: Logging Key Point", (10, 90),
                cv2.FONT_HERSHEY_SIMPLEX, 0.6, (255, 255, 255), 1,
                cv2.LINE_AA)
    if 0 <= number <= 9:
        cv2.putText(frame, "NUM:" + str(number), (10, 110),
                    cv2.FONT_HERSHEY_SIMPLEX, 0.6, (255, 255, 255), 1,
                    cv2.LINE_AA)
    
    return frame

def draw_classification_info(frame, handedness, hand_sign_text):
    info_text = handedness.classification[0].label[0:]
    if hand_sign_text != "":
        info_text = info_text + ':' + hand_sign_text
    cv2.putText(frame, info_text, (10, 60),
               cv2.FONT_HERSHEY_SIMPLEX, 1.0, (255, 255, 255), 1, cv2.LINE_AA)
    
    return frame

def action_handler(hand_sign_id, frame):
    global last_screenshot_time, last_photo_time

    current_time = time.time() # Текущее время в секундах

    if hand_sign_id == 0: # знак мира - снимок с камеры
        if current_time - last_photo_time >= action_interval:
            if frame is not None:
                timestamp = datetime.now().strftime("%H-%M-%S_%d-%m-%Y")

                cv2.imwrite(f'camera_snapshots/camera_snapshot_{timestamp}.jpg', frame)

                print(f'Camera snapshot is saved: camera_snaphsots/camera_snapshot_{timestamp}.jpg')

                last_photo_time = current_time
    elif hand_sign_id == 1: # знак ОК - снимок экрана
        if current_time - last_screenshot_time >= action_interval:
            timestamp = datetime.now().strftime("%H-%M-%S_%d-%m-%Y")

            screenshot = pyautogui.screenshot()
            screenshot.save(f'screenshots/screenshot_{timestamp}.png')

            print(f'Screenshot is saved: screenshots/screenshot_{timestamp}.png')

            last_screenshot_time = current_time
    elif hand_sign_id == 2:
        os.system('d:/university_projects/hello_world.txt')

        print('Opened: d:/university_projects/hello_world.txt')


if __name__ == '__main__':
    main()