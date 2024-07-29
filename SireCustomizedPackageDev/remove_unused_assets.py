import os
import shutil

assets_dir = "assets"
trash_dir = ".trash"
readme_path = "README.md"


def get_referenced_files(readme_path):
    referenced_files = []
    with open(readme_path, "r", encoding="utf-8") as f:
        lines = f.readlines()
        for line in lines:
            line = line.strip()
            # find first ![ in line
            r = line.find("![")
            if r == -1:
                continue
            line = line[r:]
            if line.startswith("!["):
                file_name = line.split("]")[1].strip("()")
                referenced_files.append(file_name)
    return referenced_files


def main():
    referenced_files = get_referenced_files(readme_path)

    if not os.path.exists(trash_dir):
        os.makedirs(trash_dir)

    for file_name in os.listdir(assets_dir):
        file_path = os.path.join(assets_dir, file_name)
        if os.path.isfile(file_path) and not any(file_name in referenced_file for referenced_file in referenced_files):
            print(f"Moving {file_name} to .trash")
            shutil.move(file_path, os.path.join(trash_dir, file_name))


if __name__ == "__main__":
    main()
