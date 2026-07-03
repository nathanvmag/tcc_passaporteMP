import os

def save_png_filenames_to_txt(folder_path, output_file):
    try:
        # List all .png files in the given folder
        png_files = [f for f in os.listdir(folder_path) if f.endswith('.png') and os.path.isfile(os.path.join(folder_path, f))]
        
        # Write the .png file names to the output text file
        with open(output_file, 'w') as file:
            for name in png_files:
                file.write(name + '\n')
        
        print(f".png file names saved to {output_file}")
    except Exception as e:
        print(f"An error occurred: {e}")

# Usage example
folder_path = "./"
output_file = "output.txt"
save_png_filenames_to_txt(folder_path, output_file)
