# Protobuf Generation

Assume you put your protobuf definitions in the definitions folder (only one level).

If you want to use the Go language, you have to install the tool mentioned in the documentation first.

## Generation
```
protoc --proto_path=./definitions --cpp_out=./generations/cpp --java_out=./generations/java --python_out=./generations/python --ruby_out=./generations/ruby --objc_out=./generations/objc --csharp_out=./generations/csharp ./definitions/*.proto
```

## Clean
```
rm -rf ./generations/**/[Ee]xample*
```

## Using Script
You can run the following Python script to generate the protobuf:
```
python gen.py
```
Make sure you have installed the Python protobuf package first, the you have protoc
```
pip install protobuf
```

## For Python
Add `__init__.py` to your Python folder
```
import os
import sys
sys.path.insert(0, os.path.abspath(os.path.dirname(__file__)))
```