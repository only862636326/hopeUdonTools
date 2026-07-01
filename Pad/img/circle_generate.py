import numpy as np
from PIL import Image, ImageDraw
import cv2
import os


r = 50
w = r * 2 
h = r * 2

img1 = np.zeros((h, w, 4), dtype=np.uint8)
img1.fill(255)
# 以四个角为圆心，半径为40，填充白色
cv2.circle(img1, (0, 0), r, (0, 0, 0, 0), -1)
cv2.circle(img1, (w - 1, 0), r, (0, 0, 0, 0), -1)
cv2.circle(img1, (w - 1, h - 1), r, (0, 0, 0, 0), -1)
cv2.circle(img1, (0, h - 1), r, (0, 0, 0, 0), -1)
# 以四个角为圆心，半径为40，填充白色

img2 = np.zeros((h, w, 4), dtype=np.uint8)
img2[:h//2, :w//2] = 255
cv2.circle(img2, (0, 0), r, (0, 0, 0, 0), -1)

img3 = np.zeros((h, w, 4), dtype=np.uint8)
img3[:h//2, w//2:] = 255
cv2.circle(img3, (w - 1, 0), r, (0, 0, 0, 0), -1)

img4 = np.zeros((h, w, 4), dtype=np.uint8)
img4[h//2:, w//2:] = 255
cv2.circle(img4, (w - 1, h - 1), r, (0, 0, 0, 0), -1)

img5 = np.zeros((h, w, 4), dtype=np.uint8)
img5[h//2:, :w//2] = 255
cv2.circle(img5, (0, h - 1), r, (0, 0, 0, 0), -1)

# 五个图拼一排
merged = np.hstack((img1, img2, img3, img4, img5))
cv2.imshow("merged", merged)
cv2.waitKey(0)

save_dir = os.path.dirname(__file__)
cv2.imwrite(os.path.join(save_dir, f"circle_arc_{r}_merged.png"), merged)









