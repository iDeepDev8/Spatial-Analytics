import streamlit as st
from PIL import Image
import cv2
import json
from streamlit_autorefresh import st_autorefresh
from Get_Latest_Data import GetLatestData
from Subsectioning import SubSectioning

# Read Image
img = cv2.imread('pages/Meeting_Room_Occupancy.png')
img = cv2.resize(img, (550,550))
img2 = 'pages/Meeting_Room_Occupancy.png'

# Get the latest data
Data = GetLatestData()

# ROIS for the desks on the Meeting Room Occupancy PNG image
ROIS = [[156, 108, 107, 185], [285, 108, 107, 185], [155, 315, 107, 185], [285, 315, 107, 185]]

# returns JSON object as
# a dictionary
Overlaps = SubSectioning(Data)

# Iterating through the Array that contains data as to whether the ROIs have a Overlap
for Overlap in Overlaps:

    Bool_Overlap = False

    # Itertating through the preset ROI coordinates for the Image
    for ROI in reversed(ROIS):

        if Overlap == True:
            Bool_Overlap = True

        if Overlap == False:
            Bool_Overlap = False

    roi = ROI
    ROIS.remove(ROI)

    # Check if there is an overlap that is detected if there is then create a red box else a green box
    if Bool_Overlap == True:
        x, y, w, h = roi
        cv2.rectangle(img, (x, y), (x + w, y + h), (0, 0, 220), -1)

    if Bool_Overlap == False:
        x, y, w, h = roi
        cv2.rectangle(img, (x, y), (x + w, y + h), (0, 255, 0), -1)

    cv2.imwrite('pages/Floor1-B.png', img)

    img2 = 'pages/Floor1-B.png'

# Opening the image
image = Image.open(img2)
image = image.resize((1200, 1000))

# Displaying the image on streamlit app
st.image(image, caption='Matariki', width=600)

# Refreshing this script every sec for 10,000 times
count = st_autorefresh(interval=1000, limit=10000, key="fizzbuzzcounter")
