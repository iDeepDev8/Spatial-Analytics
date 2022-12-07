#Import the required Libraries
import streamlit as st
import pandas as pd
from PIL import Image

# Title
st.title('Spatial Analytics')

# Opening the image
image = Image.open('./images/main.png')
image = image.resize((1200, 1000))

# Displaying the image on streamlit app
st.image(image, caption='Home Page', width=600)