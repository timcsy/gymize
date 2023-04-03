import io
import numpy as np
from PIL import Image

import gymize.proto.space_pb2 as space

def image_to_box(image: space.Image):
    img = Image.open(io.BytesIO(image.data))
    image_array = np.array(img, dtype='uint8')
    return image_array

def save_image(box: np.ndarray, path: str):
    img = Image.fromarray(box)
    img.save(path)