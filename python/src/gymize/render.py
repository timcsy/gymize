import os
import random
import shutil
import subprocess

from gymize.proto.render_pb2 import VideoProto

from gymize.space import from_proto, save_image

def parse_rendering(video_proto: VideoProto, render_mode: str=None, video_path: str=None):
    if render_mode == 'rgb_array':
        frames = []
        for frame in video_proto.frames:
            img = from_proto(frame.image)
            frames.append(img)
        if len(frames) == 0:
            return None
        else:
            return frames[0]
    elif render_mode == 'rgb_array_list':
        frames = []
        for frame in video_proto.frames:
            img = from_proto(frame.image)
            frames.append(img)
        return frames
    elif render_mode == 'video':
        if video_path is None:
            tmp_dir = os.path.abspath('tmp_' + str(hash(random.random())))
        else:
            video_dir = os.path.dirname(video_path)
            tmp_dir = os.path.join(video_dir, 'tmp')
        if os.path.exists(tmp_dir):
            shutil.rmtree(tmp_dir)
        os.makedirs(tmp_dir)
        
        with open(os.path.join(tmp_dir, 'audio.wav'), 'wb') as fout:
            fout.write(video_proto.audio)

        with open(os.path.join(tmp_dir, 'img.txt'), 'w') as fout:
            i = 0
            for frame in video_proto.frames:
                img = from_proto(frame.image)
                img_path = os.path.join(tmp_dir, f'img_{i}.jpg')
                save_image(img, img_path)
                fout.write(f"file 'img_{i}.jpg'\nduration {frame.duration:.6f}\n")
                i += 1
        
        return generate_video(tmp_dir, video_path)
    else:
        return None

def generate_video(video_dir, output_path, width: int=None, height: int=None, remove_original: bool=True):
    width = width or -1
    height = height or -1

    try:
        result = subprocess.run([
            'ffmpeg',
            '-y',
            '-f', 'concat',
            '-i', 'img.txt',
            '-i', 'audio.wav',
            '-pix_fmt', 'yuv420p',
            '-vf', f'scale={width}:{height},setsar=1:1',
            'tmp.mp4'
        ], check=True, cwd=video_dir)
    except:
        result = -1
    
    output = None

    if result != -1:
        if output_path is None:
            video_path = os.path.join(video_dir, 'tmp.mp4')
            with open(video_path, 'rb') as fin:
                output = fin.read()
        else:
            source = os.path.join(video_dir, 'tmp.mp4')
            destination = os.path.abspath(output_path)
            shutil.move(source, destination)
            output = destination
    
    if remove_original:
        shutil.rmtree(video_dir)
    
    return output