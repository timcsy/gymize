import shutil
import os

import_path = 'definitions'
definitions_path = 'definitions'
generations_path = 'generations'
proto_folder = 'proto'
supported_languages = {
    'cpp': 'cpp',
    'java': 'java',
    'python': 'python',
    'ruby': 'ruby',
    'objc': 'objc',
    'csharp': 'csharp'
}

generations_path = os.path.abspath(generations_path)
if os.path.exists(generations_path):
    shutil.rmtree(os.path.abspath(generations_path))

language_outputs = ''
for lang, name in supported_languages.items():
    lang_output_path = os.path.join(generations_path, name, proto_folder)
    if not os.path.exists(lang_output_path):
        os.makedirs(lang_output_path)
    language_outputs += f'--{lang}_out={lang_output_path} '
os.system(f'protoc --proto_path={os.path.abspath(import_path)} {language_outputs}{os.path.join(os.path.abspath(definitions_path), "*.proto")}')