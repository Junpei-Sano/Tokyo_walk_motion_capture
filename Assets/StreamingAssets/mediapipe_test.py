
from pickle import FALSE
import cv2
import mediapipe as mp 

''' 顔とポーズを取得 '''
# https://google.github.io/mediapipe/solutions/holistic
class mediapipe_sample(object):

    def __init__(self):
        # For webcam input:
        self.cap = cv2.VideoCapture(0)

        self.mp_drawing = mp.solutions.drawing_utils
        self.mp_drawing_styles = mp.solutions.drawing_styles
        self.mp_holistic = mp.solutions.holistic

        self.mp_drawing = mp.solutions.drawing_utils
        self.mesh_drawing_spec = self.mp_drawing.DrawingSpec(thickness=2,  color=(0,255,0))
        self.mark_drawing_spec = self.mp_drawing.DrawingSpec(thickness=3,  circle_radius=3, color=(0,0,255))

        self.holistic = self.mp_holistic.Holistic(
            min_detection_confidence=0.5,
            min_tracking_confidence=0.5)


    # 終了時に必ず呼び出し
    def close(self):
        self.holistic.close()    # withを使わない代わり？
        self.cap.release()
        cv2.destroyAllWindows()


    # with文が無くても動く？
    def loop(self):
        if self.cap.isOpened():
            success, image = self.cap.read()
            if not success:
                print("Ignoring empty camera frame.")
                # If loading a video, use 'break' instead of 'continue'.
                return

            # To improve performance, optionally mark the image as not writeable to
            # pass by reference.
            image.flags.writeable = False
            image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)

            results = self.holistic.process(image)

            # Draw landmark annotation on the image.
            image.flags.writeable = True
            image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)
        
            self.mp_drawing.draw_landmarks(    # 顔の特徴
                image,
                results.face_landmarks,
                self.mp_holistic.FACEMESH_CONTOURS,
                landmark_drawing_spec=None,
                connection_drawing_spec=self.mp_drawing_styles
                .get_default_face_mesh_contours_style())
            self.mp_drawing.draw_landmarks(    # ポーズの特徴
                image,
                results.pose_landmarks,
                self.mp_holistic.POSE_CONNECTIONS,
                landmark_drawing_spec=self.mp_drawing_styles
                .get_default_pose_landmarks_style())
        
            # 以下サンプルプログラムに追加
            self.mp_drawing.draw_landmarks(    # 顔
                image,
                results.face_landmarks,
                self.mp_holistic.FACEMESH_TESSELATION,
                landmark_drawing_spec=None,
                connection_drawing_spec=self.mp_drawing_styles
                .get_default_face_mesh_tesselation_style())

            self.mp_drawing.draw_landmarks(    # 左手
                image,
                results.left_hand_landmarks,
                self.mp_holistic.HAND_CONNECTIONS,
                landmark_drawing_spec = self.mark_drawing_spec,
                connection_drawing_spec = self.mesh_drawing_spec
            )
            self.mp_drawing.draw_landmarks(    # 右手
                image,
                results.right_hand_landmarks,
                self.mp_holistic.HAND_CONNECTIONS,
                landmark_drawing_spec = self.mark_drawing_spec,
                connection_drawing_spec = self.mesh_drawing_spec
            )
        
            # Flip the image horizontally for a selfie-view display.
            cv2.imshow('MediaPipe Holistic', cv2.flip(image, 1))

            x = results.pose_landmarks

            return x

        else:
            return None
