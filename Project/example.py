from api import Detector
import cv2
import time

# Initialize detector
detector = Detector(model_name='rapid',
                    weights_path='./weights/pL1_MWHB1024_Mar11_4000.ckpt',
                    use_cuda=False)

for i in range(0,350):
    time.sleep(30)
    file = './images/Papatuanuku' + str(i) + '.jpg'
    print("taking photo")
    cap = cv2.VideoCapture(0) # video capture source camera (Here webcam of laptop) 
    ret,frame = cap.read() # return a single frame in variable `frame`
      
    cv2.imwrite(file,frame)
    print("image saved")
    cv2.destroyAllWindows()
    cap.release()
    print("detecting people")
    # A simple example to run on a single image and plt.imshow() it
    detector.detect_one(img_path=file,
                    input_size=1024, conf_thres=0.8,
                    visualize=False)

# A simple example to run on a single image and plt.imshow() it
detector.detect_one(img_path='./images/Lunch2_000001.jpg',
                    input_size=1024, conf_thres=0.3,
                    visualize=True)

