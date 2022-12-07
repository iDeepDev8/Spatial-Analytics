import requests
import json

# Function to get the latsest Data from Azure cloud
def GetLatestData():

    # URL where the data exposed from the cloud
    url = 'https://raspeberryimagedata.azurewebsites.net/api/data/latest/date'

    # Get data
    r = requests.get(url)

    # Change r.text to json object
    data = json.loads(r.text)

    # Empty array to store the Data
    Data = []

    # Iterating through the Data and getting the X,Y coordinates data of the Bboxes as that is the only data we need
    for i in data:
        for key, value in i.items():
            if key == 'bbox':
                array = i['bbox']
                Data.append([array[0], array[1]])

    print(Data)

    return Data

GetLatestData()