## Billiard Dataset Generation

Codebase provides all nessasary features for quality billiard dataset generation, that is suitable for both **detection** or **segmentation** tasks. The generated annotations are in YOLO format.

https://github.com/user-attachments/assets/2102da6d-0546-42bc-bb2d-622053ca4628

### Billiard Ball Dataset Generation (detection task)

`Ball_Dataset_Capture` class generates dataset for detecting and classifying the billiard balls.
At each iteration:

1. Randomize billiard balls positions.
2. Randomly hide billiard balls based on probability of 40%.
3. Randomize enviroment by changing textures.
4. Randomize position of camera and make it look at table center.
5. Take a screenshot.
6. Generate precise YOLO style annotations based on mesh vertices.

### Billiard Pocket Dataset Generation (segmentation task)

`Table_Dataset_Capture` class generates dataset of pockets with regular images in one folder and segmentation masks in the other.
At each iteration:

1. Randomize billiard balls positions.
2. Randomly hide billiard balls based on probability of 80%.
3. Randomize enviroment by changing textures.
4. Randomize position of camera and make it look at table center.
5. **Switch to segmentation material.**
6. **Capture segmentation mask.**
7. **Restore original materials.**
8. Take a screenshot.
