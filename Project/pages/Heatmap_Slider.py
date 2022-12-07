#Import the required Libraries
from tkinter import font
import webbrowser
import streamlit as st
import pandas as pd
from PIL import Image
import matplotlib.image as img
from datetime import datetime, time
from scipy.interpolate import Rbf  # radial basis functions
import matplotlib.pyplot as plt
import numpy as np
import json
import plotly.figure_factory as ff
import plotly.express as px
import datetime as dt
import requests
import plotly.graph_objects as go
from dateutil.relativedelta import relativedelta # to add days or years
from scipy.stats import gaussian_kde

# Title
st.title('Heat Map of Meeting Room')

# Interactive slider to set range of data time
heatmap_change = st.slider("Choose the time to see the heatmap: ", format="hh:mm:Ss", value=(time(0, 00, 00), time(23, 59, 59)))
st.write("Heatmap at: ", heatmap_change[0])

# Setting date, start and end time of the data range
date = '2022-10-27'
startTime = heatmap_change[0]
endTime = heatmap_change[1]

# Request to Azure storage to access merged json file 
get_all_url = 'https://raspeberryimagedata.azurewebsites.net/api/data/merged/all'

# Post data
r = requests.get(get_all_url)

start = date + 'T' + str(startTime)
end = date + 'T' + str(endTime)

# change r.text to json object
data = json.loads(r.text)

bounding_boxes = []
for i in data:
    for key, value in i.items():
        if key == 'Date/Time':
            if(i[key] >= start and i[key] <= end):     
                bounding_boxes.append(i['bbox'])

# Get the first element of each list
x = [i[0] for i in bounding_boxes]
y = [i[1] for i in bounding_boxes]

# Normalize the data attributes
x = (x - np.min(x)) / (np.max(x) - np.min(x))
y = (y - np.min(y)) / (np.max(y) - np.min(y))
# Scatter plot of x and y

# Create a mesh to plot in
z = [1]*len(x) 

kde = gaussian_kde([x,y])
xi, yi = np.mgrid[0:1:100j, 0:1:100j]
zi = kde(np.vstack([xi.flatten(), yi.flatten()]))
# Plot a density
plt.pcolormesh(xi, yi, zi.reshape(xi.shape), shading='gouraud')
plt.scatter(x, y)
plt.show()

# Labeling heatmap
plt.title('Heat Map of Meeting Room')
st.pyplot(plt)

st.write(data)