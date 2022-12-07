#Import the required Libraries
import streamlit as st
import pandas as pd
from PIL import Image
import json
import cv2

from streamlit_autorefresh import st_autorefresh

# Run the autorefresh about every 2000 milliseconds (2 seconds) and stop
# after it's been refreshed 100 times.

# Title
st.title('Desk Occupancy Dashboard')

# Opening the image
image = Image.open('pages/Datacom_Floor_1.png')
image = image.resize((1200, 1000))

# Displaying the image on streamlit app
st.image(image, caption='Datacom Auckland Office - Floor 1', width=600)
st.button('Matariki')

count = st_autorefresh(interval=10000, limit=100, key="fizzbuzzcounter")

