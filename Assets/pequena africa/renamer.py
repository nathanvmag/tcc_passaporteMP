import os

# Paths to your folders
folder1 = 'Targets_Redodondos'  # Folder with the desired names
folder2 = 'darcy'  # Folder to be renamed

# Get sorted list of PNG filenames
names1 = sorted([f for f in os.listdir(folder1) if f.lower().endswith('.png')])
names2 = sorted([f for f in os.listdir(folder2) if f.lower().endswith('.png')])

# Determine the number of files to rename
count = min(len(names1), len(names2))

print(f"Renaming {count} files...")

for original, target in zip(names1[:count], names2[:count]):
    target_path = os.path.join(folder2, target)
    new_name = os.path.splitext(original)[0] + '.png'
    new_path = os.path.join(folder2, new_name)

    os.rename(target_path, new_path)
    print(f"Renamed '{target}' to '{new_name}'")

print("Renaming complete.")