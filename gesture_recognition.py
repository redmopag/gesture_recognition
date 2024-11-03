import cv2
import mediapipe as mp

import csv
import copy
import itertools
import autopy

from model.points_classifier.keypoint_classifier import KeyPointClassifier

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
    hand_sign_classifier = KeyPointClassifier()

    width, height = autopy.screen.size() # Размер экрана

    # Чтение названий классов классификации
    with open('model/points_classifier/classifier_labels.csv', encoding='utf-8-sig') as f:
        classifier_labels = csv.reader(f)
        classifier_labels = [ # Пробегаемся по каждой строке файла и считываем первый элемент
            row[0] for row in classifier_labels
        ]

    mode = 0 # Режим работы программы (0 - распознавание, 1 - запись датасета)

    while cap.isOpened(): # Выполняем пока работает захват камеры
        key = cv2.waitKey(1)
        if key == 27: # Esc
            break;
        number, mode = select_mode(key, mode)

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
                write_csv(number, mode, pre_processed_landmark_list)

                # Классификация жестов рук
                hand_sign_id = hand_sign_classifier(pre_processed_landmark_list)
                
                # Вывод информации
                debug_frame = draw_info_text(
                    debug_frame, handedness, classifier_labels[hand_sign_id]
                )
            
            # Отображение точек руки
            mp_draw.draw_landmarks(frame, result.multi_hand_landmarks[0], mp.solutions.hands.HAND_CONNECTIONS)

        cv2.imshow('Gesture Recognition', frame)

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

def write_csv(number, mode, landmark_list):
    if mode == 0:
        pass
    if mode == 1 and (0 <= number <= 9): # Режим записи датасета и задание индекса для жеста (number)
        csv_path = 'model/points_classifier/keypoint.csv'
        with open(csv_path, 'a', newline="") as f:
            writer = csv.writer(f)
            writer.writerow([number, *landmark_list])
    
    return

def draw_info_text(frame, handedness, hand_sign_text):
    info_text = handedness.classification[0].label[0:]
    if hand_sign_text != "":
        info_text = info_text + ':' + hand_sign_text
    cv2.putText(frame, info_text, (10, 60),
               cv2.FONT_HERSHEY_SIMPLEX, 1.0, (255, 255, 255), 1, cv2.LINE_AA)

if __name__ == '__main__':
    main()