import cv2 as cv
import matplotlib.pyplot as plot
import numpy as np

from envi import load_envi_image, save_envi_image

bgr = cv.imread('test.png')
assert len(bgr.shape) == 3
assert bgr.shape[-1] == 3
assert bgr.dtype == np.uint8

for interleave in ['bip', 'bil', 'bsq']:
    save_envi_image(bgr, f'test-{interleave}.raw', interleave=interleave)
