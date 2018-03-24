import os

for file in os.listdir():
    count = 0
    if file.endswith(".cs"):
        with open(file, 'r') as f:
            lines = f.read().split("\n")
            for line in lines:
                if line != '':
                    count += 1
        print(file, count)
