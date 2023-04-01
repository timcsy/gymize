from proto.space_pb2 import Image
import io
from PIL import Image
import numpy as np

def image_to_box(image: Image):
    img = Image.open(io.BytesIO(image.data))
    image_array = np.array(img, dtype='uint8')
    return image_array

def save_image(box: np.ndarray, path: str):
    img = Image.fromarray(box)
    img.save(path)