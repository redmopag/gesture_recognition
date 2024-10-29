import cv2
import mediapipe as mp
import autopy
import math

# Инициализация
hands = mp.solutions.hands.Hands( # Ключевые точки рук
    static_image_mode = False,
    max_num_hands = 1,
    min_tracking_confidence = 0.5,
    min_detection_confidence = 0.7
)
mp_draw = mp.solutions.drawing_utils

cap = cv2.VideoCapture(0) # Захват видео
cap.set(cv2.CAP_PROP_FRAME_WIDTH, 640)   # Ширина
cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)  # Высота

width, height = autopy.screen.size() # Размер экрана

def get_distance(x1, y1, x2, y2):
    return math.sqrt((x2-x1)**2 + (y2-y1)**2)

while cap.isOpened():
    success, frame = cap.read()
    if not success:
        break;
    
    frame = cv2.flip(frame,1)
    result = hands.process(frame)

    if result.multi_hand_landmarks:
        for lm in result.multi_hand_landmarks:
            h, w, _ = frame.shape
            
            index_x, index_y = int(lm.landmark[8].x * w), int(lm.landmark[8].y * h)
            thumb_x, thumb_y = int(lm.landmark[4].x * w), int(lm.landmark[4].y * h)
            middle_x, middle_y = int(lm.landmark[12].x * w), int(lm.landmark[12].y * h)

            autopy.mouse.move(index_x * width/w, index_y * height/h)
            cv2.circle(frame, (index_x,index_y), 25, (255,0,255), cv2.FILLED)

            if get_distance(thumb_x, thumb_y, middle_x, middle_y) < 25:
                autopy.mouse.click()

        mp_draw.draw_landmarks(frame, result.multi_hand_landmarks[0], mp.solutions.hands.HAND_CONNECTIONS)


    cv2.imshow('Gesture Recognition', frame)
    if cv2.waitKey(1) & 0xFF == 27:
        break;

cap.release()
cv2.destroyAllWindows()