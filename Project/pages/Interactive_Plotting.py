#Import the required Libraries
import streamlit as st
from PIL import Image
import webbrowser
import cv2
import json
from streamlit_autorefresh import st_autorefresh
from Get_Latest_Data import GetLatestData
from Subsectioning import SubSectioning

# Title
st.title('Interactive Tidal Effect Plot')

# Link to powerbi interactive plot
if st.button('Open Plot'):
    webbrowser.open_new_tab('https://app.powerbi.com/reportEmbed?reportId=b9216604-11dc-43e6-a820-abd017c950f0&autoAuth=true&ctid=866c7a4c-8a59-4bd3-ad9f-8512a581efc0')